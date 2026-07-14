using UnityEngine;
using UnityEngine.UI;

public class SVImageContril : MonoBehaviour
{
    [SerializeField]
    private Image pickerImage;
    private RawImage SVimage;
    private ColourPickerControl CC;
    private RectTransform recTransform, pickerTransform;

    [SerializeField]
    PlayerControllerHuesAndCues playerController;

    PickerFollowsCharacterScript pickerScript;


    private void Awake()
    {
        SVimage = GetComponent<RawImage>();
        CC = FindObjectOfType<ColourPickerControl>();
        recTransform = GetComponent<RectTransform>();

        pickerTransform = pickerImage.GetComponent<RectTransform>();
        pickerTransform.position = new Vector2( -(recTransform.sizeDelta.x * 0.5f), -(recTransform.sizeDelta.y * 0.5f)); //Look if this pos even works
        pickerScript = SVimage.GetComponent<PickerFollowsCharacterScript>();
    }

    private void Update()
    {
        if (playerController != null)
        {
            if(playerController.IsInteractPressed() && pickerScript.IsFollowing)
            UpdateColour();
        }
    }

    public void UpdateColour()
    {
        Vector3 pos = pickerTransform.localPosition;


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

        pickerImage.color = Color.HSVToRGB(0, 0, 1 - yNorm);

        CC.SetSV(xNorm, yNorm);

    }

}
