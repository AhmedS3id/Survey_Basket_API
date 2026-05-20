namespace Survey_Basket_API.Contract.Common
{
    public record RequestFilter
    {
        public int PageNumber { get; init; }=1;
        public int PageSize { get; init; } = 10;
    }
}
