using System.Text.Json.Serialization;

namespace Messzendzser.Utils
{
    public class ResponseMessage
    {        
        public DateTime DateTime { get; }
        public int ResponseCode { get; }
        public string Message { get; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string>? Body { get; }

        public ResponseMessage(int responseCode, string responseMessage)
        {
            DateTime = DateTime.Now;
            ResponseCode = responseCode;
            Message = responseMessage;
        }

        public ResponseMessage(int responseCode, string responseMessage,Dictionary<string,string>? body)
        {
            DateTime = DateTime.Now;
            ResponseCode = responseCode;
            Message = responseMessage;
            Body = body;
        }

        public static ResponseMessage CreateOkMessage() {
            return CreateOkMessage(null);
        }
        public static ResponseMessage CreateOkMessage(Dictionary<string, string>? body)
        {
            return new ResponseMessage(200, "Ok", body);
        }
        public static ResponseMessage CreateErrorMessage(int errorCode,string errorMessage)
        {
            return CreateErrorMessage(errorCode, errorMessage,null);
        }
        public static ResponseMessage CreateErrorMessage(int errorCode, string errorMessage, Dictionary<string, string>? body)
        {
            return new ResponseMessage(errorCode, errorMessage, body);
        }
    }
}
