using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

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

    private async Task AfterCompleteAsync(IDialogContext context, IAwaitable<object> argument)
    {
      context.Wait(MessageReceived);
    }
  }
}
