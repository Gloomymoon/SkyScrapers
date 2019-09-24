using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using V3;

public class GameBoard : PanelBase
{   
    [SerializeField]
    private int boardWidth = 500;

    [SerializeField]
    private int indicatorHeight = 50;

    public BuildingCoordination CurrentCoordination { get => currentCoordination; set => currentCoordination = value; }

    [SerializeField]
    private Text headerText;

    [SerializeField]
    private GameObject block; 

    private GridLayoutGroup blockLayout;

    private Building[,] buildings;

    [SerializeField]
    private Building buildingPrefab;

    [SerializeField]
    private Text indicatorPrefab;

    // the order must be: top right bott left
    [SerializeField]
    private GameObject[] indicatorContainer;

    private string[] indicatorArrow = {"↓","←","↑","→"};

    private BuildingCoordination currentCoordination;

    [SerializeField]
    private Sprite[] baseImages;

    [SerializeField]
    private PanelBase winPanel;

    [SerializeField]
    private PanelBase exitPanel;

    [SerializeField]
    private PanelBase helpPanel;

    [SerializeField]
    private PanelBase resetPanel;

    [SerializeField]
    private Text errorInfo;

    [SerializeField]
    private Selector selector;

    [SerializeField]
    private Button hintButton;

    [SerializeField]
    private Text hintText;

    [SerializeField]
    private float hintTimeCount = 10f;

    private float hintTime = 0;

    private string[] hints = {"Click right button to show an answer.", 
                              "Use your brain, if you have one.",
                              "Do or do not, there is no try.",
                              "Click √ to check the answer.",
                              "Fill all blank cells to win the game.",
                              "If you have any suggestions, I don't care.",
                              "Do not estimate the power you don't understand.",
                              "May the intelligence be with you.",
                              "Talk is cheep, fill the blanks.",
                              "Ha ha ha ...",
                              "Fix bug reporter instead of the bug.",
                              "Keep thinking hard.",
                              "Each man is the architect of his own fate.",
                              "Nothing is difficult to a willing heart.",
                              "Stay hungry, stay foolish",
                              "Keep moving. Don't settle.",
                              "All things are difficult before they are easy.",
                              "There is no shortcuts to success.",
                              "Time is money, my firend.",
                              "You came, you saw, you conquered.",
                              ""};

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        buildings = new Building[GameManager.Instance.MaxSize, GameManager.Instance.MaxSize];

        blockLayout = block.GetComponent<GridLayoutGroup>();

