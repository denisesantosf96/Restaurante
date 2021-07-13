using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Restaurante.Models;
using X.PagedList;


namespace Restaurante.Controllers
{
    public class PedidoController : Controller
    {
        private readonly ILogger<PedidoController> _logger;  
        private readonly DadosContext _context ;
        const int itensPorPagina = 5; 

        public PedidoController(ILogger<PedidoController> logger, DadosContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        public IActionResult Index(int? pagina)
        {
            
            var idRestaurante = 1;
            DateTime? datainicial = null;
            DateTime? datafinal = null;
            var status = HttpContext.Session.GetString("status");   
            var datainicialstring = HttpContext.Session.GetString("datainicial");
            if (!string.IsNullOrEmpty(datainicialstring)){ //faz o if se a string não for vazia
                datainicial = DateTime.Parse(datainicialstring);
            }
            var datafinalstring = HttpContext.Session.GetString("datafinal"); 
            if (!string.IsNullOrEmpty(datafinalstring)){
                datafinal = DateTime.Parse(datafinalstring);
            }     
            int numeroPagina = (pagina ?? 1);
            

            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_status", status),
                new MySqlParameter("datainicial", datainicial),
                new MySqlParameter("datafinal", datafinal),
                new MySqlParameter("_idRestaurante", idRestaurante)
                
            };
            List<Pedido> pedidos = _context.RetornarLista<Pedido>("sp_pesquisarPedido", parametros);
            
            ViewBagRestaurantes();
            return View(pedidos.ToPagedList(numeroPagina, itensPorPagina));
        }

        public IActionResult Detalhe(int id, int idrestaurante)
        {

            Models.Pedido pedido = new Models.Pedido();
            if (id > 0)  {
                MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            } ;
                pedido = _context.ListarObjeto<Pedido>("sp_buscarPedidoPorId", parametros); 
            } else {
                pedido.IdRestaurante = idrestaurante;
                pedido.Data = DateTime.Now;
            }
            
            ViewBagProdutos();
            ViewBagGarcons();
            ViewBagClientes();    
            ViewBagMesas(id > 0 ? pedido.IdRestaurante : idrestaurante);
            return View(pedido);
        }



        [HttpPost]
        public IActionResult Detalhe(Models.Pedido pedido){
            

            if(ModelState.IsValid){
           
                List<MySqlParameter> parametros = new List<MySqlParameter>(){
                    new MySqlParameter("idmesa", pedido.IdMesa),
                    new MySqlParameter("idcliente", pedido.IdCliente),
                    new MySqlParameter("data", pedido.Data),
                    new MySqlParameter("status", pedido.Status),
                    new MySqlParameter("qtde_itens", pedido.QuantidadeDeItens)
                    
                };
                if (pedido.Id > 0){
                    parametros.Add(new MySqlParameter("identificacao", pedido.Id));
                    parametros.Add(new MySqlParameter("formapagamento", pedido.FormaPagamento));
                    parametros.Add(new MySqlParameter("valor", pedido.Valor)); 
                }
                var retorno = _context.ListarObjeto<RetornoProcedure>(pedido.Id > 0? "sp_atualizarPedido" : "sp_inserirPedido", parametros.ToArray());
            
                if (retorno.Mensagem == "Ok"){
                    return RedirectToAction("Index");
                } else {
                    ModelState.AddModelError("", retorno.Mensagem);
                    
                }
            }

            ViewBagProdutos();
            ViewBagGarcons();
            ViewBagClientes();
            ViewBagMesas(pedido.IdRestaurante);
            return View(pedido);
        }

        public JsonResult Excluir(int id){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
            var retorno = _context.ListarObjeto<RetornoProcedure>("sp_excluirPedido", parametros);
            return new JsonResult(new {Sucesso = retorno.Mensagem == "Excluído", Mensagem = retorno.Mensagem });
        }

        public PartialViewResult ListaPartialView(string status, DateTime? datainicial, DateTime? datafinal,
        int idrestaurante){
            
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_status", status),
                new MySqlParameter("datainicial", datainicial),
                new MySqlParameter("datafinal", datafinal),
                new MySqlParameter("_idRestaurante", idrestaurante)
                
            };
            List<Pedido> pedidos = _context.RetornarLista<Pedido>("sp_pesquisarPedido", parametros);
            if (string.IsNullOrEmpty(status)){
                HttpContext.Session.Remove("status");
            } else {
            HttpContext.Session.SetString("status", status);
            }
            if (datainicial == null){
                HttpContext.Session.Remove("datainicial");
            } else {
            HttpContext.Session.SetString("datainicial", datainicial.ToString());
            }
            if (datafinal == null){
                HttpContext.Session.Remove("datafinal");
            } else {
            HttpContext.Session.SetString("datafinal", datafinal.ToString());
            }

            
            
            return PartialView(pedidos.ToPagedList(1, itensPorPagina));
        }

        private void ViewBagRestaurantes(){
            MySqlParameter[] param = new MySqlParameter[]{
                new MySqlParameter("_nome", "")
            };
            List<Models.Restaurante> restaurantes = new List<Models.Restaurante>(); 
            restaurantes = _context.RetornarLista<Models.Restaurante>("sp_consultarRestaurante", param);
            
            ViewBag.Restaurantes = restaurantes.Select(c => new SelectListItem(){
                Text= c.Nome, Value= c.Id.ToString()
            }).ToList();
        }

        private void ViewBagMesas(int idrestaurante){
            MySqlParameter[] param = new MySqlParameter[]{
                new MySqlParameter("_idrestaurante", idrestaurante),
                new MySqlParameter("_localizacao", "")
            };
            List<Models.Mesa> mesas = new List<Models.Mesa>(); 
            mesas = _context.RetornarLista<Models.Mesa>("sp_consultarMesa", param);
            
            ViewBag.Mesas = mesas.Select(c => new SelectListItem(){
                Text= c.NumeroDaMesa +" - "+ c.Localizacao, Value= c.Id.ToString()
            }).ToList();
        }

         private void ViewBagClientes(){
            MySqlParameter[] param = new MySqlParameter[]{
                new MySqlParameter("_nome", "")
            };
            List<Models.Cliente> clientes = new List<Models.Cliente>(); 
            clientes = _context.RetornarLista<Models.Cliente>("sp_consultarCliente", param);
            
            ViewBag.Clientes = clientes.Select(c => new SelectListItem(){
                Text= c.Nome, Value= c.Id.ToString()
            }).ToList();
        }

        private void ViewBagGarcons(){
            MySqlParameter[] param = new MySqlParameter[]{
                new MySqlParameter("_nome", "")
            };
            List<Models.Garcom> garcons = new List<Models.Garcom>(); 
            garcons = _context.RetornarLista<Models.Garcom>("sp_consultarGarcom", param);
            
            ViewBag.Garcons = garcons.Select(c => new SelectListItem(){
                Text= c.Nome, Value= c.Id.ToString()
            }).ToList();
        } 
        private void ViewBagProdutos(){
            MySqlParameter[] param = new MySqlParameter[]{
                new MySqlParameter("_nome", "")
            };
            List<Models.Produto> produtos = new List<Models.Produto>(); 
            produtos = _context.RetornarLista<Models.Produto>("sp_consultarProduto", param);
            
            ViewBag.produtos = produtos.Select(c => new SelectListItem(){
                Text= c.Id +" - "+ c.Nome, Value= c.Id.ToString()
            }).ToList();
        } 
        
    }
}