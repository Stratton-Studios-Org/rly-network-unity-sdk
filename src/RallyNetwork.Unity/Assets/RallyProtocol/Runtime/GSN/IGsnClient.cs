using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Nethereum.Web3.Accounts;

using RallyProtocol.Logging;
using RallyProtocol.Networks;

namespace RallyProtocol.GSN
{

    public interface IGsnClient
    {

        #region Properties

        public IRallyLogger Logger { get; }
        public GsnTransactionHelper TransactionHelper { get; }

        #endregion

        #region Public Methods

        public Task<string> RelayTransactionAsync(Account account, RallyNetworkConfig config, GsnTransactionDetails transaction);

        #endregion

    }

}
