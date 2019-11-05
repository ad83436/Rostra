using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable CS0649

public class SettingsMenuController : SubMenu {

	//Option Index
	private int index = 0;
	const int MAX_OPTION_COUNT = 3;

	//Highlight colors
	[SerializeField] private Color HighlightColor;
	[SerializeField] private Color UnhighlightColor;

	//Text
	[SerializeField] private UnityEngine.UI.Image[] OptionPanels;
	[SerializeField] private UnityEngine.UI.Text FullscreenText;
	[SerializeField] private UnityEngine.UI.Slider AudioSlider;

	private void UpdateAudioSlider() {
		AudioSlider.value = AudioListener.volume;
	}

	private void UpdateFullscreenText() {
		FullscreenText.text = "Fullscreen: " + (Screen.fullScreen ? "On" : "Off");
	}

	#region Submenu Functions

	public override void MenuUpdate() {
		if (Cancel) {
			ExitMenu();
			return;
		}

		OptionPanels[index].color = UnhighlightColor;
		if (Up && index > 0) --index;
		if (Down && index < MAX_OPTION_COUNT) ++index;
		OptionPanels[index].color = HighlightColor;

		/// options are:
		/// 0 = Audio Level
		/// 1 = Fullscreen
		/// 2 = Exit To Menu
		/// 3 = Exit Game
		switch (index) {
			case 0: { /// Audio

				if (Left && AudioListener.volume > 0f) AudioListener.volume -= 0.1f;
				if (Right && AudioListener.volume < 1f) AudioListener.volume += 0.1f;
				UpdateAudioSlider();

				break;
			} /// Audio
			case 1: { /// Fullscreen

				UpdateFullscreenText();

				if (Confirm) {
					if (Screen.fullScreen) {
						Screen.SetResolution(1280, 720, false);

					} else {
						Screen.SetResolution(1920, 1080, true);
					}
				}

				break;
			} /// Fullscreen
			// TODO: add exit to menu functionality
			case 2: { /// Exit to Menu

				if (Confirm) {
					Debug.Log("We should be exiting to the menu here");
                    PauseMenuController.isPaused = false;
                    GameManager.instance.DestoryUndestroyables();
                    SceneManager.LoadScene("Main Menu");
				}

				break;
			} /// Exit to Menu
			// TODO: add saving before exiting game
			case 3: { /// Exit to Desktop

				if (Confirm) {
					Application.Quit();
				}
				break;
			} /// Exit to Desktop
			default: Debug.LogError("Option missing"); break;
		}
	}

	#region Events

	public override void OnVisible() {
		UpdateAudioSlider();
		UpdateFullscreenText();
	}

	public override void OnInvisible() { }

	public override void OnActive() {
		OptionPanels[index].color = HighlightColor;
	}

	public override void OnInactive() {
		OptionPanels[index].color = UnhighlightColor;
		index = 0;
	}

	#endregion

	#endregion
}
