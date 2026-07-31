using IDMS.Infrastructure.Data;
using IDMS.Modules.Api.Master.Dto.Request.MstBrand;
using IDMS.Modules.Api.Master.Dto.Response;
using IDMS.Shared.Domain.Entities;
using IDMS.Shared.Exceptions;
using IDMS.Shared.Utils;
using Microsoft.EntityFrameworkCore;

namespace IDMS.Modules.Api.Master.Services.Impl;

public class MstBrandService : IMstBrandService
{
    private readonly AppDbContext _db;

    private readonly ICurrentUserServices _user;

    public MstBrandService(AppDbContext db, ICurrentUserServices user)
    {
        _db = db;
        _user = user;
    }

    public async Task<(List<ResMstBrandDto> Data, int Total)> GetListAsync(ReqMstBrandDto request)
    {
        var query = _db.MstBrands.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var search = request.Keyword.ToLower();
            query = query.Where(e =>
                e.Code.ToLower().Contains(search) ||
                e.Name.ToLower().Contains(search));
        }

        var total = await query.Where(x => x.DeletedAt == null && x.DeletedBy == null).CountAsync();

        var items = await query
            .OrderBy(e => e.Code)
            .Skip((request.Page - 1) * request.Limit)
            .Take(request.Limit)
            .Where(x => x.DeletedAt == null && x.DeletedBy == null)
            .Select(e => new ResMstBrandDto
            {
                Id = e.Id,
                Code = e.Code,
                Name = e.Name,
                IsActive = e.IsActive,
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
            IsActive = true,
            CreatedBy = _user.GetFullName(),
            CreatedAt = DateTime.UtcNow,
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
        var brand = await _db.MstBrands
        .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null) ?? throw new NotFoundException($"Brand with id {id} not found");

        var exists = await _db.MstBrands.AnyAsync(e => EF.Functions.ILike(e.Code, request.Code) && e.Id != id);
        if (exists)
            throw new ConflictException($"Code '{request.Code}' already exists");

        brand.Code = request.Code;
        brand.Name = request.Name;
        brand.UpdatedBy = _user.GetFullName();
        brand.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return new ResMstBrandDto
        {
            Id = brand.Id,
            Code = brand.Code,
            Name = brand.Name,
            IsActive = brand.IsActive
        };
    }

    public async Task SoftDeleteAsync(int id)
    {
        var entity = await _db.MstBrands.FindAsync(id);
        if (entity == null)
            throw new NotFoundException($"Brand with id {id} not found");

        entity.DeletedAt = DateTime.UtcNow;
        entity.DeletedBy = _user.GetFullName();

        await _db.SaveChangesAsync();
    }

    public async Task<ResMstBrandDto?> GetBrandByIdAsync(int id)
    {
        var data = await _db.MstBrands.AsNoTracking()
        .Where(v => v.Id == id && v.DeletedAt == null)
        .Select(v => new ResMstBrandDto
        {
            Id = v.Id,
            Code = v.Code,
            Name = v.Name,
            IsActive = v.IsActive
        })
        .FirstOrDefaultAsync();

        return data;
        // throw new NotImplementedException();
    }
}
