using NUnit.Framework;
using Downshift;

public class VehicleControllerTests
{
    [Test]
    public void ShiftLogicClampsAtEnds()
    {
        Assert.AreEqual(1, VehicleController.NextGear(0, +1, 5));
        Assert.AreEqual(0, VehicleController.NextGear(0, -1, 5));
        Assert.AreEqual(4, VehicleController.NextGear(4, +1, 5));
    }
}
