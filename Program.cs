using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
           WebHost.CreateDefaultBuilder(args)
               .ConfigureAppConfiguration((context, config) =>
               {
                   var builtConfig = config.Build();
                   var userAssignedId = builtConfig["UserAssignedId"];

                   AzureServiceTokenProvider azureServiceTokenProvider;

                   if (string.IsNullOrEmpty(userAssignedId))
                   {
                       azureServiceTokenProvider = new AzureServiceTokenProvider();
                   }
                   else
                   {
                       azureServiceTokenProvider = new AzureServiceTokenProvider($"RunAs=App;AppId={userAssignedId}");
                   }

                   var keyVaultClient = new KeyVaultClient(
                       new KeyVaultClient.AuthenticationCallback(
                           azureServiceTokenProvider.KeyVaultTokenCallback));

                   config.AddAzureKeyVault(
                       $"https://{builtConfig["KeyVaultName"]}.vault.azure.net/", 
                       keyVaultClient,
                       new DefaultKeyVaultSecretManager());
               })
               .UseStartup<Startup>();
    }
}
