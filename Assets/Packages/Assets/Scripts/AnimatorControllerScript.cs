using UnityEngine;
public class AnimatorControllerScript : MonoBehaviour
{
    private Animator anim;

    public float rotateSpeed = 1f;
    public float moveSpeed = 1f;
    public bool isMoving = false;

    public bool testUpdateValues = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        UpdateValues();
    }

    private void Update()
    {
        if (testUpdateValues)
        {
            UpdateValues();
            testUpdateValues = false;
        }
    }


    private void UpdateValues()
    {
        anim.SetFloat("RotateSpeed", rotateSpeed);
        anim.SetFloat("MoveSpeed", moveSpeed);
        anim.SetBool("IsMoving", isMoving);
    }
}
