using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TerrainData : UpdateableData
{
    public bool useFalloff;

    public float heightMultiplier;

    public AnimationCurve meshHeightCurve;

    public bool useFlatShading;

    public float uniformScale = 1f;
}
