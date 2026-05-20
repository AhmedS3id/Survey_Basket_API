namespace Survey_Basket_API.Abstractions
{
    public class PaginatedList<T>
    {
        public PaginatedList(List<T>items,int pageNumber,int count,int pageSize)
        {
            Items = items;
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }
        public List<T> Items {  get; private set; }
       public int PageNumber {  get; set; }
       public int TotalPages {  get; set; }
        public bool HasNextPages => PageNumber < TotalPages;
        public bool HasPreviousPages => PageNumber > 1;

        public static async Task<PaginatedList<T>>CreateAsync(IQueryable<T> query,int pageNumber, int pageSize)
        {
            var count =await query.CountAsync();
            var items= await query.Skip((pageNumber-1)*pageSize).Take(pageSize).ToListAsync();

            return  new PaginatedList<T>(items,pageNumber,count,pageSize);
        }

    }
}
