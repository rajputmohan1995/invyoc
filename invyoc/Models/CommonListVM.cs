namespace invyoc.Models;

public class CommonListVM(
    int pageNum,
    int pageSize,
    string search,
    int filterMonth,
    int filterYear)
{
    public int PageNum { get; } = pageNum;
    public int PageSize { get; } = pageSize;
    public string Search { get; } = search;
    public int FilterMonth { get; } = filterMonth;
    public int FilterYear { get; } = filterYear;
}
