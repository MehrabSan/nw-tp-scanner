namespace TPS.Core.Models;

public class PageInfo(int currentPage, int pageCount)
{
    public int CurrentPage { get; set; } = currentPage;
    public int PageCount { get; set; } = pageCount;
}