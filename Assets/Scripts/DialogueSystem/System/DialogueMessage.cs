using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DialogueQuests
{
    public enum DialogueMessageType
    {
        DialoguePanel=0, //Regular dialogue panel
        InGame=5, //In-game dialogue
    }

    public class DialogueMessage : MonoBehaviour
    {
        public ActorData actor;

        [TextArea(3, 10)]
        public string text;

        public AudioClip audio_clip = null;
        public int side = 0;

        [Tooltip("For in-game dialogues: time dialogue is shown")]
        [HideInInspector]
        public float duration = 4f;
        [Tooltip("For in-game dialogues: time of the pause between this dialogue and the next one")]
        [HideInInspector]
        public float pause = 0f;

        public UnityAction onStart;
        public UnityAction onEnd;
    }

}