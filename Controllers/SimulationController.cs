using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Benihana;
using Coravel.Queuing.Interfaces;
using Microsoft.AspNetCore.SignalR;
using BenihanaWebReact.Hubs;
using System.Text.Json;
using System.Text.Json.Serialization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BenihanaWebReact.Controllers
{
    [Route("api/[controller]")]
    public class SimulationController : ControllerBase
    {
        private readonly IQueue _queue;
        private readonly IHubContext<ChatHub> _hubContext;
        public SimulationController(IQueue queue, IHubContext<ChatHub> hubContext)
        {
            _queue = queue;
            _hubContext = hubContext;
        }
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]SimulationConfig config)
        {
            await _hubContext.Clients.All.SendAsync("setClientMessage", "Run simulation");
            Model.Statics statics = MapConfig(config);

            List<Result> result = Enumerable.Range(0, 20).Select(seed =>
            {
                var sim = new Model(statics, seed);
                while (sim.Run(1)) ;
                return sim.Result;
            }).ToList();
            return Ok(result);
        }
        [HttpPost("run/{id}")]
        public IActionResult RunSimulation(int id, [FromBody]RunSimulationModel simulation)
        {       
            _queue.QueueAsyncTask(async () =>
            {
                await Task.Run(async () =>
                 {
                     await _hubContext.Clients.Client(simulation.ConnectionId).SendAsync("setSimulationResults", "testing");
                     Model.Statics scenario = MapConfig(simulation.Config);
                     Model model = new Model(scenario, seed: id);
                     model.WarmUp(TimeSpan.FromHours(scenario.OpeningHour));
                     await  RunSimulationAsync(model, simulation.ConnectionId);
                 });
            });
            return Ok("Ok");
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private Model.Statics MapConfig(SimulationConfig config)
        {
            Model.Statics statics = new Model.Statics();
            statics.BatchingRule = (Model.Statics.BatchingRules)config.Batching;
            statics.NDiningTables = config.LayoutTable;
            statics.ExpectedDiningMinutesbeforePeak = config.DiningTimeBeforePeak;
            statics.ExpectedDiningMinutesduringPeak = config.DiningTimeDuringPeak;
            statics.ExpectedDiningMinutesafterPeak = config.DiningTimeAfterPeak;
            statics.AdsOption = (Model.Statics.AdsOptions)config.Advertisement;
            statics.AdsLevel = config.AdsLevel;
            statics.OpeningHour = config.OpeningHour;

            return statics;
        }

        private async Task RunSimulationAsync(Model model, string connectionId)
        {
            Console.WriteLine("running sim for 10 seconds");
            List<ResultModel> results = new List<ResultModel>();
            do
            {
                model.WriteToConsole();
                ResultModel result = new ResultModel();
                result.BarAvgCust = model.Result.BarAvgCust;
                result.BarAvgDrinksPerCust = model.Result.BarAvgDrinksPerCust;
                result.BarDrinksSold = model.Result.BarDrinksSold;

                results.Add(result);
                await Task.Delay(1000); ;
            } while (model.Run(TimeSpan.FromHours(1)));
            string messageString = JsonSerializer.Serialize(results);
            Console.WriteLine(messageString);
            await _hubContext.Clients.Client(connectionId).SendAsync("setSimulationResults", messageString);
            Console.WriteLine("finished running sim");
        }
    }
}
