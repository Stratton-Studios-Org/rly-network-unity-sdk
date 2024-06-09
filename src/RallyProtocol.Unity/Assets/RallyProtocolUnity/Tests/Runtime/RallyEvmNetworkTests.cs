using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using Nethereum.Web3.Accounts;

using NUnit.Framework;

using RallyProtocol;
using RallyProtocol.Accounts;
using RallyProtocol.Networks;

using RallyProtocolUnity.Accounts;
using RallyProtocolUnity.Networks;

using UnityEngine;
using UnityEngine.TestTools;

namespace RallyProtocolUnity.Tests
{

    public class RallyEvmNetworkTests
    {

        protected const decimal amount = 2;
        protected const string apiKey = "eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOjYxMX0.PUxW-wsXnE28BBRS8LhltsErvWCSPd6N0xpSCk1MkJ7TxXF4cvCfB7nDptakv5myAtIjJNMS-Gs9D_VTTH2tXQ";

        protected RallyNetworkType networkType = RallyNetworkType.Amoy;

        [Sequential]
        [UnityTest]
        public IEnumerator CreateAccountPasses()
        {
            return UniTask.ToCoroutine(async () =>
            {
                IRallyAccountManager accountManager = RallyUnityAccountManager.Default;

                try
                {

                    // Create a new account
                    await accountManager.CreateAccountAsync(new() { Overwrite = true });
                }
                catch
                {
                    Assert.Fail($"Create account must not fail if the {nameof(CreateAccountOptions.Overwrite)} flag is used.");
                }
            });
        }

        [Sequential]
        [UnityTest]
        public IEnumerator CreateAccountNotOverwritePasses()
        {
            return UniTask.ToCoroutine(async () =>
            {
                IRallyAccountManager accountManager = RallyUnityAccountManager.Default;

                try
                {

                    // Create a new account
                    Account account = await accountManager.CreateAccountAsync(new() { Overwrite = true });
                    Assert.IsNotNull(account);

                    // Force for account already exists exception
                    await accountManager.CreateAccountAsync();
                }
                catch (Exception ex)
                {
                    if (ex is RallyAccountExistsException)
                    {
                        return;
                    }

                    Assert.Fail($"Create account must fail with {nameof(RallyAccountExistsException)} if the account already exists and the {nameof(CreateAccountOptions.Overwrite)} flag is not used.");
                }
            });
        }

        [Sequential]
        [UnityTest]
        public IEnumerator ClaimRlyPasses()
        {
            return UniTask.ToCoroutine(async () =>
            {
                IRallyAccountManager accountManager = RallyUnityAccountManager.Default;
                IRallyNetwork network = RallyUnityNetworkFactory.Create(this.networkType, apiKey);

                // Create a new account
                await accountManager.CreateAccountAsync(new() { Overwrite = true });

                try
                {

                    // Claim RLY
                    string result = await network.ClaimRlyAsync();
                    Assert.IsNotEmpty(result);
                }
                catch (Exception ex)
                {
                    Assert.Fail("Claim RLY must not fail on a new account", ex);
                }
            });
        }

        [Sequential]
        [UnityTest]
        public IEnumerator TransferPasses()
        {
            return UniTask.ToCoroutine(async () =>
            {
                LogAssert.ignoreFailingMessages = true;
                IRallyAccountManager accountManager = RallyUnityAccountManager.Default;
                IRallyNetwork network = RallyUnityNetworkFactory.Create(this.networkType, apiKey);

                // Create a new account
                await accountManager.CreateAccountAsync(new() { Overwrite = true });

                try
                {

                    // Claim RLY
                    string result = await network.ClaimRlyAsync();
                    Assert.IsNotEmpty(result);

                    // Transfer RLY
                    await network.TransferAsync("0xbEAE33FE2517b007C5765f74f428452210e02813", amount, MetaTxMethod.ExecuteMetaTransaction);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    Assert.Fail("Transfer must not fail", ex);
                }
            });
        }

        [Sequential]
        [Test]
        public async Task TransferAsyncPasses()
        {
            LogAssert.ignoreFailingMessages = true;
            IRallyAccountManager accountManager = RallyUnityAccountManager.Default;
            IRallyNetwork network = RallyUnityNetworkFactory.Create(this.networkType, apiKey);

            // Create a new account
            await accountManager.CreateAccountAsync(new() { Overwrite = true });

            try
            {

                // Claim RLY
                string result = await network.ClaimRlyAsync();
                Assert.IsNotEmpty(result);

                // Transfer RLY
                await network.TransferAsync("0xbEAE33FE2517b007C5765f74f428452210e02813", amount, MetaTxMethod.ExecuteMetaTransaction);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                Assert.Fail("Transfer must not fail", ex);
            }
        }

        [Sequential]
        [UnityTest]
        public IEnumerator TransferWithPermitPasses()
        {
            return UniTask.ToCoroutine(async () =>
            {
                LogAssert.ignoreFailingMessages = true;
                IRallyAccountManager accountManager = RallyUnityAccountManager.Default;
                IRallyNetwork network = RallyUnityNetworkFactory.Create(RallyNetworkConfig.AmoyWithPermit, apiKey);

                // Create a new account
                await accountManager.CreateAccountAsync(new() { Overwrite = true });

                try
                {

                    // Claim RLY
                    string result = await network.ClaimRlyAsync();
                    Assert.IsNotEmpty(result);

                    // Get balance
                    decimal balance = await network.GetDisplayBalanceAsync();

                    // Transfer RLY
                    await network.TransferAsync("0xbEAE33FE2517b007C5765f74f428452210e02813", amount, MetaTxMethod.Permit);

                    // Get new balance
                    decimal newBalance = await network.GetDisplayBalanceAsync();

                    // The old balance - amount should be equal to new balance
                    Assert.AreEqual(newBalance, balance - amount);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    Assert.Fail("Transfer must not fail", ex);
                }
            });
        }
    }

}