using UnityEngine;

public class CameraFollowPoint : MonoBehaviour
{
    [SerializeField]
    float strafeHorizontalSpeed;
    [SerializeField]
    float strafeVerticalSpeed;
    [SerializeField]
    float verticalSpeed;

    private const float FLOOR_HEIGHT = 2f;
    private const float CEILING_HEIGHT = 25f;

    // Referenced in Start()
    private Camera mainCamera;
    private GameManager gameManager;
    private UIManager uiManager;
    private GameObject absoluteTransform;

    private void Start()
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

        // Strafing movement
        transform.position += new Vector3((absoluteTransform.transform.position.x + PlayerInput.cameraStrafeX) * strafeHorizontalSpeed, transform.position.y,
                                          (absoluteTransform.transform.position.z + PlayerInput.cameraStrafeZ) * strafeVerticalSpeed);

        // Vertical movement
        transform.Translate(absoluteTransform.transform.up * PlayerInput.verticalAxis * verticalSpeed * Time.deltaTime);


        if (transform.position.y <= FLOOR_HEIGHT)
        {
            transform.position = new Vector3(transform.position.x, FLOOR_HEIGHT, transform.position.z);
        }
        else if (transform.position.y >= CEILING_HEIGHT)
        {
            transform.position = new Vector3(transform.position.x, CEILING_HEIGHT, transform.position.z);
        }

        transform.forward = mainCamera.transform.forward;
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
    }
}
