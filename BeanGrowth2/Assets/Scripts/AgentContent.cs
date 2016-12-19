using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using System;
/***
 *  AgentContent is the processing and personal data storage of an agent.
 * 
 */
public class AgentContent : AbstractPlantAgent {

    /***
	 * agentMovement controls if an agent can move(true) or not(false) 
	 */
    private bool agentMovement = true;

    //private startTrigger = false;

    private Vector3 draft_pos = Vector3.zero;


    /**
	 * lastparObj contains the last non AbstractPlantAgent GameObject created by the agent.
	 * It will be used to modify the activeBodies list.
	 */
    private GameObject lastparObj;

    private bool saf = false;

    private bool dominated = false;

	// Use this for initialization
	void Start () {
		lastparObj = null;
        
    }
	
	// Update is called once per frame
	void Update () {
   
   }

    private void updateLeadTarget()
    {
        Vector3 pos = Vector3.zero;
        float max_light_intensity = 0.0f;
        foreach (GameObject go in mc.Lights)
            if (go.GetComponent<Light>( ).enabled)
                max_light_intensity += go.GetComponent<Light>( ).intensity;
        float tmp = 0.0f;
        bool any = false;

        foreach (GameObject go in mc.Lights)
        {
            if (go.GetComponent<Light>().enabled)
            {
                if (!any)
                    any = true;
                if (pos == Vector3.zero)
                {
                    pos = go.transform.position;
                    tmp = go.GetComponent<Light>( ).intensity;
                    continue;
                }
                pos += pos*(tmp/max_light_intensity ) - go.transform.position*(tmp/max_light_intensity);
            }
        }
        if (!any)
            pos.y = 1.0f;
        setLeadTargetTo(pos);
    }


    /***
     * DONE
     * SIMPLE VERSION: RANDOM POSITION                              -- Done
     * MODERATE VERSION: OPTIMIZED DIRECTION                        -- Done
     * NATURAL VERSION: OPTIMIZED + NEAREST ENLIGHTED FREE SPACE    -- TODO
	 */
	private void correctAngle(){
        //raycasting hier!!!
        //wenn schatten LT = selbe ebene random position bis nicht mehr schatten
        RaycastHit hit = new RaycastHit();
        Ray ray;
        Transform li;
        bool hitflag = false ;
        bool shade_avoidence_syndrome = true;
        for ( int i = 0; i < mc.Lights.Length; i++)
        {
            li = mc.Lights[i].transform;
            if (!li.GetComponent<Light>( ).enabled)
                continue;
            ray = new Ray( transform.GetChild( 0 ).transform.position, li.position - transform.GetChild( 0 ).transform.position );
            hitflag = mc.safeRaycast( ray, out hit, li.GetComponent<Light>( ).range );

            shade_avoidence_syndrome = ( 
										 hit.collider == null || 
										 !(hit.collider.transform.position == li.position) &&
                                         (hit.collider.gameObject.GetComponent<Terrain>( ) == null )
                                         );
        }

        if (shade_avoidence_syndrome && !saf)
        {
            saf = true;
            Vector3 temporaryLTPos;
            //if (DistanceTo(hit.collider.gameObject) < 0.02f)
            {
                temporaryLTPos = LeadTarget.transform.position;
                temporaryLTPos.y = temporaryLTPos.y*0.25f + this.transform.position.y;
            }
            /*else
            {
                System.Random rand = new System.Random( );
                temporaryLTPos.x = LeadTarget.transform.position.x + rand.Next( 1, 10 ) - 5;
                temporaryLTPos.y = this.transform.position.y;
                temporaryLTPos.z = this.transform.position.z + rand.Next( 1, 10 ) - 5;
            }
*/
            this.setLeadTargetTo( temporaryLTPos );
            this.transform.rotation = mc.safeLerp(this.transform.rotation, mc.safeLookRotation( temporaryLTPos - this.transform.position), (1.0f-Convert.ToSingle(DistanceTo(hit.collider.gameObject))) );
           
        }
        else
        {
            rotatePosition( LeadTarget.transform.position );
            saf = shade_avoidence_syndrome || MainBranch>0;
        }
        
    }

