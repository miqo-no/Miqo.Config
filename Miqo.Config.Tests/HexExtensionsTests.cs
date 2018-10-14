using Xunit;

namespace Miqo.Config.Tests
{
    public class HexExtensionsTests
    {
        [Fact]
        public void DecodeHex_ReturnsByteArray_WhenHexString()
        {
            var expected = new byte[]
            {
                0x73, 0x60, 0xAF, 0xA3, 0xA3, 0x27, 0xD4, 0x04,
                0x08, 0xD8, 0x30, 0xA2, 0x1D, 0x6D, 0x30, 0x3A,
                0xA7, 0x4B, 0x66, 0x94, 0xEA, 0x4F, 0x58, 0xB5,
                0x98, 0xEC, 0x18, 0xAF, 0xE5, 0x71, 0x52, 0x52
            };
            var actual = "7360afa3a327d40408d830a21d6d303aa74b6694ea4f58b598ec18afe5715252".HexToBytes();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void DecodeHex_ReturnsEmptyByteArray_WhenNotHexString()
        {
            var expected = new byte[0];
            var actual = "';[;';[".HexToBytes();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void DecodeHex_ReturnsEmptyByteArray_WhenEmptyString()
        {
            var expected = new byte[0];
            var actual = "".HexToBytes();

            Assert.Equal(expected, actual);

            actual = string.Empty.HexToBytes();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void EncodeHex_ReturnsHexString_WhenByteArray()
        {
            var expected = "7360afa3a327d40408d830a21d6d303aa74b6694ea4f58b598ec18afe5715252";
            var actual = new byte[]
            {
                0x73, 0x60, 0xAF, 0xA3, 0xA3, 0x27, 0xD4, 0x04,
                0x08, 0xD8, 0x30, 0xA2, 0x1D, 0x6D, 0x30, 0x3A,
                0xA7, 0x4B, 0x66, 0x94, 0xEA, 0x4F, 0x58, 0xB5,
                0x98, 0xEC, 0x18, 0xAF, 0xE5, 0x71, 0x52, 0x52
            }.ToHex();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void EncodeHex_ReturnsEmptyString_WhenEmptyByteArray()
        {
            var expected = string.Empty;
            var actual = new byte[0].ToHex();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void EncodeHex_ReturnsString_WhenUnevenByteCount()
        {
            var expected = "73600a";
            var actual = new byte[] { 0x73, 0x60, 0xA }.ToHex();

            Assert.Equal(expected, actual);
        }
    }
}
