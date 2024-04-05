using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace RallyProtocol
{

    [CreateAssetMenu(menuName = "Rally Protocol/Settings")]
    public class RallyProtocolSettings : ScriptableObject
    {

        [SerializeField]
        protected string apiKey;

        public string ApiKey => this.apiKey;

    }

}