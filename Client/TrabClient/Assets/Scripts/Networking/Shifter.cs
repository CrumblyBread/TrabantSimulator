using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Shifter : MonoBehaviour, IDropHandler
{

    public RectTransform[] gears;
    public Canvas canvas;

    private void Awake() {
        this.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
    }
    public void OnDrop(PointerEventData eventData){
        if(eventData.pointerDrag != null){
            float smallestDistance = float.PositiveInfinity;
            int closestGear = 0;
            for (int i = 0; i < gears.Length; i++)
            {
                float dist = Mathf.Abs(Vector2.Distance(eventData.pointerDrag.GetComponent<RectTransform>().position, gears[i].position));
                if(dist < smallestDistance){
                    smallestDistance = dist;
                    closestGear = i;   
                }
            }
            GameManager.cars[GameManager.instance.localPlayer.GetComponent<PlayerManager>().driveCarId].shiftSound.Play();
            ClientSend.CarShift(GameManager.instance.localPlayer.GetComponent<PlayerManager>().driveCarId,0,closestGear);
            eventData.pointerDrag.GetComponent<RectTransform>().position = gears[closestGear].position;
        }
    }
}
