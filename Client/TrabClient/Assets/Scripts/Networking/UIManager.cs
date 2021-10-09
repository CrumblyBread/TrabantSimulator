using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject startMenu;
    public GameObject loadingScreen;
    public GameObject carUI;
    public TMP_InputField usernameField;
    public TMP_InputField ipField;
    public GameObject questWindow; 
    public TMP_Text questText;
    public GameObject subtitleWindow; 
    public TMP_Text subtitleText;
    [HideInInspector] public float timer;
    [HideInInspector] public bool timeIt;

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
        string favName = PlayerPrefs.GetString("favUsername");
        if(favName != null){
            usernameField.text = favName;
        }
    }

    private void Update() {
        if(timeIt && timer >= 0){
            timer -= Time.deltaTime;
            if(timer <= 0){
                timeIt = false;
                startMenu.SetActive(true);
                usernameField.interactable = true;
                loadingScreen.SetActive(false);
            }
        }
    }

    public void ConnectToServer()
    {
        if(usernameField.text.Length < 4 || usernameField.text.Length >= 13){
            Debug.LogError("Invalid username length (must be from 3 to 12)");
            return;
        }
        timer = 5f;
        timeIt = true;
        PlayerPrefs.SetString("favUsername",usernameField.text);
        if(ipField.text != string.Empty){
            Client.instance.SetIP(ipField.text);
        }else{
            Client.instance.SetIP("127.0.0.1");
        }
        loadingScreen.SetActive(true);
        startMenu.SetActive(false);
        usernameField.interactable = false;
        Client.instance.ConnectToServer();
    }

    public void DrivingUIOn(){
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        carUI.SetActive(true);
    }

    public void DrivingUIOff(){
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        carUI.SetActive(false);
    }

    public void OptionsMenu(){
        usernameField.transform.parent.parent.GetComponent<Animator>().SetTrigger("Options");
    }

    public void StartMenu(){
        usernameField.transform.parent.parent.GetComponent<Animator>().SetTrigger("StartMenu");
    }

    public void QuitGame(){
        Application.Quit();
    }

    public void ShowQuestTag(){
        questWindow.SetActive(true);
    }

    public void HideQuestTag(){
        questWindow.SetActive(false);
    }

    public void Subtitle(string subText){
        subtitleText.text = subText;
        subtitleWindow.SetActive(true);
        Invoke("clearSubtitle", 5f);
    }

    public void Subtitle(string subText, float time){
        subtitleText.text = subText;
        subtitleWindow.SetActive(true);
        Invoke("clearSubtitle", time);
    }

    private void clearSubtitle(){
        subtitleText.text = string.Empty;
        subtitleWindow.SetActive(false);
    }

    public void SetQuestText(string qText){
        questText.text = qText;
    }
}