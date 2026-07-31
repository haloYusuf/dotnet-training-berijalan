namespace IDMS.Modules.Api.Master.Dto.Request.MstModel
{
    public class ReqMstModelCreateDto
    {
        public int TypeId { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int Year { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}