using UnityEngine;
using UnityEngine.SceneManagement;

namespace Downshift
{
    public class RunManager : MonoBehaviour
    {
        public VehicleController vehicle;
        public float resultsDelay = 1.5f;

        readonly RunMachine _machine = new RunMachine();
        float _distance;
        float _failTime;

        public RunState State => _machine.State;
        public float DistanceM => _distance;
        public event System.Action<RunState> StateChanged;

        void Start()
        {
            vehicle.Blown += OnBlown;
        }

        void Update()
        {
            if (_machine.State == RunState.Descending)
                _distance = Mathf.Max(_distance, vehicle.transform.position.x);

            if ((_machine.State == RunState.Crashed || _machine.State == RunState.BlownUp)
                && Time.time - _failTime > resultsDelay && _machine.ToResults())
                StateChanged?.Invoke(_machine.State);

            if (_machine.State == RunState.Results && Input.GetKeyDown(KeyCode.R))
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void NotifyCrash()
        {
            if (_machine.TryCrash())
            {
                _failTime = Time.time;
                StateChanged?.Invoke(_machine.State);
            }
        }

        void OnBlown()
        {
            if (_machine.TryBlowUp())
            {
                _failTime = Time.time;
                StateChanged?.Invoke(_machine.State);
            }
        }

        public void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
