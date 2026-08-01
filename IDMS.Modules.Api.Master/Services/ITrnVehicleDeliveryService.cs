using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Modules.Api.Master.Dto.Request.TrnVehicleDelivery;
using IDMS.Modules.Api.Master.Dto.Response;

namespace IDMS.Modules.Api.Master.Services
{
    public interface ITrnVehicleDeliveryService
    {
        Task<(IEnumerable<ResTrnVehicleDeliveryDto> data, int total)> GetListAsync(ReqTrnVehicleDeliveryDto request);

        Task<ResTrnVehicleDeliveryDto?> GetVehicleDeliveryByIdAsync(int id);

        Task<ResTrnVehicleDeliveryDto> CreateAsync(ReqTrnVehicleDeliveryCreateDto request);

        Task<ResTrnVehicleDeliveryDto> UpdateAsync(int id, ReqTrnVehicleDeliveryUpdateDto request);

        Task<ResTrnVehicleDeliveryDto> UpdateStatusAsync(int id, string status);

        Task<bool> SoftDeleteAsync(int id);
    }
}