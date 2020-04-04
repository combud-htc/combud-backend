using System;
using System.Diagnostics;

namespace HashCostTesting
{
	class Program
	{
		static void Main(string[] args)
		{
            Console.WriteLine(BCrypt.Net.BCrypt.Verify("password", "$2a$12$EY7W3.Ma0jv7xWnGnhVgn.IUM9vPO9fDHtHGTtXK4AMebPBRuHJAW", true, BCrypt.Net.HashType.SHA512));
            Console.ReadLine();

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
