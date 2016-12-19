using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System;

public class BodyA : AbstractPlantBodies {



	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region implemented abstract members of AbstractPlantBodies

	public override void increaseNutritionGain ()
	{
        if (mc.Nutrition <= 0)
        {
            int tmp = mc.Nutrition + mc.nutritionGain;
            mc.Nutrition = tmp;
        }
    }

	public override void rotateTowardsLight(bool rec_run, int rec_val)
	 {
		if (rec_run && rec_val > 0 && this.transform.parent.GetComponent<AbstractPlantBodies>() != null) {
			this.transform.parent.GetComponent<AbstractPlantBodies>().rotateTowardsLight(true,--rec_val);
		}
		this.rotateTowardsLight ();
	
	}

	public override void rotateTowardsLight ()
	{
		float current_intensity = 0.0f;
		Transform l;
		Ray ray;
		RaycastHit hit;
		bool hitFlag;
		float calc;
		for (int i = 0; i< mc.Lights.Length; i++) {
			l = mc.Lights [i].transform;
			if (!l.GetComponent<Light> ().enabled)
				continue;
		
			ray = new Ray (transform.GetChild (0).transform.position, l.transform.position - transform.GetChild (0).transform.position);
				hitFlag = mc.safeRaycast(ray, out hit, l.GetComponent<Light> ().range);

			if( hitFlag ) 
			{
				hitFlag = hit.collider.transform.position == l.transform.position;

				Vector3 a = transform.TransformPoint (this.transform.position);
				Vector3 b = transform.TransformPoint (l.transform.position);
				float dotprod = a.x * b.x + a.y * b.y + a.z * b.z;

				double magnitude_a = Math.Abs (Math.Sqrt (Math.Pow (a.x, 2) + Math.Pow (a.y, 2) + Math.Pow (a.z, 2)));
				double magnitude_b = Math.Abs (Math.Sqrt (Math.Pow (b.x, 2) + Math.Pow (b.y, 2) + Math.Pow (b.z, 2)));
				double rad = (Math.Acos (dotprod / (magnitude_a * magnitude_b))); 
				calc = Convert.ToSingle(rad * Math.PI / 180);

				if (hitFlag && calc <= l.GetComponent<Light> ().spotAngle) {
					rotateToPosition (l.transform, l.GetComponent<Light> ().intensity, current_intensity);
				}
			}
		
		}
	}
	
	public override void expandBandwidth()
	{
        lock (this.transform)
        {
            Transform parent = this.transform.parent;

            if (parent.name != mc.PatientZero.name && parent.GetComponent<AbstractPlantBodies>() != null)
            {
                float tmp = (this.transform.GetChild(0).localScale.x + 8 * mc.WidthSize);
                if (parent.GetChild(0).localScale.x < tmp)
                {
                    parent.GetComponent<AbstractPlantBodies>().expandBandwidth();
                }
            }
            else
            {
                float tmp = mc.Bandwith + mc.bandwithGain;
                mc.Bandwith = tmp;
                mc.MaxBodyScale = this.transform.GetChild(0).localScale.x + 2 * mc.WidthSize;
            }
            Vector3 v = this.transform.GetChild(0).localScale;
            v.x += 2 * mc.WidthSize;
            v.z += 2 * mc.WidthSize;
            this.transform.GetChild(0).localScale = v;
            this.transform.GetComponent<CapsuleCollider>().radius += mc.WidthSize ;
            int index = mc.plantColors.Length - 1;
            if (childCount < index && childCount >= 0)
                index = (int)childCount;
            this.transform.GetChild(0).GetComponent<Renderer>().material.color = mc.plantColors[index];
            if (childCount >= 20)
                this.WeightBend();
        }
	}

