using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TendersApi.App.Queries
{
    public class GetTendersQuery
    {
        public int Page { get; init; }
        public DateTime? Before { get; init; } = DateTime.Now;
        public DateTime? After { get; init; } 

        public int PageSize { get; private init; } = 100;

    }
}
