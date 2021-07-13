using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Restaurante.Models;
using X.PagedList;

namespace Restaurante.Controllers
{
    public class GarcomController : Controller
    {
      private readonly ILogger<GarcomController> _logger;  
        private readonly DadosContext _context ;
        const int itensPorPagina = 5; 

        public GarcomController(ILogger<GarcomController> logger, DadosContext context)
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
            List<Garcom> garcons = _context.RetornarLista<Garcom>("sp_consultarGarcom", parametros);
            
            return View(garcons.ToPagedList(numeroPagina, itensPorPagina));
        }

        public IActionResult Detalhe(int id)
        {
            Models.Garcom garcom = new Models.Garcom();
            if (id > 0)  {
                MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
                garcom = _context.ListarObjeto<Garcom>("sp_buscarGarcomPorId", parametros); 
            }
                   
            return View(garcom);
        }

        [HttpPost]
        public IActionResult Detalhe(Models.Garcom garcom){
            if(string.IsNullOrEmpty(garcom.Nome)){
                ModelState.AddModelError("", "O nome não pode ser vazio");
            }
            if(garcom.Idade < 18){
                ModelState.AddModelError("", "A idade não pode ser menor que 18");
            }

            if(ModelState.IsValid){
           
                List<MySqlParameter> parametros = new List<MySqlParameter>(){
                    new MySqlParameter("nome", garcom.Nome),
                    new MySqlParameter("idade", garcom.Idade),
                    new MySqlParameter("dataadmissao", garcom.DataAdmissao)

                };
                if (garcom.Id > 0){
                    parametros.Add(new MySqlParameter("identificacao", garcom.Id));
                }
                var retorno = _context.ListarObjeto<RetornoProcedure>(garcom.Id > 0? "sp_atualizarGarcom" : "sp_inserirGarcom", parametros.ToArray());
            
                if (retorno.Mensagem.ToLower() == "ok"){
                    return RedirectToAction("Index");
                } else {
                    ModelState.AddModelError("", retorno.Mensagem);
                }
            }
            return View(garcom);
        }

        public JsonResult Excluir(int id){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
            var retorno = _context.ListarObjeto<RetornoProcedure>("sp_excluirGarcom", parametros);
            return new JsonResult(new {Sucesso = retorno.Mensagem == "Excluído", Mensagem = retorno.Mensagem });
        }

        public PartialViewResult ListaPartialView(string nome){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_nome", nome)
            };
            List<Garcom> garcons = _context.RetornarLista<Garcom>("sp_consultarGarcom", parametros);
            if (string.IsNullOrEmpty(nome)){
                HttpContext.Session.Remove("TextoPesquisa");
            } else {
            HttpContext.Session.SetString("TextoPesquisa", nome);
            }
            return PartialView(garcons.ToPagedList(1, itensPorPagina));
        }  
    }
}