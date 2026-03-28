using System;

namespace CarDescriptionApp
{
    public interface ICar
    {
        string GetDescription();
    }

    public interface IElectric { }
    public interface IGas { }

    public interface IAutomatical { }
    public interface IMechanical { }

    public abstract class ACar : ICar
    {
        public string Brand { get; protected set; }
        public int Seats { get; protected set; }
        public string Multimedia { get; protected set; }

        protected ACar(string brand, int seats, string multimedia)
        {
            Brand = brand;
            Seats = seats;
            Multimedia = multimedia;
        }

        
        public virtual string GetDescription()
        {
            string carType = this is IElectric ? "электрический" :
                             this is IGas ? "бензиновый" :
                             "неизвестного типа";

            string transmission = this is IAutomatical ? "автоматической" :
                                  this is IMechanical ? "механической" :
                                  "неизвестной";

            return $"{Brand}: {carType} автомобиль с {transmission} коробкой передач, {Seats} местами, {Multimedia} на борту";
        }
    }

    
    public class Tesla : ACar, IElectric, IAutomatical
    {
        public Tesla() : base("Tesla", 5, "Android") { }
    }

    public class BMW : ACar, IGas, IAutomatical
    {
        public BMW() : base("BMW", 5, "iDrive") { }
    }

    public class Toyota : ACar, IGas, IMechanical
    {
        public Toyota() : base("Toyota", 5, "Toyota Touch") { }
    }

    public class Volkswagen : ACar, IGas, IAutomatical
    {
        public Volkswagen() : base("Volkswagen", 5, "MIB 3") { }
    }

    public class Mercedes : ACar, IGas, IAutomatical
    {
        public Mercedes() : base("Mercedes-Benz", 5, "MBUX") { }
    }

    public class Volvo : ACar, IGas, IAutomatical
    {
        public Volvo() : base("Volvo", 5, "Sensus") { }
    }

    public class Peugeot : ACar, IGas, IMechanical
    {
        public Peugeot() : base("Peugeot", 5, "i-Cockpit") { }
    }
    

    public enum CarType
    {
        Tesla,
        BMW,
        Toyota,
        Volkswagen,
        Mercedes,
        Volvo,
        Peugeot
    }

    public static class CarFactory
    {
        public static ICar CreateCar(CarType type)
        {
            return type switch
            {
                CarType.Tesla => new Tesla(),
                CarType.BMW => new BMW(),
                CarType.Toyota => new Toyota(),
                CarType.Volkswagen => new Volkswagen(),
                CarType.Mercedes => new Mercedes(),
                CarType.Volvo => new Volvo(),
                CarType.Peugeot => new Peugeot(),
                _ => throw new ArgumentException("Неизвестная марка автомобиля")
            };
        }
    }

    class Program
    {
        static void Main(string[] args)
        {

            while (true)
            {
                Console.WriteLine("Введите марку автомобиля или 'done' для остановки ввода.");
                string input = Console.ReadLine()?.Trim();

                if (input == null || input == "done")
                    break;
                
                if (Enum.TryParse<CarType>(input, true, out CarType carType))
                {
                    try
                    {
                        ICar car = CarFactory.CreateCar(carType);
                        Console.WriteLine($"<{car.GetDescription()}>");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Неизвестная марка. Попробуйте снова.");
                }

                Console.WriteLine();
            }
        }
    }
}
