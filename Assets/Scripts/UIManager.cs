using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

    private Label manaLabel;
    private Label heightLabel;

    private void Awake()
    {
        instance = this;
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        manaLabel = root.Q<Label>("Mana");
        heightLabel = root.Q<Label>("Height");
    }
    private void Start()
    {
        UpdateUI();
    }
    public void UpdateUI()
    {
        manaLabel.text = "Mana: " + PlayerController.instance.manaCount;
        heightLabel.text = "Height: " + LevelController.instance.bottomRow;
    }
}