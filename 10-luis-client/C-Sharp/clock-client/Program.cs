using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Azure;
using Azure.AI.Language.Conversations;
using Azure.Core;
using System.Text.Json;

// Import namespaces


namespace clock_client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                // Get config settings from AppSettings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile(
                    "appsettings.json"
                );
                IConfigurationRoot configuration = builder.Build();
                string cogSvcKey = configuration["SubscriptionKey"];
                string endpointUrl = configuration["EndpointUrl"];
                string predictionProjectName = configuration["PredictionProjectName"];
                string predictionDeploymentName = configuration["PredictionDeploymentName"];

                // Create a client for the LU app
                Uri endpoint = new Uri(endpointUrl);
                AzureKeyCredential credential = new AzureKeyCredential(cogSvcKey);

                ConversationAnalysisClient client = new ConversationAnalysisClient(
                    endpoint,
                    credential
                );

                // Get user input (until they enter "quit")
                string userText = "";
                while (userText.ToLower() != "quit")
                {
                    Console.WriteLine("\nEnter some text ('quit' to stop)");
                    userText = Console.ReadLine();
                    if (userText.ToLower() != "quit")
                    {
                        // Call the LU app to get intent and entities
                        var data = new
                        {
                            kind = "Conversation",
                            analysisInput = new
                            {
                                conversationItem = new
                                {
                                    text = userText,
                                    id = "1",
                                    participantId = "1",
                                }
                            },
                            parameters = new
                            {
                                projectName = predictionProjectName,
                                deploymentName = predictionDeploymentName,
                                // Use Utf16CodeUnit for strings in .NET.
                                stringIndexType = "Utf16CodeUnit",
                            },
                        };
                        try
                        {
                            //Console.WriteLine(JsonConvert.SerializeObject(data));

                            Response response = client.AnalyzeConversation(
                                RequestContent.Create(data)
                            );

                            using JsonDocument result = JsonDocument.Parse(response.ContentStream);
                            JsonElement conversationalTaskResult = result.RootElement;
                            JsonElement conversationPrediction = conversationalTaskResult
                                .GetProperty("result")
                                .GetProperty("prediction");

                            var topIntent = conversationPrediction
                                .GetProperty("topIntent")
                                .GetString();
                            Console.WriteLine($"Top intent: {topIntent}");

                            // Apply the appropriate action
                            switch (topIntent)
                            {
                                case "GetTime":
                                    //Get top entity
                                    var location = "local";
                                    foreach (
                                        JsonElement entity in conversationPrediction
                                            .GetProperty("entities")
                                            .EnumerateArray()
                                    )
                                    {
                                        var category = entity.GetProperty("category").GetString();
                                        if (category == "Location")
                                        {
                                            location = entity.GetProperty("text").GetString(); //get the city name
                                        }
                                    }

                                    Console.WriteLine(GetTime(location));

                                    break;
                                case "GetDay":
                                    var date = DateTime.Today.ToShortDateString();
                                    foreach (
                                        JsonElement entity in conversationPrediction
                                            .GetProperty("entities")
                                            .EnumerateArray()
                                    )
                                    {
                                        var category = entity.GetProperty("category").GetString();
                                        if (category == "Date")
                                        {
                                            date = entity.GetProperty("text").GetString(); //get the city name
                                        }
                                    }

                                    Console.WriteLine(GetDay(date));

                                    break;
                                case "GetDate":
                                    var day = DateTime.Today.DayOfWeek.ToString();
                                    foreach (
                                        JsonElement entity in conversationPrediction
                                            .GetProperty("entities")
                                            .EnumerateArray()
                                    )
                                    {
                                        var category = entity.GetProperty("category").GetString();
                                        if (category == "Weekday")
                                        {
                                            day = entity.GetProperty("text").GetString(); //get the city name
                                        }
                                    }

                                    Console.WriteLine(GetDate(day));
                                    break;
                                default:
                                    Console.WriteLine("Try another text related to time");
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static string GetTime(string location)
        {
            var timeString = "";
            var time = DateTime.Now;

            /* Note: To keep things simple, we'll ignore daylight savings time and support only a few cities.
               In a real app, you'd likely use a web service API (or write  more complex code!)
               Hopefully this simplified example is enough to get the the idea that you
               use LU to determine the intent and entitites, then implement the appropriate logic */

            switch (location.ToLower())
            {
                case "local":
                    timeString = time.Hour.ToString() + ":" + time.Minute.ToString("D2");
                    break;
                case "london":
                    time = DateTime.UtcNow;
                    timeString = time.Hour.ToString() + ":" + time.Minute.ToString("D2");
                    break;
                case "sydney":
                    time = DateTime.UtcNow.AddHours(11);
                    timeString = time.Hour.ToString() + ":" + time.Minute.ToString("D2");
                    break;
                case "new york":
                    time = DateTime.UtcNow.AddHours(-5);
                    timeString = time.Hour.ToString() + ":" + time.Minute.ToString("D2");
                    break;
                case "nairobi":
                    time = DateTime.UtcNow.AddHours(3);
                    timeString = time.Hour.ToString() + ":" + time.Minute.ToString("D2");
                    break;
                case "tokyo":
                    time = DateTime.UtcNow.AddHours(9);
                    timeString = time.Hour.ToString() + ":" + time.Minute.ToString("D2");
                    break;
                case "delhi":
                    time = DateTime.UtcNow.AddHours(5.5);
                    timeString = time.Hour.ToString() + ":" + time.Minute.ToString("D2");
                    break;
                default:
                    timeString = "I don't know what time it is in " + location;
                    break;
            }

            return timeString;
        }

        static string GetDate(string day)
        {
            string date_string = "I can only determine dates for today or named days of the week.";

            // To keep things simple, assume the named day is in the current week (Sunday to Saturday)
            DayOfWeek weekDay;
            if (Enum.TryParse(day, true, out weekDay))
            {
                int weekDayNum = (int)weekDay;
                int todayNum = (int)DateTime.Today.DayOfWeek;
                int offset = weekDayNum - todayNum;
                date_string = DateTime.Today.AddDays(offset).ToShortDateString();
            }
            return date_string;
        }

        static string GetDay(string date)
        {
            // Note: To keep things simple, dates must be entered in US format (MM/DD/YYYY)
            string day_string = "Enter a date in MM/DD/YYYY format.";
            DateTime dateTime;
            if (DateTime.TryParse(date, out dateTime))
            {
                day_string = dateTime.DayOfWeek.ToString();
            }

            return day_string;
        }
    }
}
