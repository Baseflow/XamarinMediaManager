using System;
using System.Collections.Generic;

using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ElementPlayer.Core.Assets
{
    public partial class ExoPlayerSamples
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("samples")]
        public List<Sample> Samples { get; set; }
    }

    public partial class Sample
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("uri", NullValueHandling = NullValueHandling.Ignore)]
        public string Uri { get; set; }

        [JsonProperty("extension", NullValueHandling = NullValueHandling.Ignore)]
        public string Extension { get; set; }

        [JsonProperty("drm_scheme", NullValueHandling = NullValueHandling.Ignore)]
        public DrmScheme? DrmScheme { get; set; }

        [JsonProperty("drm_license_url", NullValueHandling = NullValueHandling.Ignore)]
        public string DrmLicenseUrl { get; set; }

        [JsonProperty("playlist", NullValueHandling = NullValueHandling.Ignore)]
        public List<Playlist> Playlist { get; set; }

        [JsonProperty("ad_tag_uri", NullValueHandling = NullValueHandling.Ignore)]
        public string AdTagUri { get; set; }

        [JsonProperty("spherical_stereo_mode", NullValueHandling = NullValueHandling.Ignore)]
        public string SphericalStereoMode { get; set; }
        
        [JsonProperty("mimetype", NullValueHandling = NullValueHandling.Ignore)]
        public string MimeType { get; set; }
    }

    public partial class Playlist
    {
        [JsonProperty("uri")]
        public string Uri { get; set; }
    }

    public enum DrmScheme { Playready, Widevine };

    public partial class ExoPlayerSamples
    {
        public static Stream GetEmbeddedResourceStream(Assembly assembly, string resourceFileName)
        {
            var resourceNames = assembly.GetManifestResourceNames();

            var resourcePaths = resourceNames
                .Where(x => x.EndsWith(resourceFileName, StringComparison.CurrentCultureIgnoreCase))
                .ToArray();

            if (!resourcePaths.Any())
            {
                throw new Exception(string.Format("Resource ending with {0} not found.", resourceFileName));
            }

            if (resourcePaths.Count() > 1)
            {
                throw new Exception(string.Format("Multiple resources ending with {0} found: {1}{2}", resourceFileName, Environment.NewLine, string.Join(Environment.NewLine, resourcePaths)));
            }

            return assembly.GetManifestResourceStream(resourcePaths.Single());
        }

        public static string GetEmbeddedResourceString(Assembly assembly, string resourceFileName)
        {
            var stream = GetEmbeddedResourceStream(assembly, resourceFileName);

            using (var streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }

        public static string GetEmbeddedResourceString(string resourceFileName)
        {
            return GetEmbeddedResourceString(Assembly.GetCallingAssembly(), resourceFileName);
        }

        public static List<ExoPlayerSamples> FromJson(string json) => JsonConvert.DeserializeObject<List<ExoPlayerSamples>>(json);
    }

    public static class Serialize
    {
        public static string ToJson(this List<ExoPlayerSamples> self) => JsonConvert.SerializeObject(self);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                DrmSchemeConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class DrmSchemeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(DrmScheme) || t == typeof(DrmScheme?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "playready":
                    return DrmScheme.Playready;
                case "widevine":
                    return DrmScheme.Widevine;
            }
            throw new Exception("Cannot unmarshal type DrmScheme");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (DrmScheme)untypedValue;
            switch (value)
            {
                case DrmScheme.Playready:
                    serializer.Serialize(writer, "playready");
                    return;
                case DrmScheme.Widevine:
                    serializer.Serialize(writer, "widevine");
                    return;
            }
            throw new Exception("Cannot marshal type DrmScheme");
        }

        public static readonly DrmSchemeConverter Singleton = new DrmSchemeConverter();
    }
}
