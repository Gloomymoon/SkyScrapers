using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace V3
{
    

public class Selector : PanelBase
{
  
    [SerializeField]
    private Button[] buttons;

    [SerializeField]
    private float r = 120f;

    private float angle;

    public void InitButtonPosition(int size)
    {
        for(int i = 0;i < GameManager.Instance.MaxSize;i ++)
        {
            if(i>=size)
            {
                buttons[i].gameObject.SetActive(false);
            }
            else 
            {
                buttons[i].gameObject.SetActive(true);
                angle = Mathf.PI * 2 * i / size; 
                buttons[i].transform.localPosition = new Vector3(r * Mathf.Sin(angle), r * Mathf.Cos(angle), 0);
            }
        }
    }
}

}