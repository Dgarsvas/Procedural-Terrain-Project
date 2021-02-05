using System;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

public class AButton
{
    public bool Active = false;
    public string id = null;
    public uint index = 0;
    public enum FX : byte
    {
        Static,
        Blink,
        Breath,
        ColorWipe,
        ColorWipeInv,
        ColorWipeRev,
        ColorWipeRevInv,
        ColorWipeRandom,
        RandomColor,
        SingleDynamic,
        MultiDynamic,
        Rainbow,
        RainbowCycle,
        Scan,
        DualScan,
        Fade,
        TheaterChase,
        TheaterChaseRainbow,
        RunningLights,
        Twinkle,
        TwinkleRandom,
        TwinkleFade,
        TwinkleFadeRandom,
        Sparkle,
        FlashSparkle,
        HyperSparkle,
        Strobe,
        StrobeRainbow,
        MultiStrobe,
        BlinkRainbow,
        ChaseWhite,
        ChaseColor,
        ChaseRandom,
        ChaseRainbow,
        ChaseFlash,
        ChaseFlashRandom,
        ChaseRainbowWhite,
        ChaseBlackout,
        ChaseBlackoutRainbow,
        ColorSweepRandom,
        RunningColor,
        RunningRedBlue,
        RunningRandom,
        LarsonScanner,
        Comet,
        Fireworks,
        FireworksRandom,
        MerryChristmas,
        FireFlicker,
        FireFlickerSoft,
        FireFlickerIntense,
        CircusCombustus,
        Halloween,
        BicolorChase,
        TricolorChase,
        Icu
    }

    public Color32 color1 = Color.black;
    public Color32 color2 = Color.black;
    public Color32 color3 = Color.black;

    public AButton(uint idx = 0)
    {
        this.Active = false;
        this.id = null;
        this.index = idx;
    }

    public bool Send(byte[] data, Action<bool> callback = null)
    {
        if (this.id.IsNullOrEmpty() || this.Active == false) return false;
        bool isHost = ButtonSocketHandler.socketHandler.server.WebSocketServices.TryGetServiceHost("/glowbtn", out WebSocketServiceHost host);
        if (!isHost) return false;
        host.Sessions.SendToAsync(data, this.id, callback);
        return true;
    }

    public void Set(byte program, Color32? color1 = null, Color32? color2 = null, Color32? color3 = null)
    {
        this.color1 = color1 ?? this.color1;
        this.color2 = color2 ?? this.color2;
        this.color3 = color3 ?? this.color3;

        byte[] data = new byte[11];
        data[0] = 1;
        data[1] = program;
        data[2] = this.color1.r;
        data[3] = this.color1.g;
        data[4] = this.color1.b;
        data[5] = this.color2.r;
        data[6] = this.color2.g;
        data[7] = this.color2.b;
        data[8] = this.color3.r;
        data[9] = this.color3.g;
        data[10] = this.color3.b;
        this.Send(data);
    }

    private void Sent(bool b)
    {
        if (b)
        {
            Debug.Log($"{this.index} message was sent");
        }
        else
        {
            Debug.LogWarning($"{this.index} message NOT sent.");
        }
    }
}