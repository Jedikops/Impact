using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TendersApi.Domain
{
    public class Tender
    {
        public int Id { get; init; }
        public DateTime Date { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
    }
}
