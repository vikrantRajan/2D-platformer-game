using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueQuests
{

    public class QuestPanel : UIPanel {

        public QuestPanelLine[] lines;

        private static QuestPanel _instance;

        protected override void Awake()
        {
            base.Awake();
            _instance = this;
        }

        protected override void Start()
        {
            base.Start();

            if (NarrativeControls.Get())
            {
                NarrativeControls.Get().onPressJournal += TogglePanel;
            }
        }

        protected override void Update () {

            base.Update();
        }

        private void RefreshPanel()
        {
            foreach (QuestPanelLine line in lines)
                line.Hide();

            List<QuestData> all_quest = QuestData.GetAllActiveOrCompleted();

            all_quest.Sort((p1, p2) =>
            {
                return (p1.sort_order == p2.sort_order)
                    ? p1.title.CompareTo(p2.title) : p1.sort_order.CompareTo(p2.sort_order);
            });

            for (int i = 0; i < all_quest.Count; i++)
            {
                if (i < lines.Length)
                {
                    QuestPanelLine line = lines[i];
                    QuestData quest = all_quest[i];
                    line.SetLine(quest);
                }
            }
        }

        public void ShowPanel()
        {
            RefreshPanel();
            Show();
        }

        public void TogglePanel()
        {
            if (IsVisible())
                Hide();
            else
                ShowPanel();
        }

        public static QuestPanel Get()
        {
            return _instance;
        }
    }

}
