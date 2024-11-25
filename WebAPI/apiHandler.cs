
using System.Net.Http;

namespace CivitaiSyncExtension
{
    public static class APIHandler
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<string> FetchModelDataByHash(string hash)
        {
            string url = $"https://civitai.com/api/v1/model-versions/by-hash/{hash}";
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return null;
        }
    }
}
