using UnityEngine;
using UnityEngine.Assemblies;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class PickerFollowsCharacterScript : MonoBehaviour
{
    [SerializeField]
    RectTransform cursorTransform;

    [SerializeField]
    CharacterController characterController;

    [SerializeField]
    bool isSlider;

    private bool isFollowing = false;

    RectTransform parentRect;
    Canvas parentCanvas;
    Vector3 worldPos;


    private void Start()
    {
        if (characterController == null || cursorTransform == null)
        {
            throw new System.Exception("CharacterController and FieldOfCursor must be assigned in the inspector.");
        }

        parentRect = cursorTransform.parent as RectTransform;
        parentCanvas = cursorTransform.GetComponentInParent<Canvas>();

    }

    private void Update()
    {
        if (characterController == null || cursorTransform == null)
            return;

        // World position directly below the character on the panel/screen
        worldPos = characterController.transform.position;

        // Determine whether the character is currently over the panel (treat CharacterController like a pointer)
        bool overlap = false;
        if (parentRect != null && parentCanvas != null && parentCanvas.renderMode == RenderMode.WorldSpace)
        {
            Transform panelT = parentRect.transform;
            Vector3 panelNormal = panelT.forward;
            float distance = Vector3.Dot(panelNormal, worldPos - panelT.position);
            Vector3 projected = worldPos - panelNormal * distance;
            Vector3 localPoint = panelT.InverseTransformPoint(projected);
            Rect rect = parentRect.rect;
            overlap = rect.Contains(new Vector2(localPoint.x, localPoint.y));
            worldPos = projected; // use projected for cursor placement
        }
        else if (parentRect != null)
        {
            Camera camForCanvas = null;
            if (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                camForCanvas = parentCanvas.worldCamera != null ? parentCanvas.worldCamera : Camera.main;
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(camForCanvas, worldPos);
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPoint, camForCanvas, out Vector2 localPoint))
            {
                Rect rect = parentRect.rect;
                overlap = rect.Contains(localPoint);
            }
        }

        // Update following state based on overlap
        isFollowing = overlap;

        if (!isFollowing)
            return;

        if (parentCanvas != null && parentRect != null && parentCanvas.renderMode == RenderMode.WorldSpace)
        {
            // Project the character world position onto the plane of the panel, then convert
            // that point into the panel's local space so it maps correctly regardless of rotation.
            Transform panelT = parentRect.transform;
            Vector3 panelNormal = panelT.forward;
            // Distance from worldPos to plane along normal
            float distance = Vector3.Dot(panelNormal, worldPos - panelT.position);
            Vector3 projected = worldPos - panelNormal * distance;

            Vector3 localPoint = panelT.InverseTransformPoint(projected);

            // Clamp inside the panel rect
            Rect rect = parentRect.rect;
            float clampedX = 0;
            if (!isSlider)
            {
                clampedX = Mathf.Clamp(localPoint.x, rect.xMin, rect.xMax);
            }
            float clampedY = Mathf.Clamp(localPoint.y, rect.yMin, rect.yMax);

            // Always keep Z = 0 in local space so the cursor sits flush on the panel
            cursorTransform.localPosition = new Vector3(clampedX, clampedY, 0f);
        }
        else if (parentRect != null)
        {
            // For Screen Space canvases (Overlay or Camera) convert via screen point
            Camera camForCanvas = null;
            if (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                camForCanvas = parentCanvas.worldCamera != null ? parentCanvas.worldCamera : Camera.main;

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(camForCanvas, worldPos);
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPoint, camForCanvas, out Vector2 localPoint))
            {
                // Clamp to panel rect
                Rect rect = parentRect.rect;
                float clampedX = Mathf.Clamp(localPoint.x, rect.xMin, rect.xMax);
                float clampedY = Mathf.Clamp(localPoint.y, rect.yMin, rect.yMax);
                cursorTransform.anchoredPosition = new Vector2(clampedX, clampedY);
            }
            else
            {
                // fallback
                cursorTransform.position = screenPoint;
            }
        }
        else
        {
            // No parent rect/canvas: fallback to world XZ mapping keeping cursor Y
            Vector3 cur = cursorTransform.position;
            cursorTransform.position = new Vector3(worldPos.x, cur.y, worldPos.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (characterController == null)
            return;

        if (other.gameObject == characterController.gameObject)
        {
            isFollowing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (characterController == null)
            return;

        if (other.gameObject == characterController.gameObject)
        {
            isFollowing = false;
        }
    }
}
