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
        float _startX;
        float _failTime;

        public RunState State => _machine.State;
        public float DistanceM => Mathf.Max(0f, _distance - _startX);
        public bool IsNewBest { get; private set; }
        public event System.Action<RunState> StateChanged;

        void Start()
        {
            Wallet.Coins = 0;
            _startX = vehicle.transform.position.x;
            vehicle.Blown += OnBlown;
        }

        void Update()
        {
            if (_machine.State == RunState.Descending)
                _distance = Mathf.Max(_distance, vehicle.transform.position.x);

            if ((_machine.State == RunState.Crashed || _machine.State == RunState.BlownUp)
                && Time.time - _failTime > resultsDelay && _machine.ToResults())
            {
                BankRun();
                StateChanged?.Invoke(_machine.State);
            }

            if (_machine.State == RunState.Results && Input.GetKeyDown(KeyCode.R))
                SceneManager.LoadScene("Run");
        }

        public void NotifyCrash()
        {
            if (_machine.TryCrash())
            {
                vehicle.Shutdown();
                _failTime = Time.time;
                StateChanged?.Invoke(_machine.State);
            }
        }

        void OnBlown()
        {
            if (_machine.TryBlowUp())
            {
                vehicle.Shutdown();
                _failTime = Time.time;
                StateChanged?.Invoke(_machine.State);
            }
        }

        public void Restart() => SceneManager.LoadScene("Run");

        public void ToMenu() => SceneManager.LoadScene("Menu");

        void BankRun()
        {
            IsNewBest = DistanceM > GameSession.Save.bestDistanceM;
            GameSession.Save.coins += Wallet.Coins;
            if (IsNewBest) GameSession.Save.bestDistanceM = DistanceM;
            GameSession.Persist();
        }
    }
}