    private void rotatePosition(Vector3 target)
    {
        //Berechnung von SubTargetPoint
        //DONE:STP.Position = Target.Position
        //DONE:STP.Position = Neigungskorrektur nach Steifheit
        //TODO:STP.Position = Neigungskorrektur nach Links-Rechts-Dreher
        //DONE:Rotiere zu STP.Position
        Vector3 stp = target;
        stp.x -= (1 - mc.Stability) * (target.x - transform.position.x);
        stp.y -= (1 - mc.Stability) * mc.Stability * (target.x - transform.position.x);
        stp.z -= (1 - mc.Stability) * (target.z - transform.position.z);
        /*
        TODO: if rotation angle is greater than 33 degree don't rotate or reposition LT
        */
        int digis = 5;
        //Quaternion q = QmMR(mc.safeLerp(transform.rotation, mc.safeLookRotation(V3mMR(stp, digis) - V3mMR(transform.position, digis)), Time.deltaTime * mc.Stability), digis);
        Quaternion q = mc.safeLerp(transform.rotation, mc.safeLookRotation(stp - transform.position), Time.deltaTime * mc.Stability);
        //Quaternion q = mc.safeLerp(transform.rotation, mc.safeLookRotation(target.position - transform.position), Time.deltaTime * mc.Stability);
        if (cNEQ(q, this.transform.rotation, digis) && cNEQ(q, new Quaternion(), digis))
            transform.rotation = q;
        return;
    }
	
	/**
	 * Basis funzt
	 */
	void rotateToPosition(Transform target)
    {

        rotatePosition(target.position);

    }

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
	
    /**
	 * Basis funzt
	 */
	public override void growthPlan(){
        if (this.MainBranch == 0)
            updateLeadTarget();
		Transform bo = this.transform.parent;
		while(bo != mc.PatientZero.transform && bo.GetComponent<AbstractPlantBodies>() == null)
			bo = bo.transform.parent;
        if (agentMovement)
        {
            if (bo.GetComponent<AbstractPlantBodies>() != null)
            {
                //if (mc.Nutrition <= 0)
                {
                    //bo.GetComponent<AbstractPlantBodies>().increaseNutritionGain();
                    bo.GetComponent<BodyA>().increaseNutritionGain();
                }
                //if (mc.Bandwith <= 0)
                {
                    //bo.GetComponent<AbstractPlantBodies>().expandBandwidth();
                    bo.GetComponent<BodyA>( ).expandBandwidth( );
                }
            }
            if (mc.SegmentSize > 0 && mc.Nutrition > 0 && mc.Bandwith > 0)
            {
                growIt();

                if (this.MainBranch > 0)
                    this.MainBranch--;
            }

        }
	}
	
