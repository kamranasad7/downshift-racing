using NUnit.Framework;
using Downshift;

public class RunMachineTests
{
    [Test]
    public void StartsDescending()
    {
        Assert.AreEqual(RunState.Descending, new RunMachine().State);
    }

    [Test]
    public void CrashOnlyFromDescending()
    {
        var m = new RunMachine();
        Assert.IsTrue(m.TryCrash());
        Assert.AreEqual(RunState.Crashed, m.State);
        Assert.IsFalse(m.TryBlowUp());
        Assert.AreEqual(RunState.Crashed, m.State);
    }

    [Test]
    public void BlowUpOnlyFromDescending()
    {
        var m = new RunMachine();
        Assert.IsTrue(m.TryBlowUp());
        Assert.AreEqual(RunState.BlownUp, m.State);
        Assert.IsFalse(m.TryCrash());
    }

    [Test]
    public void ResultsFromEitherFailState()
    {
        var m = new RunMachine();
        m.TryCrash();
        Assert.IsTrue(m.ToResults());
        Assert.AreEqual(RunState.Results, m.State);
        m.Reset();
        Assert.AreEqual(RunState.Descending, m.State);
    }

    [Test]
    public void ResultsNotFromDescending()
    {
        Assert.IsFalse(new RunMachine().ToResults());
    }
}
