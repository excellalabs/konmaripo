using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Konmaripo.Web.Controllers;
using Konmaripo.Web.Models;
using Konmaripo.Web.Services;
using Microsoft.Extensions.Options;
using Moq;
using Serilog;
using Xunit;

namespace Konmaripo.Web.Tests.Unit.Controllers
{
    public class SunsettingControllerTests
    {
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public class Ctor
        {
            private readonly Mock<ILogger> _mockLogger;
            private readonly Mock<IGitHubService> _mockGitHubService;
            private readonly Mock<IOptions<ArchivalSettings>> _mockArchivalSettings = new Mock<IOptions<ArchivalSettings>>();

            public Ctor()
            {
                _mockLogger = new Mock<ILogger>();
                _mockGitHubService = new Mock<IGitHubService>();
            }

            [Fact]
            public void NullLogger_ThrowsException()
            {
                Action act = () => new SunsettingController(null, _mockGitHubService.Object, _mockArchivalSettings.Object);

                act.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("logger");
            }

            [Fact]
            public void NullGitHubService_ThrowsException()
            {
                Action act = () => new SunsettingController(_mockLogger.Object, null, _mockArchivalSettings.Object);

                act.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("gitHubService");
            }
        }
    }
}
