using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour {

	//The UID of Player
	public string UID;
	public bool DEBUG = true;

	Vector3 lastPosition = Vector3.zero;
	Vector3 nextPosition = Vector3.zero;
	Quaternion lastRotation = Quaternion.identity;
	Quaternion nextRotation = Quaternion.identity;
	Vector3 lastScale;

	// Use this for initialization
	void Start () {
		NetworkManager.OnReceiveMessageFromGameObjectUpdate += NetworkManager_OnReceiveMessageFromGameObjectUpdate;
		NetworkManager.OnDisconnectedClientUpdate += NetworkManager_OnDisconnectedClientUpdate;
		//Initialize
		lastPosition = transform.position;
		lastRotation = transform.rotation;
	}

	void NetworkManager_OnDisconnectedClientUpdate (NetworkManager.DisconnectedClientToDestroyPlayerGameObject newMessage)
	{
		if (newMessage.PlayerGameObjectUID == this.UID)
		{
			UnityMainThreadDispatcher.Instance().Enqueue(DestroyGameObjectOnTheMainThread());
			Debug.Log ("Raise event in Player GameObject");
		}
	}

	public IEnumerator ThisWillBeExecutedOnTheMainThread()
	{
		//Debug.Log("This is executed from the main thread"); //DEBUG
		//transform.position = new Vector3(newMessage.GameObjectPos.x, newMessage.GameObjectPos.y, newMessage.GameObjectPos.z);
		lastPosition = nextPosition;
		transform.position = nextPosition;
		lastRotation = nextRotation;
		transform.rotation = nextRotation;
		yield return null;
	}

	public IEnumerator DestroyGameObjectOnTheMainThread()
	{
		Destroy (gameObject);
		Debug.Log ("Destroy Player GameObject");
		yield return null;
	}

	void NetworkManager_OnReceiveMessageFromGameObjectUpdate (NetworkManager.ReceiveMessageFromGameObject newMessage)
	{
		if (DEBUG) 
		{
			Debug.Log ("Raise event in Player GameObject");
			Debug.Log (newMessage.MessageType);
			Debug.Log (newMessage.GamePlayerObjectOwner);
			Debug.Log (newMessage.GameObjectPos);
			Debug.Log (newMessage.GameObjectRot);
		}
		//Update pos and rot
		if (newMessage.GamePlayerObjectOwner == UID)
		{
			nextPosition = new Vector3(newMessage.GameObjectPos.x, newMessage.GameObjectPos.y, newMessage.GameObjectPos.z);
			nextRotation = new Quaternion(newMessage.GameObjectRot.x, newMessage.GameObjectRot.y, newMessage.GameObjectRot.z, newMessage.GameObjectRot.w);
			UnityMainThreadDispatcher.Instance().Enqueue(ThisWillBeExecutedOnTheMainThread());
		}
	}
}
