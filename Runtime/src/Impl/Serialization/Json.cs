using System.IO;
using Newtonsoft.Json;
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

        T IJson.FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, mSerializerSettings);
        }

        object IJson.FromJson(string json, System.Type type)
        {
            return JsonConvert.DeserializeObject(json, type, mSerializerSettings);
        }

        string IJson.ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        string IJson.ToJson(object obj, bool prettyPrint)
        {
            return JsonUtility.ToJson(obj, prettyPrint);
        }

        public T FromJson<T>(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            {
                using (var jsonTextReader = new JsonTextReader(streamReader))
                {
                    return mSerializer.Deserialize<T>(jsonTextReader);
                }
            }
        }
    }
}
