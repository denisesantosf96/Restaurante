using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Restaurante.Models;
using X.PagedList;
using Microsoft.AspNetCore.Http;

namespace Restaurante.Controllers
{
    public class ClienteController : Controller
    {
        private readonly ILogger<ClienteController> _logger;  
        private readonly DadosContext _context ;
        const int itensPorPagina = 5; 

        public ClienteController(ILogger<ClienteController> logger, DadosContext context)
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
            List<Cliente> clientes = _context.RetornarLista<Cliente>("sp_consultarCliente", parametros);
            
            return View(clientes.ToPagedList(numeroPagina, itensPorPagina));
        }

        public IActionResult Detalhe(int id)
        {
            Models.Cliente cliente = new Models.Cliente();
            if (id > 0)  {
                MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
                cliente = _context.ListarObjeto<Cliente>("sp_buscarClientePorId", parametros); 
            }
                   
            return View(cliente);
        }

        [HttpPost]
        public IActionResult Detalhe(Models.Cliente cliente){
            if(string.IsNullOrEmpty(cliente.Nome)){
                ModelState.AddModelError("", "O nome não pode ser vazio");
            }
            if(string.IsNullOrEmpty(cliente.Cpf)){
                ModelState.AddModelError("", "O CPF deve ser preenchido");
            }
            if(string.IsNullOrEmpty(cliente.Rg)){
                ModelState.AddModelError("", "O RG deve ser preenchido");
            }
            if(string.IsNullOrEmpty(cliente.Telefone)){
                ModelState.AddModelError("", "O Telefone deve ser preenchido");
            }
            if(string.IsNullOrEmpty(cliente.Logradouro)){
                ModelState.AddModelError("", "O Logradouro deve ser informado");
            }
            if(string.IsNullOrEmpty(cliente.Bairro)){
                ModelState.AddModelError("", "O Bairro deve ser informado");
            }
            if(string.IsNullOrEmpty(cliente.Cep)){
                ModelState.AddModelError("", "O Cep deve ser informado");
            }
            if(string.IsNullOrEmpty(cliente.Estado)){
                ModelState.AddModelError("", "O Estado deve ser informado");
            }
            if(string.IsNullOrEmpty(cliente.Genero)){
                ModelState.AddModelError("", "O Gênero não pode deixar em branco");
            }
            if(cliente.DataNascimento == null){
                ModelState.AddModelError("", "A Data de Nascimento deve ser informada");
            }

            if(ModelState.IsValid){
           
                List<MySqlParameter> parametros = new List<MySqlParameter>(){
                    new MySqlParameter("nome", cliente.Nome),
                    new MySqlParameter("cpf", cliente.Cpf),
                    new MySqlParameter("rg", cliente.Rg),
                    new MySqlParameter("telefone", cliente.Telefone),
                    new MySqlParameter("logradouro", cliente.Logradouro),
                    new MySqlParameter("numero", cliente.Numero),
                    new MySqlParameter("bairro", cliente.Bairro),
                    new MySqlParameter("cep", cliente.Cep),
                    new MySqlParameter("cidade", cliente.Cidade),
                    new MySqlParameter("estado", cliente.Estado),
                    new MySqlParameter("genero", cliente.Genero),
                    new MySqlParameter("datanascimento", cliente.DataNascimento)

                };
                if (cliente.Id > 0){
                    parametros.Add(new MySqlParameter("identificacao", cliente.Id));
                }
                var retorno = _context.ListarObjeto<RetornoProcedure>(cliente.Id > 0? "sp_atualizarCliente" : "sp_inserirCliente", parametros.ToArray());
            
                if (retorno.Mensagem == "Ok"){
                    return RedirectToAction("Index");
                } else {
                    ModelState.AddModelError("", retorno.Mensagem);
                    
                }
            }
            return View(cliente);
        }

        public JsonResult Excluir(int id){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
            var retorno = _context.ListarObjeto<RetornoProcedure>("sp_excluirCliente", parametros);
            return new JsonResult(new {Sucesso = retorno.Mensagem == "Excluído", Mensagem = retorno.Mensagem });
        }

        public PartialViewResult ListaPartialView(string nome){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_nome", nome)
            };
            List<Cliente> clientes = _context.RetornarLista<Cliente>("sp_consultarCliente", parametros);
            if (string.IsNullOrEmpty(nome)){
                HttpContext.Session.Remove("TextoPesquisa");
            } else {
            HttpContext.Session.SetString("TextoPesquisa", nome);
            }
            return PartialView(clientes.ToPagedList(1, itensPorPagina));
        }
        
    }
}