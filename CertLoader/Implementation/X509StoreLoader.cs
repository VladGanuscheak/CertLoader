using CertLoader.Contracts;
using CertLoader.Options;
using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;

namespace CertLoader.Implementation;

public class X509StoreLoader : IX509StoreLoader
{
    private readonly StoreOptions _storeOptions;

    public X509StoreLoader(IOptionsSnapshot<StoreOptions> storeOptions)
    {
        _storeOptions = storeOptions.Value;
    }

    public X509Store LoadStore()
    {
        var store = new X509Store(_storeOptions.StoreName, _storeOptions.StoreLocation);
        store.Open(OpenFlagsToFlags(_storeOptions.OpenFlags!));

        return store;
    }

    private OpenFlags OpenFlagsToFlags(OpenFlags[] openFlags)
    {
        OpenFlags flags = 0;
        foreach (var flag in openFlags)
        {
            flags |= flag;
        }
        return flags;
    }
}
