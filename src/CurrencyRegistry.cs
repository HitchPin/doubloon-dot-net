namespace Doubloon;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Currencies;

public static class CurrencyRegistry
{
    private static readonly Dictionary<string, Type> types = new Dictionary<string, Type>();
    public static T RegisterOrGetRegistrant<T>() where T : ICurrency, new ()
    {
        return Instance<T>.Value;
    }

    public static Type ByName(string name) => types[name];
    private static class Instance<T> where T :  ICurrency, new()
    {
        static Instance()
        {
            Value = new T();
            types.Add(Value.Name, typeof(T));
            
        }

        public static readonly T Value;
    }

    [ModuleInitializer]
    public static void InitializeKnownCurrencies()
    {
        RegisterOrGetRegistrant<USD>();
        RegisterOrGetRegistrant<CAD>();
        RegisterOrGetRegistrant<CVE>();
        RegisterOrGetRegistrant<EUR>();
    }
}