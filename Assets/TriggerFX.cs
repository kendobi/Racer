using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerFX : MonoBehaviour {

	public GameObject[] FX;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {



		
	}

	private void OnTriggerEnter(Collider other){
	
		for(int i = 0; i < FX.Length; i++)
		{
			FX[i].SetActive(true);
		}
		print ("TRIGGER FX");
	
	
	}
}
