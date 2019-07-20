﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Utils;
using Shared;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateHousingDialog : DialogBase
    {
        public static string Name = typeof(UpdateHousingDialog).FullName;

        public UpdateHousingDialog(StateAccessors state, DialogSet dialogs, DbInterface database, IConfiguration configuration)
            : base(state, dialogs, database, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Prompt for the open beds.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.IntPrompt,
                        new PromptOptions { Prompt = Phrases.Capacity.GetHousingOpen },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var organization = await database.GetOrganization(stepContext.Context);              

                    // Validate the numbers.
                    var open = (int)stepContext.Result;
                    if (open > organization.BedsTotal)
                    {
                        // Send error message.
                        var error = string.Format(Phrases.Capacity.GetHousingErrorFormat(organization.BedsTotal));
                        await Messages.SendAsync(error, stepContext.Context, cancellationToken);

                        // Repeat the dialog.
                        return await stepContext.ReplaceDialogAsync(Name, null, cancellationToken);
                    }

                    // Update the profile with the open beds.
                    var snapshot = await database.GetSnapshot(stepContext.Context);
                    snapshot.BedsOpen = (int)stepContext.Result;
                    await database.Save();

                    // Prompt for the waitlist length if necessary.
                    if (open == 0 && organization.BedsWaitlist)
                    {
                        return await stepContext.PromptAsync(
                            Utils.Prompts.IntPrompt,
                            new PromptOptions { Prompt = Phrases.Capacity.GetHousingWaitlist },
                            cancellationToken);
                    }

                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the profile with the waitlist length.
                    var snapshot = await database.GetSnapshot(stepContext.Context);
                    snapshot.BedsWaitlist = (int)stepContext.Result;
                    await database.Save();

                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
