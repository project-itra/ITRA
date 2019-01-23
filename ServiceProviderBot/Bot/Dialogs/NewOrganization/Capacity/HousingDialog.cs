﻿using EntityModel;
using Microsoft.Bot.Builder.Dialogs;
using ServiceProviderBot.Bot.Utils;

namespace ServiceProviderBot.Bot.Dialogs.NewOrganization.Capacity
{
    public class HousingDialog : DialogBase
    {
        public static string Name = typeof(HousingDialog).FullName;

        /// <summary>Creates a dialog for getting housing capacity.</summary>
        /// <param name="state">The state accessors.</param>
        public override WaterfallDialog Init(DbModel dbContext, StateAccessors state, DialogSet dialogs)
        {
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Prompt for the total beds.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.IntPrompt,
                        new PromptOptions { Prompt = Phrases.Capacity.GetHousingTotal },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the profile with the total beds.
                    var profile = await state.GetOrganizationProfile(stepContext.Context, cancellationToken);
                    profile.Capacity.Beds.Total = (int)stepContext.Result;

                    // Prompt for the open beds.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.IntPrompt,
                        new PromptOptions { Prompt = Phrases.Capacity.GetHousingOpen },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var profile = await state.GetOrganizationProfile(stepContext.Context, cancellationToken);

                    // Validate the numbers.
                    var open = (int)stepContext.Result;
                    if (open > profile.Capacity.Beds.Total)
                    {
                        profile.Capacity.Beds.SetToNone();

                        // Send error message.
                        await Messages.SendAsync(Phrases.Capacity.GetHousingError, stepContext.Context, cancellationToken);

                        // Repeat the dialog.
                        return await stepContext.ReplaceDialogAsync(Name, null, cancellationToken);
                    }

                    // Update the profile with the open beds.
                    profile.Capacity.Beds.Open = (int)stepContext.Result;

                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
