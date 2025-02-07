using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Commons;
using Core.Commons.Paging;
using Dapper;
using Infra.PostgreSQL.Commons.Context;
using Infra.PostgreSQL.Commons.Filter;

namespace Infra.PostgreSQL.Commons.Repository;

public abstract class Repository<T> : IRepository<T> where T : Entity
{
    private static readonly SqlTemplate Sql = DapperSqlHelper.BuildTemplate<T>();
    private static readonly Dictionary<string, string> EntityDictionary = DapperSqlHelper.GetEntityDictionary<T>();

    private readonly IUnitOfWork _unitOfWork;

    protected Repository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<T> AddAsync(T entity)
    {
        IDbConnection connection = _unitOfWork.GetWritableConnection();
        IDbTransaction transaction = _unitOfWork.GetTransaction();
        long id = await connection.ExecuteScalarAsync<long>(Sql.Insert, entity, transaction).ConfigureAwait(true);
        entity.Id = id;

        return entity;
    }

    public async Task<Optional<T>> GetByIdAsync(long id)
    {
        IDbConnection connection = _unitOfWork.GetConnection();
        IDbTransaction transaction = _unitOfWork.GetTransaction();
        T entity = await connection.QueryFirstOrDefaultAsync<T>(Sql.SelectByKeyId, new {Id = id}, transaction).ConfigureAwait(false);

        return Optional<T>.Of(entity);
    }

    public async Task<Optional<T>> GetOneAsync(FilterDefinition filterDefinition)
    {
        return await GetOneAsync(new List<FilterDefinition> {filterDefinition});
    }

    public async Task<Optional<T>> GetOneAsync(List<FilterDefinition> filterDefinitions)
    {
        string aggregate = filterDefinitions
            .Select(filter => $"{EntityDictionary[filter.Property]} {filter.SqlOperator.Name} @{filter.Property}")
            .Aggregate((current, next) => $"{current} AND {next}");

        List<KeyValuePair<string, object>> parameters = filterDefinitions
            .Select(filter => new KeyValuePair<string, object>(filter.Property, CleanParameter(filter.Value)))
            .ToList();

        string whereClause = !string.IsNullOrEmpty(aggregate) ? $"WHERE {aggregate}" : "";
        string query = $"{Sql.Select}{whereClause}";

        IDbConnection connection = _unitOfWork.GetConnection();
        IDbTransaction transaction = _unitOfWork.GetTransaction();
        T entity = await connection.QueryFirstOrDefaultAsync<T>(query, parameters, transaction).ConfigureAwait(false);

        return Optional<T>.Of(entity);
    }

    private static object CleanParameter(object parameter)
    {
        return parameter is string ? $"{parameter.ToString()?.Replace("'", "''")}" : parameter;
    }

    public async Task<Page<T>> FindAsync(FilterDefinition filterDefinition, PageRequest pageRequest)
    {
        return await FindAsync(new List<FilterDefinition> {filterDefinition}, pageRequest);
    }

    public async Task<Page<T>> FindAsync(List<FilterDefinition> filterDefinitions, PageRequest pageRequest)
    {
        List<Order> orders = pageRequest.Sort.GetOrders();

        string orderBy = orders.Count == 0
            ? $"@{Sql.KeyProperty}"
            : orders.Select(order => $"@{order.Property} {order.Direction}").Aggregate((current, next) => $"{current}, {next}");

        string aggregate = filterDefinitions
            .Select(filter => $"{EntityDictionary[filter.Property]} {filter.SqlOperator.Name} @{filter.Property}")
            .Aggregate((current, next) => $"{current} AND {next}");

        List<KeyValuePair<string, object>> parameters = filterDefinitions
            .Select(filter => new KeyValuePair<string, object>(filter.Property, CleanParameter(filter.Value)))
            .ToList();

        string whereClause = !string.IsNullOrEmpty(aggregate) ? $"WHERE {aggregate}" : "";

        string query = $@"{Sql.Select} {
            whereClause
        } ORDER BY {orderBy} OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        int page = pageRequest.PageNumber;
        int size = pageRequest.PageSize;
        string countStatement = $"{Sql.Count} {whereClause}";

        IDbConnection connection = _unitOfWork.GetConnection();
        IDbTransaction transaction = _unitOfWork.GetTransaction();
        int totalOfElements = await connection.ExecuteScalarAsync<int>(countStatement, parameters, transaction).ConfigureAwait(false);
        int totalPages = (int) Math.Ceiling((double) totalOfElements / size);

        parameters.Add(new KeyValuePair<string, object>("Offset", page * size));
        parameters.Add(new KeyValuePair<string, object>("PageSize", size));

        IReadOnlyList<T> content =
            (
                await connection.QueryAsync<T>(query, parameters, transaction).ConfigureAwait(false)
            )
            .ToList().AsReadOnly();

        Pageable pageable = new Pageable(page, size, content.Count, totalOfElements, totalPages);

        return new Page<T>(content, pageable);
    }

    public async Task<T> UpdateAsync(T entity)
    {
        IDbConnection connection = _unitOfWork.GetWritableConnection();
        IDbTransaction transaction = _unitOfWork.GetTransaction();
        _ = await connection.ExecuteAsync(Sql.Update, entity, transaction).ConfigureAwait(true);

        return (await GetByIdAsync(entity.Id)).GetValue();
    }

    public async Task<int> RemoveAsync(long id)
    {
        IDbConnection connection = _unitOfWork.GetWritableConnection();
        IDbTransaction transaction = _unitOfWork.GetTransaction();

        return await connection.ExecuteAsync(Sql.Delete, new {Id = id}, transaction).ConfigureAwait(true);
    }
}