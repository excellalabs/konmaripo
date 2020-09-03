using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Konmaripo.Web.Controllers;
using Konmaripo.Web.Models;
using Konmaripo.Web.Services;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Konmaripo.Web.Tests.Unit.Controllers
{
    public class OrgWideVisibilityControllerTests
    {
        public class Ctor
        {
            Mock<IGitHubService> _mockService = new Mock<IGitHubService>();
            [Fact]
            public void NoVisibilitySettings_ThrowsError()
            {
                Action act = () => new OrgWideVisibilityController(null, _mockService.Object);

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

                Action act = () => new OrgWideVisibilityController(settings, _mockService.Object);

                act.Should().Throw<ArgumentNullException>().And.ParamName.Should()
                    .Be("AllOrgMembersGroupName");
            }

            [Fact]
            public void NullGitHubService_ThrowsException()
            {
                var dummyOptions = new OptionsWrapper<OrgWideVisibilitySettings>(new OrgWideVisibilitySettings {AllOrgMembersGroupName = "dummy"});

                Action act = () =>
                {
                    new OrgWideVisibilityController(dummyOptions, null);
                };

                act.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("gitHubService");
            }

        }
    }
}
