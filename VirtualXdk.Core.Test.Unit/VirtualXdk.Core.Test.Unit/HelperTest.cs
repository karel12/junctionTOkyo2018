using Assets.Utils;
using NUnit.Framework;

namespace VirtualXdk.Core.Test.Unit
{
    [TestFixture]
    public class HelperTest
    {
        [Test]
        public void WhenSlicingFirstTwoBytes_ShouldReturnCorrectResult()
        {
            var sevenItems = new byte[] { 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70 };
            sevenItems.Slice(2, result => Assert.That(result.Length == 2 && result[0] == 0x10 && result[1] == 0x20));
        }

        [Test]
        public void WhenSlicingSecondTwoBytes_ShouldReturnCorrectResult()
        {
            var sevenItems = new byte[] { 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70 };
            sevenItems.Slice(2).Slice(2, result => Assert.That(result.Length == 2 && result[0] == 0x30 && result[1] == 0x40));
        }

        [Test]
        public void WhenSlicingFourBytes_ReturnValueShouldContainRest()
        {
            var sevenItems = new byte[] { 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70 };
            var result = sevenItems.Slice(2);
            Assert.That(result[0] == 0x30);
            Assert.That(result.Length == 5);
        }
    }
}
