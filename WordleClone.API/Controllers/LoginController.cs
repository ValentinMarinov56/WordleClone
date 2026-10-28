
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using WordleClone.API.DTOs;
using WordleClone.API.Services;

[ApiController]
[Route("api/[controller]")]

public sealed class LoginController: Controller
{
    private readonly ILoginService _loginServices;
    public LoginController(ILoginService loginService)
    {
        _loginServices = loginService;
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] LoginRequestDto LoginRequestDto)
    {
        var user = await _loginServices.UserAuthentication(LoginRequestDto.Username, LoginRequestDto.Password);
        if (user == null)
        {
            return Unauthorized();
        }
        return Ok(user);
    }
}