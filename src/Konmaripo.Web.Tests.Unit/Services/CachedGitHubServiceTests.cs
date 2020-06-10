using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Konmaripo.Web.Services;
using Moq;
using Xunit;

namespace Konmaripo.Web.Tests.Unit.Services
{
    public class CachedGitHubServiceTests
    {
        public class Ctor
        {
            [Fact]
            public void WithNullGithubService_ThrowsException()
            {
                Action act = () => new CachedGitHubService(null);

                act.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("gitHubService");
            }
        }
        public class GetRepositoriesForOrganizationAsync
        {
            private Mock<IGitHubService> _mockService;
            private CachedGitHubService _sut;
            public GetRepositoriesForOrganizationAsync()
            {
                _sut = new CachedGitHubService(_mockService.Object);
            }

            [Fact]
            public async Task WhenCalled_CallsUnderlyingGitHubService()
            {
                await _sut.GetRepositoriesForOrganizationAsync();

                _mockService.Verify(x=>x.GetRepositoriesForOrganizationAsync(), Times.Once);
            }

            [Fact]
            public void WhenCalledMultipleTimes_StillGetsFullRepositoryListFromUnderlyingService()
            {
                throw new NotImplementedException();
            }


            [Fact]
            public void WhenCalledMultipleTimes_CallsUnderlyingGitHubServiceOnlyOnce()
            {
                throw new NotImplementedException();
            }
        }
    }
}
