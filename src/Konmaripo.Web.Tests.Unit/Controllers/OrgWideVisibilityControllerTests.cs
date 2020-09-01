using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Konmaripo.Web.Controllers;
using Xunit;

namespace Konmaripo.Web.Tests.Unit.Controllers
{
    public class OrgWideVisibilityControllerTests
    {
        public class Ctor
        {
            [Fact]
            public void NoVisibilitySettings_ThrowsError()
            {
                Action act = () => new OrgWideVisibilityController(null);

                act.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("visibilitySettings");
            }
        }
    }
}
