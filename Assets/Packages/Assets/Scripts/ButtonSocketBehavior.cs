using UnityEngine;
using System;
using WebSocketSharp;
using WebSocketSharp.Server;

public class ButtonSocketBehavior : WebSocketBehavior
{
    public bool getBit(byte b, byte bit)
    {
        return (b & (1 << bit)) != 0;
    }
    protected override void OnMessage(MessageEventArgs e)
    {
        base.OnMessage(e);

        //byte[] outas = {};

        if (e.IsText)
        {
            Debug.Log("<-- TEXT: " + e.Data);
        }
        else if (e.IsBinary)
        {
            //if (e.RawData.Length >= 3 && e.RawData[0] == 2)
            //{
            //    if (e.RawData[1] == 1) TINK.LAS.c1Press(e.RawData[2], ID);
            //    if (e.RawData[1] == 2) TINK.LAS.c2Press(e.RawData[2], ID);
            //    Debug.Log("<-- Button Down");
            //}
            //else if (e.RawData.Length >= 3 && e.RawData[0] == 3)
            //{
            //    if (e.RawData[1] == 1) TINK.LAS.c1Rls(e.RawData[2], ID);
            //    if (e.RawData[1] == 2) TINK.LAS.c2Rls(e.RawData[2], ID);
            //    Debug.Log("<-- Button Up");
            //}
            if (e.RawData.Length > 0)
            {
                switch (e.RawData[0])
                {
                    case 2:
                        {
                            int ti = getIndex(ID);
                            if (ti <= 255)
                            {
                                ButtonSocketHandler.TriggerButtonDown(ButtonSocketHandler.socketHandler.button[ti]);
                                Debug.Log($"Button {ti} down with ID: {ID}");
                            }
                        }
                        break;
                    case 3:
                        {
                            int ti = getIndex(ID);
                            if (ti <= 255)
                            {
                                ButtonSocketHandler.TriggerButtonUp(ButtonSocketHandler.socketHandler.button[ti]);
                                Debug.Log($"Button {ti} up with ID: {ID}");
                            }
                        }
                        break;
                    case 254:
                        {
                            if (e.RawData.Length == 7)
                            {
                                if (e.RawData[1] == 2 && e.RawData[2] == 3 && e.RawData[3] == 4 && e.RawData[4] == 5)
                                {
                                    if (e.RawData[5] == 1)
                                    {
                                        ButtonSocketHandler.socketHandler.button[e.RawData[6]].id = ID;
                                        ButtonSocketHandler.socketHandler.button[e.RawData[6]].Active = true;
                                        Debug.Log($"Added button {e.RawData[6]} with ID: {ID}");
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }
    }

    private int getIndex(string id)
    {
        int ti = 500;
        ButtonSocketHandler.socketHandler.button.ForEach(b =>
        {
            if (b.id == id && b.Active) ti = (int)b.index;
        });

        return ti;
    }

    protected override void OnOpen()
    {
        Debug.Log("New connection from " + ID);
        base.OnOpen();
        //Send("HELLO|");

        byte[] data = new byte[5];
        data[0] = 254;
        data[1] = 2;
        data[2] = 3;
        data[3] = 4;
        data[4] = 5;
        SendAsync(data, null);
        //host.Sessions.SendToAsync(outas, id, null);
    }

    protected override void OnClose(CloseEventArgs e)
    {
        Debug.Log(ID + " closed connection, because " + e.Reason + ". Code: " + e.Code);
        Debug.Log(e.ToString());
        //if (TINK.LAS.p1_id == ID) TINK.LAS.p1_id = null;
        //if (TINK.LAS.p2_id == ID) TINK.LAS.p2_id = null;
        ButtonSocketHandler.socketHandler.button.ForEach(b =>
        {
            if (b.id == ID)
            {
                b.Active = false;
                b.id = null;
            }
        });
        base.OnClose(e);
    }

    protected override void OnError(ErrorEventArgs e)
    {
        Debug.Log(ID + "produced error: " + e.Message);
        base.OnError(e);
    }
}