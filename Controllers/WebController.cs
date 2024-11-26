using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Fibratek.Controllers
{
    [ApiController]
    [Route("/api")]
    public class RestController
    {
        public static HttpClient client = new HttpClient();

        public static void Start()
        {
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddRazorPages();

            var app = builder.Build();
            app.UseStaticFiles();
            app.MapRazorPages();
            app.MapControllers();
            app.Urls.Add("http://0.0.0.0:99");
            app.RunAsync();
        }


        [HttpGet]
        [Route("/api/SomeQuery")]
        public string GetLastIncident(string SomeParam)
        {
            return "Ok";
        }


        public static bool Send(string SomeParam)
        { 
            var response = client.GetAsync($"http://localhost:80/api/SomeQuery?SomeParam={SomeParam}");
            response.Wait();
            return response.Result.IsSuccessStatusCode;
        }
    }
}
//20-35