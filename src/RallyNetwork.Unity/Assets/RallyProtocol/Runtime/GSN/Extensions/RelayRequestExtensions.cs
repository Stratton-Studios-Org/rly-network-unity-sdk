using System.Collections;
using System.Collections.Generic;

using RallyProtocol.GSN.Contracts;

using UnityEngine;

namespace RallyProtocol.GSN.Contracts
{

    public static class RelayRequestExtensions
    {

        public static RelayRequest Clone(this RelayRequest relayRequest)
        {
            return new()
            {
                RelayData = relayRequest.RelayData.Clone(),
                Request = relayRequest.Request.Clone()
            };
        }

    }

    public static class RelayDataExtensions
    {

        public static RelayData Clone(this RelayData data)
        {
            return new()
            {
                MaxFeePerGas = data.MaxFeePerGas,
                MaxPriorityFeePerGas = data.MaxPriorityFeePerGas,
                Paymaster = data.Paymaster,
                PaymasterData = data.PaymasterData,
                RelayWorker = data.RelayWorker,
                TransactionCalldataGasUsed = data.TransactionCalldataGasUsed,
                Forwarder = data.Forwarder,
                ClientId = data.ClientId,
            };
        }

    }

    public static class ForwardRequestExtensions
    {

        public static ForwardRequest Clone(this ForwardRequest forwardRequest)
        {
            return new()
            {
                Data = forwardRequest.Data,
                From = forwardRequest.From,
                To = forwardRequest.To,
                Gas = forwardRequest.Gas,
                Nonce = forwardRequest.Nonce,
                ValidUntilTime = forwardRequest.ValidUntilTime,
                Value = forwardRequest.Value,
            };
        }

    }

}