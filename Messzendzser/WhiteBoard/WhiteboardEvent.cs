using Org.BouncyCastle.Crypto.Paddings;
using System.Text.Json;
using SkiaSharp;
using System.Formats.Asn1;
using System.Text.Json.Serialization;
using TinyJson;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using Newtonsoft.Json.Converters;

namespace Messzendzser.WhiteBoard
{/*
    public class WhiteboardEventSpecifiedConcreteClassConverter : DefaultContractResolver
    {
        protected override Newtonsoft.Json.JsonConverter ResolveContractConverter(Type objectType)
        {
            if (typeof(WhiteboardEvent).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null; // pretend TableSortRuleConvert is not specified (thus avoiding a stack overflow)
            return base.ResolveContractConverter(objectType);
        }
    }

    public class WhiteboardEventConverter : Newtonsoft.Json.JsonConverter
    {
        static JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new WhiteboardEventSpecifiedConcreteClassConverter() };

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(WhiteboardEvent));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            switch (jo["ObjType"].Value<EventType>())
            {
                case EventType.Image:
                    return JsonConvert.DeserializeObject<WhiteboardImageEvent>(jo.ToString(), SpecifiedSubclassConversion);
                case EventType.Dot:
                    return JsonConvert.DeserializeObject<WhiteboardDotEvent>(jo.ToString(), SpecifiedSubclassConversion);
                case EventType.Line:
                    return JsonConvert.DeserializeObject<WhiteboardLineEvent>(jo.ToString(), SpecifiedSubclassConversion);
                default:
                    throw new Exception();
            }
            throw new NotImplementedException();
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            throw new NotImplementedException(); // won't be called because CanWrite returns false
        }
    }*/
    public struct Point
    {
        public float X { get; set; }
        public float Y { get; set; }
        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
    public class WhiteboardEventConverter : CustomCreationConverter<WhiteboardEvent>
    {
        private EventType _currentObjectType;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var jobj = JObject.ReadFrom(reader);
            _currentObjectType = jobj["Type"].ToObject<EventType>();
            return base.ReadJson(jobj.CreateReader(), objectType, existingValue, serializer);
        }

        public override WhiteboardEvent Create(Type objectType)
        {
            switch (_currentObjectType)
            {
                case EventType.Image:
                    return new WhiteboardImageEvent();
                case EventType.Dot:
                    return new WhiteboardDotEvent();
                case EventType.Line:
                    return new WhiteboardLineEvent();
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public enum EventType
    {
        Image = 0,
        Dot = 1,
        Line = 2
    }
    [Newtonsoft.Json.JsonConverter(typeof(WhiteboardEventConverter))]
    public abstract class WhiteboardEvent
    {
        public EventType Type { get; set; }
        public abstract SKCanvas Draw(SKCanvas canvas);

        public WhiteboardEvent(EventType type)
        {
            Type = type;
        }


        public static WhiteboardEvent GetEventFromType(EventType type)
        {
            switch (type)
            {
                case EventType.Image:
                    return new WhiteboardImageEvent();
                case EventType.Dot:
                    return new WhiteboardDotEvent();
                case EventType.Line:
                    return new WhiteboardLineEvent();
                default:
                    return null;
            }
        }

        public string Serialize() {
            //TODO check if correct
            string json = System.Text.Json.JsonSerializer.Serialize(this, this.GetType());
            return json;
        }

        public abstract WhiteboardEvent Deserialize(string data);
    }
}
