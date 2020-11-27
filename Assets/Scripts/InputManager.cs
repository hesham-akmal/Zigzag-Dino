using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region SingletonAndAwake

    private static InputManager _instance;
    public static InputManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion SingletonAndAwake

    private bool tap = false;
    public bool Tap { get => tap; }

    public void Reset()
    {
        tap = false;
    }

    private void Update()
    {
        tap = false;

        #region Standalone Inputs

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            tap = true;

        #endregion Standalone Inputs

        #region Mobile inputs

        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
            tap = true;

        #endregion Mobile inputs
    }
}