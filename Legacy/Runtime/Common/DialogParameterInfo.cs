using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Sandbox.Dialogue
{
    [DataContract]
    public class DialogParameterInfo
    {
        [DataMember] [JsonConverter(typeof(StringEnumConverter))]
        public DialogParameterType ParameterType;

        [DataMember] public object Type;
        [DataMember] public List<object> Arguments = new();
    }
}