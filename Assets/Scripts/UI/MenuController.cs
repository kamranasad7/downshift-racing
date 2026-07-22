using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Downshift
{
    public class MenuController : MonoBehaviour
    {
        public TMP_Text coinText;
        public TMP_Text bestText;
        public TMP_Text[] tierTexts = new TMP_Text[4];
        public TMP_Text[] costTexts = new TMP_Text[4];
        public UnityEngine.UI.Button[] buyButtons = new UnityEngine.UI.Button[4];

        void Start()
        {
            Refresh();
        }

        public void BuyTrack(int track)
        {
            Upgrades.Buy(GameSession.Save, (UpgradeTrack)track, GameSession.Economy);
            GameSession.Persist();
            Refresh();
        }

        public void Play()
        {
            SceneManager.LoadScene("Run");
        }

        void Refresh()
        {
            var save = GameSession.Save;
            var econ = GameSession.Economy;
            coinText.text = $"{save.coins} c";
            bestText.text = $"BEST {Mathf.FloorToInt(save.bestDistanceM)} m";

            for (int i = 0; i < 4; i++)
            {
                var tier = save.upgradeTiers[i];
                var cost = Upgrades.CostFor(tier, econ);
                tierTexts[i].text = $"T{tier}/{econ.maxTier}";
                costTexts[i].text = cost < 0 ? "MAX" : $"{cost} c";
                buyButtons[i].interactable = Upgrades.CanBuy(save, (UpgradeTrack)i, econ);
            }
        }
    }
}
