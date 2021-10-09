using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Canvas canvas;
    private RectTransform rTransform;
    private CanvasGroup group;

    private void Awake() {
        rTransform = this.GetComponent<RectTransform>();   
        group = this.GetComponent<CanvasGroup>(); 
    }
    public void OnPointerDown(PointerEventData eventData){
    }

    public void OnBeginDrag(PointerEventData eventData){
       group.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData){
       group.blocksRaycasts = true;
       Cursor.visible = true;
       group.alpha = 1f;

       if(eventData.pointerCurrentRaycast.gameObject == null){
           this.transform.parent.GetComponentInChildren<Shifter>().OnDrop(eventData);
       }
    }

    public void OnDrag(PointerEventData eventData){
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        group.alpha = 1f;
        if(eventData.pointerCurrentRaycast.gameObject == null){
            return;
        }
        if(eventData.pointerCurrentRaycast.gameObject.transform.GetComponent<Shifter>() != null){
            group.alpha = 0.7f;
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform, Input.mousePosition,
            canvas.worldCamera,
            out pos);

            rTransform.anchoredPosition = pos;
        }
    }
}
