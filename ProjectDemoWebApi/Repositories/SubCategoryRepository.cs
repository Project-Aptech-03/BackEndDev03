using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

public class SubCategoryRepository : ISubCategoryRepository
{
    private readonly ApplicationDbContext _context;

    public SubCategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SubCategories?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.SubCategories
            .Include(sc => sc.Category) 
            .FirstOrDefaultAsync(sc => sc.Id == id, cancellationToken);
    }

    public async Task AddAsync(SubCategories subCategory, CancellationToken cancellationToken = default)
    {
        await _context.SubCategories.AddAsync(subCategory, cancellationToken);
    }

    public void Update(SubCategories subCategory)
    {
        _context.SubCategories.Update(subCategory);
    }

    public void Delete(SubCategories subCategory)
    {
        _context.SubCategories.Remove(subCategory);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
