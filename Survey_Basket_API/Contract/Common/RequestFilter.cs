namespace Survey_Basket_API.Contract.Common
{
    public record RequestFilter
    {
        public enum ValidSort
        {
            ASC,
            DESC
        }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public string SearchValue { get ; init; }=string.Empty;
        public string? SortColumn { get; init; }
        public ValidSort? SortDirection { get; init; } = ValidSort.ASC;
    }
}
