namespace TendersApi.Domain
{
    public interface ITenderQueryProcessor
    {
        IEnumerable<Tender> Filter(IEnumerable<Tender> tenders, DateTime? before, DateTime? after, int lessThan, int greaterThan);

        IEnumerable<Tender> Order(IEnumerable<Tender> tenders, OrderBy orderBy, OrderByDirection orderByDirection);
    }
}