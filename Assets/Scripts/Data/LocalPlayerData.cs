using System;

[Serializable]
public class LocalPlayerData
{
    public int PlayerIndex { get; private set; }
    public int SlotIndex { get; private set; }
    public uint InputUserId { get; private set; }
    public string DeviceName { get; private set; }
    public string ControllerScheme { get; private set; }
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

    public void AssignSlot(int slotIndex)
    {
        SlotIndex = slotIndex;
        IsReady = false;
    }

    public void ClearSlot()
    {
        SlotIndex = -1;
        IsReady = false;
    }

    public void SetReady(bool value)
    {
        IsReady = value;
    }
}
