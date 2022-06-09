using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DialogueQuests
{
    public enum NarrativeEffectType
    {
        None = 0,
        CustomInt = 1,
        CustomFloat = 2,
        CustomString = 3,

        Show = 10,
        Hide = 11,
        Spawn = 15,
        Destroy = 16,

        StartEvent = 20,
        StartEventIfMet = 21,

        ActorRelation=25,

        StartQuest = 30,
        CancelQuest = 31,
        CompleteQuest = 32,
        FailQuest = 33,

        PlaySFX=40,
        PlayMusic=42,
        StopMusic=44,

        Wait = 95,
        CustomEffect = 97,
        CallFunction = 99,
    }

    public enum NarrativeEffectOperator
    {
        Add = 0,
        Set = 1,
    }

    [System.Serializable]
    public class NarrativeEffectCallback : UnityEvent<int> { }

    public class NarrativeEffect : MonoBehaviour
    {

        public NarrativeEffectType type;
        public string target_id = "";
        public NarrativeEffectOperator oper;
        public GameObject value_object;
        public ScriptableObject value_data;
        public AudioClip value_audio;
        public CustomEffect value_custom;
        public int value_int = 0;
        public float value_float = 1f;
        public string value_string = "";

        [SerializeField]
        public UnityEvent callfunc_evt;

        private void Start()
        {
            if (value_data != null)
            {
                if (value_data is ActorData)
                    ActorData.Load((ActorData)value_data);
                if (value_data is QuestData)
                    QuestData.Load((QuestData)value_data);
            }
        }

        public void Trigger(Actor triggerer)
        {
            if (type == NarrativeEffectType.CustomInt)
            {
                if(oper == NarrativeEffectOperator.Set)
                    NarrativeData.Get().SetCustomInt(target_id, value_int);

                if (oper == NarrativeEffectOperator.Add)
                {
                    int value = NarrativeData.Get().GetCustomInt(target_id);
                    NarrativeData.Get().SetCustomInt(target_id, value + value_int);
                }
            }

            if (type == NarrativeEffectType.CustomFloat)
            {
                if (oper == NarrativeEffectOperator.Set)
                    NarrativeData.Get().SetCustomFloat(target_id, value_float);

                if (oper == NarrativeEffectOperator.Add)
                {
                    float value = NarrativeData.Get().GetCustomFloat(target_id);
                    NarrativeData.Get().SetCustomFloat(target_id, value + value_float);
                }
            }

            if (type == NarrativeEffectType.CustomString)
            {
                NarrativeData.Get().SetCustomString(target_id, value_string);
            }

            if (type == NarrativeEffectType.Show)
            {
                GameObject targ = value_object;
                if (targ)
                    targ.SetActive(true);
            }

            if (type == NarrativeEffectType.Hide)
            {
                GameObject targ = value_object;
                if (targ)
                    targ.SetActive(false);
            }

            if (type == NarrativeEffectType.Spawn)
            {
                if (value_object != null && !string.IsNullOrEmpty(target_id))
                {
                    Region region = Region.Get(target_id);
                    if(region != null)
                        Instantiate(value_object, region.transform.position, Quaternion.identity);
                }
            }

            if (type == NarrativeEffectType.Destroy)
            {
                GameObject targ = value_object;
                Destroy(targ);
            }

            if (type == NarrativeEffectType.StartEvent)
            {
                GameObject targ = value_object;
                if (targ && targ.GetComponent<NarrativeEvent>())
                {
                    NarrativeManager.Get().StartEvent(targ.GetComponent<NarrativeEvent>());
                }
            }

            if (type == NarrativeEffectType.StartEventIfMet)
            {
                GameObject targ = value_object;
                if (targ && targ.GetComponent<NarrativeEvent>())
                {
                    if (targ.GetComponent<NarrativeEvent>().AreConditionsMet())
                        NarrativeManager.Get().StartEvent(targ.GetComponent<NarrativeEvent>());
                }
            }

            if (type == NarrativeEffectType.ActorRelation)
            {
                ActorData actor = (ActorData)value_data;
                if (actor != null)
                {
                    if (oper == NarrativeEffectOperator.Set)
                        NarrativeData.Get().SetActorValue(actor, value_int);

                    if (oper == NarrativeEffectOperator.Add)
                    {
                        int value = NarrativeData.Get().GetCustomInt(target_id);
                        NarrativeData.Get().SetActorValue(actor, value + value_int);
                    }
                }
            }

            if (type == NarrativeEffectType.StartQuest)
            {
                QuestData quest = (QuestData)value_data;

                NarrativeManager.Get().StartQuest(quest);
            }

            if (type == NarrativeEffectType.CancelQuest)
            {
                QuestData quest = (QuestData) value_data;
                NarrativeManager.Get().CancelQuest(quest);
            }

            if (type == NarrativeEffectType.CompleteQuest)
            {
                QuestData quest = (QuestData) value_data;
                NarrativeManager.Get().CompleteQuest(quest);
            }

            if (type == NarrativeEffectType.FailQuest)
            {
                QuestData quest = (QuestData)value_data;
                NarrativeManager.Get().FailQuest(quest);
            }

            if (type == NarrativeEffectType.PlaySFX)
            {
                NarrativeManager.Get().PlaySFX(value_string, value_audio, value_float);
            }

            if (type == NarrativeEffectType.PlayMusic)
            {
                NarrativeManager.Get().PlayMusic(value_string, value_audio, value_float);
            }

            if (type == NarrativeEffectType.StopMusic)
            {
                NarrativeManager.Get().StopMusic(value_string);
            }

            if (type == NarrativeEffectType.CustomEffect)
            {
                if (value_custom != null)
                {
                    value_custom.DoEffect(triggerer);
                }
            }

            if (type == NarrativeEffectType.CallFunction)
            {
                if (callfunc_evt != null)
                {
                    callfunc_evt.Invoke();
                }
            }

        }

        public float GetWaitTime()
        {
            if (type == NarrativeEffectType.Wait)
            {
                return value_float;
            }
            return 0f;
        }
    }

}