namespace IDMS.Shared.Common;

public class Pagination
{
    public int CurrentPage { get; set; }
    public int Limit { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}
