﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using RestauranteWeb.Models;
using System.Reflection.Emit;
using System.Data.Entity.Validation;
using System.Web.Security;
using System.Data.Entity;

namespace RestauranteWeb.Controllers
{
    public class ClienteController : Controller
    {

        private ProyectoASP_RestauranteEntities db = new ProyectoASP_RestauranteEntities();
        private CarritoCompras carrito = new CarritoCompras();
        // GET: Cliente

        // INICIO METODOS INICIO
        public ActionResult Inicio()
        {
            var model = new ModelInicioCliente();
            model.combos = db.Combos.ToList();
            model.categorias = db.CategoriasProductos.ToList();

            //Insertar aca código para mostrar en página principal.
            return View(model);
        }
        // FIN METODOS INICIO

        //INICIO METODOS CATEGORIAS
        public ActionResult Categorias()
        {
            var model = new ModelInicioCliente();
            model.combos = db.Combos.ToList();
            model.categorias = db.CategoriasProductos.ToList();
            model.productos = db.ProductosRestaurante.ToList();

            return View(model);
        }

        public JsonResult obtenerDetalle(string idCombo)
        {
            
            List<CombosDetalle> combo = new List<CombosDetalle>();
            combo = db.CombosDetalle.Where(c=>c.IdCombo == idCombo).ToList();
            var comboClean = combo.Select(s => new {
                s.ProductosRestaurante.Imagen,
                s.ProductosRestaurante.Nombre,
                s.Cantidad,
                s.Combos.Descripcion
            });

            return Json(comboClean, JsonRequestBehavior.AllowGet);

        }

        public JsonResult obtenerDetalleCarrito(string idObjeto)
        {
            var combo = db.Combos.Find(idObjeto);
            if (db.Combos.Find(idObjeto) != null)
            {
                return obtenerDetalle(idObjeto);
            }
            else
            {
                return Json(new { isCombo = false, descripcion = (db.ProductosRestaurante.Find(idObjeto)).Descripcion });
            }

        }
        // FIN METODOS CATEGORIAS

        // INICIO METODOS CARRITO COMPRAS

        public int registrarObjetos(string idObjeto, string nombre, int cantidad, decimal precio, string imagen)
        {
                //Verificando que el objeto no exista
                if (carrito.existeObjeto(idObjeto))
                {
                    carrito.modificarObjeto(idObjeto, cantidad);
                }
                else
                {
                    carrito.insertarObjeto(idObjeto, nombre, cantidad, precio, imagen);
                }

            return carrito.countObjetos();
        }   
        
        public int countObjetos()
        {
            return carrito.countObjetos();
        }
        public JsonResult generarCarritoPlegable()
        {
            if(CarritoCompras.objetos!= null && CarritoCompras.objetos.Count!=0)
            {
                return Json(CarritoCompras.objetos.Select(s=>new {
                    s.idObjeto,
                    s.imagen,
                    s.nombre,
                    s.precio,
                    s.cantidad,
                    total = carrito.obtenerMonto()
                }));
            }
            else
            {
                return Json(new { success = false, message = "Todavía no hay productos" });
            }
        }

        public int eliminarProducto(string idObjeto)
        {
            carrito.eliminarProducto(idObjeto);

            return carrito.countObjetos();
        }

        public ActionResult Carrito()
        {
            var model = new ModelCarritoDetalles();

            model.carrito = CarritoCompras.objetos;
            model.total = carrito.obtenerMonto();

            return View(model);
        }

        public JsonResult cambiarCantidad(string idObjeto, int cantidad)
        {

            return Json( new { totalObjeto = carrito.cambiarCantidad(idObjeto, cantidad),
                               monto = carrito.obtenerMonto()});

        }

        public JsonResult reloadTablaCarrito()
        {
            if (CarritoCompras.objetos != null && CarritoCompras.objetos.Count != 0)
            {
                return Json(CarritoCompras.objetos.Select(s => new {
                    s.idObjeto,
                    s.imagen,
                    s.nombre,
                    s.precio,
                    s.cantidad,
                    monto = s.cantidad*s.precio
                }));
            }
            else
            {
                return null;
            }
        }


