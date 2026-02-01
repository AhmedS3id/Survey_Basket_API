using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Survey_Basket_API.Abstractions;

namespace Survey_Basket_API.Abstractions
{
    public class Result
    {
        public Result(bool is_success , Error error) 
        {
            if ((is_success && error != Error.None) || (!is_success && error == Error.None))
                throw new InvalidOperationException();
            IsSuccess= is_success;
            Error = error  ;
        }
        public bool IsSuccess {  get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }= default!;

        public static Result success() => new ( true ,Error.None);
        public static Result Failure(Error error) => new ( false ,error);

        public static Result<TValue> success<TValue>(TValue value) => new(value ,true,Error.None);
        public static Result<TValue> Failure<TValue>(Error error) => new(default! ,false,error);
    }
}
public class Result<TValue> : Result
{
    private readonly TValue _value;

    public Result(TValue value, bool is_success, Error error) : base(is_success, error)
    {
        _value = value;
    }
    public TValue value => IsSuccess
        ? _value
        : throw new InvalidOperationException("Cannot access value when result is failure");
}
