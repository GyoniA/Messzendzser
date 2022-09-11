using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Messzendzser.UnitTests.api
{
    class ApiTestUtils
    {
        public static string RersponseRegex(int responseCode, string responseMessage, Dictionary<string,string> body)
        {
            string format =@"^{""DateTime"":""[\d\-:T+.]*"",""ResponseCode"":"+ responseCode.ToString() + @",""Message"":""" + responseMessage + @"""";
            if(body != null && body.Count > 0)
            {
                string list = JsonSerializer.Serialize(body);

                format += String.Format(",\"Body\":{0}",list);
            }
            format += "}$";
            return format;
        }
    }
}
