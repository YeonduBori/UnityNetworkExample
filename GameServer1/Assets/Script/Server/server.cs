using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.IO;
using UnityEngine.UI;
using System.Threading;

public class server : MonoBehaviour
{
    
    public GameObject clientPrefab;
	public int port = 6321;
    private List<GameObject> clientObjects;

    private bool newOneCame = false;

    private List<ServerClient> clients;
    private List<ServerClient> disconnectList;

    
    private TcpListener Server;
    private bool serverStarted;
    private bool isGameEnd = false;
  
    private void Start()
    {
        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();
        clientObjects = new List<GameObject>();
        
        try
        {
            Server = new TcpListener(IPAddress.Any, port);
            Server.Start();

            startListening();
            serverStarted = true;
            Debug.Log("Server has been started on port" + port.ToString());
        }
        catch(Exception e)
        {
            Debug.Log("Soket error: " + e.Message);
        }
    }

    private void Update()
    {
        if (!serverStarted)
            return;

        if (newOneCame)
        {
            SendDataToAllClient();
            newOneCame = false;
        }

        foreach (ServerClient c in clients)
        {
            // Is the client still connected?
            if (!IsConnected(c.tcp))
            {
                c.tcp.Close();
                disconnectList.Add(c);
                continue;
            }
            // check for message from the client
            else
            {
                NetworkStream s = c.tcp.GetStream();
                if (s.DataAvailable)
                {
                    BinaryReader reader = new BinaryReader(s);
                    
                    if(reader != null)
                    {
                        OnIncomingData(c, reader);
                    }

                }
            }
        }

        for(int index = 0; index < clientObjects.Count; index++)
        {
            if(clientObjects[index].GetComponent<PlayerObjecter>().IsEnding && !isGameEnd)
            {
                isGameEnd = true;
                byte[] data = Message.getBytes(MessageID.NOTICE, index, $"Client{index} Win!");
                Broadcast(data, clients);
            }
        }

        for(int i = 0; i < disconnectList.Count-1; i++)
        {
            //Broadcast(disconnectList[i].clientName + " has disconnected", clients);

            clients.Remove(disconnectList[i]);
            disconnectList.RemoveAt(i);
        }
    }

    private void startListening()
    {
        Server.BeginAcceptTcpClient(AcceptTcpClient, Server);
    }

    private bool IsConnected(TcpClient c)
    {
        try
        {
            if(c !=null && c.Client !=null & c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }
                return true;

            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }
     
    
    private void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        newOneCame = true;
        clients.Add(new ServerClient(listener.EndAcceptTcpClient(ar)));
        startListening();
    }

    private void SendDataToAllClient ()
    {
        byte[] buffer;
        float x, y, z;

        for (int i = 0; i < clientObjects.Count; i++)
        {
            Debug.Log(" loop in AcceptTcpClient");
            Debug.Log("i:" + i);
            Debug.Log("clientObjects[i].GetComponent<Transform>().position.x:" + clientObjects[i].transform.position.x);

            x = clientObjects[i].GetComponent<Transform>().position.x;
            y = clientObjects[i].GetComponent<Transform>().position.y;
            z = clientObjects[i].GetComponent<Transform>().position.z;

            buffer = Message.getBytes(MessageID.NEW, i, x, y, z);
            Debug.Log(" loop in AcceptTcpClient After getBytes");
            Broadcast(buffer, new List<ServerClient>() { clients[clients.Count - 1] });
            Debug.Log(" loop in AcceptTcpClient After Broadcast");
        }

        Debug.Log("after loop in AcceptTcpClient");
        //send a message to everyone, say someone has connected
        buffer = Message.getBytes(MessageID.INIT, clients.Count-1, 0, 0, 0);
        Broadcast(buffer,new List<ServerClient>() { clients[clients.Count-1]});
        Debug.Log("end of loop in AcceptTcpClient");

    }

    private void OnIncomingData(ServerClient c, BinaryReader reader)
    {
        int messageID;
        int id;
        float x, y, z;
        byte[] buffer;
        Debug.Log("begin in OnIncomingData");

        messageID = reader.ReadInt32();
        switch (messageID)
        {
            case (int)MessageID.NEW:
                Debug.Log("new");
                id = reader.ReadInt32();
                x = reader.ReadSingle();
                y = reader.ReadSingle();
                z = reader.ReadSingle();
                Debug.Log("id:" + id + " x:" + x + "y:" + y + "z: " + z);
                clientObjects.Add(Instantiate(clientPrefab, new Vector3(x, y, z), Quaternion.identity));
                //Debug.Log("Object count:" + objNum + "client count:" + clients.Count);
                buffer = Message.getBytes(MessageID.NEW, id, x, y, z);
                Broadcast(buffer, clients);
                break;
            
            case (int)MessageID.MOVE:
                Debug.Log("move");

                id = reader.ReadInt32();
                x = reader.ReadSingle();
                y = reader.ReadSingle();
                z = reader.ReadSingle();
                Debug.Log("id:" + id + " x:" + x + "y:" + y + "z: " + z);
                clientObjects[id].GetComponent<Transform>().Translate(x, y, z);
                x = clientObjects[id].GetComponent<Transform>().position.x;
                y = clientObjects[id].GetComponent<Transform>().position.y;
                z = clientObjects[id].GetComponent<Transform>().position.z;
                buffer = Message.getBytes(MessageID.MOVE, id, x, y, z);
                Broadcast(buffer, clients);
                break;
            default:
                Debug.Log("switch default error!!");
                break;
        }
        
    }

    private void Broadcast(byte[] data,List<ServerClient> c1)
    {
        foreach(ServerClient c in c1)
        {
            try
            {
                BinaryWriter writer = new BinaryWriter(c.tcp.GetStream());
                writer.Write(data, 0, 20);
            }
            catch (Exception e)
            {
                Debug.Log("Write Error : " + e.Message + "to client" + c.clientName);
            }
        }
    }
}
public class ServerClient
{
    public TcpClient tcp;
    public string clientName;

    public ServerClient(TcpClient clientSocket)
    {
        clientName = "Guest";
        tcp = clientSocket;
    }
}