        // FIN METOROS CARRITO COMPRAS
        public ActionResult Encuentranos()
        {
            return View();
        }

        //INICIO METODOS PAGO
        
        public ActionResult Pago()
        {
            string usuario = System.Web.HttpContext.Current.Session["id"] as String;

            var model = new ModelPagoPedido();
            model.cliente = db.CuentasClientes.Find(usuario);
            model.objetos = CarritoCompras.objetos;
            model.total = carrito.obtenerMonto();

            return View(model);
        }

        public string modificarDireccion(string direccion)
        {
            string usuario = System.Web.HttpContext.Current.Session["id"] as String;

            var cliente = db.CuentasClientes.Find(usuario);
            cliente.DireccionEntrega = direccion;
            cliente.ConfimarClave = cliente.Clave;

            db.Entry(cliente).State = EntityState.Modified;
            db.SaveChanges();

            return cliente.DireccionEntrega;
        }


        public string insertarPedido(string direccion, string telefono)
        {
            string usuario = System.Web.HttpContext.Current.Session["id"] as String;
            var pedido = new PedidosClientes();
            pedido.IdPedido = db.GeneradorIdOjetos("PED");
            pedido.IdCliente = usuario;
            pedido.TelefonoContacto = telefono;
            pedido.DireccionEntrega = direccion;
            pedido.Fecha = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            pedido.Cancelado = false;

            db.PedidosClientes.Add(pedido);
            

            //Registrando detalles del pedido
            
            foreach(var item in CarritoCompras.objetos)
            {
                var detalle = new PedidosClientesDetalles();
                detalle.IdPedido = pedido.IdPedido;
                detalle.IdObjeto = item.idObjeto;
                detalle.Cantidad = item.cantidad;
                db.PedidosClientesDetalles.Add(detalle);
            }

            //Registrando el trakeo en Solicitud id 1

            var trakeo = new TrackeoPedidosClientes();
            trakeo.IdPedido = pedido.IdPedido;
            trakeo.IdEtapa = 1;
            trakeo.Fecha = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            db.TrackeoPedidosClientes.Add(trakeo);

            //guardando cambios
            db.SaveChanges();

            //limpiando carrito
            CarritoCompras.objetos = null;

            return pedido.IdPedido;
        }
        // FIN METODOS PAGO

        //INICIO METODOS PERFIL

        public ActionResult Perfil()
        {
            string usuario = System.Web.HttpContext.Current.Session["id"] as String;

            return View(db.CuentasClientes.Find(usuario));
        }

        public void modificarCliente(string nombres, string apellidos, string telefono, string movil, string correo, string direccion, string direccionEntrega)
        {
            string usuario = System.Web.HttpContext.Current.Session["id"] as String;
            var cliente = db.CuentasClientes.Find(usuario);

            cliente.Nombres = nombres;
            cliente.Apellidos = apellidos;
            cliente.TelefonoFijo = telefono;
            cliente.TelefonoCelular = movil;
            cliente.Correo = correo;
            cliente.Direccion = direccion;
            cliente.DireccionEntrega = direccionEntrega;
            
            db.Entry(cliente).State = EntityState.Modified;
            db.Configuration.ValidateOnSaveEnabled = false;
            db.SaveChanges();

        }

        public void modificarClave(string clave)
        {
            string usuario = System.Web.HttpContext.Current.Session["id"] as String;
            var cliente = db.CuentasClientes.Find(usuario);

            cliente.Clave = Crypto.Hash(clave);

            db.Entry(cliente).State = EntityState.Modified;
            db.Configuration.ValidateOnSaveEnabled = false;
            db.SaveChanges();

        }

