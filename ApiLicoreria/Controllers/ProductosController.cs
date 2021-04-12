using Licoreria.Models;
using Licoreria.Repositories;
using Microsoft.AspNetCore.Authorization;
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
    public class ProductosController : ControllerBase
    {
        IRepositoryLicoreria repo;

        public ProductosController(IRepositoryLicoreria repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        public ActionResult<List<Producto>> GetProductos(String nombre, decimal? preciomax,
            decimal? litros, bool? stock, int? idcategoria)
        {
            return this.repo.GetProductos(nombre, preciomax, litros, stock, idcategoria);
        }

        [Authorize]
        [HttpPost]
        [Route("[action]")]
        public void InsertarProducto(Producto prod)
        {
            this.repo.InsertarProducto(prod.Nombre, prod.Precio, prod.Stock, prod.Imagen, prod.Litros, prod.Categoria);
        }

        [Authorize]
        [HttpPut]
        [Route("[action]")]
        public void EditarProducto(Producto prod)
        {
            this.repo.EditarProducto(prod.IdProducto, prod.Nombre, prod.Precio, prod.Stock, prod.Imagen, prod.Litros, prod.Categoria);
        }

        [Authorize]
        [HttpDelete]
        [Route("[action]/{id}")]
        public void EliminarProducto(int id)
        {
            this.repo.EliminarProducto(id);
        }

        [HttpGet]
        [Route("[action]/{id}")]
        public ActionResult<Producto> BuscarProducto(int id)
        {
            return this.repo.BuscarProducto(id);
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult<List<decimal>> GetListaLitros()
        {
            return this.repo.GetListaLitros();
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult<decimal> GetPrecioMax(int? id)
        {
            return this.repo.GetPrecioMax(id);
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult<decimal> GetPrecioMin(int? id)
        {
            return this.repo.GetPrecioMin(id);
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult<List<Producto>> GetListaProductos([FromQuery] List<int> ids)
        {
            return this.repo.GetListaProductos(ids);
        }

        [HttpGet]
        [Route("[action]/{id}")]
        public ActionResult<int> GetStock(int id)
        {
            return this.repo.GetStock(id);
        }

        [HttpPut]
        [Route("[action]/{id}/{cantidad}")]
        public void RestarStock(int id, int cantidad)
        {
            this.repo.RestarStock(id, cantidad);
        }

        [HttpPut]
        [Route("[action]/{id}/{cantidad}")]
        public void SumarStock(int id, int cantidad)
        {
            this.repo.SumarStock(id, cantidad);
        }

    }
}