	/**
	 * basis funzt
	 */
	void growIt() {
		correctAngle ();

		//Local Var preps
		GameObject par = null;
		String tmp = "Something gone wrong";
		Transform myparent = this.transform.parent;
		int actual_branch = 0;
        bool branch = actualGrammar.Split(mc.DelimiterSign).Length > 1;

        string[] gr = actualGrammar.Split(mc.DelimiterSign);

		foreach (string s in gr) {

            if ( s == mc.END_NODE)
            {
                this.agentMovement = false;
                //mc.AddRemoveActiveAgents(this.gameObject);
                continue;
            }
            //REMOVE OR MODIFY COMMENTS
			// if letter does not contain any Object (no link given), move on // <- is not allowed state! But ok.
			if (mc.ContainsGrammarObjectKey ("" + s))
            {
				par = mc.safeInstantiate (mc.getCurrentGrammarObject ("" + s), transform.position, transform.rotation)as GameObject;


				//####################### NEXT PART IS GOING TO BE AN AGENT #######################
				//if par is an agent there are several modifications that must be done before continuing
				if (par.GetComponent<AbstractPlantAgent>() != null || branch)
                {
                    
                    int mb = 0;
                    //####################### NEXT PART SHOULD BE A BODY BUT ######################
                    // It's branching time so instead of a body a new agent will be created!
                    if (par.GetComponent<AbstractPlantAgent>() == null && branch)
                    {
                        Destroy(par);

                        par = mc.safeInstantiate(this.gameObject, transform.position, transform.rotation) as GameObject;
                        mb = 2;
           
                    }
                    else if (par.GetComponent<AbstractPlantAgent>() != null && !branch)
                    {
                        transform.Translate(0.0f, 0.0f, -mc.SegmentSize, transform.parent.transform);
                    }
                    // set neccessary default values.
                    par.GetComponent<AbstractPlantAgent>().MC = this.mc;
					par.GetComponent<AbstractPlantAgent>().ActualGrammar = "" + s;
					par.GetComponent<AbstractPlantAgent>().ScaleMe= ScaleMe;
                    par.GetComponent<AbstractPlantAgent>().MainBranch = mb;
                    par.GetComponent<AbstractPlantAgent>().LeadTarget = Instantiate(this.LeadTarget, this.LeadTarget.transform.position, this.LeadTarget.transform.rotation) as GameObject ;
                    par.GetComponent<AbstractPlantAgent>().LeadTarget.name = "LeadTarget of " + par.name;
                    if (par.GetComponent<AgentContent>( ) != null)
                    {
                        par.GetComponent<AgentContent>( ).saf = true;
                        par.GetComponent<AgentContent>( ).agentMovement = true;
                    }
                    par.transform.localScale = new Vector3(0.0f,0.0f,0.0f);//this.transform.localScale;
                    par.transform.GetChild(0).localScale = this.transform.GetChild(0).localScale;
                    
                    if(mc.DominationStrength < 1)
                        mc.AddRemoveActiveAgents(par);

                }
                //####################### NEXT PART IS A BODY ##########################
                //if par is a Body
                else if (par.GetComponent<AbstractPlantBodies>() != null)
                {
                    // set neccessary default values.
                    par.GetComponent<AbstractPlantBodies>().MC = this.mc;
					if (lastparObj != null && !branch){
                        //if there was an active body before remove it.
                        mc.AddRemoveActiveBodies(lastparObj);
                    }
                    // add this to activeBodies
                    if (this.MainBranch == 0)
                        mc.AddRemoveActiveBodies(par);
                    lastparObj = par;
                    par.GetComponent<AbstractPlantBodies>().ChildCount = 0;
                    //if actual grammar don't branch then just move forward to simulate the growth
                    //if (!branch)
                    {
                        transform.Translate( 0.0f, 0.0f, mc.SegmentSize, transform );// transform.parent.transform);
                    }
                }
                //####################### NEXT PART IS A LEAF ##########################
                else if (par.GetComponent<AbstractPlantLeaf>() != null)
                {
                    this.agentMovement = false;
                    if ( par.GetComponent<BeanLeaf>() != null)
                    {
                        par.transform.GetChild(0).GetComponent<MeshRenderer>( ).materials[0].CopyPropertiesFromMaterial(mc.safeFind( "bean3" ).GetComponent<MeshRenderer>( ).materials[0]);
                        par.transform.Translate( 0.0f, 0.0f, -mc.SegmentSize, par.transform );
                    }
                    //mc.AddRemoveActiveAgents( this.gameObject );
                }
                //####################### NEITHER AGENT NOR BODY #######################
                //########################### TIME TO DIE ##############################
                //something gone terribly wrong
                //irreparable state! time to destroy this agent
                else
                {
					mc.AddRemoveActiveAgents(this.gameObject);
                    this.agentMovement = false;
                    return;
				}
                //set its name
                par.name = s+ "-" + mc.Name_Val++;
				//rotate it correctly
				par.transform.rotation = transform.rotation;
				//scale preperations
				Vector3 scaltmp = this.ScaleMe;
				scaltmp.x =(ScaleMe.x - mc.WidthSize < mc.WidthSize)?ScaleMe.x:ScaleMe.x - mc.WidthSize;
				scaltmp.z =(ScaleMe.z - mc.WidthSize < mc.WidthSize)?ScaleMe.z:ScaleMe.z - mc.WidthSize;
                scaltmp.y = mc.SegmentSize/0.01f ;
				this.ScaleMe = scaltmp;
				//modify child objects
				par.transform.GetChild(0).localScale = ScaleMe;
				par.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color=mc.plantColors[0];
				par.SetActive (true);

				// the new body needs additional nutrition! take it from stock
				//mc.Nutrition--;

				// shrink transport bandwith of nutrition to simulate stagnating transport efficiency.
			    mc.Bandwith--;

				// Find first AbstractPlantBodies ancestor and set parent to it
				// is needed for PlantBody intercom's
				
				while(myparent != mc.PatientZero.transform && myparent.GetComponent<AbstractPlantBodies>() == null)
					myparent = myparent.parent;
                if (myparent != mc.PatientZero.transform)
                    myparent.GetComponent<AbstractPlantBodies>().ChildCount++;
                //set pars parent
                    if (par.transform.parent != myparent)
				        par.transform.parent = myparent;

                if ( branch){
					setBranchLookAtPosition(actual_branch++, par);
                     
				}
				else if (par.GetComponent<AbstractPlantBodies>() != null)
                {
					//set this new par as agents parent
					this.transform.parent = par.transform;
				}
            }
            
			tmp = s;
		}
        if (branch)
            mc.Nutrition /= actualGrammar.Split( mc.DelimiterSign ).Length;
        else
            mc.Nutrition--;

        if (par != null && par.transform.GetComponent<AbstractPlantAgent>() != null && branch)
            par.transform.Translate(0.0f, 0.0f, mc.SegmentSize, par.transform.parent.transform);

        if (agentMovement)
            agentMovement = DistanceTo( LeadTarget ) > 0;
        else
        {
            mc.AddRemoveActiveAgents( this.gameObject );
            //Destroy( this.gameObject );
        }
        actualGrammar = "" + tmp;
		getComplexNextGrammar ();
        if(mc.dominationStrength > 0)
            dominate( );
    }


