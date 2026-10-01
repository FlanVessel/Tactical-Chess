using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainSelection : MonoBehaviour
{
    [SerializeField] private Button selectionButton;

    private void Start()
    {
        SelectedButtons();
    }

    private void SelectedButtons()
    {
        EventSystem.current.SetSelectedGameObject(selectionButton.gameObject);
    }
}
