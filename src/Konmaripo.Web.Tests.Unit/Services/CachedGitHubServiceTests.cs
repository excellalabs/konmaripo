using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Konmaripo.Web.Services;
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
    }
}
