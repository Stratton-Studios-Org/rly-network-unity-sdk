using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

using UnityEngine;

namespace RallyProtocol.GSN
{

    public class OldRelayData
    {

        [JsonProperty("maxFeePerGas")]
        public string MaxFeePerGas;

        [JsonProperty("maxPriorityFeePerGas")]
        public string MaxPriorityFeePerGas;

        [JsonProperty("transactionCalldataGasUsed")]
        public string TransactionCalldataGasUsed;

        [JsonProperty("relayWorker")]
        public string RelayWorker;

        [JsonProperty("paymaster")]
        public string Paymaster;

        [JsonProperty("paymasterData")]
        public string PaymasterData;

        [JsonProperty("clientId")]
        public string ClientId;

        [JsonProperty("forwarder")]
        public string Forwarder;

        public OldRelayData Clone()
        {
            return new()
            {
                MaxFeePerGas = MaxFeePerGas,
                MaxPriorityFeePerGas = MaxPriorityFeePerGas,
                TransactionCalldataGasUsed = TransactionCalldataGasUsed,
                RelayWorker = RelayWorker,
                Paymaster = Paymaster,
                PaymasterData = PaymasterData,
                ClientId = ClientId,
                Forwarder = Forwarder,
            };
        }


    }

}