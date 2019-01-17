using Core.Utilities;
using UnityEngine;

public class TouchManager : PersistentSingleton<TouchManager>
{
    public delegate void OnTap(Vector2 pos);
    public delegate void OnZoom(Vector2 posA, Vector2 posB);

    public static event OnTap TapEvent, HoldEvent;
    public static event OnZoom ZoomEvent;

    private enum HandleType {Tap, Hold, DoubleTap, Zoom }

    [SerializeField]
    private float doubleTapThreshold = .5f;

    private bool Simulating => !Application.isMobilePlatform;
    private bool InputDetected => IsTouching(0);
    private HandleType Handle
    {
        get
        {
            bool touch0 = IsTouching(0),
                touch1 = IsTouching(1);

            // Double tap
            if (touch0 && !touchedLastFrame[0] && lastTouchDetected >= Time.time + doubleTapThreshold)
                return HandleType.DoubleTap;

            // Zoom
            if (touch0 && touchedLastFrame[0] && touch1 && touchedLastFrame[1])
                return HandleType.Zoom;

            // Hold
            if (touch0 && touchedLastFrame[0])
                return HandleType.Hold;

            // Tap
            return HandleType.Tap;
        }
    }

    private bool[] touchedLastFrame = new bool[2];
    private float lastTouchDetected = float.NegativeInfinity;

    private void Update()
    {
        if (InputDetected)
            switch (Handle)
            {
                case HandleType.Tap:
                    Tap();
                    break;
                case HandleType.DoubleTap:
                    DoubleTap();
                    break;
                case HandleType.Zoom:
                    Zoom();
                    break;
            }

        touchedLastFrame[0] = IsTouching(0);
        touchedLastFrame[1] = IsTouching(1);
    }

    private bool IsTouching(int inputIndex)
    {
        return Simulating ? Input.GetMouseButton(inputIndex) : Input.touchCount > inputIndex;
    }

    private Vector2 GetTouchPosition(int inputIndex) => Simulating ? (Vector2)Input.mousePosition : Input.GetTouch(inputIndex).position;

    #region Handle
    private void Tap()
    {
        lastTouchDetected = Time.time;
        TapEvent?.Invoke(GetTouchPosition(0));
    }

    private void DoubleTap() => HoldEvent?.Invoke(GetTouchPosition(0));
    private void Zoom() => ZoomEvent?.Invoke(GetTouchPosition(0), GetTouchPosition(1));
    #endregion
}