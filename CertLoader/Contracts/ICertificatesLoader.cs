using System.Security.Cryptography.X509Certificates;

namespace CertLoader.Contracts;

public interface ICertificatesLoader
{
    X509Certificate2Collection FindCertificates(X509Certificate2Collection collection);
}
