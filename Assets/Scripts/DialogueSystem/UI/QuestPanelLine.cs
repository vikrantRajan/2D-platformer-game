using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueQuests
{

    public class QuestPanelLine : MonoBehaviour {

        public Text quest_title;
        public Text quest_text;
        public Image quest_icon;
        public Image quest_completed;

        void Awake()
        {

        }

        private void Update()
        {
            
        }

        public void SetLine(QuestData quest)
        {
            quest_title.text = quest.title;

            if (quest_text != null)
                quest_text.text = quest.desc;

            if (quest_icon != null)
            {
                quest_icon.sprite = quest.icon;
                quest_icon.enabled = quest.icon != null;
            }

            if (quest_completed != null)
            {
                quest_completed.enabled = NarrativeData.Get().IsQuestCompleted(quest.quest_id);
            }

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }

}
