using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueQuests
{
    /// <summary>
    /// Base class for creating custom conditions, inherit from this class, add the [CreateAssetMenu()] tag
    /// And then create a data file based on this script, the IsMet() function will be called when checking the condition
    /// </summary>

    public class CustomCondition : ScriptableObject
    {
        public virtual bool IsMet(Actor triggerer) {
            return true; //Default value
        }
    }

}
