using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Numerics;

namespace PrimeSearchToDatabase
{
  internal class Program
  {
    static void Main()
    {
      Action<string> Display = Console.WriteLine;
      Display("Recherche  des nombres premiers et enregistrement dans une base de données");
      var today = DateTime.Now;
      string todayFormatted = today.ToString().Replace('/', '-').Replace(' ', '_').Replace(':', '-');
      Display($"Search started on {todayFormatted}");
      // Création d'un format personnalisé
      NumberFormatInfo formatInfo = new NumberFormatInfo
      {
        NumberGroupSeparator = " ", // Séparateur de milliers : espace
        NumberGroupSizes = new[] { 3 } // Groupes de 3 chiffres
      };

      // continuing search from the last prime number found
      var connectionString = GetConnectionString("PrimeNumbers");
      var lastMaxPrimeSqlRequest = GetMaxNumberSqlRequest();
      string lastNumberComputed = "11657"; // last known
      lastNumberComputed = ExecuteSqlQuery(connectionString, lastMaxPrimeSqlRequest);
      if (lastNumberComputed.StartsWith("ko|"))
      {
        Console.WriteLine($"erreur : {lastNumberComputed}");
        return;
      }

      Display($"Le dernier nombre premier calculé est : {lastNumberComputed}");
      //string lastNumberComputed = ReadFile("lastNumber.txt");
      var source = BigInteger.Parse(lastNumberComputed);
      if (source % 2 == 0)
      {
        source++;
      }

      var startNumber = source;
      Display(string.Empty);
      Display($"Starting searching from: {startNumber.ToString("N0", formatInfo)}");
      Display(string.Empty);
      var counter = 0;
      int increment = 5000; // Settings.Default.IncrementNumber;
      Display($"Searching for {increment} numbers after {startNumber.ToString("N0", formatInfo)}");
      //Display($"Processing numbers until{FunctionsString.Tabulation(increment.ToString().Length)}{(startNumber + increment).ToString("N0", formatInfo)}");
      Display(string.Empty);
      var endNumber = startNumber + increment;
      var primes = new List<BigInteger>();
      var chrono = new Stopwatch();
      chrono.Start();
      for (int i = 0; i < increment; i += 2)
      {
        var currentNumber = startNumber + i;
        if (IsPrime(currentNumber))
        {
          Display($"{currentNumber.ToString("N0", formatInfo)} is prime");
          counter++;
          primes.Add(currentNumber);
        }
      }

      chrono.Stop();
      string numberOfPrimefound;
      if (counter == 0)
      {
        numberOfPrimefound = "No prime found between: " + $"{startNumber.ToString("N0", formatInfo)} and " + $"{endNumber.ToString("N0", formatInfo)}";
        Display(numberOfPrimefound);
      }
      else
      {
        numberOfPrimefound = $"{counter} prime{Plural(counter)} found between: " + $"{startNumber.ToString("N0", formatInfo)} and " + $"{endNumber.ToString("N0", formatInfo)}";
        Display(numberOfPrimefound);
      }

      Display(string.Empty);
      WriteToFile("PrimeCounter.txt", numberOfPrimefound, true);
      Display($"To search for prime numbers within {increment} numbers, it took : {FormatElapseTime(chrono.Elapsed)}");
      //today = DateTime.Now;
      //todayFormatted = today.ToString().Replace('/', '-').Replace(' ', '_').Replace(':', '-');
      var filename = $"TimeTaken.txt";
      WriteToFile(filename, $"To search for prime numbers within {increment} numbers, it took : {FormatElapseTime(chrono.Elapsed)} starting at {startNumber} and ending at {endNumber}", true);
      filename = $"BigIntegerPrimes";
      WriteToFile(AddTimetoFilename(filename), primes);
      WriteToFile("lastNumber.txt", endNumber.ToString());
      Display("The result were written to a file on a disk: BigIntegerPrimes.txt");
      Display(string.Empty);
      Display($"End of processing on {DateTime.Now}");
      Display("Press any key to exit:");
      Console.ReadKey();
    }

