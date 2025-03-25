namespace Doubloon.Currencies
{
    public class USD : QuantizedCurrency
    {
        public USD() : base("USD", 2)
        {
        }

        public override string ToDisplayFormat(decimal d)
        {
            return "$" + d.ToString("#,##0.00");
        }
    }
}