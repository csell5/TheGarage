using System;
using System.Net;
using Newtonsoft.Json;

namespace GarageTestConsole.Security
{
    public class DeviceHubIdentity
    {
        public DeviceHubIdentity(NetworkCredential credential, string connectionId)
        {
            Credential = credential;
            ConnectionId = connectionId;
        }

        public NetworkCredential Credential { get; private set; }
        public string ConnectionId { get; private set; }

        public string EncryptAndEncode(byte[] key, byte[] initializationVector)
        {
            var identityToEncrypt = JsonConvert.SerializeObject(this);

            var encrypted = DeviceCommunicationSecurity.EncryptStringToBytes(identityToEncrypt, key, initializationVector);
            var identityPayload = Convert.ToBase64String(encrypted);

            return identityPayload;
        }

        public static DeviceHubIdentity Decrypt(string encodedIdentity, byte[] key, byte[] initializationVector)
        {
            var base64IdentityBytes = Convert.FromBase64String(encodedIdentity);

            var identityString = DeviceCommunicationSecurity.DecryptStringFromBytes(base64IdentityBytes, key, initializationVector);

            var identity = JsonConvert.DeserializeObject<DeviceHubIdentity>(identityString);

            return identity;
        }
    }
}