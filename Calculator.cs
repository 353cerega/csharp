Console.Write("Calculator turned on\n");

while (true) {
    Console.Write("Input ariphmetic expression with two variables (for example, 1 + 2), or $ for terminating\n");
    string input = Console.ReadLine();
    if (input == "$") break;
    string[] words = input.Split(' ');
    if (words.Length != 3) {
        Console.Write("Incorrect expression\n");
        continue;
    }
    string op = words[1];
    if (!double.TryParse(words[0], out double x)) {
        Console.Write("Incorrect expression\n");
        continue;
    }
    if (!double.TryParse(words[2], out double y)) {
        Console.Write("Incorrect expression\n");
        continue;
    }
    switch (op) {
        case "+": Console.Write($"{x + y}\n"); break;
        case "-": Console.Write($"{x - y}\n"); break;
        case "*": Console.Write($"{x * y}\n"); break;
        case "/":
            if (y == 0) {
                Console.Write("Division by zero\n");
                break;
            } else {
                Console.Write($"{x / y}\n");
                break;   
            }
        default: Console.Write("Incorrect expression\n"); break;
    }
}

Console.WriteLine("Calculator terminated.\n");
