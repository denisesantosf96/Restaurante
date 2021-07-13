using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Restaurante.Models;
using X.PagedList;
using Microsoft.AspNetCore.Http;

namespace Restaurante.Controllers
{
    public class RestauranteController : Controller
    {
        private readonly ILogger<RestauranteController> _logger;
        private readonly DadosContext _context ;
        const int itensPorPagina = 5;
  
        public RestauranteController(ILogger<RestauranteController> logger, DadosContext context)
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
            List<Restaurante.Models.Restaurante> restaurantes = _context.RetornarLista<Models.Restaurante>("sp_consultarRestaurante", parametros);
            
            return View(restaurantes.ToPagedList(numeroPagina, itensPorPagina));
        }

        public IActionResult Detalhe(int id)
        {
            Models.Restaurante restaurante = new Models.Restaurante();
            if (id > 0)  {
                MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
                restaurante = _context.ListarObjeto<Models.Restaurante>("sp_buscarRestaurantePorId", parametros); 
            }
                   
            return View(restaurante);
        }

        [HttpPost]
        public IActionResult Detalhe(Models.Restaurante restaurante){
            if(string.IsNullOrEmpty(restaurante.Nome)){
                ModelState.AddModelError("", "O nome não pode ser vazio");
            } 
            if(restaurante.Cnpj.ToString().Length < 14){
                ModelState.AddModelError("", "O Cnpj não pode ser menor que 14 caracteres");
            }
            if(string.IsNullOrEmpty(restaurante.HorarioFuncionamento)){
                ModelState.AddModelError("", "O Horário de Funcionamento deve ser informado");
            }
            if(string.IsNullOrEmpty(restaurante.QuantidadeMaxima)){
                ModelState.AddModelError("", "A Quantidade Máxima deve ser informada");
            }
            if(string.IsNullOrEmpty(restaurante.Logradouro)){
                ModelState.AddModelError("", "O Logradouro deve ser informado");
            }
            if(string.IsNullOrEmpty(restaurante.Bairro)){
                ModelState.AddModelError("", "O Bairro deve ser informado");
            }
            if(string.IsNullOrEmpty(restaurante.Cep)){
                ModelState.AddModelError("", "O Cep deve ser informado");
            }
            if(string.IsNullOrEmpty(restaurante.Estado)){
                ModelState.AddModelError("", "O Estado deve ser informado");
            }
            if(string.IsNullOrEmpty(restaurante.Pais)){
                ModelState.AddModelError("", "O País deve ser informado"); //TODO: delimitar tamanho da string em todos
            }
            if(ModelState.IsValid){
           
                List<MySqlParameter> parametros = new List<MySqlParameter>(){
                    new MySqlParameter("nome", restaurante.Nome),
                    new MySqlParameter("cnpj", restaurante.Cnpj),
                    new MySqlParameter("telefone", restaurante.Telefone),
                    new MySqlParameter("horariofuncionamento", restaurante.HorarioFuncionamento),
                    new MySqlParameter("quantidademaxima", restaurante.QuantidadeMaxima),
                    new MySqlParameter("logradouro", restaurante.Logradouro),
                    new MySqlParameter("numero", restaurante.Numero),
                    new MySqlParameter("bairro", restaurante.Bairro),
                    new MySqlParameter("cep", restaurante.Cep),
                    new MySqlParameter("cidade", restaurante.Cidade),
                    new MySqlParameter("estado", restaurante.Estado),
                    new MySqlParameter("pais", restaurante.Pais)

                };
                if (restaurante.Id > 0){
                    parametros.Add(new MySqlParameter("identificacao", restaurante.Id));
                }
                var retorno = _context.ListarObjeto<RetornoProcedure>(restaurante.Id > 0? "sp_atualizarRestaurante" : "sp_inserirRestaurante", parametros.ToArray());
            
                if (retorno.Mensagem == "Ok"){
                    return RedirectToAction("Index");
                } else {
                    ModelState.AddModelError("", retorno.Mensagem);
                    
                }
            }
            return View(restaurante);
        }

        public JsonResult Excluir(int id){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
            var retorno = _context.ListarObjeto<RetornoProcedure>("sp_excluirRestaurante", parametros);
            return new JsonResult(new {Sucesso = retorno.Mensagem == "Excluído", Mensagem = retorno.Mensagem });
        }

        public PartialViewResult ListaPartialView(string nome){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_nome", nome)
            };
            List<Restaurante.Models.Restaurante> restaurantes = _context.RetornarLista<Models.Restaurante>("sp_consultarRestaurante", parametros);
            if (string.IsNullOrEmpty(nome)){
                HttpContext.Session.Remove("TextoPesquisa");
            } else {
            HttpContext.Session.SetString("TextoPesquisa", nome);
            }

            return PartialView(restaurantes.ToPagedList(1, itensPorPagina));
        }
    }
}