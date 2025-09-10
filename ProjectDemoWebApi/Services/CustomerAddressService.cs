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
        //private readonly IDistanceCalculationService _distanceCalculationService;

        public CustomerAddressService(
            ICustomerAddressRepository customerAddressRepository,
            IMapper mapper )
            //IDistanceCalculationService distanceCalculationService)
        {
            _customerAddressRepository = customerAddressRepository;
            _mapper = mapper;
            //_distanceCalculationService = distanceCalculationService;
        }

        public async Task<ApiResponse<IEnumerable<CustomerAddressResponseDto>>> GetUserAddressesAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var addresses = await _customerAddressRepository.GetUserAddressesAsync(userId, cancellationToken);
                var addressDtos = _mapper.Map<IEnumerable<CustomerAddressResponseDto>>(addresses);

                return ApiResponse<IEnumerable<CustomerAddressResponseDto>>.Ok(addressDtos, "Successfully retrieved the list of addresses.");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CustomerAddressResponseDto>>.Fail($"Error retrieving the list of addresses: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CustomerAddressResponseDto>> CreateAddressAsync(string userId, CreateCustomerAddressDto createAddressDto, CancellationToken cancellationToken = default)
        {
            try
            {
                // Check if there are any existing addresses; if not, automatically set the new one as default.
                var existingAddresses = await _customerAddressRepository.GetUserAddressesAsync(userId, cancellationToken);
                var isFirstAddress = !existingAddresses.Any();

                var address = _mapper.Map<CustomerAddresses>(createAddressDto);
                address.UserId = userId;
                address.IsDefault = isFirstAddress || createAddressDto.IsDefault;
                address.CreatedDate = DateTime.UtcNow;

                // Calculate the distance from the shop using only FullAddress
                //var distance = await _distanceCalculationService.CalculateDistanceAsync(address.FullAddress, cancellationToken);
                Random random = new Random();
                decimal distance = (decimal)(random.NextDouble() * 19.0 + 1.0);

                //if (distance == null)
                //{
                //    return ApiResponse<CustomerAddressResponseDto>.Fail("Could not calculate the distance from the store. Please check the address and try again.", statusCode: 400);
                //}

                address.DistanceKm = distance;

                // If this address is set as default, unset the default status of all other addresses.
                if (address.IsDefault)
                {
                    await _customerAddressRepository.UnsetAllDefaultAddressesAsync(userId, cancellationToken);
                }

                var createdAddress = await _customerAddressRepository.CreateAddressAsync(address, cancellationToken);
                var addressDto = _mapper.Map<CustomerAddressResponseDto>(createdAddress);

                return ApiResponse<CustomerAddressResponseDto>.Ok(addressDto, "Address created successfully.", 201);
            }
            catch (Exception ex)
            {
                return ApiResponse<CustomerAddressResponseDto>.Fail($"Error creating the address: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CustomerAddressResponseDto?>> UpdateAddressAsync(string userId, int id, UpdateCustomerAddressDto updateAddressDto, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingAddress = await _customerAddressRepository.GetAddressByIdAsync(id, cancellationToken);

                if (existingAddress == null)
                {
                    return ApiResponse<CustomerAddressResponseDto?>.Fail("Address not found.", statusCode: 404);
                }

                if (existingAddress.UserId != userId)
                {
                    return ApiResponse<CustomerAddressResponseDto?>.Fail("You do not have permission to update this address.", statusCode: 403);
                }

                var shouldRecalculateDistance = false;

                // Update information
                if (!string.IsNullOrEmpty(updateAddressDto.AddressName))
                    existingAddress.AddressName = updateAddressDto.AddressName;

                if (!string.IsNullOrEmpty(updateAddressDto.FullName))
                    existingAddress.FullName = updateAddressDto.FullName;

                if (!string.IsNullOrEmpty(updateAddressDto.PhoneNumber))
                    existingAddress.PhoneNumber = updateAddressDto.PhoneNumber;

                if (!string.IsNullOrEmpty(updateAddressDto.FullAddress))
                {
                    existingAddress.FullAddress = updateAddressDto.FullAddress;
                    shouldRecalculateDistance = true;
                }

                // Recalculate distance if the address has changed
                //if (shouldRecalculateDistance)
                //{
                //    var distance = await _distanceCalculationService.CalculateDistanceAsync(existingAddress.FullAddress, cancellationToken);

                //    if (distance == null)
                //    {
                //        return ApiResponse<CustomerAddressResponseDto?>.Fail("Could not calculate the distance from the store. Please check the address.", statusCode: 400);
                //    }

                //    existingAddress.DistanceKm = distance.Value;
                //}
                //else if (updateAddressDto.DistanceKm.HasValue)
                //{
                //    existingAddress.DistanceKm = updateAddressDto.DistanceKm;
                //}

                if (updateAddressDto.IsActive.HasValue)
                    existingAddress.IsActive = updateAddressDto.IsActive.Value;

                // Handle default address
                if (updateAddressDto.IsDefault.HasValue && updateAddressDto.IsDefault.Value && !existingAddress.IsDefault)
                {
                    await _customerAddressRepository.UnsetAllDefaultAddressesAsync(userId, cancellationToken);
                    existingAddress.IsDefault = true;
                }

                var updatedAddress = await _customerAddressRepository.UpdateAddressAsync(existingAddress, cancellationToken);
                var addressDto = _mapper.Map<CustomerAddressResponseDto>(updatedAddress);

                return ApiResponse<CustomerAddressResponseDto?>.Ok(addressDto, "Address updated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<CustomerAddressResponseDto?>.Fail($"Error updating the address: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> SetDefaultAddressAsync(string userId, int addressId, CancellationToken cancellationToken = default)
        {
            try
            {
                var address = await _customerAddressRepository.GetAddressByIdAsync(addressId, cancellationToken);

                if (address == null)
                {
                    return ApiResponse<bool>.Fail("Address not found.", statusCode: 404);
                }

                if (address.UserId != userId)
                {
                    return ApiResponse<bool>.Fail("You do not have permission to perform this action on this address.", statusCode: 403);
                }

                if (!address.IsActive)
                {
                    return ApiResponse<bool>.Fail("An inactive address cannot be set as default.", statusCode: 400);
                }

                // Unset default status for all other addresses
                await _customerAddressRepository.UnsetAllDefaultAddressesAsync(userId, cancellationToken);

                // Set this address as the default
                address.IsDefault = true;
                await _customerAddressRepository.UpdateAddressAsync(address, cancellationToken);

                return ApiResponse<bool>.Ok(true, "Default address set successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail($"Error setting default address: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAddressAsync(string userId, int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var address = await _customerAddressRepository.GetAddressByIdAsync(id, cancellationToken);

                if (address == null)
                {
                    return ApiResponse<bool>.Fail("Address not found.", statusCode: 404);
                }

                if (address.UserId != userId)
                {
                    return ApiResponse<bool>.Fail("You do not have permission to delete this address.", statusCode: 403);
                }

                if (!address.IsActive)
                {
                    return ApiResponse<bool>.Fail("Address has already been deleted.", statusCode: 400);
                }

                // Soft delete: set IsActive = false
                address.IsActive = false;

                // If the default address is deleted, automatically set another address as default
                if (address.IsDefault)
                {
                    address.IsDefault = false;
                    await _customerAddressRepository.UpdateAddressAsync(address, cancellationToken);

                    // Find another active address to set as default
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

                return ApiResponse<bool>.Ok(true, "Address deleted successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail($"Error deleting the address: {ex.Message}");
            }
        }
    }
}