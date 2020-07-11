using UnityEngine;
using System.Collections.Generic;

public class LevelSceneManager : MonoBehaviour
{
    private const float MIN_CAMERA_Z_POSITION = -18f;
    private const float MIN_CAMERA_X_POSITION = -5f;
    private const float MAX_CAMERA_X_POSITION = 5f;
    private const float MOVEMENT_SENSITIVITY = 140f;
    private const float SMOOTH_TIME = 0.1f;
    private const float FORWARD_PEEK_DISTANCE = 15f;

    [SerializeField] Material CurrentLevelMaterial;
    [SerializeField] Material PassedLevelMaterial;
    [SerializeField] List<MeshRenderer> LevelTowers = new List<MeshRenderer>();
    [SerializeField] List<float> CameraZClamp = new List<float>();
    private Vector3 CameraPosition = new Vector3(0f, 35f, -15f);
    private Quaternion CameraRotation = new Quaternion(0.5f, 0.0f, 0.0f, 0.9f);

    private Vector3 origin;
    private Vector3 offset;
    private Vector3 cameraOrigin;
    private Vector3 velocity;
    private float currentCameraZClamp;
    private float zVelocity = 0f;

    private void Awake()
    {
        SetUpLevelScene();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            origin = Input.mousePosition / Screen.height;
            cameraOrigin = GameController.cc.transform.position;
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            offset = origin - (Input.mousePosition / Screen.height);
        }
    }

    private void LateUpdate()
    {
        float cameraZPos = cameraOrigin.z + offset.y * MOVEMENT_SENSITIVITY;
        cameraZPos = Mathf.Clamp(cameraZPos, MIN_CAMERA_Z_POSITION, currentCameraZClamp + FORWARD_PEEK_DISTANCE);
        if (cameraZPos > currentCameraZClamp)
        {
            offset = new Vector2(offset.x, 0f);
            cameraZPos = Mathf.SmoothDamp(cameraZPos, currentCameraZClamp, ref zVelocity, SMOOTH_TIME);
            cameraOrigin = new Vector3(cameraOrigin.x, cameraOrigin.y, cameraZPos);
        }
        float cameraXPos = cameraOrigin.x + offset.x * MOVEMENT_SENSITIVITY;
        cameraXPos = Mathf.Clamp(cameraXPos, MIN_CAMERA_X_POSITION, MAX_CAMERA_X_POSITION);
        GameController.cc.SetCameraPositionAndRotation(Vector3.SmoothDamp(GameController.cc.transform.position, new Vector3(cameraXPos, CameraPosition.y, cameraZPos), ref velocity, SMOOTH_TIME), CameraRotation);
    }

    private void SetUpLevelScene()
    {
        currentCameraZClamp = CameraZClamp[GameController.GetCurrentEight()];
        GameController.cc.ResetCameraSize();
        int currentLevel = GameController.GetCurrentLevel();
        for (int i = 0; i < currentLevel-1; i++)
        {
            LevelTowers[i].material = PassedLevelMaterial;
        }
        LevelTowers[currentLevel-1].material = CurrentLevelMaterial;
        GameController.cc.SetCameraPositionAndRotation(new Vector3(Mathf.Clamp(LevelTowers[currentLevel - 1].transform.position.x + CameraPosition.x, MIN_CAMERA_X_POSITION, MAX_CAMERA_X_POSITION), CameraPosition.y, LevelTowers[currentLevel - 1].transform.position.z + CameraPosition.z), CameraRotation);
        cameraOrigin = GameController.cc.transform.position;
    }
}
