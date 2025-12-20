using System;
using System.Linq.Expressions;
using ClothingShop.Domain.Entities;

namespace ClothingShop.Infrastructure.Interfaces;

public interface IRoleRepository
{

    Task<Role?> GetByNameAsync(string name);
    Task<IEnumerable<Role>> GetAllAsync();
    Task<Role?> GetByIdAsync(int id);
    Task<Role?> FindAsync(Expression<Func<Role, bool>> predicate);
    Task AddAsync(Role entity);
    Task UpdateAsync(Role entity);
    void Delete(Role entity);
}
