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
        public TMP_Text distanceText;
        public TMP_Text coinText;
        public GameObject resultsPanel;
        public TMP_Text resultsText;
        public HoldButton brakeButton;

        void Start()
        {
            resultsPanel.SetActive(false);
            runManager.StateChanged += OnState;
        }

        void OnState(RunState state)
        {
            if (state == RunState.Results)
            {
                resultsPanel.SetActive(true);
                resultsText.text = $"{Mathf.FloorToInt(runManager.DistanceM)} m";
            }
        }

        void Update()
        {
            brakeFill.fillAmount = vehicle.Brakes.Temp / vehicle.stats.brakeMaxTemp;
            brakeFill.color = vehicle.Brakes.Faded ? Color.red :
                Color.Lerp(Color.green, Color.red, brakeFill.fillAmount);
            engineFill.fillAmount = vehicle.EngineTemp / vehicle.stats.engineMaxTemp;
            engineFill.color = Color.Lerp(Color.green, Color.red, engineFill.fillAmount);
            gearText.text = $"G{vehicle.CurrentGear + 1}";
            speedText.text = $"{Mathf.FloorToInt(vehicle.SpeedMs * 3.6f)} km/h";
            distanceText.text = $"{Mathf.FloorToInt(runManager.DistanceM)} m";
            coinText.text = $"{Wallet.Coins} c";
        }
    }
}
