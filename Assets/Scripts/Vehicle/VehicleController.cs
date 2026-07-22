using UnityEngine;

namespace Downshift
{
    public class VehicleController : MonoBehaviour
    {
        public VehicleStats stats;

        Rigidbody2D _rb;
        WheelJoint2D[] _joints;
        Rigidbody2D[] _wheels;
        Collider2D[] _wheelCols;
        BrakeState _brakes;
        float _engineTemp;
        int _gear;
        bool _blown;
        bool _shutdown;
        VehicleStats _statsClone;

        public int CurrentGear => _gear;
        public float EngineRpm { get; private set; }
        public float EngineTemp => _engineTemp;
        public BrakeState Brakes => _brakes;
        public float SpeedMs => _rb != null ? _rb.linearVelocity.magnitude : 0f;
        public bool EngineBlown => _blown;
        public event System.Action Blown;

        public static int NextGear(int gear, int delta, int gearCount)
        {
            return Drivetrain.ClampGear(gear + delta, gearCount);
        }

        void Awake()
        {
            if (stats != null && GameSession.Economy != null)
            {
                stats = Upgrades.ApplyTo(stats, GameSession.Save, GameSession.Economy);
                _statsClone = stats;
            }

            _rb = GetComponent<Rigidbody2D>();
            _joints = GetComponents<WheelJoint2D>();
            _wheels = new Rigidbody2D[_joints.Length];
            _wheelCols = new Collider2D[_joints.Length];
            for (int i = 0; i < _joints.Length; i++)
            {
                _wheels[i] = _joints[i].connectedBody;
                _wheelCols[i] = _wheels[i].GetComponent<Collider2D>();
            }
            if (stats != null)
            {
                _rb.gravityScale = stats.gravityScale;
                foreach (var w in _wheels) w.gravityScale = stats.gravityScale;
            }
        }

        public void GearUp() => Shift(+1);
        public void GearDown() => Shift(-1);

        void Shift(int delta)
        {
            if (IsAirborne())
                _rb.AddTorque(delta * -stats.airTiltTorque);
            else
                _gear = NextGear(_gear, delta, stats.gearRatios.Length);
        }

        bool IsAirborne()
        {
            var filter = new ContactFilter2D();
            filter.SetLayerMask(Physics2D.AllLayers);
            filter.useTriggers = false;
            foreach (var c in _wheelCols)
                if (c.IsTouching(filter)) return false;
            return true;
        }

        public void Shutdown() => _shutdown = true;

        void OnDestroy()
        {
            if (_statsClone != null) Destroy(_statsClone);
        }

        void FixedUpdate()
        {
            if (_shutdown || _blown) { ReleaseMotors(); return; }
            float dt = Time.fixedDeltaTime;

            float wheelDeg = SpeedMs / stats.wheelRadius * Mathf.Rad2Deg;
            float ratio = stats.gearRatios[_gear];
            float rawRpm = Drivetrain.EngineRpm(wheelDeg, ratio, stats.finalDrive);
            EngineRpm = Mathf.Max(stats.idleRpm, rawRpm);

            _engineTemp = EngineHeatModel.Step(_engineTemp, EngineRpm, stats, dt);
            _brakes = BrakeModel.Step(_brakes, VehicleInput.BrakeHeld, SpeedMs, stats, dt);

            float brakeTorque = VehicleInput.BrakeHeld
                ? stats.brakeTorque * BrakeModel.Effectiveness(_brakes, stats)
                : 0f;
            float engineBrake = Drivetrain.EngineBrakeWheelTorque(
                rawRpm, stats.redlineRpm, ratio, stats.finalDrive, stats.engineBrakeTorque);

            float resist = brakeTorque + engineBrake;
            foreach (var j in _joints)
            {
                var motor = j.motor;
                motor.motorSpeed = 0f;
                motor.maxMotorTorque = resist;
                j.motor = motor;
                j.useMotor = resist > 0.01f;
            }

            if (SpeedMs < 1.5f)
                _rb.AddForce(Vector2.right * stats.creepForce);

            if (!_blown && EngineHeatModel.IsBlown(_engineTemp, stats))
            {
                _blown = true;
                Blown?.Invoke();
            }
        }

        void ReleaseMotors()
        {
            foreach (var j in _joints) j.useMotor = false;
        }

        public void CoolPartial(float engineAmount, float brakeAmount)
        {
            _engineTemp = Mathf.Max(0f, _engineTemp - engineAmount);
            var b = _brakes;
            b.Temp = Mathf.Max(0f, b.Temp - brakeAmount);
            if (b.Faded && b.Temp <= stats.brakeReengageTemp) b.Faded = false;
            _brakes = b;
        }

        public void CoolFull()
        {
            _engineTemp = 0f;
            _brakes = new BrakeState();
        }
    }
}
