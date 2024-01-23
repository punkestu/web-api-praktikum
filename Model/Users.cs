using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;
namespace web_api_praktikum.Model;

public class UserM
{
    public long? Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public UserM(string username, string password)
    {
        Username = username;
        Password = password;
    }
}

public class UserFilter
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}

public interface UserDTO
{
    public UserM? GetOne(long id);
    public List<UserM>? Get(UserFilter? filter = null);
    public UserM? FromDataRow(DataRow dr);
    public bool VerifyUserNameUnique(string username, ModelStateDictionary modelState);
    public bool Create(UserM user);
    public string? GenerateToken(UserM user, IConfiguration _config);
}

public class CredentialRequest
{
    [Required]
    public string username { get; set; }
    [Required]
    [MinLength(8)]
    public string password { get; set; }
    public CredentialRequest(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}

public class CredentialResponse
{
    public string token { get; set; }
    public CredentialResponse(string token)
    {
        this.token = token;
    }
}