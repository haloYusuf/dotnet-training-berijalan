using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Infrastructure.Data;
using IDMS.Modules.Api.Master.Dto.Request.TrnVehicleDelivery;
using IDMS.Modules.Api.Master.Dto.Response;
using IDMS.Shared.Domain.Entities;
using IDMS.Shared.Exceptions;
using IDMS.Shared.Utils;
using Microsoft.EntityFrameworkCore;

namespace IDMS.Modules.Api.Master.Services.Impl
{
    public class TrnVehicleDeliveryService : ITrnVehicleDeliveryService
    {
        private readonly AppDbContext _context;

        private readonly ICurrentUserServices _user;

        public TrnVehicleDeliveryService(AppDbContext context, ICurrentUserServices user)
        {
            _context = context;
            _user = user;
        }

        private async Task<string> GenerateDeliveryNoAsync()
        {
            var today = DateTime.UtcNow.ToLocalTime().Date;
            var dateString = today.ToString("yyyyMMdd");

            var todayCount = await _context.TrnVehicleDeliveries
                .AsNoTracking()
                .CountAsync(x => x.CreatedAt.Date == DateTime.UtcNow.Date);

            var nextSequence = todayCount + 1;

            return $"DLV{dateString}{nextSequence:D4}";
        }

        public async Task<ResTrnVehicleDeliveryDto> CreateAsync(ReqTrnVehicleDeliveryCreateDto request)
        {
            var isAppValid = await _context.Set<TrnVehicleDelivery>().AnyAsync(x => x.ApplicationId == request.ApplicationId && !x.Application.Status.Equals("APPROVED") && x.DeletedAt == null);
            if (isAppValid)
            {
                throw new ConflictException("Application Status is not valid");
            }

            var isStatusValid = await _context.Set<TrnVehicleDelivery>()
            .Include(x => x.Application)
            .AnyAsync(x => x.ApplicationId == request.ApplicationId && x.Status == request.Status && (request.Status.Equals("PLANNED") || request.Status.Equals("IN_TRANSIT")) && x.DeletedAt == null);
            if (isStatusValid)
            {
                throw new ConflictException("An entry for this application with the same status has already been recorded.");
            }
            string deliveryNo = await GenerateDeliveryNoAsync();

            var entity = new TrnVehicleDelivery
            {
                DeliveryNo = deliveryNo,
                ApplicationId = request.ApplicationId,
                DealerId = request.DealerId,
                InsuranceId = request.InsuranceId,
                DeliveryDate = request.DeliveryDate,
                DriverName = request.DriverName,
                DriverPhone = request.DriverPhone,
                PlatNumber = request.PlatNumber,
                Status = request.Status,
                Notes = request.Notes,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _user.GetFullName()
            };

            _context.TrnVehicleDeliveries.Add(entity);
            await _context.SaveChangesAsync();

            return await GetVehicleDeliveryByIdAsync(entity.Id) ?? throw new NotFoundException("Data gagal dimuat");
        }

        public async Task<(IEnumerable<ResTrnVehicleDeliveryDto> data, int total)> GetListAsync(ReqTrnVehicleDeliveryDto request)
        {
            var query = _context.Set<TrnVehicleDelivery>()
            .Include(x => x.Dealer)
            .Include(x => x.Insurance)
            .Include(x => x.Application)
            .Where(x => x.DeletedAt == null).AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                var keyword = request.Keyword.ToLower();

                query = query.Where(x => x.Status.ToLower().Contains(keyword) || x.Dealer.Name.ToLower().Contains(keyword) || x.Insurance.Name.ToLower().Contains(keyword));
            }

            var total = await query.CountAsync();

            var data = await query
            .OrderBy(x => x.UpdatedAt ?? x.CreatedAt)
            .Skip((request.Page - 1) * request.Limit)
            .Take(request.Limit)
            .Select(v => new ResTrnVehicleDeliveryDto
            {
                Id = v.Id,
                DealerId = v.DealerId,
                InsuranceId = v.InsuranceId,
                ApplicationId = v.ApplicationId,
                DeliveryNo = v.DeliveryNo,
                DealerName = v.Dealer.Name,
                InsuranceName = v.Insurance.Name,
                DeliveryDate = v.DeliveryDate,
                DriverName = v.DriverName,
                DriverPhone = v.DriverPhone,
                PlatNumber = v.PlatNumber,
                Status = v.Status,
                Notes = v.Notes,
                IsActive = v.IsActive,
            }).ToListAsync();

