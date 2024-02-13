using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Json���̊e�m�[�c�̃f�[�^
[Serializable]
public class NoteData {
    public int id;
    public string type; // �m�[�cor���߂̎��
    public float value; // ���߂̒l
    public float time; // (�ŏ���)�������ׂ��^�C�~���O
    public float length; // 
    public int x; public int y; // (�ŏ���)���W�BBottom�ȊO
    public int lane; // �o�ꂷ�郌�[���BBottom�̂�
    public float angle; // �p�x�BFlick�̂�

    // �m�[�c���ǂ���(bpm�Ȃǂ�false)
    public bool IsNote() {
        return type == "bottom" || type == "tap" || type == "flick" || type == "long" || type == "slide";
    }
}

// Json�̒��g
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
