using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface ICustomerAddressRepository : IBaseRepository<CustomerAddresses>
    {
        Task<IEnumerable<CustomerAddresses>> GetUserAddressesAsync(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<CustomerAddresses>> GetActiveUserAddressesAsync(string userId, CancellationToken cancellationToken = default);
        Task<CustomerAddresses?> GetAddressByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<CustomerAddresses> CreateAddressAsync(CustomerAddresses address, CancellationToken cancellationToken = default);
        Task<CustomerAddresses> UpdateAddressAsync(CustomerAddresses address, CancellationToken cancellationToken = default);
        Task UnsetAllDefaultAddressesAsync(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<CustomerAddresses>> GetAddressesByDistanceAsync(decimal maxDistance, CancellationToken cancellationToken = default);
    }
}