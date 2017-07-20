using System;

namespace ConsoleVersion
{
    public class Program
    {
        private static void Main(string[] args)
        {
            //Calls Luis and create Luis Object
            LuisJsonModel luisJson = Utitlities.CallLuis();

            //Returns SPARQL Query using the Luis object
            string sparqlQuery = Utitlities.ExtractLuisData(luisJson);
            Console.WriteLine(sparqlQuery);

            Console.ReadLine();
        }
    }
}