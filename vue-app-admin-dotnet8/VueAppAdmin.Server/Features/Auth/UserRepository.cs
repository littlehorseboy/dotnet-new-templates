using System.Data;

namespace VueAppAdmin.Server.Features.Auth;

// 使用者資料庫存取層（Dapper）
// 目前方法均為 TODO 骨架，待 AuthService 改為資料庫驗證後啟用
public class UserRepository(IDbConnection db) : IUserRepository
{
    private readonly IDbConnection _db = db;

    // 驗證使用者帳密：比對 DB 中的 PasswordHash
    // hashedPassword 應先以 BCrypt 雜湊後再傳入
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
