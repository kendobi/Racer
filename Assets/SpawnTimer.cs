using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTimer : MonoBehaviour {

	public GameObject prefab;
	public float InstantiationTimer = 2f;

	void Update () {
		CreatePrefab();
	}

	void CreatePrefab()
	{
		InstantiationTimer -= Time.deltaTime;
		if (InstantiationTimer <= 0)
		{
			Instantiate(prefab, gameObject.transform.position, Quaternion.identity);
			InstantiationTimer = 2f;
		}
	}
}
