using UnityEngine;
using uLink;
using System.Collections;

public class ClientSide : uLink.MonoBehaviour {

	// Use this for initialization

    void Awake()
    {
        uLink.MasterServer.ipAddress = "127.0.0.1";
        uLink.MasterServer.port = 23466;
        uLink.MasterServer.ClearHostList();
        uLink.MasterServer.RequestHostList("AGOTCG");
    }
	void Start () {
	
	}
	
	// Update is called once per frame
    void Update()
    {
        if (uLink.MasterServer.PollHostList().Length != 0)
        {
            var hostData = uLink.MasterServer.PollHostList();
            for (int i = 0; i < hostData.Length; i++)
            {
                Debug.Log("Game name: " + hostData[i].gameName);
                uLink.MasterServer.ClearHostList();
            }
        }
    }
}
