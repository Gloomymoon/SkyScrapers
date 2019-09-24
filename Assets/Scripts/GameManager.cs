using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get {
            if (instance == null){
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }


    public GameInfo GameInfo { get => gameInfo; set => gameInfo = value; }
    public int MaxSize { get => maxSize;}
    public LevelList LevelList { get => levelList;}
    public GameBoard GameBoard { get => gameBoard;}
    public Dialog Dialog { get => dialog; }

    private GameInfo gameInfo = null;

    [SerializeField]
    private LevelList levelList;

    [SerializeField]
    private GameBoard gameBoard;

    [SerializeField]
    private int maxSize = 7;

    [SerializeField]
    private Dialog dialog;

    private List<Dictionary<string, object>> repoList;

    void Awake()
    {
        repoList = CSVReader.Read ("repo");
        LoadGameInfo();
    }

    // Start is called before the first frame update
    void Start()
    {
        //sqlite = new SQLiteHelper("data source=" + Application.streamingAssetsPath + "//data.db");
        LevelList.Open();
        GameBoard.Close();
        GameBoard.CloseAllPanel();
    }

    public GameInfo GetRandomGameInfo(int size, int diff)
    {
        IEnumerable<Dictionary<string, object>> query = from repo in repoList
                                                        where repo["size"].ToString().Equals(size.ToString())
                                                        where repo["diff"].ToString().Equals(diff.ToString())
                                                        orderby Guid.NewGuid()
                                                        select repo;
        if(query != null)
        {
            Dictionary<string, object> gameInfoDic = query.ToList()[0];
            return new GameInfo(
                gameInfoDic["lca"].ToString(),
                gameInfoDic["lcq"].ToString(),
                gameInfoDic["top"].ToString(),
                gameInfoDic["right"].ToString(),
                gameInfoDic["bott"].ToString(),
                gameInfoDic["left"].ToString(),
                int.Parse(gameInfoDic["size"].ToString()),
                int.Parse(gameInfoDic["diff"].ToString())
            );
        }
        return null;
    }

    public void LoadGameInfo()
    {
        if(PlayerPrefs.HasKey("gameinfo"))
        {
            String json = PlayerPrefs.GetString("gameinfo");
            Debug.Log(json);
            gameInfo = JsonUtility.FromJson<GameInfo>(json);
            if(!(gameInfo.Size > 0))
            {
                gameInfo = null;
            }
        }
        else
        {
            gameInfo = null;
        }
        //Debug.Log(gameInfo.ToString());
    }

    public void SaveGameInfo()
    {
        string json = JsonUtility.ToJson(gameInfo);
        PlayerPrefs.SetString("gameinfo", json);
    }

    public void ContinueGame()
    {
        gameBoard.InitBoard(gameInfo);
        gameBoard.Open();
        levelList.Close();
    }


    public void StartNewGame(int size, int diff)
    {
        gameInfo = GetRandomGameInfo(size, diff);
        if(gameInfo != null)
        {
            SaveGameInfo();
            gameBoard.InitBoard(gameInfo);
            gameBoard.Open();
            levelList.Close();
        }
        else
        {
            Debug.LogError("Generate new game failed!");
        }
    }

    public void AddGameProgress()
    {
        levelList.AddGameProgress(gameInfo.Size);
    }

    public void ResetGameProgress()
    {
        //levelList.Init(GameProgresses);
        levelList.ResetGameProgress();
    }


}

[Serializable]
public class GameInfo
{
    int[,] lca;
    char[,] lcq;
    string top;
    string left;
    string right;
    string bott;
    int size;
    int diff;
    DateTime start_dt;
    DateTime end_dt;
    int[,] answer;
    int hint;
    

    public GameInfo(string lca_str, string lcq_str, string new_top, string new_right, string new_bott, string new_left, int new_size, int new_diff)
    {
        this.size = new_size;
        this.diff = new_diff;
        this.lca = new int[size, size];
        this.lcq = new char[size, size];
        this.answer = new int[size, size];

        for(int i=0;i<size;i++)
            for(int j=0;j<size;j++)
            {
                if(lca_str.Length > i*size+j)
                    lca[i,j] = int.Parse(lca_str[i*size+j].ToString());
                if(lcq_str.Length > i*size+j)
                    lcq[i,j] = lcq_str[i*size+j];
                    if(lcq[i,j] != '.')
                    {
                        answer[i,j] = int.Parse(lcq_str[i*size+j].ToString());
                    }
            }

        this.top = new_top;
        this.left = new_left;
        this.right = new_right;
        this.bott = new_bott;
        this.start_dt = DateTime.Now;
        this.end_dt = DateTime.MinValue;
        this.hint = 3;
        
        //Debug.Log("LCA_STR: " + lca_str + "\nLCQ_STR" + lcq_str + "\n" + this.ToString());

    }

    public int[,] Lca { get => lca; set => lca = value; }
    public char[,] Lcq { get => lcq; set => lcq = value; }
    public string Top { get => top; set => top = value; }
    public string Left { get => left; set => left = value; }
    public string Right { get => right; set => right = value; }
    public string Bott { get => bott; set => bott = value; }
    public int Size { get => size; set => size = value; }
    public int Diff { get => diff; set => diff = value; }
    public DateTime Start_dt { get => start_dt; set => start_dt = value; }
    public DateTime End_dt { get => end_dt; set => end_dt = value; }
    public int[,] Answer { get => answer; set => answer = value; }
    public int Hint { get => hint; set => hint = value; }

    public bool CheckAnswer()
    {
        for(int i=0;i<Size;i++)
            for(int j=0;j<Size;j++)
            {
                if (Answer[i,j] > 0 && Lca[i,j] != Answer[i,j] && Lca[i,j] > 0){
                    return false;
                }
            }
        return true;
    }

    public bool IsFilled()
    {
        for(int i=0;i<Size;i++)
            for(int j=0;j<Size;j++)
            {
                if (Answer[i,j] == 0){
                    return false;
                }
            }
        return true;
    }

    public override string ToString()
    {
        StringBuilder s = new StringBuilder();
        s.Append("LCA:\n");
        for(int i=0;i<Size;i++)
        {
            for(int j=0;j<Size;j++)
            {
                s.Append(lca[i, j].ToString());
            }
            s.Append("\n");
        }
        s.Append("LCQ:\n");
        for(int i=0;i<Size;i++)
        {
            for(int j=0;j<Size;j++)
            {
                s.Append(lcq[i, j].ToString());
            }
            s.Append("\n");
        }
        s.Append("ANSWER:\n");
        for(int i=0;i<Size;i++)
        {
            for(int j=0;j<Size;j++)
            {
                s.Append(answer[i, j].ToString());
            }
            s.Append("\n");
        }        
        s.Append("Top  : " + top + "\n");
        s.Append("Right: " + right + "\n");
        s.Append("Bott : " + bott + "\n");
        s.Append("Left : " + left + "\n");
        return s.ToString();
    }
}

