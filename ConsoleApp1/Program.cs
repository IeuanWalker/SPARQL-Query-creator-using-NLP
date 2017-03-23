using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("START!");

            //Calls Luis and create Luis Object
            LuisJSONModel LuisJson = utitlities.CallLuis();

            //Returns SPARQL Query using the Luis object
            string sparqlQuery = utitlities.ExtractLuisData(LuisJson);
            Console.WriteLine(sparqlQuery);

            //Send SPARQL Query to DBpeadia 

            Console.ReadLine();
        }
    }
}