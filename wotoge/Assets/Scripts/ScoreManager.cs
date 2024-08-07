using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Judge {
    public int NoteId { get; init; }
    public bool IsRelease { get; init; }
    public JudgeType Type { get; init; }
    public float delta { get; init; }

    public int GetScore() {
        switch (Type) {
            case JudgeType.Just:
                return 100;
            case JudgeType.Near:
                return 80;
            default:
                return 0;
        }
    }

    public float GetCR() {
        switch(Type) {
            case JudgeType.Just:
                return 1;
            case JudgeType.Near:
                return 0.8f;
            default:
                return 0;
        }
    }
}

public class ScoreManager : SingletonMonoBehaviour<ScoreManager> {
    public List<Judge> Judges { get; private set; }
    public int Score { get; private set; }
    private float CR_Score { get; set; }
    public float CR => CR_Score / TotalNotes;
    public int CurrentCombo {  get; private set; }
    public int MaxCombo {  get; private set; }
    public int LeastScore { get; private set; }

    public int TotalNotes { get; private set; }

    private int GetTotalNotes(NotesManager notesManager) {
        // ノーツの合計。Long系は2重カウントする
        int totalNotes = notesManager.Notes.Length;
        totalNotes += notesManager.Notes.Where(x => x.type == NoteType.Long).Count();
        return totalNotes;
    }

    public void Start() {
        Score = 0;
        CR_Score = 0;
        CurrentCombo = 0;
        MaxCombo = 0;
        Judges = new List<Judge>();
        TotalNotes = GetTotalNotes(NotesManager.Instance);
    }

    public void AddJudge(Judge judge) {
        Judges.Add(judge);
        int currentScore = judge.GetScore();
        Score += currentScore;
        CR_Score += judge.GetCR();
        LeastScore = Math.Min(currentScore, LeastScore);
        if(judge.Type is JudgeType.Just or JudgeType.Near) {
            CurrentCombo += 1;
            MaxCombo = Math.Max(MaxCombo, CurrentCombo);
        } else {
            CurrentCombo = 0;
        }
    }
}
