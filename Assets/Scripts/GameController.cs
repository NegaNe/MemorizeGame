using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;



public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    private Color[] ObjectColor = {Color.red, Color.green, Color.blue, Color.white, Color.yellow, Color.magenta, Color.gray, Color.cyan, Color.black};
    [SerializeField]
    private GameObject[] AnswersParent;
     
    public GameObject[] Answers;

public enum Difficulty
{
    Easy, 
    Normal, 
    Hard,
}

public Difficulty selectedDifficulty = Difficulty.Normal;


private void Start() {
    AnswersParent[(int)selectedDifficulty].SetActive(true);


GameObject SelectedParent = AnswersParent[(int)selectedDifficulty];
Answers = new GameObject[SelectedParent.transform.childCount];

    for (int i = 0; i< SelectedParent.transform.childCount; i++ ) 
    {
        Answers[i] = SelectedParent.transform.GetChild(i).gameObject;
    }
    
    Invoke(nameof(SpawnObjects), 3f);
}

private void SetDifficulty(Difficulty newDifficulty) {
    selectedDifficulty = newDifficulty;
    Start();
}

private void SpawnObjects() {
    Dictionary<Color, int> colorUsage = new Dictionary<Color, int>();

    foreach (var color in ObjectColor) {
        colorUsage[color] = 0;
    }

    foreach (var obj in Answers) {
        Color selectedColor = GetValidColor(colorUsage);
        if (selectedColor != default) {
            obj.GetComponent<Image>().color = selectedColor;
            colorUsage[selectedColor]++;
        } else {
            Debug.LogWarning("No more valid colors available.");
        }
    }
}

private Color GetValidColor(Dictionary<Color, int> colorUsage) {
    List<Color> availableColors = colorUsage
        .Where(c => c.Value < 2)  
        .Select(c => c.Key)
        .ToList();

    if (availableColors.Count > 0) {
        switch (selectedDifficulty) {
            case Difficulty.Easy:
                return availableColors[Random.Range(0, availableColors.Count-3)];
            case Difficulty.Normal:
                return availableColors[Random.Range(0, availableColors.Count-1)];
            case Difficulty.Hard:
                return availableColors[Random.Range(0, availableColors.Count)];
            default:
                return default;

        }
    }

    return default; 
}
}
