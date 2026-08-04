const double TaxRate = 0.2;

static double CalculatePay(double hours, double rate)
{
    if (hours < 0 || rate < 0)
    {
        throw new ArgumentException("Hours and rate must be positive.");
    }

    double gross = hours * rate;
    double tax = gross * TaxRate;
    double net = gross - tax;
    return net;
}

Console.Write("Enter employee name: ");
string name = Console.ReadLine();

try
{
    Console.Write("Hours worked: ");
    double hours = double.Parse(Console.ReadLine());

    Console.Write("Hourly rate: ");
    double rate = double.Parse(Console.ReadLine());

    double netPay = CalculatePay(hours, rate);
    Console.WriteLine($"{name} earned ${netPay:F2} after tax.");
}
catch (FormatException)
{
    Console.WriteLine("Invalid input: please enter numeric values for hours and rate.");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("--- Person Demo ---");

Person person = new Person("Casey", "Nguyen", 21);
Console.WriteLine($"Full name: {person.FullName()}");
Console.WriteLine($"Is adult: {person.IsAdult()}");

Person minor = new Person("Alex", "Smith", 15);
Console.WriteLine($"Full name: {minor.FullName()}");
Console.WriteLine($"Is adult: {minor.IsAdult()}");

