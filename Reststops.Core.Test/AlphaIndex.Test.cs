using System;
using Xunit;

namespace Reststops.Core.Test
{
    public class AlphaIndexTest
    {
        [Fact]
        public void GetValidAlphaTest()
        {
            // act
            string result = AlphaIndex.Get(0);

            // assert
            Assert.Equal("a", result);
        }

        [Fact]
        public void GetValidEndOfRangeAlphaTest()
        {
            // act
            string result = AlphaIndex.Get(25);

            // assert
            Assert.Equal("z", result);
        }

        [Fact]
        public void GetInvalidNegativeIndexTest()
        {
            // act
            string result = AlphaIndex.Get(-5);

            // assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetInvalidHighIndexTest()
        {
            // act
            string result = AlphaIndex.Get(int.MaxValue);

            // assert
            Assert.Equal(string.Empty, result);
        }
    }
}
