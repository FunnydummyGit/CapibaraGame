using UnityEngine;
using UnityEngine.UI;

public class SelectSliderValue : MonoBehaviour
{

    [SerializeField]
    private Image pickerImage;
    private RawImage HueImage;
    private RectTransform recTransform, pickerTransform;

    [SerializeField]
    PlayerControllerHuesAndCues playerController;

    PickerFollowsCharacterScript pickerScript;
    Slider hueSlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        HueImage = GetComponent<RawImage>();
        recTransform = GetComponent<RectTransform>();

        pickerTransform = pickerImage.GetComponent<RectTransform>();
        pickerTransform.position = new Vector2(-(recTransform.sizeDelta.x * 0.5f), -(recTransform.sizeDelta.y * 0.5f)); //Look if this pos even works
        pickerScript = HueImage.GetComponent<PickerFollowsCharacterScript>();
        hueSlider = HueImage.GetComponentInChildren<Slider>();
    }


    private void Update()
    {
        if (playerController != null)
        {
            if (playerController.IsInteractPressed() && pickerScript.IsFollowing)
                UpdateColour();
        }
    }

    public void UpdateColour()
    {
        Vector3 pos = pickerTransform.localPosition;

        float deltaY = recTransform.sizeDelta.y * 0.5f;

        if (pos.y < -deltaY) pos.y = -deltaY;
        else if (pos.y > deltaY) pos.y = deltaY;

        float y = pos.y + deltaY;

        float yNorm = y / recTransform.sizeDelta.y;

        pickerImage.color = Color.HSVToRGB(0, 0, 1 - yNorm);
        // Update the slider value based on normalized position (0..1)
        if (hueSlider != null)
        {
            // normalizedValue maps directly to 0..1 regardless of slider min/max
            hueSlider.normalizedValue = Mathf.Clamp01(yNorm);
        }

    }
}
