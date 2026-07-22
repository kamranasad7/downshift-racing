using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Downshift;

public class FullLoopPlayTests : PlayModeCleanup
{
    [TearDown]
    public void ResetGameSession()
    {
        GameSession.PathOverride = null;
        GameSession.Reset();
    }

    [UnityTest]
    public IEnumerator MenuPlayRunCrashResultsMenuLoop()
    {
        GameSession.PathOverride = Path.Combine(Path.GetTempPath(), "downshift_loop_test.json");
        if (File.Exists(GameSession.PathOverride)) File.Delete(GameSession.PathOverride);
        GameSession.Reset();

        SceneManager.LoadScene("Menu");
        yield return null;
        yield return null;

        var menu = Object.FindFirstObjectByType<MenuController>();
        Assert.IsNotNull(menu, "menu scene should contain MenuController");
        Assert.IsFalse(string.IsNullOrEmpty(menu.coinText.text));

        menu.Play();
        yield return null;
        yield return null;

        var run = Object.FindFirstObjectByType<RunManager>();
        var vc = Object.FindFirstObjectByType<VehicleController>();
        Assert.IsNotNull(run, "run scene should load with RunManager");
        Assert.IsNotNull(vc);
        run.resultsDelay = 0.3f;

        float startX = vc.transform.position.x;
        float deadline = Time.time + 5f;
        while (vc.transform.position.x < startX + 0.05f && Time.time < deadline)
            yield return new WaitForFixedUpdate();

        Wallet.Coins = 5;
        run.NotifyCrash();
        deadline = Time.time + 4f;
        while (run.State != RunState.Results && Time.time < deadline) yield return null;
        Assert.AreEqual(RunState.Results, run.State);

        var saved = SaveStore.Load(GameSession.PathOverride);
        Assert.AreEqual(5, saved.coins, "results should bank wallet coins to save");
        Assert.Greater(saved.bestDistanceM, 0f);

        var hud = Object.FindFirstObjectByType<HudController>();
        Assert.IsTrue(hud.resultsPanel.activeSelf);

        run.ToMenu();
        yield return null;
        yield return null;

        var menuAgain = Object.FindFirstObjectByType<MenuController>();
        Assert.IsNotNull(menuAgain, "ToMenu should land back in menu scene");
        StringAssert.Contains("5", menuAgain.coinText.text);
        yield return null;
    }
}
