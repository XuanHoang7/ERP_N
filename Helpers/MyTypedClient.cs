using System.Net.Http;
using System;
using System.Threading.Tasks;

namespace ERP.Helpers
{
    public class MyTypedClient
    {
        public HttpClient Client { get; set; }
        public MyTypedClient(HttpClient client)
        {
            string apiKey = "THACO2017";
            client.BaseAddress = new Uri("https://portalgroupapi.thacochulai.vn");
            client.DefaultRequestHeaders.Add("Authorization", "APIKEY " + apiKey);
            client.DefaultRequestHeaders.Add("APIKEY", apiKey);
            this.Client = client;
        }
        public async Task<string> AnhNhanVien(string MaNhanVien)
        {
            try
            {
                var response = await this.Client.GetAsync($"/api/KeySecure/AnhNhanVien?MaNhanVien={Uri.EscapeDataString(MaNhanVien)}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch 
            {
                return null;
            }
            return null;
        }
    }
}
