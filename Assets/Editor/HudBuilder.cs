using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Downshift;

namespace Downshift.EditorTools
{
    public static class HudBuilder
    {
        public static HudController Build(VehicleController vehicle, RunManager run)
        {
            var canvasGo = new GameObject("HUD");
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

            var hud = canvasGo.AddComponent<HudController>();
            hud.vehicle = vehicle;
            hud.runManager = run;

            hud.brakeFill = Gauge(canvasGo, "BrakeGauge", new Vector2(-760, 440));
            hud.engineFill = Gauge(canvasGo, "EngineGauge", new Vector2(-760, 390));
            hud.gearText = Label(canvasGo, "Gear", new Vector2(820, 440), 48);
            hud.speedText = Label(canvasGo, "Speed", new Vector2(0, 460), 40);
            hud.rpmText = Label(canvasGo, "Rpm", new Vector2(0, 405), 34);
            hud.distanceText = Label(canvasGo, "Distance", new Vector2(0, 350), 32);
            hud.coinText = Label(canvasGo, "Coins", new Vector2(820, 380), 36);

            var brake = Button(canvasGo, "BrakeBtn", new Vector2(-700, -380), new Vector2(360, 220), "BRAKE");
            hud.brakeButton = brake.gameObject.AddComponent<HoldButton>();

            var up = Button(canvasGo, "GearUp", new Vector2(760, -280), new Vector2(280, 160), "GEAR +");
            UnityEditor.Events.UnityEventTools.AddPersistentListener(up.onClick, vehicle.GearUp);
            var down = Button(canvasGo, "GearDown", new Vector2(760, -460), new Vector2(280, 160), "GEAR -");
            UnityEditor.Events.UnityEventTools.AddPersistentListener(down.onClick, vehicle.GearDown);

            var pauseBtn = Button(canvasGo, "PauseBtn", new Vector2(940, 440), new Vector2(90, 90), "II");
            UnityEditor.Events.UnityEventTools.AddPersistentListener(pauseBtn.onClick, run.TogglePause);

            var panel = new GameObject("ResultsPanel");
            panel.transform.SetParent(canvasGo.transform, false);
            var pi = panel.AddComponent<Image>();
            pi.color = new Color(0, 0, 0, 0.75f);
            var prt = panel.GetComponent<RectTransform>();
            prt.sizeDelta = new Vector2(700, 560);
            hud.resultsPanel = panel;
            hud.resultsText = Label(panel, "ResultText", new Vector2(0, 120), 56);
            hud.newBestText = Label(panel, "NewBestText", new Vector2(0, 200), 44);
            hud.newBestText.text = "NEW BEST!";
            hud.newBestText.color = new Color(1f, 0.84f, 0f);
            var restart = Button(panel, "RestartBtn", new Vector2(0, -40), new Vector2(320, 120), "RESTART");
            UnityEditor.Events.UnityEventTools.AddPersistentListener(restart.onClick, run.Restart);
            var menuBtn = Button(panel, "MenuBtn", new Vector2(0, -180), new Vector2(320, 120), "MENU");
            UnityEditor.Events.UnityEventTools.AddPersistentListener(menuBtn.onClick, run.ToMenu);

            var pausePanel = new GameObject("PausePanel");
            pausePanel.transform.SetParent(canvasGo.transform, false);
            var ppi = pausePanel.AddComponent<Image>();
            ppi.color = new Color(0, 0, 0, 0.75f);
            var ppRt = pausePanel.GetComponent<RectTransform>();
            ppRt.sizeDelta = new Vector2(700, 700);
            hud.pausePanel = pausePanel;

            Label(pausePanel, "PAUSED", new Vector2(0, 220), 56);
            var resume = Button(pausePanel, "ResumeBtn", new Vector2(0, 80), new Vector2(320, 120), "RESUME");
            UnityEditor.Events.UnityEventTools.AddPersistentListener(resume.onClick, run.TogglePause);
            var pauseRestart = Button(pausePanel, "PauseRestartBtn", new Vector2(0, -60), new Vector2(320, 120), "RESTART");
            UnityEditor.Events.UnityEventTools.AddPersistentListener(pauseRestart.onClick, run.Restart);
            var pauseMenu = Button(pausePanel, "PauseMenuBtn", new Vector2(0, -200), new Vector2(320, 120), "MENU");
            UnityEditor.Events.UnityEventTools.AddPersistentListener(pauseMenu.onClick, run.ToMenu);

            return hud;
        }

        static Image Gauge(GameObject parent, string name, Vector2 pos)
        {
            var bg = new GameObject(name);
            bg.transform.SetParent(parent.transform, false);
            var bgi = bg.AddComponent<Image>();
            bgi.color = new Color(0, 0, 0, 0.5f);
            var rt = bg.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(320, 36);
            var fill = new GameObject("Fill");
            fill.transform.SetParent(bg.transform, false);
            var fi = fill.AddComponent<Image>();
            fi.sprite = SpriteFactory.Ensure("square", false);
            fi.type = Image.Type.Filled;
            fi.fillMethod = Image.FillMethod.Horizontal;
            var frt = fill.GetComponent<RectTransform>();
            frt.anchorMin = Vector2.zero;
            frt.anchorMax = Vector2.one;
            frt.offsetMin = new Vector2(4, 4);
            frt.offsetMax = new Vector2(-4, -4);
            return fi;
        }

        static TMP_Text Label(GameObject parent, string name, Vector2 pos, float size)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            var t = go.AddComponent<TextMeshProUGUI>();
            t.fontSize = size;
            t.alignment = TextAlignmentOptions.Center;
            t.text = name;
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
            var text = Label(go, label, Vector2.zero, 40);
            text.color = Color.white;
            return btn;
        }
    }
}
