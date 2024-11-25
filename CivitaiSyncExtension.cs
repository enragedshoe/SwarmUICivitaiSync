
using System.Net.Http;
using Newtonsoft.Json;

namespace CivitaiSyncExtension
{
    public class CivitaiSyncExtension : SwarmUI.Extension
    {
        private static readonly HttpClient client = new HttpClient();

        public override void Initialize()
        {
            // Register the extension
            SwarmUI.Logger.Info("Initializing CivitaiSyncExtension...");
            SwarmUI.Events.OnModelLoad += OnModelLoad;
        }

        private async void OnModelLoad(Model model)
        {
            if (string.IsNullOrEmpty(model.Description))
            {
                // Get hash from model
                string hash = model.Hash;
                if (!string.IsNullOrEmpty(hash))
                {
                    // Fetch data from Civitai API
                    string url = $"https://civitai.com/api/v1/model-versions/by-hash/{hash}";
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        dynamic data = JsonConvert.DeserializeObject(json);

                        // Update model data
                        model.Description = data.description;
                        model.Author = data.model.creator.username;
                        model.Tags = string.Join(", ", data.model.tags);
                        
                        SwarmUI.Logger.Info($"Model {model.Name} updated with data from Civitai.");
                    }
                }
            }
        }
    }
}
