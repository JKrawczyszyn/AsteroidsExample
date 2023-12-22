using System;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public event Action Died;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Died?.Invoke();
    }
}
