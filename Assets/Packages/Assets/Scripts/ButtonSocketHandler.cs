using UnityEngine;
using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Collections.Generic;

public class ButtonSocketHandler : MonoBehaviour
{
    // EVENTS
    public delegate void buttonDown(AButton b);
    public delegate void buttonUp(AButton b);

    public static event buttonDown OnBDown;
    public static event buttonUp OnBUp;

    // ------
    public static ButtonSocketHandler socketHandler;
    public int port = 6320;
    public WebSocketServer server;
    public List<AButton> button;

    public static void TriggerButtonDown(AButton b)
    {
        OnBDown?.Invoke(b);
    }

    public static void TriggerButtonUp(AButton b)
    {
        OnBUp?.Invoke(b);
    }

    public List<AButton> GetActiveButtons()
    {
        return button.FindAll(b => b.Active = true);
    }

    //TODO map buttons to colors

    private void Awake()
    {
        if (socketHandler == null)
        {
            DontDestroyOnLoad(gameObject);
            socketHandler = this;

            server = new WebSocketServer(port);
            server.AddWebSocketService<ButtonSocketBehavior>("/glowbtn");
            server.Start();
            Debug.Log("Web socket server started...");
            button = new List<AButton>();
            for (uint i = 0; i < 256; i++)
            {
                button.Add(new AButton(i));
            }
        }
        else if (socketHandler != this)
        {
            Destroy(gameObject);
        }
    }

    public void BlinkOne(string id, byte i, Color32 color)
    {
        if (id.IsNullOrEmpty()) return;
        bool isHost = server.WebSocketServices.TryGetServiceHost("/glowbtn", out WebSocketServiceHost host);
        if (!isHost) return;
        byte[] outas = new byte[5];
        outas[0] = 2;
        outas[1] = i;
        outas[2] = color.r;
        outas[3] = color.g;
        outas[4] = color.b;
        host.Sessions.SendToAsync(outas, id, null);
    }

    public void BlinkAll(string id, byte i, Color32 highlight, Color32 color1, Color32 color2)
    {
        if (id.IsNullOrEmpty()) return;
        bool isHost = server.WebSocketServices.TryGetServiceHost("/glowbtn", out WebSocketServiceHost host);
        if (!isHost) return;
        byte[] outas = new byte[11];
        outas[0] = 2;
        outas[1] = i;
        outas[2] = color1.r;
        outas[3] = color1.g;
        outas[4] = color1.b;
        outas[5] = highlight.r;
        outas[6] = highlight.g;
        outas[7] = highlight.b;
        outas[8] = color2.r;
        outas[9] = color2.g;
        outas[10] = color2.b;
        host.Sessions.SendToAsync(outas, id, null);
    }

