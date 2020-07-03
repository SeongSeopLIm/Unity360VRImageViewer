using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class InputManager : Singleton<InputManager>, IDragHandler
{
    public Action<Vector2> onEnterToutch;
    public Action<Vector2> onEndToutch;
    public Action<Vector2> onDrag;
    public Action onEnterPitchDrag;
    /*Param  -Enter, End, Delta */ 
    public Action<Vector3, Vector3, Vector3> onPitchDrag;


    private Vector2 enterToutchPosition;
    private Vector2 endToutchPosition;
    private Vector2 preToutchPosition;

    bool isPitchDrag;
    bool isDrag; 
    // Update is called once per frame
    void Update()
    { 
        UpdateEvents();
    }
      
    void UpdateEvents()
    { 

        if (Input.GetMouseButtonUp(0))
        {
            OnEndDrag();
            endToutchPosition = Input.mousePosition;
            isDrag = false;
            onEndToutch?.Invoke(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        }


#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        UpdateMoblieInput();
#else
        UpdateWindowInput();
#endif

        if (isDrag && !isPitchDrag)
        {  
            if (onDrag != null)
                onDrag(new Vector2(Input.mousePosition.x, Input.mousePosition.y) - preToutchPosition);
            preToutchPosition = Input.mousePosition;
        }
        if (isPitchDrag)
        {
            OnPitchDrag();
            return;
        } 

    }

    void UpdateWindowInput()
    { 
        if (Input.GetMouseButtonDown(0))
            OnStartDrag();
        if (Input.GetKeyDown(KeyCode.LeftControl))
            OnStartDrag();
    }

    void UpdateMoblieInput()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;


        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            OnStartDrag();
        if (Input.touchCount > 1 && Input.GetTouch(1).phase == TouchPhase.Began)
            OnStartDrag();
         
    }

    public void OnStartDrag()
    {
        if (isDrag)
        { 
            isPitchDrag = true;
            if (onEnterPitchDrag != null)
                onEnterPitchDrag();
        }
        else
        {
            enterToutchPosition = Input.mousePosition;

            if (onEnterToutch != null)
                onEnterToutch(new Vector2(Input.mousePosition.x, Input.mousePosition.y));  
        }

        isDrag = true;
        preToutchPosition = Input.mousePosition;
    }

    public void OnEndDrag()
    {
        isDrag = false;
        isPitchDrag = false;
    }

    public void OnPitchDrag()
    {
        Vector3 deltaPosition;
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        if (Input.touchCount <= 1)
            return;
        deltaPosition = Input.mousePosition - new Vector3(preToutchPosition.x, preToutchPosition.y, 0.0f) ;
        if (onPitchDrag != null)
            onPitchDrag(Input.GetTouch(0).position, Input.GetTouch(1).position, deltaPosition); 
        
#else


        deltaPosition = Input.mousePosition - new Vector3(preToutchPosition.x, preToutchPosition.y, 0.0f) ;
        preToutchPosition = Input.mousePosition;
        if (onPitchDrag != null)
            onPitchDrag(enterToutchPosition, Input.mousePosition, deltaPosition);
#endif
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 deltaPosition = Input.mousePosition - new Vector3(preToutchPosition.x, preToutchPosition.y, 0.0f);

        preToutchPosition = Input.mousePosition;
        if (onDrag != null)
            onDrag(deltaPosition);
    }
}

