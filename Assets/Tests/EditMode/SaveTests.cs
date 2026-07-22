using System.IO;
using NUnit.Framework;
using Downshift;

public class SaveTests
{
    string TempPath() => Path.Combine(Path.GetTempPath(), "downshift_test_save.json");

    [SetUp]
    public void Clean()
    {
        if (File.Exists(TempPath())) File.Delete(TempPath());
    }

    [Test]
    public void LoadMissingFileGivesFreshModel()
    {
        var m = SaveStore.Load(TempPath());
        Assert.AreEqual(0, m.coins);
        Assert.AreEqual(0f, m.bestDistanceM);
        Assert.AreEqual(4, m.upgradeTiers.Length);
    }

    [Test]
    public void RoundTripPersistsFields()
    {
        var m = new SaveModel { coins = 123, bestDistanceM = 456.7f };
        m.upgradeTiers[2] = 3;
        SaveStore.Save(m, TempPath());
        var loaded = SaveStore.Load(TempPath());
        Assert.AreEqual(123, loaded.coins);
        Assert.AreEqual(456.7f, loaded.bestDistanceM, 0.001f);
        Assert.AreEqual(3, loaded.upgradeTiers[2]);
    }

    [Test]
    public void CorruptFileGivesFreshModel()
    {
        File.WriteAllText(TempPath(), "{not json!!");
        var m = SaveStore.Load(TempPath());
        Assert.AreEqual(0, m.coins);
    }
}
