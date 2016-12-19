using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchKeyForStartup : MonoBehaviour {

    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input( (int)trackedObj.index ); } }
    private SteamVR_TrackedObject trackedObj;

    // Use this for initialization
    void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>( );
    }
	
	// Update is called once per frame
	void Update () {
        if (controller != null)
        {
            if ((controller.GetPressUp( Valve.VR.EVRButtonId.k_EButton_A )) ||
                 (controller.GetPressUp( Valve.VR.EVRButtonId.k_EButton_ApplicationMenu )) ||
                 (controller.GetPressUp( Valve.VR.EVRButtonId.k_EButton_DPad_Down )) ||
                 (controller.GetPressUp( Valve.VR.EVRButtonId.k_EButton_Grip )) ||
                 (controller.GetPressUp( Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad )) ||
                 (controller.GetPressUp( Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger )) ||
                 (controller.GetPressUp( Valve.VR.EVRButtonId.k_EButton_System ))
               )
                Application.LoadLevel( Application.loadedLevel + 1 );


        }
    }
}
