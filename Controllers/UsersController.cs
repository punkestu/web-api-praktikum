using Microsoft.AspNetCore.Mvc;
using web_api_praktikum.Model;

namespace web_api_praktikum.Controllers;
[ApiController]
[Route("[controller]")]

public class UsersController : Controller
{
    private readonly ILogger<UsersController> _logger;
    private readonly IConfiguration _config;
    private readonly UserDTO _userDTO;

    public UsersController(ILogger<UsersController> logger, IConfiguration config,
                           UserDTO userDTO)
    {
        _logger = logger;
        _config = config;
        _userDTO = userDTO;
    }
    [HttpPost("/login")]
    public IActionResult Login([FromBody] CredentialRequest loginRequest)
    {
        var user = _userDTO.Get(new UserFilter { Username = loginRequest.username });
        if (user == null)
        {
            ModelState.AddModelError("username", "Username not found");
            return new InvalidInputResult(ModelState);
        }
        if (user[0].Password != loginRequest.password)
        {
            ModelState.AddModelError("password", "Password is incorrect");
            return new InvalidInputResult(ModelState);
        }
        var token = _userDTO.GenerateToken(user[0], _config);
        if (token == null)
        {
            return StatusCode(500, "Failed to generate token");
        }
        return Ok(new CredentialResponse(token));
    }

    [HttpPost("/register")]
    public IActionResult Register([FromBody] CredentialRequest registerRequest)
    {
        if (!_userDTO.VerifyUserNameUnique(registerRequest.username, ModelState))
        {
            return new InvalidInputResult(ModelState);
        }
        var newUser = new UserM(registerRequest.username, registerRequest.password);
        if (!_userDTO.Create(newUser))
        {
            return StatusCode(500, "Failed to create user");
        }
        var token = _userDTO.GenerateToken(newUser, _config);
        if (token == null)
        {
            return StatusCode(500, "Failed to generate token");
        }
        return Ok(new CredentialResponse(token));
    }
}