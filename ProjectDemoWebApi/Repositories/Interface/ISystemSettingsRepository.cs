using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface ISystemSettingsRepository : IBaseRepository<SystemSettings>
    {
        Task<SystemSettings?> GetByKeyAsync(string settingKey, CancellationToken cancellationToken = default);
        Task<string?> GetSettingValueAsync(string settingKey, CancellationToken cancellationToken = default);
        Task<T?> GetSettingValueAsync<T>(string settingKey, CancellationToken cancellationToken = default) where T : struct;
        Task<bool> UpdateSettingAsync(string settingKey, string settingValue, string? description = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<SystemSettings>> GetAllSettingsAsync(CancellationToken cancellationToken = default);
        Task<bool> IsSettingExistsAsync(string settingKey, CancellationToken cancellationToken = default);
    }
}