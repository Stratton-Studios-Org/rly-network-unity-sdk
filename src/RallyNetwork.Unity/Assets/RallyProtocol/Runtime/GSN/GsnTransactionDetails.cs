using System.Collections;
using System.Collections.Generic;

using Nethereum.Hex.HexTypes;

using UnityEngine;

namespace RallyProtocol
{

    public class GsnTransactionDetails
    {

        /// <summary>
        /// Users address
        /// </summary>
        public string From;

        /// <summary>
        /// Transaction data
        /// </summary>
        public string Data;

        /// <summary>
        /// Contract address
        /// </summary>
        public string To;

        /// <summary>
        /// Ether value
        /// </summary>
        public string? Value;
        //optional gas
        public HexBigInteger? Gas;

        //should be hex
        public HexBigInteger MaxFeePerGas;
        //should be hex
        public HexBigInteger MaxPriorityFeePerGas;
        //paymaster contract address
        public string? PaymasterData;

        //Value used to identify applications in RelayRequests.
        public string? ClientId;

        // Optional parameters for RelayProvider only:
        /**
         * Set to 'false' to create a direct transaction
         */
        public bool? UseGSN;

    }

}