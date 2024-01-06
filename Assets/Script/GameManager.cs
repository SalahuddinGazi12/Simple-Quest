using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TextMeshProUGUI targetNumberText;
    public TextMeshProUGUI signtext;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI Gameover_scoreText;
    public GameObject gameOverScreen;
    public GameObject tilePrefab;
    public Transform canvasTransform;

    private int targetNumber;
    private int score;
    private char currentSign;
    public List<Tile> tiles;
    private int firstClickedNumber = 0;
    private int number1;
    private int number2;
    private bool isFirstTimeGeneratingTiles = true;
    public List<Tile> matchedTilesList;
    private int row1,row2;
    private int column1,column2;
    int numberOfColumns;
    int numberOfRows;
    float tileWidth = 100f;
    float tileHeight = 100f;
    float spaceBetweenTiles = 50f;
    int state;
    public List<int> numberSet;
    [SerializeField] private Tile[,] tileArray;

    int timer = 120;
    public TextMeshProUGUI timer_text;

    int hint_remain = 3;
    public TextMeshProUGUI hint_remain_text;
    int undo_remain = 2;
    public TextMeshProUGUI undo_remain_text;

    private HashSet<int> generatedTargetNumbers;

    private Tile lastClickedTile;
    private void Awake()
    {
        Instance = this;
        matchedTilesList = new List<Tile>();
        state = 0;
        generatedTargetNumbers = new HashSet<int>();
    }

    void Start()
    {
        StartNewRound();
        StartCoroutine(StartTimer());
    }

    void StartNewRound()
    {

         score = 0;
         gameOverScreen.SetActive(false);
         Invoke("GenerateTiles", 0.5f);
         lastClickedTile = null;

    }
    IEnumerator StartTimer()
    {
        while (timer > 0)
        {
            yield return new WaitForSeconds(1);
            timer--;

            if (timer == 0)
            {
                GameOver();
            }
        }
    }

    private void Update()
    {
        UpdateUI();
    }

    public void GenerateTargetNumber()
    {
        StartCoroutine(TargetNUmber());
    }
    public void GenerateTargetNumberforhint()
    {
        if (hint_remain > 0)
        {
            StartCoroutine(TargetNUmber());
            hint_remain--;
        }
        else
        {
            GameObject hintbutton = GameObject.Find("Hints");
            hintbutton.GetComponent<Button>().enabled = false;
        }
    }
    public IEnumerator TargetNUmber()
    {
        yield return new WaitForSeconds(1);

        do
        {
            targetNumber = GenerateRandomNumber();

        } while (generatedTargetNumbers.Contains(targetNumber));

        generatedTargetNumbers.Add(targetNumber);
   

    }

    public int GenerateRandomNumber()
    {

        bool useSummation = Random.Range(0, 2) == 0;

        if (useSummation)
        {
            currentSign = '+';
        }
        else
        {
            currentSign = '-';
        }

        signtext.text = "Sign: " + currentSign;

        int index1 = Random.Range(0, numberSet.Count);
        int index2;

        do
        {
            index2 = Random.Range(0, numberSet.Count);
        } while (index2 == index1);

        number1 = numberSet[index1];
        number2 = numberSet[index2];

        int result = useSummation ? number1 + number2 : number1 - number2;

        
        result = Mathf.Clamp(result, 10, 100);
        

        Debug.Log("Number 1: " + number1);
        Debug.Log("Number 2: " + number2);
        Debug.Log("Result: " + result);

        return result;
    }


    void GenerateTiles()
    {
        int totalTiles = Random.Range(10, 20);
        numberOfColumns = Mathf.CeilToInt(Mathf.Sqrt(totalTiles));
        numberOfRows = Mathf.CeilToInt((float)totalTiles / numberOfColumns);

        if (isFirstTimeGeneratingTiles)
        {
            tileArray = new Tile[numberOfRows, numberOfColumns];
            isFirstTimeGeneratingTiles = false;
        }

        float totalWidth = numberOfColumns * (tileWidth + spaceBetweenTiles);
        float totalHeight = numberOfRows * (tileHeight + spaceBetweenTiles);

        float startX = -totalWidth / 2f + tileWidth / 2f;
        float startY = totalHeight / 2f - tileHeight / 2f;

        int tileIndex = 0;

        for (int row = 0; row < numberOfRows; row++)
        {
            for (int column = 0; column < numberOfColumns; column++)
            {
                Tile currentTile;

                if (state == 1)
                {
                    currentTile = tileArray[row, column];
                }
                else
                {
                    currentTile = Instantiate(tilePrefab, canvasTransform).GetComponent<Tile>();
                    tileArray[row, column] = currentTile;
                    StartCoroutine(GetNumber(currentTile));
                }

                currentTile.number = (row == row1 && column == column1) ? number1 :
                                     (row == row2 && column == column2) ? number2 :
                                     Random.Range(0, 99);

                float tileX = startX + column * (tileWidth + spaceBetweenTiles);
                float tileY = startY - row * (tileHeight + spaceBetweenTiles);

                currentTile.transform.localPosition = new Vector3(tileX, tileY, 0f);

                RectTransform tileRectTransform = currentTile.GetComponent<RectTransform>();
                tileRectTransform.sizeDelta = new Vector2(tileWidth, tileHeight);

                tileIndex++;
            }
        }

        for (int i = tileIndex; i < tiles.Count; i++)
        {
            tiles[i].gameObject.SetActive(false);
        }

        GenerateTargetNumber();


    }

    IEnumerator GetNumber(Tile currentTile)
    {
        yield return new WaitForSeconds(1);
        int number = currentTile.GetComponent<Tile>().number;

        numberSet.Add(number);

    }


    public void TileClicked(Tile clickedTile)
    {
        if (firstClickedNumber == 0)
        {

            firstClickedNumber = clickedTile.number;
            matchedTilesList.Add(clickedTile);
            lastClickedTile = clickedTile;
        }
        else
        {

            int secondClickedNumber = clickedTile.number;
            matchedTilesList.Add(clickedTile);

            int result = (currentSign == '+') ? firstClickedNumber + secondClickedNumber : firstClickedNumber - secondClickedNumber;


            if (result == targetNumber)
            {

                score++;
                RemoveMatchedTiles();
                state = 1;
                AudioManager.instance.Play("correct");

            }
            else
            {
                AudioManager.instance.Play("wrong");
                GameOver();
            }


            firstClickedNumber = 0;
        }
    }
    public void UndoClicked()
    {
        if (lastClickedTile != null)
        {
            if (undo_remain > 0)
            {
                firstClickedNumber = 0;
                matchedTilesList.Remove(lastClickedTile);
                lastClickedTile = null;
                undo_remain--;
            }
            else
            {
                GameObject undobutton = GameObject.Find("undo");
                undobutton.GetComponent<Button>().enabled = false;
            }
        }
    }

    void RemoveMatchedTiles()
    {


        foreach (Tile matchedTile in matchedTilesList)
        {
            int index = tiles.IndexOf(matchedTile);

            if (index != -1)
            {

                tiles.RemoveAt(index);
            }

            matchedTile.isRemoved = true;
            Destroy(matchedTile.gameObject);
            numberSet.Remove(matchedTile.number);
            
            

        }

        matchedTilesList.Clear();

        if (numberSet.Count == 0)
        {
            GameOver();
        }
        else
        {
            GenerateTargetNumber();
        }
    }
    void UpdateUI()
    {

        scoreText.text = "Score: " + score;
        timer_text.text = "" + timer;
        Gameover_scoreText.text = "Your Score: " + score;
        hint_remain_text.text = "" + hint_remain;
        undo_remain_text.text = "" + undo_remain;
        targetNumberText.text = "" + targetNumber;
    }
    void GameOver()
    {

        gameOverScreen.SetActive(true);

    }
}