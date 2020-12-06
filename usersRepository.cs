using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using AzureBackend;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

public class UsersRepository
{
    private readonly DbConnectionString connectionString;

    public UsersRepository(DbConnectionString postgreConnectionString) {
        this.connectionString = postgreConnectionString;
    }
    public UsersRepository(IOptions<DbConnectionString> connectionString)
    {
        this.connectionString = connectionString.Value;
    }
    public User GetUserById(int id)
    {
        var connectionString = this.connectionString.DomainConnectionString;
        
        using (var connection = new SqlConnection(connectionString))
        {
            IEnumerable<User> queryResult = connection.Query<User>($"SELECT [FirstName], [LastName] FROM dbo.[Users] WHERE Id={id}");
            return queryResult.ToList().FirstOrDefault();
        }
    }
    public IList<User> GetAllUsers()
    {
        var connectionString = this.connectionString.DomainConnectionString;
        
        using (var connection = new SqlConnection(connectionString))
        {
            IEnumerable<User> queryResult = connection.Query<User>($"SELECT [Id], [FirstName], [LastName] FROM dbo.[Users]");
            return queryResult.ToList();
        }
    }
}