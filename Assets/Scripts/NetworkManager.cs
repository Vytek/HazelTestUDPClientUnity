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

	private static NetworkManager _instance = null;

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
    public void SendMessage(int Type, int IDObject, Vec3 Pos, Vec4 Rot) {
        // Create flatbuffer class
        FlatBufferBuilder fbb = new FlatBufferBuilder(1);

        HazelTest.Object.StartObject(fbb);
        HazelTest.Object.AddID(fbb, IDObject);
        HazelTest.Object.AddPos(fbb, Vec3.CreateVec3(fbb, Pos.X, Pos.Y, Pos.Z));
        HazelTest.Object.AddRot(fbb, Vec4.CreateVec4(fbb, Rot.X, Rot.Y, Rot.Z, Rot.W));
        var offset = HazelTest.Object.EndObject(fbb);

        HazelTest.Object.FinishObjectBuffer(fbb, offset);
        //CubeOne.transform.position

        using (var ms = new MemoryStream(fbb.DataBuffer.Data, fbb.DataBuffer.Position, fbb.Offset))
        {
            serverConn.SendBytes(ms.ToArray(), SendOption.Reliable);
            Debug.Log("Message sent!");
        }
    }

    public void RecieveMessage(byte[] BufferReciever)
    {
        ByteBuffer bb = new ByteBuffer(BufferReciever);

        /*
        if (!HazelTest.Object.))
        {
            throw new Exception("Identifier test failed, you sure the identifier is identical to the generated schema's one?");
        }
        */

        HazelTest.Object ObjectRecieved = HazelTest.Object.GetRootAsObject(bb);

        Debug.Log("LOADED DATA : ");
        Debug.Log("POS : " + ObjectRecieved.Pos.X + ", " + ObjectRecieved.Pos.Y + ", " + ObjectRecieved.Pos.Z);
    }
    #endregion
}