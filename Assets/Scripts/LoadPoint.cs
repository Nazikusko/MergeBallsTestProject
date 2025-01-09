using System;
using System.Collections;
using UnityEngine;

public class LoadPoint : MonoBehaviour
{
    private const float SCAN_RADIUS = 2.15f;
    private const float GAME_OVER_TIMEOUT = 3f;
    private const float CHECK_LOAD_ZONE_TIME_INTERVAL = 0.1f;

    private bool _isLoadZoneClear;
    private bool _isBallLoaded;
    private Coroutine _gameOverRoutine;

    public event Action OnLoadZoneClear;
    public event Action OnLoadZoneOccupied;
    public event Action OnGameOver;

    public bool IsLoadZoneClear => _isLoadZoneClear;
    public bool IsBallLoaded => _isBallLoaded;
    private Coroutine _scanZoneRoutine = null;

    public void StartScanLoadPoint()
    {
        _scanZoneRoutine = StartCoroutine(CheckLoadZoneStatus());
    }

    public void StopScanLoadPoint()
    {
        if (_scanZoneRoutine != null) StopCoroutine(_scanZoneRoutine);
        _isBallLoaded = false;
        _isLoadZoneClear = false;
    }

    private IEnumerator CheckLoadZoneStatus()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(CHECK_LOAD_ZONE_TIME_INTERVAL);
            Collider2D[] ballsInLoadZone = Physics2D.OverlapCircleAll(transform.position, SCAN_RADIUS, LayerMask.GetMask("Balls"));

            if (ballsInLoadZone.Length > 0 && _isLoadZoneClear)
            {
                OnLoadZoneOccupied?.Invoke();
            }
            if (ballsInLoadZone.Length == 0 && !_isLoadZoneClear)
            {
                OnLoadZoneClear?.Invoke();
                if (_gameOverRoutine != null)
                {
                    StopCoroutine(_gameOverRoutine);
                    _gameOverRoutine = null;
                }
            }

            _isLoadZoneClear = ballsInLoadZone.Length == 0;
        }
    }

    private IEnumerator WaiteGameOverTimeout()
    {
        yield return new WaitForSecondsRealtime(GAME_OVER_TIMEOUT);
        OnGameOver?.Invoke();
    }

    public void BallLoaded()
    {
        _isBallLoaded = true;
        _isLoadZoneClear = true; // this is for guaranteed call OnLoadZoneOccupied event
    }

    public void BallThrow()
    {
        _isBallLoaded = false;
        _isLoadZoneClear = false;// this is for guaranteed call OnLoadZoneClear event

        if (_gameOverRoutine == null) _gameOverRoutine = StartCoroutine(WaiteGameOverTimeout());
    }
}