    private void dominate( )
    {
        Transform myparent = this.transform.parent;
        while (myparent != mc.PatientZero.transform && myparent.GetComponent<BodyA>( ) == null)
            myparent = myparent.parent;
        if (myparent == mc.PatientZero.transform)
            return;
        
        if(agentMovement)
            myparent.GetComponent<BodyA>( ).dominate( mc.DominationStrength );
        else
            myparent.GetComponent<BodyA>( ).dominate( 0 );

        //cheat to not dominate itself
        mc.AddRemoveActiveAgents( this.gameObject );
    }

    private double DistanceTo(GameObject target)
    {
        double value = 0.0f ;

        Vector3 tmp = target.transform.position - this.transform.position;

        value = tmp.x * tmp.x + tmp.y * tmp.y + tmp.z * tmp.z;
        value = Math.Abs(Math.Sqrt(value));

            return value;
    }

    
	private void setBranchLookAtPosition(int actual_branch, GameObject par){

        if ( par.GetComponent<AbstractPlantAgent>() != null)
        {
            //double angle = ((actual_branch % 2 == 0) ? (-1) : 1) * actual_branch * mc.splitRadius;
            //Random rand = new Random();

            double angle = ((actual_branch %2 == 0)?(-1):1) * ((actual_branch%2==0)?actual_branch:actual_branch-1) *(210 / (actualGrammar.Split(mc.DelimiterSign).Length+1));
            Vector3 tmp = par.GetComponent<AbstractPlantAgent>().LeadTarget.transform.position;
            float tmp2 = (float)(Math.Cos(angle) * tmp.x - Math.Sin(angle) * tmp.y);
            float tmp3 = (float)(Math.Sin(angle) * tmp.x + Math.Cos(angle) * tmp.y);
            float tmp4 = (float)(Math.Sin(angle) * tmp.z + Math.Cos(angle) * ((actual_branch % 3 == 2)?tmp.y:tmp.x));
            tmp.x = tmp2/((actual_branch % 3 == 2) ? 1:2);
            tmp.y = tmp3;
            tmp.z = tmp4;
            par.GetComponent<AbstractPlantAgent>().LeadTarget.transform.position = tmp;

        }


    }

    /**
	 * DONE: simple Grammatic selection
	 */
    private void getNextGrammar(){
		if (mc.GSExists (actualGrammar))
			actualGrammar = mc.getGSVals (actualGrammar) [0];
		else {
			//if there is no next Grammar to this (possible state)
			agentMovement = false;
			//stop agent by removing it from the activeList
			//mc.AddRemoveActiveAgents(this.gameObject);
			//Destroy(this);
		}
	}

	/**
	 *  DONE: complex Grammatic selection
	 */
	private void getComplexNextGrammar(){
		if (mc.GSExists (actualGrammar)) {
			String[] gs = mc.getGSVals (actualGrammar);
			if (mc.Nutrition > mc.BranchBarrier && gs.Length > 1){
				actualGrammar = gs [UnityEngine.Random.Range (1, gs.Length - 1)];
				mc.Nutrition  /= 2;
			}else
				actualGrammar = gs[0];
		}else {
			//if there is no next Grammar to this (possible state)
			agentMovement = false;
			//stop agent by removing it from the activeList
			//mc.AddRemoveActiveAgents(this.gameObject);
		}
	}


    /**
     * TODO: 
     */
	private void getNaturalNextGrammar(){

	}

    void OnTriggerEnter( Collider other )
    {
        /*
        if ((other.GetComponent<AbstractPlantAgent>( ) != null))//|| other.GetComponent<AbstractPlantBodies>() != null))
        {

            //draft_pos = other.transform.position;
            this.MainBranch = 5;
            Vector3 tmp = LeadTarget.transform.position;
            tmp.z = tmp.y;
            tmp.y = LeadTarget.transform.position.z;
            setLeadTargetTo( tmp );//LeadTarget.transform.position );
        }

        if (other.transform.GetComponent<Terrain>( ) != null)
            this.MainBranch = 0;

        //startTrigger = true;
        */
    }

    void OnTriggerStay( Collider other )
    {

        if ((other.GetComponent<AbstractPlantAgent>( ) != null))//|| other.GetComponent<AbstractPlantBodies>() != null))
        {

            //draft_pos = other.transform.position;
            //this.MainBranch = 5;
            //Vector3 tmp = LeadTarget.transform.position;
            //tmp.z = tmp.y;
            //tmp.y = LeadTarget.transform.position.z;
            //setLeadTargetTo( tmp );//LeadTarget.transform.position );
        }

        //if (other.transform.GetComponent<Terrain>( ) != null)
            //this.MainBranch = 0;

        //startTrigger = true;
    }



    void OnTriggerExit(Collider other)
    {
        //startTrigger = other.GetComponent<Terrain>() != null;
        //draft_pos = Vector3.zero;
    }
	
}
