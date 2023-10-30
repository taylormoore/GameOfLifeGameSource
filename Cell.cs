using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Cell : MonoBehaviour
{
    public int neighborCount;
    public Material aliveMaterial;
    public Material mouseOverMaterial;
    public AudioClip highlightSound;
    public AudioClip placementSound;

    private bool isAlive;
    private bool livesAfterStep;

    private const int MAX_NEIGHBOR_COUNT = 8;
    private const float NEIGHBOR_CHECK_DISTANCE = 1f;
    public bool mousedOver;
    private bool canBePlaced = true;

    // Referenced in Start()
    private GameManager gameManager;
    private SoundManager soundManager;
    private MeshRenderer meshRenderer;
    private Animator animator;


	private void Start()
	{
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        animator = GetComponent<Animator>();

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        soundManager = GameObject.Find("Sound Manager").GetComponent<SoundManager>();
        animator.enabled = false;
	}

    private void Update()
	{
        if (gameManager.GetCurrentCameraState() == GameManager.CameraState.FreeLook)
            return;

        if (mousedOver)
        {
            if (PlayerInput.pressedLeftClick || PlayerInput.holdingLeftClick)
            {
                if (canBePlaced)
                {
                    canBePlaced = false;
                    animator.enabled = true;
                    ResetAnimatorTriggers();
                    animator.SetTrigger("mousePlaced");
                    soundManager.PlaySound(placementSound, new Vector2(.9f, 1.05f), new Vector2(.9f, 1f));
                    ToggleAlive();
                    if (isAlive)
                        gameManager.aliveCells.Add(this);
                    else
                        gameManager.aliveCells.Remove(this);
                }
            }
        }

        if (PlayerInput.releasedLeftClick)
        {
            canBePlaced = true;
        }
    }

    private bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public void EvaluateStep()
    {
        if (!IsAlive() && GetNeighborCount() == 3)
        {
            livesAfterStep = true;
            return;
        }

        if (IsAlive())
        {
            if (GetNeighborCount() > 3 || GetNeighborCount() < 2)
            {
                livesAfterStep = false;
            }
            else
            {
                livesAfterStep = true;
            }
        }
    }

    public void UpdateNeighborToAdjacentCells(int amount)
    {
        int angle = 0;
        for (int i = 0; i < MAX_NEIGHBOR_COUNT; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Quaternion.Euler(0, angle, 0) * new Vector3(0, 0, -1), out hit, NEIGHBOR_CHECK_DISTANCE))
            {
                Cell hitCell = hit.transform.GetComponent<Cell>();
                hitCell.neighborCount += amount;
            }

            angle += 45;
        }
    }

	public void UpdateNeighborCount()
    {
        int total = 0;
        int angle = 0;
        for (int i = 0; i < MAX_NEIGHBOR_COUNT; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Quaternion.Euler(0, angle, 0) * new Vector3(0, 0, -1), out hit, NEIGHBOR_CHECK_DISTANCE))
            {
                Cell hitCell = hit.transform.GetComponent<Cell>();

                if (hitCell.IsAlive())
                    total += 1;
            }

            angle += 45;
        }
        neighborCount = total;
    }

    public bool ShouldRemainListener()
    {
        UpdateNeighborCount();
        return (IsAlive() && (GetNeighborCount() > 3 || GetNeighborCount() < 2)) || (!IsAlive() && GetNeighborCount() == 3);
    }

    public void OnStepCleanup()
    {
        if (livesAfterStep)
        {
            if (!IsAlive())
            {
                ToggleAlive();
                animator.enabled = true;
                ResetAnimatorTriggers();
                animator.SetTrigger("mousePlaced");
                gameManager.aliveCells.Add(this);
            }
        }
        else
        {
            if (IsAlive())
            {
                animator.enabled = true;
                ResetAnimatorTriggers();
                animator.SetTrigger("mouseExit");
                gameManager.aliveCells.Remove(this);
                StartCoroutine(WaitThenToggleAlive(.2f));
            }
        }
    }

    public bool IsAlive()
    {
        return isAlive;
    }

    public int GetNeighborCount()
    {
        return neighborCount;
    }

	public void OnMouseEnter()
	{
        if (gameManager.GetCurrentCameraState() == GameManager.CameraState.FreeLook || IsMouseOverUI())
            return;

        if (!PlayerInput.holdingLeftClick)
            soundManager.PlaySound(highlightSound, new Vector2(.95f, 1.03f), new Vector2(.9f, 1f));
        mousedOver = true;
        meshRenderer.enabled = true;
        meshRenderer.material = mouseOverMaterial;
        animator.enabled = true;
        ResetAnimatorTriggers();
        animator.SetTrigger("mousedOver");
	}

	private void OnMouseExit()
	{
        mousedOver = false;

        if (!IsAlive())
        {
            ResetAnimatorTriggers();
            animator.SetTrigger("mouseExit");
        }
        else
            meshRenderer.material = aliveMaterial;
	}

    public void AddNeighborsToListeners()
    {
        int angle = 0;
        for (int i = 0; i < MAX_NEIGHBOR_COUNT; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Quaternion.Euler(0, angle, 0) * new Vector3(0, 0, -1), out hit, NEIGHBOR_CHECK_DISTANCE))
            {
                Cell hitCell = hit.transform.GetComponent<Cell>();

                if (!hitCell.IsAlive() && hitCell.neighborCount == 3 && !gameManager.listeningCells.Contains(hitCell))
                {
                    gameManager.listeningCells.Add(hitCell);
                }

                angle += 45;
            }
        }
    }

    IEnumerator WaitThenToggleAlive(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        ToggleAlive();
    }

    public void ToggleAlive()
    {
        isAlive = !isAlive;
        meshRenderer.enabled = isAlive;
        meshRenderer.material = IsAlive() ? aliveMaterial : mouseOverMaterial;

        if (isAlive)
            UpdateNeighborToAdjacentCells(1);
        else
            UpdateNeighborToAdjacentCells(-1);
    }

    public void Anim_OnMouseExitEnd()
    {
        meshRenderer.enabled = false;
        animator.enabled = false;
    }

    public void Anim_OnPlaceEnd()
    {
        animator.enabled = false;
	}

	public void ResetAnimatorTriggers()
	{
        animator.ResetTrigger("mouseExit");
        animator.ResetTrigger("mousedOver");
        animator.ResetTrigger("mousePlaced");
    }

}
