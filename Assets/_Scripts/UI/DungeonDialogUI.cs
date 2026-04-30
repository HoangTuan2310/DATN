using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Dungeon entry confirmation popup.
/// Attach to Canvas. Wire up references in Inspector.
///
/// UI hierarchy to build:
///   Canvas
///   ├── DialogPanel (Panel)
///   │   ├── MessageText  (TextMeshProUGUI)
///   │   ├── EnterButton  (Button + child label "Enter")
///   │   └── NoButton     (Button + child label "No")
/// </summary>
public class DungeonDialogUI : MonoBehaviour
{
    [Header("Dialog UI")]
    public GameObject dialogPanel;
    public Button enterButton;
    public Button noButton;
    private Action _onEnter;
    private Action _onCancel;

    private void Awake()
    {
        if (dialogPanel != null) dialogPanel.SetActive(false);
        if (enterButton != null) enterButton.onClick.AddListener(ClickEnter);
        if (noButton    != null) noButton.onClick.AddListener(ClickNo);
    }

    public void Show(Action onEnter, Action onCancel)
    {
        _onEnter  = onEnter;
        _onCancel = onCancel;
        if (dialogPanel != null) dialogPanel.SetActive(true);
    }


    private void ClickEnter()
    {
        if (dialogPanel != null) dialogPanel.SetActive(false);
        _onEnter?.Invoke();
    }

    private void ClickNo()
    {
        if (dialogPanel != null) dialogPanel.SetActive(false);
        _onCancel?.Invoke();
    }
}
