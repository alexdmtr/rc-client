using UnityEngine;
using System.Collections;

public class Video : MonoBehaviour {

	// Use this for initialization
    public MovieTexture movieTexture;
    
	void Start () {
        GetComponent<Renderer>().material.mainTexture = movieTexture;
        movieTexture.Play();
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
