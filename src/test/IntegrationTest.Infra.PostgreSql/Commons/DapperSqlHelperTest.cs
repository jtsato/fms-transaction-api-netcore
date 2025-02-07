using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Infra.PostgreSQL.Commons.Filter;
using Xunit;

namespace IntegrationTest.Infra.PostgreSql.Commons;

public sealed class DapperSqlHelperTest
{
    [Trait("Category", "Infrastructure (DB) Integration tests")]
    [Fact(DisplayName = "Successful to get table name from entity with TableAttribute")]
    public void SuccessfulToGetTableNameFromEntityWithTableAttribute()
    {
        // Arrange
        // Act
        string actual = DapperSqlHelper.BuildTemplate<RegularEntity>().TableName;

        // Assert
        Assert.Equal("Employees", actual);
    }

    [Trait("Category", "Infrastructure (DB) Integration tests")]
    [Fact(DisplayName = "Fail to get table name from entity without TableAttribute")]
    public void FailToGetTableNameFromEntityWithoutTableAttribute()
    {
        // Arrange
        // Act
        // Assert
        ArgumentException argumentException =
            Assert.Throws<ArgumentException>(
                DapperSqlHelper.BuildTemplate<NoTableEntity>
            );

        Assert.Equal("The type NoTableEntity does not have a Table Attribute defined.", argumentException.Message);
    }

    [Trait("Category", "Infrastructure (DB) Integration tests")]
    [Fact(DisplayName = "Successful to get key property name")]
    public void SuccessfulToGetKeyPropertyName()
    {
        // Arrange
        List<PropertyInfo> properties = typeof(RegularEntity).GetProperties().ToList();
        
        // Act
        string actual = DapperSqlHelper.GetKeyPropertyName<RegularEntity>(properties);

        // Assert
        Assert.Equal("Id", actual);
    }

    [Trait("Category", "Infrastructure (DB) Integration tests")]
    [Fact(DisplayName = "Fail to get key property name")]
    public void FailToGetKeyPropertyName()
    {
        // Arrange
        List<PropertyInfo> properties = typeof(NoKeyEntity).GetProperties().ToList();
        // Act
        // Assert
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => DapperSqlHelper.GetKeyPropertyName<NoKeyEntity>(properties)
            );
        
