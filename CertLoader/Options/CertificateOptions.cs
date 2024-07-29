using System.Security.Cryptography.X509Certificates;

namespace CertLoader.Options;

public class CertificateOptions
{
    private X509FindType _x509FindType;
    private object _findValue;

    public X509FindType X509FindType
    {
        get => _x509FindType;
        set
        {
            _x509FindType = value;
            if (_findValue != null)
            {
                FindValue = _findValue;
            }
        }
    }

    public object FindValue
    {
        get => _findValue;
        set
        {
            if (X509FindType == X509FindType.FindByTimeExpired || X509FindType == X509FindType.FindByTimeNotYetValid || X509FindType == X509FindType.FindByTimeValid)
            {
                if (value is string stringValue && DateTime.TryParse(stringValue, out DateTime parsedDate))
                {
                    _findValue = parsedDate;
                }
                else
                {
                    throw new ArgumentException($"FindValue must be a valid DateTime string when X509FindType is {X509FindType}.");
                }
            }
            else
            {
                _findValue = value as string ?? throw new ArgumentException("FindValue must be a string.");
            }
        }
    }

    public bool ValidOnly { get; set; }
}
