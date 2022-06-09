using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueQuests
{
    [CreateAssetMenu(fileName ="Quest", menuName = "DialogueQuests/Quest", order= 0)]
    public class QuestData : ScriptableObject {

        [Tooltip("Important: make sure all quests have a unique ID")]
        public string quest_id;

        public string title;
        public Sprite icon;
        [TextArea(3, 5)]
        public string desc;
        public int sort_order;

        public static void Load(QuestData quest)
        {
            if (NarrativeManager.Get())
            {
                List<QuestData> list = GetAll();
                if (!list.Contains(quest))
                {
                    list.Add(quest);
                }
            }
        }

        public static QuestData Get(string actor_id)
        {
            if (NarrativeManager.Get())
            {
                foreach (QuestData quest in GetAll())
                {
                    if (quest.quest_id == actor_id)
                        return quest;
                }
            }
            return null;
        }

        public static List<QuestData> GetAllActive()
        {
            List<QuestData> valid_list = new List<QuestData>();
            foreach (QuestData aquest in GetAll())
            {
                if (NarrativeData.Get().IsQuestActive(aquest.quest_id))
                    valid_list.Add(aquest);
            }
            return valid_list;
        }

        public static List<QuestData> GetAllActiveOrCompleted()
        {
            List<QuestData> valid_list = new List<QuestData>();
            foreach (QuestData aquest in GetAll())
            {
                if (NarrativeData.Get().IsQuestActive(aquest.quest_id) || NarrativeData.Get().IsQuestCompleted(aquest.quest_id))
                    valid_list.Add(aquest);
            }
            return valid_list;
        }

        public static List<QuestData> GetAll()
        {
            return NarrativeManager.Get().quest_list;
        }
    }
}
