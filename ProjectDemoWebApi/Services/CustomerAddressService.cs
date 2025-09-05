using AutoMapper;
using ProjectDemoWebApi.DTOs.CustomerAddress;
using ProjectDemoWebApi.DTOs.Shared;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Services
{
    public class CustomerAddressService : ICustomerAddressService
    {
        private readonly ICustomerAddressRepository _customerAddressRepository;
        private readonly IMapper _mapper;

        public CustomerAddressService(ICustomerAddressRepository customerAddressRepository, IMapper mapper)
        {
            _customerAddressRepository = customerAddressRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<CustomerAddressResponseDto>>> GetUserAddressesAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var addresses = await _customerAddressRepository.GetUserAddressesAsync(userId, cancellationToken);
                var addressDtos = _mapper.Map<IEnumerable<CustomerAddressResponseDto>>(addresses);
                
                return ApiResponse<IEnumerable<CustomerAddressResponseDto>>.Ok(addressDtos, "L?y danh sách ??a ch? thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CustomerAddressResponseDto>>.Fail($"L?i khi l?y danh sách ??a ch?: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CustomerAddressResponseDto>> CreateAddressAsync(string userId, CreateCustomerAddressDto createAddressDto, CancellationToken cancellationToken = default)
        {
            try
            {
                // Ki?m tra xem có ??a ch? nào ch?a, n?u ch?a thì t? ??ng ??t là m?c ??nh
                var existingAddresses = await _customerAddressRepository.GetUserAddressesAsync(userId, cancellationToken);
                var isFirstAddress = !existingAddresses.Any();

                var address = _mapper.Map<CustomerAddresses>(createAddressDto);
                address.UserId = userId;
                address.IsDefault = isFirstAddress || createAddressDto.IsDefault;
                address.CreatedDate = DateTime.UtcNow;

                // N?u ??t là m?c ??nh, b? default c?a các ??a ch? khác
                if (address.IsDefault)
                {
                    await _customerAddressRepository.UnsetAllDefaultAddressesAsync(userId, cancellationToken);
                }

                var createdAddress = await _customerAddressRepository.CreateAddressAsync(address, cancellationToken);
                var addressDto = _mapper.Map<CustomerAddressResponseDto>(createdAddress);
                
                return ApiResponse<CustomerAddressResponseDto>.Ok(addressDto, "T?o ??a ch? thành công", 201);
            }
            catch (Exception ex)
            {
                return ApiResponse<CustomerAddressResponseDto>.Fail($"L?i khi t?o ??a ch?: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CustomerAddressResponseDto?>> UpdateAddressAsync(string userId, int id, UpdateCustomerAddressDto updateAddressDto, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingAddress = await _customerAddressRepository.GetAddressByIdAsync(id, cancellationToken);
                
                if (existingAddress == null)
                {
                    return ApiResponse<CustomerAddressResponseDto?>.Fail("Không tìm th?y ??a ch?", statusCode: 404);
                }

                if (existingAddress.UserId != userId)
                {
                    return ApiResponse<CustomerAddressResponseDto?>.Fail("B?n không có quy?n c?p nh?t ??a ch? này", statusCode: 403);
                }

                // C?p nh?t thông tin
                if (!string.IsNullOrEmpty(updateAddressDto.AddressName))
                    existingAddress.AddressName = updateAddressDto.AddressName;
                
                if (!string.IsNullOrEmpty(updateAddressDto.FullAddress))
                    existingAddress.FullAddress = updateAddressDto.FullAddress;
                
                if (updateAddressDto.District != null)
                    existingAddress.District = updateAddressDto.District;
                
                if (updateAddressDto.City != null)
                    existingAddress.City = updateAddressDto.City;
                
                if (updateAddressDto.PostalCode != null)
                    existingAddress.PostalCode = updateAddressDto.PostalCode;
                
                if (updateAddressDto.DistanceKm.HasValue)
                    existingAddress.DistanceKm = updateAddressDto.DistanceKm;

                if (updateAddressDto.IsActive.HasValue)
                    existingAddress.IsActive = updateAddressDto.IsActive.Value;

                // X? lý default address
                if (updateAddressDto.IsDefault.HasValue && updateAddressDto.IsDefault.Value && !existingAddress.IsDefault)
                {
                    await _customerAddressRepository.UnsetAllDefaultAddressesAsync(userId, cancellationToken);
                    existingAddress.IsDefault = true;
                }

                var updatedAddress = await _customerAddressRepository.UpdateAddressAsync(existingAddress, cancellationToken);
                var addressDto = _mapper.Map<CustomerAddressResponseDto>(updatedAddress);
                
                return ApiResponse<CustomerAddressResponseDto?>.Ok(addressDto, "C?p nh?t ??a ch? thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<CustomerAddressResponseDto?>.Fail($"L?i khi c?p nh?t ??a ch?: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> SetDefaultAddressAsync(string userId, int addressId, CancellationToken cancellationToken = default)
        {
            try
            {
                var address = await _customerAddressRepository.GetAddressByIdAsync(addressId, cancellationToken);
                
                if (address == null)
                {
                    return ApiResponse<bool>.Fail("Không tìm th?y ??a ch?", statusCode: 404);
                }

                if (address.UserId != userId)
                {
                    return ApiResponse<bool>.Fail("B?n không có quy?n thao tác v?i ??a ch? này", statusCode: 403);
                }

                if (!address.IsActive)
                {
                    return ApiResponse<bool>.Fail("Không th? ??t ??a ch? không ho?t ??ng làm m?c ??nh", statusCode: 400);
                }

                // B? default c?a t?t c? ??a ch? khác
                await _customerAddressRepository.UnsetAllDefaultAddressesAsync(userId, cancellationToken);
                
                // ??t ??a ch? này làm m?c ??nh
                address.IsDefault = true;
                await _customerAddressRepository.UpdateAddressAsync(address, cancellationToken);
                
                return ApiResponse<bool>.Ok(true, "??t ??a ch? m?c ??nh thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail($"L?i khi ??t ??a ch? m?c ??nh: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAddressAsync(string userId, int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var address = await _customerAddressRepository.GetAddressByIdAsync(id, cancellationToken);
                
                if (address == null)
                {
                    return ApiResponse<bool>.Fail("Không tìm th?y ??a ch?", statusCode: 404);
                }

                if (address.UserId != userId)
                {
                    return ApiResponse<bool>.Fail("B?n không có quy?n xóa ??a ch? này", statusCode: 403);
                }

                if (!address.IsActive)
                {
                    return ApiResponse<bool>.Fail("??a ch? ?ã ???c xóa tr??c ?ó", statusCode: 400);
                }

                // Soft delete: ??t IsActive = false
                address.IsActive = false;
                
                // N?u xóa ??a ch? m?c ??nh, t? ??ng ??t ??a ch? khác làm m?c ??nh
                if (address.IsDefault)
                {
                    address.IsDefault = false;
                    await _customerAddressRepository.UpdateAddressAsync(address, cancellationToken);
                    
                    // Tìm ??a ch? active khác ?? ??t làm m?c ??nh
                    var remainingAddresses = await _customerAddressRepository.GetActiveUserAddressesAsync(userId, cancellationToken);
                    var firstActive = remainingAddresses.FirstOrDefault(a => a.Id != id);
                    if (firstActive != null)
                    {
                        firstActive.IsDefault = true;
                        await _customerAddressRepository.UpdateAddressAsync(firstActive, cancellationToken);
                    }
                }
                else
                {
                    await _customerAddressRepository.UpdateAddressAsync(address, cancellationToken);
                }

                return ApiResponse<bool>.Ok(true, "Xóa ??a ch? thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail($"L?i khi xóa ??a ch?: {ex.Message}");
            }
        }
    }
}