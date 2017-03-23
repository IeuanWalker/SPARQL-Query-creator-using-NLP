using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //Calls Luis and create Luis Object
            LuisJSONModel LuisJson = utitlities.CallLuis();

            //Returns SPARQL Query using the Luis object
            string sparqlQuery = utitlities.ExtractLuisData(LuisJson);
            Console.WriteLine(sparqlQuery);

            Console.ReadLine();
        }
    }
}