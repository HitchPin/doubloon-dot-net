namespace Doubloon.Currencies;

public class CAD : QuantizedCurrency
{
    public CAD() : base("CAD", 2)
    {
    }

    public override string ToDisplayFormat(decimal d)
    {
        return "$" + d.ToString("#,##0.00");
    }
}