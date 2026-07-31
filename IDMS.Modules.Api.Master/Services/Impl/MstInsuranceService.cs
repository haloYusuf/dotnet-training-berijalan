using IDMS.Infrastructure.Data;
using IDMS.Modules.Api.Master.Dto.Request.MstInsurance;
using IDMS.Modules.Api.Master.Dto.Response;
using IDMS.Shared.Domain.Entities;
using IDMS.Shared.Exceptions;
using IDMS.Shared.Utils;
using Microsoft.EntityFrameworkCore;

namespace IDMS.Modules.Api.Master.Services.Impl
{
    public class MstInsuranceService : IMstInsuranceService
    {
        private readonly AppDbContext _context;

        private readonly ICurrentUserServices _user;

        public MstInsuranceService(AppDbContext context, ICurrentUserServices user)
        {
            _context = context;
            _user = user;
        }

        public async Task<ResMstInsuranceDto> CreateAsync(ReqMstInsuranceCreateDto request)
        {
            var isCodeExist = await _context.Set<MstInsurance>().AnyAsync(x => x.Code == request.Code);
            if (isCodeExist)
            {
                throw new ConflictException("Code already exists");
            }

            var entity = new MstInsurance
            {
                Code = request.Code,
                Name = request.Name,
                CoverageType = request.CoverageType,
                Rate = request.Rate,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _user.GetFullName()
            };

            _context.mstInsurances.Add(entity);
            await _context.SaveChangesAsync();

            return new ResMstInsuranceDto
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                CoverageType = entity.CoverageType,
                Rate = entity.Rate,
                IsActive = entity.IsActive,
            };
        }

        public async Task<ResMstInsuranceDto?> GetInsuranceByIdAsync(int id)
        {
            var data = await _context.mstInsurances.AsNoTracking()
            .Where(v => v.Id == id && v.DeletedAt == null)
            .Select(v => new ResMstInsuranceDto
            {
                Id = v.Id,
                Code = v.Code,
                Name = v.Name,
                CoverageType = v.CoverageType,
                Rate = v.Rate,
                IsActive = v.IsActive,
            })
            .FirstOrDefaultAsync();

            return data;
        }

        public async Task<(IEnumerable<ResMstInsuranceDto> data, int total)> GetListAsync(ReqMstInsuranceDto request)
        {
            var query = _context.Set<MstInsurance>()
            .Where(x => x.DeletedAt == null).AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                var keyword = request.Keyword.ToLower();

                query = query.Where(x => x.Name.ToLower().Contains(keyword) || x.Code.ToLower().Contains(keyword));
            }

            var total = await query.CountAsync();

            var data = await query
            .OrderBy(x => x.UpdatedAt ?? x.CreatedAt)
            .Skip((request.Page - 1) * request.Limit)
            .Take(request.Limit)
            .Select(v => new ResMstInsuranceDto
            {
                Id = v.Id,
                Code = v.Code,
                Name = v.Name,
                CoverageType = v.CoverageType,
                Rate = v.Rate,
                IsActive = v.IsActive,
            }).ToListAsync();

            return (data, total);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var entity = await _context.Set<MstInsurance>().FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null) ?? throw new NotFoundException("Dealers not found");

            entity.IsActive = false;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedBy = _user.GetFullName();

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ResMstInsuranceDto> UpdateAsync(int id, ReqMstInsuranceUpdateDto request)
        {
            var entity = await _context.Set<MstInsurance>().FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null) ?? throw new NotFoundException("Dealer not found");

            entity.Code = request.Code;
            entity.Name = request.Name;
            entity.CoverageType = request.CoverageType;
            entity.Rate = request.Rate;
            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = _user.GetFullName();

            await _context.SaveChangesAsync();

            return new ResMstInsuranceDto
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                CoverageType = entity.CoverageType,
                Rate = entity.Rate,
                IsActive = entity.IsActive,
            };
        }
    }
}