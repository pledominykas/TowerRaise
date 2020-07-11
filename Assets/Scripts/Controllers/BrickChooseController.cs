using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BrickChooseController : MonoBehaviour
{
    public enum MainButtonState { Use, Buy, Locked }
    private MainButtonState CurrentState = MainButtonState.Use;

    private float BackgroundRotationSpeed = 10f;
    [SerializeField] Image BackgroundImage;
    [SerializeField] Material LockedBrickMaterial;
    [SerializeField] Material LockedBrickBackground;
    private List<BrickBuy> Bricks;
    private List<Transform> BrickObjects = new List<Transform>();
    private Vector3 BrickPosition = new Vector3(0f, 0f, 6f);
    private Vector3 BrickRotation = new Vector3(-60f, 30f, -10f);
    private Vector3 BrickRotationSpeed = new Vector3(15f, 45f, 0f);

    private int ActiveBrick = 0;
    private List<int> BoughtBricks;

    private float MinSwipeSqrMagnitude = 0.06f;
    private Vector3 mouseOrigin;
    private float SwipeSensitivity = 3f;
    private Vector3 brickVel;

    private void Start()
    {
        GameController.cc.SetCameraPositionAndRotation(Vector3.zero, Quaternion.identity);
        SetUpBrickChooseScene();
    }

    private void SetUpBrickChooseScene()
    {

        ActiveBrick = GameController.bm.GetSelectedBrickId();
        BoughtBricks = GameController.bm.GetBoughtBricks();
        Bricks = GameController.bm.GetBricks();

        for (int i = 0; i < Bricks.Count; i++)
        {
            Transform brick = Instantiate(Bricks[i].BrickPrefab, Vector3.zero, Quaternion.identity).transform;
            brick.GetComponent<Rigidbody>().isKinematic = true;
            if (i < ActiveBrick) { brick.position = BrickPosition + Vector3.left * 10f; }
            else { brick.position = BrickPosition + Vector3.right * 10f; }
            brick.rotation = Quaternion.Euler(BrickRotation);
            BrickObjects.Add(brick);
            brick.SetParent(transform);
        }
        OnSwipe(0);
    }

    private void Update()
    {
        BackgroundImage.transform.Rotate(-Vector3.forward * BackgroundRotationSpeed * Time.deltaTime);
        BrickObjects[ActiveBrick].Rotate(BrickRotationSpeed * Time.deltaTime, Space.World);
        DetectSwipes();
    }

    private void DetectSwipes()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            mouseOrigin = new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            BrickObjects[ActiveBrick].position = BrickPosition + new Vector3((Input.mousePosition.x / Screen.width) - mouseOrigin.x, 0f, 0f) * SwipeSensitivity;
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            float swipeMag = Vector3.SqrMagnitude(mouseOrigin - new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height));
            if (swipeMag >= MinSwipeSqrMagnitude)
            {
                if (mouseOrigin.x - (Input.mousePosition.x / Screen.width) < 0) { OnSwipe(-1); }
                else { OnSwipe(1); }
            }
            else
            {
                StartCoroutine(SpringBack());
            }
        }
    }

    private IEnumerator SpringBack()
    {
        for(float i=0f; i < 1f; i += Time.deltaTime)
        {
            BrickObjects[ActiveBrick].position = Vector3.SmoothDamp(BrickObjects[ActiveBrick].position, BrickPosition, ref brickVel, 0.1f);
            if(Vector3.SqrMagnitude(BrickObjects[ActiveBrick].position - BrickPosition) <= 0.005f)
            {
                break;
            }
            yield return null;
        }
    }

    private void OnSwipe(int _dir)
    {
        if (ActiveBrick + _dir < 0 || ActiveBrick + _dir >= BrickObjects.Count) { StartCoroutine(SpringBack()); return; }

        BrickObjects[ActiveBrick].position = -Vector3.right * 10f * _dir + BrickPosition;
        ActiveBrick += _dir;
        StartCoroutine(SpringBack());
        if (BoughtBricks.Contains(ActiveBrick))
        {
            BackgroundImage.material = GameController.bm.GetBackgroundMaterial(ActiveBrick);
            BrickObjects[ActiveBrick].GetComponent<MeshRenderer>().material = Bricks[ActiveBrick].BrickPrefab.GetComponent<Renderer>().sharedMaterial;
            GameController.uc.SetBrickChooseButtonText("Use");
            CurrentState = MainButtonState.Use;
            if(_dir != 0) { GameController.uc.DisableBrickChoosePriceText(); GameController.uc.SetBrickChoosePriceTextFontSize(70); }
        }
        else
        {
            BackgroundImage.material = LockedBrickBackground;
            BrickObjects[ActiveBrick].GetComponent<MeshRenderer>().material = LockedBrickMaterial;
            for (int i = 1; i < BrickObjects[ActiveBrick].childCount; i++)
            {
                BrickObjects[ActiveBrick].GetChild(i).gameObject.SetActive(false);
            }

            GameController.uc.SetBrickChoosePriceTextFontSize(70);
            if (Bricks[ActiveBrick].isBuyable)
            {
                GameController.uc.SetBrickChooseButtonText("Buy!");
                CurrentState = MainButtonState.Buy;
                GameController.uc.EnableBrickChoosePriceText();
                GameController.uc.SetBrickChoosePriceText(Bricks[ActiveBrick].Price.ToString() + "  <sprite=0>", new Vector3(10f, 175f));
            }
            else
            {
                GameController.uc.SetBrickChooseButtonText("Lvl " + Bricks[ActiveBrick].UnlockLevel);
                CurrentState = MainButtonState.Locked;
                GameController.uc.EnableBrickChoosePriceText();
                GameController.uc.SetBrickChoosePriceText("Locked", new Vector3(0f, 175f));
            }
        }
    }

    public MainButtonState GetCurrentState() { return CurrentState; }
    public int GetCurrentActiveBrick() { return ActiveBrick; }
    public int GetActiveBrickPrice() { return Bricks[ActiveBrick].Price; }
    public void UpdateMainButton() { OnSwipe(0); }
}


