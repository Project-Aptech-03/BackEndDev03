using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface ICustomerAddressRepository : IBaseRepository<CustomerAddresses>
    {
        Task<IEnumerable<CustomerAddresses>> GetUserAddressesAsync(string userId, CancellationToken cancellationToken = default);
        Task<CustomerAddresses?> GetUserDefaultAddressAsync(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<CustomerAddresses>> GetActiveUserAddressesAsync(string userId, CancellationToken cancellationToken = default);
        Task SetDefaultAddressAsync(string userId, int addressId, CancellationToken cancellationToken = default);
        Task<IEnumerable<CustomerAddresses>> GetAddressesByDistanceAsync(decimal maxDistance, CancellationToken cancellationToken = default);
    }
}