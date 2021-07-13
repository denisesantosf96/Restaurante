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
    public class ItensPedidoController : Controller
    {
       private readonly ILogger<ItensPedidoController> _logger;  
        private readonly DadosContext _context ;
        const int itensPorPagina = 5; 

        public ItensPedidoController(ILogger<ItensPedidoController> logger, DadosContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        public IActionResult Index(int? pagina)
        {
            var idpedido = HttpContext.Session.GetString("TextoPesquisa");          
            int numeroPagina = (pagina ?? 1);

            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_idPedido", idpedido)
            };
            List<ItensPedido> itenspedidos = _context.RetornarLista<ItensPedido>("sp_consultarItensPedido", parametros);
            
            return View(itenspedidos.ToPagedList(numeroPagina, itensPorPagina));
        }

        public IActionResult Editar(int id)
        {
            Models.ItensPedido itenspedido = new Models.ItensPedido();
            if (id > 0)  {
                MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
                itenspedido = _context.ListarObjeto<ItensPedido>("sp_buscarItensPedidoPorId", parametros); 
            } 

                  
            return new JsonResult(new {Sucesso = itenspedido.Id > 0, Itenspedido = itenspedido});
        }

        [HttpPost]
        public IActionResult Salvar(Models.ItensPedido itenspedido){
            
            string mensagem = "";

            
           
                List<MySqlParameter> parametros = new List<MySqlParameter>(){
                    new MySqlParameter("idpedido", itenspedido.IdPedido),
                    new MySqlParameter("idgarcom", itenspedido.IdGarcom),
                    new MySqlParameter("idproduto", itenspedido.IdProduto),
                    new MySqlParameter("quantidade", itenspedido.Quantidade),
                    new MySqlParameter("valoritem", itenspedido.ValorItem)
                };
                if (itenspedido.Id > 0){
                    parametros.Add(new MySqlParameter("identificacao", itenspedido.Id));
                }
                var retorno = _context.ListarObjeto<RetornoProcedure>(itenspedido.Id > 0? "sp_atualizarItensPedido" : "sp_inserirItensPedido", parametros.ToArray());
            
                if (retorno.Mensagem == "Ok"){
                    return new JsonResult(new {Sucesso = retorno.Mensagem == "Ok"});
                } else {
                    mensagem = retorno.Mensagem;
                    
                }
            

            
            return new JsonResult(new {Sucesso = false, Mensagem = mensagem});
        }

        public JsonResult Excluir(int id){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
            var retorno = _context.ListarObjeto<RetornoProcedure>("sp_excluirItensPedido", parametros);
            return new JsonResult(new {Sucesso = retorno.Mensagem == "Excluído", Mensagem = retorno.Mensagem });
        }

        public PartialViewResult ListaPartialView(int idpedido){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_idPedido", idpedido)
            };
            List<ItensPedido> itenspedidos = _context.RetornarLista<ItensPedido>("sp_consultarItensPedido", parametros);
            if (idpedido != 0){
                HttpContext.Session.Remove("TextoPesquisa");
            } else {
            HttpContext.Session.SetInt32("TextoPesquisa", idpedido);
            }
            return PartialView(itenspedidos.ToPagedList(1, itensPorPagina));
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