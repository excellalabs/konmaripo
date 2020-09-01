using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Konmaripo.Web.Controllers;
using Konmaripo.Web.Models;
using Microsoft.Extensions.Options;
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

            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("   ")]
            public void NoGroupName_ThrowsArgumentException(string settingToTry)
            {
                var testOptions = new OrgWideVisibilitySettings { AllOrgMembersGroupName = settingToTry};
                var settings = new OptionsWrapper<OrgWideVisibilitySettings>(testOptions);

                Action act = () => new OrgWideVisibilityController(settings);

                act.Should().Throw<ArgumentNullException>().And.ParamName.Should()
                    .Be("AllOrgMembersGroupName");
            }
        }
    }
}
