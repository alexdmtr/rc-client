using UnityEngine;
using System.Collections;
using System.Data;
using System.Collections.Generic;
using CardWar.Models;

public class ClassesPanel : MonoBehaviour {

    public MapSceneHandler MapSceneHandler;
    public GameObject ClassButtonPrefab;
    public ClassInfo ClassInfo;


	// Use this for initialization
	void Start () {
        //DataTable dt = SQL.Query("SELECT * FROM Classes WHERE Type = 'CLASS_LANNISTER'");
        List<ClassModel> classes = Db.Classes;

        foreach (ClassModel c in classes)
        {
            string type = c.Type;
            
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
