using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CandidateData
{
    public string candidateName = string.Empty;
    public Color candidateColor;
    public bool isEliminated = false;

    public IReadOnlyList<CandidateData> SampleCandidateData => sampleCandidateData;

    private readonly static CandidateData[] sampleCandidateData = new CandidateData[10]
    {
        new()
        {
            candidateName = "Orange",
            candidateColor = Color.orange
        },
        new()
        {
            candidateName = "Alice Blue",
            candidateColor = Color.aliceBlue
        },
        new()
        {
            candidateName = "Violet Red",
            candidateColor = Color.mediumVioletRed
        },
        new()
        {
            candidateName = "Brown",
            candidateColor = Color.saddleBrown
        },
        new()
        {
            candidateName = "Yellow",
            candidateColor = Color.yellow
        },
        new()
        {
            candidateName = "Green",
            candidateColor = Color.green
        },
        new()
        {
            candidateName = "Wheat",
            candidateColor = Color.wheat
        },
        new()
        {
            candidateName = "Turquoise",
            candidateColor = Color.turquoise
        },
        new()
        {
            candidateName = "White",
            candidateColor = Color.whiteSmoke
        },
        new()
        {
            candidateName = "Rose",
            candidateColor = Color.mistyRose
        }
    };
}