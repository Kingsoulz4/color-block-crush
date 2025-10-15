using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace ColorBlockCrush.Tools
{
    public class DraggableTankLineElementItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,
        IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerExitHandler
    {
        [Tooltip("Canvas to parent the dragged item into for proper rendering")]
        public Canvas dragCanvas;

        [Tooltip("Seconds to hold before dragging starts")]
        public float holdThreshold = 0.7f;

        [Tooltip("Enable if your list is bottom-to-top oriented")]
        public bool isBottomToTop = true;

        [Tooltip("Speed of auto-scroll when dragging near edges")]
        public float autoScrollSpeed = 5f;

        [Tooltip("Proportion of viewport height defining edge threshold (0-0.5)")]
        public float edgeThreshold = 0.1f;

        private ItemTankLineElementView _itemTankLineElementView;

        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private Transform originalParent;
        private GameObject placeholder;
        private ScrollRect scrollRect;
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
            _itemTankLineElementView = GetComponent<ItemTankLineElementView>();

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
            //canDrag = false;
        }

        private IEnumerator HoldDelay()
        {
            Debug.Log("Start Hold Delay");
            yield return new WaitForSeconds(holdThreshold);
            Debug.Log("End Hold Delay");
            canDrag = true;
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
                float topY = RectTransformUtility.WorldToScreenPoint(cam, corners[1]).y;
                float bottomY = RectTransformUtility.WorldToScreenPoint(cam, corners[0]).y;

                if (eventData.position.y > topY)
                    scrollRect.verticalNormalizedPosition += autoScrollSpeed * Time.deltaTime;
                else if (eventData.position.y < bottomY)
                    scrollRect.verticalNormalizedPosition -= autoScrollSpeed * Time.deltaTime;
            }

            // Determine new sibling index in original parent
            VerticalLayoutGroup verticalLayoutGroup = originalParent.GetComponent<VerticalLayoutGroup>();
            int newIndex = 0;
            for (int i = 0; i < originalParent.childCount; i++)
            {
                int childIndex = verticalLayoutGroup.reverseArrangement ? i : originalParent.childCount - 1 - i;
                var child = originalParent.GetChild(i);
                //if (child == placeholder.transform) continue;

                bool condition = isBottomToTop
                    ? rectTransform.position.y > child.position.y
                    : rectTransform.position.y < child.position.y;

                if (condition)
                    newIndex = i;
            }

            placeholder.transform.SetSiblingIndex(newIndex);
            _itemTankLineElementView.UpdateUiLinePos();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!canDrag) return;
            Debug.Log("On End Drag");
            // Place item into placeholder spot
            transform.SetParent(originalParent);
            int finalIndex = placeholder.transform.GetSiblingIndex();
            transform.SetSiblingIndex(finalIndex);
            _itemTankLineElementView.ChangeIndex(finalIndex);
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
