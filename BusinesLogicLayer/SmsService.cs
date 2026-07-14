using Microsoft.Identity.Client;
using System.Net.Http.Json;
namespace SolarVolt.BusinesLogicLayer
{
    public class SmsService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        public SmsService(IConfiguration configuration,HttpClient httpClient)
        {
            _configuration= configuration; ;
            _httpClient = httpClient;   

        }
        public async Task<bool> SendSms(string phoneNumber, string message)
        {
            var smsSetting = _configuration.GetSection("SmsGateway");
            var apiUrl = smsSetting["ApiUrl"];
            var token = smsSetting["Token"];
            var body = new
            {
                to = phoneNumber,
                message = message
            };

            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            Console.WriteLine(apiUrl);
            request.Headers.TryAddWithoutValidation("Authorization",token);
            request.Content = JsonContent.Create(body);
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
