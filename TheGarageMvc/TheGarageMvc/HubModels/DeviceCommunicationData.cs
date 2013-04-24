using System;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace TheGarageMvc.HubModels
{

    public class FromServerToClientData
    {
        public DateTime Now { get; set; }
        public int Integer { get; set; }
        public string Text { get; set; }
        public string Command { get; set; }
    }

    public class FromClientToServerData
    {
        public string Text { get; set; }
        public string Command { get; set; }
    }

    public class GarageComponentCommand
    {
        public string component { get; set; }
        public int componentNumber { get; set; }
        public string command { get; set; }
    }


    public enum RequestType
    {
        SignOn = 0,
        Info = 1,
        Get = 2,
        Set = 3
    }

    public class Response
    {
        public int Version { get; set; }
        public string Payload { get; set; }
    }

    /// <summary>
    /// Server to Client Communication (Requesting Data)
    /// </summary>
    public class Request
    {
        public int Version { get; set; }
        public RequestType MessageType { get; set; }
        public string Payload { get; set; }
    }

    public class SignOnRequest
    {
        public string LocationId { get; set; }
        public string Identity { get; set; }
    }

    public class DeviceHubIdentity
    {
        public DeviceHubIdentity(NetworkCredential credential, string connectionId)
        {
            Credential = credential;
            ConnectionId = connectionId;
        }

        public NetworkCredential Credential { get; private set; }
        public string ConnectionId { get; private set; }

        public string Base64Encoded()
        {
            var thisSerialized = JsonConvert.SerializeObject(this);

            return (Convert.ToBase64String(Encoding.UTF8.GetBytes(thisSerialized)));
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