using System;
using System.Threading.Tasks;
using Bot.Services;
using Bot.Utilities;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;

namespace Bot
{
  public static class DialogHelper
  {
    public static IDialog<Outcome> Build(string type = null)
    {
      var chain1 = type == null
        ? Chain.From(() => FormDialog.FromForm(InsuranceType.BuildForm, FormOptions.PromptInStart))
        : Chain.Return(new InsuranceType { Name = type });

      var chain2 = chain1
        .ContinueWith(async (c, arg) =>
        {
          var result = await arg;
          if (!string.Equals(result.Name, "Car", StringComparison.OrdinalIgnoreCase))
          {
            return Chain.Return((Car)null);
          }

          return FormDialog.FromForm(Car.BuildForm, FormOptions.PromptInStart);
        });

      var chain3 = chain2
        .ContinueWith(async (c, arg) =>
        {
          var result = await arg;
          if (result == null)
          {
            return Chain.Return((Outcome)null);
          }

          return FormDialog.FromForm(Outcome.BuildForm, FormOptions.PromptInStart);
        });

      return chain3;
    }
  }
  
  [Serializable]
  public class InsuranceType
  {
    private const string Site = "https://cisbotstore.blob.core.windows.net/public";

    public string Name { get; set; }

    public static IForm<InsuranceType> BuildForm()
    {
      return new FormBuilder<InsuranceType>()
        .Field(new FieldReflector<InsuranceType>(nameof(Name))
          .SetDefine((state, field) =>
          {
            field.SetType(null);
            field.SetPrompt(new PromptAttribute("What kind of insurance do you need? {||}"));

            foreach (var image in new[] {
              new {name = "Car", url = $"{Site}/auto_600x400.png"},
              new {name = "Property", url = $"{Site}/property_600x400.jpg"},
              new {name = "Life", url = $"{Site}/life_600x400.jpg"}
            })
            {
              field.AddDescription(image.name, image.name, image.url);
              field.AddTerms(image.name, image.name);
            }

            return Task.FromResult(true);
          })
        )
        //.Prompter(CustomPrompter.Prompt)
        .Build();
    }
  }

  [Serializable]
  public class Car
  {
    [Prompt("What kind of car is it? {||}")]
    public CarType CarType { get; set; }
    [Prompt("What make of car do you want to insure?")]
    public string Make { get; set; }
    [Prompt("And the model?")]
    public string Model { get; set; }
    [Numeric(1900, 2017), Prompt("And the year?")]
    public int Year { get; set; }

    public static IForm<Car> BuildForm()
    {
      return new FormBuilder<Car>()
        .Field(nameof(CarType))
        .Field(new FieldReflector<Car>(nameof(Make))
          .SetValidate((state, field) =>
          {
            var result = new ValidateResult {IsValid = true, Value = field};
            if ((string) field == "Ford")
            {
              result.IsValid = false;
              result.Feedback = "Hmmm I don't recognize that make";
            }

            var parts = ((string) field)?.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts != null && parts.Length == 3 && int.TryParse(parts[2], out var year))
            {
              state.Make = parts[0];
              result.Value = state.Make;
              state.Model = parts[1];
              state.Year = year;
            }

            return Task.FromResult(result);
          }))
        .Field(nameof(Model))
        .Field(new FieldReflector<Car>(nameof(Year))
          .SetActive(state => state.Year == 0))
        .Message("That's all I need. Thanks!")
        //.Prompter(CustomPrompter.Prompt)
        .Build();
    }
  }

  [Serializable]
  public class Outcome
  {
    public bool Escalate { get; set; }

    public static IForm<Outcome> BuildForm()
    {
      return new FormBuilder<Outcome>()
        .Message("We can insure your new car for just $116.25 per month. This includes coverage for your whole family and a 10% discount given your existing policy with us.")
        .Field(new FieldReflector<Outcome>(nameof(Escalate))
          .SetPrompt(new PromptAttribute("What do you think? If this sounds good, we can start your coverage right now!"))
          .SetDefine((state, field) =>
          {
            field.SetType(null);
            field.SetRecognizer(new RecognizeString<Outcome>(field));
            return Task.FromResult(true);
          })
          .SetValidate(async (state, field) =>
          {
            var result = new ValidateResult {IsValid = true};
            var sentiment = await TextSentimentService.GetTextSentiment((string) field);
            result.Value = sentiment < 0.5;

            return result;
          })
        )
        .OnCompletion(async (context, state) =>
        {
          await context.PostAsync(
            state.Escalate
              ? "I understand. We really want to make it work. Let me see if a customer service agent is available to review this in more detail."
              : "Great! We are going to prepare everything for you."
          );
        })
        //.Prompter(CustomPrompter.Prompt)
        .Build();
    }
  }

  public enum CarType
  {
    Unknown,
    Sedan,
    [Describe("SUV")]
    Suv,
    Sports
  }
}
