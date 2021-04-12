using Licoreria.Models;
using Licoreria.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiLicoreria.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        IRepositoryLicoreria repo;

        public PedidosController(IRepositoryLicoreria repo)
        {
            this.repo = repo;
        }

        [HttpPost]
        [Route("[action]/{id}/{st}")]
        public void CreatePedido(int id, decimal st, Carrito carrito)
        {
            this.repo.CreatePedido(id, st, carrito);
        }

        [HttpGet]
        [Route("[action]/{id}")]
        public ActionResult<Pedido> BuscarPedido(int id)
        {
            return this.repo.BuscarPedido(id);
        }

        [HttpDelete]
        [Route("[action]/{id}")]
        public void CancelarPedido(int id)
        {
            this.repo.CancelarPedido(id);
        }

        [HttpGet]
        [Route("[action]/{id}")]
        public ActionResult<List<Pedido>> GetPedidosUsuario(int id)
        {
            return this.repo.GetPedidosUsuario(id);
        }

        [HttpGet]
        [Route("[action]/{id}")]
        public ActionResult<Carrito> GetProductosPedido(int id)
        {
            return this.repo.GetProductosPedido(id);
        }

    }
}
