using System;
using UnityEngine;

public class RectTransformPositionTween
{
    private RectTransform _rectTransform;
    private bool _isTweening;
    private float _startTime;
    private float _timeToEnd;
    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private Transform _originalParent;
    private Transform _tweenParent;
    private Action _onComplete;

    public bool IsTweening => _isTweening;

    public void StartTween(RectTransform rectTransform, Vector3 targetPosition, float duration, 
        Transform tweenParent = null, System.Action onComplete = null)
    {
        _rectTransform = rectTransform;
        _targetPosition = targetPosition;
        _startPosition = _rectTransform.position;
        _onComplete = onComplete;

        _startTime = Time.time;
        _timeToEnd = Time.time + duration;

        if (tweenParent != null)
        {
            _originalParent = _rectTransform.parent;
            _tweenParent = tweenParent;
            _rectTransform.SetParent(_tweenParent, worldPositionStays: true);
        }

        _isTweening = true;
    }

    public void UpdateTween()
    {
        if (!_isTweening || _rectTransform == null)
        {
            _isTweening = false;
            return;
        }

        float currentTime = Time.time;
        if (currentTime >= _timeToEnd)
        {
            FinishTween();
            return;
        }

        float duration = _timeToEnd - _startTime;
        float elapsed = currentTime - _startTime;
        float progress = elapsed / duration;

        // Quadratic ease-in-out
        float easedProgress;
        if (progress < 0.5f)
        {
            easedProgress = 2 * progress * progress;
        }
        else
        {
            progress = progress - 1;
            easedProgress = 1 - (2 * progress * progress);
        }

        Vector3 newPosition = Vector3.Lerp(_startPosition, _targetPosition, easedProgress);
        _rectTransform.position = newPosition;
    }

    private void FinishTween()
    {
        if (_rectTransform != null)
        {
            _rectTransform.position = _targetPosition;
            
            if (_originalParent != null)
            {
                _rectTransform.SetParent(_originalParent, worldPositionStays: true);
                _originalParent = null;
                _tweenParent = null;
            }
        }

        _isTweening = false;
        _onComplete?.Invoke();
    }
}