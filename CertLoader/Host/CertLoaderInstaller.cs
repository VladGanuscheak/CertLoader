using CertLoader.Contracts;
using CertLoader.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using NJsonSchema;
using CertLoader.Options;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace CertLoader.Host;

public static class CertLoaderInstaller
{
    private static readonly string Directory =
            Path.GetDirectoryName(typeof(CertLoaderInstaller).Assembly.Location)!;

    private static readonly string Location = $"{Directory}\\JsonSchema\\";

    public async static Task AddX509StoreServices(this IServiceCollection services, IConfiguration configuration, string section)
    {
        var storeSchemaJson = File.ReadAllText($"{Location}storeSchema.json");

        var storeSchema = await JsonSchema.FromJsonAsync(storeSchemaJson);

        services.AddSingleton(storeSchema);

        var sectionConfig = configuration.GetSection(section);
        var sectionJson = sectionConfig.ToJToken().ToString();
        var validationErrors = storeSchema.Validate(sectionJson);

        if (validationErrors.Count > 0)
        {
            throw new Exception($"Configuration validation failed for section '{section}': {string.Join(", ", validationErrors)}");
        }

        services.Configure<StoreOptions>(sectionConfig!);

        services.AddTransient<IX509StoreLoader, X509StoreLoader>();
        
        services.AddKeyedTransient<IX509StoreLoader, X509StoreLoader>(section, (provider, name) 
            => (X509StoreLoader)provider.GetService<IX509StoreLoader>()!);
    }

    public async static Task AddCertificatesLoaderServices(this IServiceCollection services, IConfiguration configuration, string section)
    {
        var certificateSchemaJson = File.ReadAllText($"{Location}certificateSchema.json");
        var certificateSchema = await JsonSchema.FromJsonAsync(certificateSchemaJson);

        services.AddSingleton(certificateSchema);

        var sectionConfig = configuration.GetSection(section);
        var sectionJson = sectionConfig.ToJToken().ToString();
        var validationErrors = certificateSchema.Validate(sectionJson);

        if (validationErrors.Count > 0)
        {
            throw new Exception($"Configuration validation failed for section '{section}': {string.Join(", ", validationErrors)}");
        }

        services.Configure<CertificateOptions>(sectionConfig!);

        services.AddTransient<ICertificatesLoader, CertificatesLoader>();

        services.AddKeyedTransient<ICertificatesLoader, CertificatesLoader>(section, (provider, name)
           => (CertificatesLoader)provider.GetService<ICertificatesLoader>()!);
    }

    public static JToken ToJToken(this IConfigurationSection section)
    {
        if (section.GetChildren().Any())
        {
            if (section.GetChildren().First().Key.All(char.IsDigit))
            {
                var jArray = new JArray();
                foreach (var child in section.GetChildren())
                {
                    jArray.Add(child.ToJToken());
                }
                return jArray;
            }
            else
            {
                var jObject = new JObject();
                foreach (var child in section.GetChildren())
                {
                    jObject.Add(child.Key, child.ToJToken());
                }
                return jObject;
            }
        }
        else
        {
            return ConvertToJToken(section.Value!);
        }
    }

    private static JToken ConvertToJToken(string value)
    {
        if (bool.TryParse(value, out bool boolValue))
        {
            return new JValue(boolValue);
        }
        if (int.TryParse(value, out int intValue))
        {
            return new JValue(intValue);
        }
        if (double.TryParse(value, out double doubleValue))
        {
            return new JValue(doubleValue);
        }
        if (DateTime.TryParse(value, out DateTime dateTimeValue))
        {
            return new JValue(dateTimeValue);
        }
        return new JValue(value);
    }
}
