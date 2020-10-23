using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System.Linq;

namespace ProfitOfParka
{
    class Program
    {
        public class Weather
        {
            public string Date { get; set; }
            public double Temperature { get; set; }
        }

        static void Main(string[] args)
        {
            string error = null;
            try
            {
                //Open the Json file
                string filepath = "weather_data.json";
                if (!File.Exists(filepath))
                {
                    string myError = string.Format("Cannot find file [{0}].", filepath);
                    throw new ApplicationException(myError);
                }

                //Read the Json file
                using (StreamReader r = new StreamReader(filepath))
                {
                    string json = r.ReadToEnd();

                    List<Weather> w = JsonSerializer.Deserialize<List<Weather>>(json);

                    //Sort object weather in the weatherList by Date
                    List<Weather> weatherList = w.OrderBy(d => d.Date).ToList();

                    //Calculate maximum profit per parka
                    double minPrice = double.MaxValue;
                    double maxProfit = 0.0;
                    double maximumProfit = 0.0;

                    for (int i = 0; i <= weatherList.Count - 1; i++)
                    {
                        double price = 754 / (Math.Exp(weatherList[i].Temperature / 100));
                        if (price < minPrice)
                            minPrice = price;
                        else if ((price - minPrice) > maxProfit)
                        {
                            maxProfit = price - minPrice;
                        }
                    }

                    Console.WriteLine("The maximum profit I can obtain per parka is ${0:#,0.00}.", maxProfit);
                    Console.WriteLine();

                    GetMaximumProfit(weatherList, ref maximumProfit);

                    Console.WriteLine("The Maximum Profit I can obtain is ${0:#,0.00}.", maximumProfit);
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                Console.WriteLine();
                Console.WriteLine("Failed to run the application.");
                Console.WriteLine("Error as below:");
                Console.WriteLine(error);
            }

            Console.Read();
        }

        public static void GetMaximumProfit(List<Weather> weathers, ref double maxProfit)
        {
            {
                double maxPrice = 0.0;
                int pos = 0;
                int num = 0;

                //Find out the highest selling price of parkas in the list and log its position
                for (int i = 0; i <= weathers.Count - 1; i++)
                {
                    double price = 754 / (Math.Exp(weathers[i].Temperature / 100));

                    if (price > maxPrice)
                    {
                        maxPrice = price;
                        pos = i;
                    }
                }

                //If the index of the highest price of parkas is not the first one in the list,
                //start buying parkas, calculate costs and amounts of parkas you bought;
                //then calculate the maximum profit you can obtain when you sold all parkas at the highest selling price
                if (pos > 0)
                {
                    double price = 0.0;
                    for (int i = 0; i < pos; i++)
                    {
                        price += 754 / (Math.Exp(weathers[i].Temperature / 100));
                        num++;
                    }

                    maxProfit += maxPrice * num - price;
                }

                // If the index of the highest price of parkas is the last one in the list, calculation is done and return to caller
                if (pos == weathers.Count - 1)
                    return;

                //Pass the remains of the list                
                List<Weather> newWeatherList = new List<Weather>();

                for (int i = pos + 1; i <= weathers.Count - 1; i++)
                    newWeatherList.Add(weathers[i]);

                GetMaximumProfit(newWeatherList, ref maxProfit);
            }
        }
    }
}