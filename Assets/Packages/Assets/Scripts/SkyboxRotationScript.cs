using UnityEngine;

public class SkyboxRotationScript : MonoBehaviour
{
    public float speed = 1f;
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time*speed);
    }
}
