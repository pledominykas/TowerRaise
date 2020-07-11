using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public const float DEFAULT_CAM_FOV = 45f;
    private const float LEVEL_LOAD_ANIMATION_SPEED = 0.1f;
    private const float LEVEL_OVERVIEW_TIME = 0.5f;
    private const float CAMERA_SIZE_MULTIPLIER = 7.5f;
    private const float CAMERA_X_OFFSET_MULTIPLIER = 0.005f;

    private Vector3 PositionOffset = new Vector3(0f, 2f, -14f);
    private Vector3 CameraRotation = new Vector3(35f, -45f, 0f);
    private float SmoothTime = 0.5f;
    private Camera cam;

    private Vector3 targetPos;
    private Vector3 velPos;
    private float targetSize;
    private float velSize;


    private void Start()
    {
        cam = transform.GetChild(0).GetComponent<Camera>();
    }

    public void MoveCamera(Tower _currentTower, Tower _nextTower)
    {
        transform.localEulerAngles = CameraRotation;
        Bounds currB = _currentTower.GetTowerBounds();
        Bounds nextB = _nextTower.GetTowerBounds();
        Vector3 MinPoint;
        Vector3 MaxPoint;
        if (currB.min.z < nextB.min.z)
        {
            MinPoint = currB.min;
            MaxPoint = nextB.max;
        }
        else
        {
            MinPoint = nextB.min;
            MaxPoint = currB.max;
        }
        MinPoint = new Vector3(MinPoint.x, _currentTower.GetTowerPosition().y, MinPoint.z);
        MaxPoint = new Vector3(MaxPoint.x, _nextTower.GetTowerPosition().y, MaxPoint.z);

        Vector2 camFwd = new Vector2(transform.forward.x, transform.forward.z);
        Vector2 Twr = new Vector2(MinPoint.x, MinPoint.z) - new Vector2(MaxPoint.x, MaxPoint.z);
        float beta = Vector2.Angle(camFwd, Twr);
        float l = (Twr.magnitude * Mathf.Sin(beta * Mathf.Deg2Rad));
        l = l * CAMERA_SIZE_MULTIPLIER;
        targetSize = Mathf.Clamp(l, DEFAULT_CAM_FOV, 90f);
        StartCoroutine(Resize(SmoothTime));

        Vector3 midPoint = (MinPoint + MaxPoint) * 0.5f;
        transform.localEulerAngles = CameraRotation;
        targetPos = midPoint + transform.TransformDirection(PositionOffset + Vector3.left * l * CAMERA_X_OFFSET_MULTIPLIER);
        StartCoroutine(Move(SmoothTime));

    }
    public void MoveCamera(Vector3 _tower)
    {
        Vector3 midPoint = _tower;
        transform.localEulerAngles = CameraRotation;
        targetPos = midPoint + transform.TransformDirection(PositionOffset);
        StartCoroutine(Move(SmoothTime));
    }
    public void MoveCamera(Vector3 _tower, float _smoothTime)
    {
        Vector3 midPoint = _tower;
        transform.localEulerAngles = CameraRotation;
        targetPos = midPoint + transform.TransformDirection(PositionOffset);
        StartCoroutine(Move(_smoothTime));
    }

    public void SetCameraPosition(Tower _currentTower, Tower _nextTower)
    {
        transform.localEulerAngles = CameraRotation;
        Bounds currB = _currentTower.GetTowerBounds();
        Bounds nextB = _nextTower.GetTowerBounds();
        Vector3 MinPoint;
        Vector3 MaxPoint;
        if (currB.min.z < nextB.min.z)
        {
            MinPoint = currB.min;
            MaxPoint = nextB.max;
        }
        else
        {
            MinPoint = nextB.min;
            MaxPoint = currB.max;
        }
        MinPoint = new Vector3(MinPoint.x, _currentTower.GetTowerPosition().y, MinPoint.z);
        MaxPoint = new Vector3(MaxPoint.x, _nextTower.GetTowerPosition().y, MaxPoint.z);

        Vector2 camFwd = new Vector2(transform.forward.x, transform.forward.z);
        Vector2 Twr = new Vector2(MinPoint.x, MinPoint.z) - new Vector2(MaxPoint.x, MaxPoint.z);
        float beta = Vector2.Angle(camFwd, Twr);
        float l = (Twr.magnitude * Mathf.Sin(beta * Mathf.Deg2Rad));
        l = l * CAMERA_SIZE_MULTIPLIER;
        targetSize = Mathf.Clamp(l, DEFAULT_CAM_FOV, 90f);
        cam.fieldOfView = targetSize;

        Vector3 midPoint = (MinPoint + MaxPoint) * 0.5f;
        transform.localEulerAngles = CameraRotation;
        targetPos = midPoint + transform.TransformDirection(PositionOffset + Vector3.left * l * CAMERA_X_OFFSET_MULTIPLIER);
        transform.position = targetPos;
        velPos = Vector3.zero;
    }
    public void SetCameraPosition(Vector3 _tower)
    {
        transform.localEulerAngles = CameraRotation;
        Vector3 midPoint = _tower;
        transform.position = midPoint + transform.TransformDirection(PositionOffset);
        targetPos = _tower + transform.TransformDirection(PositionOffset);
        velPos = Vector3.zero;
    }
    public void SetCameraPositionAndRotation(Vector3 _position, Quaternion _rotation)
    {
        transform.position = _position;
        transform.rotation = _rotation;
    }

    public void ResetCameraSize()
    {
        cam.fieldOfView = DEFAULT_CAM_FOV;
    }

    private IEnumerator Move(float _smoothTime)
    {
        for (float i = 0; i < Mathf.Infinity; i += Time.deltaTime)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velPos, _smoothTime);
            if (Vector3.SqrMagnitude(transform.position - targetPos) <= 0.001f)
            {
                break;
            }
            yield return null;
        }
    }

    public void ResizeCamera(float _fov)
    {
        targetSize = _fov;
        Resize(SmoothTime);
    }

    private IEnumerator Resize(float _smoothTime)
    {
        for (float i = 0; i < Mathf.Infinity; i += Time.deltaTime)
        {
            cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, targetSize, ref velSize, _smoothTime);
            if (Mathf.Abs(targetSize - cam.fieldOfView) <= 0.05f)
            {
                break;
            }
            yield return null;
        }
    }

    public void OnLevelCompleteAnimation()
    {
        StartCoroutine(LevelCompleteAnimation());
    }

    private IEnumerator LevelCompleteAnimation()
    {
        StopCoroutine(Move(0f));
        StopCoroutine(Resize(0f));
        Transform targetTransform = GameController.tc.GetCameraTargetTransform();
        for (float i = 0f; i < 1f; i += Time.deltaTime * LEVEL_LOAD_ANIMATION_SPEED)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetTransform.rotation, i);
            transform.position = Vector3.Lerp(transform.position, targetTransform.position, i);
            if (Vector3.SqrMagnitude(transform.position - targetTransform.position) <= 0.05f) { break; }
            yield return null;
        }
        yield return new WaitForSeconds(LEVEL_OVERVIEW_TIME);
        GameController.gc.OnLevelCompleteAnimationOver();
    }
}
