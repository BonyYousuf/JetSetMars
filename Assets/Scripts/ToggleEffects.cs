using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using UnityStandardAssets.Characters.ThirdPerson;

public class ToggleEffects : MonoBehaviour {


	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.B))
		{
			GetComponent<BloomOptimized>().enabled = !GetComponent<BloomOptimized>().enabled;
			Debug.Log("Bloom: " + (GetComponent<BloomOptimized>().enabled ? "On" : "Off"));
		}
		else if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.S))
		{
			GetComponent<SunShafts>().enabled = !GetComponent<SunShafts>().enabled;
			Debug.Log("SunShafts: " + (GetComponent<SunShafts>().enabled ? "On" : "Off"));
		}
		else if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.C))
		{
			GetComponent<ColorCorrectionCurves>().enabled = !GetComponent<ColorCorrectionCurves>().enabled;
			Debug.Log("ColorCorrectionCurves: " + (GetComponent<ColorCorrectionCurves>().enabled ? "On" : "Off"));
		}
		else if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.D))
		{
			GetComponent<DepthOfField>().enabled = !GetComponent<DepthOfField>().enabled;
			Debug.Log("DepthOfField: " + (GetComponent<DepthOfField>().enabled ? "On" : "Off"));
		}
		else if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.V))
		{
			GetComponent<VignetteAndChromaticAberration>().enabled = !GetComponent<VignetteAndChromaticAberration>().enabled;
			Debug.Log("VignetteAndChromaticAberration: " + (GetComponent<VignetteAndChromaticAberration>().enabled ? "On" : "Off"));
		}
		else if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.F))
		{
			GetComponent<GlobalFog>().enabled = !GetComponent<GlobalFog>().enabled;
			Debug.Log("GlobalFog: " + (GetComponent<GlobalFog>().enabled ? "On" : "Off"));
		}
		else if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.M))
		{
			GetComponent<CameraMotionBlur>().enabled = !GetComponent<CameraMotionBlur>().enabled;
			Debug.Log("CameraMotionBlur: " + (GetComponent<CameraMotionBlur>().enabled ? "On" : "Off"));
		}
		else if(Input.GetKeyUp(KeyCode.J))
		{
			ThirdPersonCharacter.Instance.setJetPack(!ThirdPersonCharacter.Instance.hasJetPack());
		}
	}
}
