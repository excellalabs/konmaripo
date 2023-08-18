using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Konmaripo.Web.Tests.Unit
{
    public class QuickLinqTest
    {
        [Fact]
        public void LinqSequenceStuff()
        {
            var fullList = new List<int> { 1, 2, 3, 4, 5 };
            var firstExcept = new List<int> { 1, 2};
            var secondExcept = new List<int> { 5 };

            var result = fullList.Except(firstExcept).Except(secondExcept);

            result.Count().Should().Be(2);
            result.Should().NotContain(1);
            result.Should().NotContain(2);
            result.Should().Contain(3);
            result.Should().Contain(4);
            result.Should().NotContain(5);

            result.Should().BeEquivalentTo(new List<int>() { 4, 3 });

        }
    }
}