        public ActionResult Historial()
        {
            string usuario = System.Web.HttpContext.Current.Session["id"] as String;
            var model = new ModelHistorialOrdenes();

            model.pedidosClientes = db.PedidosClientes.Where(x=>x.IdCliente == usuario).ToList();

            return View(model);
        }

        [HttpPost]
        public JsonResult obtenerClienteOrden(string idPedido)
        {
            PedidosClientes pedidos = db.PedidosClientes.Find(idPedido);

            return Json(new
            {
                nombre = (pedidos.CuentasClientes.Nombres + " " + pedidos.CuentasClientes.Apellidos),
                direccion = pedidos.DireccionEntrega,
                telefono = pedidos.TelefonoContacto,
                correo = pedidos.CuentasClientes.Correo,
                fecha = pedidos.Fecha,
                estado = pedidos.TrackeoPedidosClientes.EtapasPedidos.Nombre,
                total = db.ObtenerMontoPedido(idPedido)
            });
        }

        [HttpPost]
        public JsonResult obtenerDetalleOrden(string idPedido)
        {
            var detalle = db.ObtenerDetallePedido(idPedido);

            return Json(detalle, JsonRequestBehavior.AllowGet);
        }
        //FIN METODOS PERFIL




        [HttpGet]
        public ActionResult Registrarme()
        {
            return View();
        }

        //Obtener datos de el formulario de registro
        [HttpPost]
        public ActionResult Registrarme([Bind(Exclude = "Validado,IdCliente,Fecha")] CuentasClientes cuentasClientes)
        {
            bool Status = false;
            string Message = "";
            //validar en el modelo
            if (ModelState.IsValid)
            {
                
                #region //Validar si existe el correo
                    
                var existeEmail = Correo_existe(cuentasClientes.Correo);
                if (existeEmail)
                {
                    ModelState.AddModelError("Correo_Existe","Error: este correo ya se encuentra en uso");
                    return View(cuentasClientes);
                }
                #endregion

                #region contraseña hash
                cuentasClientes.Clave = Crypto.Hash(cuentasClientes.ConfimarClave);
                cuentasClientes.ConfimarClave = Crypto.Hash(cuentasClientes.ConfimarClave);
                #endregion

                cuentasClientes.Validado = false;

                int asar = new Random().Next(0, 100000); ;
                string codigo = "TC" + asar.ToString();
                var Existe_id = Id_extiste(codigo);
                if (Existe_id)
                {
                    
                }
                else
                {
                    do
                    {
                         asar = new Random().Next(0, 100000); ;
                         codigo = "TC" + asar.ToString();
                         Existe_id = Id_extiste(codigo);
                    } while (Existe_id==true);
                    
                }
                cuentasClientes.IdCliente = codigo;

                DateTime now = DateTime.Now;
                cuentasClientes.Fecha = now;


                #region guardar en base de datos 
                using (ProyectoASP_RestauranteEntities pr = new ProyectoASP_RestauranteEntities())
                {
                    pr.CuentasClientes.Add(cuentasClientes);
                    
                        pr.SaveChanges();
                        //enviar email de validacion;
                        Correo_Verificaion(cuentasClientes.Correo, cuentasClientes.IdCliente.ToString());
                        Message = "¡Usted ha sido registrado exitosamente! \n Revise su correo electrónico para validar su cuenta: " + cuentasClientes.Correo;
                        Status = true;
                }
                #endregion

                
            }
            else
            {
                Message = "Verifique que haya rellenado correctamente los campos";
            }
            ViewBag.Status = Status;
            ViewBag.Message = Message;

            return View(cuentasClientes);
        }

        //verificar cuenta
        [HttpGet]
        public ActionResult VerificarCuenta(string id)
        {
            bool Status = false;
            using (ProyectoASP_RestauranteEntities pr = new ProyectoASP_RestauranteEntities())
            {
                pr.Configuration.ValidateOnSaveEnabled = false;// esta linea confirma que las contraseñas no coinsidas
                var v = pr.CuentasClientes.Where(a => a.IdCliente == id).FirstOrDefault();
                if (v!=null)
                {
                    v.Validado = true;
                    pr.SaveChanges();
                    Status = true;
                    ViewBag.Status = Status;
                    return View();
                }
                else
                {
                    ViewBag.Message = "No se pudo validar";
                    return RedirectToAction("inicio", "Cliente");
                }

            }
            
        }


