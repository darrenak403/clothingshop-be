namespace ClothingShop.Application.Wrapper
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public PagedResult(IEnumerable<T> items, int pageNumber, int pageSize, int totalRecords)
        {
            Items = items ?? new List<T>();
            PageNumber = pageNumber > 0 ? pageNumber : 1;
            PageSize = pageSize > 0 ? pageSize : 10;
            TotalRecords = totalRecords >= 0 ? totalRecords : 0;
        }
        public PagedResult()
        {
            Items = new List<T>();
            PageNumber = 1;
            PageSize = 10;
            TotalRecords = 0;
        }
    }
}