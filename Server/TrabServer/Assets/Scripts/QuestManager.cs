using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{   
    public static QuestManager instance;
    public Quest[] quests;

    public GameObject[] questItems;

    private void Awake() {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
        quests = new Quest[3];
        quests[1] = new Quest("MechanicQuest",0,2,false);
        quests[2] = new Quest("LumberJackQuest",0,2,false);

        questItems[0].SetActive(false);
        questItems[1].SetActive(false);
        questItems[2].SetActive(true);
        questItems[3].SetActive(false);
        questItems[4].SetActive(false);
        questItems[5].SetActive(false);
        questItems[6].SetActive(false);
        questItems[7].SetActive(true);
    }

    public void AdvanceQuest(int id){
        if(quests[id].stage == 0){
            ActivateQuest(id);
        }
        quests[id].stage++;
        if(quests[id].stage == quests[id].maxStage){
            quests[id].done = true;
            quests[id].active = false;
            CompleteQuest(id);
        }
        ServerSend.Quest(id,quests[id].stage);
    }

    public void ActivateQuest(int id){
        switch (id)
        {
            case 1:
                quests[1].active = true;
                questItems[0].SetActive(true);
                questItems[1].SetActive(true);
                questItems[2].SetActive(true);
                questItems[3].SetActive(false);
                break;
            case 2:
                quests[2].active = true;
                questItems[4].SetActive(true);
                questItems[5].SetActive(true);
                questItems[6].SetActive(false);
                questItems[7].SetActive(true);
                break;
        }
    }

    public void CompleteQuest(int id){
        switch (id)
        {
            case 1:
                questItems[0].SetActive(false);
                questItems[1].SetActive(false);
                questItems[2].SetActive(false);
                questItems[3].SetActive(true);
                AdvanceQuest(2);
                break;
            case 2:
                questItems[4].SetActive(false);
                questItems[5].SetActive(false);
                questItems[6].SetActive(true);
                questItems[7].SetActive(false);
                break;
            default:
                break;
        }
    }
}
