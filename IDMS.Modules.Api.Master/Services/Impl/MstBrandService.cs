using IDMS.Infrastructure.Data;
using IDMS.Modules.Api.Master.Dto.Request;
using IDMS.Modules.Api.Master.Dto.Response;
using IDMS.Shared.Domain.Entities;
using IDMS.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace IDMS.Modules.Api.Master.Services.Impl;

public class MstBrandService : IMstBrandService
{
    private readonly AppDbContext _db;

    public MstBrandService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(List<ResMstBrandDto> Data, int Total)> GetListAsync(ReqMstBrandDto request)
    {
        var query = _db.MstBrands.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(e =>
                e.Code.ToLower().Contains(search) ||
                e.Name.ToLower().Contains(search));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(e => e.Code)
            .Skip((request.Page - 1) * request.Limit)
            .Take(request.Limit)
            .Select(e => new ResMstBrandDto
            {
                Id = e.Id,
                Code = e.Code,
                Name = e.Name,
                IsActive = e.IsActive
            })
            .ToListAsync();

        return (items, total);
    }

    public async Task<ResMstBrandDto> CreateAsync(ReqMstBrandCreateDto request)
    {
        var exists = await _db.MstBrands.AnyAsync(e => EF.Functions.ILike(e.Code, request.Code));
        if (exists)
            throw new ConflictException($"Code '{request.Code}' already exists");

        var entity = new MstBrand
        {
            Code = request.Code,
            Name = request.Name,
            IsActive = true
        };

        _db.MstBrands.Add(entity);
        await _db.SaveChangesAsync();

        return new ResMstBrandDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            IsActive = entity.IsActive
        };
    }

    public async Task<ResMstBrandDto> UpdateAsync(int id, ReqMstBrandUpdateDto request)
    {
        var entity = await _db.MstBrands.FindAsync(id);
        if (entity == null)
            throw new NotFoundException($"Brand with id {id} not found");

        var exists = await _db.MstBrands.AnyAsync(e => EF.Functions.ILike(e.Code, request.Code) && e.Id != id);
        if (exists)
            throw new ConflictException($"Code '{request.Code}' already exists");

        entity.Code = request.Code;
        entity.Name = request.Name;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return new ResMstBrandDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            IsActive = entity.IsActive
        };
    }

    public async Task SoftDeleteAsync(int id)
    {
        var entity = await _db.MstBrands.FindAsync(id);
        if (entity == null)
            throw new NotFoundException($"Brand with id {id} not found");

        entity.DeletedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
    }
}
