﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Message : MonoBehaviour
{
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
    
    public static byte[] getBytes(MessageID messageID, int clientID, string message)
    {
        byte[] btBuffer = new byte[4096];
        MemoryStream ms = new MemoryStream(btBuffer, true);
        BinaryWriter bw = new BinaryWriter(ms);
        bw.Write((int)messageID);
        bw.Write(clientID);
        bw.Write(message);
        bw.Close();
        ms.Close();
        return btBuffer;
    }
}
