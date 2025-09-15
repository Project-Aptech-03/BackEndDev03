using ProjectDemoWebApi.DTOs.SePay;
using ProjectDemoWebApi.DTOs.Shared;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface ISePayService
    {
        /// <summary>
        /// Retrieves the list of transactions from SePay API
        /// </summary>
        Task<SePayTransactionResponse> GetTransactionsAsync();

        /// <summary>
        /// Finds a specific transaction by order number and amount
        /// </summary>
        Task<ApiResponse<SePayTransaction?>> FindTransactionAsync(string orderNumber, decimal amount);
    }
}