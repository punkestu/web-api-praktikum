using System.Data;
using System.Data.SQLite;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;

namespace web_api_praktikum.Model.DTO.Sqlite;

public class UserDTOSQLite : UserDTO
{
      private readonly DB db;
      public UserDTOSQLite(DB db)
      {
            this.db = db;
      }
      public UserM? GetOne(long id)
      {
            SQLiteCommand command = (SQLiteCommand)db.CreateCommand();
            command.CommandText = "SELECT * FROM users WHERE id=@id";
            command.Parameters.AddWithValue("@id", id);
            var dr = db.GetOne(command);
            return dr == null ? null : FromDataRow(dr);
      }
      public List<UserM>? Get(UserFilter? filter = null)
      {
            SQLiteCommand command = (SQLiteCommand)db.CreateCommand();
            command.CommandText = "SELECT * FROM users";
            List<string> where = new List<string>();
            if (filter?.Username != null)
            {
                  where.Add("username = @username");
                  command.Parameters.AddWithValue("@username", filter.Username);
            }
            if (filter?.Password != null)
            {
                  where.Add("password = @password");
                  command.Parameters.AddWithValue("@password", filter.Password);
            }
            if (where.Count > 0)
            {
                  command.CommandText += " WHERE " + string.Join(" AND ", where);
            }
            DataTable dt = db.GetMany(command);
            List<UserM> users = new List<UserM>();
            foreach (DataRow dr in dt.Rows)
            {
                  UserM? user = FromDataRow(dr);
                  if (user == null)
                  {
                        throw new Exception("Invalid data in database");
                  }
                  users.Add(user);
            }
            return users.Count > 0 ? users : null;
      }
      public UserM? FromDataRow(DataRow dr)
      {
            var username = dr["username"].ToString();
            var password = dr["password"].ToString();
            if (username == null || password == null)
            {
                  return null;
            }
            UserM user = new UserM(username, password)
            {
                  Id = Convert.ToInt32(dr["id"])
            };
            return user;
      }
      public bool VerifyUserNameUnique(string username, ModelStateDictionary modelState)
      {
            var users = Get(new UserFilter { Username = username });
            if (users != null)
            {
                  modelState.AddModelError("username", "Username already exists");
                  return false;
            }
            return true;
      }
      public bool Create(UserM user)
      {
            SQLiteCommand cmd = (SQLiteCommand)db.CreateCommand();
            cmd.CommandText =
                "INSERT INTO users (username, password) VALUES (@username, @password); select last_insert_rowid();";
            cmd.Parameters.AddWithValue("@username", user.Username);
            cmd.Parameters.AddWithValue("@password", user.Password);
            user.Id = db.ExecuteAndId(cmd);
            return user.Id != null;
      }
      public string? GenerateToken(UserM user, IConfiguration _config)
      {
            var securityKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials =
                new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var userId = user.Id.ToString();
            if (userId == null)
            {
                  return null;
            }
            var claims = new Claim[] { new Claim(ClaimTypes.Authentication, userId) };
            var Sectoken = new JwtSecurityToken(
               _config["Jwt:Issuer"], _config["Jwt:Issuer"], claims,
                expires: DateTime.Now.AddMinutes(120), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(Sectoken);
      }
}