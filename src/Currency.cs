namespace Doubloon
{
    public interface ICurrency
    {
        /// <summary>
        /// ISO 4217 Currency Code
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Number of decimal places. Usually two, for cents.
        /// </summary>
        int NumberOfDecimalPlaces { get; }

        /// <summary>
        /// Rounds the decimal using banker's rounding for final retrieval out of a doubloon.
        /// Rounding is not done after every operation, only when transiting out of the doubloon
        /// sphere of protection.
        /// </summary>
        /// <param name="d">Decimal to quantize.</param>
        /// <returns>The quantized decimal.</returns>
        decimal Quantize(decimal d);

        /// <summary>
        /// Validates that an incoming decimal has the correct precision.
        /// Incoming decimals must not have any more precision than the
        /// currency allows (2 decimal places for USD). We won't round it
        /// for you. NO IMPLICIT BEHAVIOR.
        /// </summary>
        /// <param name="d">Decimal to inspect.</param>
        /// <returns>In this overload, the same value, unless it throws.</returns>
        /// <exception cref="ImplicitRoundingForbiddenException">If the incoming decimal has too many decimal places.</exception>
        decimal AsSafeDecimal(decimal d);

        /// <summary>
        /// Attempts to parse the string into a decimal. The same rules as the decimal overload apply,
        /// but additional errors could occur if the string is not a valid decimal at all.
        /// </summary>
        /// <param name="s">The wannabe decimal</param>
        /// <returns>The decimal value.</returns>
        /// <exception cref="ImplicitRoundingForbiddenException">If the incoming decimal has too many decimal places.</exception>
        /// <exception cref="System.FormatException">If the string is not a valid decimal.</exception>
        decimal AsSafeDecimal(string s);
        
        /// <summary>
        /// Format the currency with its symbol for display purposes.
        /// </summary>
        /// <param name="d">The safe decimal to format.</param>
        /// <returns>A string representation of this currency suitable for display to the user.</returns>
        string ToDisplayFormat(decimal d);
    }
}