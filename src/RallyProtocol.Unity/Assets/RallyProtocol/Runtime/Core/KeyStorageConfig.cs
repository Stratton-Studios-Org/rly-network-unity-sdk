using System.Collections;
using System.Collections.Generic;

namespace RallyProtocol.Core
{

    public class KeyStorageConfig
    {

        public virtual bool? SaveToCloud { get; set; }
        public virtual bool? RejectOnCloudSaveFailure { get; set; }

    }

}