namespace Doubloon
{
    using System;

    /// <summary>
    /// Step attribute for a workflow.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
#if BUILDING_SOURCE_GENERATOR
    internal
#else
    public
#endif
    class RegisteredCurrencyAttribute : Attribute
    {
      public RegisteredCurrencyAttribute()
      {
      }
    }
}