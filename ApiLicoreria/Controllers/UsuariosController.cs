using Licoreria.Models;
using Licoreria.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiLicoreria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        IRepositoryLicoreria repo;

        public UsuariosController(IRepositoryLicoreria repo)
        {
            this.repo = repo;
        }

        [Authorize]
        [HttpGet]
        [Route("[action]/{id}")]
        public ActionResult<Usuario> BuscarUsuario(int id)
        {
            return this.repo.BuscarUsuario(id);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public ActionResult<Usuario> GetUsuario()
        {
            List<Claim> claims = HttpContext.User.Claims.ToList();
            String json = claims.SingleOrDefault(z => z.Type == "UserData").Value;
            Usuario usu = JsonConvert.DeserializeObject<Usuario>(json);
            return usu;
        }

        [HttpPost]
        [Route("[action]")]
        public void InsertarUsuario(Usuario user, String password)
        {
            this.repo.InsertarUsuario(user.UserName, user.Nombre, user.Correo, password, user.Direccion, user.Telefono);
        }

        [Authorize]
        [HttpPut]
        [Route("[action]")]
        public void EditarUsuario(int idusuario, String nombre, String direccion, String telefono)
        {
            this.repo.EditarUsuario(idusuario, nombre, direccion, telefono);
        }

        [Authorize]
        [HttpPut]
        [Route("[action]")]
        public void CambiarContraseña(int idusuario, String password)
        {
            this.repo.CambiarContraseña(idusuario, password);
        }

        [HttpGet]
        [Route("[action]/{username}")]
        public ActionResult<bool> UserNameExists(String username)
        {
            return this.repo.UserNameExists(username);
        }
    }
}
