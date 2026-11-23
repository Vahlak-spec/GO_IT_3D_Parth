using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    void Start()
    {
        GetComponent<MeshRenderer>().material.color = Color.white;
    }
}
