﻿using EntityModel;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity;
using Shared;
using Shared.ApiInterface;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateMentalHealthDialogTests : DialogTestBase
    {
        [Fact]
        public async Task Update()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<MentalHealthData>(this.api, organization.Id);
            var data = await TestHelpers.CreateMentalHealthData(this.api, service.Id, true, true, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateMentalHealthDialog.Name, user)
                .Test("test", Phrases.Capacity.MentalHealth.GetInPatientOpen)
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.MentalHealth.GetOutPatientOpen)
                .Send(TestHelpers.DefaultOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<MentalHealthData>(this.userToken, true);
            Assert.Equal(TestHelpers.DefaultOpen, resultData.InPatientOpen);
            Assert.Equal(TestHelpers.DefaultOpen, resultData.OutPatientOpen);
        }

        [Fact]
        public async Task Waitlist()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<MentalHealthData>(this.api, organization.Id);
            var data = await TestHelpers.CreateMentalHealthData(this.api, service.Id, true, true, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateMentalHealthDialog.Name, user)
                .Test("test", Phrases.Capacity.MentalHealth.GetInPatientOpen)
                .Test("0", Phrases.Capacity.GetWaitlistLength(Phrases.Capacity.MentalHealth.InPatientName))
                .Test(TestHelpers.DefaultWaitlistLength.ToString(), Phrases.Capacity.MentalHealth.GetOutPatientOpen)
                .Test("0", Phrases.Capacity.GetWaitlistLength(Phrases.Capacity.MentalHealth.OutPatientName))
                .Send(TestHelpers.DefaultWaitlistLength.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<MentalHealthData>(this.userToken, true);
            Assert.Equal(0, resultData.InPatientOpen);
            Assert.Equal(0, resultData.OutPatientOpen);
            Assert.Equal(TestHelpers.DefaultWaitlistLength, resultData.InPatientWaitlistLength);
            Assert.Equal(TestHelpers.DefaultWaitlistLength, resultData.OutPatientWaitlistLength);
        }

        [Fact]
        public async Task NoWaitlist()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<MentalHealthData>(this.api, organization.Id);
            var data = await TestHelpers.CreateMentalHealthData(this.api, service.Id, true, false, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateMentalHealthDialog.Name, user)
                .Test("test", Phrases.Capacity.MentalHealth.GetInPatientOpen)
                .Test("0", Phrases.Capacity.MentalHealth.GetOutPatientOpen)
                .Send("0")
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<MentalHealthData>(this.userToken, true);
            Assert.Equal(0, resultData.InPatientOpen);
            Assert.Equal(0, resultData.OutPatientOpen);
        }
    }
}