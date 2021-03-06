using Licoreria.Data;
using Licoreria.Helpers;
using Licoreria.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Licoreria.Repositories
{
    #region SQL

    //CREATE PROCEDURE PRODUCTOSFILTROS
    //(@NOMBRE NVARCHAR(50) = NULL,
    //@PRECIOMAX DECIMAL(10, 2) = NULL, @LITROS DECIMAL(10, 2) = NULL, @STOCK BIT = NULL,
    //@IDCATEGORIA INT = NULL)
    //AS

    //    SELECT* FROM PRODUCTO
    //   WHERE
    //   (@NOMBRE IS NULL OR NOMBRE LIKE '%' + @NOMBRE + '%')
    //    AND
    //    (@PRECIOMAX IS NULL OR PRECIO <= @PRECIOMAX)
    //    AND
    //    (@LITROS IS NULL OR LITROS = @LITROS)
    //    AND
    //    (@STOCK IS NULL OR STOCK != 0)
    //    AND
    //    (@IDCATEGORIA IS NULL OR IDCATEGORIA = @IDCATEGORIA)
    //GO

    #endregion

    public class RepositoryLicoreria: IRepositoryLicoreria
    {
        LicoreriaContext context;

        public RepositoryLicoreria(LicoreriaContext context)
        {
            this.context = context;
        }

        #region CATEGORIAS

        public List<Categoria> GetCategorias()
        {
            return this.context.Categorias.ToList();
        }

        public String GetNombreCategoria(int idcategoria)
        {
            Categoria cat = this.context.Categorias.Where(z => z.IdCategoria == idcategoria).FirstOrDefault();
            return cat.Nombre;
        }

        #endregion

        #region PRODUCTOS

        public List<Producto> GetProductos(String nombre, decimal? preciomax,
            decimal? litros, bool? stock, int? idcategoria)
        {
            String sql = "PRODUCTOSFILTROS @NOMBRE, @PRECIOMAX, @LITROS, @STOCK, @IDCATEGORIA";

            int contador = 0;
            object[] parametros = new object[5];

            SqlParameter pamnom;
            if (nombre == null)
                pamnom = new SqlParameter("@NOMBRE", DBNull.Value);
            else
                pamnom = new SqlParameter("@NOMBRE", nombre);
            parametros[contador] = pamnom;
            contador++;

            SqlParameter pampmax;
            if (preciomax == null)
                pampmax = new SqlParameter("@PRECIOMAX", DBNull.Value);
            else
                pampmax = new SqlParameter("@PRECIOMAX", preciomax);
            parametros[contador] = pampmax;
            contador++;

            SqlParameter pamlit;
            if (litros == null)
                pamlit = new SqlParameter("@LITROS", DBNull.Value);
            else
                pamlit = new SqlParameter("@LITROS", litros);
            parametros[contador] = pamlit;
            contador++;

            SqlParameter pamsto;
            if (stock == null)
                pamsto = new SqlParameter("@STOCK", DBNull.Value);
            else
                pamsto = new SqlParameter("@STOCK", stock);
            parametros[contador] = pamsto;
            contador++;

            SqlParameter pamcat;
            if (idcategoria == null)
                pamcat = new SqlParameter("@IDCATEGORIA", DBNull.Value);
            else
                pamcat = new SqlParameter("@IDCATEGORIA", idcategoria);
            parametros[contador] = pamcat;

            List<Producto> productos = this.context.Productos.FromSqlRaw(sql, parametros).ToList();
            
            return productos;
        }

        public void InsertarProducto(String nombre, decimal precio, int stock,
            String imagen, decimal litros, int idcategoria)
        {
            Producto prod = new Producto();
            prod.IdProducto = this.GetMaxId(Tablas.Productos);
            prod.Nombre = nombre;
            prod.Precio = precio;
            prod.Stock = stock;
            prod.Imagen = imagen;
            prod.Litros = litros;
            prod.Categoria = idcategoria;

            this.context.Productos.Add(prod);
            this.context.SaveChanges();
        }

        public void EditarProducto(int idproducto, String nombre, decimal precio,
            int stock, String imagen, decimal litros, int idcategoria)
        {
            Producto prod = this.BuscarProducto(idproducto);
            prod.Nombre = nombre;
            prod.Precio = precio;
            prod.Stock = stock;
            if(imagen != null)
                prod.Imagen = imagen;
            prod.Litros = litros;
            prod.Categoria = idcategoria;

            this.context.SaveChanges();
        }

        public void EliminarProducto(int idproducto)
        {
            Producto prod = this.BuscarProducto(idproducto);

            this.context.Productos.Remove(prod);

            this.context.SaveChanges();
        }

        public Producto BuscarProducto(int idproducto)
        {
            return this.context.Productos.Where(z => z.IdProducto == idproducto).FirstOrDefault();
        }

        public List<decimal> GetListaLitros()
        {
            return this.context.Productos.Select(z => z.Litros).Distinct().ToList();
        }

        public decimal GetPrecioMax(int? idcategoria)
        {
            if (idcategoria == null)
                return this.context.Productos.Select(z => z.Precio).Max();
            else
                return this.context.Productos.Where(z => z.Categoria == idcategoria)
                    .Select(z => z.Precio).Max();
        }

        public decimal GetPrecioMin(int? idcategoria)
        {
            if (idcategoria == null)
                return this.context.Productos.Select(z => z.Precio).Min();
            else
                return this.context.Productos.Where(z => z.Categoria == idcategoria)
                    .Select(z => z.Precio).Min();
        }

        public List<Producto> GetListaProductos(List<int> idproductos)
        {
            List<Producto> productos = new List<Producto>();
            foreach(int id in idproductos)
            {
                Producto prod = this.context.Productos.Where(z => z.IdProducto == id).FirstOrDefault();
                productos.Add(prod);
            }
            return productos;
        }

        public int GetStock(int idproducto) {
            Producto prod = this.BuscarProducto(idproducto);

            return prod.Stock;
        }

        public void RestarStock(int idproducto, int cantidad)
        {
            Producto prod = this.context.Productos.Where(z => z.IdProducto == idproducto).FirstOrDefault();
            prod.Stock -= cantidad;
            this.context.SaveChanges();
        }

        public void SumarStock(int idproducto, int cantidad)
        {
            Producto prod = this.context.Productos.Where(z => z.IdProducto == idproducto).FirstOrDefault();
            prod.Stock += cantidad;
            this.context.SaveChanges();
        }

        #endregion

        #region USUARIOS

        public Usuario BuscarUsuario(int idusuario)
        {
            return this.context.Usuarios.Where(z => z.IdUsuario == idusuario).FirstOrDefault();
        }

        public void InsertarUsuario(String username, String nombre, String correo, String password, String direccion, String telefono)
        {
            if (!this.UserNameExists(username))
            {
                Usuario user = new Usuario();
                user.IdUsuario = this.GetMaxId(Tablas.Usuarios);
                user.UserName = username;
                user.Nombre = nombre;
                user.Correo = correo;
                user.Direccion = direccion;
                user.Telefono = telefono;

                String salt = CypherService.GetSalt();
                user.Salt = salt;
                user.Password = CypherService.CifrarContenido(password, salt);

                user.Validado = true;
                user.Rol = 0;

                this.context.Usuarios.Add(user);
                this.context.SaveChanges();
            }
        }

        public void EditarUsuario(int idusuario, String nombre, String direccion, String telefono)
        {
            Usuario user = this.BuscarUsuario(idusuario);
            user.Nombre = nombre;
            user.Direccion = direccion;
            user.Telefono = telefono;
            this.context.SaveChanges();
        }

        public void CambiarContraseña(int idusuario, String password)
        {
            Usuario user = this.BuscarUsuario(idusuario);

            String salt = CypherService.GetSalt();
            user.Salt = salt;
            user.Password = CypherService.CifrarContenido(password, salt);

            this.context.SaveChanges();
        }

        public Usuario LoginUsuario(String username, String password)
        {
            if (this.UserNameExists(username))
            {
                Usuario user = this.context.Usuarios.Where(z => z.UserName.ToUpper() == username.ToUpper()).FirstOrDefault();
                if (user == null)
                {
                    return null;
                } else if (!user.Validado)
                {
                    return null;
                }
                else
                {
                    String salt = user.Salt;
                    byte[] passbbdd = user.Password;
                    byte[] passtemporal = CypherService.CifrarContenido(password, salt);

                    bool respuesta = ToolkitService.CompararArrayBytes(passbbdd, passtemporal);
                    if (respuesta)
                    {
                        return user;
                    }
                    else
                    {
                        return null;
                    }
                }
            } else
            {
                return null;
            }
            
        }

        public bool UserNameExists(String username)
        {
            bool res = this.context.Usuarios.Where(z => z.UserName == username).Any();
            if (res)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region PEDIDOS

        public void CreatePedido(int idusuario, decimal subtotal, Carrito carrito) {
            String direccion = this.BuscarUsuario(idusuario).Direccion;
            
            Pedido pedido = new Pedido();
            pedido.IdPedido = this.GetMaxId(Tablas.Pedidos);
            pedido.Usuario = idusuario;
            pedido.Fecha = DateTime.Now;
            pedido.Coste = subtotal;
            pedido.Direccion = direccion;
            this.context.Pedidos.Add(pedido);
            this.context.SaveChanges();

            List<Producto> productos = this.GetListaProductos(carrito.Productos);

            int contador = 0;
            foreach(Producto prod in productos)
            {
                ProductosPedido pp = new ProductosPedido();
                pp.IdProductosPedido = this.GetMaxId(Tablas.ProductosPedido);
                pp.Pedido = pedido.IdPedido;
                pp.Producto = prod.IdProducto;
                pp.Cantidad = carrito.Cantidades[contador];

                this.RestarStock(prod.IdProducto, carrito.Cantidades[contador]);

                this.context.ProductosPedidos.Add(pp);
                this.context.SaveChanges();
                contador++;
            }
        }

        public Pedido BuscarPedido(int idpedido)
        {
            return this.context.Pedidos.Where(z => z.IdPedido == idpedido).FirstOrDefault();
        }

        public void CancelarPedido(int idpedido)
        {
            Carrito productos = this.GetProductosPedido(idpedido);
            int cont = 0;

            foreach(int id in productos.Productos)
            {
                Producto prod = this.BuscarProducto(id);
                this.SumarStock(prod.IdProducto, productos.Cantidades[cont]);
                cont++;
            }

            Pedido ped = this.BuscarPedido(idpedido);
            this.context.Pedidos.Remove(ped);

            List<ProductosPedido> productosped = this.context.ProductosPedidos.Where(z => z.Pedido == idpedido).ToList();

            foreach(ProductosPedido pp in productosped)
            {
                this.context.ProductosPedidos.Remove(pp);
            }

            this.context.SaveChanges();
        }

        public List<Pedido> GetPedidosUsuario(int idusuario)
        {
            return this.context.Pedidos.Where(z => z.Usuario == idusuario).ToList();
        }

        public Carrito GetProductosPedido(int idpedido)
        {
            List<ProductosPedido> productosped = this.context.ProductosPedidos.Where(z => z.Pedido == idpedido).ToList();
            Carrito carrito = new Carrito();
            carrito.Cantidades = new List<int>();
            carrito.Productos = new List<int>();

            foreach(ProductosPedido pp in productosped)
            {
                carrito.Cantidades.Add(pp.Cantidad);
                carrito.Productos.Add(pp.Producto);
            }

            return carrito;
        }

        #endregion

        #region MISC

        public int GetMaxId(Tablas tabla)
        {
            if(tabla == Tablas.Usuarios)
            {
                if(this.context.Usuarios.Count() != 0)
                {
                    return this.context.Usuarios.Max(z => z.IdUsuario) + 1;
                } else
                {
                    return 1;
                }
                
            } else if(tabla == Tablas.Productos)
            {
                if (this.context.Productos.Count() != 0)
                {
                    return this.context.Productos.Max(z => z.IdProducto) + 1;
                }
                else
                {
                    return 1;
                }
            } else if(tabla == Tablas.Pedidos)
            {
                if (this.context.Pedidos.Count() != 0)
                {
                    return this.context.Pedidos.Max(z => z.IdPedido) + 1;
                }
                else
                {
                    return 1;
                }
            } else if(tabla == Tablas.Categorias)
            {
                if (this.context.Categorias.Count() != 0)
                {
                    return this.context.Categorias.Max(z => z.IdCategoria) + 1;
                }
                else
                {
                    return 1;
                }
            } else if(tabla == Tablas.ProductosPedido)
            {
                if (this.context.ProductosPedidos.Count() != 0)
                {
                    return this.context.ProductosPedidos.Max(z => z.IdProductosPedido) + 1;
                }
                else
                {
                    return 1;
                }
            } else
            {
                return -1;
            }
        }

        #endregion

    }
}
