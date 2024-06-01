using System.Collections;
using System.Collections.Generic;

using RallyProtocol.Contracts.Forwarder;
using RallyProtocol.Contracts.RelayHub;

namespace RallyProtocol.Extensions
{

    /// <summary>
    /// Extensions for <see cref="RelayRequest"/> type.
    /// </summary>
    public static class RelayRequestExtensions
    {

        /// <summary>
        /// Creates a new instance and copies all properties.
        /// </summary>
        /// <param name="relayRequest">The instance to clone</param>
        /// <returns>Returns the cloned instance</returns>
        public static RelayRequest Clone(this RelayRequest relayRequest)
        {
            return new()
            {
                RelayData = relayRequest.RelayData.Clone(),
                Request = relayRequest.Request.Clone()
            };
        }

    }

    /// <summary>
    /// Extensions for <see cref="RelayData"/> type.
    /// </summary>
    public static class RelayDataExtensions
    {

        /// <summary>
        /// Creates a new instance and copies all properties.
        /// </summary>
        /// <param name="data">The instance to clone</param>
        /// <returns>Returns the cloned instance</returns>
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

    /// <summary>
    /// Extensions for <see cref="ForwardRequest"/> type.
    /// </summary>
    public static class ForwardRequestExtensions
    {

        /// <summary>
        /// Creates a new instance and copies all properties.
        /// </summary>
        /// <param name="forwardRequest">The instance to clone</param>
        /// <returns>Returns the cloned instance</returns>
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