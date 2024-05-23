using System.Collections;
using System.Collections.Generic;

using Nethereum.ABI.EIP712;
using Nethereum.Signer;
using Nethereum.Signer.EIP712;
using Nethereum.Web3.Accounts;

namespace RallyProtocol.Accounts
{

    /// <summary>
    /// Helper extensions and methods for <see cref="Account"/> class.
    /// </summary>
    public static class AccountExtensions
    {

        public static string SignTypedDataV4(this Account account, TypedDataRaw eip712Data)
        {
            return Eip712TypedDataSigner.Current.SignTypedDataV4(eip712Data.ToJson(), new EthECKey(account.PrivateKey));
        }

        public static string SignTypedDataV4<TDomain>(this Account account, TypedData<TDomain> eip712Data) where TDomain : IDomain
        {
            return Eip712TypedDataSigner.Current.SignTypedDataV4(eip712Data, new EthECKey(account.PrivateKey));
        }

        public static string SignTypedData<TDomain>(this Account account, TypedData<TDomain> eip712Data) where TDomain : IDomain
        {
            return Eip712TypedDataSigner.Current.SignTypedData(eip712Data, new EthECKey(account.PrivateKey));
        }

        public static string SignMessage(this Account account, byte[] message)
        {
            return new EthereumMessageSigner().HashAndSign(message, new EthECKey(account.PrivateKey));
        }

    }

}