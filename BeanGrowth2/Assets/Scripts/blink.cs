using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class blink : MonoBehaviour {
    private bool flag;
    // Use this for initialization
    void Start () {
        flag = true;
        InvokeRepeating( "FlashText", 1f, 0.75f );

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    
    void FlashText()
    {
        this.GetComponent<Text>( ).enabled = flag;
        flag = !flag;
    }
}
