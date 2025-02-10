using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Infra.PostgreSql.Commons.Filter;

public static class DapperSqlHelper
{
    public static SqlTemplate BuildTemplate<T>()
    {
        string tableName = GetTableName<T>();

        List<PropertyInfo> properties = GetDecorateProperties<T>();
        string keyProperty = GetKeyPropertyName<T>(properties);

        Dictionary<string, string> columnTable = BuildEntityDictionary(properties);
        Dictionary<string, string> columnTableWithoutKey = columnTable
            .Where(keyValue => keyValue.Key != keyProperty)
            .ToDictionary(keyValue => keyValue.Key, keyValue => keyValue.Value);

        string keyColumn = columnTable[keyProperty];
        string noKeyProperties = string.Join(", ", columnTableWithoutKey.Keys.Select(key => $"@{key}"));
        string noKeyColumns = string.Join(", ", columnTableWithoutKey.Values);
        string columns = string.Join(", ", columnTable.Select(keyValue => $"{keyValue.Value} AS {keyValue.Key}"));

        return new SqlTemplate
        {
            TableName = tableName,
            KeyProperty = keyProperty,
            Insert = $"INSERT INTO {tableName} ({noKeyColumns}) VALUES ({noKeyProperties}) RETURNING {keyColumn} AS {keyProperty}",
            Select = $"SELECT {columns} FROM {tableName} ",
            SelectByKeyId = $"SELECT {columns} FROM {tableName} WHERE {keyColumn} = @{keyProperty}",
            Count = $"SELECT COUNT(*) FROM {tableName} ",
            Delete = $"DELETE FROM {tableName} WHERE {keyColumn} = @{keyProperty}",
            Update = $@"UPDATE {tableName} SET {
                string.Join(", ", columnTableWithoutKey
                    .Select(keyValue => $"{keyValue.Value} = @{keyValue.Key}")
                )
            } WHERE {keyColumn} = @{keyProperty}".Trim(),
        };
    }

    private static string GetTableName<T>()
    {
        TableAttribute attribute = typeof(T).GetCustomAttribute<TableAttribute>();
        if (attribute != null) return attribute.Name;
        throw new ArgumentException($"The type {typeof(T).Name} does not have a Table Attribute defined.");
    }

    public static Dictionary<string, string> GetEntityDictionary<T>(bool excludeKey = false)
    {
        List<PropertyInfo> properties = GetDecorateProperties<T>(excludeKey);
        return BuildEntityDictionary(properties);
    }

    private static List<PropertyInfo> GetDecorateProperties<T>(bool excludeKey = false)
    {
        return typeof(T).GetProperties()
            .Where(property => property.GetCustomAttribute<ColumnAttribute>() != null)
            .Where(property => !excludeKey || property.GetCustomAttribute<KeyAttribute>() == null)
            .OrderByDescending(property => property.GetCustomAttribute<KeyAttribute>() != null)
            .ToList();
    }

    private static Dictionary<string, string> BuildEntityDictionary(List<PropertyInfo> properties)
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();

        foreach (PropertyInfo property in properties)
        {
            object[] columnAttributes = property.GetCustomAttributes(typeof(ColumnAttribute), true);
            ColumnAttribute columnAttribute = (ColumnAttribute) columnAttributes[0];
            dictionary.Add(property.Name, columnAttribute.Name);
        }

        return dictionary;
    }

    public static string GetKeyPropertyName<T>(List<PropertyInfo> properties)
    {
        foreach (PropertyInfo property in properties)
        {
            object[] keyAttributes = property.GetCustomAttributes(typeof(KeyAttribute), true);
            if (keyAttributes.Length <= 0) continue;
            return property.Name;
        }

        throw new ArgumentException($"The type {typeof(T).Name} does not have a Key Attribute defined.");
    }
}