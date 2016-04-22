using UnityEngine;
using System.Collections;

public class Trivia : MonoBehaviour {

	// Use this for initialization
	void Start () {
		TriviaController.IncreaseTriviaCountInScene();
	}


	void OnCollisionEnter(Collision collision) {
		TriviaController.IncreaseTriviaCollected();
		gameObject.SetActive(false);
	}

}
