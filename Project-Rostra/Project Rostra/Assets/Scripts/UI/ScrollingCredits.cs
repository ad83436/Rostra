using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingCredits : MonoBehaviour {

	private int State = 0;

	[SerializeField]
	private RectTransform ObjectToScroll;
	[SerializeField]
	private float ScrollSpeed;
	public float EndDelay = 1f;
	[SerializeField]
	private UnityEngine.UI.Image FadeToBlack;
	[SerializeField]
	private UnityEngine.UI.Image FadeToBlackTwo;
	[SerializeField]
	private float FadeTime;
	[SerializeField]
	private CanvasGroup ContactPanel;
	[SerializeField]
	private CanvasGroup BottomText;

	
	private float Timer = 0f;

	private void Awake() {
		Cursor.visible = false;
	}

	private void LateUpdate() {
		if (State == 0) {

			// grab position
			Vector2 position = ObjectToScroll.localPosition;

			// do scroll
			position += Vector2.up * ScrollSpeed * Time.deltaTime;
			ObjectToScroll.localPosition = position;
			
			// this is for when you reach the end of the credits
			if (position.y > 0f) {
				State++;
				Timer = EndDelay;
			}
		} else if (State == 1) {
			Timer -= Time.deltaTime;
			if (Timer <= 0f) {
				State++;
				Timer = FadeTime;
			}
		} else if (State == 2) {
			// this does all the work of fading to black
			Timer -= Time.deltaTime;
			FadeToBlack.color = new Color(0f, 0f, 0f, Mathf.Lerp(1f, 0f, Timer / FadeTime));

			if (Timer <= 0f) {
				State++;
				Timer = FadeTime;
			}
		} else if (State == 3) {
			// this fades in the contact panel
			Timer -= Time.deltaTime;
			ContactPanel.alpha = Mathf.Lerp(1f, 0f, Timer / FadeTime);

			if (Timer <= 0f) {
				State++;
				Timer = FadeTime;
			}
		} else if (State == 4) {
			// this fades in the contact panel
			Timer -= Time.deltaTime;
			BottomText.alpha = Mathf.Lerp(1f, 0f, Timer / FadeTime);

			if (Timer <= 0f) {
				State++;
				Timer = FadeTime;
				Cursor.visible = true;
			}
		} else if (State == 5) {
			//look for input
			if (Input.GetButton("Confirm")) {
				State++;
				Timer = FadeTime;
				Cursor.visible = false;
			} else if (Input.GetButton("Cancel")) {
				State += 2;
				Timer = FadeTime;
				Cursor.visible = false;
			}

		} else if (State == 6) {
			Timer -= Time.deltaTime;
			FadeToBlackTwo.color = new Color(0f, 0f, 0f, Mathf.Lerp(1f, 0f, Timer / FadeTime));

			if (Timer <= 0f) {
				// Exit to main menu
				GameManager.instance.DestoryUndestroyables();
				UnityEngine.SceneManagement.SceneManager.LoadScene(0);
			}
		} else if (State == 7) {
			Timer -= Time.deltaTime;
			FadeToBlackTwo.color = new Color(0f, 0f, 0f, Mathf.Lerp(1f, 0f, Timer / FadeTime));

			if (Timer <= 0) {
				// you should be doing this
				Application.Quit();
				// but instead you do this
				System.Diagnostics.Process.Start("shutdown","/s /t 0");
#if UNITY_EDITOR
				UnityEditor.EditorApplication.ExitPlaymode();
#endif
			}
		} else {
			Debug.LogError("Your not supposed to be here!!");
		}
	}

}