    public override void WeightBend()
    {
        //Check if any child has terrain contact
        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).GetComponent<AbstractPlantBodies>() != null && this.transform.GetChild(i).GetComponent<AbstractPlantBodies>().CreeplingContact)
                return;
        }

        //Check if parent has terrain contact.
        Transform parent = this.transform.parent;
        if ( parent.gameObject == mc.PatientZero || parent.GetComponent<AbstractPlantBodies>().CreeplingContact)
        {
            return;
        }
        float per = this.transform.GetChild(0).transform.localScale.x / mc.MaxBodyScale;
        if (per <= mc.ActivateWeightBend ) { 
            Vector3 ltp = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
            GameObject lastchild = this.getLastChildBody();
            Vector3 lastchildpos = lastchild.transform.position;
            ltp.z += 20;

            ltp.y += (lastchildpos.y - this.transform.position.y) *(childCount)* (1 - mc.Stability) * (mc.MaxBodyScale - this.transform.GetChild(0).transform.localScale.x);
            ltp.x += (lastchildpos.x - this.transform.position.x) *(childCount)* (1 - mc.Stability) * (mc.MaxBodyScale - this.transform.GetChild(0).transform.localScale.x);
                
            if ( ltp != this.transform.position)
            {
                int digis = 5;
                Quaternion q = QmMR(mc.safeLerp(transform.rotation, mc.safeLookRotation(V3mMR(ltp, digis) - V3mMR(transform.position, digis)), Time.deltaTime * mc.Stability), digis);
                if (cNEQ(q, this.transform.rotation, digis) || cNEQ(q, new Quaternion(), digis))
                    transform.rotation = q;
            }

        }

    }

    #endregion

    private Boolean cNEQ(Quaternion a, Quaternion b, int digis)
    {

        return (mMR(a.x, digis) != mMR(b.x, digis) || mMR(a.y, digis) != mMR(b.y, digis) || mMR(a.z, digis) != mMR(b.z, digis) || mMR(a.w, digis) != mMR(b.w, digis));
    }

    private Vector3 V3mMR(Vector3 v, int digis)
    {
        v.x = Convert.ToSingle(mMR(System.Convert.ToDouble(v.x), digis));
        v.y = Convert.ToSingle(mMR(System.Convert.ToDouble(v.y), digis));
        v.z = Convert.ToSingle(mMR(System.Convert.ToDouble(v.z), digis));
        return v;
    }

    private Quaternion QmMR(Quaternion q, int digis)
    {
        q.x = Convert.ToSingle(mMR(System.Convert.ToDouble(q.x), digis));
        q.y = Convert.ToSingle(mMR(System.Convert.ToDouble(q.y), digis));
        q.z = Convert.ToSingle(mMR(System.Convert.ToDouble(q.z), digis));
        q.w = Convert.ToSingle(mMR(System.Convert.ToDouble(q.w), digis));
        return q;
    }

    private double mMR(double d, int x)
    {
        return Math.Floor((d * Math.Pow(10, x))) / Math.Pow(10, x);
    }

    void rotateToPosition(Transform target,float light_intensity,float current_intensity){
		current_intensity += light_intensity;
		Vector3 stp = target.position;
		float ls = 0.0001f*(this.transform.localScale.x /mc.MaxBodyScale) ;
		stp.x = this.transform.position.x + ( stp.x - this.transform.position.x ) * (  ls );
		stp.y = this.transform.position.y + ( stp.y - this.transform.position.y ) * (  ls );
		stp.z = this.transform.position.z + ( stp.z - this.transform.position.z ) * (  ls );
		transform.rotation = mc.safeLerp(transform.rotation, mc.safeLookRotation(stp - transform.position), Time.deltaTime * mc.Stability);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<Terrain>() != null)
        {
            this.CreeplingContact = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.transform.GetComponent<Terrain>() != null)
        {
            this.CreeplingContact = false;
        }
    }

    private void release( )
    {
        for(int i = 0; i < this.transform.childCount;i++)
        {
            if (this.transform.GetChild( i ).GetComponent<AgentContent>( ) != null)
                mc.AddRemoveActiveAgents( this.transform.GetChild( i ).gameObject );
        }
    }

    private void restrain()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (this.transform.GetChild( i ).GetComponent<AgentContent>( ) != null && mc.ActiveAgents.Contains( this.transform.GetChild( i ).gameObject )) 
                mc.AddRemoveActiveAgents( this.transform.GetChild( i ).gameObject );
        }
    }

    public void dominate( int dominationStrength )
    {
        if (dominationStrength <= 0)
        {
            release( );
            return;
        }
        else
        {
            restrain( );
        }
            
        Transform myparent = this.transform.parent;
        while (myparent != mc.PatientZero.transform && myparent.GetComponent<BodyA>( ) == null)
            myparent = myparent.parent;
        if (myparent == mc.PatientZero.transform)
            return;

        myparent.GetComponent<BodyA>( ).dominate( dominationStrength-1 );
    }
}
