using ProjectDemoWebApi.Models;
using System.Threading;
using System.Threading.Tasks;

public interface ISubCategoryRepository
{
    Task<SubCategories?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(SubCategories subCategory, CancellationToken cancellationToken = default);
    void Update(SubCategories subCategory);
    void Delete(SubCategories subCategory);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
