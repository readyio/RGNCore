using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RGN.ImplDependencies.Serialization;
using UnityEngine;

namespace RGN.Impl.Firebase.Serialization
{
    public sealed class Json : IJson
    {
        private readonly JsonSerializerSettings mSerializerSettings = new JsonSerializerSettings()
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            NullValueHandling = NullValueHandling.Ignore,
        };
        private readonly JsonSerializer mSerializer;

        public Json()
        {
            mSerializer = JsonSerializer.Create(mSerializerSettings);
        }

        string IJson.ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        string IJson.ToJson(object obj, bool prettyPrint)
        {
            return JsonUtility.ToJson(obj, prettyPrint);
        }
        
        T IJson.FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, mSerializerSettings);
        }

        T IJson.FromJson<T>(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                return mSerializer.Deserialize<T>(jsonTextReader);
            }
        }
        
        object IJson.FromJson(string json, System.Type type)
        {
            return JsonConvert.DeserializeObject(json, type, mSerializerSettings);
        }
        
        IDictionary<string, object> IJson.FromJsonAsDictionary(string json)
        {
            var settings = new JsonSerializerSettings(mSerializerSettings)
            {
                Converters = { new NestedObjectsAsDictionaryConverter() }
            };
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(json, settings);
        }
        
        IDictionary<string, object> IJson.FromJsonAsDictionary(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                mSerializer.Converters.Add(new NestedObjectsAsDictionaryConverter());
                return mSerializer.Deserialize<Dictionary<string, object>>(jsonTextReader);
            }
        }
    }

    internal class NestedObjectsAsDictionaryConverter : CustomCreationConverter<IDictionary<string, object>>
    {
        public override IDictionary<string, object> Create(System.Type objectType) =>
            new Dictionary<string, object>();

        public override bool CanConvert(System.Type objectType) =>
            objectType == typeof(object) || base.CanConvert(objectType);

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject || reader.TokenType == JsonToken.Null)
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }
            return serializer.Deserialize(reader);
        }
    }
}
