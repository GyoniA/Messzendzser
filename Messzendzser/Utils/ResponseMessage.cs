namespace Messzendzser.Utils
{
    public class ResponseMessage
    {
        public DateTime DateTime { get; }
        public int ResponseCode { get; }
        public string Message { get; }

        public KeyValuePair<string, string>[]? Body { get; }

        public string ToJson() {
            String response = DateTime.ToString() + " " + ResponseCode.ToString() + " " + Message;
            if(Body != null)
            {
                foreach(KeyValuePair<string, string> pair in Body)
                {
                    response += "\n"+pair.Key+": "+pair.Value;
                }
            }
            return response;
        }
        public ResponseMessage(int responseCode, string responseMessage)
        {
            DateTime = DateTime.Now;
            ResponseCode = responseCode;
            Message = responseMessage;
        }

        public ResponseMessage(int responseCode, string responseMessage,KeyValuePair<string,string>[]? body)
        {
            DateTime = DateTime.Now;
            ResponseCode = responseCode;
            Message = responseMessage;
            Body = body;
        }

        public static ResponseMessage CreateOkMessage() {
            return CreateOkMessage(null);
        }
        public static ResponseMessage CreateOkMessage(KeyValuePair<string, string>[]? body)
        {
            return new ResponseMessage(200, "Ok", body);
        }
        public static ResponseMessage CreateErrorMessage(int errorCode,string errorMessage)
        {
            return CreateErrorMessage(errorCode, errorMessage,null);
        }
        public static ResponseMessage CreateErrorMessage(int errorCode, string errorMessage, KeyValuePair<string, string>[]? body)
        {
            return new ResponseMessage(errorCode, errorMessage, body);
        }
    }
}
