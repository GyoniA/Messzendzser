using Messzendzser.Model.DB;
using Messzendzser.Model.DB.Models;
using SIPSorcery.SIP;
using System.Text;

namespace Messzendzser.Voip
{
    public class VoipCredentialManager
    {
        private static VoipCredential GetVoipCredential(string username)
        {
            IDataSource dataSource = new MySQLDbConnection();

            VoipCredential  cred = dataSource.GetCredentialsForUser(username);
            return cred;
        }

        private static string MD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
        public static bool ValidateDigestResponse(SIPAuthenticationHeader header,string method)
        {
            if (header == null)
                throw new ArgumentNullException();
            string a1;
            try
            {
                 a1 = GetVoipCredential(header.SIPDigest.Username).VoipRealmHash;
            }
            catch (ArgumentException) { // user not found
                return false;
            }
            string a2 = MD5($"{method}:{header.SIPDigest.URI}");
            string response = MD5($"{a1}:{header.SIPDigest.Nonce}:{a2}");
            if (response == header.SIPDigest.Response)
                return true;
            return false;
        }
    }
}
