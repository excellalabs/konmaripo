using Bogus;
using Octokit;

namespace Konmaripo.Web.Tests.Unit.Helpers
{
    public class OrganizationBuilder
    {
        private readonly Faker<Organization> _faker = new Faker<Organization>();
        private readonly Faker<Plan> _planFaker = new Faker<Plan>();

        public OrganizationBuilder WithPrivateRepoLimit(long limit)
        {
            _planFaker.RuleFor(pl => pl.PrivateRepos, limit);
            return this;
        }

        public Organization Build()
        {
            var plan = _planFaker.Generate();
            _faker.RuleFor(x => x.Plan, plan);
            _faker.AssertConfigurationIsValid();
            return _faker.Generate();
        }

        public OrganizationBuilder WithPrivateRepoCount(int privateRepoCount)
        {
            _faker.RuleFor(x => x.OwnedPrivateRepos, privateRepoCount);
            return this;
        }
    }
}