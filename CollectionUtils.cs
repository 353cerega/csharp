using System;
using System.Collections.Generic;

public class Product
{
    public int Id { get; }
    public string Name { get; }
    public double Price { get; }

    public Product(int id, string name, double price)
    {
        Id = id;
        Name = name;
        Price = price;
    }

    public override string ToString() => $"[Product] Id={Id}, Name={Name}, Price={Price:C}";
}

public static class CollectionUtils
{
    public static List<T> Distinct<T>(List<T> source)
    {   
        var result = new List<T>();
        var seen = new HashSet<T>();
        
        foreach (var item in source)
        {
            if (!seen.Contains(item))
            {
                seen.Add(item);
                result.Add(item);
            }
        }
        return result;
    }

    public static Dictionary<TKey, List<TValue>> GroupBy<TValue, TKey>(
        List<TValue> source,
        Func<TValue, TKey> keySelector) where TKey : notnull
    {      
        var result = new Dictionary<TKey, List<TValue>>();
        
        foreach (var item in source)
        {
            TKey key = keySelector(item);
            if (!result.ContainsKey(key))
                result[key] = new List<TValue>();
            result[key].Add(item);
        }
        return result;
    }

    public static Dictionary<TKey, TValue> Merge<TKey, TValue>(
        Dictionary<TKey, TValue> first,
        Dictionary<TKey, TValue> second,
        Func<TValue, TValue, TValue> conflictResolver) where TKey : notnull
    { 
        var result = new Dictionary<TKey, TValue>(first);

        foreach (var pair in second)
        {
            if (result.TryGetValue(pair.Key, out TValue existingValue))
            {
                result[pair.Key] = conflictResolver(existingValue, pair.Value);
            }
            else
            {
                result[pair.Key] = pair.Value;
            }
        }
        return result;
    }

    public static T MaxBy<T, TKey>(List<T> source, Func<T, TKey> selector)
        where TKey : IComparable<TKey>
    {
        if (source.Count == 0)
            throw new InvalidOperationException("Sequence contains no elements");
        
        T maxElement = source[0];
        TKey maxKey = selector(maxElement);
        
        for (int i = 1; i < source.Count; i++)
        {
            T current = source[i];
            TKey currentKey = selector(current);
            if (currentKey.CompareTo(maxKey) > 0)
            {
                maxElement = current;
                maxKey = currentKey;
            }
        }
        return maxElement;
    }
}

public static class CollectionUtilsTests
{
    public static void TestDistinctInts()
    {
        Console.WriteLine("\n--- TestDistinctInts ---");
        var input = new List<int> { 1, 2, 2, 4, 1, 3, 5, 3 };
        var result = CollectionUtils.Distinct(input);
        Console.WriteLine($"Input: [{string.Join(", ", input)}]");
        Console.WriteLine($"Result: [{string.Join(", ", result)}]");
        bool ok = result.Count == 5 && result[0] == 1 && result[1] == 2 && result[2] == 4 && result[3] == 3 && result[4] == 5;
        Console.WriteLine(ok ? "OK" : "FAIL");
    }

    public static void TestDistinctStrings()
    {
        Console.WriteLine("\n--- TestDistinctStrings ---");
        var input = new List<string> { "apple", "banana", "apple", "orange", "banana", "grape" };
        var result = CollectionUtils.Distinct(input);
        Console.WriteLine($"Input: [{string.Join(", ", input)}]");
        Console.WriteLine($"Result: [{string.Join(", ", result)}]");
        bool ok = result.Count == 4 && result[0] == "apple" && result[1] == "banana" && result[2] == "orange" && result[3] == "grape";
        Console.WriteLine(ok ? "OK" : "FAIL");
    }

    public static void TestGroupBy()
    {
        Console.WriteLine("\n--- TestGroupBy ---");
        var words = new List<string> { "cat", "dog", "elephant", "ant", "bird", "fox", "lion", "tiger" };
        var grouped = CollectionUtils.GroupBy(words, w => w.Length);
        Console.WriteLine("Result:");
        foreach (var group in grouped)
        {
            Console.WriteLine($"  Length {group.Key}: {string.Join(", ", group.Value)}");
        }
        bool ok = grouped.Count == 4 && grouped[3].Count == 4 && grouped[4].Count == 2 && grouped[5].Count == 1 && grouped[8].Count == 1;
        Console.WriteLine(ok ? "OK" : "FAIL");
    }

    public static void TestMerge()
    {
        Console.WriteLine("\n--- TestMerge ---");
        var dict1 = new Dictionary<string, int>
        {
            { "apple", 998 },
            { "banana", 244 },
            { "orange", 353 }
        };
        var dict2 = new Dictionary<string, int>
        {
            { "banana", 1 },
            { "orange", 2 },
            { "grape", 3 }
        };
        var merged = CollectionUtils.Merge(dict1, dict2, (v1, v2) => v1 + v2);
        Console.WriteLine($"First:  {string.Join(", ", dict1)}");
        Console.WriteLine($"Second: {string.Join(", ", dict2)}");
        Console.WriteLine($"Merged: {string.Join(", ", merged)}");
        bool ok = merged.Count == 4 &&
                  merged["apple"] == 998 &&
                  merged["banana"] == 245 &&
                  merged["orange"] == 355 &&
                  merged["grape"] == 3;
        Console.WriteLine(ok ? "OK" : "FAIL");
    }

    public static void TestMaxBy()
    {
        Console.WriteLine("\n--- TestMaxBy ---");
        var products = new List<Product>
        {
            new Product(1, "A", 998),
            new Product(2, "B", 244),
            new Product(3, "C", 353),
            new Product(4, "D", 998244353),
            new Product(5, "E", 100000007)
        };
        var max = CollectionUtils.MaxBy(products, p => p.Price);
        Console.WriteLine("Products:");
        foreach (var p in products) Console.WriteLine($"  {p}");
        Console.WriteLine($"Max price product: {max}");
        bool ok = max.Id == 4;
        Console.WriteLine(ok ? "OK" : "FAIL");
    }

    public static void TestMaxByEmpty()
    {
        Console.WriteLine("\n--- TestMaxByEmpty ---");
        var empty = new List<Product>();
        try
        {
            CollectionUtils.MaxBy(empty, p => p.Price);
            Console.WriteLine("FAIL: Exception not thrown");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Caught exception: {ex.Message}\nOK");
        }
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("===== TESTING =====");
        
        CollectionUtilsTests.TestDistinctInts();
        CollectionUtilsTests.TestDistinctStrings();
        CollectionUtilsTests.TestGroupBy();
        CollectionUtilsTests.TestMerge();
        CollectionUtilsTests.TestMaxBy();
        CollectionUtilsTests.TestMaxByEmpty();

        Console.WriteLine("\n======= FINISHED =======");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
