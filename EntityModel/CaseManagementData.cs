﻿using EntityModel.Luis;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace EntityModel
{
    public class CaseManagementData : ServiceData
    {
        public const string TABLE_NAME = "TODO";
        public const string PRIMARY_KEY = "TODO";
        public const string SERVICE_NAME = "Case Management";

        [JsonProperty(PropertyName = "TODO")]
        public int Total { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int Open { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool HasWaitlist { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool WaitlistIsOpen { get; set; }

        public override IContractResolver ContractResolver() { return Resolver.Instance; }
        public override string TableName() { return TABLE_NAME; }
        public override string PrimaryKey() { return PRIMARY_KEY; }
        public override ServiceType ServiceType() { return EntityModel.ServiceType.CaseManagement; }
        public override string ServiceTypeName() { return SERVICE_NAME; }
        public override List<SubService> SubServices()
        {
            return new List<SubService>()
            {
                new SubService()
                {
                    Name = SERVICE_NAME,
                    ServiceFlag = ServiceFlags.CaseManagement,
                    LuisEntityNames = new List<string>() { nameof(LuisModel.Entities.CaseManangement) },

                    TotalPropertyName = nameof(this.Total),
                    OpenPropertyName = nameof(this.Open),
                    HasWaitlistPropertyName = nameof(this.HasWaitlist),
                    WaitlistIsOpenPropertyName = nameof(this.WaitlistIsOpen)
                }
            };
        }

        public override void CopyStaticValues<T>(T data)
        {
            var d = data as CaseManagementData;

            this.Total = d.Total;
            this.HasWaitlist = d.HasWaitlist;

            base.CopyStaticValues(data);
        }

        public class Resolver : ContractResolver<CaseManagementData>
        {
            public static Resolver Instance = new Resolver();

            private Resolver()
            {
                AddMap(x => x.Id, "TODO");
                AddMap(x => x.ServiceId, "TODO");
            }
        }
    }
}
