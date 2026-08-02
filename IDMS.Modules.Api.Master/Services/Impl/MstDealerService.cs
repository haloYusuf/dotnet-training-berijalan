using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Infrastructure.Data;
using IDMS.Modules.Api.Master.Dto.Request.MstDealer;
using IDMS.Modules.Api.Master.Dto.Response;
using IDMS.Shared.Domain.Entities;
using IDMS.Shared.Exceptions;
using IDMS.Shared.Utils;
using Microsoft.EntityFrameworkCore;

namespace IDMS.Modules.Api.Master.Services.Impl
{
    public class MstDealerService : IMstDealerService
    {
        private readonly AppDbContext _context;

        private readonly ICurrentUserServices _user;

        public MstDealerService(AppDbContext context, ICurrentUserServices user)
        {
            _context = context;
            _user = user;
        }

        public async Task<ResMstDealerDto> CreateAsync(ReqMstDealerCreateDto request)
        {
            var isCodeExist = await _context.Set<MstDealer>().AnyAsync(x => x.Code == request.Code);
            if (isCodeExist)
            {
                throw new ConflictException($"Code {request.Code} already exists");
            }

            var entity = new MstDealer
            {
                Code = request.Code,
                Name = request.Name,
                Address = request.Address,
                City = request.City,
                Region = request.Region,
                Phone = request.Phone,
                Email = request.Email,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _user.GetFullName()
            };

            _context.MstDealers.Add(entity);
            await _context.SaveChangesAsync();

            return new ResMstDealerDto
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                Address = entity.Address!,
                City = entity.City,
                Region = entity.Region,
                Phone = entity.Phone!,
                Email = entity.Email!,
                IsActive = entity.IsActive,
            };
        }

        public async Task<(IEnumerable<ResMstDealerDto> data, int total)> GetListAsync(ReqMstDealerDto request)
        {
            var query = _context.Set<MstDealer>()
            .Where(x => x.DeletedAt == null).AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                var keyword = request.Keyword.ToLower();

                query = query.Where(x => x.Name.ToLower().Contains(keyword) || x.Code.ToLower().Contains(keyword) || x.City.ToLower().Contains(keyword));
            }

            var total = await query.CountAsync();

            var data = await query
            .OrderBy(x => x.UpdatedAt ?? x.CreatedAt)
            .Skip((request.Page - 1) * request.Limit)
            .Take(request.Limit)
            .Select(x => new ResMstDealerDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Address = x.Address!,
                City = x.City,
                Region = x.Region,
                Phone = x.Phone!,
                Email = x.Email!,
                IsActive = x.IsActive,
            }).ToListAsync();

            return (data, total);
        }

        public async Task<ResMstDealerDto?> GetDealerByIdAsync(int id)
        {
            var data = await _context.MstDealers.AsNoTracking()
            .Where(v => v.Id == id && v.DeletedAt == null)
            .Select(v => new ResMstDealerDto
            {
                Id = v.Id,
                Code = v.Code,
                Name = v.Name,
                Address = v.Address!,
                City = v.City,
                Region = v.Region,
                Phone = v.Phone!,
                Email = v.Email!,
                IsActive = v.IsActive,
            })
            .FirstOrDefaultAsync();

            return data;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var entity = await _context.Set<MstDealer>().FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null) ?? throw new NotFoundException("Dealers not found");

            entity.IsActive = false;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedBy = _user.GetFullName();

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ResMstDealerDto> UpdateAsync(int id, ReqMstDealerUpdateDto request)
        {
            var entity = await _context.Set<MstDealer>().FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null) ?? throw new NotFoundException("Dealer not found");

            var exists = await _context.MstDealers.AnyAsync(e => EF.Functions.ILike(e.Code, request.Code) && e.Id != id);
            if (exists)
                throw new ConflictException($"Code '{request.Code}' already exists");

            entity.Code = request.Code;
            entity.Name = request.Name;
            entity.City = request.City;
            entity.Region = request.Region;
            entity.Address = request.Address;
            entity.Phone = request.Phone;
            entity.Email = request.Email;
            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = _user.GetFullName();

            await _context.SaveChangesAsync();

            return new ResMstDealerDto
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                City = entity.City,
                Region = entity.Region,
                Address = entity.Address!,
                Phone = entity.Phone!,
                Email = entity.Email!,
                IsActive = entity.IsActive,
            };
        }
    }
}