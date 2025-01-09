using System;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public event Action<Vector2> OnTouchOnScreenStart;
    public event Action<Vector2> OnTouchOnScreenMoved;
    public event Action<Vector2> OnTouchOnScreenEnd;
    public event Action<Vector2> OnTapOnScreen;

    public TouchPhase TouchPhase => _touchPhase;

    private bool _isSwipe;
    private bool _isGameInputActive;
    private TouchPhase _touchPhase = TouchPhase.Canceled;
    private Vector2 _StartTouchLastposition;

    public bool IsGameInputActive
    {
        get => _isGameInputActive;
        set
        {
            if (!value)
            {
                _touchPhase = TouchPhase.Canceled;
                _isSwipe = false;
            }

            _isGameInputActive = value;
        }
    }

    void Update()
    {
        if (!IsGameInputActive)
        {
            return;
        }

        TouchOrMouseInputUpdate();
    }

    private void TouchOrMouseInputUpdate()
    {
        var isPressed = IsPressed(out var position);

        if (isPressed)
        {
            if (_touchPhase == TouchPhase.Began || _touchPhase == TouchPhase.Moved)
            {
                if (_touchPhase == TouchPhase.Began && _StartTouchLastposition == position)
                {
                    return;
                }
                _touchPhase = TouchPhase.Moved;
                _isSwipe = true;
                OnTouchOnScreenMoved?.Invoke(position);
            }
            else
            {
                _touchPhase = TouchPhase.Began;
                _isSwipe = false;
                _StartTouchLastposition = position;
                OnTouchOnScreenStart?.Invoke(position);
            }
        }
        else
        {
            if (_touchPhase == TouchPhase.Ended || _touchPhase == TouchPhase.Canceled)
            {
                _touchPhase = TouchPhase.Canceled;
            }
            else
            {
                if (_isSwipe)
                {
                    OnTouchOnScreenEnd?.Invoke(position);
                }
                else
                {
                    OnTapOnScreen?.Invoke(position);
                }

                _isSwipe = false;

                _touchPhase = TouchPhase.Ended;
            }
        }

    }

    private bool IsPressed(out Vector2 position)
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            position = touch.position;
            return true;
        }

        if (Input.GetMouseButton(0))
        {
            position = Input.mousePosition;
            return true;
        }

        position = Input.mousePosition;
        return false;
    }
}
