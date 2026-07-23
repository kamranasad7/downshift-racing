using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Downshift
{
    public class HudController : MonoBehaviour
    {
        public VehicleController vehicle;
        public RunManager runManager;
        public Image brakeFill;
        public Image engineFill;
        public TMP_Text gearText;
        public TMP_Text speedText;
        public TMP_Text rpmText;
        public TMP_Text distanceText;
        public TMP_Text coinText;
        public GameObject resultsPanel;
        public TMP_Text resultsText;
        public TMP_Text newBestText;
        public GameObject pausePanel;
        public HoldButton brakeButton;

        void Start()
        {
            resultsPanel.SetActive(false);
            newBestText.gameObject.SetActive(false);
            pausePanel.SetActive(false);
            runManager.StateChanged += OnState;
        }

        void OnState(RunState state)
        {
            if (state == RunState.Results)
            {
                resultsPanel.SetActive(true);
                resultsText.text = $"{Mathf.FloorToInt(runManager.DistanceM)} m";
                newBestText.gameObject.SetActive(runManager.IsNewBest);
            }
        }

        void Update()
        {
            pausePanel.SetActive(runManager.IsPaused);

            brakeFill.fillAmount = vehicle.Brakes.Temp / vehicle.stats.brakeMaxTemp;
            brakeFill.color = vehicle.Brakes.Faded ? Color.red :
                Color.Lerp(Color.green, Color.red, brakeFill.fillAmount);
            engineFill.fillAmount = vehicle.EngineTemp / vehicle.stats.engineMaxTemp;
            engineFill.color = Color.Lerp(Color.green, Color.red, engineFill.fillAmount);
            gearText.text = $"G{vehicle.CurrentGear + 1}";
            speedText.text = $"{Mathf.FloorToInt(vehicle.SpeedMs * 3.6f)} km/h";
            rpmText.text = $"{Mathf.FloorToInt(vehicle.EngineRpm)} rpm";
            rpmText.color = Color.Lerp(Color.white, Color.red,
                Mathf.Clamp01(vehicle.EngineRpm / vehicle.stats.redlineRpm));
            distanceText.text = $"{Mathf.FloorToInt(runManager.DistanceM)} m";
            coinText.text = $"{Wallet.Coins} c";
        }
    }
}
