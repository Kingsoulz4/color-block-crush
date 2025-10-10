using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ColorBlockCrush.Tools
{
    public class DraggableTunnelItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerExitHandler
{
    [Tooltip("Canvas to parent the dragged item into for proper rendering")] public Canvas dragCanvas;
    [Tooltip("Seconds to hold before dragging starts")] public float holdThreshold = 0.7f;
    [Tooltip("Enable if your list is right-to-left oriented")] public bool isRightToLeft = true;
    [Tooltip("Speed of auto-scroll when dragging near edges")]
    public float autoScrollSpeed = 5f;
    [Tooltip("Proportion of viewport height defining edge threshold (0-0.5)")]
    public float edgeThreshold = 0.1f;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private GameObject placeholder;
    private ScrollRect scrollRect;
    private int oldIndex;
    [SerializeField] private bool canDrag = false;
    private Coroutine holdCoroutine;
    [SerializeField] private Image bg;

    void Awake()
    {
        if (dragCanvas == null)
        {
            var rootCanvas = GetComponentInParent<Canvas>();
            if (rootCanvas != null)
                dragCanvas = rootCanvas;
            else
                Debug.LogError("DraggableItem: No Canvas found at the root of the hierarchy.");
        }
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = true;
        bg.color = Color.black;
    }

    private void Start()
    {
        scrollRect = transform.parent.parent.parent.GetComponent<ScrollRect>();
        if (scrollRect == null)
            Debug.LogError("DraggableItem: No ScrollRect found in parent hierarchy.");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Start hold timer        
        holdCoroutine = StartCoroutine(HoldDelay());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Cancel dragging if released before threshold
        if (holdCoroutine != null)
            StopCoroutine(holdCoroutine);
        transform.localScale = Vector3.one;
        //canDrag = false;
    }

    private IEnumerator HoldDelay()
    {
        Debug.Log("Start Hold Delay");
        yield return new WaitForSeconds(holdThreshold);
        Debug.Log("End Hold Delay");
        canDrag = true;
        transform.localScale = Vector3.one * 1.25f;
        bg.color = Color.white;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag)
        {
            return; // ignore drag if hold not passed
        }
        Debug.Log("On Begin Drag");
        // Create placeholder to reserve space
        oldIndex = transform.GetSiblingIndex(); Debug.Log("Old Index " + oldIndex);
        placeholder = new GameObject("Placeholder");
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        var rt = placeholder.AddComponent<RectTransform>();
        rt.sizeDelta = rectTransform.sizeDelta;
        placeholder.transform.SetParent(transform.parent, false);
        placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex());        
        originalParent = transform.parent;
        // Move dragged item to canvas root to avoid layout constraints        
        transform.SetParent(dragCanvas.transform);        
        canvasGroup.blocksRaycasts = false;
        //canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;

        // Follow pointer
        Vector2 localPoint;
        RectTransform canvasRect = dragCanvas.transform as RectTransform;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            rectTransform.anchoredPosition = localPoint;
        }

        // auto-scroll if near edges
        if (scrollRect != null)
        {
            var vp = scrollRect.viewport;
            // get world corners of viewport
            Vector3[] corners = new Vector3[4];
            vp.GetWorldCorners(corners);
            Camera cam = eventData.pressEventCamera;
            float topX = RectTransformUtility.WorldToScreenPoint(cam, corners[1]).x;
            float bottomX = RectTransformUtility.WorldToScreenPoint(cam, corners[0]).x;

            if (eventData.position.x > topX)
                scrollRect.verticalNormalizedPosition += autoScrollSpeed * Time.deltaTime;
            else if (eventData.position.x < bottomX)
                scrollRect.verticalNormalizedPosition -= autoScrollSpeed * Time.deltaTime;
        }

        // Determine new sibling index in original parent
        int newIndex = 0;
        for (int i = 0; i < originalParent.childCount; i++)
        {
            var child = originalParent.GetChild(i);
            //if (child == placeholder.transform) continue;

            bool condition = isRightToLeft
                ? rectTransform.position.x > child.position.x
                : rectTransform.position.x < child.position.x;

            if (condition)
                newIndex = i;
        }
        placeholder.transform.SetSiblingIndex(newIndex);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        Debug.Log("On End Drag");
        // Place item into placeholder spot
        transform.SetParent(originalParent);
        int finalIndex = placeholder.transform.GetSiblingIndex();
        transform.SetSiblingIndex(finalIndex);
        transform.localScale = Vector3.one;
        GetComponent<ItemTunnelQueueView>().ChangeIndex(oldIndex, finalIndex);
        // Cleanup
        Destroy(placeholder);
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        canDrag = false;
        bg.color = Color.black;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (holdCoroutine != null)
            StopCoroutine(holdCoroutine);       
    }
}
}
