using System.Collections;
using TendersApi.App.Queries;
using TendersApi.Domain;

namespace TendersApi.Tests
{
    public class TendersTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {


            yield return new object[] { new GetTendersQuery() {
                After = new DateTime(2021, 12, 12),
                Before = new DateTime(2022, 3, 12),
                OrderBy = OrderBy.Date,
                OrderByDirection = OrderByDirection.Descending }};

            yield return new object[] {  new GetTendersQuery() {
                After = new DateTime(2021, 12, 12),
                Before = new DateTime(2022, 3, 12),
                OrderBy = OrderBy.Date,
                OrderByDirection = OrderByDirection.Ascending }};

            yield return new object[] { new GetTendersQuery() {
                LessThan = 5345,
                GreaterThan = 324,
                OrderBy = OrderBy.Value,
                OrderByDirection = OrderByDirection.Descending }};

            yield return new object[] { new GetTendersQuery() {
                LessThan = 5345, 
                GreaterThan = 324, 
                OrderBy = OrderBy.Value, 
                OrderByDirection = OrderByDirection.Ascending
            }
            };

        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
