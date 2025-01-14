using System;

public static class RandomExtensions
{
    public static int Next(this Random random, int min, int max)
    {
        return random.Next(min, max + 1);
    }
}

public static class RandomHelper
{
    private static readonly Random _random = new Random();

    public static int Range(int min, int max)
    {
        return _random.Next(min, max);
    }

    public static float Range(float min, float max)
    {
        return (float)_random.NextDouble() * (max - min) + min;
    }

    public static bool Bool()
    {
        return _random.NextDouble() < 0.5;
    }
}
