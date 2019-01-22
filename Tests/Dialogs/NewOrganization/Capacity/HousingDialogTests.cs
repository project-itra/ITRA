﻿using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.NewOrganization.Capacity;
using ServiceProviderBot.Bot.Models.OrganizationProfile;
using ServiceProviderBot.Bot.Utils;
using Xunit;

namespace Tests.Dialogs.NewOrganization.Capacity
{
    public class HousingDialogTests : DialogTestBase
    {
        [Fact]
        public async Task Valid()
        {
            var expected = new OrganizationProfile();
            expected.Capacity.Beds.Total = 10;
            expected.Capacity.Beds.Open = 5;

            // Execute the conversation.
            await CreateTestFlow(HousingDialog.Name)
                .Test("begin", Phrases.Capacity.GetHousingTotal)
                .Test(expected.Capacity.Beds.Total.ToString(), Phrases.Capacity.GetHousingOpen)
                .Send(expected.Capacity.Beds.Open.ToString())
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expected);
        }

        [Fact]
        public async Task Invalid()
        {
            // Execute the conversation.
            await CreateTestFlow(HousingDialog.Name)
                .Test("begin", Phrases.Capacity.GetHousingTotal)
                .Test("5", Phrases.Capacity.GetHousingOpen)
                .Test("10", Phrases.Capacity.GetHousingError)
                .StartTestAsync();
        }
    }
}
