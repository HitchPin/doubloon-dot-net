namespace Doubloon
{
    using System;

    public class ImplicitRoundingForbiddenException : Exception
    {
        private const string DefaultMessage =
            "The decimal provided had too many decimal places. This library disallows implict behavior " +
            "such as rounding or truncating. Please handle the extra digits appropriately before constructing " +
            "a doubloon.";
        public ImplicitRoundingForbiddenException() : base(DefaultMessage)
        {
        }
    }
}