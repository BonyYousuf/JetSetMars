using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TriviaController : MonoBehaviour {

	private int triviaCollected = 0;
	private static int _TriviaExistsInScene;

	public Trivia[] trivia;

	private Text txtTriviaCount;

	public Button btnTrivia;
	public TriviaWindow triviaWindow;

	private List<string> triviaInfo;

	public static TriviaController Instance;

	// Use this for initialization
	void Start () {

		Instance = this;

		_TriviaExistsInScene = 0;

		txtTriviaCount = GetComponent<Text>();

		btnTrivia.onClick.AddListener(delegate() {
			Debug.Log("Clicked");
			triviaWindow.show();
		});

		triviaInfo = new List<string>();
		triviaInfo.Add("Mars surface gravity is 3.71 m/s^2. Which is almost 1/3 of Earth's.");
		triviaInfo.Add("Mars has two moons named Phobos and Deimos.");
		triviaInfo.Add("The atmosphere of Mars is about 100 times thinner than Earth's, and it is 95 percent carbon dioxide.");
		triviaInfo.Add("Mars’ crust is thicker than Earth’s and is made up of one piece, unlike Earth’s crust which consists of several moving plates.");
		triviaInfo.Add("Mars’ seasons are twice as long as those on Earth because it takes Mars 687 days to orbit the sun, twice as long as Earth’s 365-day journey.");
		triviaInfo.Add("During a Mars winter, almost 20% of the air freezes.");
		triviaInfo.Add("The Red Planet is actually many colors. At the surface we see colors such as brown, golden, tan and butterscotch. The reason Mars looks so red is due to oxidization -- or rusting -- of iron in the rocks, soil and dust of Mars. This dust gets kicked up into the atmosphere and from a distance makes the planet appear mostly red.");
		triviaInfo.Add("No human could survive the low pressure of Mars. If you went to Mars without an appropriate space suit, the oxygen in your blood would literally turn into bubbles, causing immediate death.");
		triviaInfo.Add("On August 27, 2003, Mars made its closest approach to Earth in nearly 60,000 years. The next time it will be that close again will be in 2287.");
		triviaInfo.Add("Mars is about half the size of Earth.");
		triviaInfo.Add("Mars is the fourth planet from the Sun.");
		triviaInfo.Add("The temperature on Mars varies from cold to extremely cold.");
		triviaInfo.Add("Mars is about 35 million miles from Earth and 141.71 million miles from the Sun.");

		updateTriviaCount();


	}


	public string getTriviaTexts()
	{
		string txt = "";
		for(int i=0; i<triviaCollected; i++)
		{
			txt += "Trivia " + (i+1) + ":\n";
			txt += triviaInfo[i] + "\n\n";
		}

		return txt;
	}



	public void updateTriviaCount()
	{
		txtTriviaCount.text = "Trivia\n" + triviaCollected + "/" + _TriviaExistsInScene;
	}

	public static void IncreaseTriviaCollected()
	{
		Instance.triviaCollected++;
		Instance.updateTriviaCount();
	}

	public static void IncreaseTriviaCountInScene()
	{
		_TriviaExistsInScene++;

		Instance.updateTriviaCount();
	}
}
