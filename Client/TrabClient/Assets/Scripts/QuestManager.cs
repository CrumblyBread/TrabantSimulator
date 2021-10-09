using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{

    public static QuestManager instance;

    public GameObject mechanicObject;
    public GameObject lumberjackObject;
    public GameObject[] questItems;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }

        questItems[0].SetActive(false);
        questItems[1].SetActive(false);
        questItems[2].SetActive(true);
        questItems[3].SetActive(false);
        questItems[4].SetActive(false);
        questItems[5].SetActive(false);
        questItems[6].SetActive(false);
        questItems[7].SetActive(true);
    }

    public void MechanicQuestStage(int stage){
        switch (stage)
        {
            case 1:
                questItems[0].SetActive(true);
                questItems[1].SetActive(true);
                questItems[2].SetActive(true);
                questItems[3].SetActive(false);
                mechanicObject.GetComponentInChildren<AudioSource>().Play();
                UIManager.instance.Subtitle("Mechanic: You guy's look capable, can you haul my brothers bags into the forest for me?");
                UIManager.instance.SetQuestText("Carry the two LumberJack's bags to his forest cabin");
                UIManager.instance.ShowQuestTag();
                break;
            case 2:
                questItems[0].SetActive(false);
                questItems[1].SetActive(false);
                questItems[2].SetActive(false);
                questItems[3].SetActive(true);
                LumberJackQuestStage(1);
                break;
            default:
                break;
        }
    }

    public void LumberJackQuestStage(int stage){
        switch (stage)
        {
            case 1:
                questItems[4].SetActive(true);
                questItems[5].SetActive(true);
                questItems[6].SetActive(false);
                questItems[7].SetActive(true);
                lumberjackObject.GetComponentInChildren<AudioSource>().Play();
                UIManager.instance.Subtitle("Lumberjack: Many thanks! Now you can bring him back these logs, okay?");
                UIManager.instance.SetQuestText("Bring Lumberjack's wood to the Mechanic");
                UIManager.instance.ShowQuestTag();
                break;
            case 2:
                questItems[4].SetActive(false);
                questItems[5].SetActive(false);
                questItems[6].SetActive(true);
                questItems[7].SetActive(false);
                mechanicObject.GetComponentInChildren<AudioSource>().Play();
                UIManager.instance.Subtitle("Mechanic: You did it, I didn't expext that to be honest");
                UIManager.instance.SetQuestText(string.Empty);
                UIManager.instance.HideQuestTag();
                break;
            default:
                break;
        }
    }
}
