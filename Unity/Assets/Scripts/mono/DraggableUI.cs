using UnityEngine;
using System.Collections;

public class DraggableUI : MonoBehaviour {

    Vector3 offset;

	// Use this for initialization
	void Start () {
	}


    public void UpdateOffset()
    {
        offset = Input.mousePosition - transform.position;
    }

    public void UpdatePosition()
    {
        transform.position = Input.mousePosition - offset;
    }

	// Update is called once per frame
	void Update () {
	
	}
}
