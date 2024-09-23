using UnityEngine;
using UnityEngine.EventSystems; // Needed for handling UI events

namespace AHUI
{
    public class DragableWindow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private RectTransform rectTransform;
        private Canvas canvas;
        private Vector2 offset;

        private void Awake()
        {
            // Get the RectTransform of the UI element
            rectTransform = GetComponent<RectTransform>();

            // Find the parent Canvas, required for converting mouse positions
            canvas = GetComponentInParent<Canvas>();
        }

        // This is called when dragging starts
        public void OnBeginDrag(PointerEventData eventData)
        {
            // Calculate the offset between the mouse position and the window's position
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out offset);
        }

        // This is called during dragging
        public void OnDrag(PointerEventData eventData)
        {
            if (rectTransform != null && canvas != null)
            {
                Vector2 mousePos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out mousePos);
                rectTransform.anchoredPosition = mousePos - offset; // Update position
            }
        }

        // This is called when dragging ends (optional for cleanup)
        public void OnEndDrag(PointerEventData eventData)
        {
            // You can add any logic here when the drag ends if needed
        }
    }
}
