using Microsoft.Extensions.Options;
using ProjectDemoWebApi.DTOs.SePay;
using ProjectDemoWebApi.DTOs.Shared;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Services.Interface;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjectDemoWebApi.Services
{
    public class SePayService : ISePayService
    {
        private readonly HttpClient _httpClient;
        private readonly SePaySettings _sePaySettings;

        public SePayService(
            HttpClient httpClient,
            IOptions<SePaySettings> sePaySettings)
        {
            _httpClient = httpClient;
            _sePaySettings = sePaySettings.Value;
        }

        /// <summary>
        /// Retrieves the list of transactions from SePay API
        /// </summary>
        public async Task<SePayTransactionResponse> GetTransactionsAsync()
        {
            try
            {
                var url = $"/userapi/transactions/list?account_number={_sePaySettings.AccountNumber}&limit=50";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                var transactionResponse = JsonSerializer.Deserialize<SePayTransactionResponse>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return transactionResponse ?? new SePayTransactionResponse();
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (JsonException)
            {
                throw;
            }
        }

        /// <summary>
        /// Finds a specific transaction by order number and amount with retry mechanism
        /// </summary>
        public async Task<ApiResponse<SePayTransaction?>> FindTransactionAsync(string orderNumber, decimal amount)
        {
            const int maxRetries = 10;
            const int delaySeconds = 3;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    var transactionResponse = await GetTransactionsAsync();
                    if (transactionResponse == null || !transactionResponse.Messages.Success)
                    {
                        if (attempt == maxRetries)
                        {
                            return ApiResponse<SePayTransaction?>.Fail(
                                "Failed to retrieve transactions from SePay API after all retry attempts",
                                new List<string> { "Transaction response was null or failed", $"Attempted {maxRetries} times" },
                                500
                            );
                        }
                        await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                        continue;
                    }

                    var transaction = transactionResponse.Transactions.FirstOrDefault(t =>
                        t.TransactionContent.Contains(orderNumber) &&
                        t.AmountIn == amount);

                    if (transaction != null)
                    {
                        return ApiResponse<SePayTransaction?>.Ok(
                            transaction,
                            $"Transaction found successfully on attempt {attempt}",
                            200
                        );
                    }

                    if (attempt == maxRetries)
                    {
                        return ApiResponse<SePayTransaction?>.Fail(
                            "Transaction not found after all retry attempts",
                            new List<string> {
                                $"No transaction found for order number: {orderNumber} with amount: {amount}",
                                $"Searched {maxRetries} times over {maxRetries * delaySeconds} seconds"
                            },
                            404
                        );
                    }
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                }
                catch (Exception ex)
                {
                    if (attempt == maxRetries)
                    {
                        return ApiResponse<SePayTransaction?>.Fail(
                            "Error occurred while finding transaction after all retry attempts",
                            new List<string> { ex.Message, $"Failed after {maxRetries} attempts" },
                            500
                        );
                    }
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                }
            }
            return ApiResponse<SePayTransaction?>.Fail(
                "Unexpected error in transaction search",
                new List<string> { "Transaction search completed unexpectedly" },
                500
            );
        }
    }
}