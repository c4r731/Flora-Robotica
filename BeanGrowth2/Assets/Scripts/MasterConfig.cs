using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;


public class MasterConfig : MonoBehaviour {
    [Header( "Time and growth handling" )]
    [Range(0.005f, 0.99f)]
    public float DayCycleInSeconds;
    [Range( 0.0f, 1.0f )]
    public float segmentSize;
    [Range( 0.0f, 1.0f )]
    public float daily_growth_size;
    public float widthSize;
	public bool pauseGrowth;

    public GameObject PatientZero;
    [Header( "Initial parameters" )]
    public int name_val;
	public int nutrition;
    public int nutritionGain;
	public float bandwith;
    public float bandwithGain;
    public int BranchBarrier;
    public Color[] plantColors;


    [Header( "Additional parameters" )]
    [Range( -1, 256 )]
    public int dominationStrength;
    [Range(0.0f, 1.0f)]
    public float stability;
    [Range(0.0f, 1.0f)]
    public float ActivateWeightBend;

    [Header( "Plant Grammatic" )]
    public string[] grammar;
	public string StartWithGrammar;
	public char DelimiterSign;
    public string END_NODE;
	//public float splitRadius;
	public GameObject[] grammarObjects;
    [Header( "Leaf configuration" )]
    public float leafMaxSize;
    public float leafSizeStep;
    public float leafAgingStep;
    public float leafMaxAge;
    public float leafEnergy;
    [Range( 0.0f, 1.0f )]
    public float leafEfficiency;
    [Header( "Lights" )]
    public GameObject[] Lights;
    [Header( "Camera" )]
    public GameObject Camera;
	public float CameraSpeed;
    [Header( "Record_Simulation" )]
    public bool record_active;
    public string record_absolute_file_name;
    [Range(15,90)]
    public int rec_fps;


    //#####################THREAD LOCKS##############
    //*************LOCAL LOCKS

    private Object Agent2AgentLock = new Object(); //LOCK für inter agent com.
	private Object Agent2BeanLock = new Object(); // LOCK für agent Bean com.

	private Object TimeLock = new Object(); 
	private Object NutritionLock = new Object(); //LOCK für die Nahrungszufuhr
	private Object BandwithLock = new Object();  //LOCK für die Bandbreitenzufuhr
	private Object GrammarAccessLock = new Object (); //LOCK für den Zugriff auf die Grammatik
	private Object GrammarObjectAccessLock = new Object(); //LOCK für den Zugriff auf die Grammatik Objekte
	private Object FindLock = new Object(); //LOCK für die static Find Funktion von Unity
	private Object MaxBodyScaleLock = new Object();
    private Object LerpLock = new Object();
    private Object LookRotLock = new Object();
    private Object leafMaxSizeLock = new Object( );
    private Object leafSizeStepLock = new Object( );
    private Object leafAgingStepLock = new Object( );
    private Object leafMaxAgeLock = new Object( );
    private Object leafEnergyLock = new Object( );
    private Object leafEfficiencyLock = new Object( );
    private Object dominationStrengthLock = new Object( );
    //*************GLOBAL LOCKS
    private Object instatiateLock = new Object (); 
	private Object rayCastLock = new Object(); 
	private Object vector3AngleLock = new Object ();
    private Object speedUpLock = new Object();
	private Object lockActiveAgents = new Object (); 
	private Object lockActiveBodies = new Object ();
    private Object lockActiveWeights = new Object();
    private Object lockRemAgentList = new Object();
	private Object lockRemBodyList = new Object();
    private Object lockDGS = new Object();
	private Object lockGrowthPause = new Object();
	//#######################SOMETHING ELSE##########
	private bool TockLock;
	private int hours;
	private int days;
	private float maxBodyScale;
	private ArrayList activeAgents = new ArrayList();
	private ArrayList activeBodies = new ArrayList();
	private ArrayList remAgentList = new ArrayList();
	private ArrayList remBodyList = new ArrayList();
    private ArrayList activeWeights = new ArrayList();

    //######################GROWTH GRAMMATICS#########
    private SortedDictionary<string, GameObject> grammarobs = new SortedDictionary<string, GameObject>();
	private SortedDictionary<string, string[]> grammardef = new SortedDictionary<string, string[]>();

	//Abfrage findet von Agenten statt. MUSS Thread sicher sein
	public float Stability{
		get{ return stability;}
		set{ stability = value;}
	}

    //##############REMEMBER THERE IS A BETTER WAY FOR LOCKONWRITE
    //SEE: https://msdn.microsoft.com/en-us/library/system.threading.readerwriterlock.aspx


    //Abfrage findet von Agenten statt. MUSS Thread sicher sein
    public float Bandwith{
		get{
            lock (BandwithLock)
            {
                return bandwith;
            }
        }
		set{
            lock (BandwithLock)
            {
                bandwith = value;
            }
        }
	}

	//Abfrage findet von Agenten statt. MUSS Thread sicher sein
	public int Nutrition{
		get{ lock (NutritionLock) { return nutrition;  } }
		set{ lock (NutritionLock) { nutrition = value; } }
	}

