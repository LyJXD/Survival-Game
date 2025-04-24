using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoSingleton<SelectionManager>
{
    public GameObject foresight;
    public GameObject interaction_Info_UI;

    // 射线击中物体 - 选中的物体
    private RaycastHit hit;
    private bool hasObjectOnTarget;
    private GameObject HighlightObject;
    [SerializeField]
    private float highlightDistanceThreshold = 5f;
    public GameObject SelectedObject { get; private set; }
    public EntityStatus SelectedObjectStatus { get; private set; }
    public bool IsTreeSelected{ get; private set; }
    public bool IsMineralSelected{ get; private set; }
    public bool IsCharacterSelected{ get; private set; }

    private void Start()
    {
        hasObjectOnTarget = false;
    }

    private void Update()
    {
        CheckSelection();
        HighLightSelection();
        // 鼠标射线击中目标物体
        if (hasObjectOnTarget)
        {
            var item = SelectedObject.GetComponent<ItemPickUp>();
            var choppableTree = SelectedObject.GetComponent<Tree>();

            // 玩家指向了树
            if (choppableTree)
            {
                IsTreeSelected = true;

            }

            // 玩家指向了矿物

            // 玩家指向了其他生物

            // 可捡起物体
            if (item && PlayerManager.Instance.Player.isObjectCanInteract)
            {
                interaction_Info_UI.SetActive(true);
            }
            else
            {
                interaction_Info_UI.SetActive(false);
            }
        }
        else
        {
            interaction_Info_UI.SetActive(false);
        }
    }

    private void CheckSelection()
    {
        // 创建玩家朝向正前方的射线
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit) && Vector3.Distance(hit.transform.position, PlayerManager.Instance.Player.transform.position) < highlightDistanceThreshold)
        {
            hasObjectOnTarget = true;
            SelectedObject = hit.transform.gameObject;
            Debug.Log(SelectedObject.name);
            try
            {
                SelectedObjectStatus = SelectedObject.GetComponent<Entity>().Status;
            }
            catch(NullReferenceException)
            {
                Debug.Log("Not an entity.");
            }
        }
        else
        {
            hasObjectOnTarget = false;
            SelectedObject = null;
        }
    }

    /// <summary>
    /// 为选中物体添加高光轮廓
    /// </summary>
    private void HighLightSelection()
    {
        // 当被选中物体为空时，不允许出现高光物体
        if(SelectedObject == null)
        {
            if (HighlightObject != null)
            {
                HighlightObject.GetComponent<Outline>().enabled = false;
                HighlightObject = null;
            }
        }

        // 当被选中物体不为空且高光物体非选中物体时
        if (SelectedObject != null && SelectedObject != HighlightObject && SelectedObject.CompareTag("Selectable"))
        {
            // 若高光物体不为空，则先将其高光关闭并重新设定高光物体
            if (HighlightObject != null)
            {
                HighlightObject.GetComponent<Outline>().enabled = false;
                HighlightObject = null;
            }
            HighlightObject = SelectedObject;
            Debug.Log(HighlightObject.name);
            if (HighlightObject.GetComponent<Outline>() != null)
            {
                HighlightObject.GetComponent<Outline>().enabled = true;
            }
            else
            {
                Outline outline = HighlightObject.AddComponent<Outline>();
                outline.enabled = true;
            }
        }
    }

    public void EnableSelection()
    {
         foresight.SetActive(true);
    }

    public void DisableSelection()
    {
        foresight.SetActive(false);
        interaction_Info_UI.SetActive(false);
    }
}
