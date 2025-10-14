using DG.Tweening;
using ColorBlockCrush;
using System.Collections;
using UnityEngine;

public class ShuffleBooster : BoosterBase
{
    [SerializeField] Hammer hammerPrefab;

    protected override int CurrentCount { get => UserDataManager.ShuffleBooster; set => UserDataManager.ShuffleBooster = value; }

    public override void Init()
    {
        base.Init();
        CurrentCount = UserDataManager.ShuffleBooster;
    }

    private void Update()
    {
        if (IsShowConfirm && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Transform tile = hitInfo.collider.GetComponent<Transform>();
                if (tile != null)
                {
                    StartCoroutine(DoBooster(tile));
                }
            }
        }
    }

    public override void CancelBooster()
    {
        base.CancelBooster();
    }

    public override void ActiveBooster()
    {
        base.ActiveBooster();
        UpdateVisualBooster();
        IsShowConfirm = false;
        OnStartUseBooster?.Invoke(this, CurrentCount);
    }

    protected override void ShowBooster()
    {
        base.ShowBooster();
        IsShowConfirm = true;
    }

    protected override void Done()
    {
        base.Done();

    }

    private IEnumerator DoBooster(Transform tile)
    {
        ActiveBooster();

        yield return new WaitForEndOfFrame();

        Hammer hammer = Instantiate(hammerPrefab);
        hammer.SmashToBlock(tile.transform.position, 0.35f, () =>
        {
            RemoveHammer(hammer);
            Done();
        });
    }

    private void RemoveHammer(Hammer hammer)
    {
        hammer.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
        {
            Destroy(hammer.gameObject);
        }).SetEase(Ease.InOutBack);
    }

}
