﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using SearchBot.Bot.State;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;

namespace SearchBot.Bot.Dialogs.Service
{
    public class LocationDialog : DialogBase
    {
        public static string Name = typeof(LocationDialog).FullName;

        public LocationDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (dialogContext, cancellationToken) =>
                {
                    // Check if the location is known.
                    var conversationContext = await this.state.GetConversationContext(dialogContext.Context, cancellationToken);
                    if (string.IsNullOrEmpty(conversationContext.Location))
                    {
                        // Prompt for the location.
                        return await dialogContext.PromptAsync(
                            Prompt.TextPrompt,
                            new PromptOptions {
                                Prompt = Phrases.Search.GetLocation(conversationContext.GetServicesString())
                            },
                            cancellationToken);
                        }

                    // Skip this step.
                    return await dialogContext.NextAsync();
                },
                async (dialogContext, cancellationToken) =>
                {
                    if (dialogContext.Result != null)
                    {
                        // Save the location.
                        // TODO: Validate with Maps API
                        var conversationContext = await this.state.GetConversationContext(dialogContext.Context, cancellationToken);
                        conversationContext.Location = (string)dialogContext.Result;
                    }

                    // End this dialog to pop it off the stack.
                    return await dialogContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}