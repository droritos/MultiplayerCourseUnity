using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class BombVisualController : MonoBehaviour
{
    public event UnityAction OnCreated;
    public event UnityAction OnExplode;

    [SerializeField] Transform visualTransfromMesh;

    private float _timerToExplode;
    private Sequence _bombSequence;

    private void OnEnable()
    {
        OnCreated?.Invoke();
    }
    [ContextMenu("Test Start Bomb Animation")]
    private void TestStartBombAnimation()      // <-- no parameters, shows in menu
    {
        StartBombAnimation(3f);
    }
    public void StartBombAnimation(float timerToExplode)
    {
        InitializedBomb(timerToExplode);

        // Kill old sequence if any
        _bombSequence?.Kill();

        _bombSequence = DOTween.Sequence();

        // Step 1: Wiggle rotation (small tilt back and forth)
        _bombSequence.Append(
            visualTransfromMesh.DORotate(new Vector3(0, 0, 10f), 0.1f)
                .SetEase(Ease.InOutSine)
                .SetLoops(Mathf.RoundToInt((_timerToExplode - 0.3f) / 0.2f), LoopType.Yoyo)
        );

        // Step 2: Shrink quickly
        _bombSequence.Append(
            visualTransfromMesh.DOScale(Vector3.one * 0.8f, 0.1f).SetEase(Ease.InQuad)
        );

        // Step 3: Big "pop" scale
        _bombSequence.Append(
            visualTransfromMesh.DOScale(Vector3.one * 1.5f, 0.2f).SetEase(Ease.OutBack)
        );

        // Step 4: Call finish
        _bombSequence.OnComplete(FinishAnimation);
    }
    private void InitializedBomb(float timerToExplode)
    {
        _timerToExplode = timerToExplode;
    }

    private void FinishAnimation()
    {
        OnExplode?.Invoke();
        Destroy(gameObject);
    }

}
