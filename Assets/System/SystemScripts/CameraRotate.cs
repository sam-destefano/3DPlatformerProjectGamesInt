using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public enum CameraMode { Free, Fixed }
    public CameraMode cameraMode = CameraController.CameraMode.Free;

    [Header("Free Camera Variables")]
    [Tooltip("How far from the player the camera is.")]
    public float cameraDistance = 10;
    private float cameraDistanceInternal = 10;
    public Vector3 cameraOffset = Vector3.zero;
    public LayerMask cameraLayerMask;
    private Transform playerTransform;
    [Tooltip("If the camera has to get closer to the player to not be behind an object, this variable can offset it a little closer/farther.")]
    public float cameraClipOffset = 0;

    [Header("Fixed Camera Variables")]
    public Vector3 targetPosition = Vector3.zero;
    public Quaternion freeTargetRotation = Quaternion.identity;
    public Quaternion fixedTargetRotation = Quaternion.identity;
    private Vector3 positionVelocity;
    private float moveTime = 0.3f;
    private float rotateTime = 25f;
    private float transitionTime = 1f;
    private float rotateStartTime = 0;

    [Header("Camera Zoom")]
    private float originalZoom = 0;
    private float refZoomVelocity;
    [Tooltip("How smoothly the camera zooms in/out")]
    public float cameraZoomSmoothTime = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        targetPosition = transform.localPosition;
        freeTargetRotation = transform.localRotation;
        fixedTargetRotation = transform.localRotation;
        originalZoom = cameraDistance;
        cameraDistance = cameraDistance;
    }

    void Start()
    {
        playerTransform = PlayerController.instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraMode == CameraMode.Free)
        {
            DoFreeCameraMovement();
        }
        else
        {
            DoFixedCameraMovement();
        }

        cameraDistanceInternal = Mathf.SmoothDamp(cameraDistanceInternal, cameraDistance, ref refZoomVelocity, cameraZoomSmoothTime);
    }

    private void DoFixedCameraMovement()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPosition, ref positionVelocity, moveTime);

        if ((Time.time - rotateStartTime) < rotateTime)
        {
            float rotateTimePerc = (Time.time - rotateStartTime) / rotateTime;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, fixedTargetRotation, rotateTimePerc);
        }
        else
        {
            transform.localRotation = fixedTargetRotation;
        }
    }

    private void DoFreeCameraMovement()
    {
        float currentCameraDistance = cameraDistanceInternal;

        if (Physics.Raycast(playerTransform.position, -transform.forward, out RaycastHit hitInfo, cameraDistanceInternal, cameraLayerMask))
        {
            currentCameraDistance = hitInfo.distance - cameraClipOffset;
        }


        if ((Time.time - rotateStartTime) < transitionTime)
        {
            float rotateTimePerc = (Time.time - rotateStartTime) / rotateTime;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, freeTargetRotation, rotateTimePerc);

            Vector3 freeTargetPosition = cameraOffset - (transform.forward * currentCameraDistance);
            transform.localPosition = Vector3.Lerp(transform.localPosition, freeTargetPosition, rotateTimePerc);
        }
        else
        {
            transform.localRotation = freeTargetRotation;

            transform.localPosition = cameraOffset - (transform.forward * currentCameraDistance);
        }
    }

    public void SetTargetPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
    }

    public void SetTargetRotation(Quaternion newRotation)
    {
        fixedTargetRotation = newRotation;
        rotateStartTime = Time.time;
    }

    public void SetPlayerCameraAngle(Quaternion newRotation)
    {
        freeTargetRotation = newRotation;
    }

    public void SetCameraModeToFree()
    {
        cameraMode = CameraMode.Free;
        rotateStartTime = Time.time;
    }

    public void SetCameraModeToFixed()
    {
        cameraMode = CameraMode.Fixed;
    }

    public void SetCameraZoom(float amount)
    {
        cameraDistance = amount;
    }

    public void ResetCameraZoom()
    {
        cameraDistance = originalZoom;
    }

    public void SetZoomSmoothTime(float amount)
    {
        cameraZoomSmoothTime = amount;
    }
}
