using System;
using System.IO;
using Newtonsoft.Json;

namespace ConsoleVersion
{
    public static class Utitlities
    {
        public static LuisJsonModel CallLuis()
        {
            var file = Path.Combine(@".\test2LuisData.json");
            var data = JsonConvert.DeserializeObject<LuisJsonModel>(File.ReadAllText(file));

            return data;
        }

        public static string ExtractLuisData(LuisJsonModel luisJson)
        {
            int numberOfItems = 0;
            string genre = String.Empty;
            int year = 0;
            string exactDate = String.Empty;

            foreach (var i in luisJson.Entities)
            {
                switch (i.Type)
                {
                    case "builtin.number":
                        if (int.TryParse(i.Resolution.Value, out int number))
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

        private static string CreateSparqlQuery(int numberOfItems, string genre, int year, string exactDate)
        {
            Console.WriteLine("Number of items: " + numberOfItems);
            Console.WriteLine("Genre: " + genre);
            Console.WriteLine("Year: " + year);
            Console.WriteLine("Exact data: " + exactDate);
            Console.WriteLine();

            string limit = numberOfItems > 0 ? String.Format("LIMIT({0})", numberOfItems) : "";
            string genreMatch = !String.IsNullOrEmpty(genre.Trim()) ? String.Format("FILTER ( regex (str(?genre), '{0}', 'i'))", genre) : "";
            string dateMatch = "";

            if (!String.IsNullOrEmpty(exactDate.Trim()))
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