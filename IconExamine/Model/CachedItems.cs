using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FontExamine.Model
{
    internal class CachedItems
    {
        public dynamic Dynamic { get; set; }=new System.Dynamic.ExpandoObject();
        internal JsonSerializerOptions DefaultJsonSerializerOptions= new JsonSerializerOptions() { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, Converters = { new JsonStringEnumConverter() } };
    }
}
