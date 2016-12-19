using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	private MasterConfig mc;




    private int rec_frame_num = 0;
    private float rec_last_frame_time = 0.0f;

	public MasterConfig MC{
		set{ this.mc = value;}
	}

	// Use this for initialization
	void Start () {
		
	}

	void OnMouseUp() {
	}

    void Update ()
    {
        if (mc.record_active && rec_last_frame_time < Time.time + (1/mc.rec_fps) )
        {
            recordFrame( );
        }

    }

    private void recordFrame()
    {
        Application.CaptureScreenshot( mc.record_absolute_file_name + rec_frame_num++ + ".png" );
        rec_last_frame_time = Time.time;
    }

	// Update is called once per frame
	void LateUpdate () {
		if (mc == null)
			return;
		float x, y, z;
		x = y = z = 0.0f;
		if (Input.GetKey (KeyCode.W))
			z += mc.CameraSpeed;
		
		if (Input.GetKey (KeyCode.A))
			x -= mc.CameraSpeed;
		
		if (Input.GetKey (KeyCode.S))
			z -= mc.CameraSpeed;
		
		if (Input.GetKey (KeyCode.D))
			x += mc.CameraSpeed;
		
		if (Input.GetKey (KeyCode.Space))
			y += mc.CameraSpeed;
		
		if (Input.GetKey (KeyCode.C))
			y -= mc.CameraSpeed;

		if (Input.GetKeyUp (KeyCode.P))
			Time.timeScale = (Time.timeScale < 1.0f) ? 1.0f : 0.0f;

		if (Input.GetKeyUp( KeyCode.N) && Time.timeScale == 0.0f)
			mc.PatientZero.transform.parent.transform.GetComponent<GrowthScript> ().nextTick ();

        if (Input.GetKeyUp( KeyCode.T ) && Time.timeScale == 0.0f)
            mc.Lights[0].transform.parent.parent.GetComponent<LightToggler>( ).Switch( );

        if (Input.GetKeyUp( KeyCode.B ))
            mc.Lights[0].transform.parent.parent.GetComponent<LightToggler>( ).blackout( );


		mc.Camera.transform.parent.Translate (x, y, z);

		if (Input.GetKey (KeyCode.Escape)) {
			Cursor.visible = true;

		}

		if (Input.GetKey (KeyCode.Q))
			Application.Quit ();
		   
        if ( Input.GetKey(KeyCode.Plus))
        {
            if (mc.DayCycleInSeconds + 0.02f > 10.0f)
                mc.DayCycleInSeconds += 0.02f;
            else
                mc.DayCycleInSeconds = 10;
        }
        if (Input.GetKey(KeyCode.Minus))
        {
            if (mc.DayCycleInSeconds - 0.02f > 0.0f)
                mc.DayCycleInSeconds -= 0.02f;
            else
                mc.DayCycleInSeconds = 0;
         
        }
	}
}
