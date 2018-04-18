using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteOnStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log("Destroying : " + gameObject.name);
        DestroyImmediate(gameObject);
	}

}
