using System;
using DG.Tweening;
using UnityEngine;

public abstract class VfxController : MonoBehaviour
{
    public event Action OnCreated;
    public event Action OnCompleted;

    [SerializeField] protected GameObject parent;

    [Header("Lifecycle")]
    [SerializeField] private bool destroyOnComplete = true;
    [SerializeField] private bool playDuringPause = false;

    protected Sequence sequence;

    protected virtual void OnEnable() => OnCreated?.Invoke();

    #region << Safty Lifecycle >>
    protected virtual void OnDisable() => KillSequence();
    protected virtual void OnDestroy() => KillSequence();
    #endregion

    public void Play(float duration)
    {
        KillSequence();

        sequence = BuildSequence(duration) ?? DOTween.Sequence();
        if (playDuringPause) sequence.SetUpdate(true);

        sequence
            .SetAutoKill(true)
            .SetLink(gameObject)
            .OnComplete(() =>
            {
                OnComplete();
            })
            .Play();
    }

    public void Stop()
    {
        KillSequence();
        OnCompleted?.Invoke();
        if (destroyOnComplete) Destroy(parent);
    }
    protected abstract Sequence BuildSequence(float duration);
    protected virtual void OnComplete()
    {
        this.OnCompleted?.Invoke();
        if (destroyOnComplete) Destroy(parent);
    }

    private void KillSequence()
    {
        if (sequence != null && sequence.IsActive())
        {
            sequence.Kill();
            sequence = null;
        }
    }

    private void OnValidate()
    {
        if (!parent)
            parent = transform.root != null ? transform.root.gameObject : null;
    }
}
