using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	public Button btnExplore;
	public Button btnQuit;


	public MapsWindow mapWindow;

	void Awake()
	{
		btnExplore.onClick.AddListener(delegate() {
			mapWindow.show();
		});

		btnQuit.onClick.AddListener(delegate() {
			Application.Quit();
		});
	}
}
