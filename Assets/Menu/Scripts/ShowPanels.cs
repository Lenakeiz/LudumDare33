using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShowPanels : MonoBehaviour {

	public GameObject optionsPanel;							//Store a reference to the Game Object OptionsPanel 
	public GameObject optionsTint;							//Store a reference to the Game Object OptionsTint 
	public GameObject menuPanel;							//Store a reference to the Game Object MenuPanel 
	public GameObject pausePanel;							//Store a reference to the Game Object PausePanel 
	public GameObject gamePanel;
	public GameObject gameOverPanel;

	public Button[] MenuButtons;
	
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

	public void ShowGamePanel()
	{
		gamePanel.SetActive(true);
	}

	//Call this function to activate and display the Options panel during the main menu
	public void ShowOptionsPanel()
	{
		optionsPanel.SetActive(true);
		optionsTint.SetActive(true);
	}

	//Call this function to deactivate and hide the Options panel during the main menu
	public void HideOptionsPanel()
	{
		optionsPanel.SetActive(false);
		optionsTint.SetActive(false);
	}

	//Call this function to activate and display the main menu panel during the main menu
	public void ShowMenu()
	{
		menuPanel.SetActive (true);
	}

	//Call this function to deactivate and hide the main menu panel during the main menu
	public void HideMenu()
	{
		menuPanel.SetActive (false);
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
