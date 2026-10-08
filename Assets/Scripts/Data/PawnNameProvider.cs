using UnityEngine;
using System.Collections.Generic;

public class PawnNameProvider
{
    private readonly PawnNameData _nameData;
    private readonly List<string> _availableNames = new();

    public PawnNameProvider(PawnNameData nameData)
    {
        _nameData = nameData;
        RefillNames();
    }

    public string GetNextName()
    {
        if (_availableNames.Count == 0)
        {
            RefillNames();
        }

        if (_availableNames.Count == 0)
        {
            return "Peón sin nombre";
        }

        int randomIndex = Random.Range(0, _availableNames.Count);
        string selectedName = _availableNames[randomIndex];

        _availableNames.RemoveAt(randomIndex);

        return selectedName;
    }

    private void RefillNames()
    {
        _availableNames.Clear();

        if (_nameData == null) return;

        foreach (string pawnName in _nameData.PawnNames)
        {
            if (string.IsNullOrWhiteSpace(pawnName)) continue;
            if (_availableNames.Contains(pawnName)) continue;

            _availableNames.Add(pawnName);
        }
    }
}
