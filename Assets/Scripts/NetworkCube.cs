using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCube : MonoBehaviour {

    //The ID of the client that owns this player (so we can check if it's us updating)
    public ushort objectID;
    public bool DEBUG = true;

    Vector3 lastPosition;
    Quaternion lastRotation;
    Vector3 lastScale;

    // Use this for initialization
    void Start () {
		NetworkManager.OnReceiveMessageFromGameObjectUpdate += NetworkManager_OnReceiveMessageFromGameObjectUpdate;
	}

    void NetworkManager_OnReceiveMessageFromGameObjectUpdate (NetworkManager.ReceiveMessageFromGameObject newMessage)
    {
		Debug.Log ("Raise event in GameObject");
		Debug.Log (newMessage.GameObjectID);
    }
	
	// Update is called once per frame
	void Update () {
        if ((Vector3.Distance(lastPosition, transform.position) > 0.05f) || (transform.rotation != lastRotation))
        {
            NetworkManager.instance.SendMessage(0, 1, transform.position, transform.rotation);
        }
        //Update stuff
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }
}
