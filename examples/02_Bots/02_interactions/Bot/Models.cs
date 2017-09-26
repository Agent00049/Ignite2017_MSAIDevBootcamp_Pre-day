using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;

namespace Bot
{
  public static class DialogHelper
  {
    public static IDialog<Car> Build()
    {
      //var chain1 = Chain.From(() =>
      //  FormDialog.FromForm(InsuranceType.BuildForm, FormOptions.PromptInStart));

      //var chain2 = chain1
      //  .ContinueWith(async (c, arg) =>
      //  {
      //    var result = await arg;
      //    if (!string.Equals(result.Name, "Car", StringComparison.OrdinalIgnoreCase))
      //    {
      //      return Chain.Return((Car)null);
      //    }

      //    return FormDialog.FromForm(Car.BuildForm, FormOptions.PromptInStart);
      //  });

      //return chain2;

      return FormDialog.FromForm(Car.BuildForm, FormOptions.PromptInStart);
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
        //.Field(new FieldReflector<InsuranceType>(nameof(Name))
        //  .SetDefine((state, field) =>
        //  {
        //    field.SetType(null);
        //    field.SetPrompt(new PromptAttribute("What kind of insurance do you need? {||}"));

        //    foreach (var image in new[] {
        //      new {name = "Car", url = $"{Site}/auto_600x400.png"},
        //      new {name = "Property", url = $"{Site}/property_600x400.jpg"},
        //      new {name = "Life", url = $"{Site}/life_600x400.jpg"}
        //    })
        //    {
        //      field.AddDescription(image.name, image.name, image.url);
        //      field.AddTerms(image.name, image.name);
        //    }

        //    return Task.FromResult(true);
        //  })
        //)
        .Build();
    }
  }

  [Serializable]
  public class Car
  {
    //[Prompt("What kind of car is it? {||}")]
    public CarType CarType { get; set; }
    //[Describe("What badge does it have?")]
    public string Make { get; set; }
    public string Model { get; set; }
    //[Numeric(1900, 2017), Prompt("What year is it?")]
    public int Year { get; set; }

    public static IForm<Car> BuildForm()
    {
      return new FormBuilder<Car>()
        //.Field(nameof(CarType))
        //.Field(new FieldReflector<Car>(nameof(Make))
        //  .SetValidate((state, field) =>
        //  {
        //    var result = new ValidateResult {IsValid = true, Value = field};
        //    if ((string) field == "Ford")
        //    {
        //      result.IsValid = false;
        //      result.Feedback = "Hmmm I don't recognize that make";
        //    }

        //    return Task.FromResult(result);
        //  })
        //  .SetPrompt(new PromptAttribute("And the make?", "What make is it?")))
        //.AddRemainingFields()
        //.Message("That's all I need. Thanks!")
        .Build();
    }
  }

  public enum CarType
  {
    Unknown,
    Sedan,
    //[Describe("SUV")]
    Suv,
    Sports
  }
}
