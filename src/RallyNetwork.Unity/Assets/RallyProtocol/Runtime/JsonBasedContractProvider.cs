using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace RallyProtocol.Contracts
{

    public class JsonBasedContractProvider
    {

        public const string TokenFaucetFileName = "token-faucet";
        public const string Erc20FileName = "erc20";
        public const string ForwarderFileName = "forwarder";
        public const string RelayHubFileName = "relay-hub";

        public static string LoadFileContent(string fileName)
        {
            TextAsset asset = Resources.Load<TextAsset>(fileName);
            return asset.text;
        }

        public static string LoadTokenFaucet()
        {
            return LoadFileContent(TokenFaucetFileName);
        }

        public static string LoadErc20()
        {
            return LoadFileContent(Erc20FileName);
        }

        public static string LoadForwarder()
        {
            return LoadFileContent(ForwarderFileName);
        }

        public static string LoadRelayHub()
        {
            return LoadFileContent(RelayHubFileName);
        }

    }

}