using System;
using System.IO;
using System.Text;
using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Extensions.Configuration;

// Import namespaces


namespace text_analysis
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Get config settings from AppSettings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile(
                    "appsettings.json"
                );
                IConfigurationRoot configuration = builder.Build();
                string cogSvcEndpoint = configuration["CognitiveServicesEndpoint"];
                string cogSvcKey = configuration["CognitiveServiceKey"];

                // Set console encoding to unicode
                Console.InputEncoding = Encoding.Unicode;
                Console.OutputEncoding = Encoding.Unicode;

                // Create client using endpoint and key
                var key = new AzureKeyCredential(cogSvcKey);
                var endpoint = new Uri(cogSvcEndpoint);
                var client = new TextAnalyticsClient(endpoint, key);

                // Analyze each text file in the reviews folder
                var folderPath = Path.GetFullPath("./reviews");
                DirectoryInfo folder = new DirectoryInfo(folderPath);
                foreach (var file in folder.GetFiles("*.txt"))
                {
                    // Read the file contents
                    Console.WriteLine("\n-------------\n" + file.Name);
                    StreamReader sr = file.OpenText();
                    var text = sr.ReadToEnd();
                    sr.Close();
                    Console.WriteLine("\n" + text);

                    // Get language
                    DetectedLanguage detectedLanguage = client.DetectLanguage(text);
                    Console.WriteLine("\nDetected Language is: " + detectedLanguage.Name);

                    // Get sentiment
                    DocumentSentiment detectedSentiment = client.AnalyzeSentiment(text);
                    Console.WriteLine("\nDetected Total Sentiment is: " + detectedSentiment.Sentiment);

                    // Get key phrases

                    KeyPhraseCollection detectedKeyPhrases = client.ExtractKeyPhrases(text);
                    Console.WriteLine("\nDetected Key Phrases are: ");
                    foreach(var dkp in detectedKeyPhrases){
                        Console.Write($"{dkp} ");
                    }
                    
                    // Get entities
                    CategorizedEntityCollection detectedEntities = client.RecognizeEntities(text);
                    Console.WriteLine("\nExtracted Entities are: ");
                    foreach(var de in detectedEntities){
                        Console.Write($"{de.Text} {de.Category}");
                    }
                    // Get linked entities
                    LinkedEntityCollection linkedEntities = client.RecognizeLinkedEntities(text);
                    Console.WriteLine("\nLinked Entities are: ");
                    foreach(var le in linkedEntities){
                        Console.Write($"{le.Name} {le.Url}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
