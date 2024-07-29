using System.Security.Cryptography.X509Certificates;

namespace CertLoader.Contracts;

public interface IX509StoreLoader
{
    X509Store LoadStore();
}
