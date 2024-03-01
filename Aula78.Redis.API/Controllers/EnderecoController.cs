using Aula78.Redis.DAL;
using Aula78.Redis.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Aula78.Redis.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnderecoController : ControllerBase
    {
        private readonly IDistributedCache _distributedCache;

        public EnderecoController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        [HttpGet]
        public async Task<CepDTO> GetEnderecoPorCEPRedis(string cep)
        {

            for (int i = 0; i < 3; i++)
            {
                var dadosRedis = await _distributedCache.GetStringAsync(cep);

                if (!string.IsNullOrEmpty(dadosRedis))
                {
                    return JsonConvert.DeserializeObject<CepDTO>(dadosRedis);
                }
                await Task.Delay(5000);
            }
            throw new Exception("Erro: CEP não encontrado!");
        }
        [HttpPut]
        public async Task AtualizarEndereco(CepDTO cep)
        {
            //SQl
            await new CepDAL().AlterarCEPBancoDeDadosSQL(cep);
            //Redis
            await _distributedCache.SetStringAsync(cep.CEP, JsonConvert.SerializeObject(cep));
        }
    }
}
