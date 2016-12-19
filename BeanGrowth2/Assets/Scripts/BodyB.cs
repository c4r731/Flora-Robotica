using UnityEngine;
using System.Collections;
using AssemblyCSharp;
public class BodyB : BodyA {

	public void OnCollisionEnter(Collision other){
		if (other.gameObject.transform.IsChildOf(this.transform) || this.transform.IsChildOf(other.transform))
			return;
		Vector3 dir = this.transform.position - other.transform.position ;
		dir.z = 0;
		this.GetComponent<Rigidbody>().AddForce(dir*0.002f);
	}
	
	public void OnCollisionExit(Collision other){

	}
	
	public void OnCollisionStay(Collision other){

	}
	
	public void OnTriggerEnter(Collider other){
		if (other.gameObject.transform.IsChildOf(this.transform) || this.transform.IsChildOf(other.transform))
			return;
		
		Transform child = this.transform.GetChild(0);
		child.GetComponent<MeshCollider>().enabled = true;

	}
	
	public void OnTriggerExit(Collider other){
		if (other.gameObject.transform.IsChildOf(this.transform) || this.transform.IsChildOf(other.transform))
			return;

		Transform child = this.transform.GetChild(0);
		child.GetComponent<MeshCollider>().enabled = false;

	}
	
	public void OnTriggerStay(Collider other){

	}

}

