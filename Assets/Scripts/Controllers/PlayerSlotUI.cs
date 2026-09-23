using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerSlotUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private TMP_Text playerNumberText;
    [SerializeField] private TMP_Text deviceNameText;
    [SerializeField] private TMP_Text statusText;

    [SerializeField] private Image deviceIcon;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image selectionBorder;

    [Header("Colores")]
    [SerializeField] private Color availableColor = Color.gray;
    [SerializeField] private Color occupiedColor = Color.white;
    [SerializeField] private Color readyColor = Color.green;

    private int _slotIndex;

    public int SlotIndex => _slotIndex;

    public void Initialize(int slotIndex)
    {
        _slotIndex = slotIndex;

        if (playerNumberText != null) playerNumberText.text = $"JUGADOR {_slotIndex + 1}";

        ShowAvailable();
        ShowSelection(false, Color.white);
    }

    public void ShowAvailable()
    {
        if (deviceNameText != null) deviceNameText.text = "Sin dispositivo";
        if (statusText != null) statusText.text = "DISPONIBLE";
        if (deviceIcon != null)
        {
            deviceIcon.sprite = null;
            deviceIcon.enabled = false;
        }
        if (backgroundImage != null) backgroundImage.color = availableColor;
    }

    public void ShowOccupied(LocalPlayerData playerData, Sprite icon = null)
    {
        if (playerData == null) return;
        if (deviceNameText != null) deviceNameText.text = playerData.DeviceName;
        if (statusText != null) statusText.text = "PRESIONA CONFIRMAR";

        SetDeviceIcon(icon);

        if (backgroundImage != null) backgroundImage.color = occupiedColor;
    }

    public void ShowReady(LocalPlayerData playerData, Sprite icon = null)
    {
        if (playerData == null) return;
        if (deviceNameText != null) deviceNameText.text = playerData.DeviceName;
        if (statusText != null) statusText.text = "LISTO";

        SetDeviceIcon(icon);

        if (backgroundImage != null) backgroundImage.color = readyColor;

        ShowSelection(false, Color.white);
    }

    public void ShowSelection(bool visible, Color color)
    {
        if (selectionBorder == null) return;

        selectionBorder.enabled = visible;

        if (visible) selectionBorder.color = color;
    }

    private void SetDeviceIcon(Sprite icon)
    {
        if (deviceIcon == null) return;

        deviceIcon.sprite = icon;
        deviceIcon.enabled = icon != null;
    }
}
