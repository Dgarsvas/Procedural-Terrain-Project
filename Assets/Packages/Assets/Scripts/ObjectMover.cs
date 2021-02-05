using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectMover : MonoBehaviour
{
    public void SetObjectColor(Color newColor)
    {
        GetComponent<MeshRenderer>().material.SetColor("_Color", newColor);
    }

    public void Launch(float moveSpeed, float spinSpeed)
    {
        GetComponent<Rigidbody>().AddForce(-Vector3.forward * moveSpeed, ForceMode.Impulse);
        SetRandomRotation(spinSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Kill"))
        {
            Destroy(gameObject);
        }
    }

    private void SetRandomRotation(float spinSpeed)
    {
        Vector3 angularVector = new Vector3(Random.Range(-spinSpeed, spinSpeed), Random.Range(-spinSpeed, spinSpeed), Random.Range(-spinSpeed, spinSpeed));

        GetComponent<Rigidbody>().angularVelocity = angularVector;
    }
}
