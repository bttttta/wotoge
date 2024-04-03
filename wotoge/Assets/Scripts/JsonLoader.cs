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
    public float time; // (最初の)処理すべきタイミング。拍
    public float length; // 伸ばす長さ。拍
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

    public (GameObject[], Note[], Event[]) GetNotes(GameObject parent = null) {
        if (stageData == null) { throw new NullReferenceException(); }
        
        List<GameObject> gameObjects = new List<GameObject>(stageData.Notes.Length);
        List<Note> notes = new List<Note>(stageData.Notes.Length);
        List<Event> events = new List<Event>(stageData.Notes.Length);
        float bpm = 120;
        foreach(NoteData note in stageData.Notes) {
            if (note.IsNote()) {
                GameObject go = new GameObject($"Note_{note.id}");
                switch (note.type) {
                    case "bottom":
                        NoteBottom bottom = go.AddComponent<NoteBottom>();
                        bottom.type = note.type;
                        bottom.beat = note.time;
                        bottom.lane = note.lane;
                        bottom.bpm = bpm;
                        notes.Add(bottom);
                        break;
                    case "tap":
                        NoteTap tap = go.AddComponent<NoteTap>();
                        tap.type = note.type;
                        tap.beat = note.time;
                        tap.pos = new Unity.Mathematics.int2(note.x, note.y);
                        tap.bpm = bpm;
                        notes.Add(tap);
                        break;
                    case "flick":
                        NoteFlick flick = go.AddComponent<NoteFlick>();
                        flick.type = note.type;
                        flick.beat = note.time;
                        flick.pos = new Unity.Mathematics.int2(note.x, note.y);
                        flick.bpm = bpm;
                        flick.angle = note.angle;
                        notes.Add(flick);
                        break;
                    case "long":
                        NoteLong nLong = go.AddComponent<NoteLong>();
                        nLong.type = note.type;
                        nLong.beat = note.time;
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
                // Event
                GameObject go = new GameObject($"Event_{note.id}");
                Event nEvent = go.AddComponent<Event>();
                nEvent.type = note.type;
                nEvent.beat = note.time;
                nEvent.value = note.value;
                nEvent.bpm = bpm;
                switch(note.type) {
                    case "bpm":
                        bpm = note.value;
                        nEvent.bpm = bpm;
                        break;
                    default:
                        break;
                }
            }

        }

        return (gameObjects.ToArray(), notes.ToArray(), events.ToArray());
    }
}