    private static string GetConnectionString(string database)
    {
      return $"Server=localhost;Database={database};Trusted_Connection=True;";
    }

    private static string GetMaxNumberSqlRequest()
    {
      return "SELECT MAX(CAST(PrimeNumber AS BIGINT)) AS MaxValeur FROM Primes WHERE ISNUMERIC(PrimeNumber) = 1;";
    }

    private static string ExecuteSqlQuery(string connectionString, string sqlRequest)
    {
      var result = string.Empty;
      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        try
        {
          connection.Open();
          using (SqlCommand command = new SqlCommand(sqlRequest, connection))
          {
            using (SqlDataReader reader = command.ExecuteReader())
            {
              while (reader.Read())
              {
                var value = reader.GetValue(0); 
                if (value != DBNull.Value)
                {
                  result = value.ToString(); 
                }
              }
            }
          }
        }
        catch (Exception exception)
        {
          return $"ko|{exception.Message}";
        }
      }

      return result;
    }

    /// <summary>Calculate if a big Integer number is prime.</summary>
    /// <param name="number">The number to calculate its primality.</param>
    /// <returns>Returns True if the number is a prime, False otherwise.</returns>
    public static bool IsPrime(BigInteger number)
    {
      if (number.IsEven)
      {
        return false;
      }

      if (number.Sign == 0 || number.Sign == -1)
      {
        return false; // calculate only positive numbers
      }

      if (number == 2 || number == 3 || number == 5)
      {
        return true;
      }

      if (number % 2 == 0 || number % 3 == 0 || number % 5 == 0)
      {
        return false;
      }

      BigInteger squareRoot = (BigInteger)Math.Pow(Math.E, BigInteger.Log(number) / 2);
      for (BigInteger divisor = 7; divisor < squareRoot; divisor += 2)
      {
        if (number % divisor == 0)
        {
          return false;
        }
      }

      return true;
    }

    private static string ReadFile(string filename)
    {
      string result = "2";
      try
      {
        using (StreamReader sr = new StreamReader(filename))
        {
          result = sr.ReadLine();
        }
      }
      catch (Exception)
      {
      }

      return result;
    }

    private static string AddTimetoFilename(string filename)
    {
      var today = DateTime.Now;
      string todayFormatted = today.ToString().Replace('/', '-').Replace(' ', '_').Replace(':', '-');
      return $"{filename}-{todayFormatted}.txt";
    }
    private static void WriteToFile(string filename, string message, bool append = false)
    {
      try
      {
        using (StreamWriter sw = new StreamWriter(filename, append))
        {
          sw.WriteLine(message);
        }
      }
      catch (Exception)
      {
        throw;
      }
    }

    private static string FormatElapseTime(TimeSpan timeSpan)
    {
      var result = string.Empty;
      if (timeSpan.Hours > 0)
      {
        result += $"{timeSpan.Hours} heure{Plural(timeSpan.Hours)} ";
      }

      if (timeSpan.Minutes > 0)
      {
        result += $"{timeSpan.Minutes} minute{Plural(timeSpan.Minutes)} ";
      }

      result += $"{timeSpan.Seconds} seconde{Plural(timeSpan.Seconds)}";
      return result;
    }

    private static void WriteToFile(string filename, List<BigInteger> primes)
    {
      if (primes.Count == 0)
      {
        return;
      }

      try
      {
        using (StreamWriter sw = new StreamWriter(filename))
        {
          sw.WriteLine($"Prime numbers between {primes[0]} and {primes[primes.Count - 1]}");
          foreach (var number in primes)
          {
            sw.WriteLine(number);
          }
        }
      }
      catch (Exception)
      {
        throw;
      }
    }

    private static string Plural(int counter)
    {
      return counter > 1 ? "s" : string.Empty;
    }
  }
}
