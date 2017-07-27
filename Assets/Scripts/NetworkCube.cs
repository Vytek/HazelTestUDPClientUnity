using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCube : MonoBehaviour {

    //The ID of the client that owns this player (so we can check if it's us updating)
    public ushort objectID;
    public bool DEBUG = true;

    Vector3 lastPosition = Vector3.zero;
    Quaternion lastRotation = Quaternion.identity;
    Vector3 lastScale;

    // Use this for initialization
    void Start () {
		NetworkManager.OnReceiveMessageFromGameObjectUpdate += NetworkManager_OnReceiveMessageFromGameObjectUpdate;
	}

    void NetworkManager_OnReceiveMessageFromGameObjectUpdate (NetworkManager.ReceiveMessageFromGameObject newMessage)
    {
		Debug.Log ("Raise event in GameObject");
        Debug.Log (newMessage.MessageType);
		Debug.Log (newMessage.GameObjectID);
        Debug.Log (newMessage.GameObjectPos);
        Debug.Log (newMessage.GameObjectRot);
    }
	
	// Update is called once per frame
	void Update () {
        if ((Vector3.Distance(transform.position, lastPosition) > 0.05) || (Quaternion.Angle(transform.rotation, lastRotation) > 0.3))
        {
            NetworkManager.instance.SendMessage(NetworkManager.PacketId.OBJECT_MOVE, 1, transform.position, transform.rotation);
        }
        //Update stuff
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }
}
