using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace RallyProtocolUnity.Components
{
    /// <summary>
    /// Base MonoBehaviour class for all Rally Protocol Unity components.
    /// Provides common functionality and constants for Rally Protocol components.
    /// </summary>
    /// <remarks>
    /// All Rally Protocol component classes should inherit from this class
    /// to ensure consistent behavior and menu organization in the Unity editor.
    /// 
    /// This class serves as the foundation for the component-based architecture of the Rally Protocol SDK.
    /// By extending this class, components automatically gain:
    /// <list type="bullet">
    /// <item><description>Consistent menu placement in the Unity Component menu</description></item>
    /// <item><description>Access to shared naming conventions</description></item>
    /// <item><description>Future common functionality that may be added to all Rally components</description></item>
    /// </list>
    /// 
    /// When creating custom components that integrate with Rally Protocol, it is recommended
    /// to inherit from this class rather than directly from MonoBehaviour.
    /// </remarks>
    /// <example>
    /// Here's an example of creating a custom Rally component:
    /// <code>
    /// using RallyProtocolUnity.Components;
    /// using UnityEngine;
    /// 
    /// [AddComponentMenu(AddComponentMenuNameBase + "/My Custom Rally Component")]
    /// public class MyCustomRallyComponent : RallyBehaviour
    /// {
    ///     // Your custom Rally Protocol integration logic here
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="RallyUnityManager"/>
    /// <seealso cref="RallyClaimRly"/>
    /// <seealso cref="RallyGetBalance"/>
    public class RallyBehaviour : MonoBehaviour
    {
        /// <summary>
        /// The base name for the Add Component menu path in the Unity editor.
        /// Used to group all Rally Protocol components under a common menu.
        /// </summary>
        /// <remarks>
        /// All derived classes should use this constant when specifying their
        /// <see cref="AddComponentMenuAttribute"/> to ensure consistent menu organization.
        /// 
        /// Example usage:
        /// <code>[AddComponentMenu(AddComponentMenuNameBase + "/My Component Name")]</code>
        /// </remarks>
        protected const string AddComponentMenuNameBase = "Rally Protocol";
    }
}
