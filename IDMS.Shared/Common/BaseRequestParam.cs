namespace IDMS.Shared.Common;

public class BaseRequestParam
{
    public string? Keyword { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
}
