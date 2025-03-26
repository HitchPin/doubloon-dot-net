namespace Doubloon.Currencies;

public class EUR : QuantizedCurrency
{
    public EUR() : base("EUR", 2)
    {
        
    }

    public override string ToDisplayFormat(decimal d)
    {
        return $"{Quantize(d)}€";
    }
}