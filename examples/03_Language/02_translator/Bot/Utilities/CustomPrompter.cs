using System.Linq;
using System.Threading.Tasks;
using Bot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using Microsoft.Bot.Connector;

namespace Bot.Utilities
{
  public class CustomPrompter
  {
    public static async Task<FormPrompt> Prompt<T>(IDialogContext context, FormPrompt prompt, T state, IField<T> field)
      where T : class
    {
      var lang = context.ConversationData.GetValueOrDefault("lang", "en");

      var preamble = context.MakeMessage();
      var promptMessage = context.MakeMessage();
      if (prompt.GenerateMessages(preamble, promptMessage))
      {
        await context.PostAsync(preamble);
      }

      if (lang != "en")
      {
        //promptMessage.Text = await TranslatorService.Translate(promptMessage.Text, "en", lang);
        //foreach (var card in promptMessage.Attachments.Select(x => x.Content).OfType<HeroCard>())
        //{
        //  card.Text = await TranslatorService.Translate(card.Text, "en", lang);
        //  foreach (var button in card.Buttons)
        //  {
        //    button.Title = await TranslatorService.Translate(button.Title, "en", lang);
        //    button.Value = await TranslatorService.Translate((string)button.Value, "en", lang);
        //  }
        //}
      }

      await context.PostAsync(promptMessage);

      return prompt;
    }
  }
}
