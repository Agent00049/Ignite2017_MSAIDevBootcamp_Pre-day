using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Bot.Services
{
  /// <summary>
  /// Service to call the Custom Vision API
  /// </summary>
  public class CustomVisionService
  {
    private static readonly HttpClient Client = new HttpClient();
    private readonly string _endpoint;
    private readonly string _key;

    public CustomVisionService()
    {
      _endpoint = $"https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/{ConfigurationManager.AppSettings["CustomVisionProjectId"]}/image";
      _key = ConfigurationManager.AppSettings["CustomVisionKey"];
    }

    //public async Task<CustomVisionResponse> Analyze(string imageUrl)
    //{
    //  // Fetch image
    //  var image = await Client.GetByteArrayAsync(imageUrl);

    //  // Prepare message
    //  var request = new HttpRequestMessage(HttpMethod.Post, _endpoint);
    //  request.Headers.Add("Prediction-Key", _key);
    //  request.Content = new ByteArrayContent(image)
    //  {
    //    Headers = {ContentType = new MediaTypeHeaderValue("application/octet-stream")}
    //  };

    //  // Submit
    //  var response = await Client.SendAsync(request);
    //  response.EnsureSuccessStatusCode();

    //  return JsonConvert.DeserializeObject<CustomVisionResponse>(
    //    await response.Content.ReadAsStringAsync());
    //}
  }

  public class CustomVisionResponse
  {
    public List<Prediction> Predictions { get; set; }

    public Prediction TopPrediction
    {
      get
      {
        return Predictions.OrderByDescending(item => item.Probability).First();
      }
    }
  }

  public class Prediction
  {
    public string Tag { get; set; }
 
    public double? Probability { get; set; }
  }
}
