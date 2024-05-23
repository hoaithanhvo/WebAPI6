using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebAPI6.Models;
using WebAPI6.Repository;

namespace WebAPI6.Controllers
    {
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
        {
        private readonly IAccountRepository _repo;

        public AccountController(IAccountRepository repo) {
            _repo = repo;
            }
        [HttpPost("SignUp")]

        public async Task<IActionResult> SignUp(SignUpModel signUpModel)
            {
            var result = await _repo.SignUpAsync(signUpModel);
            if(result.Succeeded)
                {
                return Ok(result.Succeeded);
                }
            return StatusCode(500);
            }

        [HttpPost("SignIn")]

        public async Task<IActionResult> SignIn(SignInModel signInModel)
            {
            var result = await _repo.SignInAsync(signInModel);
            if (result.Empty!="")
                {
                return Unauthorized();
                }
            return Ok(result);  
            }

        [HttpPost("RenewToken")]
        public async Task<IActionResult> RenewToken(TokenModel model)
            {
            var result = await _repo.RenewToken(model);
            return Ok(result);
            }

        }



    }
