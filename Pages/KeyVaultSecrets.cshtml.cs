using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace demo.Pages
{
    public class KeyVaultSecretsModel : PageModel
    {
        private readonly ILogger<KeyVaultSecretsModel> _logger;

        public KeyVaultSecretsModel(ILogger<KeyVaultSecretsModel> logger)
        {
            _logger = logger;
        }

        public string SecretValue { get; set; }
        public async Task OnGetAsync()
        {
            SecretValue = "Secret value is currently empty";

            try
            {
                AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
                KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                var secret = await keyVaultClient.GetSecretAsync("https://cm-identity-kv.vault.azure.net/secrets/KVSercret")
                        .ConfigureAwait(false);
                SecretValue = secret.Value;
            }

            catch (KeyVaultErrorException keyVaultException)
            {
                SecretValue = keyVaultException.Message;
            }
        }
    }
}
