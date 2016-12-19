using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class GrowthScript : MonoBehaviour {

	public GameObject agent;
	//public Text Hours;
	public Text Days;
	public MasterConfig mc;

    private float d;
    // Use this for initialization
    void Start () {
		if (checkGrammar ()) {
			mc.PauseGrowth = false;
			InvokeRepeating ("TickTock", 2f, mc.DayCycleInSeconds);
            d = mc.DayCycleInSeconds;
            //agent = mc.safeInstantiate(mc.getCurrentGrammarObject(mc.StartWithGrammar), mc.PatientZero.transform.position, mc.PatientZero.transform.rotation) as GameObject;
            if(agent.transform.parent != mc.PatientZero.transform)
                agent.transform.parent = mc.PatientZero.transform;

			agent.GetComponent<AbstractPlantAgent>().ActualGrammar=mc.StartWithGrammar;
			agent.GetComponent<AbstractPlantAgent>().MC = this.mc;
            agent.GetComponent<AbstractPlantAgent>().MainBranch = 0;
            agent.GetComponent<AbstractPlantAgent>().LeadTarget = new GameObject();
            agent.GetComponent<AbstractPlantAgent>().LeadTarget.name = "LeadTarget of " + agent.name;

            foreach (GameObject go in mc.Lights)
                if (go.GetComponent<Light>().enabled)
                    agent.GetComponent<AbstractPlantAgent>().setLeadTargetTo(go.transform.position);

            agent.SetActive(true);
			mc.AddToActiveAgent(agent);
			mc.MaxBodyScale=agent.transform.localScale.x;

			mc.Camera.GetComponent<CameraControl>().MC = this.mc;

		}else
			throw new MissingComponentException("Grammatic contains faulty entry! Recheck recommended...\r\n - Check Grammatic entries\r\n - Check if every Letter points to a body\r\n - Check your startingLetter!");
	}

    float foobar = 0.0f;
	// Update is called once per frame	
	void Update () {

	}

	public void nextTick(){
		TickTock();
	}

	void TickTock(){
		if (mc.PauseGrowth)
			return;
        if (mc.DayCycleInSeconds != d)
        {
            restartInvoke();
            return;
        }

        if(foobar % mc.DailyGrowthSize == 0 && foobar != 0)
        {
            mc.Days++;
            foobar = 0.0f;
        }
            

        lock (mc.LockActiveAgents)
        {
            for (int i = 0; i < mc.ActiveAgents.Count; i++){
                if (mc.ActiveAgents[i] == null)
                    continue;
                else
                    ((GameObject)mc.ActiveAgents[i]).transform.GetComponent<AbstractPlantAgent>().growthPlan();
            }
			
		}

        lock (mc.LockActiveBodies)
        { 
            for (int i = 0; i < mc.ActiveBodies.Count; i++){
				((GameObject)mc.ActiveBodies[i]).transform.GetComponent<AbstractPlantBodies> ().rotateTowardsLight (true,20);
		    }

        }

        foobar += mc.segmentSize;

        mc.updateActivityLists ();

		//Hours.text = "Hours: "+mc.Hours;
		Days.text = "Days: "+mc.Days;
	}
	

	bool checkGrammar(){
		string[] keyval;
		string[] vals;
		GameObject go = null;
		foreach (string s in mc.grammar) {
			if (s.Contains(">"))
			{
				keyval = s.Split('>');

				if (mc.GSExists(keyval[0])){
					vals = mc.getGSVals(keyval[0]);
					Array.Resize(ref vals, vals.Length + 1 );
					vals[vals.Length -1] = keyval[1];
				}else{
					vals = new string[1];
					vals[0] = keyval[1];
				}
				mc.AddGrammarSyntax(keyval[0], vals);
			}
			else if (s.Contains("="))
			{
				keyval = s.Split('=');
				go = mc.safeFind (keyval[1]);
				if (go == null) return false;
				mc.AddGrammarObject(keyval[0], go);
			}
			else
				return false;
		}
        
		foreach (string s in mc.grammar) {
            
            if (s.Contains(">"))
			{	
				keyval = s.Split('>');
				vals = mc.getGSVals(keyval[0]);
                foreach (string sv in vals)
                {
                    string[] splits = sv.Split(mc.DelimiterSign);
                    for (int i = 0; i < splits.Length; i++)
                    {
                        if (splits[i].Equals(mc.END_NODE))
                            continue;
                        if (!(mc.ContainsGrammarObjectKey("" + splits[i])))
                            return false;
                    }
                }
			}
			else
				keyval = s.Split('=');

			if (! (mc.ContainsGrammarObjectKey(keyval[0]))){
					//return false;
			}
		}

		if (!(mc.ContainsGrammarObjectKey (mc.StartWithGrammar)))
			return false;

		return true;
	}


    private void restartInvoke()
    {
        if ( mc.DayCycleInSeconds > 0.000001f)
        {
            CancelInvoke("TickTock");
            InvokeRepeating("TickTock", d, mc.DayCycleInSeconds);
        }
            
        d = mc.DayCycleInSeconds;
    }

	private void ToggleInvoke()
	{
		if(mc.PauseGrowth)
			CancelInvoke("TickTock");
		else
			InvokeRepeating("TickTock",0.01f,mc.DayCycleInSeconds);
		mc.PauseGrowth=!mc.PauseGrowth;
		
	}


}
