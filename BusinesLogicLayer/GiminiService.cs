namespace SolarVolt.BusinesLogicLayer
{
    using System.Text;
    using System.Text.Json;

    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Gemini:ApiKey"] ?? throw new ArgumentException("Gemini ApiKey is missing");
        }

        public async Task<string> ExtractDeviceInfoFromImageAsync(byte[] imageBytes)
        {
            var base64Image = Convert.ToBase64String(imageBytes);

            // Prompt يجبر النموذج على إرجاع JSON مطابق لطلب إضافة الـ Item بالـ Session
            string prompt = @"
قم باستخراج بيانات الجهاز الكهربائي من الصورة وأرجع النتيجة حصراً بصيغة JSON بدون أي نصوص أو توضيحات إضافية:
{
  ""applianceID"": 0,
  ""watt"": 0,
  ""quantity"": 1,
  ""opeatingTime"": ""8h""
}

قواعد مهمة:
1. إذا كانت الصورة تحتوي على ملصق مواصفات (Sticker/Label)، استخرج الاستهلاك (watt) منه بدقة.
2. إذا كانت الصورة للجهاز نفسه فقط ولا توجد كتابة أو ملصق، تعرف على نوع الجهاز وافترض قيمة الاستهلاك التقريبية (watt) بناءً على المعدل الشائع لهذا الجهاز.
3. صيغة opeatingTime يجب أن تنتهي بحرف h أو m (مثال: 8h أو 30m). إذا لم تتضح الساعات ضع الافتراضي ""8h"".
";

            var requestBody = new
            {
                contents = new[]
                {
                new {
                    parts = new object[]
                    {
                        new { text = prompt },
                        new { inline_data = new { mime_type = "image/jpeg", data = base64Image } }
                    }
                }
            }
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"v1beta/models/gemini-3.6-flash:generateContent?key={_apiKey}", content);


            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(responseContent);

                var extractedText = document.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text").GetString();

                return extractedText ?? "{}";
            }

            throw new Exception($"Gemini Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
        }
    }
}
