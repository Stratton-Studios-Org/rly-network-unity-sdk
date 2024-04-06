using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace RallyProtocol
{

    public class GsnTransactionDetails
    {

        /// <summary>
        /// Users address
        /// </summary>
        public readonly string From;

        /// <summary>
        /// Transaction data
        /// </summary>
        public readonly string Data;

        /// <summary>
        /// Contract address
        /// </summary>
        public readonly string To;

        /// <summary>
        /// Ether value
        /// </summary>
        public readonly string? Value;
        //optional gas
        public string? Gas;

        //should be hex
        public string MaxFeePerGas;
        //should be hex
        public string MaxPriorityFeePerGas;
        //paymaster contract address
        public readonly string? PaymasterData;

        //Value used to identify applications in RelayRequests.
        public readonly string? ClientId;

        // Optional parameters for RelayProvider only:
        /**
         * Set to 'false' to create a direct transaction
         */
        public readonly bool? UseGSN;

    }

}