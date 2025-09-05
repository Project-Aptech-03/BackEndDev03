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
                
                return ApiResponse<IEnumerable<CustomerAddressResponseDto>>.Ok(addressDtos, "L?y danh s�ch ??a ch? th�nh c�ng");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CustomerAddressResponseDto>>.Fail($"L?i khi l?y danh s�ch ??a ch?: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CustomerAddressResponseDto>> CreateAddressAsync(string userId, CreateCustomerAddressDto createAddressDto, CancellationToken cancellationToken = default)
        {
            try
            {
                // Ki?m tra xem c� ??a ch? n�o ch?a, n?u ch?a th� t? ??ng ??t l� m?c ??nh
                var existingAddresses = await _customerAddressRepository.GetUserAddressesAsync(userId, cancellationToken);
                var isFirstAddress = !existingAddresses.Any();

                var address = _mapper.Map<CustomerAddresses>(createAddressDto);
                address.UserId = userId;
                address.IsDefault = isFirstAddress || createAddressDto.IsDefault;
                address.CreatedDate = DateTime.UtcNow;

                // N?u ??t l� m?c ??nh, b? default c?a c�c ??a ch? kh�c
                if (address.IsDefault)
                {
                    await _customerAddressRepository.UnsetAllDefaultAddressesAsync(userId, cancellationToken);
                }

                var createdAddress = await _customerAddressRepository.CreateAddressAsync(address, cancellationToken);
                var addressDto = _mapper.Map<CustomerAddressResponseDto>(createdAddress);
                
                return ApiResponse<CustomerAddressResponseDto>.Ok(addressDto, "T?o ??a ch? th�nh c�ng", 201);
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
                    return ApiResponse<CustomerAddressResponseDto?>.Fail("Kh�ng t�m th?y ??a ch?", statusCode: 404);
                }

                if (existingAddress.UserId != userId)
                {
                    return ApiResponse<CustomerAddressResponseDto?>.Fail("B?n kh�ng c� quy?n c?p nh?t ??a ch? n�y", statusCode: 403);
                }

                // C?p nh?t th�ng tin
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

                // X? l� default address
                if (updateAddressDto.IsDefault.HasValue && updateAddressDto.IsDefault.Value && !existingAddress.IsDefault)
                {
                    await _customerAddressRepository.UnsetAllDefaultAddressesAsync(userId, cancellationToken);
                    existingAddress.IsDefault = true;
                }

                var updatedAddress = await _customerAddressRepository.UpdateAddressAsync(existingAddress, cancellationToken);
                var addressDto = _mapper.Map<CustomerAddressResponseDto>(updatedAddress);
                
                return ApiResponse<CustomerAddressResponseDto?>.Ok(addressDto, "C?p nh?t ??a ch? th�nh c�ng");
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
                    return ApiResponse<bool>.Fail("Kh�ng t�m th?y ??a ch?", statusCode: 404);
                }

                if (address.UserId != userId)
                {
                    return ApiResponse<bool>.Fail("B?n kh�ng c� quy?n thao t�c v?i ??a ch? n�y", statusCode: 403);
                }

                if (!address.IsActive)
                {
                    return ApiResponse<bool>.Fail("Kh�ng th? ??t ??a ch? kh�ng ho?t ??ng l�m m?c ??nh", statusCode: 400);
                }

                // B? default c?a t?t c? ??a ch? kh�c
                await _customerAddressRepository.UnsetAllDefaultAddressesAsync(userId, cancellationToken);
                
                // ??t ??a ch? n�y l�m m?c ??nh
                address.IsDefault = true;
                await _customerAddressRepository.UpdateAddressAsync(address, cancellationToken);
                
                return ApiResponse<bool>.Ok(true, "??t ??a ch? m?c ??nh th�nh c�ng");
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
                    return ApiResponse<bool>.Fail("Kh�ng t�m th?y ??a ch?", statusCode: 404);
                }

                if (address.UserId != userId)
                {
                    return ApiResponse<bool>.Fail("B?n kh�ng c� quy?n x�a ??a ch? n�y", statusCode: 403);
                }

                if (!address.IsActive)
                {
                    return ApiResponse<bool>.Fail("??a ch? ?� ???c x�a tr??c ?�", statusCode: 400);
                }

                // Soft delete: ??t IsActive = false
                address.IsActive = false;
                
                // N?u x�a ??a ch? m?c ??nh, t? ??ng ??t ??a ch? kh�c l�m m?c ??nh
                if (address.IsDefault)
                {
                    address.IsDefault = false;
                    await _customerAddressRepository.UpdateAddressAsync(address, cancellationToken);
                    
                    // T�m ??a ch? active kh�c ?? ??t l�m m?c ??nh
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

                return ApiResponse<bool>.Ok(true, "X�a ??a ch? th�nh c�ng");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail($"L?i khi x�a ??a ch?: {ex.Message}");
            }
        }
    }
}