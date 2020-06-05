using System;
using Xunit;

namespace Konmaripo.Web.Tests.Unit
{
    public class MyDoodadTests
    {
        [Fact]
        public void OnePlusOneShouldBeTwo()
        {
            var result = 1 + 1;

            Assert.Equal(2, result);
        }
    }
}
