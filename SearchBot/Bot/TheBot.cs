﻿using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Dialogs;
using ServiceProviderBot.Bot.Prompts;
using Shared;
using Shared.ApiInterface;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceProviderBot.Bot
{
    public class TheBot : IBot
    {
        private readonly StateAccessors state;
        private readonly DialogSet dialogs;
        private readonly IApiInterface api;
        private readonly IConfiguration configuration;
        private string userToken;

        public TheBot(IConfiguration configuration, StateAccessors state, EfInterface api)
        {
            this.configuration = configuration;

            this.state = state ?? throw new ArgumentNullException(nameof(state));
            this.dialogs = new DialogSet(state.DialogContextAccessor);

            this.api = api ?? throw new ArgumentNullException(nameof(api));

            // Register prompts.
            Prompt.Register(this.dialogs);
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.userToken = Helpers.GetUserToken(turnContext);

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Establish context for our dialog from the turn context.
                DialogContext dialogContext = await this.dialogs.CreateContextAsync(turnContext, cancellationToken);

                // Make sure this channel is supported.
                if (!Phrases.ValidChannels.Contains(turnContext.Activity.ChannelId))
                {
                    await Messages.SendAsync(Phrases.Greeting.InvalidChannel(turnContext), turnContext, cancellationToken);
                    return;
                }

                // Create the master dialog.
                var masterDialog = new MasterDialog(this.state, this.dialogs, this.api, this.configuration);

                // Attempt to continue any existing conversation.
                DialogTurnResult result = await masterDialog.ContinueDialogAsync(dialogContext, cancellationToken);

                // Start a new conversation if there isn't one already.
                if (result.Status == DialogTurnStatus.Empty)
                {
                    await masterDialog.BeginDialogAsync(dialogContext, MasterDialog.Name, null, cancellationToken);
                }
            }
        }
    }
}