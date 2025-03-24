# GsnTransactionDetails

The `GsnTransactionDetails` class contains the parameters required for executing a gasless transaction through the Gas Station Network (GSN).

## Class Definition

```csharp
using System.Numerics;

namespace RallyProtocol.GSN.Models
{
    public class GsnTransactionDetails
    {
        // Required properties
        public string From { get; set; }
        public string To { get; set; }
        public string Data { get; set; }
        public uint Gas { get; set; }
        
        // Optional properties
        public BigInteger? Value { get; set; }
        public BigInteger? MaxFeePerGas { get; set; }
        public BigInteger? MaxPriorityFeePerGas { get; set; }
        public string? PaymasterData { get; set; }
    }
}
```

## Properties

### Required Properties

| Property | Type | Description |
|----------|------|-------------|
| `From` | `string` | The Ethereum address of the transaction sender. This should be the user's wallet address. |
| `To` | `string` | The Ethereum address of the target contract that the transaction will interact with. |
| `Data` | `string` | The encoded function call data (ABI-encoded method signature and parameters). |
| `Gas` | `uint` | The gas limit for the transaction. This is an upper bound on how much gas the transaction can use. |

### Optional Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `BigInteger?` | The amount of ETH in wei to send with the transaction. For token transfers and most contract interactions, this should be 0. |
| `MaxFeePerGas` | `BigInteger?` | The maximum fee per gas the sender is willing to pay (EIP-1559). If not specified, a network default will be used. |
| `MaxPriorityFeePerGas` | `BigInteger?` | The maximum priority fee per gas (tip) the sender is willing to pay to miners (EIP-1559). If not specified, a network default will be used. |
| `PaymasterData` | `string?` | Optional data to be passed to the paymaster. This is used in advanced GSN scenarios. |

## Usage

The `GsnTransactionDetails` object is used when making gasless transactions with the Rally Protocol SDK. It provides all the necessary information for the GSN relayer to execute the transaction on behalf of the user.

### Basic Example

```csharp
// Create GSN transaction details for a token transfer
GsnTransactionDetails txDetails = new GsnTransactionDetails
{
    From = await rlyNetwork.AccountManager.GetPublicAddressAsync(),
    To = "0xContractAddress",  // Token contract address
    Data = "0x...",  // Encoded transfer function call
    Gas = 100000,    // Gas limit
    Value = 0        // No ETH value being sent
};

// Relay the transaction through GSN
string txHash = await rlyNetwork.RelayAsync(txDetails);
```

### Advanced Example with EIP-1559 Parameters

```csharp
// Create GSN transaction details with EIP-1559 gas parameters
GsnTransactionDetails txDetails = new GsnTransactionDetails
{
    From = await rlyNetwork.AccountManager.GetPublicAddressAsync(),
    To = "0xContractAddress",
    Data = "0x...",
    Gas = 120000,
    Value = 0,
    MaxFeePerGas = BigInteger.Parse("50000000000"),         // 50 Gwei max fee
    MaxPriorityFeePerGas = BigInteger.Parse("1500000000")   // 1.5 Gwei priority fee
};

// Relay the transaction
string txHash = await rlyNetwork.RelayAsync(txDetails);
```

## Creating Transaction Data

The `Data` property requires ABI-encoded function call data. Here's how to create it:

```csharp
// Get the Web3 provider
Web3 web3 = await rlyNetwork.GetProviderAsync();

// Get a contract instance (example with an ERC-20 token contract)
var tokenContract = web3.Eth.GetContract(tokenAbi, tokenAddress);

// Get the encoded function call data for a transfer
var transferFunction = tokenContract.GetFunction("transfer");
string data = transferFunction.GetData(
    recipientAddress,  // Recipient address
    new BigInteger(1000000000000000000)  // 1 token with 18 decimals
);

// Use this data in the GsnTransactionDetails
GsnTransactionDetails txDetails = new GsnTransactionDetails
{
    From = await rlyNetwork.AccountManager.GetPublicAddressAsync(),
    To = tokenAddress,
    Data = data,
    Gas = 100000,
    Value = 0
};
```

## Estimating Gas

Before setting the `Gas` value, you can estimate how much gas the transaction will likely consume:

```csharp
// Get the Web3 provider
Web3 web3 = await rlyNetwork.GetProviderAsync();

// Create a transaction call request
var callRequest = new Nethereum.RPC.Eth.DTOs.CallInput
{
    From = userAddress,
    To = contractAddress,
    Data = data
};

// Estimate gas
var gasEstimate = await web3.Eth.Transactions.EstimateGas.SendRequestAsync(callRequest);

// Add a safety margin and use in GSN transaction
uint gasLimit = (uint)(gasEstimate.Value * 1.2); // Add 20% safety margin

GsnTransactionDetails txDetails = new GsnTransactionDetails
{
    From = userAddress,
    To = contractAddress,
    Data = data,
    Gas = gasLimit,
    Value = 0
};
```

## Common Issues and Solutions

### Insufficient Gas Limit

If a transaction fails with an "out of gas" error, the `Gas` value was likely set too low:

```csharp
// Increase the gas limit
txDetails.Gas = 200000; // Doubled from previous example
```

### Transaction Underpriced

If a transaction fails with "transaction underpriced" error, the gas price parameters need adjustment:

```csharp
// Increase gas price parameters
txDetails.MaxFeePerGas = BigInteger.Parse("70000000000");         // 70 Gwei
txDetails.MaxPriorityFeePerGas = BigInteger.Parse("2000000000");  // 2 Gwei
```

### Invalid Recipient

Ensure that the `To` address is a valid contract address that can receive the transaction:

```csharp
// Validate the contract address first
bool isContractAddress = await web3.Eth.GetCode.SendRequestAsync(contractAddress) != "0x";
if (!isContractAddress)
{
    throw new Exception("The provided address is not a contract");
}
```

## Related Documentation

- [GSN Operations](./gsn-operations.md): General documentation on using the Gas Station Network
- [IRallyNetwork.RelayAsync](./IRallyNetwork.md#relayasync): Method used to relay GSN transactions
- [MetaTxMethod](./transaction-operations.md#using-metatxmethod-with-transfers): Simplified GSN usage with transfer methods

## GsnConfig Class

The SDK also provides a `GsnConfig` class for configuring GSN parameters:

```csharp
public class GsnConfig
{
    public string PaymasterAddress { get; set; }
    public string RelayHubAddress { get; set; }
    public int ChainId { get; set; }
    public uint MaxAcceptanceBudget { get; set; }
    public string DomainSeparatorName { get; set; }
}
```

This configuration is typically set at the network level and applies to all GSN transactions.
