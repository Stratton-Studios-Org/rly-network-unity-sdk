# Conversion Utilities

The Rally Protocol Unity SDK provides several utility functions for converting between different unit formats used in blockchain operations. These utilities help developers work with token denominations, unit conversions, and other common blockchain value transformations.

## Token Unit Conversions

When working with blockchain tokens, you'll often need to convert between human-readable decimal values (e.g., 1.5 tokens) and the smallest unit values used by the blockchain (wei, with 18 decimal places for most ERC-20 tokens).

The SDK provides utilities through the `ValueUtils` class:

```csharp
using RallyProtocol.Utils;
using System.Numerics;

// Convert from decimal (user-friendly) to wei (blockchain value)
decimal userValue = 1.5m; // 1.5 tokens
BigInteger weiValue = ValueUtils.ToWei(userValue);
// Result: 1500000000000000000 (1.5 * 10^18)

// Convert from wei (blockchain value) to decimal (user-friendly)
BigInteger weiAmount = BigInteger.Parse("1500000000000000000");
decimal displayAmount = ValueUtils.FromWei(weiAmount);
// Result: 1.5m
```

## Handling Different Token Decimals

While most ERC-20 tokens use 18 decimal places, some tokens may use different decimal precision. The SDK includes utility functions that support specifying the number of decimals:

```csharp
// Convert with custom decimal places (e.g., USDC with 6 decimals)
int usdcDecimals = 6;
decimal usdcAmount = 10.5m;
BigInteger smallestUnitAmount = ValueUtils.ToWei(usdcAmount, usdcDecimals);
// Result: 10500000 (10.5 * 10^6)

// Convert back to decimal representation
decimal displayUsdcAmount = ValueUtils.FromWei(smallestUnitAmount, usdcDecimals);
// Result: 10.5m
```

## Working with BigInteger

The SDK provides helper methods for working with `BigInteger` values, which are commonly used for representing token amounts on the blockchain:

```csharp
// Parse a string to BigInteger
BigInteger value = ValueUtils.ParseBigInteger("1500000000000000000");

// Convert a BigInteger to a hex string
string hexValue = ValueUtils.ToHexString(value);
// Result: "0x14d1120d7b160000"

// Parse a hex string to BigInteger
BigInteger parsedValue = ValueUtils.ParseHexString("0x14d1120d7b160000");
// Result: 1500000000000000000
```

## Formatting for Display

The SDK includes formatting utilities to present token values in a user-friendly way:

```csharp
// Format a decimal with up to 2 decimal places
string formattedValue = ValueUtils.FormatDecimal(1.5m, 2);
// Result: "1.50"

// Format a wei value for display with up to 4 decimal places
BigInteger weiValue = BigInteger.Parse("1234567890000000000"); 
string displayValue = ValueUtils.FormatFromWei(weiValue, 4);
// Result: "1.2346"

// Format with currency symbol
string withSymbol = ValueUtils.FormatWithSymbol(1.5m, "RLY", 2);
// Result: "1.50 RLY"
```

## Percentage Calculations

For scenarios involving percentage-based calculations:

```csharp
// Calculate percentage of a value
decimal total = 100m;
decimal percentage = 15m; // 15%
decimal result = ValueUtils.CalculatePercentage(total, percentage);
// Result: 15m

// Calculate percentage between two values
decimal value1 = 150m;
decimal value2 = 100m;
decimal percentChange = ValueUtils.CalculatePercentageChange(value1, value2);
// Result: 50m (value1 is 50% higher than value2)
```

## Gas and Fee Calculations

For calculating transaction fees and gas costs:

```csharp
// Calculate transaction fee from gas used and gas price
BigInteger gasUsed = BigInteger.Parse("21000");
BigInteger gasPrice = BigInteger.Parse("50000000000"); // 50 Gwei
BigInteger fee = ValueUtils.CalculateTransactionFee(gasUsed, gasPrice);
// Result: 1050000000000000 wei (0.00105 ETH)

// Convert fee to display value
decimal displayFee = ValueUtils.FromWei(fee);
// Result: 0.00105m
```

## Related Documentation

- [Web3 Utilities](./web3-utilities.md) - For Web3-specific utility functions
- [IRallyNetwork](./IRallyNetwork.md) - For main network interface documentation
