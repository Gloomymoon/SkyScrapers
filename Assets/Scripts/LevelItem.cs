using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using V3;

public class LevelItem : MonoBehaviour
{
    private GameProgress gameProgress;

    [SerializeField]
    private Image levelFill;

    [SerializeField]
    private Text fillText;

    [SerializeField]
    private Text levelText;
    
    [SerializeField]
    private Text buttonText;

    [SerializeField]
    private Button button;

    [SerializeField]
    private int maxProgress = 100;

    [SerializeField]
    private int easyCount = 75;

    [SerializeField]
    private int mediumCount = 90;

    public GameProgress GameProgress { get => gameProgress; }

    public string Diff_desc
    {
        get {
            if(GameProgress.Progress >= mediumCount)
                return "HARD";
            else if(GameProgress.Progress >= EasyCount)
                return "MEDIUM";
            else
                return "EASY";
        }
    }

    public int EasyCount { get => easyCount; set => easyCount = value; }

    public void Init(int level)
    {
        gameProgress = new GameProgress(level);
        gameProgress.Load();
        UpdateLevelItem();
    }
    
    public void UpdateLevelItem()
    {
        levelText.text = gameProgress.Level.ToString() + "x" + gameProgress.Level.ToString();
        levelFill.fillAmount = gameProgress.Progress / maxProgress;
        fillText.text = gameProgress.Progress.ToString() + " / " + maxProgress.ToString();
        if(gameProgress.Level >= 7 && !gameProgress.Enabled)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            if(gameProgress.Enabled)
            {
                button.interactable = true;
                buttonText.text = "GO";
            }
            else
            {
                button.interactable = false;
                buttonText.text = "LOCK";
            }
        }
    }

    public void StartNewGame()
    {
        int diff = 1;
        if(gameProgress.Progress>= mediumCount)
            diff = 3;
        else if(gameProgress.Progress >= EasyCount)
            diff = 2;
        GameManager.Instance.StartNewGame(gameProgress.Level, diff);
        Debug.Log("Start new game: size=" + gameProgress.Level.ToString() + " diff="+ diff.ToString());
    }

    public void AddProgress()
    {
        gameProgress.Progress += 1;
        if(gameProgress.Progress > maxProgress)
        {
            gameProgress.Progress = maxProgress;
        }
        gameProgress.Save();
        UpdateLevelItem();
    }
}


[Serializable]
public class GameProgress
{
    int level;
    int progress;
    bool enabled;

    public GameProgress(int level)
    {
        this.level = level;
        progress = 0;
        if(level > 6)
            enabled = false;
        else
            enabled = true;
    }

    public GameProgress(int level, int progress, bool enabled)
    {
        this.level = level;
        this.progress = progress;
        this.enabled = enabled;
    }

    public int Level { get => level;}
    public int Progress { get => progress; set => progress = value; }
    public bool Enabled { get => enabled; set => enabled = value; }

    protected string Key {get => "lvl" + level.ToString(); }

    public void Load()
    {
        if(!PlayerPrefs.HasKey(Key))
        {
            String json = PlayerPrefs.GetString(Key);
            CopyFrom(JsonUtility.FromJson<GameProgress>(json));
        }
        else
        {
            Save();
        }
    }

    public void CopyFrom(GameProgress gp)
    {
        this.level = gp.Level;
        this.progress = gp.Progress;
        this.enabled = gp.Enabled;
    } 

    public void Save()
    {
        PlayerPrefs.SetString(Key, JsonUtility.ToJson(this));
    }

    public void Reset()
    {
        progress = 0;
        Save();        
    }
}