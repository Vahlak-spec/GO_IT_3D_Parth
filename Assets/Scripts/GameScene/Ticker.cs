using System;
using UnityEngine;


public class Ticker : MonoBehaviour
{
    public Action onUpdate;
    public Action onFixedUpdate;

    private void Update()
    {
        onUpdate?.Invoke();
    }
    private void FixedUpdate()
    {
        onFixedUpdate?.Invoke();
    }
}

