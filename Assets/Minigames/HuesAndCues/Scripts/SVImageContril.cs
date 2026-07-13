using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SVImageContril : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    [SerializeField]
    private Image pickerImage;
    private RawImage SVimage;
    private ColourPickerControl CC;
    private RectTransform recTransform, pickerTransform;


    private void Awake()
    {
        SVimage = GetComponent<RawImage>();
        CC = FindObjectOfType<ColourPickerControl>();
        recTransform = GetComponent<RectTransform>();

        pickerTransform = pickerImage.GetComponent<RectTransform>();
        pickerTransform.position = new Vector2( -(recTransform.sizeDelta.x * 0.5f), -(recTransform.sizeDelta.y * 0.5f)); //Look if this pos even works
    }

    void UpdateColour(PointerEventData eventData)
    {
        Vector3 pos = recTransform.InverseTransformPoint(eventData.position);

        float deltaX = recTransform.sizeDelta.x * 0.5f;
        float deltaY = recTransform.sizeDelta.y * 0.5f;

        if(pos.x < -deltaX) pos.x = -deltaX;
        else if(pos.x > deltaX) pos.x = deltaX;

        if(pos.y < -deltaY) pos.y = -deltaY;
        else if (pos.y > deltaY) pos.y = deltaY;

        float x = pos.x + deltaX;
        float y = pos.y + deltaY;

        float xNorm = x / recTransform.sizeDelta.x;
        float yNorm = y / recTransform.sizeDelta.y;

        pickerTransform.localPosition = pos;
        pickerImage.color = Color.HSVToRGB(0, 0, 1 - yNorm);

        CC.SetSV(xNorm, yNorm);

    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateColour(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UpdateColour(eventData);
    }
}
