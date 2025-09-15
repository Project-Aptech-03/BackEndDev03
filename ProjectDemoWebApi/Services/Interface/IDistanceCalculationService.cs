namespace ProjectDemoWebApi.Services.Interface
{
    public interface IDistanceCalculationService
    {
        Task<decimal?> CalculateDistanceAsync(string customerAddress, CancellationToken cancellationToken = default);
    }
}