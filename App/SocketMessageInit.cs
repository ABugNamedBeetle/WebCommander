using System;
using System.Text;

namespace WebCommander.App
{
    public class SocketMessageInit
    {
        public static string SecretKeyHash { get; private set; } = "";
        public static bool HashPresent { get; private set; } = false;

        public static void InitSecretKeyHash(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                SecretKeyHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
                HashPresent = true;
            }
        }
    }

    public static class MessageType
    {
       public static string HEALTH="health";
       public static string HEALTHRESPONSE="healthresponse";
       public static string REQUEST="request";
       public static string BROADCAST="broadcast";
       public static string RESPONSE="response";
    }


}
