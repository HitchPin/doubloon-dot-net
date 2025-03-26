namespace Doubloon.Currencies;

public class CVE : QuantizedCurrency
{
    public CVE() : base("CVE", 2)
    {
        
    }

    public override string ToDisplayFormat(decimal d)
    {
        var qs = Quantize(d).ToString().Split('.');
        return qs[0] + "$" + qs[1];
    }
}