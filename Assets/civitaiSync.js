// CivitaiSyncExtension JavaScript for SwarmUI

(async function () {
    // Wait until SwarmUI is ready
    SwarmUI.Events.onReady(() => {
        console.log("CivitaiSyncExtension loaded!");
        syncMissingDescriptions();
    });

    async function syncMissingDescriptions() {
        const models = SwarmUI.Models.getAll(); // Get all models in SwarmUI

        for (const model of models) {
            if (!model.description || model.description.trim() === "") {
                console.log(`Syncing model: ${model.name}`);

                // Fetch model description from the backend API
                const response = await fetch(`/api/civitai-sync/${model.hash}`);
                if (response.ok) {
                    const data = await response.json();

                    // Update the model UI in SwarmUI
                    SwarmUI.Models.updateModelDescription(model.id, data.description);
                    console.log(`Updated description for ${model.name}`);
                } else {
                    console.error(`Failed to sync model: ${model.name}`);
                }
            }
        }
    }
})();
