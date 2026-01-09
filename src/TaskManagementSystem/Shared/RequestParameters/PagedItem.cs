namespace Shared.RequestParameters;

public class PagedItemList<T> : List<T>
{
    public MetaData metaData { get; set; }

    public PagedItemList(List<T> items, int pageSize, int pageNumber, int count)
    {
        metaData = new MetaData() 
        {
            totalCount = count,
            pageSize = pageSize,
            currentPage = pageNumber,
            totalPages = (int)Math.Ceiling((double)count / pageSize)
        };
        AddRange(items);
    }

    public static PagedItemList<T> ToPagedListItems(IEnumerable<T> items, int count, int pageSize, int pageNumber)
    {
        return new PagedItemList<T>(items.ToList(), pageSize, pageNumber, count);
    }
}
