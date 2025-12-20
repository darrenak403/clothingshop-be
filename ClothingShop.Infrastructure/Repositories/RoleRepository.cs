using System.Linq.Expressions;
using ClothingShop.Domain.Entities;
using ClothingShop.Infrastructure.Interfaces;
using ClothingShop.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ClothingShop.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    protected readonly ClothingShopDbContext _context;

    public RoleRepository(ClothingShopDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> FindAsync(Expression<Func<Role, bool>> predicate)
    {
        return await _context.Roles.FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<Role>> GetAllAsync() => await _context.Roles.ToListAsync();

    public async Task<Role?> GetByIdAsync(int id) => await _context.Roles.FindAsync(id);
    public async Task AddAsync(Role entity) => await _context.Roles.AddAsync(entity);

    public async Task UpdateAsync(Role entity) => _context.Roles.Update(entity);

    public void Delete(Role entity) => _context.Roles.Remove(entity);
    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Name == name);
    }
}