            return (data, total);
        }

        public async Task<ResTrnVehicleDeliveryDto?> GetVehicleDeliveryByIdAsync(int id)
        {
            var data = await _context.TrnVehicleDeliveries
            .Include(x => x.Dealer)
            .Include(x => x.Insurance)
            .Include(x => x.Application)
            .AsNoTracking()
            .Where(v => v.Id == id && v.DeletedAt == null)
            .Select(v => new ResTrnVehicleDeliveryDto
            {
                Id = v.Id,
                DealerId = v.DealerId,
                InsuranceId = v.InsuranceId,
                ApplicationId = v.ApplicationId,
                DeliveryNo = v.DeliveryNo,
                DealerName = v.Dealer.Name,
                InsuranceName = v.Insurance.Name,
                DeliveryDate = v.DeliveryDate,
                DriverName = v.DriverName,
                DriverPhone = v.DriverPhone,
                PlatNumber = v.PlatNumber,
                Status = v.Status,
                Notes = v.Notes,
                IsActive = v.IsActive,
            })
            .FirstOrDefaultAsync();

            return data;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var entity = await _context.Set<TrnVehicleDelivery>().FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null) ?? throw new NotFoundException("Transaction not found");

            entity.IsActive = false;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedBy = _user.GetFullName();

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ResTrnVehicleDeliveryDto> UpdateAsync(int id, ReqTrnVehicleDeliveryUpdateDto request)
        {
            var isStatusValid = await _context.Set<TrnVehicleDelivery>()
            .Include(x => x.Application)
            .AnyAsync(x => x.ApplicationId == request.ApplicationId && !x.Application.Status.Equals("APPROVED") && x.DeletedAt == null);
            if (isStatusValid)
            {
                throw new ConflictException("Application Status is not valid");
            }

            var isAnyData = await _context.Set<TrnVehicleDelivery>()
            .Include(x => x.Application)
            .AnyAsync(x => x.Id != id && x.Status == request.Status && x.ApplicationId == request.ApplicationId && (!x.Status.Equals("DELIVERED") || !x.Status.Equals("CANCELLED")) && x.DeletedAt == null);
            if (isAnyData)
            {
                throw new ConflictException("An entry for this application with the same status has already been recorded.");
            }
            var entity = await _context.Set<TrnVehicleDelivery>().FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null) ?? throw new NotFoundException("Data not found");

            entity.ApplicationId = request.ApplicationId;
            entity.DealerId = request.DealerId;
            entity.InsuranceId = request.InsuranceId;
            entity.DeliveryDate = request.DeliveryDate;
            entity.DriverName = request.DriverName;
            entity.DriverPhone = request.DriverPhone;
            entity.PlatNumber = request.PlatNumber;
            entity.Status = request.Status;
            entity.Notes = request.Notes;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = _user.GetFullName();

            await _context.SaveChangesAsync();

            return await GetVehicleDeliveryByIdAsync(entity.Id) ?? throw new NotFoundException("Data gagal dimuat");
        }

        public async Task<ResTrnVehicleDeliveryDto> UpdateStatusAsync(int id, string status)
        {
            var entity = await _context.Set<TrnVehicleDelivery>().FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null) ?? throw new NotFoundException("Data not found");

            var isAnyData = await _context.Set<TrnVehicleDelivery>()
            .Include(x => x.Application)
            .AnyAsync(x => x.Id != id && x.Status == status && x.ApplicationId == entity.ApplicationId && !x.Status.Equals("DELIVERED") && !x.Status.Equals("CANCELLED") && x.DeletedAt == null);
            if (isAnyData)
            {
                throw new ConflictException("An entry for this application with the same status has already been recorded.");
            }

            entity.Status = status;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = _user.GetFullName();

            await _context.SaveChangesAsync();

            return await GetVehicleDeliveryByIdAsync(entity.Id) ?? throw new NotFoundException("Data gagal dimuat");
        }
    }
}