using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.NewOrganization;
using ServiceProviderBot.Bot.Models.OrganizationProfile;
using ServiceProviderBot.Bot.Utils;
using Xunit;

namespace Tests.Dialogs.NewOrganization
{
    public class NewOrganizationDialogTests : DialogTestBase
    {
        [Fact]
        public async Task YesToAll()
        {
            var expected = CreateDefaultTestProfile();
            expected.Demographic.AgeRange.Start = 14;
            expected.Demographic.AgeRange.End = 24;
            expected.Capacity.Beds.Total = 10;
            expected.Capacity.Beds.Open = 5;

            // Execute the conversation.
            await CreateTestFlow(NewOrganizationDialog.Name)
                .Test("begin", Phrases.NewOrganization.GetName)
                .Test(expected.Name, Phrases.Location.GetLocation)
                .Test(expected.Location.Zip, StartsWith(Phrases.Demographic.GetHasDemographic))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicMen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicWomen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicAgeRange))
                .Test("yes", Phrases.AgeRange.GetAgeRangeStart)
                .Test(expected.Demographic.AgeRange.Start.ToString(), Phrases.AgeRange.GetAgeRangeEnd)
                .Test(expected.Demographic.AgeRange.End.ToString(), StartsWith(Phrases.Capacity.GetHasHousing))
                .Test("yes", Phrases.Capacity.GetHousingTotal)
                .Test(expected.Capacity.Beds.Total.ToString(), Phrases.Capacity.GetHousingOpen)
                .Test(expected.Capacity.Beds.Open.ToString(), Phrases.NewOrganization.Closing)
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expected);
        }

        [Fact]
        public async Task NoToAll()
        {
            var expected = CreateDefaultTestProfile();

            // Execute the conversation.
            await CreateTestFlow(NewOrganizationDialog.Name)
                .Test("begin", Phrases.NewOrganization.GetName)
                .Test(expected.Name, Phrases.Location.GetLocation)
                .Test(expected.Location.Zip, StartsWith(Phrases.Demographic.GetHasDemographic))
                .Test("no", StartsWith(Phrases.Capacity.GetHasHousing))
                .Test("no", Phrases.NewOrganization.Closing)
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expected);
        }

        [Fact]
        public async Task NoDemographic()
        {
            var expected = CreateDefaultTestProfile();
            expected.Capacity.Beds.Total = 10;
            expected.Capacity.Beds.Open = 5;

            await CreateTestFlow(NewOrganizationDialog.Name)
                .Test("begin", Phrases.NewOrganization.GetName)
                .Test(expected.Name, Phrases.Location.GetLocation)
                .Test(expected.Location.Zip, StartsWith(Phrases.Demographic.GetHasDemographic))
                .Test("no", StartsWith(Phrases.Capacity.GetHasHousing))
                .Test("yes", Phrases.Capacity.GetHousingTotal)
                .Test(expected.Capacity.Beds.Total.ToString(), Phrases.Capacity.GetHousingOpen)
                .Test(expected.Capacity.Beds.Open.ToString(), Phrases.NewOrganization.Closing)
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expected);
        }

        [Fact]
        public async Task NoAgeRange()
        {
            var expected = CreateDefaultTestProfile();
            expected.Capacity.Beds.Total = 10;
            expected.Capacity.Beds.Open = 5;

            // Execute the conversation.
            await CreateTestFlow(NewOrganizationDialog.Name)
                .Test("begin", Phrases.NewOrganization.GetName)
                .Test(expected.Name, Phrases.Location.GetLocation)
                .Test(expected.Location.Zip, StartsWith(Phrases.Demographic.GetHasDemographic))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicMen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicWomen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicAgeRange))
                .Test("no", StartsWith(Phrases.Capacity.GetHasHousing))
                .Test("yes", Phrases.Capacity.GetHousingTotal)
                .Test(expected.Capacity.Beds.Total.ToString(), Phrases.Capacity.GetHousingOpen)
                .Test(expected.Capacity.Beds.Open.ToString(), Phrases.NewOrganization.Closing)
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expected);
        }

        [Fact]
        public async Task NoHousing()
        {
            var expected = CreateDefaultTestProfile();
            expected.Demographic.AgeRange.Start = 14;
            expected.Demographic.AgeRange.End = 24;

            // Execute the conversation.
            await CreateTestFlow(NewOrganizationDialog.Name)
                .Test("begin", Phrases.NewOrganization.GetName)
                .Test(expected.Name, Phrases.Location.GetLocation)
                .Test(expected.Location.Zip, StartsWith(Phrases.Demographic.GetHasDemographic))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicMen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicWomen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicAgeRange))
                .Test("yes", Phrases.AgeRange.GetAgeRangeStart)
                .Test(expected.Demographic.AgeRange.Start.ToString(), Phrases.AgeRange.GetAgeRangeEnd)
                .Test(expected.Demographic.AgeRange.End.ToString(), StartsWith(Phrases.Capacity.GetHasHousing))
                .Test("no", Phrases.NewOrganization.Closing)
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expected);
        }
    }
}