        for(int i=0;i<GameManager.Instance.MaxSize;i++)
        {
            for(int j=0;j<GameManager.Instance.MaxSize;j++)
            {
                Building go = Instantiate(buildingPrefab) as Building;
                go.transform.SetParent(block.transform);
                go.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
                go.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
                go.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                go.SetLevel(0);
                go.Coordination = new BuildingCoordination(i, j);
                go.transform.localScale = new Vector3(1, 1, 1);       
                //if(i >= 4 || j >= 4)
                go.gameObject.SetActive(false);         
                buildings[i,j] = go;

                if(i < 4)
                {
                    Text t = Instantiate(indicatorPrefab) as Text;
                    t.rectTransform.localScale = new Vector3Int(1, 1, 1);
                    t.transform.SetParent(indicatorContainer[i].transform);
                }
            }
        }
    }

    void Update()
    {
        hintTime = hintTime + Time.deltaTime;
        if (hintTime > hintTimeCount)
        {   hintText.text = "";
            if(UnityEngine.Random.Range(1, 100) > 90)
            {
                hintText.text = string.Join("", hints.ToList().OrderBy(d => Guid.NewGuid()).Take(1));
            }
            hintTime = 0;
        }
    }


    public void InitBoard(GameInfo gameInfo)
    {
        selector.InitButtonPosition(gameInfo.Size);

        hintButton.GetComponentInChildren<Text>().text = gameInfo.Hint.ToString();

        
        headerText.text = gameInfo.Size.ToString() + "x" + gameInfo.Size.ToString() + " " + GameManager.Instance.LevelList.LevelItems[gameInfo.Size].Diff_desc;

        // Building的尺寸是按照100设计的
        float scale = boardWidth / 100f / (gameInfo.Size);
        block.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, boardWidth / scale);
        block.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, boardWidth / scale);
        block.transform.localScale = new Vector2(scale, scale);

        int indicatorWidth = boardWidth / gameInfo.Size;

        indicatorContainer[0].GetComponent<GridLayoutGroup>().cellSize = new Vector2(indicatorWidth, indicatorHeight);
        indicatorContainer[1].GetComponent<GridLayoutGroup>().cellSize = new Vector2(indicatorHeight, indicatorWidth);
        indicatorContainer[2].GetComponent<GridLayoutGroup>().cellSize = new Vector2(indicatorWidth, indicatorHeight);
        indicatorContainer[3].GetComponent<GridLayoutGroup>().cellSize = new Vector2(indicatorHeight, indicatorWidth);

        for(int i=0;i<GameManager.Instance.MaxSize;i++)
            for(int j=0;j<GameManager.Instance.MaxSize;j++)
            {
                if(i >= gameInfo.Size || j >= gameInfo.Size)
                {
                    buildings[i,j].gameObject.SetActive(false);
                }
                else{
                    buildings[i,j].gameObject.SetActive(true);
                    if(gameInfo.Lcq[i,j].Equals('.'))
                    {
                        buildings[i,j].Changeable = true;
                        buildings[i,j].SetLevel(0);
                    }
                    else
                    {
                        buildings[i,j].Changeable = false;
                        buildings[i,j].SetLevel(int.Parse(gameInfo.Lcq[i,j].ToString()));
                    }
                }

                if(i < 4)
                {
                    indicatorContainer[i].transform.GetChild(j).GetComponent<Text>().text = "";
                    indicatorContainer[i].transform.GetChild(j).GetComponent<Text>().transform.localScale = new Vector3(1,1,1);
                    if(j < gameInfo.Size)
                    {
                        if(i==0 && gameInfo.Top[j] != '0')
                        {
                            //indicatorContainer[i].transform.GetChild(j).GetComponent<Text>().text = gameInfo.Top[j].ToString() + "\n" + indicatorArrow[i];
                            indicatorContainer[i].transform.GetChild(j).GetComponent<Text>().text = gameInfo.Top[j].ToString();
                        }
                        if(i==1 && gameInfo.Right[j] != '0')
                        {
                            //indicatorContainer[i].transform.GetChild(j).GetComponent<Text>().text = gameInfo.Right[j].ToString() + indicatorArrow[i];
                            indicatorContainer[i].transform.GetChild(j).GetComponent<Text>().text = gameInfo.Right[j].ToString();
                        }
                        if(i==2 && gameInfo.Bott[j] != '0')
                        {
                            //indicatorContainer[i].transform.GetChild(j).GetComponent<Text>().text = indicatorArrow[i] + "\n" + gameInfo.Bott[j].ToString();
                            indicatorContainer[i].transform.GetChild(j).GetComponent<Text>().text = gameInfo.Bott[j].ToString();
                        }
                        if(i==3 && gameInfo.Left[j] != '0')
                        {
                            //indicatorContainer[i].transform.GetChild(j).GetComponent<Text>().text = indicatorArrow[i] + gameInfo.Left[j].ToString();
                            indicatorContainer[i].transform.GetChild(j).GetComponent<Text>().text = gameInfo.Left[j].ToString();
                        }
                    }

                }
            }
        CloseAllPanel();
    }

    public void RestartNewGame()
    {
        int size = GameManager.Instance.GameInfo.Size;
        int diff = GameManager.Instance.GameInfo.Diff;
        GameManager.Instance.GameInfo = null;
        GameManager.Instance.SaveGameInfo();
        GameManager.Instance.StartNewGame(size, diff);
        CloseAllPanel();
        
    }

    public void RestGoard()
    {
        InitBoard(GameManager.Instance.GameInfo);
    }

    public void Hint()
    {
        GameInfo gameInfo = GameManager.Instance.GameInfo;
        if(gameInfo.Hint > 0 && !(gameInfo.CheckAnswer() && gameInfo.IsFilled()))
        {
            int i = UnityEngine.Random.Range(0, gameInfo.Size);
            int j = UnityEngine.Random.Range(0, gameInfo.Size);
            while(gameInfo.Answer[i,j] == gameInfo.Lca[i,j])
            {
                i = UnityEngine.Random.Range(0, gameInfo.Size);
                j = UnityEngine.Random.Range(0, gameInfo.Size);
            }
            currentCoordination = new BuildingCoordination(i, j);
            //Debug.Log(gameInfo.Lca[i,j]);
            gameInfo.Lcq[i, j] = gameInfo.Lca[i,j].ToString()[0];
            buildings[i, j].Changeable = false;
            SetCurrentLevel(gameInfo.Lca[i,j]);
            GameManager.Instance.GameInfo.Hint--;
            hintButton.GetComponentInChildren<Text>().text = GameManager.Instance.GameInfo.Hint.ToString();

            Debug.Log(gameInfo.ToString());
        }
    }

    public void BackToMain()
    {
        GameManager.Instance.SaveGameInfo();
        GameManager.Instance.LevelList.Open();
        CloseAllPanel();
        Close();
    }

    public void ClearAndExit()
    {
        GameManager.Instance.GameInfo = null;
        BackToMain();
    }

    public void ShowHelp()
    {
        helpPanel.Open();
    }

    public void ShowExit()
    {
        GameManager.Instance.Dialog.Show("Exit", "Do you want to quit the game?", "EXIT", "CANCEL", 2, 0, 
                                         new UnityAction(BackToMain), 
                                         null);
    }

    public void ShowReset()
    {
        GameManager.Instance.Dialog.Show("RESTART", "Do you want to restart?", "RESTART", "CANCEL", 2, 0, 
                                         new UnityAction(RestGoard), 
                                         null);
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }

    public void CheckGame()
    {
        string checkInfo = "Everything looks fine :)";
        if(!GameManager.Instance.GameInfo.CheckAnswer())
        {
            checkInfo = "Oops, something is wrong...";
        }
        errorInfo.text = checkInfo;
        errorInfo.color = Color.white;
    }


    public void ShowSelector(Building sender)
    {
        selector.Open();
        currentCoordination = sender.Coordination;
    }

    public void SetCurrentLevel(int newLevel)
    {
        errorInfo.color = new Color(0,0,0,0);
        buildings[currentCoordination.X, currentCoordination.Y].SetLevel(newLevel);
        GameManager.Instance.GameInfo.Answer[currentCoordination.X, currentCoordination.Y] = newLevel;
        selector.Close();
        errorInfo.color = new Color(0,0,0,0);
        GameManager.Instance.SaveGameInfo();
        if(GameManager.Instance.GameInfo.IsFilled())
        {
            if(GameManager.Instance.GameInfo.CheckAnswer())
            {
                // Game End
                GameManager.Instance.AddGameProgress();
                //winPanel.Open();
                
                GameManager.Instance.Dialog.Show("YOU WIN", "Contratulations!\nWant to play again?", "NEW GAME", "EXIT", 2, 0, 
                                         new UnityAction(RestartNewGame), 
                                         new UnityAction(ClearAndExit));
            }
            else {
                // Wrong answer
                errorInfo.color = Color.white;
            }
        }

    }

    public void CloseAllPanel()
    {
        errorInfo.color = new Color(0,0,0,0);
        selector.Close();
        //winPanel.Close();
        //exitPanel.Close();
        helpPanel.Close();
        //resetPanel.Close();
    }

}


