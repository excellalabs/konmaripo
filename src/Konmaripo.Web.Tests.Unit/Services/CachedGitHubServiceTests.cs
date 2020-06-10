using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using Konmaripo.Web.Models;
using Konmaripo.Web.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace Konmaripo.Web.Tests.Unit.Services
{
    public class CachedGitHubServiceTests
    {
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public class Ctor
        {
            [Fact]
            public void WithNullGithubService_ThrowsException()
            {
                Action act = () => new CachedGitHubService(null, new Mock<IMemoryCache>().Object);

                act.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("gitHubService");
            }

            [Fact]
            public void WithNullMemoryCache_ThrowsException()
            {
                Action act = () => new CachedGitHubService(new Mock<IGitHubService>().Object, null);

                act.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("memoryCache");

            }
        }
        public class GetRepositoriesForOrganizationAsync : IDisposable
        {
            private readonly Mock<IGitHubService> _mockService;
            private readonly MemoryCache _memoryCache;
            private readonly CachedGitHubService _sut;
            public GetRepositoriesForOrganizationAsync()
            {
                _mockService = new Mock<IGitHubService>();
                _mockService.Setup(x => x.GetRepositoriesForOrganizationAsync())
                    .Returns(Task.FromResult(new List<GitHubRepo>()));

                _memoryCache = new MemoryCache(new MemoryCacheOptions());
                _sut = new CachedGitHubService(_mockService.Object, _memoryCache);
            }
            public void Dispose()
            {
                _memoryCache?.Dispose();
            }

            [Fact]
            public async Task WhenCalled_CallsUnderlyingGitHubService()
            {
                await _sut.GetRepositoriesForOrganizationAsync();

                _mockService.Verify(x=>x.GetRepositoriesForOrganizationAsync(), Times.Once);
            }

            [Fact]
            public async Task WhenCalledOnce_GetsFullRepositoryListFromUnderlyingService()
            {
                var fakeRepos = new List<GitHubRepo>
                {
                    new GitHubRepoBuilder().WithId(1).Build(),
                    new GitHubRepoBuilder().WithId(12).Build(),
                    new GitHubRepoBuilder().WithId(123).Build()
                };

                _mockService.Setup(x => x.GetRepositoriesForOrganizationAsync())
                    .Returns(Task.FromResult(fakeRepos));

                var result = await _sut.GetRepositoriesForOrganizationAsync();

                result.Should().BeEquivalentTo(fakeRepos);
            }

            [Fact]
            public async Task WhenCalledMultipleTimes_CallsUnderlyingGitHubServiceOnlyOnce()
            {
                await _sut.GetRepositoriesForOrganizationAsync();
                await _sut.GetRepositoriesForOrganizationAsync();
                await _sut.GetRepositoriesForOrganizationAsync();

                _mockService.Verify(x => x.GetRepositoriesForOrganizationAsync(), Times.Once);
            }

            [Fact]
            public void WhenCalledMultipleTimes_StillGetsFullRepositoryListFromUnderlyingService()
            {
                throw new NotImplementedException();
            }

        }

        public class GitHubRepoBuilder
        {
            private long _id;
            public GitHubRepoBuilder WithId(long id)
            {
                _id = id;
                return this;
            }

            public GitHubRepo Build()
            {
                return new GitHubRepo(_id,string.Empty,0,false,0,0,DateTimeOffset.Now,DateTimeOffset.Now, string.Empty,false,DateTimeOffset.Now);
            }

        }
    }
}
