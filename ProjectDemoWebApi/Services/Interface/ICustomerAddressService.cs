using ProjectDemoWebApi.DTOs.CustomerAddress;
using ProjectDemoWebApi.DTOs.Shared;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface ICustomerAddressService
    {
        Task<ApiResponse<IEnumerable<CustomerAddressResponseDto>>> GetUserAddressesAsync(string userId, CancellationToken cancellationToken = default);
        Task<ApiResponse<CustomerAddressResponseDto>> CreateAddressAsync(string userId, CreateCustomerAddressDto createAddressDto, CancellationToken cancellationToken = default);
        Task<ApiResponse<CustomerAddressResponseDto?>> UpdateAddressAsync(string userId, int id, UpdateCustomerAddressDto updateAddressDto, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> SetDefaultAddressAsync(string userId, int addressId, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> DeleteAddressAsync(string userId, int id, CancellationToken cancellationToken = default);
    }
}