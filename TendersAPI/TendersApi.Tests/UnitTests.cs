//using Bogus;
//using Moq;
//using TendersApi.App.Common;
//using TendersApi.App.Handlers;
//using TendersApi.App.Interfaces;
//using TendersApi.App.Queries;
//using TendersApi.Domain;
//using TendersApi.Tests.Extensions;
//using TendersAPI.App.Interfaces;

//namespace TendersApi.Tests
//{
//    //TODO: Needs updating after scooping out the TenderQueryProcessor from the TenderQueryHandler
//    public class UnitTests : IDisposable
//    {
//        private readonly List<Result<PaginatedResult<Tender>>> _fakePaginatedResult;
//        private Mock<ITenderRepository> _mockRepo;
//        private Mock<ICacheService> _mockCache;
//        private readonly GetTendersQueryHandler _queryTenderHandler;
//        private Mock<ITenderQueryProcessor> _mockTenderQueryProcessor;

//        public UnitTests()
//        {

//            var index = 1;
//            var page = 1;

//            int minValue = int.MaxValue;
//            int maxValue = 0;
//            DateTime oldest = DateTime.MaxValue;
//            DateTime newest = DateTime.MinValue;

//            var tenderFaker = new Faker<Tender>()
//                                   .RuleFor(t => t.Id, f => index++)
//                                   .RuleFor(t => t.Title, f => f.Commerce.ProductName())
//                                   .RuleFor(t => t.Description, f => f.Lorem.Paragraph())
//                                   .RuleFor(t => t.Date, f =>
//                                   {
//                                       var dateTime = f.Date.Between(DateTime.Now.AddYears(-5), DateTime.Now.AddYears(5));
//                                       oldest = dateTime < oldest ? dateTime : oldest;
//                                       newest = dateTime > newest ? dateTime : newest;
//                                       return dateTime;
//                                   })
//                                   .RuleFor(t => t.Value, f =>
//                                   {
//                                       var val = f.Random.Int(1000, 1000000);
//                                       minValue = val < minValue ? val : minValue;
//                                       maxValue = val > maxValue ? val : maxValue;
//                                       return val;
//                                   });

//            var paginatedResultFaker = new Faker<PaginatedResult<Tender>>()
//                           .RuleFor(p => p.Page, f => page++)
//                           .RuleFor(p => p.Size, f => 100)
//                           .RuleFor(p => p.Items, f => tenderFaker.Generate(100));




//            var _fakePaginatedResult = new Faker<Result<PaginatedResult<Tender>>>()
//                 .CustomInstantiator(f => Result<PaginatedResult<Tender>>.Success(paginatedResultFaker.Generate()))
//                 .RuleFor(p => p.IsSuccess, f => true)
//                 .RuleFor(p => p.Error, f => null);



//            _mockRepo = new Mock<ITenderRepository>();
//            _mockRepo.Setup(repo => repo.GetAllAsync()).Returns(_fakePaginatedResult.Generate(100).GetFakePaginatedResultAsync());
            
//            _mockCache = new Mock<ICacheService>();
//            _mockCache.Setup(s => s.GetAsync<PaginatedResult<Tender>?>(It.IsAny<string>())).ReturnsAsync(() => null);
//            _mockCache.Setup(s=> s.SetAsync(It.IsAny<string>(), It.IsAny<PaginatedResult<Tender>>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
//                .Returns(Task.CompletedTask);

//            _mockTenderQueryProcessor = new Mock<ITenderQueryProcessor>();
//            _mockTenderQueryProcessor.Setup(p => p.Filter(It.IsAny<IEnumerable<Tender>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
//                .Returns((IEnumerable<Tender> tenders, DateTime after, DateTime before, decimal greaterThan, decimal lessThan) =>
//                    tenders.Where(t => t.Date > after && t.Date < before && t.Value > greaterThan && t.Value < lessThan));

//            _queryTenderHandler = new GetTendersQueryHandler(_mockRepo.Object, _mockCache.Object, _mockTenderQueryProcessor.Object);

//        }

//        public void Dispose()
//        {

//        }

//        [Theory]
//        [ClassData(typeof(TendersTestData))]
//        public async Task TestTenderseFilters(GetTendersQuery getTendersQuery)
//        {
//            var tendersResult = await _queryTenderHandler.Handle(getTendersQuery);

//            Assert.DoesNotContain(tendersResult.Value.Items,
//                x => x.Date < getTendersQuery.Before &&
//                x.Date > getTendersQuery.After &&
//                x.Value > getTendersQuery.GreaterThan &&
//                x.Value < getTendersQuery.LessThan);

//        }

//        [Theory]
//        [ClassData(typeof(TendersTestData))]
//        public async Task TestTenderseOrder(GetTendersQuery getTendersQuery)
//        {
//            var tendersResult = await _queryTenderHandler.Handle(getTendersQuery);

//            Assert.True(getTendersQuery switch
//            {
//                { OrderBy: OrderBy.Date, OrderByDirection: OrderByDirection.Descending } => 
//                    tendersResult.Value.Items.SequenceEqual(tendersResult.Value.Items.OrderByDescending(t => t.Date)),
//                { OrderBy: OrderBy.Date, OrderByDirection: OrderByDirection.Ascending } => 
//                    tendersResult.Value.Items.SequenceEqual(tendersResult.Value.Items.OrderBy(t => t.Date)),

//                { OrderBy: OrderBy.Value, OrderByDirection: OrderByDirection.Descending } =>
//                    tendersResult.Value.Items.SequenceEqual(tendersResult.Value.Items.OrderByDescending(t => t.Value)),

//                { OrderBy: OrderBy.Value, OrderByDirection: OrderByDirection.Ascending } =>
//                    tendersResult.Value.Items.SequenceEqual(tendersResult.Value.Items.OrderBy(t => t.Value)),

//                _ => true
//            });

//        }
//    }
//}
