using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;



public class Client : MonoBehaviour
{
    public string ClientName;
    [Header("GameObject"), Space(3)]
    public GameObject myPrefab;
    public GameObject prefab;

    [Header("UI"), Space(3)]
    public InputField HostInputField;
    public InputField PortInputField;
    public GameObject LoginObject;

    private int clientID;

    private float speed = 2.5f;

    [SerializeField]
    private List<GameObject> clientObjects;
    [SerializeField]
    private Player player;

    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    
    private BinaryWriter bWriter;
    private BinaryReader bReader;


    #region Unity Function
    private void Start()
    {
        clientID = -1;
        clientObjects = new List<GameObject>();
        HostInputField.text = "10.21.20.36";
        PortInputField.text = "6321";
        ConnectedToServer();
    }

    private void Update()
    {
        float  move = speed * Time.deltaTime;

        float ver = Input.GetAxis("Vertical");
        float hor = Input.GetAxis("Horizontal");

        Vector3 moveAmount = Vector3.forward * ver * move + Vector3.right * hor * move;
        if (socketReady)
        {
            //Vector3.magnitude => Vector3 스칼라
            if (moveAmount.magnitude > 0 && clientID >=0)
            {
                bWriter.Write((int)MessageID.MOVE);
                bWriter.Write(clientID);
                bWriter.Write(moveAmount.x);
                bWriter.Write(moveAmount.y);
                bWriter.Write(moveAmount.z);
                if (player != null)
                    player.animator.SetBool("IsMove", true);
            }
            else if(clientID >= 0)
            {
                if (player != null)
                    player.animator.SetBool("IsMove", false);
            }

            if (stream.DataAvailable)
            {
                OnIncomingData();
            }
        }
    }
    #endregion

    #region NetworkFunction
    public void ConnectedToServer()
    {
        //if already connected, ignore this function
        if (socketReady)
            return;

        // Default host / port 
        string host = "127.0.0.1";
        int port = 6321;

        string hostIP;
        int portNumber;

        hostIP = HostInputField.text;
        if (hostIP != "")
            host = hostIP;

        int.TryParse(PortInputField.text, out portNumber);
        if (portNumber != 0)
            port = portNumber;

        //create the socket
        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();

            bWriter = new BinaryWriter(stream);
            bReader = new BinaryReader(stream);
            socketReady = true;
            LoginObject.SetActive(false);
        }
        catch (Exception e)
        {
            Debug.Log("Socket error : " + e.Message);
        }

    }

    public void OnSendButton()
    {
        //string message = GameObject.Find("SendInput").GetComponent<InputField>().text;
        //Send(message);
    }

    private void OnIncomingData()
    {

        //        Debug.Log(data);
        //        Debug.Log(data.Split('|')[0]);
        //        Debug.Log(data.Split('|')[1]);
        //        Debug.Log(data.Split('|')[2]);

        int messageID = bReader.ReadInt32();
        int id;
        float x=-1, y=-1, z=-1;
        byte[] buffer;

        switch(messageID)
        {
            case (int)MessageID.INIT:
                //Debug.Log("Init Client");
                id = bReader.ReadInt32();
                clientID = id;
                x = bReader.ReadSingle();
                y = bReader.ReadSingle();
                z = bReader.ReadSingle();
                buffer = Message.getBytes(MessageID.NEW, id, id*2, 0, 0);
                //Debug.Log($"ID: {id}, X: {x}, Y:{y} z:{z}");
                Send(buffer);
                break;
            
            case (int)MessageID.NEW:
                Debug.Log("New Client");
                id = bReader.ReadInt32();
                x = bReader.ReadSingle();
                y = bReader.ReadSingle();
                z = bReader.ReadSingle();
                //Debug.Log($"ID: {id}, X: {x}, Y:{y} z:{z}");

                if (id == clientID)
                {
                    GameObject clientObject = Instantiate(myPrefab, new Vector3(x, y, z), Quaternion.identity);
                    clientObjects.Add(clientObject);
                    player = clientObject.GetComponentInChildren<Player>();
                }
                else
                {
                    GameObject clientObject = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity);
                    clientObjects.Add(clientObject);
                }
                break;
            
            case (int)MessageID.MOVE:
                Debug.Log("Move");

                id = bReader.ReadInt32();
                x = bReader.ReadSingle();
                y = bReader.ReadSingle();
                z = bReader.ReadSingle();

                //Debug.Log($"ID: {id}, X: {x}, Y:{y} z:{z}");
                clientObjects[id].transform.position = new Vector3(x, y, z);
                break;
            case (int)MessageID.NOTICE:
                Debug.Log("Server Notice");
                break;
            default:
                Debug.Log($"MessageID({messageID}) in switch. Error");
                break;
            
        }
        
    
    }

    private void Send(byte[] data)
    {
        if (!socketReady)
            return;
        bWriter.Write(data, 0, 20);
        Debug.Log($"[Client({clientID})] Sent Data: {data}");
    }

    private void CloseSocket()
    {
        if (!socketReady)
            return;
        bWriter.Close();
        bReader.Close();
        socket.Close();
        socketReady = false;
    }

    #endregion

    private void OnApplicationQuit()
    {
        CloseSocket();
    }

    private void OnDisable()
    {
        CloseSocket();
    }
}
