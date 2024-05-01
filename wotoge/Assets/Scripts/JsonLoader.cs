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

public static class JsonLoader {
    public static StageData LoadStage(TextAsset asset) {
        string json = asset.text;
        StageData stageData = JsonUtility.FromJson<StageData>(json);
        return stageData;
    }
}
