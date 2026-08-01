using IDMS.Infrastructure.Data;
using IDMS.Modules.Api.Master.Dto.Request.MstType;
using IDMS.Modules.Api.Master.Dto.Response;
using IDMS.Shared.Domain.Entities;
using IDMS.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace IDMS.Modules.Api.Master.Services.Impl
{
    public class MstTypeService : IMstTypeService
    {
        private readonly AppDbContext _context;

        public MstTypeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<ResMstTypeDto> data, int total)> GetListAsync(ReqMstTypeDto request)
        {
            var query = _context.Set<MstType>()
                .Include(x => x.Brand)
                .Where(x => x.DeletedAt == null) // Filter Soft Delete
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                var keyword = request.Keyword.ToLower();
                query = query.Where(x => x.Name.ToLower().Contains(keyword) || x.Code.ToLower().Contains(keyword));
            }

            var total = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.Year) // Default sorting
                .Skip((request.Page - 1) * request.Limit)
                .Take(request.Limit)
                .Select(x => new ResMstTypeDto
                {
                    Id = x.Id,
                    BrandId = x.BrandId,
                    BrandName = x.Brand.Name,
                    Code = x.Code,
                    Name = x.Name,
                    Year = x.Year,
                    IsActive = x.IsActive
                })
                .ToListAsync();

            return (data, total);
        }

        public async Task<ResMstTypeDto> CreateAsync(ReqMstTypeCreateDto request)
        {
            var isBrandExist = await _context.Set<MstBrand>().AnyAsync(x => x.Id == request.BrandId && x.DeletedAt == null);
            if (!isBrandExist) throw new BadRequestException("Brand does not exist");

            var isCodeExist = await _context.Set<MstType>().AnyAsync(x => x.Code.ToLower() == request.Code.ToLower() && x.DeletedAt == null);
            if (isCodeExist) throw new ConflictException("Code already exists");

            var entity = new MstType
            {
                BrandId = request.BrandId,
                Code = request.Code,
                Name = request.Name,
                Year = request.Year,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Set<MstType>().Add(entity);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(entity.Id);
        }

        public async Task<ResMstTypeDto> UpdateAsync(int id, ReqMstTypeUpdateDto request)
        {
            var entity = await _context.Set<MstType>().FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
            if (entity == null) throw new NotFoundException("Type not found");

            var isBrandExist = await _context.Set<MstBrand>().AnyAsync(x => x.Id == request.BrandId && x.DeletedAt == null);
            if (!isBrandExist) throw new BadRequestException("Brand does not exist");

            var isCodeExist = await _context.Set<MstType>()
                .AnyAsync(x => x.Code.ToLower() == request.Code.ToLower() && x.Id != id && x.DeletedAt == null);
            if (isCodeExist) throw new ConflictException("Code already exists");

            entity.BrandId = request.BrandId;
            entity.Code = request.Code;
            entity.Name = request.Name;
            entity.Year = request.Year;
            entity.IsActive = request.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(entity.Id);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var entity = await _context.Set<MstType>().FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
            if (entity == null) throw new NotFoundException("Type not found");

            entity.IsActive = false;
            entity.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        // Private helper method agar kode lebih bersih
        private async Task<ResMstTypeDto> GetByIdAsync(int id)
        {
            var data = await _context.Set<MstType>()
                .Include(x => x.Brand)
                .FirstOrDefaultAsync(x => x.Id == id);

            return new ResMstTypeDto
            {
                Id = data!.Id,
                BrandId = data.BrandId,
                BrandName = data.Brand.Name,
                Code = data.Code,
                Name = data.Name,
                Year = data.Year,
                IsActive = data.IsActive
            };
        }

        public async Task<ResMstTypeDto?> GetTypeByIdAsync(int id)
        {
            var data = await _context.MstTypes.AsNoTracking()
            .Where(v => v.Id == id && v.DeletedAt == null)
            .Select(
                v => new ResMstTypeDto
                {
                    Id = v.Id,
                    BrandId = v.BrandId,
                    BrandName = v.Brand.Name,
                    Code = v.Code,
                    Name = v.Name,
                    IsActive = v.IsActive,
                    Year = v.Year
                }
            ).FirstOrDefaultAsync();

            return data;
        }
    }
}