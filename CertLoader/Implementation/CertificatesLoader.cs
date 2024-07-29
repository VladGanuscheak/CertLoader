using CertLoader.Contracts;
using CertLoader.Options;
using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;

namespace CertLoader.Implementation;

public class CertificatesLoader : ICertificatesLoader
{
    private readonly CertificateOptions _certificateOptions;

    public CertificatesLoader(IOptionsSnapshot<CertificateOptions> certificateOptions)
    {
        _certificateOptions = certificateOptions.Value;
    }

    public X509Certificate2Collection FindCertificates(X509Certificate2Collection collection)
    {
        return collection.Find(
            (X509FindType)Enum.Parse(typeof(X509FindType), 
            _certificateOptions.X509FindType.ToString()), 
            _certificateOptions.FindValue, 
            _certificateOptions.ValidOnly);
    }
}
