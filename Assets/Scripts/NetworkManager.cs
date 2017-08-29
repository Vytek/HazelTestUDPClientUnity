using System.Collections;
using System.Collections.Generic;
using System;
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
        public sbyte MessageType;
        public int GameObjectID;
		public Vector3 GameObjectPos;
		public Quaternion GameObjectRot;
	};

	private static NetworkManager _instance = null;

	//Events
	public delegate void ReceiveMessageUpdate(ReceiveMessageFromGameObject newMessage);
	public static event ReceiveMessageUpdate OnReceiveMessageFromGameObjectUpdate;

	/// <summary>
	/// Send type.
	/// </summary>
	public enum SendType {
		SENDTOALL = 0,
		SENDTOOTHER = 1,
		SENDTOSERVER = 2
	}

	/// <summary>
	/// Packet identifier.
	/// </summary>
	public enum PacketId: sbyte {
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
		
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start() {
		CustomLogger.LogIt ("--- Starting Logging ---");
		Debug.Log("Network idle.");
		StartClient (ipAddress);
		Debug.Log("Network Started.");
		serverConn.SendBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, SendOption.Reliable);
		Debug.Log("Network Trasmitted.");
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
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
    public void SendMessage(SendType SType, PacketId Type, int IDObject, Vector3 Pos, Quaternion Rot) {
        sbyte TypeBuffer = 0;
        byte STypeBuffer = 0;

        //Choose who to send message
        switch (SType)
        {
            case SendType.SENDTOALL:
                STypeBuffer = 0;
                break;
            case SendType.SENDTOOTHER:
                STypeBuffer = 1;
                break;
            case SendType.SENDTOSERVER:
                STypeBuffer = 2;
                break;
            default:
                STypeBuffer = 0;
                break;
        }
        Debug.Log("SENDTYPE SENT: " + STypeBuffer);

        //Choose type message (TO Modify)
        switch (Type)
        {
            case PacketId.PLAYER_JOIN:
                TypeBuffer = 0;
                break;
            case PacketId.OBJECT_MOVE:
                TypeBuffer = 1;
                break;
            default:
                TypeBuffer = 1;
                break;
        }
        Debug.Log("TYPE SENT: " + TypeBuffer);

        // Create flatbuffer class
        FlatBufferBuilder fbb = new FlatBufferBuilder(1);

        HazelTest.Object.StartObject(fbb);
        HazelTest.Object.AddType(fbb, TypeBuffer);
        HazelTest.Object.AddID(fbb, IDObject);
        Debug.Log("ID SENT: " + IDObject);
        HazelTest.Object.AddPos(fbb, Vec3.CreateVec3(fbb, Pos.x, Pos.y, Pos.z));
        Debug.Log("POS SENT: " + Pos.x.ToString() + ", " + Pos.y.ToString() + ", " + Pos.z.ToString());
        HazelTest.Object.AddRot(fbb, Vec4.CreateVec4(fbb, Rot.x, Rot.y, Rot.z, Rot.w));
        Debug.Log("ROT SENT: " + Rot.x.ToString() + ", " + Rot.y.ToString() + ", " + Rot.z.ToString() + ", " + Rot.w.ToString());
        var offset = HazelTest.Object.EndObject(fbb);

        HazelTest.Object.FinishObjectBuffer(fbb, offset);
        //CubeOne.transform.position

        using (var ms = new MemoryStream(fbb.DataBuffer.Data, fbb.DataBuffer.Position, fbb.Offset))
        {
            //Add type!
            //https://stackoverflow.com/questions/5591329/c-sharp-how-to-add-byte-to-byte-array
            byte[] newArray = new byte[ms.ToArray().Length + 1];
            ms.ToArray().CopyTo(newArray, 1);
            newArray[0] = STypeBuffer;
            serverConn.SendBytes(newArray, SendOption.Reliable);
            Debug.Log("Message sent!");
        }
    }

	/// <summary>
	/// Receives the message.
	/// </summary>
	/// <param name="BufferReceiver">Buffer receiver.</param>
    private void ReceiveMessage(byte[] BufferReceiver)
    {
        //Remove first byte (type)
        //https://stackoverflow.com/questions/31550484/faster-code-to-remove-first-elements-from-byte-array
        byte STypeBuffer = BufferReceiver[0]; //This is NOT TypeBuffer ;-)
        byte[] NewBufferReceiver = new byte[BufferReceiver.Length - 1];
        Array.Copy(BufferReceiver, 1, NewBufferReceiver, 0, NewBufferReceiver.Length);
        ByteBuffer bb = new ByteBuffer(NewBufferReceiver);

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
        Debug.Log("ROT RECEIVED: " + ObjectReceived.Rot.X + ", " + ObjectReceived.Rot.Y + ", " + ObjectReceived.Rot.Z + ", "+ ObjectReceived.Rot.W);
        var ReceiveMessageFromGameObjectBuffer = new ReceiveMessageFromGameObject();
        sbyte TypeBuffer = ObjectReceived.Type;

		if ((byte)PacketId.PLAYER_JOIN == ObjectReceived.Type)
        {
            //Code for new Player
            //Spawn something?
            //Using Dispatcher?
            Debug.Log("New Player!");
		} else if ((byte)PacketId.OBJECT_MOVE == ObjectReceived.Type)
        {
            ReceiveMessageFromGameObjectBuffer.MessageType = ObjectReceived.Type;
            ReceiveMessageFromGameObjectBuffer.GameObjectID = ObjectReceived.ID;
            ReceiveMessageFromGameObjectBuffer.GameObjectPos = new Vector3(ObjectReceived.Pos.X, ObjectReceived.Pos.Y, ObjectReceived.Pos.Z);
            ReceiveMessageFromGameObjectBuffer.GameObjectRot = new Quaternion(ObjectReceived.Rot.X, ObjectReceived.Rot.Y, ObjectReceived.Rot.Y, ObjectReceived.Rot.W);

            if (OnReceiveMessageFromGameObjectUpdate != null)
                OnReceiveMessageFromGameObjectUpdate(ReceiveMessageFromGameObjectBuffer);
        } 
    }
    #endregion

	#region Utility
	//https://stackoverflow.com/questions/29693870/conversion-between-vector3-coordinates-and-string
	public static string Vector3ToString(Vector3 v){ // change 0.00 to 0.0000 or any other precision you desire, i am saving space by using only 2 digits
		return string.Format("{0:0.00},{1:0.00},{2:0.00}", v.x, v.y, v.z);
	}

	public static Vector3 Vector3FromString(String s){
		string[] parts = s.Split(new string[] { "," }, StringSplitOptions.None);
		return new Vector3(
			float.Parse(parts[0]),
			float.Parse(parts[1]),
			float.Parse(parts[2]));
	}

	//http://answers.unity3d.com/questions/1134997/string-to-vector3.html
	public static Vector3 StringToVector3(string sVector)
	{
		// Remove the parentheses
		if (sVector.StartsWith ("(") && sVector.EndsWith (")")) {
			sVector = sVector.Substring(1, sVector.Length-2);
		}

		// split the items
		string[] sArray = sVector.Split(',');

		// store as a Vector3
		Vector3 result = new Vector3(
			float.Parse(sArray[0]),
			float.Parse(sArray[1]),
			float.Parse(sArray[2]));

		return result;
	}
	#endregion
}
