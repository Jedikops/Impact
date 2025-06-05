using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TendersApi.App.Common
{
    public class Result<T>
    {
        public bool IsSuccess => Status == ResultStatus.Success;
        public ResultStatus Status { get; }
        public T? Value { get; }
        public string? Error { get; }

        private Result(T value)
        {
            Status = ResultStatus.Success;
            Value = value;
        }

        private Result(ResultStatus status, string error)
        {
            Status = status;
            Error = error;
        }

        public static Result<T> Success(T value) => new(value);

        public static Result<T> Failure(ResultStatus status, string error) => new(status, error);
    }
}
