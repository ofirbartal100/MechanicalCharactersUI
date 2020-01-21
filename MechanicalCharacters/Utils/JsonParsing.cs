using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MechanicalCharacters.Utils
{
    public class JsonConfigurationForArticulatedCharacter
    {
        public List<double> Degrees { get; set; }
    }

    public class JsonForArticulatedCharacter
    {
        public Point AnchorPoint { get; set; }
        public List<Point> DegreesAndLengths { get; set; }
    }
    public class JsonForAssemblies
    {
        public List<double> Degrees { get; set; }
    }


    public class JsonAssembly
    {
        [JsonConverter(typeof(ParseObjectConverter))]
        public List<ParseObject> Components { get; set; }
    }


    public class ParseObjectConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return string.Empty;
            }
            else if (reader.TokenType == JsonToken.String)
            {
                return serializer.Deserialize(reader, objectType);
            }
            else
            {
                JObject obj = JObject.Load(reader);
                if (obj["components"] != null)
                    return new ParseObject()
                    {
                        id = JsonConvert.DeserializeObject<int>(obj["id"].ToString()),
                        length = JsonConvert.DeserializeObject<double>(obj["length"].ToString()),
                        orientation = JsonConvert.DeserializeObject<List<double>>(obj["orientation"].ToString()),
                        position = JsonConvert.DeserializeObject<List<double>>(obj["position"].ToString())
                    };
                else
                    return serializer.Deserialize(reader, objectType);
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return false;
        }
    }

    public class JsonAssemblyConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return string.Empty;
            }
            else if (reader.TokenType == JsonToken.String)
            {
                return serializer.Deserialize(reader, objectType);
            }
            else
            {
                JObject obj = JObject.Load(reader);
                if (obj["components"] != null)
                    return new JsonAssembly()
                    {
                        Components = JsonConvert.DeserializeObject<List<ParseObject>>(obj["components"].ToString())
                    };
                else
                    return serializer.Deserialize(reader, objectType);
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return false;
        }
    }

    public class JsonCurve
    {
        public List<Point> Points { get; set; }
        public List<double> Feature { get; set; }
    }
    public class JsonCurveConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return string.Empty;
            }
            else if (reader.TokenType == JsonToken.String)
            {
                return serializer.Deserialize(reader, objectType);
            }
            else
            {
                JObject obj = JObject.Load(reader);
                if (obj["points"] != null && obj["features"] != null)
                    return new JsonCurve()
                    {
                        Feature = JsonConvert.DeserializeObject<List<double>>(obj["features"].ToString()),
                        Points = JsonConvert.DeserializeObject<List<List<double>>>(obj["points"].ToString())
                        .Select(list => new Point(list[0],list[1])).ToList(),
                    };
                else
                    return serializer.Deserialize(reader, objectType);
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return false;
        }
    }


    public class JsonOfCurveAndAssembly
    {
        [JsonConverter(typeof(JsonCurveConverter))]
        public JsonCurve Curve { get; set; }
        [JsonConverter(typeof(JsonAssemblyConverter))]
        public JsonAssembly Assembly { get; set; }
    }
}
