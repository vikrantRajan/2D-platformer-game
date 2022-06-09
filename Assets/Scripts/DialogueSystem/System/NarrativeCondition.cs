using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DialogueQuests
{
    public enum NarrativeConditionType
    {
        None = 0,
        CustomInt = 1,
        CustomFloat = 2,
        CustomString = 3,

        IsVisible = 10,
        InsideRegion = 12,

        EventTriggered = 20,

        ActorRelation=25,

        QuestStarted =30, //Either active or completed
        QuestActive = 32, //Started but not completed
        QuestCompleted =34, //Quest is completed
        QuestFailed =35, //Quest is failed

        CustomCondition=99,
    }

    public enum NarrativeConditionOperator
    {
        Equal = 0,
        NotEqual = 1,
        GreaterEqual = 2,
        LessEqual = 3,
        Greater = 4,
        Less = 5,
    }

    public enum NarrativeConditionOperator2
    {
        IsTrue = 0,
        IsFalse = 1,
    }

    public enum NarrativeConditionTargetType
    {
        Value = 0,
        Target = 1,
    }
    
    public class NarrativeCondition : MonoBehaviour
    {
        public NarrativeConditionType type;
        public NarrativeConditionOperator oper;
        public NarrativeConditionOperator2 oper2;
        public NarrativeConditionTargetType target_type;
        public string target_id = "";
        public string other_target_id;
        public GameObject value_object;
        public ScriptableObject value_data;
        public CustomCondition value_custom;
        public int value_int = 0;
        public float value_float = 0f;
        public string value_string = "";

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

        public bool IsMet(Actor triggerer)
        {
            bool condition_met = false;

            if (type == NarrativeConditionType.None)
            {
                condition_met = true;
                if (oper2 == NarrativeConditionOperator2.IsFalse)
                    condition_met = !condition_met;
            }

            if (type == NarrativeConditionType.CustomInt)
            {
                int i1 = NarrativeData.Get().GetCustomInt(target_id);
                int i2 = target_type == NarrativeConditionTargetType.Target ? NarrativeData.Get().GetCustomInt(other_target_id) : value_int;
                condition_met = CompareInt(i1, i2);
            }

            if (type == NarrativeConditionType.CustomFloat)
            {
                float f1 = NarrativeData.Get().GetCustomFloat(target_id);
                float f2 = target_type == NarrativeConditionTargetType.Target ? NarrativeData.Get().GetCustomFloat(other_target_id) : value_float;
                condition_met = CompareFloat(f1, f2);
            }

            if (type == NarrativeConditionType.CustomString)
            {
                string s1 = NarrativeData.Get().GetCustomString(target_id);
                string s2 = target_type == NarrativeConditionTargetType.Target ? NarrativeData.Get().GetCustomString(other_target_id) : value_string;
                condition_met = CompareString(s1, s2);
            }

            if (type == NarrativeConditionType.IsVisible)
            {
                GameObject targ = value_object;
                condition_met = (targ != null && targ.activeSelf);
                if (targ != null)
                {
                    condition_met = targ.activeSelf;
                }
                if (oper2 == NarrativeConditionOperator2.IsFalse)
                {
                    condition_met = !condition_met;
                }
            }
            
            if (type == NarrativeConditionType.InsideRegion)
            {
                ActorData adata = (ActorData)value_data;
                if (adata)
                {
                    Actor actor = Actor.Get(adata);
                    Region region = Region.Get(target_id);
                    if (actor && region)
                        condition_met = region.IsInsideXZ(actor.transform.position);
                }
                if (oper2 == NarrativeConditionOperator2.IsFalse)
                    condition_met = !condition_met;
            }
            
            if (type == NarrativeConditionType.QuestStarted)
            {
                QuestData quest = (QuestData)value_data;
                if (quest != null){
                    condition_met = NarrativeData.Get().IsQuestStarted(quest.quest_id);
                    if (oper2 == NarrativeConditionOperator2.IsFalse)
                        condition_met = !condition_met;
                }
            }

            if (type == NarrativeConditionType.QuestActive)
            {
                QuestData quest = (QuestData)value_data;
                if (quest != null)
                {
                    condition_met = NarrativeData.Get().IsQuestActive(quest.quest_id);
                    if (oper2 == NarrativeConditionOperator2.IsFalse)
                        condition_met = !condition_met;
                }
            }

            if (type == NarrativeConditionType.QuestCompleted)
            {
                QuestData quest = (QuestData)value_data;
                if (quest != null)
                {
                    condition_met = NarrativeData.Get().IsQuestCompleted(quest.quest_id);
                    if (oper2 == NarrativeConditionOperator2.IsFalse)
                        condition_met = !condition_met;
                }
            }

            if (type == NarrativeConditionType.QuestFailed)
            {
                QuestData quest = (QuestData)value_data;
                if (quest != null)
                {
                    condition_met = NarrativeData.Get().IsQuestFailed(quest.quest_id);
                    if (oper2 == NarrativeConditionOperator2.IsFalse)
                        condition_met = !condition_met;
                }
            }

            if (type == NarrativeConditionType.EventTriggered)
            {
                GameObject targ = value_object;
                if (targ && targ.GetComponent<NarrativeEvent>())
                {
                    NarrativeEvent evt = targ.GetComponent<NarrativeEvent>();
                    condition_met = evt.GetTriggerCount() >= 1;
                    if (oper2 == NarrativeConditionOperator2.IsFalse)
                        condition_met = !condition_met;
                }
            }

            if (type == NarrativeConditionType.ActorRelation)
            {
                ActorData actor = (ActorData)value_data;
                if (actor != null)
                {
                    int avalue = NarrativeData.Get().GetActorValue(actor);
                    condition_met = CompareInt(avalue, value_int);
                }
            }

            if (type == NarrativeConditionType.CustomCondition)
            {
                if (value_custom != null)
                {
                    condition_met = value_custom.IsMet(triggerer);
                    if (oper2 == NarrativeConditionOperator2.IsFalse)
                        condition_met = !condition_met;
                }
            }

            return condition_met;
        }

        public bool CompareInt(int ival1, int ival2)
        {
            bool condition_met = true;
            if (oper == NarrativeConditionOperator.Equal && ival1 != ival2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.NotEqual && ival1 == ival2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.GreaterEqual && ival1 < ival2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.LessEqual && ival1 > ival2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.Greater && ival1 <= ival2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.Less && ival1 >= ival2)
            {
                condition_met = false;
            }
            return condition_met;
        }

        public bool CompareFloat(float fval1, float fval2)
        {
            bool condition_met = true;
            if (oper == NarrativeConditionOperator.Equal && fval1 != fval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.NotEqual && fval1 == fval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.GreaterEqual && fval1 < fval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.LessEqual && fval1 > fval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.Greater && fval1 <= fval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.Less && fval1 >= fval2)
            {
                condition_met = false;
            }
            return condition_met;
        }

        public bool CompareString(string sval1, string sval2)
        {
            bool condition_met = true;
            if (oper == NarrativeConditionOperator.Equal && sval1 != sval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.NotEqual && sval1 == sval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.GreaterEqual && sval1 != sval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.LessEqual && sval1 != sval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.Greater && sval1 == sval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.Less && sval1 == sval2)
            {
                condition_met = false;
            }
            return condition_met;
        }
    }

}
