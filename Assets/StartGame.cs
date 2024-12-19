using System;
using UnityEngine;
using UnityEngine.Events;

public class StartGame : MonoBehaviour
{
    public UnityEvent OnStartGame;

    private void OnTriggerEnter(Collider other)
    {
        OnStartGame.Invoke();
    }
}
