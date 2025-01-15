using Newtonsoft.Json;

namespace SatTrack.Satellites.DTO
{
    namespace SatTrack.DTO
    {
        public class SatDTO
        {
            [JsonProperty("OBJECT_NAME")]
            public string? ObjectName { get; set; }

            [JsonProperty("OBJECT_ID")]
            public string? ObjectId { get; set; }

            [JsonProperty("EPOCH")]
            public DateTime? Epoch { get; set; }

            [JsonProperty("MEAN_MOTION")]
            public decimal? MeanMotion { get; set; }

            [JsonProperty("ECCENTRICITY")]
            public decimal? Eccentricity { get; set; }

            [JsonProperty("INCLINATION")]
            public decimal? Inclination { get; set; }

            [JsonProperty("RA_OF_ASC_NODE")]
            public decimal? RaOfAscNode { get; set; }

            [JsonProperty("ARG_OF_PERICENTER")]
            public decimal? ArgOfPericenter { get; set; }

            [JsonProperty("MEAN_ANOMALY")]
            public decimal? MeanAnomaly { get; set; }

            [JsonProperty("EPHEMERIS_TYPE")]
            public int? EphemerisType { get; set; }

            [JsonProperty("CLASSIFICATION_TYPE")]
            public string? ClassificationType { get; set; }

            [JsonProperty("NORAD_CAT_ID")]
            public int? NoradCatId { get; set; }

            [JsonProperty("ELEMENT_SET_NO")]
            public int? ElementSetNo { get; set; }

            [JsonProperty("REV_AT_EPOCH")]
            public int? RevAtEpoch { get; set; }

            [JsonProperty("BSTAR")]
            public decimal? Bstar { get; set; }

            [JsonProperty("MEAN_MOTION_DOT")]
            public decimal? MeanMotionDot { get; set; }

            [JsonProperty("MEAN_MOTION_DDOT")]
            public decimal? MeanMotionDdot { get; set; }

            public int SatId { get; set; }
            public int SatGroupID { get; set; }
            public List<int>? SatGroupIds { get; set; }
        }
    }

}
