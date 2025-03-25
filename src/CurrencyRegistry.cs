namespace Doubloon;

using System;
using System.Collections.Generic;

public static class CurrencyRegistry
{
    private static readonly Dictionary<string, Type> types = new Dictionary<string, Type>();
    public static T RegisterOrGetRegistrant<T>() where T : ICurrency, new ()
    {
        return Instance<T>.Value;
    }
    
    private static class Instance<T> where T :  ICurrency, new()
    {
        static Instance()
        {
            Value = new T();
            types.Add(Value.Name, typeof(T));
            
        }

        public static readonly T Value;
    }
}