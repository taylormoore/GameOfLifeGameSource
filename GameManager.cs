using UnityEngine;
using System.Collections.Generic;
using Michsky.UI.ModernUIPack;

public class GameManager : MonoBehaviour
{
	public float gameSpeed;

	private GameState currentGameState;
	private CameraState currentCameraState;

	public List<Cell> aliveCells;
	public List<Cell> listeningCells;
	public List<Cell> toBeRemoved;

	float lastCycle;
	private int cycleIndex;
	private UIManager uiManager;

	[SerializeField]
	TooltipContent playButtonTooltip;


	private void Start()
	{
		uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
	}

	public enum GameState
	{
		Building,
		Simulating
	};

	public enum CameraState
	{
		Locked,
		FreeLook
	};

	private void Update()
	{
		if (GetCurrentGameState() == GameState.Building)
			return;

		// Simulating state
		if (Time.time >= lastCycle + gameSpeed)
		{
			switch (cycleIndex)
			{
				case 0:
					UpdateListeners();
					cycleIndex++;
					break;
				case 1:
					Evaluate();
					cycleIndex++;
					break;
				case 2:
					ApplyEvaluation();
					cycleIndex++;
					break;
				case 3:
					AddListenersToBeRemoved();
					cycleIndex++;
					break;
				case 4:
					ScrubListeners();
					cycleIndex = 0;
					lastCycle = Time.time;
					break;
			}
		}
	}

	public void UpdateListeners()
	{
		if (aliveCells.Count == 0)
		{
			ToggleSimulation();
			uiManager.ShowNotification("Simulation stopped!", "No live cells remaining");
		}

		foreach (Cell c in aliveCells)
		{
			if (!listeningCells.Contains(c) && (c.GetNeighborCount() > 3 || c.GetNeighborCount() < 2)) // Only add a live cell to listeners if we know it will change next step.
				listeningCells.Add(c);
			c.AddNeighborsToListeners();
		}
	}

	public void Evaluate()
	{
		foreach (Cell c in listeningCells)
			c.EvaluateStep();
	}

	public void ApplyEvaluation()
	{
		foreach (Cell c in listeningCells)
			c.OnStepCleanup();
	}

	public void AddListenersToBeRemoved()
	{

		foreach (Cell c in listeningCells)
			if (!c.ShouldRemainListener())
				toBeRemoved.Add(c);
	}

	public void ScrubListeners()
	{
		foreach (Cell c in toBeRemoved)
			listeningCells.Remove(c);

		toBeRemoved.Clear();
	}

	// Called via UI button
	public void Step()
	{
		if (GetCurrentGameState() == GameState.Simulating)
		{
			uiManager.ShowNotification("Error!", "Cannot step forward while simulation is running");
			return;
		}

		UpdateListeners();

		Evaluate();

		ApplyEvaluation();

		AddListenersToBeRemoved();

		ScrubListeners();
	}

	public void ClearAllCells()
	{
		if (GetCurrentGameState() == GameState.Simulating)
		{
			uiManager.ShowNotification("Error!", "Cannot clear all cells while simulation is running");
			return;
		}

		foreach (Cell c in aliveCells)
		{
			c.ToggleAlive();
		}

		aliveCells.Clear();
		listeningCells.Clear();
		toBeRemoved.Clear();
	}
	public void ChangeGameState(GameState newState)
	{
		currentGameState = newState;
		uiManager.ChangeSimButtonIcon(currentGameState);
	}

	public GameState GetCurrentGameState()
	{
		return currentGameState;
	}

	public void ChangeCameraState(CameraState newState)
	{
		currentCameraState = newState;
	}

	public CameraState GetCurrentCameraState()
	{
		return currentCameraState;
	}

	// Called via UI button
	public void ToggleSimulation()
	{
		if (GetCurrentGameState() == GameState.Building)
		{
			playButtonTooltip.description = "Pause Simulation";
			ChangeGameState(GameState.Simulating);
		}
		else
		{
			playButtonTooltip.description = "Play Simulation";
			ChangeGameState(GameState.Building);
		}
	}
}
