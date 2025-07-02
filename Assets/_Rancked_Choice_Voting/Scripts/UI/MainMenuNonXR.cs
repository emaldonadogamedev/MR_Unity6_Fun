using System;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainMenuNonXR : MonoBehaviour
{
    [SerializeField]
    private UIDocument mainMenuNonXR;

    private Button startSimulationButton;

    void Start()
    {
        var root = mainMenuNonXR.rootVisualElement;
        root.dataSource = RankedChoicedVotingSimManager.Instance;

        startSimulationButton = root.Q<Button>("startSimulation");
        startSimulationButton.clicked += StartSimulationButton_clicked;
    }

    private void StartSimulationButton_clicked()
    {
        RankedChoicedVotingSimManager.Instance.StartSimulation();
    }
}
