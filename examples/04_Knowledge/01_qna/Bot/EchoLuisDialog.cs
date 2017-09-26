using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace Bot
{
  [LuisModel("", ""), Serializable]
  public class EchoLuisDialog : LuisDialog<object>
  {
    [LuisIntent("INeedInsurance")]
    public async Task INeedInsurance(IDialogContext context, LuisResult result)
    {
      var type = result.Entities.FirstOrDefault(x => x.Type == "InsuranceType")?.Entity;
      context.Call(DialogHelper.Build(type), AfterCompleteAsync);
    }

    [LuisIntent("None")]
    public async Task None(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
    {
      //var message = await activity;
      //await context.Forward(new EchoQnADialog(), AfterQnAMakerAsync, message, CancellationToken.None);
    }

    private async Task AfterCompleteAsync(IDialogContext context, IAwaitable<Outcome> argument)
    {
      context.Wait(MessageReceived);
    }
    private async Task AfterQnAMakerAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
    {
      context.Wait(MessageReceived);
    }
  }
}
