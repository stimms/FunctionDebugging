using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FunctionDebugging
{
    public static class DecodeVIN
    {
        const string OVERALL_LENGTH = "OL";
        const string OVERALL_WIDTH = "OW";
        const string OVERALL_HEIGHT = "OH";

        static HttpClient client = new HttpClient();
        [FunctionName("DecodeVIN")]
        public static async Task<IActionResult> Decode(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "decode/{vin}")] HttpRequest req, String vin,
            ILogger log)
        {
            log.LogInformation(vin);

            string vinDecodeUrl = $"https://vpic.nhtsa.dot.gov/api/vehicles/decodevinvaluesextended/{vin}?format=json";
            var result = await client.GetStringAsync(vinDecodeUrl);
            var envelope = System.Text.Json.JsonSerializer.Deserialize<DecodedVinEnvelope>(result);
            var decodedVin = envelope.Results.FirstOrDefault();

            string vehicleSpecUrl = $"https://vpic.nhtsa.dot.gov/api/vehicles/GetCanadianVehicleSpecifications/?Year={decodedVin.ModelYear}&Make={decodedVin.Make}&Model={decodedVin.Model}&units=&format=json";
            var vehicleSize = await client.GetStringAsync(vehicleSpecUrl);
            var specEnvelope = System.Text.Json.JsonSerializer.Deserialize<DecodedVehicleSpecEnvelope>(vehicleSize);
            var length = Int32.Parse(specEnvelope.Results.First().Specs.Single(x => x.Name == OVERALL_LENGTH).Value);
            var width =  Int32.Parse(specEnvelope.Results.First().Specs.Single(x => x.Name == OVERALL_WIDTH).Value);
            var height = Int32.Parse(specEnvelope.Results.First().Specs.Single(x => x.Name == OVERALL_HEIGHT).Value);

            return new OkObjectResult(length * width * height);
        }
    }
}
