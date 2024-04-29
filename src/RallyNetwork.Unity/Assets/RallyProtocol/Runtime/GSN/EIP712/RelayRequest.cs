using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using Newtonsoft.Json;

using RallyProtocol.GSN.Contracts;

using UnityEngine;

namespace RallyProtocol.GSN
{

    public class GsnForwardRequest
    {

        [JsonProperty("from")]
        public virtual string From { get; set; }

        [JsonProperty("to")]
        public virtual string To { get; set; }

        [JsonProperty("value")]
        public virtual string Value { get; set; }

        [JsonProperty("gas")]
        public virtual string Gas { get; set; }

        [JsonProperty("nonce")]
        public virtual string Nonce { get; set; }

        [JsonProperty("data")]
        public virtual string Data { get; set; }

        [JsonProperty("validUntilTime")]
        public virtual string ValidUntilTime { get; set; }

        public GsnForwardRequest Clone()
        {
            return new()
            {
                From = From,
                To = To,
                Value = Value,
                Gas = Gas,
                Nonce = Nonce,
                Data = Data,
                ValidUntilTime = ValidUntilTime
            };
        }

    }

    public class GsnRelayData
    {

        [JsonProperty("maxFeePerGas")]
        public virtual string MaxFeePerGas { get; set; }

        [JsonProperty("maxPriorityFeePerGas")]
        public virtual string MaxPriorityFeePerGas { get; set; }

        [JsonProperty("transactionCalldataGasUsed")]
        public virtual string TransactionCalldataGasUsed { get; set; }

        [JsonProperty("relayWorker")]
        public virtual string RelayWorker { get; set; }

        [JsonProperty("paymaster")]
        public virtual string Paymaster { get; set; }

        [JsonProperty("forwarder")]
        public virtual string Forwarder { get; set; }

        [JsonProperty("paymasterData")]
        public virtual string PaymasterData { get; set; }

        [JsonProperty("clientId")]
        public virtual string ClientId { get; set; }

        public GsnRelayData Clone()
        {
            return new()
            {
                MaxFeePerGas = MaxFeePerGas,
                MaxPriorityFeePerGas = MaxPriorityFeePerGas,
                TransactionCalldataGasUsed = TransactionCalldataGasUsed,
                RelayWorker = RelayWorker,
                Paymaster = Paymaster,
                Forwarder = Forwarder,
                PaymasterData = PaymasterData,
                ClientId = ClientId
            };
        }

    }

    public class GsnRelayHttpRequestMetadata
    {

        [JsonProperty("maxAcceptanceBudget")]
        public string MaxAcceptanceBudget;

        [JsonProperty("relayHubAddress")]
        public string RelayHubAddress;

        [JsonProperty("signature")]
        public string Signature;

        [JsonProperty("approvalData")]
        public string ApprovalData;

        [JsonProperty("relayLastKnownNonce")]
        public BigInteger RelayLastKnownNonce;

        [JsonProperty("relayMaxNonce")]
        public BigInteger RelayMaxNonce;

        [JsonProperty("domainSeparatorName")]
        public string DomainSeparatorName;

        [JsonProperty("relayRequestId")]
        public string RelayRequestId;

    }

    public class GsnRelayRequest
    {

        [JsonProperty("request")]
        public virtual GsnForwardRequest Request { get; set; }

        [JsonProperty("relayData")]
        public virtual GsnRelayData RelayData { get; set; }

        public GsnRelayRequest Clone()
        {
            return new()
            {
                Request = Request.Clone(),
                RelayData = RelayData.Clone(),
            };
        }

    }

    public class GsnRelayHttpRequest
    {

        [JsonProperty("relayRequest")]
        public GsnRelayRequest RelayRequest;

        [JsonProperty("metadata")]
        public GsnRelayHttpRequestMetadata Metadata;

    }

}