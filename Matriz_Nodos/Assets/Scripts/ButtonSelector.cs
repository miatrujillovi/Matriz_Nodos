using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSelector : MonoBehaviour
{
    //Public Variables
    public Button[,] buttonsGrid;
    public int rows, cols;
    public RectTransform movingImg;
    public float moveSpeed;

    //Private Variables
    private Button initialBTN = null;
    private Button finalBTN = null;

    //Start is called before the first frame update
    void Start()
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                Button button = buttonsGrid[r, c];
                if (button != null)
                {
                    button.onClick.AddListener(() => ButtonClicked(button));
                }
            }
        }
    }

    void Awake()
    {
        buttonsGrid = new Button[rows, cols];
        Button[] allButtons = GetComponentsInChildren<Button>();

        int index = 0;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (index < allButtons.Length)
                {
                    buttonsGrid[r, c] = allButtons[index];
                    index++;
                }
            }
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

            List<Button> path = FindPath(initialBTN, finalBTN);
            StartCoroutine(MoveImage(path));
        }
        else
        {
            ResetSelections();
        }
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
        Debug.Log("Selections reseted");
    }

    //Function to make the button appear has "selected"
    void HighlightButton(Button button, bool highlight)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = highlight ? Color.red : Color.white;
        button.colors = colors;
    }

    //Function that helps us get the closest buttons to our location
    List<Button> GetNeighbors(Button current)
    {
        List<Button> neighbors = new List<Button>();
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (buttonsGrid[r, c] == current)
                {
                    if (r > 0) neighbors.Add(buttonsGrid[r - 1, c]); // Up Btn
                    if (r < rows - 1) neighbors.Add(buttonsGrid[r + 1, c]); // Down Btn
                    if (c > 0) neighbors.Add(buttonsGrid[r, c - 1]); // Left Btn
                    if (c < cols - 1) neighbors.Add(buttonsGrid[r, c + 1]); // Right Btn
                }
            }
        }
        return neighbors;
    }

    //Function that helps us find the best path
    List<Button> FindPath(Button start, Button end)
    {
        Queue<Button> queue = new Queue<Button>();
        Dictionary<Button, Button> cameFrom = new Dictionary<Button, Button>();
        queue.Enqueue(start);
        cameFrom[start] = null;

        while (queue.Count > 0)
        {
            Button current = queue.Dequeue();

            if (current == end) break;

            foreach (Button neighbor in GetNeighbors(current))
            {
                if (!cameFrom.ContainsKey(neighbor))
                {
                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }

        List<Button> path = new List<Button>();
        Button step = end;

        while (step != null)
        {
            path.Add(step);
            step = cameFrom[step];
        }

        path.Reverse();
        return path;
    }

    //Coroutine to move the Image through the buttons
    IEnumerator MoveImage(List<Button> path)
    {
        movingImg.gameObject.SetActive(true);
        movingImg.position = path[0].transform.position;

        foreach (Button button in path)
        {
            Vector3 targetPosition = button.transform.position;

            while ((movingImg.position - targetPosition).magnitude > 0.1f)
            {
                float step = moveSpeed * Time.deltaTime;
                movingImg.position = Vector3.MoveTowards(movingImg.position, targetPosition, step);
                yield return null;
            }

            movingImg.position = targetPosition; // Asegúrate de que se ajuste exactamente
        }

        movingImg.gameObject.SetActive(false);
        ResetSelections();
    }
}
