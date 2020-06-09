using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Konmaripo.Web.Controllers;
using Konmaripo.Web.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Konmaripo.Web.Tests.Unit.Controllers
{
    public class HomeControllerTests
    {
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public class Ctor
        {
            private readonly Mock<ILogger<HomeController>> _mockLogger;
            private readonly Mock<IGitHubService> _mockGitHubService;

            public Ctor()
            {
                _mockLogger = new Mock<ILogger<HomeController>>();
                _mockGitHubService = new Mock<IGitHubService>();
            }

            [Fact]
            public void NullLogger_ThrowsException()
            {
                Action act = () => new HomeController(null, _mockGitHubService.Object);

                act.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("logger");
            }

            [Fact]
            public void NullGitHubService_ThrowsException()
            {
                Action act = () => new HomeController(_mockLogger.Object, null);

                act.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("gitHubService");
            }
        }
    }
}