	//Abfrage findet von Agenten statt. MUSS Thread sicher sein
	public int Name_Val{
		get{ lock (Agent2AgentLock) { return name_val;  } }
		set{ lock (Agent2AgentLock) { name_val = value;	} }
	}

	//Abfrage findet von Agenten statt. MUSS Thread sicher sein
	public int Days{
		get{ lock (TimeLock) { return days;  } }
		set{ lock (TimeLock) { days = value; } }
	}

	//Abfrage findet von Agenten statt. MUSS Thread sicher sein
	public int Hours{
		get{ lock (TimeLock) { return hours;  } }
		set{ lock (TimeLock) { hours = value; } }
	}

	//Abfrage findet von Agenten statt. MUSS Thread sicher sein
	public float SegmentSize{
		get{ lock (Agent2BeanLock) { return segmentSize;  } }
		set{ lock (Agent2BeanLock) { segmentSize = value; } }
	}

    //Abfrage 
    public float DailyGrowthSize
    {
        get { lock(lockDGS) { return daily_growth_size; } }
        set { lock(lockDGS) { this.daily_growth_size = value; } }
    }

	public bool PauseGrowth
	{
		get{ lock (lockGrowthPause) { return pauseGrowth; } }
		set{ lock (lockGrowthPause) { this.pauseGrowth=value; } }
	}

	//Abfrage findet von Agenten statt. MUSS Thread sicher sein
	public float WidthSize{
		get{ lock (Agent2BeanLock) { return widthSize;  			} }
		set{ lock (Agent2BeanLock) { widthSize = Mathf.Abs(value);	} }
	}

	//Abfrage findet von BodyA statt
	public float MaxBodyScale{
		get{lock(MaxBodyScaleLock) return maxBodyScale;}
		set{lock(MaxBodyScaleLock) maxBodyScale = value;}
	}

	//Abfrage findet von TODO
	public Object InstatiateLock{
		get{ return instatiateLock; }
		set{ instatiateLock = value; }
	}

	//Abfrage findet von AgentContent BodyA statt
	public Object RayCastLock{
		get{ return rayCastLock; }
		set{ rayCastLock = value; }
	}

	//Abfrage findet von AgentContent, BodyA statt
	public Object Vector3AngleLock{
		get{ return vector3AngleLock; }
		set{ vector3AngleLock = value; }
	}

    public Object SpeedUpLock
    {
        get { return speedUpLock; }
        set { this.speedUpLock = value; }
    }

	//Abfrage findet von THIS statt
	public Object LockActiveAgents{
		
		get{return lockActiveAgents;}
		set{lockActiveAgents = value;}
		
	}

	//Abfrage findet von GrowthScript statt
	public Object LockActiveBodies{
		
		get{return lockActiveBodies;}
		set{lockActiveBodies = value;}
		
	}

    public ArrayList ActiveWeights
    {
        get { lock (lockActiveWeights) return activeWeights; }
        set { lock (lockActiveWeights) activeWeights = value; }
    }

	//Abfrage findet von GrowthScript statt
	public ArrayList ActiveAgents{
		get{lock(lockActiveAgents) return activeAgents;}
		set{lock(lockActiveAgents) activeAgents = value;}
	}

	//Abfrage findet von GrowthScript statt
	public ArrayList ActiveBodies{
		get{lock(lockActiveBodies) return activeBodies;}
		set{lock(lockActiveBodies) activeBodies = value;}
	}

    public float LeafMaxSize
    {
        get
        { lock(leafMaxSizeLock)return leafMaxSize;}
        set
        { lock(leafMaxSizeLock) this.leafMaxSize = value; }
    }

    public float LeafSizeStep
    {
        get
        { lock (leafSizeStepLock) return leafSizeStep; }
        set
        { lock (leafSizeStepLock) this.leafSizeStep = value; }
    }

    public float LeafAgingStep
    {
        get
        { lock (leafAgingStepLock) return leafAgingStep; }
        set
        { lock (leafAgingStepLock) this.leafAgingStep = value; }
    }

    public float LeafMaxAge
    {
        get
        { lock (leafMaxAgeLock) return leafMaxAge; }
        set
        { lock (leafMaxAgeLock) this.leafMaxAge = value; }
    }

    public float LeafEnergy
    {
        get
        { lock (leafEnergyLock) return leafEnergy; }
        set
        { lock (leafEnergyLock) this.leafEnergy = value; }
    }

    public float LeafEfficiency
    {
        get
        { lock (leafEfficiencyLock) return leafEfficiency; }
        set
        { lock (leafEfficiencyLock) this.leafEfficiency = value; }
    }

    public int DominationStrength
    {
        get { lock(dominationStrengthLock)return dominationStrength; }
        set { lock(dominationStrengthLock) this.dominationStrength = value; }
    }

    public void AddRemoveActiveAgents(GameObject agent){
		lock (lockRemAgentList) {
			if (agent.GetComponent<AbstractPlantAgent>()!=null)
				if ( remAgentList.Contains(agent))
					remAgentList.Remove(agent);
				else
				remAgentList.Add(agent);
		}
	}

