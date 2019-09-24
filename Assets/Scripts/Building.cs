using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    [SerializeField]
    private int currentLevel = 0;

    private string answer = "";

    [SerializeField]
    private Image buildingImage;

    [SerializeField]
    private Text levelNumber;

    [SerializeField]
    private Sprite[] baseSprite;

    private Vector3 basePosition;

    private BuildingCoordination coordination;

    private bool changeable;

    public BuildingCoordination Coordination { get => coordination; set => coordination = value; }
    public bool Changeable { get => changeable; set => changeable = value; }

    [SerializeField]
    private Color[] colors;

    [SerializeField]
    private Color[] shadowColors; 

    [SerializeField]
    private int buildingHeight = 5;

    // Start is called before the first frame update
    void Start()
    {
        if(buildingImage == null)
            buildingImage = GetComponentInChildren<Image>();
        if(levelNumber == null)
            levelNumber = GetComponentInChildren<Text>();
        basePosition = buildingImage.rectTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLevelText(string newAnswer)
    {
        if(newAnswer.Equals("0"))
        {
            levelNumber.text = "";
        }
        else {
            levelNumber.text = newAnswer;
        }
    }

    public void SetLevel(int newLevel)
    {
        if (newLevel <= GameManager.Instance.MaxSize)
        {
            this.currentLevel = newLevel;

            //Debug.Log(buildingImage.rectTransform.localPosition);
            //Debug.Log(basePosition);
            //buildingImage.rectTransform.localPosition = new Vector3(basePosition.x, basePosition.y + 10 * currentLevel, basePosition.z);
            buildingImage.rectTransform.localPosition = new Vector3(0, buildingHeight * currentLevel, 0);
            buildingImage.GetComponent<Shadow>().effectDistance= new Vector2(0, -buildingHeight * currentLevel);
            SetLevelText(newLevel.ToString());
            if(currentLevel <= 0)
            {
                gameObject.GetComponent<Image>().sprite = baseSprite[0];
                buildingImage.enabled = false;
            }
            else {
                gameObject.GetComponent<Image>().sprite = baseSprite[1];
                buildingImage.enabled = true;
            }
            if(changeable)
            {
                buildingImage.color = colors[1];
                buildingImage.GetComponent<Shadow>().effectColor = shadowColors[1];
                levelNumber.GetComponent<Shadow>().effectColor = shadowColors[1];
            }
            else
            {
                buildingImage.color = colors[0];
                buildingImage.GetComponent<Shadow>().effectColor = shadowColors[0];
                levelNumber.GetComponent<Shadow>().effectColor = shadowColors[0];
            }
        }
    }

    public void OnBuildingClick()
    {
        if(changeable)
            GameManager.Instance.GameBoard.ShowSelector(this);
        //Selector.Instance.Show();
    }
    
}


public struct BuildingCoordination
{
    int x;
    int y;

    public int X { get => x; set => x = value; }
    public int Y { get => y; set => y = value; }

    public BuildingCoordination(int newX, int newY)
    {
        x = newX;
        y = newY;
    }

}