using System;
using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using Nethereum.Web3.Accounts;

using NUnit.Framework;

using RallyProtocol.Accounts;
using RallyProtocol.Networks;

using UnityEngine;
using UnityEngine.TestTools;

namespace RallyProtocol.Tests
{

    public class RallyEvmNetworkTests
    {

        protected string apiKey = "eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOjYxMX0.PUxW-wsXnE28BBRS8LhltsErvWCSPd6N0xpSCk1MkJ7TxXF4cvCfB7nDptakv5myAtIjJNMS-Gs9D_VTTH2tXQ";
        protected RallyNetworkType networkType = RallyNetworkType.Amoy;

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

        [UnityTest]
        public IEnumerator ClaimRlyPasses()
        {
            return UniTask.ToCoroutine(async () =>
            {
                IRallyAccountManager accountManager = RallyUnityAccountManager.Default;
                IRallyNetwork network = RallyUnityNetworkFactory.Create(this.networkType, this.apiKey);

                // Create a new account
                await accountManager.CreateAccountAsync(new() { Overwrite = true });

                try
                {

                    // Claim RLY
                    string result = await network.ClaimRly();
                    Assert.IsNotEmpty(result);
                }
                catch (Exception ex)
                {
                    Assert.Fail("Claim RLY must not fail on a new account", ex);
                }
            });
        }
    }

}