        //Login
       /* [HttpGet]
        public ActionResult Login()
        {
            return View();
        }*/
        //Login POST
        [HttpPost]
        public JsonResult Login(Logiarme login,string returnUrl="")
        {
            
            using(ProyectoASP_RestauranteEntities pr = new ProyectoASP_RestauranteEntities())
            {
                var v = pr.CuentasClientes.Where(a => a.Correo == login.Correo).FirstOrDefault();
                if (v != null)
                {
                    if (string.Compare(Crypto.Hash(login.Clave), v.Clave) == 0)
                    {
                        int timeout = login.recordarme ? 52560 : 20; //525600 es una hora
                        var ticket = new FormsAuthenticationTicket(login.Correo, login.recordarme, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);
                        Session["nombre"] = v.Nombres;
                        Session["id"] = v.IdCliente;
                        Session["email"] = v.Correo;
                        Session["user"] = "Cliente";

                        return Json(1);
                    }
                    else
                    {
                        return Json(3);
                    }

                }
                else
                {
                    var emp = pr.CuentasEmpleados.Where(a => a.Usuario == login.Correo).FirstOrDefault();
                    if (emp != null)
                    {
                        if (string.Compare(Crypto.Hash(login.Clave), emp.Clave) == 0)
                        {
                            int timeout = login.recordarme ? 52560 : 20; //525600 es una hora
                            var ticket = new FormsAuthenticationTicket(login.Correo, login.recordarme, timeout);
                            string encrypted = FormsAuthentication.Encrypt(ticket);
                            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                            cookie.Expires = DateTime.Now.AddMinutes(timeout);
                            cookie.HttpOnly = true;
                            Response.Cookies.Add(cookie);
                            Session["nombre"] = emp.Nombres+ " "+emp.Apellidos;
                            Session["id"] = emp.Usuario;


                            //hay que hacer la comparacion si es delivery , admin o restaurante
                            if (emp.IdRol == 1)
                            {
                                Session["user"] = "Admin";
                                return Json(2);
                            }
                            else if (emp.IdRol==2)
                            {
                                Session["user"] = "Restaurante";
                                return Json(5);
                            }
                            else
                            {
                                Session["user"] = "Delivery";
                                return Json(6);
                            }
                        }
                        else
                        {
                            return Json(3);
                        }
                    }
                    else
                    {
                        return Json(4);
                    }
                }
            }
            
        }

        //Logout
        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session["nombre"] = null;
            Session["id"] = null;
            Session["email"] = null;
            Session["user"] = null;
            //Limpiando carrito de compras
            CarritoCompras.objetos = null;

            return RedirectToAction("inicio","Cliente");
        }



        [HttpPost]
        public JsonResult RecuperacionCuenta(string Correo)
        {
            using (ProyectoASP_RestauranteEntities pr = new ProyectoASP_RestauranteEntities())
            {
                pr.Configuration.ValidateOnSaveEnabled = false;
                var cl = pr.CuentasClientes.Where(a => a.Correo == Correo).FirstOrDefault();
                if(cl != null)
                {
                    string nueva = "EsperandoClave";
                    cl.Clave = Crypto.Hash(nueva);
                    cl.Validado = false;
                    string id = cl.IdCliente;
                    pr.SaveChanges();
                    
                    mandarCorreo(Correo,id);
                    return Json(1); //Existosamente guardado

                }
                else
                {
                    return Json(3);
                }
            }
        }


        [HttpGet]
        public ActionResult MandarAdmin()
        {
            return RedirectToAction("Inicio", "Administrador");
        }

