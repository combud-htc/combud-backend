using System;
using System.Diagnostics;

namespace HashCostTesting
{
	class Program
	{
		static void Main(string[] args)
		{
            var cost = 16;
            var timeTarget = 100; // Milliseconds
            long timeTaken;
            do
            {
                var sw = Stopwatch.StartNew();

                BCrypt.Net.BCrypt.HashPassword("RwiKnN>9xg3*C)1AZl.)y8f_:GCz,vt3T]PI", workFactor: cost);

                sw.Stop();
                timeTaken = sw.ElapsedMilliseconds;

                cost -= 1;

            } while ((timeTaken) >= timeTarget);

            Console.WriteLine("Appropriate Cost Found: " + (cost + 1));
            Console.ReadLine();
        }
	}
}
