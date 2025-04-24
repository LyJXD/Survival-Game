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

    // ���߻������� - ѡ�е�����
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
        // ������߻���Ŀ������
        if (hasObjectOnTarget)
        {
            var item = SelectedObject.GetComponent<ItemPickUp>();
            var choppableTree = SelectedObject.GetComponent<Tree>();

            // ���ָ������
            if (choppableTree)
            {
                IsTreeSelected = true;

            }

            // ���ָ���˿���

            // ���ָ������������

            // �ɼ�������
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
        // ������ҳ�����ǰ��������
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
    /// Ϊѡ��������Ӹ߹�����
    /// </summary>
    private void HighLightSelection()
    {
        // ����ѡ������Ϊ��ʱ����������ָ߹�����
        if(SelectedObject == null)
        {
            if (HighlightObject != null)
            {
                HighlightObject.GetComponent<Outline>().enabled = false;
                HighlightObject = null;
            }
        }

        // ����ѡ�����岻Ϊ���Ҹ߹������ѡ������ʱ
        if (SelectedObject != null && SelectedObject != HighlightObject && SelectedObject.CompareTag("Selectable"))
        {
            // ���߹����岻Ϊ�գ����Ƚ���߹�رղ������趨�߹�����
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
