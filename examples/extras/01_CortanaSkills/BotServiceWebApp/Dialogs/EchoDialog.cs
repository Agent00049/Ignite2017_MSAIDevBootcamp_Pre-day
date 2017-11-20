using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace Bot.Dialogs
{
  [Serializable]
  public class EchoDialog : LuisDialog<object>
  {
    private const string RideSharingApiUrl = "https://connect-ride-sharing-api.azurewebsites.net/api/ride-sharing/price";

    public EchoDialog()
      : base(new LuisService(new LuisModelAttribute(ConfigurationManager.AppSettings["LuisAppId"], ConfigurationManager.AppSettings["LuisAPIKey"])))
    {
    }

    [LuisIntent("GetKindOfVehicles")]
    public async Task GetKindOfVehicles(IDialogContext context, LuisResult result)
    {
      // Initialize Carousel
      var replyToConversation = context.MakeMessage();
      replyToConversation.AttachmentLayout = AttachmentLayoutTypes.Carousel;
      replyToConversation.Type = ActivityTypes.Message;

      // Add card to carousel
      var carTypes = new string[] { "Sedan", "SUV", "Sports car" };
      var actions = new List<CardAction>();
      foreach (string type in carTypes)
      {
        CardAction kind = new CardAction
        {
          Value = type,
          Type = "imBack",
          Title = type
        };
        actions.Add(kind);
      }

      // Add card with options to the Carousel
      HeroCard card = new HeroCard
      {
        Title = "Kinds of cars available",
        Buttons = actions
      };

      replyToConversation.Attachments = new List<Attachment> { card.ToAttachment() };

      await context.PostAsync(replyToConversation);
      context.Wait(MessageReceived);
    }

    [LuisIntent("None")]
    public async Task None(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult result)
    {
      var messageToForward = await message;
      await context.Forward(new QnADialog(), AfterQnADialog, messageToForward, CancellationToken.None);
    }

    private async Task AfterQnADialog(IDialogContext context, IAwaitable<object> result)
    {
      context.Wait(MessageReceived);
    }

    [LuisIntent("Greetings")]
    [LuisIntent("")]
    public async Task Greetings(IDialogContext context, LuisResult result)
    {
      var message = context.MakeMessage();
      message.Text = "Hello! I am your ride sharing assistant.";
      message.Speak = "<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en-US\"><emphasis level=\"moderate\">Hello!</emphasis> I am your ride sharing assistant.</speak>";
      await context.PostAsync(message);
      context.Wait(MessageReceived);
    }

    [LuisIntent("GetPriceEstimate")]
    public async Task GetPriceEstimate(IDialogContext context, LuisResult result)
    {
      EntityRecommendation location;
      if (result.TryFindEntity("location", out location))
      {
        var activity = (IMessageActivity)context.Activity;

        var token = (string)activity.Entities.First(x => x.Type == "AuthorizationToken").Properties["token"];
        string name;
        using (var client = new HttpClient())
        {
          client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
          var rawResponse = await client.GetStringAsync($"https://graph.microsoft.com/v1.0/me");
          var meResponse = JsonConvert.DeserializeObject<MeResponse>(rawResponse);
          name = meResponse.GivenName;
        }

        dynamic userInfo = activity.Entities.First(x => x.Type == "UserInfo").Properties;
        string lat = userInfo.Location.Hub.Latitude;
        string lon = userInfo.Location.Hub.Longitude;

        string rs;
        using (var client = new HttpClient())
        {
          var response = await client.GetAsync($"{RideSharingApiUrl}?from={lat},{lon}&to={location.Entity}");
          response.EnsureSuccessStatusCode();
          rs = await response.Content.ReadAsAsync<string>();
        }

        var message = context.MakeMessage();
        message.Text = $"{name}, I estimate that it will cost {rs} to get to {location.Entity}.";
        message.Speak = $"<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en-US\">{name}, I estimate that it will cost <emphasis level=\"moderate\">{rs}</emphasis> to get to {location.Entity}.</speak>";

        await context.PostAsync(message);
      }

      context.Wait(MessageReceived);
    }
  }

  [Serializable]
  public class MeResponse
  {
    public string GivenName { get; set; }

    public string Surname { get; set; }
  }
}