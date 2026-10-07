using System;

public enum LocalDeviceType {None, KeyboardMouse, Xbox, Playstation, GenericGamepad}

[Serializable]
public class LocalPlayerData
{
    public int PlayerIndex { get; private set; }
    public int SlotIndex { get; private set; }
    public uint InputUserId { get; private set; }
    public string DeviceName { get; private set; }
    public string ControllerScheme { get; private set; }
    public bool IsReady { get; private set; }
    public LocalDeviceType DeviceType { get; private set; }
    public PawnCandidateData RecruitedPawn { get; private set; }
    
    public LocalPlayerData(int playerIndex, uint inputUserId, string deviceName, string controllerScheme, LocalDeviceType deviceType)
    {
        PlayerIndex = playerIndex;
        InputUserId = inputUserId;
        DeviceName = deviceName;
        ControllerScheme = controllerScheme;
        DeviceType = deviceType;
        
        SlotIndex = -1;
        IsReady = false;
    }

    public void AssignSlot(int slotIndex)
    {
        SlotIndex = slotIndex;
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
    
    public void RecruitPawn(PawnCandidateData pawn)
    {
        if (pawn == null) return;
        RecruitedPawn = pawn;
    }
    
    public void ClearRecruitedPawn()
    {
        RecruitedPawn = null;
    }
}
