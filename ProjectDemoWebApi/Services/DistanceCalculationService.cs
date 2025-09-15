using Microsoft.Extensions.Options;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Services.Interface;
using System.Text.Json;

namespace ProjectDemoWebApi.Services
{
    public class DistanceCalculationService : IDistanceCalculationService
    {
        private readonly HttpClient _httpClient;
        private readonly ShopSettings _shopSettings;
        private readonly GoogleMapsSettings _googleMapsSettings;
        private readonly ILogger<DistanceCalculationService> _logger;

        public DistanceCalculationService(
            HttpClient httpClient, 
            IOptions<ShopSettings> shopSettings,
            IOptions<GoogleMapsSettings> googleMapsSettings,
            ILogger<DistanceCalculationService> logger)
        {
            _httpClient = httpClient;
            _shopSettings = shopSettings.Value;
            _googleMapsSettings = googleMapsSettings.Value;
            _logger = logger;
        }

        public async Task<decimal?> CalculateDistanceAsync(string customerAddress, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(_googleMapsSettings.ApiKey) || 
                    _googleMapsSettings.ApiKey == "YOUR_GOOGLE_MAPS_API_KEY_HERE")
                {
                    _logger.LogError("Google Maps API key not configured");
                    return null;
                }

                var shopLocation = $"{_shopSettings.Latitude},{_shopSettings.Longitude}";
                var encodedCustomerAddress = Uri.EscapeDataString(customerAddress);
                
                var url = $"{_googleMapsSettings.DistanceMatrixUrl}?" +
                         $"origins={shopLocation}&" +
                         $"destinations={encodedCustomerAddress}&" +
                         $"units=metric&" +
                         $"mode=driving&" +
                         $"language=vi&" +
                         $"key={_googleMapsSettings.ApiKey}";

                _logger.LogInformation("Calling Google Distance Matrix API with URL: {Url}", url.Replace(_googleMapsSettings.ApiKey, "***"));
                _logger.LogInformation("Shop location: {ShopLocation}", shopLocation);
                _logger.LogInformation("Customer address: {CustomerAddress}", customerAddress);

                var response = await _httpClient.GetStringAsync(url, cancellationToken);
                _logger.LogInformation("Google API Response: {Response}", response);
                
                using var doc = JsonDocument.Parse(response);
                var root = doc.RootElement;
                
                if (!root.TryGetProperty("status", out var statusElement))
                {
                    _logger.LogError("No status property in Google API response");
                    return null;
                }

                var status = statusElement.GetString();
                _logger.LogInformation("Google API Status: {Status}", status);

                if (status != "OK")
                {
                    if (root.TryGetProperty("error_message", out var errorMessageElement))
                    {
                        var errorMessage = errorMessageElement.GetString();
                        _logger.LogError("Google Distance Matrix API error: {ErrorMessage}", errorMessage);
                    }
                    return null;
                }

                if (root.TryGetProperty("rows", out var rowsElement) && 
                    rowsElement.GetArrayLength() > 0)
                {
                    var firstRow = rowsElement[0];
                    if (firstRow.TryGetProperty("elements", out var elementsElement) && 
                        elementsElement.GetArrayLength() > 0)
                    {
                        var firstElement = elementsElement[0];
                        
                        if (firstElement.TryGetProperty("status", out var elementStatus))
                        {
                            var elementStatusValue = elementStatus.GetString();
                            _logger.LogInformation("Element status: {ElementStatus}", elementStatusValue);
                            
                            if (elementStatusValue == "OK")
                            {
                                if (firstElement.TryGetProperty("distance", out var distanceElement) &&
                                    distanceElement.TryGetProperty("value", out var valueElement))
                                {
                                    var distanceMeters = valueElement.GetInt32();
                                    var distanceKm = (decimal)distanceMeters / 1000m;
                                    _logger.LogInformation("Distance calculated successfully: {DistanceKm} km", distanceKm);
                                    return Math.Round(distanceKm, 2);
                                }
                                else
                                {
                                    _logger.LogError("Distance property not found in element");
                                }
                            }
                            else
                            {
                                _logger.LogError("Element status is not OK: {ElementStatus}", elementStatusValue);
                                if (firstElement.TryGetProperty("error_message", out var elementErrorMessage))
                                {
                                    _logger.LogError("Element error message: {ElementErrorMessage}", elementErrorMessage.GetString());
                                }
                            }
                        }
                        else
                        {
                            _logger.LogError("Element status property not found");
                        }
                    }
                    else
                    {
                        _logger.LogError("No elements found in first row");
                    }
                }
                else
                {
                    _logger.LogError("No rows found in response");
                }

                return null;
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "HTTP error when calling Google Maps API for address: {Address}", customerAddress);
                return null;
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "JSON parsing error for Google Maps API response for address: {Address}", customerAddress);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error calculating distance for address: {Address}", customerAddress);
                return null;
            }
        }
    }
}