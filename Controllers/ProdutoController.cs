using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Restaurante.Models;
using X.PagedList;

namespace Restaurante.Controllers
{
    public class ProdutoController : Controller
    {
        private readonly ILogger<ProdutoController> _logger;  
        private readonly DadosContext _context ;
        const int itensPorPagina = 5; 

        public ProdutoController(ILogger<ProdutoController> logger, DadosContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        public IActionResult Index(int? pagina)
        {
            var nome = HttpContext.Session.GetString("TextoPesquisa");          
            int numeroPagina = (pagina ?? 1);

            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_nome", nome)
            };
            List<Produto> produtos = _context.RetornarLista<Produto>("sp_consultarProduto", parametros);
            
            return View(produtos.ToPagedList(numeroPagina, itensPorPagina));
        }

        public IActionResult Detalhe(int id)
        {
            Models.Produto produto = new Models.Produto();
            if (id > 0)  {
                MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
                produto = _context.ListarObjeto<Produto>("sp_buscarProdutoPorId", parametros); 
            }
                   
            return View(produto);
        }

        [HttpPost]
        public IActionResult Detalhe(Models.Produto produto){
            if(string.IsNullOrEmpty(produto.Nome)){
                ModelState.AddModelError("", "O nome não pode ser vazio");
            }
            if(produto.ValorUnitario == 0){
                ModelState.AddModelError("", "O Valor Unitário deve ser preenchido");
            }

            if(ModelState.IsValid){
           
                List<MySqlParameter> parametros = new List<MySqlParameter>(){
                    new MySqlParameter("nome", produto.Nome),
                    new MySqlParameter("valorunitario", produto.ValorUnitario)
                    
                };
                if (produto.Id > 0){
                    parametros.Add(new MySqlParameter("identificacao", produto.Id));
                }
                var retorno = _context.ListarObjeto<RetornoProcedure>(produto.Id > 0? "sp_atualizarProduto" : "sp_inserirProduto", parametros.ToArray());
            
                if (retorno.Mensagem == "Ok"){
                    return RedirectToAction("Index");
                } else {
                    ModelState.AddModelError("", retorno.Mensagem);
                    
                }
            }
            return View(produto);
        }

        public JsonResult Excluir(int id){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
            var retorno = _context.ListarObjeto<RetornoProcedure>("sp_excluirProduto", parametros);
            return new JsonResult(new {Sucesso = retorno.Mensagem == "Excluído", Mensagem = retorno.Mensagem });
        }

        public PartialViewResult ListaPartialView(string nome){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_nome", nome)
            };
            List<Produto> produtos = _context.RetornarLista<Produto>("sp_consultarProduto", parametros);
            if (string.IsNullOrEmpty(nome)){
                HttpContext.Session.Remove("TextoPesquisa");
            } else {
            HttpContext.Session.SetString("TextoPesquisa", nome);
            }
            return PartialView(produtos.ToPagedList(1, itensPorPagina));
        }
    }
}