    void Update()
    {
        //WebSocketServiceHost host;
        //IWebSocketSession mgr;
        //bool isHost = wssv.WebSocketServices.TryGetServiceHost("/glowbtn", out host);
        //host.Sessions.SendTo()

        /*
        Color32 mcol = new Color32();
        Vector3 tpos = new Vector3();

        for (byte i = 0; i < 4; i++)
        {
            mats1[i].GetComponent<Renderer>().material.SetColor("_Color", Color.Lerp(mats1[i].GetComponent<Renderer>().material.GetColor("_Color"), Color.black, 0.2f));
            mats2[i].GetComponent<Renderer>().material.SetColor("_Color", Color.Lerp(mats2[i].GetComponent<Renderer>().material.GetColor("_Color"), Color.black, 0.2f));
            if (p1_btns[i])
            {
                p1_btns[i] = false;
                tpos = mats1[i].transform.position;
                mats1[i].transform.position = new Vector3(tpos.x, -0.2f, tpos.z);

                mcol = UnityEngine.Random.ColorHSV();

                if (tDrop.value == 0)
                {
                    mats1[i].GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                    BlinkOne(p1_id, i, Color.white);
                }
                else if (tDrop.value == 1)
                {
                    mats1[i].GetComponent<Renderer>().material.SetColor("_Color", mcol);
                    BlinkOne(p1_id, i, mcol);
                }
                else
                {
                    mats1[i].GetComponent<Renderer>().material.SetColor("_Color", mcol);
                    BlinkAll(p1_id, i, mcol, UnityEngine.Random.ColorHSV(0f, 1f, 0.8f, 1f), UnityEngine.Random.ColorHSV(0f, 1f, 0.8f, 1f));
                }
            }
            if (p2_btns[i])
            {
                p2_btns[i] = false;
                tpos = mats2[i].transform.position;
                mats2[i].transform.position = new Vector3(tpos.x, -0.2f, tpos.z);

                mcol = UnityEngine.Random.ColorHSV();

                if (tDrop.value == 0)
                {
                    mats2[i].GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                    BlinkOne(p2_id, i, Color.white);
                }
                else if (tDrop.value == 1)
                {
                    mats2[i].GetComponent<Renderer>().material.SetColor("_Color", mcol);
                    BlinkOne(p2_id, i, mcol);
                }
                else
                {
                    mats2[i].GetComponent<Renderer>().material.SetColor("_Color", mcol);
                    BlinkAll(p2_id, i, mcol, UnityEngine.Random.ColorHSV(), UnityEngine.Random.ColorHSV());
                }
            }
            if (p1_btnsr[i])
            {
                p1_btnsr[i] = false;
                tpos = mats1[i].transform.position;
                mats1[i].transform.position = new Vector3(tpos.x, 0f, tpos.z);
            }
            if (p2_btnsr[i])
            {
                p2_btnsr[i] = false;
                tpos = mats2[i].transform.position;
                mats2[i].transform.position = new Vector3(tpos.x, 0f, tpos.z);
            }
            
        }*/
    }

    void OnApplicationQuit()
    {
        server.Stop();
        Debug.Log("Stopping web socket!");
    }

    public bool getBit(byte b, byte bit)
    {
        return (b & (1 << bit)) != 0;
    }

    public void SendAll(byte[] outas, Action<bool> callback = null)
    {
        socketHandler.button.ForEach(b =>
        {
            if (!b.id.IsNullOrEmpty() && b.Active)
            {
                b.Send(outas, callback);
            };
        });
    }

    public void SetAll(byte program, Color32? color1 = null, Color32? color2 = null, Color32? color3 = null)
    {
        socketHandler.GetActive().ForEach(b =>
        {
            b.Set(program, color1, color2, color3);
        });
    }

    public List<AButton> GetActive()
    {
        List<AButton> nl = new List<AButton>();
        socketHandler.button.ForEach(b =>
        {
            if (!b.id.IsNullOrEmpty() && b.Active)
            {
                nl.Add(b);
            };
        });
        return nl;
    }

    public void SendIndex(uint index, byte[] outas, Action<bool> callback = null)
    {
        socketHandler.button.ForEach(b =>
        {
            if (!b.id.IsNullOrEmpty() && b.Active && b.index == index)
            {
                b.Send(outas, callback);
            };
        });
    }

    public void SetIndex(uint index, byte program, Color32? color1 = null, Color32? color2 = null, Color32? color3 = null)
    {
        Color32 c1 = color1 ?? Color.black;
        Color32 c2 = color2 ?? Color.black;
        Color32 c3 = color3 ?? Color.black;

        byte[] outas = new byte[11];
        outas[0] = 1;
        outas[1] = program;
        outas[2] = c1.r;
        outas[3] = c1.g;
        outas[4] = c1.b;
        outas[5] = c2.r;
        outas[6] = c2.g;
        outas[7] = c2.b;
        outas[8] = c3.r;
        outas[9] = c3.g;
        outas[10] = c3.b;
        socketHandler.SendIndex(index, outas);
    }
}