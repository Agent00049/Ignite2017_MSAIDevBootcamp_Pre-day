using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Bot.Services
{
  public class TranslatorService
  {
    private static readonly HttpClient Client = new HttpClient();
    private static string _token;
    private static DateTime? _renew;

    public static async Task<string> Detect(string input)
    {
      var address = "https://api.microsofttranslator.com/v2/Http.svc/Detect?"
        + $"text={WebUtility.UrlEncode(input)}";

      var message = new HttpRequestMessage(HttpMethod.Get, address)
      {
        //Headers = { Authorization = new AuthenticationHeaderValue("Bearer", 
        //  await GetAuthenticationToken()) }
      };

      var result = await Client.SendAsync(message);
      result.EnsureSuccessStatusCode();

      var content = await result.Content.ReadAsStringAsync();
      return XElement.Parse(content).Value;
    }

    public static async Task<string> Translate(string input, string from, string to)
    {
      var address = "https://api.microsofttranslator.com/v2/Http.svc/Translate?"
        + $"text={WebUtility.UrlEncode(input)}&from={from}&to={to}&contentType=text/plain";

      var message = new HttpRequestMessage(HttpMethod.Get, address)
      {
        //Headers = {Authorization = new AuthenticationHeaderValue("Bearer", 
        //  await GetAuthenticationToken())}
      };

      var result = await Client.SendAsync(message);
      result.EnsureSuccessStatusCode();

      var content = await result.Content.ReadAsStringAsync();
      return XElement.Parse(content).Value;
    }

    private static async Task<string> GetAuthenticationToken()
    {
      const string endpoint = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken";

      if (_renew == null || DateTime.Now > _renew)
      {
        //var message = new HttpRequestMessage(HttpMethod.Post, endpoint);
        //message.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["TranslatorServiceKey"]);

        //var response = await Client.SendAsync(message);
        //_token = await response.Content.ReadAsStringAsync();
        _renew = DateTime.Now.AddMinutes(5);
      }

      return _token;
    }
  }
}
