using System.Text.Json.Serialization;

namespace IDMS.Shared.Common;

public class ApiResponse<T>
{
    public string ReqId { get; set; } = string.Empty;
    public string Status { get; set; } = "success";
    public string Message { get; set; } = "Data retrieved successfully";
    public T? Data { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Pagination? Pagination { get; set; }
}
