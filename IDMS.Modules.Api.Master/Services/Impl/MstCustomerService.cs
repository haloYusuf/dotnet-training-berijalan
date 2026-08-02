using IDMS.Infrastructure.Data;
using IDMS.Modules.Api.Master.Dto.Request.MstCustomer;
using IDMS.Modules.Api.Master.Dto.Response;
using IDMS.Shared.Domain.Entities;
using IDMS.Shared.Exceptions;
using IDMS.Shared.Utils;
using Microsoft.EntityFrameworkCore;

namespace IDMS.Modules.Api.Master.Services.Impl
{
    public class MstCustomerService : IMstCustomerService
    {
        private readonly AppDbContext _context;
        
        private readonly ICurrentUserServices _user;

        public MstCustomerService(AppDbContext context, ICurrentUserServices user)
        {
            _context = context;
            _user = user;
        }

        public async Task<(IEnumerable<ResMstCustomerDto> data, int total)> GetListAsync(ReqMstCustomerDto request)
        {
            var query = _context.Set<MstCustomer>()
                .Where(x => x.DeletedAt == null)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                var keyword = request.Keyword.ToLower();
                // Pencarian berdasarkan Nama atau NIK
                query = query.Where(x => x.FullName.ToLower().Contains(keyword) || x.Nik.Contains(keyword));
            }

            var total = await query.CountAsync();

            var data = await query
                .OrderBy(x => x.FullName)
                .Skip((request.Page - 1) * request.Limit)
                .Take(request.Limit)
                .Select(x => new ResMstCustomerDto
                {
                    Id = x.Id,
                    Nik = x.Nik,
                    FullName = x.FullName,
                    BirthDate = x.BirthDate,
                    Phone = x.Phone,
                    Email = x.Email,
                    Address = x.Address,
                    IsActive = x.IsActive
                })
                .ToListAsync();

            return (data, total);
        }

        public async Task<ResMstCustomerDto> CreateAsync(ReqMstCustomerCreateDto request)
        {
            // Validasi NIK unik
            var isNikExist = await _context.Set<MstCustomer>().AnyAsync(x => x.Nik == request.Nik && x.DeletedAt == null);
            if (isNikExist) throw new ConflictException("NIK already exists");

            var isEmailExist = await _context.Set<MstCustomer>().AnyAsync(x => x.Email.ToLower() == request.Email.ToLower());
            if (isEmailExist) throw new ConflictException("Email already exists");

            var entity = new MstCustomer
            {
                Nik = request.Nik,
                FullName = request.FullName,
                BirthDate = request.BirthDate,
                Phone = request.Phone,
                Email = request.Email,
                Address = request.Address,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy= _user.GetFullName(),
            };

            _context.Set<MstCustomer>().Add(entity);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(entity.Id);
        }

        public async Task<ResMstCustomerDto> UpdateAsync(int id, ReqMstCustomerUpdateDto request)
        {
            var entity = await _context.Set<MstCustomer>().FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
            if (entity == null) throw new NotFoundException("Customer not found");

            var isNikExist = await _context.Set<MstCustomer>()
                .AnyAsync(x => x.Nik == request.Nik && x.Id != id && x.DeletedAt == null);
            if (isNikExist) throw new ConflictException("NIK already exists");

            entity.Nik = request.Nik;
            entity.FullName = request.FullName;
            if (request.BirthDate.Kind == DateTimeKind.Unspecified)
            {
                entity.BirthDate = DateTime.SpecifyKind(request.BirthDate, DateTimeKind.Utc);
            }
            else
            {
                entity.BirthDate = request.BirthDate.ToUniversalTime();
            }
            entity.Phone = request.Phone;
            entity.Email = request.Email;
            entity.Address = request.Address;
            entity.IsActive = request.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = _user.GetFullName();

            await _context.SaveChangesAsync();

            return await GetByIdAsync(entity.Id);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var entity = await _context.Set<MstCustomer>().FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
            if (entity == null) throw new NotFoundException("Customer not found");

            entity.IsActive = false;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedBy = _user.GetFullName();

            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<ResMstCustomerDto> GetByIdAsync(int id)
        {
            var data = await _context.Set<MstCustomer>().FirstOrDefaultAsync(x => x.Id == id);

            return new ResMstCustomerDto
            {
                Id = data!.Id,
                Nik = data.Nik,
                FullName = data.FullName,
                BirthDate = data.BirthDate,
                Phone = data.Phone,
                Email = data.Email,
                Address = data.Address,
                IsActive = data.IsActive
            };
        }

        public async Task<ResMstCustomerDto?> GetCustomerByIdAsync(int id)
        {
            var data = await _context.MstCustomers.AsNoTracking()
        .Where(v => v.Id == id && v.DeletedAt == null)
        .Select(v => new ResMstCustomerDto
        {
            Id = v.Id,
            Nik = v.Nik,
            FullName = v.FullName,
            BirthDate = v.BirthDate,
            Phone = v.Phone,
            Email = v.Email,
            Address = v.Address,
            IsActive = v.IsActive
        })
        .FirstOrDefaultAsync();

            return data;
        }
    }
}