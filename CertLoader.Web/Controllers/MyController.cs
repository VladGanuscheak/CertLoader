using CertLoader.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Https;


namespace CertLoader.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MyController : ControllerBase
    {
        private readonly IX509StoreLoader _storeLoader;
        private readonly ICertificatesLoader _certificatesLoader;

        public MyController(IServiceProvider serviceProvider)
        {
            _storeLoader = serviceProvider.GetKeyedService<IX509StoreLoader>("X509Store")!;
            _certificatesLoader = serviceProvider.GetKeyedService<ICertificatesLoader>("X509Certificate")!;
        }

        [HttpGet]
        public IActionResult GetCertificates()
        {
            using var store = _storeLoader.LoadStore();

            var certificates = _certificatesLoader.FindCertificates(store.Certificates);
            return Ok(certificates.Count);
        }
    }
}
