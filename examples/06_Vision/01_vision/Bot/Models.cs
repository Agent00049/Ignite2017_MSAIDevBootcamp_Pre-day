using System;
using System.Threading.Tasks;
using Bot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;

namespace Bot
{
  public static class DialogHelper
  {
    public static IDialog<Car> Build(string type = null)
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

      return chain2;
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
        .Build();
    }
  }

  [Serializable]
  public class Car
  {
    [Prompt("What kind of car is it? {||}")]
    public CarType CarType { get; set; }
    [Prompt("Please upload an image of your car to continue.")]
    public string ImageUrl { get; set; }

    public static IForm<Car> BuildForm()
    {
      return new FormBuilder<Car>()
        .Field(nameof(CarType))
        .Field(new FieldReflector<Car>(nameof(ImageUrl))
          .SetValidate(async (state, field) =>
          {
            var url = (string) field;
            if (!Uri.TryCreate(url, UriKind.Absolute, out var _))
            {
              return new ValidateResult {IsValid = false, Feedback = "Hmmm something went wrong. Please try again."};
            }

            //var computerVisionService = new ComputerVisionService();
            //var detectResult = await computerVisionService.Detect(url);

            //if (!detectResult.IsCar)
            //{
            //  var message = $"That doesn't look like a car. It looks more like {detectResult.Description}.";

            //  var faceRecognitionService = new FaceRecognitionService();
            //  var isUser = await faceRecognitionService.Verify(url);
            //  if (isUser)
            //  {
            //    message += " That's a nice photo of you by the way!";
            //  }

            //  return new ValidateResult {IsValid = false, Feedback = message};
            //}

            //var customVisionService = new CustomVisionService();
            //var customResult = await customVisionService.Analyze(url);
            //var isRightCarType = string.Equals(customResult.TopPrediction.Tag, state.CarType.ToString(), StringComparison.OrdinalIgnoreCase);
            //if (!isRightCarType)
            //{
            //  return new ValidateResult {IsValid = false, Feedback = $"That doesn’t look like a {state.CarType}."};
            //}

            return new ValidateResult {IsValid = true, Value = url};
          }))
        .Message("That looks great. Nice purchase!")
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
