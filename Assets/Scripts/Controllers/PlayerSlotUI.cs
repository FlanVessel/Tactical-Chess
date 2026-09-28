using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerSlotUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private TMP_Text playerNumberText;
    [SerializeField] private TMP_Text deviceNameText;
    [SerializeField] private TMP_Text statusText;
    
    [Header("Imagenes de referencia")]
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
        deviceNameText.text = "Sin dispositivo";
        statusText.text = "DISPONIBLE";
        
        deviceIcon.sprite = null;
        deviceIcon.enabled = false;
        
        backgroundImage.color = availableColor;
    }

    public void ShowOccupied(LocalPlayerData playerData, Sprite icon)
    {
        if (playerData == null) return;
        deviceNameText.text = playerData.DeviceName;
        statusText.text = "PRESIONA CONFIRMAR";
        
        deviceIcon.sprite = icon;
        deviceIcon.enabled = icon  != null;

        SetDeviceIcon(icon);

        backgroundImage.color = occupiedColor;
    }

    public void ShowReady(LocalPlayerData playerData, Sprite icon)
    {
        if (playerData == null) return;
        deviceNameText.text = playerData.DeviceName;
        statusText.text = "LISTO";
        
        deviceIcon.sprite = icon;
        deviceIcon.enabled = icon != null;

        SetDeviceIcon(icon);

        backgroundImage.color = readyColor;

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
