using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
public enum MessageID
{
    INIT=10,
    NEW,
    MOVE,
}

public class Message : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static byte[] getBytes(MessageID messageID, int clientID, float x, float y, float z)
    {
        byte[] btBuffer = new byte[4096];
        MemoryStream ms = new MemoryStream(btBuffer, true);
        BinaryWriter bw = new BinaryWriter(ms);
        bw.Write((int)messageID);
        bw.Write(clientID);
        bw.Write(x);
        bw.Write(y);
        bw.Write(z);
        bw.Close();
        ms.Close();
        return btBuffer;
    }

        public int add(int a, int b)
    {
        return a + b;
    }
    
}
