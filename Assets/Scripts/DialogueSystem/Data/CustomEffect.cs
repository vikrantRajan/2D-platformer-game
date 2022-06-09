using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueQuests
{
    /// <summary>
    /// Base class for creating custom effects, inherit from this class, add the [CreateAssetMenu()] tag
    /// And then create a data file based on this script, the DoEffect() function will be called when resolving the effect
    /// </summary>
    /// 
    public class CustomEffect : ScriptableObject
    {
        public virtual void DoEffect(Actor triggerer) {
            
        }
    }

}
