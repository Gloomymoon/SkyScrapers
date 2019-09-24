using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    

public class LevelList : PanelBase
{
    public GameObject ContinuePanel { get => continuePanel; set => continuePanel = value; }
    public Dictionary<int, LevelItem> LevelItems { get => levelItems; set => levelItems = value; }

    [SerializeField]
    private GameObject continuePanel;

    [SerializeField]
    private Transform levelItemContainer;

    [SerializeField]
    private LevelItem levelItemPrefab;

    private Dictionary<int, LevelItem> levelItems = new Dictionary<int, LevelItem>();

    void Start()
    {
        if(GameManager.Instance.GameInfo != null)
        {
            continuePanel.SetActive(true);
        }
        else
        {
            continuePanel.SetActive(false);
        }
        for(int level=4;level<=7;level++)
        {
            LevelItem li = Instantiate(levelItemPrefab);
            li.Init(level);
            li.transform.SetParent(levelItemContainer, false);
            LevelItems.Add(level, li);
        }
    }

    public override void Open()
    {
        base.Open();
        UpdateLevelList();
    }

    public void UpdateLevelList()
    {
        if(GameManager.Instance.GameInfo != null)
        {
            continuePanel.SetActive(true);
        }
        else
        {
            continuePanel.SetActive(false);
        }
        foreach(LevelItem li in levelItems.Values)
        {
            li.UpdateLevelItem();
        }
    }

    public void StartNewGame(int size)
    {
        GameManager.Instance.StartNewGame(size, 1);
    }

    public void AddGameProgress(int level)
    {
        LevelItems[level].AddProgress();
        if(level < 7 && LevelItems[level].GameProgress.Progress >= LevelItems[level].EasyCount)
        {
            LevelItems[level + 1].GameProgress.Enabled = true;
        }
    }

    public void ResetGameProgress()
    {
        foreach(KeyValuePair<int, LevelItem> kv in LevelItems)
        {
            kv.Value.GameProgress.Progress = 0;
            kv.Value.GameProgress.Save();
            kv.Value.UpdateLevelItem();
        }
    }
    
}
