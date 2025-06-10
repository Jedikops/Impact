using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TendersApi.Domain
{
    public class TenderQueryProcessor : ITenderQueryProcessor
    {
        public IEnumerable<Tender> Filter(IEnumerable<Tender> tenders, DateTime? before, DateTime? after, int lessThan, int greaterThan)
        {
            if (after != null || before != null || lessThan < int.MaxValue || greaterThan > 0)
            {
                return tenders
                 .Where(x =>
                     (!after.HasValue || x.Date > after.Value) &&
                     (!before.HasValue || x.Date < before.Value) &&
                     (greaterThan >= 0 && x.Value > greaterThan) &&
                     (lessThan >= 0 && x.Value < lessThan))
                 .ToList();
            }

            return tenders;
        }

        public IEnumerable<Tender> FilterBySupplierId(IEnumerable<Tender> items, int id)
        {
            return items.Where(t => t.SupplierIds.Any(x => x == id)).ToList();
        }

        public IEnumerable<Tender> Order(IEnumerable<Tender> tenders, OrderBy orderBy, OrderByDirection orderByDirection)
        {
            if (orderBy != OrderBy.NotSet)
            {
                return (orderBy, orderByDirection) switch
                {
                    { orderBy: OrderBy.Date, orderByDirection: OrderByDirection.Descending } => tenders.OrderByDescending(t => t.Date).ToList(),
                    { orderBy: OrderBy.Date } => tenders.OrderBy(t => t.Date).ToList(),
                    { orderBy: OrderBy.Value, orderByDirection: OrderByDirection.Descending } => tenders.OrderByDescending(t => t.Value).ToList(),
                    { orderBy: OrderBy.Value } => tenders.OrderBy(t => t.Value).ToList(),
                    _ => tenders
                };
            }

            return tenders;
        }

    }
}
