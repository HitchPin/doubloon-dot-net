namespace Doubloon;

public static class TypeCache
{
    public static T GetInstance<T>() where T : new()
    {
        return Instance<T>.Value;
    }
    private static class Instance<T> where T : new()
    {
        public static readonly T Value = new T();
    }
}