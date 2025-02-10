using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Commons;
using Core.Commons.Paging;
using Infra.PostgreSql.Commons.Filter;

namespace Infra.PostgreSql.Commons.Repository;

public interface IRepository<T> where T : Entity
{
    Task<T> AddAsync(T entity);
    Task<Optional<T>> GetByIdAsync(long id);
    Task<Optional<T>> GetOneAsync(FilterDefinition filterDefinition);
    Task<Optional<T>> GetOneAsync(List<FilterDefinition> filterDefinitions);
    Task<Page<T>> FindAsync(FilterDefinition filterDefinition, PageRequest pageRequest);
    Task<Page<T>> FindAsync(List<FilterDefinition> filterDefinitions, PageRequest pageRequest);
    Task<T> UpdateAsync(T entity);
    Task<int> RemoveAsync(long id);
}