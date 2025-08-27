using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;
using System.Globalization;

namespace ProjectDemoWebApi.Repositories
{
    public class SystemSettingsRepository : BaseRepository<SystemSettings>, ISystemSettingsRepository
    {
        public SystemSettingsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<SystemSettings?> GetByKeyAsync(string settingKey, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(s => s.SettingKey == settingKey, cancellationToken);
        }

        public async Task<string?> GetSettingValueAsync(string settingKey, CancellationToken cancellationToken = default)
        {
            var setting = await GetByKeyAsync(settingKey, cancellationToken);
            return setting?.SettingValue;
        }

        public async Task<T?> GetSettingValueAsync<T>(string settingKey, CancellationToken cancellationToken = default) where T : struct
        {
            var stringValue = await GetSettingValueAsync(settingKey, cancellationToken);
            
            if (string.IsNullOrEmpty(stringValue))
                return null;

            try
            {
                if (typeof(T) == typeof(decimal))
                {
                    if (decimal.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalResult))
                        return (T)(object)decimalResult;
                }
                else if (typeof(T) == typeof(int))
                {
                    if (int.TryParse(stringValue, out var intResult))
                        return (T)(object)intResult;
                }
                else if (typeof(T) == typeof(bool))
                {
                    if (bool.TryParse(stringValue, out var boolResult))
                        return (T)(object)boolResult;
                }
                else if (typeof(T) == typeof(double))
                {
                    if (double.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var doubleResult))
                        return (T)(object)doubleResult;
                }

                return (T)Convert.ChangeType(stringValue, typeof(T), CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateSettingAsync(string settingKey, string settingValue, string? description = null, CancellationToken cancellationToken = default)
        {
            var setting = await _dbSet.FirstOrDefaultAsync(s => s.SettingKey == settingKey, cancellationToken);
            
            if (setting != null)
            {
                setting.SettingValue = settingValue;
                setting.UpdatedDate = DateTime.UtcNow;
                if (!string.IsNullOrEmpty(description))
                    setting.Description = description;
                
                _dbSet.Update(setting);
            }
            else
            {
                setting = new SystemSettings
                {
                    SettingKey = settingKey,
                    SettingValue = settingValue,
                    Description = description,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };
                
                await _dbSet.AddAsync(setting, cancellationToken);
            }

            return true;
        }

        public async Task<IEnumerable<SystemSettings>> GetAllSettingsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .OrderBy(s => s.SettingKey)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsSettingExistsAsync(string settingKey, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(s => s.SettingKey == settingKey, cancellationToken);
        }
    }
}