using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MouseItemSlot : MonoBehaviour
{
    public Image ItemSprite;
    public TextMeshProUGUI ItemCount;
    public InventorySlot AssignedInventorySlot;

    private Transform _playerTransform;
    private GameObject item;
    [SerializeField] private float dropOffset = 3f;

    private void Awake()
    {
        ItemSprite.color = Color.clear;
        ItemSprite.preserveAspect = true;
        ItemCount.text = "";
        
        _playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        if( _playerTransform == null)
        {
            Debug.Log("Player not found!");
        }
    }

    public void UpdateMouseSlot(InventorySlot invSlot)
    {
        AssignedInventorySlot.AssignItem(invSlot);
        UpdateMouseSlot();
        
    }
    public void UpdateMouseSlot()
    {        
        ItemSprite.sprite = AssignedInventorySlot.ItemData.Icon;
        ItemSprite.color = Color.white;
        ItemCount.text = AssignedInventorySlot.StackSize > 1 ? AssignedInventorySlot.StackSize.ToString() : "";
    }

    private void Update()
    {
        if (AssignedInventorySlot.ItemData != null)
        {
            transform.position = Mouse.current.position.ReadValue();

            // ����Ʒ�Ӳ��ж�������Ϸ����
            if (Mouse.current.leftButton.wasPressedThisFrame && !IsPointerOverUIObject())
            {
                if (AssignedInventorySlot.ItemData.ItemPrefab != null)
                {
                    item = Instantiate(AssignedInventorySlot.ItemData.ItemPrefab, 
                        _playerTransform.position + _playerTransform.forward * dropOffset, Quaternion.identity);
                    item.GetComponent<Animator>().enabled = false;
                    
                }
                if (AssignedInventorySlot.StackSize > 1)
                {
                    AssignedInventorySlot.AddToStack(-1);
                    UpdateMouseSlot();
                }
                else
                {
                    var id = item.GetComponent<UniqueID>().ID;
                    SaveGameManager.data.activeItems.Remove(id);
                    ClearSlot();
                }
            }
        }
    }

    public void ClearSlot()
    {
        AssignedInventorySlot.ClearSlot();
        ItemSprite.sprite = null;
        ItemSprite.color = Color.clear;
        ItemCount.text = "";
    }

    // �ж��Ƿ�����UI
    public static bool IsPointerOverUIObject()
    {
        // �����µĵ���¼�����ȡ�û��������Ļ���꣬Ͷ��������UI�������������еĽ��
        PointerEventData eventDataCurrentPosition = new(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }
}
