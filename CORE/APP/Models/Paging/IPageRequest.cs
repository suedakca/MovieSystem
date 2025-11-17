namespace CORE.APP.Models.Paging;

public interface IPageRequest
{
    public int PageNumber { get; set; }
    
    public int CountPerPage { get; set; }
    
    public int TotalCountForPaging { get; set; }
}