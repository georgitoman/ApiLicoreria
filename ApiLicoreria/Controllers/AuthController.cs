using ApiSeriesGeorge.Models;
using ApiSeriesGeorge.Token;
using Licoreria.Models;
using Licoreria.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiLicoreria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IRepositoryLicoreria repo;
        HelperToken helper;

        public AuthController(IRepositoryLicoreria repo
            , IConfiguration configuration)
        {
            this.helper = new HelperToken(configuration);
            this.repo = repo;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Login(LoginModel model)
        {
            Usuario usu =
                this.repo.LoginUsuario(model.Username
                , model.Password);
            if (usu != null)
            {
                Claim[] claims = new[]
                {
                    new Claim("UserData",
                    JsonConvert.SerializeObject(usu))
                };

                JwtSecurityToken token = new JwtSecurityToken
                    (
                     issuer: helper.Issuer
                     , audience: helper.Audience
                     , claims: claims
                     , expires: DateTime.UtcNow.AddMinutes(10)
                     , notBefore: DateTime.UtcNow
                     , signingCredentials:
new SigningCredentials(this.helper.GetKeyToken(), SecurityAlgorithms.HmacSha256)
                    );
                return Ok(
                    new
                    {
                        response =
                        new JwtSecurityTokenHandler().WriteToken(token)
                    });
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
