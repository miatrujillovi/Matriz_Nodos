using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSelector : MonoBehaviour
{
    //Public Variables
    public List<Button> buttons;
    public RectTransform movingImg;
    public float moveSpeed;

    //Private Variables
    private Button initialBTN = null;
    private Button finalBTN = null;

    //Start is called before the first frame update
    void Update()
    {
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => ButtonClicked(button));
        }
    }

    //Function for when a Button is selected
    void ButtonClicked(Button clickedButton)
    {
        if (initialBTN == null)
        {
            initialBTN = clickedButton;
            HighlightButton(clickedButton, true);
        } 
        else if (finalBTN == null && clickedButton != initialBTN)
        {
            finalBTN = clickedButton;
            HighlightButton(clickedButton, true);

            StartCoroutine(MoveImage());
        } 
        else
        {
            ResetSelections();
        }
    }

    IEnumerator MoveImage()
    {
        Vector3 startPosition = initialBTN.transform.position;
        Vector3 endPosition = finalBTN.transform.position;

        movingImg.gameObject.SetActive(true);
        movingImg.position = startPosition;

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            elapsedTime += moveSpeed * Time.deltaTime;
            movingImg.position = Vector3.Lerp(startPosition, endPosition, elapsedTime);
            yield return null;
        }

        movingImg.position = endPosition;
    }

    //Function to reset selected buttons
    void ResetSelections()
    {
        if (initialBTN != null)
        {
            HighlightButton(initialBTN, false);
        }

        if (finalBTN != null)
        {
            HighlightButton(finalBTN, false);
        }

        initialBTN = null;
        finalBTN = null;
    }

    //Function to make the button appear has "selected"
    void HighlightButton(Button button, bool highlight)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = highlight ? Color.red : Color.white;
        button.colors = colors;
    }
}
