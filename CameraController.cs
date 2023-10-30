using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Texture2D cursorTexture;
    [SerializeField]
    float mouseLookSensitivity;
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    float moveDelta;

    private const float FLOOR_HEIGHT = 2f;
    private const float CEILING_HEIGHT = 25f;

    // Referenced in Start()
    private Camera mainCamera;
    private GameManager gameManager;
    private UIManager uiManager;
    private GameObject absoluteTransform;

    Vector3 lookChange;

    void Start()
    {

        mainCamera = Camera.main;
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        absoluteTransform = new GameObject();
        absoluteTransform.name = "Transform Reference";

        EnableLockedCamera();
    }

    void Update()
    {
        if (PlayerInput.pressedTab)
            if (gameManager.GetCurrentCameraState() == GameManager.CameraState.Locked)
            {
                EnableFreeLookCamera();
                return;
            }
            else
            {
                EnableLockedCamera();
                return;
            }

        if (gameManager.GetCurrentCameraState() == GameManager.CameraState.Locked)
            return;

        if (PlayerInput.pressedLeftClick || PlayerInput.pressedRightClick)
        {
            uiManager.ShowNotification("Free Look Mode On!", "Press tab key to switch to build mode");
        }
    }

    private void LateUpdate()
    {
        if (gameManager.GetCurrentCameraState() == GameManager.CameraState.Locked)
            return;

        lookChange = Vector3.MoveTowards(lookChange, new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0f), moveDelta);

        transform.eulerAngles += mouseLookSensitivity * lookChange * Time.deltaTime;

        transform.Translate(moveSpeed * Vector3.forward * PlayerInput.cameraStrafeZ * Time.deltaTime);
        transform.Translate(moveSpeed * Vector3.right * PlayerInput.cameraStrafeX * Time.deltaTime);

        transform.Translate(moveSpeed * Vector3.up * PlayerInput.verticalAxis * Time.deltaTime, Space.World);

        if (transform.position.y <= FLOOR_HEIGHT)
        {
            transform.position = new Vector3(transform.position.x, FLOOR_HEIGHT, transform.position.z);
        }
        else if (transform.position.y >= CEILING_HEIGHT)
        {
            transform.position = new Vector3(transform.position.x, CEILING_HEIGHT, transform.position.z);
        }

    }

    public void EnableFreeLookCamera()
    {
        gameManager.ChangeCameraState(GameManager.CameraState.FreeLook);
        uiManager.ChangeCameraModeText("Free Look");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void EnableLockedCamera()
    {
        gameManager.ChangeCameraState(GameManager.CameraState.Locked);
        uiManager.ChangeCameraModeText("Locked");
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }
}
