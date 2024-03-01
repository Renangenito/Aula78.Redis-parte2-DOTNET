using Aula78.Redis.DAL;
using Aula78.Redis.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Aula78.Redis.CarregarRedis
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IDistributedCache _distributedCache;
        private readonly string cepBaseKey = "01001000";

        public Worker(ILogger<Worker> logger, IDistributedCache distributedCache)
        {
            _logger = logger;
            this._distributedCache = distributedCache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                await ValidarRedis();



                await Task.Delay(1000, stoppingToken);
            }
        }

        public async Task ValidarRedis()
        {
            var retornoRedis = await _distributedCache.GetStringAsync(cepBaseKey);

            if (string.IsNullOrWhiteSpace(retornoRedis))
            {
                List<CepDTO> dadosBancoSQL = await new CepDAL().ObterListaCEPBancoDeDadosSQL();

                if(dadosBancoSQL.Count > 0)
                {
                    await AdicionarDadosRedis(dadosBancoSQL);
                }
            }
            
        }
        public async Task AdicionarDadosRedis(List<CepDTO> ceps)
        {
            var memoryCacheEntryPoints = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1),
                //SlidingExpiration = TimeSpan.FromMinutes(10)
            };

            foreach (var item in ceps)
            {
                await _distributedCache.SetStringAsync(item.CEP, JsonConvert.SerializeObject(item), memoryCacheEntryPoints);
            }
        }

    }
}