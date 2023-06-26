using System;
using System.Media;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;

// Import namespaces


namespace speaking_clock
{
    class Program
    {
        private static SpeechConfig speechConfig;

        static async Task Main(string[] args)
        {
            try
            {
                // Get config settings from AppSettings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile(
                    "appsettings.json"
                );
                IConfigurationRoot configuration = builder.Build();
                string cogSvcKey = configuration["CognitiveServiceKey"];
                string cogSvcRegion = configuration["CognitiveServiceRegion"];

                // Configure speech service
                speechConfig = SpeechConfig.FromSubscription(cogSvcKey, cogSvcRegion);
                Console.WriteLine("Ready to use Speech Service in " + speechConfig.Region);

                // Get spoken input
                string command = "";
                command = await TranscribeCommand();
                if (command.ToLower() == "what time is it?")
                {
                    await TellTime();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static async Task<string> TranscribeCommand()
        {
            string command = "";

            // Configure speech recognition
            // using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            // using var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);
            // Configure speech recognition
            string audioFile = "time.wav";
            SoundPlayer wavPlayer = new SoundPlayer(audioFile);
            wavPlayer.Play();
            using AudioConfig audioConfig = AudioConfig.FromWavFileInput(audioFile);
            using SpeechRecognizer speechRecognizer = new SpeechRecognizer(
                speechConfig,
                audioConfig
            );
            //Console.WriteLine("Speak now...");

            // Process speech input
            SpeechRecognitionResult recognizeResult = await speechRecognizer.RecognizeOnceAsync();
            if (recognizeResult.Reason == ResultReason.RecognizedSpeech)
            {
                command = recognizeResult.Text;
            }
            else
            {
                Console.WriteLine(recognizeResult.Reason);
                if (recognizeResult.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(recognizeResult);
                    Console.WriteLine(cancellation.Reason);
                    Console.WriteLine(cancellation.ErrorCode);
                    Console.WriteLine(cancellation.ErrorDetails);
                }
            }
            // Return the command
            return command;
        }

        static async Task TellTime()
        {
            var now = DateTime.Now;
            string responseText =
                "The time is " + now.Hour.ToString() + ":" + now.Minute.ToString("D2");

            // Configure speech synthesis

            speechConfig.SpeechSynthesisVoiceName = "en-GB-LibbyNeural";
            //speechConfig.SpeechSynthesisVoiceName = "en-GB-RyanNeural";
            using SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer(speechConfig);

            // Synthesize spoken output
            string responseSsml =
                $@"
                <speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-US'>
                    <voice name='en-GB-LibbyNeural'>
                        {responseText}
                        <break strength='weak'/>
                        Time to end this lab!
                    </voice>
                </speak>";
            SpeechSynthesisResult speak = await speechSynthesizer.SpeakSsmlAsync(responseSsml);
            if (speak.Reason != ResultReason.SynthesizingAudioCompleted)
            {
                Console.WriteLine(speak.Reason);
            }

            // SpeechSynthesisResult speak = await speechSynthesizer.SpeakTextAsync(responseText);
            // if (speak.Reason != ResultReason.SynthesizingAudioCompleted)
            // {
            //     Console.WriteLine(speak.Reason);
            // }

            // Print the response
            Console.WriteLine(responseText);
        }
    }
}
