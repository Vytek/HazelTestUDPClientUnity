using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Events;
using UnityEngine;
using System.Net;
using Hazel;
using Hazel.Udp;
using MessagePack;
using FlatBuffers;
using HazelTest;

/// <summary>
/// Network manager.
/// </summary>
public class NetworkManager : MonoBehaviour {

	public int portNumber = 4296;
	public string ipAddress = "127.0.0.1";

  public GameObject CubeOne;

	public struct ReceiveMessageFromGameObject {
		public int GameObjectID;
		public Vector3 GameObjectPos;
		public Quaternion GameObjectRot;
	};

	private static NetworkManager _instance = null;

	//Events
	public delegate void ReceiveMessageUpdate(ReceiveMessageFromGameObject newMessage);
	public static event ReceiveMessageUpdate OnReceiveMessageFromGameObjectUpdate;

	public enum PacketId {
		PLAYER_JOIN = 0,
		OBJECT_MOVE = 1
	}

	// Client Data
	Connection serverConn;

	/// <summary>
	/// Gets the instance.
	/// </summary>
	/// <value>The instance.</value>
	public static NetworkManager instance {
		get {
			if (_instance == null) {
				_instance = FindObjectOfType<NetworkManager> ();
				if (_instance == null) {
					GameObject go = new GameObject ();
					go.name = "NetworkManager";
					instance = go.AddComponent<NetworkManager> ();
					DontDestroyOnLoad (go);
				}
				Debug.Assert (_instance != null);
			}
			return _instance;
		}
		set {
		}
	}

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		if(_instance == null)
		{
			_instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void Start() {
		Debug.Log("Network idle.");
		StartClient (ipAddress);
		Debug.Log("Network Started.");
		serverConn.SendBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, SendOption.Reliable);
		Debug.Log("Network Trasmitted.");
	}

	void OnDestroy() {
		if (serverConn != null) serverConn.Close();
	}

	#region Client
	public void StartClient(string ipAddress) {
		NetworkEndPoint endPoint = new NetworkEndPoint(ipAddress, portNumber);
		serverConn = new UdpClientConnection(endPoint);
		serverConn.DataReceived += HandleDataFromServer;
		Debug.Log("Connecting to " + endPoint);
		serverConn.Connect();
	}

	void HandleDataFromServer(object sender, DataReceivedEventArgs args) {
		Connection connection = (Connection)sender;
		Debug.Log("Received " + args.Bytes.Length + " bytes from server at " + connection.EndPoint.ToString());
        //connection.SendBytes(args.Bytes, args.SendOption);
        ReceiveMessage(args.Bytes);
		args.Recycle();
	}

	void HandleServerDisconnect(object sender, DisconnectedEventArgs args) {
		Connection connection = (Connection)sender;
		Debug.Log("Server connection at " + connection.EndPoint + " lost");
		serverConn = null;
		args.Recycle();
	}
    #endregion

    #region NetworkLogic
    /// <summary>
    /// SendMessage
    /// </summary>
    /// <param name="Type"></param>
    /// <param name="IDObject"></param>
    /// <param name="Pos"></param>
    /// <param name="Rot"></param>
    public void SendMessage(byte Type, int IDObject, Vector3 Pos, Quaternion Rot) {
        // Create flatbuffer class
        FlatBufferBuilder fbb = new FlatBufferBuilder(1);

        HazelTest.Object.StartObject(fbb);
        HazelTest.Object.AddID(fbb, IDObject);
        Debug.Log("ID SENT: " + IDObject);
        HazelTest.Object.AddPos(fbb, Vec3.CreateVec3(fbb, Pos.x, Pos.y, Pos.z));
        Debug.Log("POS SENT: " + Pos.x.ToString() + ", " + Pos.y.ToString() + ", " + Pos.z.ToString());
        HazelTest.Object.AddRot(fbb, Vec4.CreateVec4(fbb, Rot.x, Rot.y, Rot.z, Rot.w));
        var offset = HazelTest.Object.EndObject(fbb);

        HazelTest.Object.FinishObjectBuffer(fbb, offset);
        //CubeOne.transform.position

        using (var ms = new MemoryStream(fbb.DataBuffer.Data, fbb.DataBuffer.Position, fbb.Offset))
        {
            //Add type?
            //https://stackoverflow.com/questions/5591329/c-sharp-how-to-add-byte-to-byte-array
            serverConn.SendBytes(ms.ToArray(), SendOption.Reliable);
            Debug.Log("Message sent!");
        }
    }

    private void ReceiveMessage(byte[] BufferReceiver)
    {
      //Remove first byte (type)
      //https://stackoverflow.com/questions/31550484/faster-code-to-remove-first-elements-from-byte-array
      ByteBuffer bb = new ByteBuffer(BufferReceiver);

      /*
      if (!HazelTest.Object.))
      {
          throw new Exception("Identifier test failed, you sure the identifier is identical to the generated schema's one?");
      }
      */

      //Please see: https://stackoverflow.com/questions/748062/how-can-i-return-multiple-values-from-a-function-in-c
      HazelTest.Object ObjectReceived = HazelTest.Object.GetRootAsObject(bb);

      Debug.Log("RECEIVED DATA : ");
      Debug.Log("IDObject RECEIVED : " + ObjectReceived.ID);
      Debug.Log("POS RECEIVED: " + ObjectReceived.Pos.X + ", " + ObjectReceived.Pos.Y + ", " + ObjectReceived.Pos.Z);

			var ReceiveMessageFromGameObjectBuffer = new ReceiveMessageFromGameObject();
			ReceiveMessageFromGameObjectBuffer.GameObjectID = ObjectReceived.ID;
      ReceiveMessageFromGameObjectBuffer.GameObjectPos = new Vector3(ObjectReceived.Pos.X, ObjectReceived.Pos.Y, ObjectReceived.Pos.Z);
      ReceiveMessageFromGameObjectBuffer.GameObjectRot = new Quaternion(ObjectReceived.Rot.X, ObjectReceived.Rot.Y, ObjectReceived.Rot.Y, ObjectReceived.Rot.W);

        if (OnReceiveMessageFromGameObjectUpdate != null)
					OnReceiveMessageFromGameObjectUpdate(ReceiveMessageFromGameObjectBuffer);
    }
    #endregion
}
