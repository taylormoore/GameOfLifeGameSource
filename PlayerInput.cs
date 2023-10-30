using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static bool pressedLeftClick;
    public static bool pressedRightClick;
    public static bool holdingLeftClick;
    public static bool releasedLeftClick;
    public static bool pressedTab;
    public static float verticalAxis;

    public static float cameraStrafeX;
    public static float cameraStrafeZ;

    public static Vector3 cursorWorldPosition;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        pressedLeftClick = Input.GetMouseButtonDown(0);
        pressedRightClick = Input.GetMouseButtonDown(1);
        holdingLeftClick = Input.GetMouseButton(0);
        releasedLeftClick = Input.GetMouseButtonUp(0);

        pressedTab = Input.GetKeyDown(KeyCode.Tab);
        verticalAxis = Input.GetAxis("UpDown");

        cameraStrafeX = Input.GetAxis("Horizontal");
        cameraStrafeZ = Input.GetAxis("Vertical");
    }
}
