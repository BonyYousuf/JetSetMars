using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TriviaWindow : FloatingWindow {

	public Text triviaText;

	// Use this for initialization
	void Start () {
	
	}

	void OnEnable()
	{
		triviaText.text = TriviaController.Instance.getTriviaTexts();
	}
}
