using UnityEngine;
using System.Collections;
using System.Data;

public class ClassesPanel : MonoBehaviour {

    public MapSceneHandler MapSceneHandler;
    public GameObject ClassButtonPrefab;
    public ClassInfo ClassInfo;


	// Use this for initialization
	void Start () {
        DataTable dt = SQL.Query("SELECT * FROM Classes WHERE Type = 'CLASS_LANNISTER'");
      
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            string type = (string)dt.Rows[i]["Type"];

            Debug.Log(type);
            ClassButton ClassButton = GameObject.Instantiate(ClassButtonPrefab).GetComponent<ClassButton>();
            ClassButton.transform.parent = transform;
            ClassButton.ClassType = type;
            ClassButton.ClassInfo = ClassInfo;
            ClassButton.UpdateText();
        }


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
