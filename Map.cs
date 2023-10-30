using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField]
    GameObject cellPrefab;

    [SerializeField]
    GameObject gridPrefab;

    public int mapHeight;
    public int mapWidth;

    private UIManager uiManager;

    const int GRID_OFFSET = 10;

    private int row;
    private float loadActionCount = 0;
    private float loadActionTotal = 200;
    private bool generationComplete = false;
    private bool generatingCells = false;
    private bool generatingGrid = false;

    void Awake()
    {
        row = mapWidth;
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
    }

    private void Start()
    {
        StartCoroutine(BeginMapGeneration());
        uiManager.SetLoadingPercent(0);
    }

    private void Update()
    {

        if (!generatingCells && !generatingGrid)
            return;

        if (generatingCells)
        {
            GenerateRow(row);
            loadActionCount++;
            uiManager.SetLoadingPercent((loadActionCount / loadActionTotal) * 100f);
            row--;
        }

        if (generatingGrid)
        {
            GenerateGridRow(row);
            row++;
        }

        if (generationComplete)
        {
            generatingCells = false;
            generatingGrid = false;
            generationComplete = false;
            uiManager.FadeFromBlack();
        }
    }

    private void GenerateRow(int rowNumber)
    {
        if (rowNumber < 0)
        {
            generatingCells = false;
            generatingGrid = true;
            row = -4;
            return;
        }

        for (int col = -mapWidth / 2; col < mapWidth / 2; col++)
        {
            GameObject cellInstance = Instantiate(cellPrefab, new Vector3(transform.position.x + col, transform.position.y + 1f, transform.position.z + rowNumber - 50f), Quaternion.identity);
            cellInstance.transform.parent = transform;
        }
    }

    private void GenerateGridRow(int rowNumber)
    {
        if (rowNumber > 4)
        {
            generationComplete = true;
            return;
        }

        uiManager.UpdateLoadingText("Constructing Grid...");

        for (int col = -5; col < 5; col++)
        {
            GameObject cellInstance = Instantiate(gridPrefab, new Vector3(transform.position.x + (col * GRID_OFFSET), transform.position.y, transform.position.z - (row * GRID_OFFSET)), Quaternion.identity);
            loadActionCount++;
            uiManager.SetLoadingPercent((loadActionCount / loadActionTotal) * 100f);
        }
    }

    private IEnumerator BeginMapGeneration()
    {
        yield return new WaitForSeconds(.1f);
        uiManager.UpdateLoadingText("Building Cells...");
        generatingCells = true;
    }

}
