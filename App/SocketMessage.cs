using System.Security.Cryptography;
using System.Text;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebCommander.App
{

    public class SocketMessage
    {
        private string type;
        private string message = String.Empty;   //encoded Message allways
        private string destination;
        private string origin;
        private string integrity = String.Empty;
        private string correlationID = String.Empty;
        
        [JsonPropertyName("type")]
        public string Type { get => type; set => type = value; }

        [JsonPropertyName("message")]
        public string Message { get => message; set => message = value; }
        [JsonPropertyName("origin")]
        public string Origin { get => origin; set => origin = value; }
        [JsonPropertyName("integrity")]
        public string Integrity { get => integrity; set => integrity = value; }
        [JsonPropertyName("correlationID")]
        public string CorrelationID { get => correlationID; set => correlationID = value; }
        [JsonPropertyName("destination")]
        public string Destination { get => destination; set => destination = value; }

        public SocketMessage(string type, string destination, string origin)
        {
            this.type = type;
            this.Destination = destination;
            this.origin = origin;
        }

        public SocketMessage SetDestination(string destination)
        {
            this.Destination = destination;
            return this;
        }

        /// <summary>
        /// Create a JSON string.
        /// </summary>
        public string PreparePacket()
        {
            if (SocketMessageInit.HashPresent)
            {
                if (!string.IsNullOrWhiteSpace(message))
                {
                    

                    integrity = EncryptMessage(SocketMessageInit.SecretKeyHash, message);
                    correlationID = GenerateHashKeyFromTime((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
                    return JsonSerializer.Serialize(this, WebCommanderGenerationContext.Default.SocketMessage);
                }
                else
                {
                    throw new Exception("Message Failed to Valid text.");
                }
            }
            else
            {
                throw new Exception("Secret Hash is Not Present.");
            }


        }

        public static SocketMessage ReadPacket(string jsonRespMsg){
            return JsonSerializer.Deserialize<SocketMessage>(jsonRespMsg, WebCommanderGenerationContext.Default.SocketMessage)!;
           
        }

        /// <summary>
        ///  Returns a UTF8 string for Base64 encoded string.
        /// </summary>
        public static string decodeMessage(string encodedMessage){
            return Encoding.UTF8.GetString(Convert.FromBase64String(encodedMessage));
           
        }
         /// <summary>
        /// Returns a Base64 encoded equivalent to Input string.
        /// </summary>
        public static string encodeMessage(string stringMessage){
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(stringMessage));
           
        }

        private string EncryptMessage(string secretKeyHash, string encodedMessage)
        {
            string combinedStr = secretKeyHash + encodedMessage;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedStr));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < 6; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private string GenerateHashKeyFromTime(int utcTimeInSeconds)
        {
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int characterLength = characters.Length;
            StringBuilder hashKey = new StringBuilder();
            int remainingTime = utcTimeInSeconds;

            while (hashKey.Length < 16)
            {
                int index = Math.Clamp(Math.Abs( remainingTime % characterLength), 0, characterLength-1);
                //Console.WriteLine(index);
                hashKey.Append(characters[index]);
                remainingTime = (int)Math.Ceiling((remainingTime / (double)characterLength) * new Random().NextDouble() * 100);
            }

            return hashKey.ToString();
        }
    }



}
