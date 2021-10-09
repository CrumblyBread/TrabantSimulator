using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    
   public string questName;
   public QuestType type;
   public bool active;
   public int stage;
   public int maxStage;
   public bool done;

   public Quest(string _name, int _type, int _maxStage) {
        questName = _name;
        type = (QuestType)_type;
        stage = 0;
        maxStage = _maxStage;
   }

   public Quest(string _name, int _type, int _maxStage , bool _active) {
        questName = _name;
        type = (QuestType)_type;
        stage = 0;
        maxStage = _maxStage;
        active = _active;
   }
}

public enum QuestType{
    fetch,
}
