using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;

using Dapper;
using System.Data.SqlClient;

namespace DemoWebApi2.Controllers
{
    /// <summary>
    /// Classe de controler com um webhook para eventgrid com autenticação azure ad
    /// Para configurar o eventgrid para auntenticar api protegida pelo AAD 
    /// vide: https://docs.microsoft.com/pt-br/azure/event-grid/secure-webhook-delivery
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [System.Web.Http.RoutePrefix("api/produto")]
    public class ProdutoController : ApiController
    {

        private const string SqlInsertProduto = "Insert into produto(Id, Nome, DataCriacao, DataModificacao, Preco) values (@Id, @Nome, @DataCriacao, @DataModificacao, @Preco)";
        //Exemplo de body para teste da api no postman. Necessario informar um token no header.
        /*
[
{
  "id": "5bdd721c-a4cd-45cf-9e34-4c02cf062c7c",
  "subject": "AssinaturaDocumento",
  "data": {
    "Id": "ade27a30-6289-ea11-8ce1-005056a7283d",
    "Nome": "Teste",
    "Preco": 10
  },
  "eventType": "statusassinatura-insert",
  "eventTime": "2020-04-29T12:49:48.0786034Z",
  "dataVersion": "1.0",
  "metadataVersion": "1",
  "topic": "/subscriptions/eef4cb92-52e8-4756-b6e1-814c16d1db14/resourceGroups/MRV-EventGrid-QAS/providers/Microsoft.EventGrid/domains/egdmrvqas/topics/assinaturadocumento"
}
]

*/

        /// <summary>
        /// Url de webhook a ser chamada pela via event grid pelo MS AssinaturaDocumento
        /// vide: https://docs.microsoft.com/pt-br/azure/event-grid/secure-webhook-delivery
        /// 
        /// </summary>
        [HttpPost()]
        [Route("notificar")]
        [Authorize]
        public IHttpActionResult Notificar([FromBody]EventGridEvent[] ev)
        {
            if (ev == null && ev.Length == 0)
            {
                return BadRequest();
            }
            try
            {
                foreach (EventGridEvent item in ev)
                {
                    if (item.EventType == EventTypes.EventGridSubscriptionValidationEvent)
                    {
                        var data = (item.Data as JObject).ToObject<SubscriptionValidationEventData>();
                        var response = new Microsoft.Azure.EventGrid.Models.SubscriptionValidationResponse(data.ValidationCode);
                        return Ok(response);
                    }
                    else
                    {
                        //Deserializar dados
                        var registro = (item.Data as JObject).ToObject<Models.Produto>();
                        registro.DataCriacao = DateTime.Now;
                        registro.DataModificacao = DateTime.Now;
                        using (var connection = new SqlConnection(ConfigurationManager.AppSettings["db"]))
                        {
                            connection.Open();
                            connection.Execute(SqlInsertProduto, registro);                        
                        }

                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                //Retornar um 403 para o EventGrid fazer o retry.
                return Content(HttpStatusCode.Forbidden, ex.Message);
            }

        }

    }
}
