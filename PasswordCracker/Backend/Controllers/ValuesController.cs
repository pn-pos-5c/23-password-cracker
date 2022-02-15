using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    private readonly PasswordService passwordService;

    public ValuesController(PasswordService passwordService)
    {
        this.passwordService = passwordService;
    }

    [HttpGet]
    public IActionResult Test()
    {
        return Ok(passwordService.CrackPassword("26775436073E00D207E192857EE3730CFCA19DE16F01F0780096EF217C2919EF", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890", 5).Result);
    }
}
