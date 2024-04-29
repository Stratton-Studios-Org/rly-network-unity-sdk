using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

using Newtonsoft.Json;

using UnityEngine;

namespace RallyProtocol.GSN.Contracts
{

    public partial class RelayHubDeployment : RelayHubDeploymentBase
    {
        public RelayHubDeployment() : base(BYTECODE) { }
        public RelayHubDeployment(string byteCode) : base(byteCode) { }
    }

    public class RelayHubDeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "";
        public RelayHubDeploymentBase() : base(BYTECODE) { }
        public RelayHubDeploymentBase(string byteCode) : base(byteCode) { }

    }

    public partial class AddRelayWorkersFunction : AddRelayWorkersFunctionBase { }

    [Function("addRelayWorkers")]
    public class AddRelayWorkersFunctionBase : FunctionMessage
    {
        [Parameter("address[]", "newRelayWorkers", 1)]
        public virtual List<string> NewRelayWorkers { get; set; }
    }

    public partial class OnRelayServerRegisteredFunction : OnRelayServerRegisteredFunctionBase { }

    [Function("onRelayServerRegistered")]
    public class OnRelayServerRegisteredFunctionBase : FunctionMessage
    {
        [Parameter("address", "relayManager", 1)]
        public virtual string RelayManager { get; set; }
    }

    public partial class DepositForFunction : DepositForFunctionBase { }

    [Function("depositFor")]
    public class DepositForFunctionBase : FunctionMessage
    {
        [Parameter("address", "target", 1)]
        public virtual string Target { get; set; }
    }

    public partial class WithdrawFunction : WithdrawFunctionBase { }

    [Function("withdraw")]
    public class WithdrawFunctionBase : FunctionMessage
    {
        [Parameter("address", "dest", 1)]
        public virtual string Dest { get; set; }
        [Parameter("uint256", "amount", 2)]
        public virtual BigInteger Amount { get; set; }
    }

    public partial class WithdrawMultipleFunction : WithdrawMultipleFunctionBase { }

    [Function("withdrawMultiple")]
    public class WithdrawMultipleFunctionBase : FunctionMessage
    {
        [Parameter("address[]", "dest", 1)]
        public virtual List<string> Dest { get; set; }
        [Parameter("uint256[]", "amount", 2)]
        public virtual List<BigInteger> Amount { get; set; }
    }

    public partial class RelayCallFunction : RelayCallFunctionBase { }

    [Function("relayCall", typeof(RelayCallOutputDTO))]
    public class RelayCallFunctionBase : FunctionMessage
    {
        [Parameter("string", "domainSeparatorName", 1)]
        public virtual string DomainSeparatorName { get; set; }
        [Parameter("uint256", "maxAcceptanceBudget", 2)]
        public virtual BigInteger MaxAcceptanceBudget { get; set; }
        [Parameter("tuple", "relayRequest", 3)]
        public virtual RelayRequest RelayRequest { get; set; }
        [Parameter("bytes", "signature", 4)]
        public virtual byte[] Signature { get; set; }
        [Parameter("bytes", "approvalData", 5)]
        public virtual byte[] ApprovalData { get; set; }
    }

    public partial class PenalizeFunction : PenalizeFunctionBase { }

    [Function("penalize")]
    public class PenalizeFunctionBase : FunctionMessage
    {
        [Parameter("address", "relayWorker", 1)]
        public virtual string RelayWorker { get; set; }
        [Parameter("address", "beneficiary", 2)]
        public virtual string Beneficiary { get; set; }
    }

    public partial class SetConfigurationFunction : SetConfigurationFunctionBase { }

    [Function("setConfiguration")]
    public class SetConfigurationFunctionBase : FunctionMessage
    {
        [Parameter("tuple", "_config", 1)]
        public virtual RelayHubConfig Config { get; set; }
    }

    public partial class SetMinimumStakesFunction : SetMinimumStakesFunctionBase { }

    [Function("setMinimumStakes")]
    public class SetMinimumStakesFunctionBase : FunctionMessage
    {
        [Parameter("address[]", "token", 1)]
        public virtual List<string> Token { get; set; }
        [Parameter("uint256[]", "minimumStake", 2)]
        public virtual List<BigInteger> MinimumStake { get; set; }
    }

    public partial class DeprecateHubFunction : DeprecateHubFunctionBase { }

    [Function("deprecateHub")]
    public class DeprecateHubFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "_deprecationTime", 1)]
        public virtual BigInteger DeprecationTime { get; set; }
    }

    public partial class EscheatAbandonedRelayBalanceFunction : EscheatAbandonedRelayBalanceFunctionBase { }

    [Function("escheatAbandonedRelayBalance")]
    public class EscheatAbandonedRelayBalanceFunctionBase : FunctionMessage
    {
        [Parameter("address", "relayManager", 1)]
        public virtual string RelayManager { get; set; }
    }

    public partial class CalculateChargeFunction : CalculateChargeFunctionBase { }

    [Function("calculateCharge", "uint256")]
    public class CalculateChargeFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "gasUsed", 1)]
        public virtual BigInteger GasUsed { get; set; }
        [Parameter("tuple", "relayData", 2)]
        public virtual RelayData RelayData { get; set; }
    }

    public partial class CalculateDevChargeFunction : CalculateDevChargeFunctionBase { }

    [Function("calculateDevCharge", "uint256")]
    public class CalculateDevChargeFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "charge", 1)]
        public virtual BigInteger Charge { get; set; }
    }

    public partial class GetConfigurationFunction : GetConfigurationFunctionBase { }

    [Function("getConfiguration", typeof(GetConfigurationOutputDTO))]
    public class GetConfigurationFunctionBase : FunctionMessage
    {

    }

    public partial class GetMinimumStakePerTokenFunction : GetMinimumStakePerTokenFunctionBase { }

    [Function("getMinimumStakePerToken", "uint256")]
    public class GetMinimumStakePerTokenFunctionBase : FunctionMessage
    {
        [Parameter("address", "token", 1)]
        public virtual string Token { get; set; }
    }

    public partial class GetWorkerManagerFunction : GetWorkerManagerFunctionBase { }

    [Function("getWorkerManager", "address")]
    public class GetWorkerManagerFunctionBase : FunctionMessage
    {
        [Parameter("address", "worker", 1)]
        public virtual string Worker { get; set; }
    }

    public partial class GetWorkerCountFunction : GetWorkerCountFunctionBase { }

    [Function("getWorkerCount", "uint256")]
    public class GetWorkerCountFunctionBase : FunctionMessage
    {
        [Parameter("address", "manager", 1)]
        public virtual string Manager { get; set; }
    }

    public partial class BalanceOfFunction : BalanceOfFunctionBase { }

    [Function("balanceOf", "uint256")]
    public class BalanceOfFunctionBase : FunctionMessage
    {
        [Parameter("address", "target", 1)]
        public virtual string Target { get; set; }
    }

    public partial class GetStakeManagerFunction : GetStakeManagerFunctionBase { }

    [Function("getStakeManager", "address")]
    public class GetStakeManagerFunctionBase : FunctionMessage
    {

    }

    public partial class GetPenalizerFunction : GetPenalizerFunctionBase { }

    [Function("getPenalizer", "address")]
    public class GetPenalizerFunctionBase : FunctionMessage
    {

    }

    public partial class GetRelayRegistrarFunction : GetRelayRegistrarFunctionBase { }

    [Function("getRelayRegistrar", "address")]
    public class GetRelayRegistrarFunctionBase : FunctionMessage
    {

    }

    public partial class GetBatchGatewayFunction : GetBatchGatewayFunctionBase { }

    [Function("getBatchGateway", "address")]
    public class GetBatchGatewayFunctionBase : FunctionMessage
    {

    }

    public partial class VerifyRelayManagerStakedFunction : VerifyRelayManagerStakedFunctionBase { }

    [Function("verifyRelayManagerStaked")]
    public class VerifyRelayManagerStakedFunctionBase : FunctionMessage
    {
        [Parameter("address", "relayManager", 1)]
        public virtual string RelayManager { get; set; }
    }

    public partial class IsRelayEscheatableFunction : IsRelayEscheatableFunctionBase { }

    [Function("isRelayEscheatable", "bool")]
    public class IsRelayEscheatableFunctionBase : FunctionMessage
    {
        [Parameter("address", "relayManager", 1)]
        public virtual string RelayManager { get; set; }
    }

    public partial class IsDeprecatedFunction : IsDeprecatedFunctionBase { }

    [Function("isDeprecated", "bool")]
    public class IsDeprecatedFunctionBase : FunctionMessage
    {

    }

    public partial class GetDeprecationTimeFunction : GetDeprecationTimeFunctionBase { }

    [Function("getDeprecationTime", "uint256")]
    public class GetDeprecationTimeFunctionBase : FunctionMessage
    {

    }

    public partial class GetCreationBlockFunction : GetCreationBlockFunctionBase { }

    [Function("getCreationBlock", "uint256")]
    public class GetCreationBlockFunctionBase : FunctionMessage
    {

    }

    public partial class VersionHubFunction : VersionHubFunctionBase { }

    [Function("versionHub", "string")]
    public class VersionHubFunctionBase : FunctionMessage
    {

    }

    public partial class AggregateGasleftFunction : AggregateGasleftFunctionBase { }

    [Function("aggregateGasleft", "uint256")]
    public class AggregateGasleftFunctionBase : FunctionMessage
    {

    }

    public partial class AbandonedRelayManagerBalanceEscheatedEventDTO : AbandonedRelayManagerBalanceEscheatedEventDTOBase { }

    [Event("AbandonedRelayManagerBalanceEscheated")]
    public class AbandonedRelayManagerBalanceEscheatedEventDTOBase : IEventDTO
    {
        [Parameter("address", "relayManager", 1, true)]
        public virtual string RelayManager { get; set; }
        [Parameter("uint256", "balance", 2, false)]
        public virtual BigInteger Balance { get; set; }
    }

    public partial class DepositedEventDTO : DepositedEventDTOBase { }

    [Event("Deposited")]
    public class DepositedEventDTOBase : IEventDTO
    {
        [Parameter("address", "paymaster", 1, true)]
        public virtual string Paymaster { get; set; }
        [Parameter("address", "from", 2, true)]
        public virtual string From { get; set; }
        [Parameter("uint256", "amount", 3, false)]
        public virtual BigInteger Amount { get; set; }
    }

    public partial class HubDeprecatedEventDTO : HubDeprecatedEventDTOBase { }

    [Event("HubDeprecated")]
    public class HubDeprecatedEventDTOBase : IEventDTO
    {
        [Parameter("uint256", "deprecationTime", 1, false)]
        public virtual BigInteger DeprecationTime { get; set; }
    }

    public partial class RelayHubConfiguredEventDTO : RelayHubConfiguredEventDTOBase { }

    [Event("RelayHubConfigured")]
    public class RelayHubConfiguredEventDTOBase : IEventDTO
    {
        [Parameter("tuple", "config", 1, false)]
        public virtual RelayHubConfig Config { get; set; }
    }

    public partial class RelayWorkersAddedEventDTO : RelayWorkersAddedEventDTOBase { }

    [Event("RelayWorkersAdded")]
    public class RelayWorkersAddedEventDTOBase : IEventDTO
    {
        [Parameter("address", "relayManager", 1, true)]
        public virtual string RelayManager { get; set; }
        [Parameter("address[]", "newRelayWorkers", 2, false)]
        public virtual List<string> NewRelayWorkers { get; set; }
        [Parameter("uint256", "workersCount", 3, false)]
        public virtual BigInteger WorkersCount { get; set; }
    }

    public partial class StakingTokenDataChangedEventDTO : StakingTokenDataChangedEventDTOBase { }

    [Event("StakingTokenDataChanged")]
    public class StakingTokenDataChangedEventDTOBase : IEventDTO
    {
        [Parameter("address", "token", 1, false)]
        public virtual string Token { get; set; }
        [Parameter("uint256", "minimumStake", 2, false)]
        public virtual BigInteger MinimumStake { get; set; }
    }

    public partial class TransactionRejectedByPaymasterEventDTO : TransactionRejectedByPaymasterEventDTOBase { }

    [Event("TransactionRejectedByPaymaster")]
    public class TransactionRejectedByPaymasterEventDTOBase : IEventDTO
    {
        [Parameter("address", "relayManager", 1, true)]
        public virtual string RelayManager { get; set; }
        [Parameter("address", "paymaster", 2, true)]
        public virtual string Paymaster { get; set; }
        [Parameter("bytes32", "relayRequestID", 3, true)]
        public virtual byte[] RelayRequestID { get; set; }
        [Parameter("address", "from", 4, false)]
        public virtual string From { get; set; }
        [Parameter("address", "to", 5, false)]
        public virtual string To { get; set; }
        [Parameter("address", "relayWorker", 6, false)]
        public virtual string RelayWorker { get; set; }
        [Parameter("bytes4", "selector", 7, false)]
        public virtual byte[] Selector { get; set; }
        [Parameter("uint256", "innerGasUsed", 8, false)]
        public virtual BigInteger InnerGasUsed { get; set; }
        [Parameter("bytes", "reason", 9, false)]
        public virtual byte[] Reason { get; set; }
    }

    public partial class TransactionRelayedEventDTO : TransactionRelayedEventDTOBase { }

    [Event("TransactionRelayed")]
    public class TransactionRelayedEventDTOBase : IEventDTO
    {
        [Parameter("address", "relayManager", 1, true)]
        public virtual string RelayManager { get; set; }
        [Parameter("address", "relayWorker", 2, true)]
        public virtual string RelayWorker { get; set; }
        [Parameter("bytes32", "relayRequestID", 3, true)]
        public virtual byte[] RelayRequestID { get; set; }
        [Parameter("address", "from", 4, false)]
        public virtual string From { get; set; }
        [Parameter("address", "to", 5, false)]
        public virtual string To { get; set; }
        [Parameter("address", "paymaster", 6, false)]
        public virtual string Paymaster { get; set; }
        [Parameter("bytes4", "selector", 7, false)]
        public virtual byte[] Selector { get; set; }
        [Parameter("uint8", "status", 8, false)]
        public virtual byte Status { get; set; }
        [Parameter("uint256", "charge", 9, false)]
        public virtual BigInteger Charge { get; set; }
    }

    public partial class TransactionResultEventDTO : TransactionResultEventDTOBase { }

    [Event("TransactionResult")]
    public class TransactionResultEventDTOBase : IEventDTO
    {
        [Parameter("uint8", "status", 1, false)]
        public virtual byte Status { get; set; }
        [Parameter("bytes", "returnValue", 2, false)]
        public virtual byte[] ReturnValue { get; set; }
    }

    public partial class WithdrawnEventDTO : WithdrawnEventDTOBase { }

    [Event("Withdrawn")]
    public class WithdrawnEventDTOBase : IEventDTO
    {
        [Parameter("address", "account", 1, true)]
        public virtual string Account { get; set; }
        [Parameter("address", "dest", 2, true)]
        public virtual string Dest { get; set; }
        [Parameter("uint256", "amount", 3, false)]
        public virtual BigInteger Amount { get; set; }
    }

    public partial class RelayCallOutputDTO : RelayCallOutputDTOBase { }

    [FunctionOutput]
    public class RelayCallOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("bool", "paymasterAccepted", 1)]
        public virtual bool PaymasterAccepted { get; set; }
        [Parameter("uint256", "charge", 2)]
        public virtual BigInteger Charge { get; set; }
        [Parameter("uint8", "status", 3)]
        public virtual byte Status { get; set; }
        [Parameter("bytes", "returnValue", 4)]
        public virtual byte[] ReturnValue { get; set; }
    }

    public partial class CalculateChargeOutputDTO : CalculateChargeOutputDTOBase { }

    [FunctionOutput]
    public class CalculateChargeOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class CalculateDevChargeOutputDTO : CalculateDevChargeOutputDTOBase { }

    [FunctionOutput]
    public class CalculateDevChargeOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class GetConfigurationOutputDTO : GetConfigurationOutputDTOBase { }

    [FunctionOutput]
    public class GetConfigurationOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("tuple", "config", 1)]
        public virtual RelayHubConfig Config { get; set; }
    }

    public partial class GetMinimumStakePerTokenOutputDTO : GetMinimumStakePerTokenOutputDTOBase { }

    [FunctionOutput]
    public class GetMinimumStakePerTokenOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class GetWorkerManagerOutputDTO : GetWorkerManagerOutputDTOBase { }

    [FunctionOutput]
    public class GetWorkerManagerOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class GetWorkerCountOutputDTO : GetWorkerCountOutputDTOBase { }

    [FunctionOutput]
    public class GetWorkerCountOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class BalanceOfOutputDTO : BalanceOfOutputDTOBase { }

    [FunctionOutput]
    public class BalanceOfOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class GetStakeManagerOutputDTO : GetStakeManagerOutputDTOBase { }

    [FunctionOutput]
    public class GetStakeManagerOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class GetPenalizerOutputDTO : GetPenalizerOutputDTOBase { }

    [FunctionOutput]
    public class GetPenalizerOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class GetRelayRegistrarOutputDTO : GetRelayRegistrarOutputDTOBase { }

    [FunctionOutput]
    public class GetRelayRegistrarOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class GetBatchGatewayOutputDTO : GetBatchGatewayOutputDTOBase { }

    [FunctionOutput]
    public class GetBatchGatewayOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }



    public partial class IsRelayEscheatableOutputDTO : IsRelayEscheatableOutputDTOBase { }

    [FunctionOutput]
    public class IsRelayEscheatableOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("bool", "", 1)]
        public virtual bool ReturnValue1 { get; set; }
    }

    public partial class IsDeprecatedOutputDTO : IsDeprecatedOutputDTOBase { }

    [FunctionOutput]
    public class IsDeprecatedOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("bool", "", 1)]
        public virtual bool ReturnValue1 { get; set; }
    }

    public partial class GetDeprecationTimeOutputDTO : GetDeprecationTimeOutputDTOBase { }

    [FunctionOutput]
    public class GetDeprecationTimeOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class GetCreationBlockOutputDTO : GetCreationBlockOutputDTOBase { }

    [FunctionOutput]
    public class GetCreationBlockOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class VersionHubOutputDTO : VersionHubOutputDTOBase { }

    [FunctionOutput]
    public class VersionHubOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("string", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class AggregateGasleftOutputDTO : AggregateGasleftOutputDTOBase { }

    [FunctionOutput]
    public class AggregateGasleftOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class RelayHubConfig : RelayHubConfigBase { }

    public class RelayHubConfigBase
    {
        [Parameter("uint256", "maxWorkerCount", 1)]
        public virtual BigInteger MaxWorkerCount { get; set; }
        [Parameter("uint256", "gasReserve", 2)]
        public virtual BigInteger GasReserve { get; set; }
        [Parameter("uint256", "postOverhead", 3)]
        public virtual BigInteger PostOverhead { get; set; }
        [Parameter("uint256", "gasOverhead", 4)]
        public virtual BigInteger GasOverhead { get; set; }
        [Parameter("uint256", "minimumUnstakeDelay", 5)]
        public virtual BigInteger MinimumUnstakeDelay { get; set; }
        [Parameter("address", "devAddress", 6)]
        public virtual string DevAddress { get; set; }
        [Parameter("uint8", "devFee", 7)]
        public virtual byte DevFee { get; set; }
        [Parameter("uint80", "baseRelayFee", 8)]
        public virtual BigInteger BaseRelayFee { get; set; }
        [Parameter("uint16", "pctRelayFee", 9)]
        public virtual ushort PctRelayFee { get; set; }
    }

    [Struct("RelayData")]
    public partial class RelayData : RelayDataBase { }

    public class RelayDataBase
    {
        [Parameter("uint256", "maxFeePerGas", 1)]
        [JsonProperty("maxFeePerGas")]
        public virtual BigInteger MaxFeePerGas { get; set; }

        [Parameter("uint256", "maxPriorityFeePerGas", 2)]
        [JsonProperty("maxPriorityFeePerGas")]
        public virtual BigInteger MaxPriorityFeePerGas { get; set; }

        [Parameter("uint256", "transactionCalldataGasUsed", 3)]
        [JsonProperty("transactionCalldataGasUsed")]
        public virtual BigInteger TransactionCalldataGasUsed { get; set; }

        [Parameter("address", "relayWorker", 4)]
        [JsonProperty("relayWorker")]
        public virtual string RelayWorker { get; set; }

        [Parameter("address", "paymaster", 5)]
        [JsonProperty("paymaster")]
        public virtual string Paymaster { get; set; }

        [Parameter("address", "forwarder", 6)]
        [JsonProperty("forwarder")]
        public virtual string Forwarder { get; set; }

        [Parameter("bytes", "paymasterData", 7)]
        [JsonProperty("paymasterData")]
        public virtual byte[] PaymasterData { get; set; }

        [Parameter("uint256", "clientId", 8)]
        [JsonProperty("clientId")]
        public virtual BigInteger ClientId { get; set; }
    }

    [Struct("RelayRequest")]
    public partial class RelayRequest : RelayRequestBase { }

    public class RelayRequestBase
    {
        [Parameter("tuple", "request", 1, "ForwardRequest")]
        [JsonProperty("request")]
        public virtual ForwardRequest Request { get; set; }
        [Parameter("tuple", "relayData", 2, "RelayData")]
        [JsonProperty("relayData")]
        public virtual RelayData RelayData { get; set; }
    }

}