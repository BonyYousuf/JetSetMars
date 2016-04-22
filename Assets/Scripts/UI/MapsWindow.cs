using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapsWindow : FloatingWindow {


	public Button btnMap1;
	public Button btnMap2;
	public Button btnMap3;



	void Start()
	{
		btnMap1.onClick.AddListener(delegate() {
			SceneManager.LoadScene("Training Ground");
		});

		btnMap2.onClick.AddListener(delegate() {
			SceneManager.LoadScene("HeightMap");
		});

		btnMap3.onClick.AddListener(delegate() {
			SceneManager.LoadScene("Test");
		});
	}
}