        Assert.Equal("The type NoKeyEntity does not have a Key Attribute defined.", exception.Message);
    }

    [Trait("Category", "Infrastructure (DB) Integration tests")]
    [Fact(DisplayName = "Successful to get all columns")]
    public void SuccessfulToGetAllColumns()
    {
        // Arrange
        // Act
        string actual = DapperSqlHelper.GetEntityDictionary<RegularEntity>()
            .Values.Aggregate((current, next) => $"{current}, {next}");

        // Assert
        Assert.Equal("id, name, surname, birth_date, number_of_children, salary", actual);
    }

    [Trait("Category", "Infrastructure (DB) Integration tests")]
    [Fact(DisplayName = "Successful to get columns excluding key")]
    public void SuccessfulToGetColumnsExcludingKey()
    {
        // Arrange
        // Act
        SqlTemplate actual = DapperSqlHelper.BuildTemplate<EmptyEntity>();

        // Assert
        Assert.Equal("Empty", actual.TableName);
        Assert.Equal("Id", actual.KeyProperty);
        Assert.Equal("INSERT INTO Empty () VALUES () RETURNING id AS Id", actual.Insert);
        Assert.Equal("SELECT id AS Id FROM Empty ", actual.Select);
        Assert.Equal("SELECT id AS Id FROM Empty WHERE id = @Id", actual.SelectByKeyId);
        Assert.Equal("SELECT COUNT(*) FROM Empty ", actual.Count);
        Assert.Equal("DELETE FROM Empty WHERE id = @Id", actual.Delete);
        Assert.Equal("UPDATE Empty SET  WHERE id = @Id", actual.Update);
    }
    
    [Trait("Category", "Infrastructure (DB) Integration tests")]
    [Fact(DisplayName = "Successful to get conversion table")]
    public void SuccessfulToGetConversionTable()
    {
        // Arrange
        // Act
        Dictionary<string, string> actual = DapperSqlHelper.GetEntityDictionary<RegularEntity>();

        // Assert
        Assert.Equal(6, actual.Count);
        Assert.Equal("id", actual["Id"]);
        Assert.Equal("name", actual["Name"]);
        Assert.Equal("surname", actual["Surname"]);
        Assert.Equal("birth_date", actual["BirthDate"]);
        Assert.Equal("number_of_children", actual["NumberOfChildren"]);
        Assert.Equal("salary", actual["Salary"]);
    }

    [Trait("Category", "Infrastructure (DB) Integration tests")]
    [Fact(DisplayName = "Fail to get conversion table")]
    public void FailToGetConversionTable()
    {
        // Arrange
        // Act
        Dictionary<string, string> actual = DapperSqlHelper.GetEntityDictionary<EmptyEntity>(excludeKey: true);

        // Assert
        Assert.Empty(actual);
    }
    
    [Trait("Category", "Infrastructure (DB) Integration tests")]
    [Fact(DisplayName = "Successful to build select sql template")]
    public void SuccessfulToBuildSelectSqlTemplate()
    {
        // Arrange
        // Act
        SqlTemplate sql = DapperSqlHelper.BuildTemplate<RegularEntity>();

        // Assert
        Assert.Equal("Employees", sql.TableName);
        Assert.Equal("Id", sql.KeyProperty);
        Assert.Equal("INSERT INTO Employees (name, surname, birth_date, number_of_children, salary) VALUES (@Name, @Surname, @BirthDate, @NumberOfChildren, @Salary) RETURNING id AS Id", sql.Insert);
        Assert.Equal("SELECT id AS Id, name AS Name, surname AS Surname, birth_date AS BirthDate, number_of_children AS NumberOfChildren, salary AS Salary FROM Employees ", sql.Select);
        Assert.Equal("SELECT id AS Id, name AS Name, surname AS Surname, birth_date AS BirthDate, number_of_children AS NumberOfChildren, salary AS Salary FROM Employees WHERE id = @Id", sql.SelectByKeyId);
        Assert.Equal("SELECT COUNT(*) FROM Employees ", sql.Count);
        Assert.Equal("DELETE FROM Employees WHERE id = @Id", sql.Delete);
        Assert.Equal("UPDATE Employees SET name = @Name, surname = @Surname, birth_date = @BirthDate, number_of_children = @NumberOfChildren, salary = @Salary WHERE id = @Id", sql.Update);
    }

    [Trait("Category", "Infrastructure (DB) Integration tests")]
    [Fact(DisplayName = "Successful to get property names")]
    public void SuccessfulToGetPropertyNames()
    {
        // Arrange
        // Act
        string propertyNames = DapperSqlHelper.GetEntityDictionary<RegularEntity>()
            .Keys.Select(key => $"@{key}")
            .Aggregate((current, next) => $"{current}, {next}");

        // Assert
        Assert.Equal("@Id, @Name, @Surname, @BirthDate, @NumberOfChildren, @Salary", propertyNames);
    }

    [Trait("Category", "Infrastructure (DB) Integration tests")]
    [Fact(DisplayName = "Successful to get property names excluding key")]
    public void SuccessfulToGetPropertyNamesExcludingKey()
    {
        // Arrange
        // Act
        string propertyNames = DapperSqlHelper.GetEntityDictionary<RegularEntity>(excludeKey: true)
            .Keys.Select(key => $"@{key}")
            .Aggregate((current, next) => $"{current}, {next}");

        // Assert
        Assert.Equal("@Name, @Surname, @BirthDate, @NumberOfChildren, @Salary", propertyNames);
    }
}