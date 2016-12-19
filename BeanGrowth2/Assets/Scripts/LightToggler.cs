using UnityEngine;
using System.Collections;

public class LightToggler : MonoBehaviour {
	public GameObject Light1;
	public GameObject Light2;
	public float tgglT;

	// Use this for initialization
	void Start () {
		
		InvokeRepeating ("Toggle", 2f, tgglT);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Switch(){
		Toggle ();
	}

	void Toggle () {
		//while(true){
			Light1.GetComponent<Light>().enabled = !Light1.GetComponent<Light>().enabled;
			Light2.GetComponent<Light>().enabled = !Light2.GetComponent<Light>().enabled;
			//yield WaitForSeconds(tgglT);
		//}
	}

    public void blackout( )
    {
        if(Light1.GetComponent<Light>( ).enabled== Light1.GetComponent<Light>( ).enabled == false)
        {

            Light1.GetComponent<Light>( ).enabled = true;
            Light2.GetComponent<Light>( ).enabled = false;
            InvokeRepeating( "Toggle", 0f, tgglT );
        }
        else
        {
            Light1.GetComponent<Light>( ).enabled = false;
            Light2.GetComponent<Light>( ).enabled = false;
            CancelInvoke( "Toggle" );
        }
    }

}
