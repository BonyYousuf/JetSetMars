using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloatingWindow : MonoBehaviour {

	public Button btnClose;




	void Awake()
	{
		btnClose.onClick.AddListener(delegate() {
			hide();
		});
	}


	public void show()
	{
		gameObject.SetActive(true);
	}


	public void hide()
	{
		gameObject.SetActive(false);
	}
}
