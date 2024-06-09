using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using FluentAssertions;

using NUnit.Framework;

using RallyProtocol.Contracts.Forwarder;
using RallyProtocol.Contracts.RelayHub;
using RallyProtocol.Extensions;

using UnityEngine;

namespace RallyProtocolUnity
{

    public class RelayRequestExtensionsTests
    {

        public static readonly ForwardRequest DummyForwardRequest = new()
        {
            Data = new byte[] { 1, 2, 3 },
            From = "from_address",
            To = "to_address",
            Gas = 1,
            Nonce = 2,
            ValidUntilTime = 3,
            Value = 4,
        };

        public static readonly RelayData DummyRelayData = new()
        {
            ClientId = BigInteger.One,
            Forwarder = "forwarder",
            MaxFeePerGas = BigInteger.Zero,
            MaxPriorityFeePerGas = BigInteger.Zero,
            Paymaster = "paymaster",
            PaymasterData = new byte[] { 1 },
            RelayWorker = "relay_worker",
            TransactionCalldataGasUsed = BigInteger.Zero
        };

        public static readonly RelayRequest DummyRelayRequest = new()
        {
            RelayData = DummyRelayData,
            Request = DummyForwardRequest
        };

        [Test]
        public void ForwardRequestClonePasses()
        {
            ForwardRequest cloned = DummyForwardRequest.Clone();
            cloned.Should().BeEquivalentTo(DummyForwardRequest);
        }

        [Test]
        public void RelayDataClonePasses()
        {
            RelayData cloned = DummyRelayData.Clone();
            cloned.Should().BeEquivalentTo(DummyRelayData);
        }

        [Test]
        public void RelayRequestClonePasses()
        {
            RelayRequest cloned = DummyRelayRequest.Clone();
            cloned.Should().BeEquivalentTo(DummyRelayRequest);
        }

    }

}