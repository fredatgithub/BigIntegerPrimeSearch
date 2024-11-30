using System;

namespace CheckIfPrime
{
  internal class Program
  {
    static void Main()
    {
      long start = 2_000_000_000;
      long number = start + 1;
      //Le premier nombre premier après 2 000 000 000 est  2000000011
      while (true)
      {
        if (IsPrime(number))
        {
          Console.WriteLine($"Le premier nombre premier après {start} est : {number}");
          break;
        }

        number++;
      }
    }

    static bool IsPrime(long num)
    {
      if (num < 2) return false;
      if (num == 2 || num == 3) return true;
      if (num % 2 == 0) return false;

      long limit = (long)Math.Sqrt(num);
      for (long i = 3; i <= limit; i += 2)
      {
        if (num % i == 0)
        {
          return false;
        }
      }

      return true;
    }
  }
}
