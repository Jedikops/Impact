using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TendersApi.App.Common
{
    public enum ResultStatus
    {
        Success,
        ValidationError,
        NotFound,
        ExternalApiError,
        InternalError
    }
}
