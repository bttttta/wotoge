using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerPath
{
    public bool IsActive = false;
    public bool Down = false; // 押し始めた？
    public Vector2 Position; // 譜面座標(≠タッチした座標)
    public Vector2 Delta = Vector2.zero; // 前フレームからの位相差分
    public Vector2 StartPosition;
    public int FingerId;
    public Note DeterminedNote = null; // 確定済みのノート
    public Dictionary<Note, float> NominatedNote = new Dictionary<Note, float>(100); // 候補のノート

    public NotesManager NotesManager;

    // タッチ座標→譜面座標に変換
    public Vector2 AdjustPosition(Vector2 position) {
        Vector2 result = new Vector2();
        // 拡大率の計算
        float scaleWidth = (float)Screen.width / 1080.0f;
        float scaleHeight = (float)Screen.height / 1920.0f;

        if(scaleWidth < scaleHeight) { // 縦長→サイズは横に合わせる
            result.x = position.x / scaleWidth;
            float y_delta = (Screen.height - 1920.0f *  scaleWidth) / 2;
            result.y = (position.y - y_delta) / scaleWidth;
        } else { // 横長→サイズは縦に合わせる
            // TODO
        }
        return result;
    }

    public void Activate(Vector2 position, int fingerId, NotesManager notesManager) {
        IsActive = true;
        Down = true;
        Position = AdjustPosition(position);
        StartPosition = this.Position;
        Delta = Vector2.zero;
        FingerId = fingerId;
        DeterminedNote = null;
        NotesManager = notesManager;
    }
    public void MoveTo(Vector2 position) {
        if(position == Position) { return; }
        position = AdjustPosition(position);
        Delta = position - Position;
        Position = position;
    }
    public void Release() {
        IsActive = false;
    }
    
    public void Update() {
        if(!IsActive) { return; }

        if(DeterminedNote == null) {
            // ノート確定していない場合
            // 候補ノートの検索
            foreach(var note in NotesManager.Notes) {
                float rate = note.CheckHit(this);
                if(rate >= 0) {
                    NominatedNote.Add(note, rate);
                }
            }
            // ノート確定判定
            float maxRate = -1;
            foreach (var (note, rate) in NominatedNote){
                // 1以上なら確定
                if(rate > maxRate && rate >= 1) {
                    DeterminedNote = note;
                    maxRate = rate;
                }
            }
            NominatedNote.Clear();

            // 確定したノートの処理
            if(DeterminedNote != null) {
                DeterminedNote.Hit(this);
                DeterminedNote = null;
            }
        } else {
            
        }
        Down = false;
    }
}

public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update

    public NotesManager NotesManager;

    int touchCount = 0; // fingersで使用している数
    FingerPath[] fingers;

    const int maxFinger = 20; // fingersの最大数=最大同時押し数

    void Start()
    {
        fingers = new FingerPath[maxFinger];
        for (int i = 0; i < fingers.Length; i++) {
            fingers[i] = new FingerPath();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 各指のTouch状況からfingersを更新
        for(int i = 0; i < Input.touchCount; i++) {
            Touch touch = Input.GetTouch(i);
            switch (touch.phase) {
                case TouchPhase.Began: // 押されたとき
                    // 使っていないfingers要素を検索しそこに入れる
                    for(int j = 0; j < maxFinger; j++) {
                        if(fingers[j].IsActive == false) {
                            fingers[j].Activate(touch.position, touch.fingerId, NotesManager);
                            break;
                        }
                    }
                    break;
                case TouchPhase.Moved: // 動かしたとき
                    for(int j=0; j < maxFinger; j++) {
                        if(fingers[j].FingerId == touch.fingerId) {
                            fingers[j].MoveTo(touch.position);
                            break;
                        }
                    }
                    break;
                case TouchPhase.Ended: // 離したとき
                case TouchPhase.Canceled:
                    for(int j = 0; j < maxFinger; j++) {
                        if(fingers[j].FingerId == touch.fingerId) {
                            fingers[j].Release();
                            break;
                        }
                    }
                    break;
            }
        }

        foreach(FingerPath finger in fingers) {
            finger.Update();
        }
    }
}
