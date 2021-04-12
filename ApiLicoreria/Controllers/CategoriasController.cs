using Licoreria.Models;
using Licoreria.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiLicoreria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        IRepositoryLicoreria repo;

        public CategoriasController(IRepositoryLicoreria repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        public ActionResult<List<Categoria>> GetCategorias()
        {
            return this.repo.GetCategorias();
        }

        [HttpGet]
        [Route("[action]/{id}")]
        public ActionResult<String> GetNombreCategoria(int id)
        {
            return this.repo.GetNombreCategoria(id);
        }
    }
}
