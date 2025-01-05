using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    private Color[] ObjectColor = {
        new Color(0f, 1f, 1f), // Cyan
        new Color(0f, 1f, 0f), // Green
        new Color(1f, 0f, 1f), // Magenta
        new Color(0f, 0f, 0f), // Black
        new Color(0f, 0.5f, 0.5f), // Dark Cyan
        new Color(0.5f, 0.5f, 0f), // Olive
        new Color(0.5f, 0f, 0.5f), // Purple
        new Color(0f, 0.5f, 0f), // Dark Green
        new Color(0.5f, 0f, 0f), // Maroon
        new Color(0f, 0f, 0.5f), // Navy
        new Color(1f, 1f, 0f), // Yellow
        new Color(1f, 0.5f, 0f), // Orange
        new Color(1f, 0f, 0f), // Red
        new Color(0.5f, 0.5f, 0.5f), // Gray
        new Color(0.75f, 0.75f, 0.75f), // Light Gray
        new Color(1f, 1f, 1f), // White
    };

    [SerializeField] private GameObject[] AnswersParent;
    public GameObject[] Answers;

    public Sprite hiddenSprite;
    public Sprite revealedSprite;

    public enum Difficulty { Easy, Normal, Hard }
    public Difficulty selectedDifficulty = Difficulty.Normal;

    private List<Color> correctColors = new List<Color>();
    private int currentAnswerIndex = 0;

    [SerializeField] private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;

    private GameObject firstSelectedObject = null;
    private GameObject secondSelectedObject = null;

    private HashSet<GameObject> revealedObjects = new HashSet<GameObject>();

    private void Start()
    {
        AnswersParent[(int)selectedDifficulty].SetActive(true);
        GameObject selectedParent = AnswersParent[(int)selectedDifficulty];
        Answers = new GameObject[selectedParent.transform.childCount];

        for (int i = 0; i < selectedParent.transform.childCount; i++)
        {
            Answers[i] = selectedParent.transform.GetChild(i).gameObject;
        }

        Invoke(nameof(SpawnObjects), 1f);
    }

    private void SpawnObjects()
    {
        List<Color> shuffledColors = new List<Color>(ObjectColor);
        shuffledColors = shuffledColors.OrderBy(x => Random.value).ToList();

        correctColors.Clear();
        int pairCount = Answers.Length / 2;

        for (int i = 0; i < pairCount; i++)
        {
            correctColors.Add(shuffledColors[i]);
            correctColors.Add(shuffledColors[i]);
        }

        correctColors = correctColors.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < Answers.Length; i++)
        {
            Answers[i].GetComponent<Image>().color = correctColors[i];
        }

        Invoke(nameof(HideColors), 3f);
    }

    private void HideColors()
    {
        foreach (var obj in Answers)
        {
            obj.GetComponent<Image>().sprite = hiddenSprite;
            obj.GetComponent<Image>().color = Color.white;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DetectClick();
        }
    }

    private void DetectClick()
    {
        pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        if (results.Count > 0)
        {
            GameObject clickedObject = results[0].gameObject;
            if (!revealedObjects.Contains(clickedObject))
            {
                HandleSelection(clickedObject);
            }
        }
    }

    private void HandleSelection(GameObject selectedObject)
    {
        if (firstSelectedObject == null)
        {
            firstSelectedObject = selectedObject;
            RevealColor(firstSelectedObject);
        }
        else if (secondSelectedObject == null && selectedObject != firstSelectedObject)
        {
            secondSelectedObject = selectedObject;
            RevealColor(secondSelectedObject);

            Invoke(nameof(CheckPair), 1f);
        }
    }

    private void RevealColor(GameObject obj)
    {
        int index = Array.IndexOf(Answers, obj);
        if (index >= 0)
        {
            obj.GetComponent<Image>().sprite = revealedSprite;
            obj.GetComponent<Image>().color = correctColors[index];
        }
    }

    private void CheckPair()
    {
        if (firstSelectedObject != null && secondSelectedObject != null)
        {
            Color firstColor = firstSelectedObject.GetComponent<Image>().color;
            Color secondColor = secondSelectedObject.GetComponent<Image>().color;

            if (firstColor == secondColor)
            {
                revealedObjects.Add(firstSelectedObject);
                revealedObjects.Add(secondSelectedObject);
            }
            else
            {
                firstSelectedObject.GetComponent<Image>().sprite = hiddenSprite;
                secondSelectedObject.GetComponent<Image>().sprite = hiddenSprite;
                firstSelectedObject.GetComponent<Image>().color = Color.white;
                secondSelectedObject.GetComponent<Image>().color = Color.white;
            }

            firstSelectedObject = null;
            secondSelectedObject = null;
        }
    }
}
