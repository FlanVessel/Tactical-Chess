using System;

[Serializable]
public class LocalPlayerData
{
    public int PlayerIndex { get; private set; }
    public int SlotIndex { get; private set; }
    public uint InputUserId { get; private set; }
    public string DeviceName { get; private set; }
    public string ControllerScheme { get; private set; }
    public int SelectedSlotIndex { get; private set; } = 0;
    public bool IsReady { get; private set; }
    
    public LocalPlayerData(int playerIndex, uint inputUserId, string deviceName, string controllerScheme)
    {
        PlayerIndex = playerIndex;
        InputUserId = inputUserId;
        DeviceName = deviceName;
        ControllerScheme = controllerScheme;
        
        SlotIndex = -1;
        IsReady = false;
    }

    public void MoveSelectedSlot(int direction, int slotCount)
    {
        if (slotCount <= 0) return;
        
        SelectedSlotIndex += direction;

        if (SelectedSlotIndex < 0)
        {
            SelectedSlotIndex = slotCount - 1;
        }
        else if (SelectedSlotIndex >= slotCount)
        {
            SelectedSlotIndex = 0;
        }
    }

    public void AssignSlot(int slotIndex)
    {
        SlotIndex = slotIndex;
        SelectedSlotIndex = slotIndex;
        IsReady = false;
    }

    public void RealeaseSlot()
    {
        SlotIndex = -1;
        IsReady = false;
    }

    public void SetReady(bool value)
    {
        IsReady = value;
    }
}
