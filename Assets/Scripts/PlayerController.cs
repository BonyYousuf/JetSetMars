using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.Cameras;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

	public ThirdPersonCharacter thirdPersonController;
	public ThirdPersonCharacterSegway segwayController;

	public static PlayerController Instance;

	private Camera cam;

	private Gear currentGear;
	private int currentGearKey;

	private enum Gear{
		Default,
		Exoskeleton,
		Segway,
		JetPack
	}

	private Gear[] gearSequence;


	void Awake()
	{
		Instance = this;

		cam = Camera.main;

		gearSequence = new Gear[4];
		gearSequence[0] = Gear.Default;
		gearSequence[1] = Gear.Exoskeleton;
		gearSequence[2] = Gear.Segway;
		gearSequence[3] = Gear.JetPack;

		currentGearKey = 0;
		setGear(currentGearKey);
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(KeyCode.Escape))
		{
			SceneManager.LoadScene("Main");
		}
		else if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.B))
		{
			cam.GetComponent<BloomOptimized>().enabled = !cam.GetComponent<BloomOptimized>().enabled;
			Debug.Log("Bloom: " + (cam.GetComponent<BloomOptimized>().enabled ? "On" : "Off"));
		}
		else if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.S))
		{
			cam.GetComponent<SunShafts>().enabled = !cam.GetComponent<SunShafts>().enabled;
			Debug.Log("SunShafts: " + (cam.GetComponent<SunShafts>().enabled ? "On" : "Off"));
		}
		else if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.C))
		{
			cam.GetComponent<ColorCorrectionCurves>().enabled = !cam.GetComponent<ColorCorrectionCurves>().enabled;
			Debug.Log("ColorCorrectionCurves: " + (cam.GetComponent<ColorCorrectionCurves>().enabled ? "On" : "Off"));
		}
		else if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.D))
		{
			cam.GetComponent<DepthOfField>().enabled = !cam.GetComponent<DepthOfField>().enabled;
			Debug.Log("DepthOfField: " + (cam.GetComponent<DepthOfField>().enabled ? "On" : "Off"));
		}
		else if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.V))
		{
			cam.GetComponent<VignetteAndChromaticAberration>().enabled = !cam.GetComponent<VignetteAndChromaticAberration>().enabled;
			Debug.Log("VignetteAndChromaticAberration: " + (cam.GetComponent<VignetteAndChromaticAberration>().enabled ? "On" : "Off"));
		}
		else if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.F))
		{
			cam.GetComponent<GlobalFog>().enabled = !cam.GetComponent<GlobalFog>().enabled;
			Debug.Log("GlobalFog: " + (cam.GetComponent<GlobalFog>().enabled ? "On" : "Off"));
		}
		else if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.M))
		{
			cam.GetComponent<CameraMotionBlur>().enabled = !cam.GetComponent<CameraMotionBlur>().enabled;
			Debug.Log("CameraMotionBlur: " + (cam.GetComponent<CameraMotionBlur>().enabled ? "On" : "Off"));
		}
		/*
		else if(Input.GetKeyUp(KeyCode.J))
		{
			segwayController.hide();
			thirdPersonController.show();
			thirdPersonController.setJetPack(!thirdPersonController.hasJetPack());
		}
		else if(Input.GetKeyUp(KeyCode.E))
		{
			segwayController.hide();
			thirdPersonController.show();
			thirdPersonController.setExoskeleton(!thirdPersonController.hasExoskeleton());
		}
		*/
		else if(Input.GetKeyUp(KeyCode.E))
		{
			setNextGear();
		}
		else if(Input.GetKeyUp(KeyCode.Q))
		{
			setPreviousGear();
		}
	}

	private void setPreviousGear()
	{
		setGear( (currentGearKey + 3) % gearSequence.Length );
	}

	private void setNextGear()
	{
		setGear( (currentGearKey + 1) % gearSequence.Length );
	}

	private void setGear(int gearIndex)
	{
		setGear(gearSequence[gearIndex]);
	}

	private void setGear(Gear gear)
	{
		currentGear = gear;
		setCurrentGearKey();

		if(segwayController.gameObject.activeInHierarchy)
		{
			thirdPersonController.transform.position = segwayController.transform.position;
			thirdPersonController.transform.rotation = segwayController.transform.rotation;
		}
		else
		{
			segwayController.transform.position = thirdPersonController.transform.position;
			segwayController.transform.rotation = thirdPersonController.transform.rotation;
		}

		if(currentGear == Gear.Default)
		{
			segwayController.hide();
			thirdPersonController.show();
			thirdPersonController.setJetPack(false);
			thirdPersonController.setExoskeleton(false);
		}
		else if(currentGear == Gear.Exoskeleton)
		{
			segwayController.hide();
			thirdPersonController.show();
			thirdPersonController.setJetPack(false);
			thirdPersonController.setExoskeleton(true);
		}
		else if(currentGear == Gear.Segway)
		{
			segwayController.show();
			thirdPersonController.hide();
		}
		else if(currentGear == Gear.JetPack)
		{
			segwayController.hide();
			thirdPersonController.show();
			thirdPersonController.setJetPack(true);
			thirdPersonController.setExoskeleton(false);
		}
	}

	private void setCurrentGearKey()
	{
		for(int i=0; i<gearSequence.Length; i++)
		{
			if(gearSequence[i] == currentGear)
			{
				currentGearKey = i;
				break;
			}
		}
	}


	public static void TogglePlayerTransform(Transform newTransform)
	{
		Instance.cam.GetComponent<DepthOfField>().focalTransform = newTransform;
		FreeLookCam.Instance.SetTarget(newTransform);
	}
}
