using System.Data;

namespace VueAppAdmin.Server.Features.Auth;

public class UserRepository(IDbConnection db) : IUserRepository
{
    private readonly IDbConnection _db = db;

    public bool ExistsWithCredentials(string username, string hashedPassword)
    {
        // TODO: 替換為實際 SQL 查詢
        // const string sql = "SELECT COUNT(1) FROM Users WHERE Username = @Username AND PasswordHash = @PasswordHash";
        // return db.ExecuteScalar<int>(sql, new { Username = username, PasswordHash = hashedPassword }) > 0;
        return false;
    }

    public string? FindDisplayNameByUsername(string username)
    {
        // TODO: 替換為實際 SQL 查詢
        // const string sql = "SELECT DisplayName FROM Users WHERE Username = @Username";
        // return db.QuerySingleOrDefault<string>(sql, new { Username = username });
        return null;
    }
}
