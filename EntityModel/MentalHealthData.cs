﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EntityModel
{
    public class MentalHealthData : ServiceModelBase
    {
        public static string TABLE_NAME = "tira_substanceuses";
        public static string PRIMARY_KEY = "TODO";

        [JsonIgnore]
        public override string TableName { get { return TABLE_NAME; } }

        [JsonIgnore]
        public override IContractResolver ContractResolver { get { return Resolver.Instance; } }


        [JsonProperty(PropertyName = "TODO")]
        public int InPatientTotal { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int InPatientOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int InPatientWaitlistLength { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int OutPatientTotal { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int OutPatientOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int OutPatientWaitlistLength { get; set; }

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
