namespace Doubloon.Currencies
{
    using System;

    public abstract class QuantizedCurrency : ICurrency
    {
        public QuantizedCurrency(string name, int decimalPlaces)
        {
            this.Name = name;
            this.NumberOfDecimalPlaces = decimalPlaces;
        }
        
        /// <inheritdoc cref="ICurrency.AsSafeDecimal(string)"/>
        public string Name { get; }
        
        /// <inheritdoc cref="ICurrency.AsSafeDecimal(string)"/>
        public int NumberOfDecimalPlaces { get; }
        
        /// <inheritdoc cref="ICurrency.AsSafeDecimal(string)"/>
        public decimal AsSafeDecimal(decimal d)
        {
            int decimalCount = BitConverter.GetBytes(decimal.GetBits(d)[3])[2];
            if (decimalCount > NumberOfDecimalPlaces)
            {
                throw new ImplicitRoundingForbiddenException();
            }

            return decimalCount;
        }

        /// <inheritdoc cref="ICurrency.AsSafeDecimal(string)"/>
        public decimal AsSafeDecimal(string s)
        {
            decimal d = decimal.Parse(s);
            return AsSafeDecimal(d);
        }

        public decimal Quantize(decimal d) => Math.Round(d, MidpointRounding.ToEven);
        
        public abstract string ToDisplayFormat(decimal d);
    }
}