﻿using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.State;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceProviderBot.Bot.Dialogs.Preferences
{
    public class TimeDialog : DialogBase
    {
        public static string Name = typeof(TimeDialog).FullName;

        public TimeDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        // Check their current time to determine their timezone.
                        return await dialogContext.PromptAsync(
                            Prompt.HourMinutePrompt,
                            new PromptOptions {
                                Prompt = Phrases.Preferences.GetCurrentTime,
                                RetryPrompt = Phrases.Preferences.GetCurrentTimeRetry
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // Get the result. This was already validated by the prompt.
                        DateTimeHelper.ParseHourAndMinute((string)dialogContext.Result, out DateTime dt);

                        // Determine the difference between the user's time and UTC and save the result in the user context.
                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                        userContext.TimezoneOffset = (dt - DateTime.UtcNow).Hours;

                        return await dialogContext.NextAsync(cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // Get the time they want to be contacted.
                        return await dialogContext.PromptAsync(
                            Prompt.HourPrompt,
                            new PromptOptions {
                                Prompt = Phrases.Preferences.GetUpdateTime,
                                RetryPrompt = Phrases.Preferences.GetUpdateTimeRetry
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // Get the result. This was already validated by the prompt.
                        DateTimeHelper.ParseHour((string)dialogContext.Result, out DateTime dt);

                        // Adjust the contact time by the timezone offset so that it is stored in UTC.
                        // Negated in order to undo the offset.
                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                        dt = dt.AddHours(-userContext.TimezoneOffset);

                        // Update the user's preference.
                        var user = await api.GetUser(dialogContext.Context);
                        user.ReminderTime = dt.ToShortTimeString();
                        await this.api.Update(user);

                        // Send a confirmation message.
                        await Messages.SendAsync(Phrases.Preferences.Updated, dialogContext.Context, cancellationToken);

                        // End this dialog to pop it off the stack.
                        return await dialogContext.EndDialogAsync(cancellationToken);
                    }
                });
            });
        }
    }
}