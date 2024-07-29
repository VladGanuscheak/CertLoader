using System.Security.Cryptography.X509Certificates;

namespace CertLoader.Options;

public class StoreOptions
{
    public StoreName StoreName { get; set; }
    public StoreLocation StoreLocation { get; set; }
    public OpenFlags[]? OpenFlags { get; set; }
}
