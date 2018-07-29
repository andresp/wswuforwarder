
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using WUForwarder.Models;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;

namespace WUForwarder
{
    public static class HttpTrigger
    {
        static readonly string wuStationId = "";
        static readonly string wuStationPassword = "";
        static readonly string wuUploadUrl = "http://weatherstation.wunderground.com/weatherstation/updateweatherstation.php";
        static readonly string wuUpdatePreamble = "action=updateraw";
        static readonly string wuIdArg = "ID";
        static readonly string wuPwdArg = "PASSWORD";
        static readonly string wuDateUtcArg = "dateutc";
        static readonly string wuWindDirArg = "winddir";
        static readonly string wuWindSpeedMphArg = "windspeedmph";
        static readonly string wuWindGustDirArg = "windgustdir_10m";
        static readonly string wuWindGustMphArg = "windgustmph_10m";
        static readonly string wuHumidityArg = "humidity";
        static readonly string wuTempFArg = "tempf";
        static readonly string wuPressureArg = "baromin";
        static readonly string wuDewPoint = "dewptf";

        [FunctionName("HttpTrigger")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            var particleMessage = JsonConvert.DeserializeObject<ParticleMessage>(requestBody);
            var data = new StationData(particleMessage.data);

            string dataArg = $"{wuIdArg}={wuStationId}&{wuPwdArg}={wuStationPassword}&{wuDateUtcArg}={WebUtility.UrlEncode(particleMessage.published_at.ToString("yyyy-MM-dd HH:mm:ss"))}&{wuWindDirArg}={data.WindDirection}&{wuWindSpeedMphArg}={data.WindMph.ToString("F2")}&{wuWindGustMphArg}={data.GustMph.ToString("F2")}&{wuWindGustDirArg}={data.GustDirection}&{wuHumidityArg}={data.Humidity.ToString("F2")}&{wuPressureArg}={data.PressureInch.ToString("F2")}&{wuTempFArg}={data.TemperatureF.ToString("F2")}&{wuDewPoint}={data.DewPointF.ToString("F2")}";

            string uri = $"{wuUploadUrl}?{wuUpdatePreamble}&{dataArg}";
            var client = new HttpClient();
            var response = client.GetAsync(uri).GetAwaiter().GetResult();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return (ActionResult)new OkObjectResult(response.Content);
            }

            return new BadRequestObjectResult($"Bad response from WU: {response.StatusCode}. Content: {response.Content}");
        }
    }
}
