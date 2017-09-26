using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;

namespace Bot.Services
{
  public class TextSentimentService
  {
    public static async Task<double> GetTextSentiment(string input)
    {
      var client = new TextAnalyticsAPI
      {
        AzureRegion = AzureRegions.Westus,
        SubscriptionKey = ConfigurationManager.AppSettings["TextAnalyticsKey"]
      };

      var language = await client.DetectLanguageAsync(new BatchInput(
        new[] { new Input(id: "0", text: input) }
      ));

      var langCode = language.Documents[0].DetectedLanguages[0].Iso6391Name;

      var sentiment = await client.SentimentAsync(new MultiLanguageBatchInput(
        new[] { new MultiLanguageInput(id: "0", language: langCode, text: input) }
      ));

      return sentiment.Documents[0].Score.GetValueOrDefault();
    }
  }
}
