using System.Globalization;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Practical Assignment: Exceptions and Classes");
        Console.WriteLine("1 - Convert a digit string to an int");
        Console.WriteLine("2 - Calculate a multiplication expression");
        Console.WriteLine("3 - Create a credit card");
        Console.Write("Choose a task: ");

        string? choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                Task1.Run();
                break;
            case "2":
                Task2.Run();
                break;
            case "3":
                Task3.Run();
                break;
            default:
                Console.WriteLine("Error: enter 1, 2, or 3");
                break;
        }
    }
}

internal static class Task1
{
    public static void Run()
    {
        Console.Write("Enter a string of digits from 0 to 9: ");
        string? input = Console.ReadLine();

        try
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new FormatException("The input cannot be empty");
            }

            foreach (char symbol in input)
            {
                if (symbol < '0' || symbol > '9')
                {
                    throw new FormatException("Only digits from 0 to 9 are allowed");
                }
            }

            int number = int.Parse(input, CultureInfo.InvariantCulture);
            Console.WriteLine($"Converted number: {number}");
        }
        catch (FormatException exception)
        {
            Console.WriteLine($"Input format error: {exception.Message}");
        }
        catch (OverflowException)
        {
            Console.WriteLine("Error: the number is outside the range of the int type");
        }
    }
}

internal static class Task2
{
    public static void Run()
    {
        Console.Write("Enter an expression: ");
        string? expression = Console.ReadLine();

        try
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new FormatException("The expression cannot be empty");
            }

            string[] parts = expression.Split('*');
            long result = 1;

            foreach (string part in parts)
            { 
                long number = long.Parse(part.Trim(), CultureInfo.InvariantCulture);
                result = checked(result * number);
            }

            Console.WriteLine($"Result: {result}");
        }
        catch (FormatException)
        {
            Console.WriteLine("Error: enter only integers separated by the * operator.");
        }
        catch (OverflowException)
        {
            Console.WriteLine("Error: the number or result is too large for the long type.");
        }
    }
}

internal static class Task3
{
    public static void Run()
    {
        try
        {
            Console.Write("Enter the card number (13-19 digits): ");
            string number = Console.ReadLine() ?? "";

            Console.Write("Enter the cardholder's full name: ");
            string ownerName = Console.ReadLine() ?? "";

            Console.Write("Enter the CVC (3 or 4 digits): ");
            string cvc = Console.ReadLine() ?? "";

            Console.Write("Enter the expiration month (1-12): ");
            int month = int.Parse(Console.ReadLine() ?? "", CultureInfo.InvariantCulture);

            Console.Write("Enter the expiration year: ");
            int year = int.Parse(Console.ReadLine() ?? "", CultureInfo.InvariantCulture);

            CreditCard card = new CreditCard(number, ownerName, cvc, month, year);

            Console.WriteLine("The credit card was created successfully.");
            Console.WriteLine($"Cardholder: {card.OwnerName}");
            Console.WriteLine($"Card number: {card.MaskedNumber}");
            Console.WriteLine($"Expires: {card.ExpirationMonth:D2}/{card.ExpirationYear}");
        }
        catch (FormatException)
        {
            Console.WriteLine("Error: enter a number in the correct format");
        }
        catch (ArgumentException exception)
        {
            Console.WriteLine($"Invalid card data: {exception.Message}");
        }
        catch (OverflowException)
        {
            Console.WriteLine("Error: the entered number is too large");
        }
    }
}

internal sealed class CreditCard
{
    public string Number { get; }
    public string OwnerName { get; }
    public string Cvc { get; }
    public int ExpirationMonth { get; }
    public int ExpirationYear { get; }

    public string MaskedNumber => new string('*', Number.Length - 4) + Number[^4..];

    public CreditCard(string number, string ownerName, string cvc,
                      int expirationMonth, int expirationYear)
    {
        if (number.Length < 13 || number.Length > 19 || !ContainsOnlyDigits(number))
        {
            throw new ArgumentException("The card number must contain 13 to 19 digits");
        }

        if (string.IsNullOrWhiteSpace(ownerName))
        {
            throw new ArgumentException("The cardholder's full name cannot be empty");
        }

        if ((cvc.Length != 3 && cvc.Length != 4) || !ContainsOnlyDigits(cvc))
        {
            throw new ArgumentException("The CVC must contain 3 or 4 digits");
        }

        if (expirationMonth < 1 || expirationMonth > 12)
        {
            throw new ArgumentException("The expiration month must be from 1 to 12");
        }

        DateTime lastDayOfExpirationMonth;
        try
        {
            int daysInMonth = DateTime.DaysInMonth(expirationYear, expirationMonth);
            lastDayOfExpirationMonth = new DateTime(expirationYear, expirationMonth, daysInMonth);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new ArgumentException("The expiration year is invalid");
        }

        if (lastDayOfExpirationMonth.Date < DateTime.Today)
        {
            throw new ArgumentException("The card has expired");
        }

        Number = number;
        OwnerName = ownerName.Trim();
        Cvc = cvc;
        ExpirationMonth = expirationMonth;
        ExpirationYear = expirationYear;
    }

    private static bool ContainsOnlyDigits(string value)
    {
        if (value.Length == 0)
        {
            return false;
        }

        foreach (char symbol in value)
        {
            if (symbol < '0' || symbol > '9')
            {
                return false;
            }
        }

        return true;
    }
}
