using System;

namespace PrimeSearchToDatabase
{
  internal class Program
  {
    static void Main()
    {
      Action<string> Display = Console.WriteLine;
      Display("Recherche  des nombres premiers et enregistrement dans une base de données");


      Display("Press any key to exit:");
      Console.ReadKey();
    }
  }
}
