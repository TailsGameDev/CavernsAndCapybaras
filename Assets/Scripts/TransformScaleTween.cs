using System;
using UnityEngine;

public class TransformScaleTween
{
    private Transform _transform;
    private bool _isTweening;
    private float _startTime;
    private float _timeToEnd;
    private Vector3 _startScale;
    private Vector3 _targetScale;
    private System.Action _onComplete;

    public bool IsTweening => _isTweening;

    public void StartTween(Transform transform, Vector3 targetScale, float duration, 
        System.Action onComplete = null)
    {
        _transform = transform;
        _targetScale = targetScale;
        _startScale = _transform.localScale;
        _onComplete = onComplete;

        _startTime = Time.time;
        _timeToEnd = Time.time + duration;

        _isTweening = true;
    }

    public void Update()
    {
        if (!_isTweening || _transform == null)
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

        Vector3 newScale = Vector3.Lerp(_startScale, _targetScale, easedProgress);
        _transform.localScale = newScale;
    }

    private void FinishTween()
    {
        if (_transform != null)
        {
            _transform.localScale = _targetScale;
        }

        _isTweening = false;
        _onComplete?.Invoke();
    }
}