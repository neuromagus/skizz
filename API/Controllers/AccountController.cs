using API.DTOs;
using API.Extensions;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class AccountController(SignInManager<AppUser> signInManager) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto registerDto)
    {
        var user = new AppUser
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            UserName = registerDto.Email
        };

        var result = await signInManager.UserManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded) 
        {
            foreach (var error in result.Errors)
            {
                // we can use this here, because in top level of controller (BaseApiController) added
                // [ApiController], who added automatical validation errors in ModelState dictionary
                ModelState.AddModelError(error.Code, error.Description);
            }

            return ValidationProblem();
        }

        return Ok();
    }

    [Authorize]
    [HttpPost("address")]
    public async Task<ActionResult<Address>> CreateOrUpdateAdress(AddressDto addressDto)
    {
        var user = await signInManager.UserManager.GetUserByEmailWithAddress(User);
    
        if (user.Address is null)
        {
            user.Address = addressDto.ToEntity();
        } else 
        {
            user.Address.UpdateFromDto(addressDto);
        }

        var result = await signInManager.UserManager.UpdateAsync(user);

        return result.Succeeded ? Ok(user.Address.ToDto()) : BadRequest("Problem updating User Address");
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        await signInManager.SignOutAsync();

        return NoContent();
    }

    [HttpGet("user-info")]
    public async Task<ActionResult> GetUserInfo()
    {
        if (User.Identity?.IsAuthenticated == false) return NoContent();
        
        var user = await signInManager.UserManager.GetUserByEmailWithAddress(User);

        return Ok(new {
            user.FirstName,
            user.LastName,
            user.Email,
            Address = user.Address?.ToDto()
        }); 
    }

    [HttpGet("auth-status")]
    public ActionResult GetAuthState() => Ok(new {IsAuthenticated = User.Identity?.IsAuthenticated ?? false });
}
