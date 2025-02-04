﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShowPanels : MonoBehaviour {

	public GameObject optionsPanel;							//Store a reference to the Game Object OptionsPanel 
	public GameObject optionsTint;							//Store a reference to the Game Object OptionsTint 
	public GameObject menuPanel;							//Store a reference to the Game Object MenuPanel 
	public GameObject pausePanel;							//Store a reference to the Game Object PausePanel 
	public GameObject gamePanel;
	public GameObject gameOverPanel;
	public GameObject creditsPanel;
	public GameObject tutorialPanel;

	public Button[] MenuButtons;
	public Button[] GameOverButtons;

	public void GameOverActivate()
	{
		gameOverPanel.GetComponent<CanvasGroup>().alpha = 1.0f;
		gameOverPanel.SetActive(true);
	}

	public void DisableMenuButtons()
	{
		for (int i = 0; i < MenuButtons.Length; i++) {
			MenuButtons[i].interactable = false;
		}
	}

	public void EnableMenuButtons()
	{
		for (int i = 0; i < MenuButtons.Length; i++) {
			MenuButtons[i].interactable = true;
		}
	}

	public void DisableGameOverButtons()
	{
		for (int i = 0; i < GameOverButtons.Length; i++) {
			GameOverButtons[i].interactable = false;
		}
	}

	public void EnableGameOverButtons()
	{
		for (int i = 0; i < GameOverButtons.Length; i++) {
			GameOverButtons[i].interactable = true;
		}
	}

	public void ShowGamePanel()
	{
		gamePanel.SetActive(true);
	}

	//Call this function to activate and display the Options panel during the main menu
	public void ShowOptionsPanel()
	{
		menuPanel.SetActive(false);
		optionsPanel.SetActive(true);
		optionsTint.SetActive(true);
	}

	//Call this function to deactivate and hide the Options panel during the main menu
	public void HideOptionsPanel()
	{
		optionsPanel.SetActive(false);
		optionsTint.SetActive(false);
		menuPanel.SetActive(true);
	}

	public void HideCredits()
	{
		creditsPanel.SetActive(false);
	}

	//Call this function to activate and display the main menu panel during the main menu
	public void ShowMenu()
	{
		menuPanel.GetComponent<CanvasGroup>().alpha = 1.0f;
		menuPanel.SetActive (true);
	}
	
	public void ShowTutorial()
	{
		tutorialPanel.SetActive(true);
		tutorialPanel.GetComponent<Tutorial>().SetInitializeTrigger();
	}

	//Call this function to deactivate and hide the main menu panel during the main menu
	public void HideMenu()
	{
		menuPanel.SetActive (false);
	}

	public void ShowCreditsPanel()
	{
		creditsPanel.GetComponent<CanvasGroup>().alpha = 1.0f;
		creditsPanel.SetActive(true);
	}

	public void HideGameOverMenu()
	{
		gameOverPanel.SetActive(false);
	}
	
	//Call this function to activate and display the Pause panel during game play
	public void ShowPausePanel()
	{
		pausePanel.SetActive (true);
		optionsTint.SetActive(true);
	}

	//Call this function to deactivate and hide the Pause panel during game play
	public void HidePausePanel()
	{
		pausePanel.SetActive (false);
		optionsTint.SetActive(false);

	}
}
