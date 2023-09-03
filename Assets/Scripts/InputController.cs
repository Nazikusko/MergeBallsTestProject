using System;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public event Action<Vector2> OnTouchOnScreenStart;
    public event Action<Vector2> OnTouchOnScreenMoved;
    public event Action<Vector2> OnTouchOnScreenEnd;
    public event Action<Vector2> OnTapOnScreen;

    private bool _isSwipe;
    private bool _isGameInputActive;

    public bool IsGameInputActive
    {
        get => _isGameInputActive;
        set
        {
            _isSwipe = false;
            _isGameInputActive = value;
        }
    }

    void Update()
    {
        if (!IsGameInputActive)
            return;
        TapOnScreenUpdate();
    }

    private void TapOnScreenUpdate()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            _isSwipe = false;
            OnTouchOnScreenStart?.Invoke(touch.position);
        }

        if (touch.phase == TouchPhase.Moved)
        {
            _isSwipe = true;
            OnTouchOnScreenMoved?.Invoke(touch.position);
        }

        if (touch.phase == TouchPhase.Ended)
        {
            if (_isSwipe)
                OnTouchOnScreenEnd?.Invoke(touch.position);
            else
                OnTapOnScreen?.Invoke(touch.position);
        }
    }
}