	//Abfrage findet von AgentContent statt
	public void AddToActiveAgent(GameObject agent){
		if (agent.transform.GetComponent<AbstractPlantAgent> () == null || activeAgents.Contains (agent))
			return;
		lock(lockActiveAgents)
			activeAgents.Add (agent);
	}
	
	//Abfrage findet von AgentContent statt
	public void RemoveFromActiveAgent(GameObject agent){
		if (agent.transform.GetComponent<AbstractPlantAgent> () == null || !activeAgents.Contains (agent))
			return;
		lock(lockActiveAgents)
			activeAgents.Remove (agent);
	}

	public void AddRemoveActiveBodies(GameObject body){
		lock (lockRemBodyList) {
			if (body.GetComponent<AbstractPlantAgent>()!=null)
				if ( remBodyList.Contains(body))
					remBodyList.Remove(body);
			else
				remBodyList.Add(body);
		}
	}

	public void updateActivityLists(){
		lock (lockRemAgentList)
        {
            lock (lockActiveAgents)
            {
                foreach (GameObject go in remAgentList)
                {
                    if (activeAgents.Contains( go ))
                        activeAgents.Remove( go );
                    else
                        activeAgents.Add( go );
                    
                }
            }
            remAgentList.Clear( );
		}

		lock (lockRemBodyList) {
            lock (LockActiveBodies)
            {
                foreach (GameObject go in remBodyList)
                {
                    if (activeBodies.Contains( go ))
                        activeBodies.Remove( go );
                    else
                        activeBodies.Add( go );
                }
            }
            remBodyList.Clear( );
		}
	}

	//Abfrage findet von AgentContent, BodyA statt
	public void AddToActiveBodies(GameObject body){
		if (body.transform.GetComponent<AbstractPlantBodies> () == null || activeBodies.Contains (body))
			return;
		lock(lockActiveBodies)
			activeBodies.Add (body);
	}
	
	//Abfrage findet von AgentContent, BodyA statt
	public void RemoveFromActiveBodies(GameObject body){
		if (body.transform.GetComponent<AbstractPlantBodies> () == null || !activeBodies.Contains (body))
			return;
		lock(lockActiveBodies)
			activeBodies.Remove (body);
	}

	//Abfrage findet von Agenten statt. MUSS Thread sicher sein
	public GameObject getCurrentGrammarObject(string cur){
		lock (GrammarObjectAccessLock) {
			return grammarobs[cur];
		}
	}

	//Abfrage findet nur von GrowthScript statt.
	public void AddGrammarObject (string key, GameObject go){
		lock (GrammarObjectAccessLock) {
			grammarobs.Add(key, go);
		}
	}

	//Abfrage findet von GrowthScript stattt
	public bool ContainsGrammarObjectKey(string key){
		lock (GrammarObjectAccessLock) {
			return grammarobs.ContainsKey (key);
		}
	}

	//Abfrage findet von GrowthScript, ContentAgent
	public bool ContainsGrammarObject(GameObject go) {
		lock (GrammarObjectAccessLock) {
			return grammarobs.ContainsValue(go);
		}
	}

	//Abfrage findet nur von GrowthScript statt.
	public void AddGrammarSyntax(string key, string[] value){
		lock (GrammarAccessLock) {
			if(grammardef.ContainsKey(key))
				grammardef.Remove(key);
			grammardef.Add(key, value);
		}
	}

	//Abfrage findet nur von GrowthScript statt.
	public bool GSExists(string key){
		return grammardef.ContainsKey (key);
	}

	//Abfrage findet nur von GrowthScript statt.
	public string[] getGSVals(string key){
		return grammardef [key];
	}

    // Use this for initialization
    void Start () {
		
		hours = 0;
		days = 0;

	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public Quaternion safeLerp(Quaternion a, Quaternion b, float t)
    {
        
        lock (LerpLock)
        {
            if (Quaternion.Angle(a, b) == 0)
                return a;
            return Quaternion.Slerp(a, b, t);
        }
    }

    public Quaternion safeLookRotation(Vector3 pos)
    {
        if (pos == this.transform.position || pos == Vector3.zero)
            return this.transform.rotation;
        lock (LookRotLock)
        {
            return Quaternion.LookRotation(pos);
        }
    }

    public UnityEngine.Object safeInstantiate(UnityEngine.Object original, Vector3 position, Quaternion rotation)
    {
        lock (instatiateLock)
        {
            return Instantiate(original, position, rotation);
        }
    }

    public bool safeRaycast(Ray ray, out RaycastHit hitInfo, float maxDistance)
    {
        lock (rayCastLock)
        {
            return Physics.Raycast(ray, out hitInfo, maxDistance);
        }
    }

    //Abfrage findet von jedem Statt.
    public GameObject safeFind( string name )
    {
        GameObject val = null;
        lock (FindLock)
        {
            val = GameObject.Find( name );
        }
        return val;
    }


}
