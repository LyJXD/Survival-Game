using UnityEngine.InputSystem;

/// <summary>
/// Hotbar display.
/// 底部玩家物品栏，使用热键实现相关功能。
/// </summary>
public class HotbarDisplay : StaticInventoryDisplay
{
    private int _maxIndexSize = 6;
    private int _currentIndex = 0;

    private PlayerControls _playerControls;

    private void Awake()
    {
        _playerControls = new PlayerControls();
    }

    protected override void Start()
    {
        base.Start();

        _currentIndex = 0;
        _maxIndexSize = slots.Length - 1;

        slots[_currentIndex].ToggleHighlight();
    }

    private void Update()
    {
        // 鼠标滚轮向上滚动
        if (_playerControls.Player.MouseWheel.ReadValue<float>() > 0.1f)
        {
            ChangeIndex(-1);
        }
        // 鼠标滚轮向下滚动
        if (_playerControls.Player.MouseWheel.ReadValue<float>() < -0.1f)
        {
            ChangeIndex(1);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _playerControls.Enable();

        _playerControls.Player.Hotbar1.performed += Hotbar1;
        _playerControls.Player.Hotbar2.performed += Hotbar2;
        _playerControls.Player.Hotbar3.performed += Hotbar3;
        _playerControls.Player.Hotbar4.performed += Hotbar4;
        _playerControls.Player.Hotbar5.performed += Hotbar5;
        _playerControls.Player.Hotbar6.performed += Hotbar6;
        _playerControls.Player.Hotbar7.performed += Hotbar7;
        _playerControls.Player.UseItem.performed += UseItem;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _playerControls.Disable();

        _playerControls.Player.Hotbar1.performed -= Hotbar1;
        _playerControls.Player.Hotbar2.performed -= Hotbar2;
        _playerControls.Player.Hotbar3.performed -= Hotbar3;
        _playerControls.Player.Hotbar4.performed -= Hotbar4;
        _playerControls.Player.Hotbar5.performed -= Hotbar5;
        _playerControls.Player.Hotbar6.performed -= Hotbar6;
        _playerControls.Player.Hotbar7.performed -= Hotbar7;
        _playerControls.Player.UseItem.performed -= UseItem;
    }

    #region Hotbar Select Methods 热键选择方法
    private void Hotbar1(InputAction.CallbackContext obj)
    {
        SetIndex(0);
    }
    private void Hotbar2(InputAction.CallbackContext context)
    {
        SetIndex(1);
    }
    private void Hotbar3(InputAction.CallbackContext context)
    {
        SetIndex(2);
    }
    private void Hotbar4(InputAction.CallbackContext context)
    {
        SetIndex(3);
    }
    private void Hotbar5(InputAction.CallbackContext context)
    {
        SetIndex(4);
    }
    private void Hotbar6(InputAction.CallbackContext context)
    {
        SetIndex(5);
    }
    private void Hotbar7(InputAction.CallbackContext context)
    {
        SetIndex(6);
    }
    #endregion

    private void UseItem(InputAction.CallbackContext context)
    {
        if (slots[_currentIndex].AssignedInventorySlot.ItemData != null)
        {
            slots[_currentIndex].AssignedInventorySlot.ItemData.UseItem();
        }
    }

    private void SetIndex(int newIndex)
    {
        int lastIndex = _currentIndex;
        slots[_currentIndex].ToggleHighlight();
        slots[_currentIndex].DestroyHandheld();

        if (newIndex < 0)
        {
            newIndex = 0;
        }
        if (newIndex > _maxIndexSize)
        {
            newIndex = _maxIndexSize;
        }

        _currentIndex = newIndex;
        slots[_currentIndex].ToggleHighlight();
        slots[_currentIndex].CreateHandheld(slots[_currentIndex].AssignedInventorySlot.ItemData);
        EquipmentManager.Instance.ChangeTool((WeaponItemData)slots[lastIndex].AssignedInventorySlot.ItemData,
            (WeaponItemData)slots[_currentIndex].AssignedInventorySlot.ItemData);
    }

    private void ChangeIndex(int direction)
    {
        int lastIndex = _currentIndex;
        slots[_currentIndex].ToggleHighlight();
        slots[_currentIndex].DestroyHandheld();

        _currentIndex += direction;
        if (_currentIndex < 0)
        {
            _currentIndex = _maxIndexSize;
        }
        if (_currentIndex > _maxIndexSize)
        {
            _currentIndex = 0;
        }

        slots[_currentIndex].ToggleHighlight();
        slots[_currentIndex].CreateHandheld(slots[_currentIndex].AssignedInventorySlot.ItemData);
        EquipmentManager.Instance.ChangeTool(
            (WeaponItemData)slots[_currentIndex].AssignedInventorySlot.ItemData,
            (WeaponItemData)slots[lastIndex].AssignedInventorySlot.ItemData);
    }
}
