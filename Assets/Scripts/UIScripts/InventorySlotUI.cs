using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] 
    private Image itemSprite;
    [SerializeField] 
    private GameObject _slotHighlight;
    [SerializeField] 
    private TextMeshProUGUI itemCount;
    [SerializeField] 
    private InventorySlot assignedInventorySlot;

    private GameObject item;
    private Button button;

    public InventorySlot AssignedInventorySlot => assignedInventorySlot;
    public InventoryDisplay ParentDisplay {get; private set;}

    private void Awake()
    {
        ClearSlot();

        itemSprite.preserveAspect = true;

        button = GetComponent<Button>();
        button?.onClick.AddListener(OnUISlotClicked);

        ParentDisplay = transform.parent.GetComponent<InventoryDisplay>();
    }

    private void Update()
    {
        if(_slotHighlight.activeInHierarchy && assignedInventorySlot.ItemData != null &&
            PlayerManager.Instance.Player.WeaponHolder.transform.childCount < 1)
        {
            CreateHandheld(assignedInventorySlot.ItemData);
        }
    }

    public void CreateHandheld(InventoryItemData itemData)
    {      
        Player player = PlayerManager.Instance.Player;

        if (_slotHighlight.activeInHierarchy && itemData != null)
        {
            if (itemData.ItemPrefab != null)
            {
                player.Animator.SetBool("Handhold", true);

                item = Instantiate(itemData.ItemPrefab);
                item.GetComponent<ItemPickUp>().IsPickable = false;
                item.GetComponent<ItemPickUp>().rotationSpeed = 0f;
                item.GetComponent<Collider>().enabled = false;
                item.transform.SetParent(player.WeaponHolder.transform, false);

                item.layer = LayerMask.NameToLayer("Hand");
                if (item.transform.childCount != 0)
                {
                    item.transform.Find("Model").gameObject.layer = LayerMask.NameToLayer("Hand");
                }
            }
            else
            {
                player.Animator.SetBool("Handhold", false);
            }
        }
        else
        {
            player.Animator.SetBool("Handhold", false);
        }
    }

    public void DestroyHandheld()
    {
        Destroy(item);
    }

    public void Init(InventorySlot slot)
    {
        assignedInventorySlot = slot;
        UpdateUISlot(slot);
    }

    public void UpdateUISlot(InventorySlot slot)
    {
        if(slot.ItemData != null)
        {
            itemSprite.sprite = slot.ItemData.Icon;
            itemSprite.color = Color.white;

            if(slot.StackSize > 1)
            {
                itemCount.text = slot.StackSize.ToString();
            }
            else
            {
                itemCount.text = "";
            }
        }
        else
        {
            ClearSlot();
        }
    }

    public void UpdateUISlot()
    {
        if(assignedInventorySlot != null)
        {
            UpdateUISlot(assignedInventorySlot);
        }
    }

    public void ClearSlot()
    {
        assignedInventorySlot?.ClearSlot();
        itemSprite.sprite = null;
        itemSprite.color = Color.clear;
        itemCount.text = "";
    }
    
    public void OnUISlotClicked()
    {
        ParentDisplay?.SlotClicked(this);
    }

    /// <summary>
    /// 切换槽位高光选中
    /// </summary>
    public void ToggleHighlight()
    {
        _slotHighlight.SetActive(!_slotHighlight.activeInHierarchy);
    }
}
