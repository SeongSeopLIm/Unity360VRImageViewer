using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRInteriorPlayerControlelr : PlayerController
{

    [SerializeField] private GameObject CameraYRoot;
    [SerializeField] private Vector2 CameraRotateSpeed;
    [SerializeField] private Vector2 CameraXRotClamp;
    [SerializeField] private Vector2 CameraGrowClamp;
    [SerializeField] private float GorwSpeed;
    private Quaternion PreYRotation;
    private Quaternion PreXRotation;
    private Quaternion CurYRotation;
    private Quaternion CurXRotation;
    private Vector3 DeltaRot;
    private Vector3 slideDeltaRot;
    private float slideAlpha;
    private InputManager inputManager => InputManager.Instance;
    float ImageDeltaCount;
    bool isSliding;

    public float SlideSpeed = 10.0f;
    
    public float preLength { get; private set; }

    private void Awake()
    {
        Enable();
    }

    private void OnDestroy()
    {
        Disable();
    }

    private void Update()
    {
        UpdateSlide();
    }

    void UpdateSlide()
    {
        if (!isSliding)
            return;
        PreYRotation = CurYRotation;
        PreXRotation = CurXRotation;
        CurYRotation = CameraYRoot.transform.localRotation ;
        CurXRotation = playerCamera.transform.localRotation;

        Quaternion deltaRotation = CurYRotation * Quaternion.Inverse(PreYRotation);
        DeltaRot = deltaRotation.eulerAngles;
        deltaRotation = CurXRotation * Quaternion.Inverse(PreXRotation);
        DeltaRot += deltaRotation.eulerAngles;

        DeltaRot.x = (DeltaRot.x % 360);
        DeltaRot.y = DeltaRot.y % 360;
        DeltaRot.z = DeltaRot.z % 360;

        DeltaRot.x = DeltaRot.x > 180 ? DeltaRot.x - 360 : DeltaRot.x;
        DeltaRot.y = DeltaRot.y > 180 ? DeltaRot.y - 360 : DeltaRot.y;
        DeltaRot.z = DeltaRot.z > 180 ? DeltaRot.z - 360 : DeltaRot.z;
         

        slideAlpha = Mathf.Lerp(slideAlpha, 0, Time.deltaTime * SlideSpeed);
        slideDeltaRot = Vector3.Lerp(slideDeltaRot, Vector3.zero, Time.deltaTime * SlideSpeed);
        CameraYRoot.transform.Rotate(new Vector3(0.0f, slideDeltaRot.y, 0.0f));
        playerCamera.transform.Rotate(new Vector3(slideDeltaRot.x, 0.0f, 0.0f));

    }

    public override void Enable()
    {
        base.Enable();  
        inputManager.onEnterToutch += OnEnterTouch;
        inputManager.onEndToutch += OnEndTouch;
        inputManager.onDrag += OnDrag;
        inputManager.onPitchDrag += OnPitchDrag;
        inputManager.onEnterPitchDrag += OnEnterDoubleDrag; 
    }

    public override void Disable()
    {
        base.Disable();   
        if (inputManager.onEnterToutch == OnEnterTouch)
            inputManager.onEnterToutch -= OnEnterTouch;
        if (inputManager.onEndToutch == OnEndTouch)
            inputManager.onEndToutch -= OnEndTouch;
        if (inputManager.onDrag == OnDrag)
            inputManager.onDrag -= OnDrag;
        if (inputManager.onPitchDrag == OnPitchDrag)
            inputManager.onPitchDrag -= OnPitchDrag;
        if (inputManager.onEnterPitchDrag == OnEnterDoubleDrag)
            inputManager.onEnterPitchDrag -= OnEnterDoubleDrag;
    }

    public void ResetCamera()
    {
        playerCamera.transform.rotation = Quaternion.identity;
        CameraYRoot.transform.rotation = Quaternion.identity;
    }

    public void StartSlide()
    {

    } 

    #region Input

    void OnEnterTouch(Vector2 screenPosition)
    {
        slideAlpha = 0.0f;
        isSliding = false;
        slideDeltaRot = Vector3.zero;
    }

    void OnEndTouch(Vector2 screenPosition)
    {
        slideAlpha = 1.0f;
        isSliding = true;
        slideDeltaRot = DeltaRot;
    }

    void OnDrag(Vector2 deltaScale)
    {
        playerCamera.transform.Rotate(new Vector3(deltaScale.y * CameraRotateSpeed.y, 0.0f, 0.0f));
        if (playerCamera.transform.eulerAngles.x < CameraXRotClamp.x && playerCamera.transform.eulerAngles.x > CameraXRotClamp.y)
        {
            float aDelta = Mathf.Abs( playerCamera.transform.eulerAngles.x - CameraXRotClamp.x);
            float bDelta = Mathf.Abs(playerCamera.transform.eulerAngles.x - CameraXRotClamp.y);
            if(aDelta < bDelta)
                playerCamera.transform.localEulerAngles = new Vector3(CameraXRotClamp.x, 0.0f, 0.0f);
            else
                playerCamera.transform.localEulerAngles = new Vector3(CameraXRotClamp.y, 0.0f, 0.0f);
        }
        

        CameraYRoot.transform.Rotate(new Vector3(0.0f, deltaScale.x * CameraRotateSpeed.x, 0.0f));
    }

    void OnEnterDoubleDrag()
    {
        preLength = -1;
    }

    void OnPitchDrag(Vector3 EnterPosition, Vector3 EndPosition, Vector3 DeltaPosition)
    { 
        float newLength = Vector3.Distance(EnterPosition, EndPosition);
        if (preLength == -1)
            preLength = newLength;
        float DeltaLength = newLength - preLength;
        float FOVDelta = DeltaLength * GorwSpeed;
        float clamped = playerCamera.fieldOfView - FOVDelta;
        preLength = newLength;

        clamped = Mathf.Clamp(clamped, CameraGrowClamp.x, CameraGrowClamp.y);
        playerCamera.fieldOfView = clamped;

    }

    #endregion

}
