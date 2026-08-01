using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Infrastructure.Data;
using IDMS.Modules.Api.Master.Dto.Request.TrnApplication;
using IDMS.Modules.Api.Master.Dto.Response;
using IDMS.Shared.Domain.Entities;
using IDMS.Shared.Exceptions;
using IDMS.Shared.Utils;
using Microsoft.EntityFrameworkCore;

namespace IDMS.Modules.Api.Master.Services.Impl
{
    public class TrnApplicationService : ITrnApplicationService
    {
        private readonly AppDbContext _context;

        private readonly ICurrentUserServices _user;

        public TrnApplicationService(AppDbContext context, ICurrentUserServices user)
        {
            _context = context;
            _user = user;
        }

        private async Task<string> GenerateApplicationNoAsync()
        {
            var today = DateTime.UtcNow.Date;
            var dateString = today.ToString("yyyyMMdd");

            var todayCount = await _context.TrnApplications
                .AsNoTracking()
                .CountAsync(x => x.CreatedAt.Date == DateTime.UtcNow.Date);

            var nextSequence = todayCount + 1;

            return $"APP{dateString}{nextSequence:D4}";
        }

        public async Task<ResTrnApplicationDto> CreateAsync(ReqTrnApplicationCreateDto request)
        {
            string appNo = await GenerateApplicationNoAsync();

            var entity = new TrnApplication
            {
                ApplicationNo = appNo,
                CustomerId = request.CustomerId,
                ModelId = request.ModelId,
                OtrPrice = request.OtrPrice,
                DpAmount = request.DpAmount,
                TenorMonth = request.TenorMonth,
                InterestRate = request.InterestRate,
                Status = request.Status,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _user.GetFullName()
            };

            _context.TrnApplications.Add(entity);
            await _context.SaveChangesAsync();

            return new ResTrnApplicationDto
            {
                Id = entity.Id,
                ApplicationNo = entity.ApplicationNo,
                CustomerName = entity.Customer.FullName,
                ModelName = entity.Model.Name,
                OtrPrice = entity.OtrPrice,
                DpAmount = entity.DpAmount,
                TenorMonth = entity.TenorMonth,
                InterestRate = entity.InterestRate,
                Status = entity.Status,
                IsActive = entity.IsActive,
            };
        }

        public async Task<ResTrnApplicationDto?> GetApplicationByIdAsync(int id)
        {
            var data = await _context.TrnApplications.AsNoTracking()
            .Where(v => v.Id == id && v.DeletedAt == null)
            .Select(v => new ResTrnApplicationDto
            {
                Id = v.Id,
                ApplicationNo = v.ApplicationNo,
                CustomerName = v.Customer.FullName,
                ModelName = v.Model.Name,
                OtrPrice = v.OtrPrice,
                DpAmount = v.DpAmount,
                TenorMonth = v.TenorMonth,
                InterestRate = v.InterestRate,
                Status = v.Status,
                IsActive = v.IsActive,
            })
            .FirstOrDefaultAsync();

            return data;
        }

        public async Task<(IEnumerable<ResTrnApplicationDto> data, int total)> GetListAsync(ReqTrnApplicationDto request)
        {
            var query = _context.Set<TrnApplication>()
            .Where(x => x.DeletedAt == null).AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                var keyword = request.Keyword.ToLower();

                query = query.Where(x => x.Status.ToLower().Contains(keyword) || x.Customer.FullName.ToLower().Contains(keyword) || x.Model.Name.ToLower().Contains(keyword));
            }

            var total = await query.CountAsync();

            var data = await query
            .OrderBy(x => x.UpdatedAt ?? x.CreatedAt)
            .Skip((request.Page - 1) * request.Limit)
            .Take(request.Limit)
            .Select(v => new ResTrnApplicationDto
            {
                Id = v.Id,
                ApplicationNo = v.ApplicationNo,
                CustomerName = v.Customer.FullName,
                ModelName = v.Model.Name,
                OtrPrice = v.OtrPrice,
                DpAmount = v.DpAmount,
                TenorMonth = v.TenorMonth,
                InterestRate = v.InterestRate,
                Status = v.Status,
                IsActive = v.IsActive,
            }).ToListAsync();

            return (data, total);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var entity = await _context.Set<TrnApplication>().FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null) ?? throw new NotFoundException("Application not found");

            entity.IsActive = false;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedBy = _user.GetFullName();

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ResTrnApplicationDto> UpdateAsync(int id, ReqTrnApplicationUpdateDto request)
        {
            var entity = await _context.Set<TrnApplication>().FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null) ?? throw new NotFoundException("Data not found");

            entity.CustomerId = request.CustomerId;
            entity.ModelId = request.ModelId;
            entity.OtrPrice = request.OtrPrice;
            entity.DpAmount = request.DpAmount;
            entity.TenorMonth = request.TenorMonth;
            entity.InterestRate = request.InterestRate;
            entity.Status = request.Status;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = _user.GetFullName();

            await _context.SaveChangesAsync();

            return new ResTrnApplicationDto
            {
                Id = entity.Id,
                ApplicationNo = entity.ApplicationNo,
                CustomerName = entity.Customer.FullName,
                ModelName = entity.Model.Name,
                OtrPrice = entity.OtrPrice,
                DpAmount = entity.DpAmount,
                TenorMonth = entity.TenorMonth,
                InterestRate = entity.InterestRate,
                Status = entity.Status,
                IsActive = entity.IsActive,
            };
        }

        public async Task<ResTrnApplicationDto> UpdateStatusAsync(int id, string status)
        {
            var entity = await _context.Set<TrnApplication>().FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null) ?? throw new NotFoundException("Data not found");

            entity.Status = status;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = _user.GetFullName();

            await _context.SaveChangesAsync();

            return new ResTrnApplicationDto
            {
                Id = entity.Id,
                ApplicationNo = entity.ApplicationNo,
                CustomerName = entity.Customer.FullName,
                ModelName = entity.Model.Name,
                OtrPrice = entity.OtrPrice,
                DpAmount = entity.DpAmount,
                TenorMonth = entity.TenorMonth,
                InterestRate = entity.InterestRate,
                Status = entity.Status,
                IsActive = entity.IsActive,
            };
        }
    }
}