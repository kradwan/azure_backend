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
    private readonly DbConnectionString postgreConnectionString;

    public UsersRepository(DbConnectionString postgreConnectionString) {
        this.postgreConnectionString = postgreConnectionString;
    }
    public UsersRepository(IOptions<DbConnectionString> postgreConnectionString)
    {
        this.postgreConnectionString = postgreConnectionString.Value;
    }
    public User GetUserById(int id)
    {
        var connectionString = postgreConnectionString.DomainConnectionString;
        
        using (var connection = new SqlConnection(connectionString))
        {
            IEnumerable<User> queryResult = connection.Query<User>($"SELECT [FirstName], [LastName] FROM dbo.[Users] WHERE Id={id}");
            return queryResult.ToList().FirstOrDefault();
        }
    }
}