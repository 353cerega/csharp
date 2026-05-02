using System;
using System.Collections.Generic;

public interface IEntity
{
    int Id { get; }
}

public class Repository<T> where T : IEntity
{
    private readonly Dictionary<int, T> _items = new Dictionary<int, T>();

    public int Count => _items.Count;

    public void Add(T item)
    {
        if (_items.ContainsKey(item.Id))
            throw new InvalidOperationException($"Item with Id {item.Id} already exists.");
        _items.Add(item.Id, item);
    }

    public bool Remove(int id)
    {
        return _items.Remove(id);
    }

    public T? GetById(int id)
    {
        _items.TryGetValue(id, out T? item);
        return item;
    }

    public IReadOnlyList<T> GetAll()
    {
        return new List<T>(_items.Values);
    }

    public IReadOnlyList<T> Find(Predicate<T> predicate)
    {
        var results = new List<T>();
        foreach (var item in _items.Values)
            if (predicate(item))
                results.Add(item);
        return results;
    }
}

public class Product : IEntity
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

public class User : IEntity
{
    public int Id { get; }
    public string Name { get; }
    public string Email { get; }

    public User(int id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
    }

    public override string ToString() => $"[User] Id={Id}, Name={Name}, Email={Email}";
}

public static class RepositoryTests
{
    public static void TestAddAndCount()
    {
        Console.WriteLine("\n--- TestAddAndCount ---");
        var repo = new Repository<Product>();
        repo.Add(new Product(1, "A", 1200.0));
        repo.Add(new Product(2, "B", 25.0));
        bool ok = repo.Count == 2;
        Console.WriteLine($"Count = {repo.Count} -> {(ok ? "OK" : "FAIL")}");
    }

    public static void TestAddDuplicate()
    {
        Console.WriteLine("\n--- TestAddDuplicate ---");
        var repo = new Repository<Product>();
        repo.Add(new Product(1, "A", 353));
        try
        {
            repo.Add(new Product(1, "B", 535));
            Console.WriteLine("FAIL: Exception not thrown");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"OK: {ex.Message}");
        }
    }

    public static void TestGetById()
    {
        Console.WriteLine("\n--- TestGetById ---");
        var repo = new Repository<Product>();
        repo.Add(new Product(998, "A", 244));
        var product = repo.GetById(998);
        bool ok = product != null && product.Id == 998 && product.Name == "A";
        Console.WriteLine($"GetById(10) item:  {product}");
        var missing = repo.GetById(353);
        ok = missing == null;
        Console.WriteLine($"GetById(353) -> {(ok ? "OK" : "FAIL")}");
    }

    public static void TestGetAll()
    {
        Console.WriteLine("\n--- TestGetAll ---");
        var repo = new Repository<Product>();
        repo.Add(new Product(1, "A", 998));
        repo.Add(new Product(2, "B", 244));
        repo.Add(new Product(3, "C", 353));
        
        var all = repo.GetAll();
        Console.WriteLine($"GetAll returned {all.Count} items:");
        foreach (var product in all)
            Console.WriteLine($"  {product}");
        
        bool ok = all.Count == 3;
        Console.WriteLine($"Count check -> {(ok ? "OK" : "FAIL")}");
    }

    public static void TestRemove()
    {
        Console.WriteLine("\n--- TestRemove ---");
        var repo = new Repository<Product>();
        repo.Add(new Product(998, "A", 244));
        bool removed = repo.Remove(998);
        bool ok = removed && repo.Count == 0;
        Console.WriteLine($"Remove existing -> {(ok ? "OK" : "FAIL")}");
        removed = repo.Remove(998);
        ok = !removed;
        Console.WriteLine($"Remove missing -> {(ok ? "OK" : "FAIL")}");
    }

    public static void TestFind()
    {
        Console.WriteLine("\n--- TestFind ---");
        var repo = new Repository<Product>();
        repo.Add(new Product(1, "A", 998));
        repo.Add(new Product(2, "B", 244));
        repo.Add(new Product(3, "C", 353));
        
        var products = repo.Find(p => p.Price < 500);
        Console.WriteLine($"Find price < 500 -> found {products.Count} items:");
        foreach (var p in products) Console.WriteLine($"  {p}");
        bool ok = products.Count == 2;
        Console.WriteLine($"Check -> {(ok ? "OK" : "FAIL")}");
        
        var empty = repo.Find(p => p.Price > 1000);
        ok = empty.Count == 0;
        Console.WriteLine($"Find price > 1000 -> found {empty.Count} -> {(ok ? "OK" : "FAIL")}");
    }

    public static void TestDifferentType()
    {
        Console.WriteLine("\n--- TestDifferentType ---");
        var userRepo = new Repository<User>();
        userRepo.Add(new User(100, "Vasya", "vasya@test.com"));
        userRepo.Add(new User(200, "Petya", "petya@test.com"));
        bool ok = userRepo.Count == 2;
        Console.WriteLine($"User count = {userRepo.Count} -> {(ok ? "OK" : "FAIL")}");
        
        var user = userRepo.GetById(200);
        ok = user != null && user.Name == "Petya";
        Console.WriteLine($"GetById(200) -> {(ok ? "OK" : "FAIL")}");
        
        var filtered = userRepo.Find(u => u.Email.Contains("vasya"));
        Console.WriteLine($"Find users with 'vasya' in email -> found {filtered.Count}:");
        foreach (var u in filtered) Console.WriteLine($"  {u}");
        ok = filtered.Count == 1 && filtered[0].Id == 100;
        Console.WriteLine($"Check -> {(ok ? "OK" : "FAIL")}");
    }

}

class Program
{
    static void Main()
    {
        Console.WriteLine("======== TESTING ========");
        RepositoryTests.TestAddAndCount();
        RepositoryTests.TestAddDuplicate();
        RepositoryTests.TestGetById();
        RepositoryTests.TestGetAll();
        RepositoryTests.TestRemove();
        RepositoryTests.TestFind();
        RepositoryTests.TestDifferentType();
        Console.WriteLine("\n======= FINISHED =======");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
