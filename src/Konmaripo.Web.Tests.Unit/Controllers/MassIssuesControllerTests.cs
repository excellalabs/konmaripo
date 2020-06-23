using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Konmaripo.Web.Controllers;
using Konmaripo.Web.Services;
using Moq;
using Serilog;
using Xunit;

namespace Konmaripo.Web.Tests.Unit.Controllers
{
    public class MassIssuesControllerTests
    {
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public class Ctor
        {
            private readonly Mock<ILogger> _mockLogger;
            private readonly Mock<IGitHubService> _mockGitHubService;
            private readonly Mock<IMassIssueCreator> _mockIssueCreator;

            public Ctor()
            {
                _mockLogger = new Mock<ILogger>();
                _mockGitHubService = new Mock<IGitHubService>();
                _mockIssueCreator = new Mock<IMassIssueCreator>();
            }

            [Fact]
            public void NullLogger_ThrowsException()
            {
                Action act = () => new MassIssuesController(null, _mockGitHubService.Object, _mockIssueCreator.Object);

                act.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("logger");
            }

            [Fact]
            public void NullGitHubService_ThrowsException()
            {
                Action act = () => new MassIssuesController(_mockLogger.Object, null, _mockIssueCreator.Object);

                act.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("gitHubService");
            }

            [Fact]
            public void NullMassIssueCreator_ThrowsException()
            {
                Action act = () => new MassIssuesController(_mockLogger.Object, _mockGitHubService.Object, null);

                act.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("massIssueCreator");
            }
        }
    }
}