        [HttpGet]
        public ActionResult MandarDelivery()
        {
            return RedirectToAction("Index", "Delivery");
        }

        [HttpGet]
        public ActionResult MandarRestaurante()
        {
            return RedirectToAction("Index", "Restaurante");
        }



        [HttpGet]
        public ActionResult CambiarContra(string id)
        {
            using (ProyectoASP_RestauranteEntities pr = new ProyectoASP_RestauranteEntities())
            {
                string nueva = "EsperandoClave";
                nueva = Crypto.Hash(nueva);
                var cl = pr.CuentasClientes.Where(a => a.IdCliente == id).Where(b=>b.Clave==nueva).FirstOrDefault();
                if(cl != null)
                {
                    ViewBag.idCliente = id;
                    return View();
                }
                else
                {
                    return RedirectToAction("inicio", "Cliente");
                }
            }
        }

        [HttpPost]
        public JsonResult CambiarContraNueva(string contra,string id)
        {
            using (ProyectoASP_RestauranteEntities pr = new ProyectoASP_RestauranteEntities())
            {
                pr.Configuration.ValidateOnSaveEnabled = false;// esta linea confirma que las contraseñas no coinsidas
                string nueva = Crypto.Hash(contra);
                var cl = pr.CuentasClientes.Where(a => a.IdCliente == id).FirstOrDefault();
                if (cl != null)
                {
                    cl.Clave = nueva;
                    cl.Validado = true;
                    pr.SaveChanges();
                    return Json(1);
                }
                else
                {
                    //Usuario no encontrado
                    return Json(2);
                }
            }
        }


        [NonAction]
        public bool Correo_existe (string Correo)
        {
            using(ProyectoASP_RestauranteEntities pre = new ProyectoASP_RestauranteEntities())
            {
                var v = pre.CuentasClientes.Where(a => a.Correo == Correo).FirstOrDefault();
                return v != null;
            }
        }

        [NonAction]
        public bool Id_extiste(string id)
        {
            using (ProyectoASP_RestauranteEntities pre = new ProyectoASP_RestauranteEntities())
            {
                var v = pre.CuentasClientes.Where(a => a.IdCliente == id).FirstOrDefault();
                return v != null;
            }
        }

        [NonAction]
        public void Correo_Verificaion(string email, string id)
        {
            var verificarurl = "/Cliente/VerificarCuenta/" + id;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verificarurl);

            var fromEmail = new MailAddress("isidroprueba005@gmail.com", "Burger");
            var toEmail = new MailAddress(email);
            var fromEmailContra = "pruebas-8";
            string subject = "¡Su cuenta ha sido creada exitosamente!";
            string body = "<br><br> Nos encontramos muy felices de que usted quiera formar parte de la familia BurgerCity <br>" +
                "Su cuenta ha sido creada exitosamente. Por favor ingrese al siguiente enlace para finalizar el proceso de verificación de su cuenta:" +
                "<br><br><a href='"+link+"'>"+link+"</a>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailContra)
            };
            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            smtp.Send(message);
        }

        //Correo de reuperacion de cuenta
        [NonAction]
        public void mandarCorreo(string email, string id)
        {

            var verificarurl = "/Cliente/CambiarContra/" + id;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verificarurl);

            var fromEmail = new MailAddress("isidroprueba005@gmail.com", "Burger - Recuperar cuenta");
            var toEmail = new MailAddress(email);
            var fromEmailContra = "pruebas-8";
            string subject = "Solicitud de reestablecimiento de contraseña";
            string body = "<br><br> Estimado cliente, su solicitud de reestablecimiento de contraseña ha sido generada exitosamente." +
                " Para reestablecer su contraseña, ingrese al siguiente enlace:" +
                "<br><br><a href='" + link + "'>" + link + "</a>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailContra)
            };
            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }


    }


    public class Logiarme
    {
        public string Correo { get; set; }
        public string Clave { get; set; }
        public bool recordarme { get; set; }

    }

   
}