using System.Text.Json.Serialization;

namespace Messzendzser.Utils
{
    public class ResponseMessage<T>
    {        
        public DateTime DateTime { get; }
        public int ResponseCode { get; }
        public string Message { get; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string>? Errors { get; private set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Body { get; }

        public ResponseMessage(int responseCode, string responseMessage)
        {
            DateTime = DateTime.Now;
            ResponseCode = responseCode;
            Message = responseMessage;
        }

        public ResponseMessage(int responseCode, string responseMessage,T? body)
        {
            DateTime = DateTime.Now;
            ResponseCode = responseCode;
            Message = responseMessage;
            Body = body;
        }

        public static ResponseMessage<T> CreateOkMessage() {
            return CreateOkMessage(default(T));
        }
        public static ResponseMessage<T> CreateOkMessage(T? body)
        {
            return new ResponseMessage<T>(200, "Ok", body);
        }
        public static ResponseMessage<T> CreateErrorMessage(int errorCode,string errorMessage)
        {
            return CreateErrorMessage(errorCode, errorMessage, null);
        }
        public static ResponseMessage<T> CreateErrorMessage(int errorCode, string errorMessage, Dictionary<string, string> errors)
        {
            return new ResponseMessage<T>(errorCode, errorMessage, default(T)) { Errors = errors };
        }
    }
}
