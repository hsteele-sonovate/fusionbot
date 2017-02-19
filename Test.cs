using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.FormFlow;
using System.Linq;

// For more information about this template visit http://aka.ms/azurebots-csharp-luis
[Serializable]
public class BasicLuisDialog : LuisDialog<object>
{
    public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute("0fe67cd2-68de-46f6-bc8b-a4867e97bdcd", "d4c81f2fc107452da3b7f979c22b101b")))
    {
    }

    [LuisIntent("None")]
    public async Task NoneIntent(IDialogContext context, LuisResult result)
    {
        await context.PostAsync($"You have reached the none intent. You said: {result.Query}"); //
        context.Wait(MessageReceived);
    }

    [LuisIntent("Greet")]
    public async Task GreetIntent(IDialogContext context, LuisResult result)
    {
        await context.PostAsync($"How can I help?"); //
        context.Wait(MessageReceived);
    }


    [LuisIntent("GetPlacementHelp")]
    public async Task MyIntent(IDialogContext context, LuisResult result)
    {
        await context.PostAsync($"Alright, I'll try and help.");
        var frm = new PlacementForm();

        var val = result.TopScoringIntent.Actions.FirstOrDefault()?.Parameters.FirstOrDefault()?.Value.FirstOrDefault().Resolution.FirstOrDefault().Value;
        if (val != null)
        {
            frm.PlacementId = val;
        }

        var placementForm = new FormDialog<PlacementForm>(frm, PlacementForm.BuildForm, FormOptions.PromptInStart);
        context.Call(placementForm, PlacementFormComplete);
    }

    public async Task PlacementFormComplete(IDialogContext ctx, IAwaitable<PlacementForm> result)
    {
        var placement = await result;
        await ctx.PostAsync($"Let me have a look for placement {placement.PlacementId}");
    }

    [Serializable]
    public class PlacementForm
    {
        [Prompt("What's the placement's id?")]
        public string PlacementId { get; set; }

        public static IForm<PlacementForm> BuildForm()
        {
            return new FormBuilder<PlacementForm>()
                .Field(nameof(PlacementId))
                .AddRemainingFields()
                .Build();
        }
    }
}