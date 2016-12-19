using UnityEngine;
//using Valve.VR;
using System.Collections;

public class VRControllerScript : MonoBehaviour {

	private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
	private Valve.VR.EVRButtonId padButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
	private bool triggerButtonPressed = false;
	private bool padButtonPressed = false;
	private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input ((int)trackedObj.index); } }
	private SteamVR_TrackedObject trackedObj;


	// Use this for initialization
	void Start () {
		trackedObj = GetComponent<SteamVR_TrackedObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (controller != null) {
			if (controller.GetPressDown (padButton))
				padButtonPressed = !padButtonPressed;
			if (controller.GetPressUp (padButton) && padButtonPressed) {
				Time.timeScale = 0.0f;
			}
			if (controller.GetPressUp (padButton) && !padButtonPressed ) {
					Time.timeScale = 1.0f;
			}
		}
	}

	private void DebugLogger(uint index, string action, GameObject target)
	{
		Debug.Log("Controller on index '" + index + "' is " + action + " an object named " + target.name);
	}
}
