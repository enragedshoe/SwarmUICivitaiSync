using System.Net.Http;
using Newtonsoft.Json;

namespace CivitaiSyncExtension
{
    public class CivitaiSyncExtension : SwarmUI.Extension
    {
        private static readonly HttpClient client = new HttpClient();

        public override void Initialize()
        {
            base.Initialize();

            // Register the extension
            SwarmUI.Logger.Info("Initializing CivitaiSyncExtension...");

            // Register JavaScript and other assets
            ScriptFiles.Add("Assets/civitaiSync.js");
            StyleSheetFiles.Add("Assets/civitaiSync.css"); // Optional
            OtherAssets.Add("Images/example_icon.png"); // Optional

            // Populate metadata
            ExtensionName = "CivitaiSyncExtension";
            Description = "Automatically syncs model descriptions from Civitai.";
            ExtensionAuthor = "Your Name";
            Version = "1.0.0";
            License = "MIT";

            // Hook into the model load event
            SwarmUI.Events.OnModelLoad += OnModelLoad;
        }

        private async void OnModelLoad(Model model)
        {
            if (string.IsNullOrEmpty(model.Description))
            {
                string hash = model.Hash;
                if (!string.IsNullOrEmpty(hash))
                {
                    string url = $"https://civitai.com/api/v1/model-versions/by-hash/{hash}";
                    try
                    {
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
                        else
                        {
                            SwarmUI.Logger.Warning($"Failed to fetch data for model {model.Name}: {response.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        SwarmUI.Logger.Error($"Error fetching data for model {model.Name}: {ex.Message}");
                    }
                }
            }
        }
    }
}
