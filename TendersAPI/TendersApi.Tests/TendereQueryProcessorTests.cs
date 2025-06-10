using Bogus;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TendersApi.App.Queries;
using TendersApi.Domain;

namespace TendersApi.Tests
{
    public class TendereQueryProcessorTests
    {
        List<Tender> _fakeTenders;
        TenderQueryProcessor _tenderQueryProcessor;

        public TendereQueryProcessorTests()
        {
            _tenderQueryProcessor = new TenderQueryProcessor();


            var faker = new Faker<Tender>()
                .RuleFor(t => t.Date, f => f.Date.Between(DateTime.UtcNow.AddYears(-1), DateTime.UtcNow))
                .RuleFor(t => t.Value, f => f.Random.Decimal(1000, 50000));

            _fakeTenders = faker.Generate(10000);
        }



        [Theory]
        [ClassData(typeof(TendersTestData))]
        public void TestTenderFilter_AppliesCorrectFilter(GetTendersQuery getTendersQuery)
        {

            var filtered = _tenderQueryProcessor.Filter(_fakeTenders, getTendersQuery.After, getTendersQuery.Before, getTendersQuery.GreaterThan, getTendersQuery.LessThan);

            Assert.All(filtered, item =>
                Assert.True(
                    item.Date > getTendersQuery.After &&
                    item.Date < getTendersQuery.Before &&
                    item.Value > getTendersQuery.GreaterThan &&
                    item.Value < getTendersQuery.LessThan
                )
            );
        }

        [Theory]
        [ClassData(typeof(TendersTestData))]
        public void TestTenderOrder_AppliesCorrectOrder(GetTendersQuery getTendersQuery)
        {
            var ordered = _tenderQueryProcessor.Order(_fakeTenders, getTendersQuery.OrderBy, getTendersQuery.OrderByDirection);

            for (int i = 1; i < ordered.Count(); i++)
            {
                Assert.True(
                    getTendersQuery.OrderBy switch
                    {
                        OrderBy.Date when getTendersQuery.OrderByDirection == OrderByDirection.Descending =>
                            ordered.ElementAt(i).Date <= ordered.ElementAt(i - 1).Date,

                        OrderBy.Date when getTendersQuery.OrderByDirection == OrderByDirection.Ascending =>
                            ordered.ElementAt(i).Date >= ordered.ElementAt(i - 1).Date,

                        OrderBy.Value when getTendersQuery.OrderByDirection == OrderByDirection.Descending =>
                            ordered.ElementAt(i).Value <= ordered.ElementAt(i - 1).Value,

                        OrderBy.Value when getTendersQuery.OrderByDirection == OrderByDirection.Ascending =>
                            ordered.ElementAt(i).Value >= ordered.ElementAt(i - 1).Value,

                        _ => throw new ArgumentException($"Invalid OrderBy value: {getTendersQuery.OrderBy}")
                    },
                    $"Item at index {i} does not satisfy the ordering condition.");
            }


        }
    }
}
