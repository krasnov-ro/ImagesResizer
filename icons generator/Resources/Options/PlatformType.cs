using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace icons_generator.Resources.Options
{
    public class PlatformType
    {
        [JsonPropertyName("android")]
        public ImageSizes Android { get; set; }
        [JsonPropertyName("iOS")]
        public ImageSizes iOS { get; set; }
    }
}
