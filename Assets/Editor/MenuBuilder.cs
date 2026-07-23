using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Downshift;

namespace Downshift.EditorTools
{
    public static class MenuBuilder
    {
        static readonly string[] TrackLabels = { "BRAKES", "RADIATOR", "GEARBOX", "TIRES" };

        [MenuItem("Downshift/Build/Menu Scene")]
        public static void Build()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            var canvasGo = new GameObject("Canvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGo.AddComponent<GraphicRaycaster>();

            if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            var menu = canvasGo.AddComponent<MenuController>();

            Label(canvasGo, "Title", new Vector2(0, 480), 72, "DOWNSHIFT");
            menu.coinText = Label(canvasGo, "CoinText", new Vector2(820, 480), 40, "0 c");
            menu.bestText = Label(canvasGo, "BestText", new Vector2(0, 400), 36, "BEST 0 m");

            float[] rowY = { 180, 40, -100, -240 };
            for (int i = 0; i < 4; i++)
            {
                var y = rowY[i];
                Label(canvasGo, $"TrackLabel{i}", new Vector2(-650, y), 36, TrackLabels[i]);
                menu.tierTexts[i] = Label(canvasGo, $"TierText{i}", new Vector2(-320, y), 32, "T0/8");
                menu.costTexts[i] = Label(canvasGo, $"CostText{i}", new Vector2(-60, y), 32, "0 c");
                var buy = Button(canvasGo, $"BuyBtn{i}", new Vector2(260, y), new Vector2(240, 90), "BUY");
                menu.buyButtons[i] = buy;
                UnityEditor.Events.UnityEventTools.AddIntPersistentListener(buy.onClick, menu.BuyTrack, i);
            }

            var play = Button(canvasGo, "PlayBtn", new Vector2(0, -420), new Vector2(400, 140), "PLAY");
            UnityEditor.Events.UnityEventTools.AddPersistentListener(play.onClick, menu.Play);

            var sfxBtn = Button(canvasGo, "SfxToggleBtn", new Vector2(-820, -480), new Vector2(280, 80), "SFX ON");
            menu.sfxToggleText = sfxBtn.GetComponentInChildren<TMP_Text>();
            UnityEditor.Events.UnityEventTools.AddPersistentListener(sfxBtn.onClick, menu.ToggleSfx);

            var musicBtn = Button(canvasGo, "MusicToggleBtn", new Vector2(-820, -380), new Vector2(280, 80), "MUSIC ON");
            menu.musicToggleText = musicBtn.GetComponentInChildren<TMP_Text>();
            UnityEditor.Events.UnityEventTools.AddPersistentListener(musicBtn.onClick, menu.ToggleMusic);

            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
                AssetDatabase.CreateFolder("Assets", "Scenes");
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/Menu.unity");

            var scenes = new[]
            {
                new EditorBuildSettingsScene("Assets/Scenes/Menu.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/Run.unity", true)
            };
            EditorBuildSettings.scenes = scenes;
        }

        static TMP_Text Label(GameObject parent, string name, Vector2 pos, float size, string text)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            var t = go.AddComponent<TextMeshProUGUI>();
            t.fontSize = size;
            t.alignment = TextAlignmentOptions.Center;
            t.text = text;
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(500, size + 20);
            return t;
        }

        static Button Button(GameObject parent, string name, Vector2 pos, Vector2 size, string label)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            var img = go.AddComponent<Image>();
            img.color = new Color(1f, 1f, 1f, 0.25f);
            var btn = go.AddComponent<Button>();
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
            var text = Label(go, label, Vector2.zero, 32, label);
            text.color = Color.white;
            return btn;
        }
    }
}
