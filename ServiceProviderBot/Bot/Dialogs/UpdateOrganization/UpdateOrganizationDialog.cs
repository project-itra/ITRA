﻿using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity;
using ServiceProviderBot.Bot.Utils;
using Shared;
using Shared.ApiInterface;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization
{
    public class UpdateOrganizationDialog : DialogBase
    {
        public static string Name = typeof(UpdateOrganizationDialog).FullName;

        public UpdateOrganizationDialog(StateAccessors state, DialogSet dialogs, ApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    var needsUpdate = await NeedsUpdate(state, api, stepContext.Context);
                    if (!needsUpdate)
                    {
                        // Nothing to update.
                        await Messages.SendAsync(Phrases.Update.NothingToUpdate, stepContext.Context, cancellationToken);

                        // End this dialog to pop it off the stack.
                        return await stepContext.EndDialogAsync(cancellationToken);
                    }

                    // Push the update capacity dialog onto the stack.
                    return await BeginDialogAsync(stepContext, UpdateCapacityDialog.Name, null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Send the closing message.
                    await Messages.SendAsync(Phrases.Update.Closing, stepContext.Context, cancellationToken);

                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }

        private static async Task<bool> NeedsUpdate(StateAccessors state, ApiInterface api, ITurnContext context)
        {
            var serviceCount = await api.GetServiceCount(Helpers.GetPhoneNumber(context));
            return serviceCount > 0;
        }
    }
}
