using UnityEngine;

public class GroundTriger : MonoBehaviour
{
    public bool IsGround = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            IsGround = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            IsGround = false;
        }
    }
}
