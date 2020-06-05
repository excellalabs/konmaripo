using System;
using FluentAssertions;
using Xunit;

namespace Konmaripo.Web.Tests.Unit
{
    public class MyDoodadTests
    {
        [Fact]
        public void OnePlusOneShouldBeTwo()
        {
            var result = 1 + 1;

            result.Should().Be(2);
        }
    }
}
