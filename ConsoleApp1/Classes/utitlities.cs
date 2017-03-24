using Newtonsoft.Json;
using System;
using System.IO;

namespace ConsoleVersion
{
    abstract class utitlities
    {
        public static LuisJSONModel CallLuis()
        {
            LuisJSONModel data = new LuisJSONModel();

            var file = Path.Combine(@"C:\Users\Ieuan\OneDrive\Documents\Work\Uni\Year 3\Dissertation\The program\ASP\ConsoleApp1\ConsoleApp1\test2LuisData.json");
            data = JsonConvert.DeserializeObject<LuisJSONModel>(File.ReadAllText(file));

            return data;
        }

        public static string ExtractLuisData(LuisJSONModel luisJson)
        {
            int numberOfItems = 0;
            string genre = "";
            int year = 0;
            string exactDate = "";

            foreach (var i in luisJson.entities)
            {
                switch (i.type)
                {
                    case "builtin.number":
                        if (int.TryParse(i.resolution.value, out int number))
                        {
                            if (number < 1000)
                            {
                                numberOfItems = number;
                            }
                        }
                        break;

                    case "genre":
                        genre = i.entity;
                        break;

                    case "builtin.datetime.date":
                        if (DateTime.TryParse(i.entity, out DateTime exactDateTime))
                        {
                            exactDate = exactDateTime.ToString();
                        }
                        else if (int.TryParse(i.entity, out int yearDateTime) && (i.entity.Length == 4))
                        {
                            year = yearDateTime;
                        }
                        break;
                }
            }
            return CreateSparqlQuery(numberOfItems, genre, year, exactDate);
        }

        static string CreateSparqlQuery(int numberOfItems, string genre, int year, string exactDate)
        {
            Console.WriteLine("Number of items: " + numberOfItems);
            Console.WriteLine("Genre: " + genre);
            Console.WriteLine("Year: " + year);
            Console.WriteLine("Exact data: " + exactDate);
            Console.WriteLine();

            string limit = numberOfItems > 0 ? String.Format("LIMIT({0})", numberOfItems) : "";
            string genreMatch = !String.IsNullOrEmpty(genre.Trim()) ? String.Format("FILTER ( regex (str(?genre), '{0}', 'i'))", genre) : "";
            string dateMatch = "";

            if (exactDate.Equals(DateTime.Now) && year.Equals(0))
            {
                //Means that both haven't been assigned
            }
            else if (!String.IsNullOrEmpty(exactDate.Trim()))
            {
                dateMatch = String.Format("FILTER ( regex (str(?releaseDate), '{0}', 'i'))", exactDate);
            }
            else if (!year.Equals(0))
            {
                dateMatch = String.Format("FILTER ((?releaseDate >= '{0}-01-01'^^xsd:date) && (?releaseDate < '{0}-12-31'^^xsd:date))", year);
            }

            string queryPattern =
                "PREFIX xsd: <http://www.w3.org/2001/XMLSchema#> " +
                "PREFIX db: <http://dbpedia.org/ontology/> " +
                "PREFIX prop: <http://dbpedia.org/property/> " +
                "SELECT ?movieLink ?title ?genreLink ?genre ?releaseDate " +
                "WHERE {{ " +
                    "?movieLink rdf:type db:Film; " +
                               "foaf:name ?title. " +
                    "OPTIONAL {{ ?movieLink prop:genre ?genreLink. " +
                               "?genreLink rdfs:label ?genre. " +
                               "FILTER(lang(?genre) = 'en') }}. " +
                    "OPTIONAL {{ ?movieLink <http://dbpedia.org/ontology/releaseDate> ?releaseDate }}. " +

                    "{0}" +
                    "{1}" +
                    "FILTER(lang(?title) = 'en') " +
                "}}" +
                "ORDER BY DESC(?releaseDate)" +
                "{2}";

            return String.Format(queryPattern, genreMatch, dateMatch, limit);
        }
    }
}