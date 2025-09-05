using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class CustomerAddressRepository : BaseRepository<CustomerAddresses>, ICustomerAddressRepository
    {
        public CustomerAddressRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CustomerAddresses>> GetUserAddressesAsync(string userId, CancellationToken cancellationToken = default)
        {
            // Ch? l?y ??a ch? ?ang active cho user thông th??ng, s?p x?p ??a ch? m?c ??nh lên ??u
            return await _dbSet.AsNoTracking()
                .Where(ca => ca.UserId == userId && ca.IsActive)
                .OrderByDescending(ca => ca.IsDefault)
                .ThenBy(ca => ca.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<CustomerAddresses>> GetActiveUserAddressesAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Where(ca => ca.UserId == userId && ca.IsActive)
                .OrderByDescending(ca => ca.IsDefault)
                .ThenBy(ca => ca.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<CustomerAddresses?> GetAddressByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(ca => ca.Id == id, cancellationToken);
        }

        public async Task<CustomerAddresses> CreateAddressAsync(CustomerAddresses address, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(address, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return address;
        }

        public async Task<CustomerAddresses> UpdateAddressAsync(CustomerAddresses address, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(address);
            await _context.SaveChangesAsync(cancellationToken);
            return address;
        }

        public async Task UnsetAllDefaultAddressesAsync(string userId, CancellationToken cancellationToken = default)
        {
            var userAddresses = await _dbSet
                .Where(ca => ca.UserId == userId && ca.IsDefault)
                .ToListAsync(cancellationToken);

            foreach (var address in userAddresses)
            {
                address.IsDefault = false;
            }

            if (userAddresses.Any())
            {
                _dbSet.UpdateRange(userAddresses);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<CustomerAddresses>> GetAddressesByDistanceAsync(decimal maxDistance, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Where(ca => ca.IsActive && ca.DistanceKm.HasValue && ca.DistanceKm <= maxDistance)
                .OrderBy(ca => ca.DistanceKm)
                .ToListAsync(cancellationToken);
        }
    }
}