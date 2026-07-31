using IDMS.Infrastructure.Data;
using IDMS.Modules.Api.Master.Dto.Request.MstModel;
using IDMS.Modules.Api.Master.Dto.Response;
using IDMS.Shared.Domain.Entities;
using IDMS.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace IDMS.Modules.Api.Master.Services.Impl
{
    public class MstModelService : IMstModelService
    {
        private readonly AppDbContext _context;

        public MstModelService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<ResMstModelDto> data, int total)> GetListAsync(ReqMstModelDto request)
        {
            var query = _context.Set<MstModel>()
                .Include(x => x.Type)
                .Where(x => x.DeletedAt == null)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                var keyword = request.Keyword.ToLower();
                query = query.Where(x => x.Name.ToLower().Contains(keyword) || x.Code.ToLower().Contains(keyword));
            }

            var total = await query.CountAsync();

            var data = await query
                .OrderBy(x => x.Code)
                .Skip((request.Page - 1) * request.Limit)
                .Take(request.Limit)
                .Select(x => new ResMstModelDto
                {
                    Id = x.Id,
                    TypeId = x.TypeId,
                    TypeName = x.Type.Name,
                    Code = x.Code,
                    Name = x.Name,
                    Year = x.Year,
                    Price = x.Price,
                    Stock = x.Stock,
                    IsActive = x.IsActive
                })
                .ToListAsync();

            return (data, total);
        }

        public async Task<ResMstModelDto> CreateAsync(ReqMstModelCreateDto request)
        {
            var isTypeExist = await _context.Set<MstType>().AnyAsync(x => x.Id == request.TypeId && x.DeletedAt == null);
            if (!isTypeExist) throw new BadRequestException("Type does not exist");

            var isCodeExist = await _context.Set<MstModel>().AnyAsync(x => x.Code.ToLower() == request.Code.ToLower() && x.DeletedAt == null);
            if (isCodeExist) throw new ConflictException("Code already exists");

            var entity = new MstModel
            {
                TypeId = request.TypeId,
                Code = request.Code,
                Name = request.Name,
                Year = request.Year,
                Price = request.Price,
                Stock = request.Stock,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Set<MstModel>().Add(entity);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(entity.Id);
        }

        public async Task<ResMstModelDto> UpdateAsync(int id, ReqMstModelUpdateDto request)
        {
            var entity = await _context.Set<MstModel>().FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
            if (entity == null) throw new NotFoundException("Model not found");

            var isTypeExist = await _context.Set<MstType>().AnyAsync(x => x.Id == request.TypeId && x.DeletedAt == null);
            if (!isTypeExist) throw new BadRequestException("Type does not exist");

            var isCodeExist = await _context.Set<MstModel>()
                .AnyAsync(x => x.Code.ToLower() == request.Code.ToLower() && x.Id != id && x.DeletedAt == null);
            if (isCodeExist) throw new ConflictException("Code already exists");

            entity.TypeId = request.TypeId;
            entity.Code = request.Code;
            entity.Name = request.Name;
            entity.Year = request.Year;
            entity.Price = request.Price;
            entity.Stock = request.Stock;
            entity.IsActive = request.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            // _context.Set<MstModel>().Update(entity);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(entity.Id);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var entity = await _context.Set<MstModel>().FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
            if (entity == null) throw new NotFoundException("Model not found");

            entity.IsActive = false;
            entity.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<ResMstModelDto> GetByIdAsync(int id)
        {
            var data = await _context.Set<MstModel>()
                .Include(x => x.Type)
                .FirstOrDefaultAsync(x => x.Id == id);

            return new ResMstModelDto
            {
                Id = data!.Id,
                TypeId = data.TypeId,
                TypeName = data.Type.Name,
                Code = data.Code,
                Name = data.Name,
                Year = data.Year,
                Price = data.Price,
                Stock = data.Stock,
                IsActive = data.IsActive
            };
        }

        public async Task<ResMstModelDto?> GetTypeByIdAsync(int id)
        {
            var data = await _context.MstModels.AsNoTracking()
            .Where(v => v.Id == id && v.DeletedAt == null)
            .Select(
                v => new ResMstModelDto
                {
                    Id = v.Id,
                    TypeId = v.TypeId,
                    TypeName = v.Type.Name,
                    Code = v.Code,
                    Name = v.Name,
                    IsActive = v.IsActive,
                    Year = v.Year,
                    Price = v.Price,
                    Stock = v.Stock
                }
            ).FirstOrDefaultAsync();

            return data;
        }
    }
}