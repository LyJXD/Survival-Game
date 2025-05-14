using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyTabGroup : MonoBehaviour
{
    public List<MyTabButton> tabButtons;

    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;
    public MyTabButton tabSelected;
    public List<GameObject> pagesToSwap;

    public void Subscribe(MyTabButton button)
    {
        if(tabButtons == null)
        {
            tabButtons = new List<MyTabButton>();
        }

        tabButtons.Add(button);
    }

    public void OnTabEnter(MyTabButton button)
    {
        ResetTabs();
        // 未选中标签或即将选择标签非当前选择标签
        if (tabSelected == null || button!=tabSelected)
        {
            button.background.sprite = tabHover;
        }
    }

    public void OnTabExit(MyTabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(MyTabButton button)
    {
        if(tabSelected != null)
        {
            tabSelected.Deselect();
        }
        tabSelected = button;
        tabSelected.Select();

        ResetTabs();
        button.background.sprite = tabActive;
        int index = button.transform.GetSiblingIndex(); // 获取同级索引
        for(int i = 0; i < pagesToSwap.Count; i++)
        {
            if (i == index)
            {
                pagesToSwap[i].SetActive(true);
            }
            else
            {
                pagesToSwap[i].SetActive(false);
            }
        }
    }

    public void ResetTabs()
    {
        foreach(MyTabButton button in tabButtons)
        {
            if(tabSelected != null&& button == tabSelected)
            {
                continue;
            }
            button.background.sprite = tabIdle;
        }
    }

}
