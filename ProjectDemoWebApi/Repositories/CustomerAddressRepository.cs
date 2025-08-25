using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class CustomerAddressRepository : BaseRepository<CustomerAddresses>, ICustomerAddressRepository
    {
        public CustomerAddressRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CustomerAddresses>> GetUserAddressesAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Where(ca => ca.UserId == userId)
                .OrderByDescending(ca => ca.IsDefault)
                .ThenBy(ca => ca.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<CustomerAddresses?> GetUserDefaultAddressAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(ca => ca.UserId == userId && ca.IsDefault && ca.IsActive, cancellationToken);
        }

        public async Task<IEnumerable<CustomerAddresses>> GetActiveUserAddressesAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Where(ca => ca.UserId == userId && ca.IsActive)
                .OrderByDescending(ca => ca.IsDefault)
                .ThenBy(ca => ca.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task SetDefaultAddressAsync(string userId, int addressId, CancellationToken cancellationToken = default)
        {
            // First, remove default from all user addresses
            var userAddresses = await _dbSet
                .Where(ca => ca.UserId == userId)
                .ToListAsync(cancellationToken);

            foreach (var address in userAddresses)
            {
                address.IsDefault = address.Id == addressId;
            }

            _dbSet.UpdateRange(userAddresses);
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