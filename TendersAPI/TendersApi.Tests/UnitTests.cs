using Bogus;
using Moq;
using TendersApi.App.Common;
using TendersApi.App.Handlers;
using TendersApi.App.Interfaces;
using TendersApi.App.Queries;
using TendersApi.Domain;
using TendersApi.Tests.Extensions; 

namespace TendersApi.Tests
{

    public class UnitTests : IDisposable
    {
        private readonly List<Result<PaginatedResult<Tender>>> _fakePaginatedResult;
        private Mock<ITenderRepository> _mockRepo;
        private readonly GetTendersQueryHandler _queryTenderHandler;
        public UnitTests()
        {
            var index = 1;
            var page = 1;

            int minValue = int.MaxValue;
            int maxValue = 0;
            DateTime oldest = DateTime.MaxValue;
            DateTime newest = DateTime.MinValue;

            _fakePaginatedResult = new Faker<Result<PaginatedResult<Tender>>>()
                .RuleFor(p => p.IsSuccess, f => true)
                .RuleFor(p => p.Error, f => null)
                .RuleFor(p => p.Value, f =>
                {
                    return new Faker<PaginatedResult<Tender>>()
                           .RuleFor(p => p.Page, f => page++)
                           .RuleFor(p => p.Size, f => 100)
                           .RuleFor(p => p.Items, f =>
                           {
                               return new Faker<Tender>()
                                   .RuleFor(t => t.Id, f => index++)
                                   .RuleFor(t => t.Title, f => f.Commerce.ProductName())
                                   .RuleFor(t => t.Description, f => f.Lorem.Paragraph())
                                   .RuleFor(t => t.Date, f =>
                                   {
                                       var dateTime = f.Date.Between(DateTime.Now.AddYears(-5), DateTime.Now.AddYears(5));
                                       oldest = dateTime < oldest ? dateTime : oldest;
                                       newest = dateTime > newest ? dateTime : newest;
                                       return dateTime;
                                   })
                                   .RuleFor(t => t.Value, f =>
                                   {
                                       var val = f.Random.Int(1000, 1000000);
                                       minValue = val < minValue ? val : minValue;
                                       maxValue = val > maxValue ? val : maxValue;
                                       return val;
                                   }).Generate(100);
                           });
                }).Generate(100);

            _mockRepo = new Mock<ITenderRepository>();
            _mockRepo.Setup(repo => repo.GetAllAsync()).Returns(_fakePaginatedResult.GetFakePaginatedResultAsync());

            _queryTenderHandler = new GetTendersQueryHandler(_mockRepo.Object);

        }

        public void Dispose()
        {
            
        }

        //[Theory]
        //[InlineData(new GetTendersQuery() { })] // TODO: tomorrow
        public void TestTenderseQuery()
        {
          //  _queryTenderHandler.Handle()
            Assert.True(true);
        }

        // Instructions:
        // 1. Add a project reference to the target AppHost project, e.g.:
        //
        //    <ItemGroup>
        //        <ProjectReference Include="../MyAspireApp.AppHost/MyAspireApp.AppHost.csproj" />
        //    </ItemGroup>
        //
        // 2. Uncomment the following example test and update 'Projects.MyAspireApp_AppHost' to match your AppHost project:
        //
        // [Fact]
        // public async Task GetWebResourceRootReturnsOkStatusCode()
        // {
        //     // Arrange
        //     var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.MyAspireApp_AppHost>();
        //     appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        //     {
        //         clientBuilder.AddStandardResilienceHandler();
        //     });
        //     // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging
        //
        //     await using var app = await appHost.BuildAsync();
        //     var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();
        //     await app.StartAsync();

        //     // Act
        //     var httpClient = app.CreateHttpClient("webfrontend");
        //     await resourceNotificationService.WaitForResourceAsync("webfrontend", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        //     var response = await httpClient.GetAsync("/");

        //     // Assert
        //     Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        // }
    }
}
