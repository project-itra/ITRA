﻿using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Shared.ApiInterface;
using System.Diagnostics;

namespace Shared
{
    public static class Helpers
    {
        /// <summary>
        /// Gets a user token from the turn context.
        /// This will vary based on the channel the message is rec.
        /// </summary>
        public static string GetUserToken(ITurnContext turnContext)
        {
            switch (turnContext.Activity.ChannelId)
            {
                case Channels.Emulator: return turnContext.Activity.From.Id;
                case Channels.Sms: return PhoneNumber.Standardize(turnContext.Activity.From.Id);
                default: Debug.Fail("Missing channel type"); return string.Empty;
            }
        }

        /// <summary>
        /// Gets the service type for a service model.
        /// </summary>
        public static ServiceType GetServiceType<T>() where T : ServiceDataBase
        {
            var type = typeof(T);

            if (type == typeof(CaseManagementData))
            {
                return ServiceType.CaseManagement;
            }
            if (type == typeof(HousingData))
            {
                return ServiceType.Housing;
            }
            else if (type == typeof(EmploymentData))
            {
                return ServiceType.Employment;
            }
            else if (type == typeof(MentalHealthData))
            {
                return ServiceType.MentalHealth;
            }
            else if (type == typeof(SubstanceUseData))
            {
                return ServiceType.SubstanceUse;
            }

            Debug.Assert(false, "Service type not recognized");
            return ServiceType.Invalid;
        }

        /// <summary>
        /// Gets the table name for a service.
        /// </summary>
        public static string GetServiceTableName(ServiceType serviceType)
        {
            switch (serviceType)
            {
                case ServiceType.CaseManagement: return CaseManagementData.TABLE_NAME;
                case ServiceType.Housing: return HousingData.TABLE_NAME;
                case ServiceType.Employment: return EmploymentData.TABLE_NAME;
                case ServiceType.MentalHealth: return MentalHealthData.TABLE_NAME;
                case ServiceType.SubstanceUse: return SubstanceUseData.TABLE_NAME;
                default: return string.Empty;
            }
        }

        /// <summary>
        /// Gets the primary key for a service.
        /// </summary>
        public static string GetServicePrimaryKey(ServiceType serviceType)
        {
            switch (serviceType)
            {
                case ServiceType.CaseManagement: return CaseManagementData.PRIMARY_KEY;
                case ServiceType.Housing: return HousingData.PRIMARY_KEY;
                case ServiceType.Employment: return EmploymentData.PRIMARY_KEY;
                case ServiceType.MentalHealth: return MentalHealthData.PRIMARY_KEY;
                case ServiceType.SubstanceUse: return SubstanceUseData.PRIMARY_KEY;
                default: return string.Empty;
            }
        }

        /// <summary>
        /// Gets the name for a service.
        /// </summary>
        public static string GetServiceName(ServiceType serviceType, bool toLower = false)
        {
            var result = string.Empty;

            switch (serviceType)
            {
                case ServiceType.CaseManagement: result = Phrases.Services.CaseManagement.ServiceName; break;
                case ServiceType.Housing: result = Phrases.Services.Housing.ServiceName; break;
                case ServiceType.Employment: result = Phrases.Services.Employment.ServiceName; break;
                case ServiceType.MentalHealth: result = Phrases.Services.MentalHealth.ServiceName; break;
                case ServiceType.SubstanceUse: result = Phrases.Services.SubstanceUse.ServiceName; break;
                default: return string.Empty;
            }

            return toLower ? result.ToLower() : result;
        }
    }
}
