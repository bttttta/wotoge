using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Json内の各ノーツのデータ
[Serializable]
public class NoteData {
    public int id;
    public string type; // ノーツor命令の種類
    public float value; // 命令の値
    public float time; // (最初の)処理すべきタイミング
    public float length; // 
    public int x; public int y; // (最初の)座標。Bottom以外
    public int lane; // 登場するレーン。Bottomのみ
    public float angle; // 角度。Flickのみ

    // ノーツかどうか(bpmなどはfalse)
    public bool IsNote() {
        return type == "bottom" || type == "tap" || type == "flick" || type == "long" || type == "slide";
    }
}

// Jsonの中身
[Serializable]
public class StageData {
    public NoteData[] Notes;
}

public class JsonLoader
{
    StageData stageData;

    public void LoadStage(TextAsset asset) {
        string json = asset.text;
        stageData = JsonUtility.FromJson<StageData>(json);
    }

    public (GameObject[], Note[]) GetNotes(GameObject parent = null) {
        if (stageData == null) { throw new NullReferenceException(); }
        
        List<GameObject> gameObjects = new List<GameObject>(stageData.Notes.Length);
        List<Note> notes = new List<Note>(stageData.Notes.Length);
        float bpm = 120;
        foreach(NoteData note in stageData.Notes) {
            if (note.IsNote()) {
                GameObject go = new GameObject($"Note_{note.id}");
                switch (note.type) {
                    case "bottom":
                        NoteBottom bottom = go.AddComponent<NoteBottom>();
                        bottom.time = note.time;
                        bottom.lane = note.lane;
                        bottom.bpm = bpm;
                        notes.Add(bottom);
                        break;
                    case "tap":
                        NoteTap tap = go.AddComponent<NoteTap>();
                        tap.time = note.time;
                        tap.pos = new Unity.Mathematics.int2(note.x, note.y);
                        tap.bpm = bpm;
                        notes.Add(tap);
                        break;
                    case "flick":
                        NoteFlick flick = go.AddComponent<NoteFlick>();
                        flick.time = note.time;
                        flick.pos = new Unity.Mathematics.int2(note.x, note.y);
                        flick.bpm = bpm;
                        flick.angle = note.angle;
                        notes.Add(flick);
                        break;
                    case "long":
                        NoteLong nLong = go.AddComponent<NoteLong>();
                        nLong.time = note.time;
                        nLong.pos = new Unity.Mathematics.int2(note.x, note.y);
                        nLong.length = note.length;
                        nLong.bpm = bpm;
                        notes.Add(nLong);
                        break;
                }
                if(parent != null) {
                    go.transform.parent = parent.transform;
                }
                gameObjects.Add(go);
            } else {
                switch(note.type) {
                    case "bpm":
                        bpm = note.value; break;
                    default:
                        break;
                }
            }

        }

        return (gameObjects.ToArray(), notes.ToArray());
    }
}
