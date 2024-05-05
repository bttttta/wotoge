using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class NoteLong : Note
{
    public float length; // 長押しの時間。拍
    public float release_beat; // 長押しを離すタイミング。拍
    public float release_time; // 長押しを離すタイミング。秒
    public float switch_start_beat; // 長押しを中断したタイミング。拍
    public float switch_start_time; // 長押しを中断したタイミング。秒

    public FingerPath HoldingFinger; // 長押し最中の指

    Transform noteTransform;
    Transform timingTransform;
    SpriteRenderer noteSpriteRenderer;
    SpriteRenderer timingSpriteRenderer;
    Vector3 notePosition;

    public NoteLong(){
        type_str = "long";
        type = NoteType.Long;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        release_beat = beat + length;
        release_time = timeManager.BeatToTime(release_beat);

        noteObject = CreateNoteGameObject(NoteType.Long);
        noteTransform = noteObject.transform;
        noteTransform.position = new Vector3(pos.x, pos.y, 0);
        noteSpriteRenderer = noteObject.GetComponent<SpriteRenderer>();
        noteObject.SetActive(false);
        timingObject = CreateTimingGameObject(NoteType.Long);
        timingTransform = timingObject.transform;
        timingTransform.position = new Vector3(pos.x, pos.y, 0);
        timingTransform.localScale = Vector3.one * 4f;
        timingSpriteRenderer = timingObject.GetComponent<SpriteRenderer>();
        timingObject.SetActive(false);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        var delta = DeltaBeat();
        switch(state) {
            case NoteState.NotExisted:
                if(delta <= 2f) {
                    state = NoteState.Appeared;
                    noteObject.SetActive(true);
                    timingObject.SetActive(true);
                }
                break;
            case NoteState.Appeared:
                timingTransform.localScale = Vector3.one * (delta * 2f);
                if(time - timeManager.music_time < time_far) {
                    state = NoteState.Ready;
                }
                break;
            case NoteState.Ready:
                timingTransform.localScale = Vector3.one * (delta * 2f);
                if(timeManager.music_time - time > time_far) {
                    state = NoteState.Lost;
                }
                break;
            case NoteState.Hit:
                timingObject.SetActive(false);
                judge = GetJudgeNow();
                CreateJudgeGameObject(judge);
                state = NoteState.Hold;
                break;
            case NoteState.Hold:
                // 押したときの判定を時間経過で消す
                result_time += Time.deltaTime;
                if(result_time >= time_result) {
                    judgeObject.SetActive(false);
                }
                // 判定
                if(HoldingFinger.IsActive) {
                    if(timeManager.music_time - release_time > time_near) {
                        Debug.Log($"osippa {timeManager.music_time}/{release_time}/{time_near}");
                        // 押しっぱなしの場合
                        judge = JudgeType.Near;
                        timingObject.SetActive(false);
                        noteObject.SetActive(false);
                        CreateJudgeGameObject(judge);
                        state = NoteState.Judged;
                    }
                } else {
                    // 離した瞬間
                    Debug.Log($"release {timeManager.music_time}/{release_time}/{time_near}");
                    if(Mathf.Abs(timeManager.music_time - release_time) <= time_near) {
                        judge = JudgeType.Just;
                        timingObject.SetActive(false);
                        noteObject.SetActive(false);
                        CreateJudgeGameObject(judge);
                        state = NoteState.Judged;
                    } else {
                        state = NoteState.Switch;
                    }
                }
                break;
            case NoteState.Lost:
                timingObject.SetActive(false);
                judge = JudgeType.Far;
                CreateJudgeGameObject(judge);
                state = NoteState.Switch;
                break;
            case NoteState.Judged:
                result_time += Time.deltaTime;
                if(result_time >= time_result) {
                    state = NoteState.Disappeared;
                }
                break;
            case NoteState.Disappeared:
                break;
            default:
                break;
        }
    }

    public override float CheckHit(FingerPath fingerPath) {
        // 判定可能か
        if(state != NoteState.Ready && state != NoteState.Switch) { return -1; }
        // 距離の計算
        float distance2 = Mathf.Pow(fingerPath.Position.x - pos.x, 2) + Mathf.Pow(fingerPath.Position.y - pos.y, 2);
        float tDistance2 = Mathf.Pow(time - timeManager.music_time, 2);

        if(fingerPath.Down) {
            // 押し判定
            if(distance2 > Mathf.Pow(400, 2)) { return -1; }
            HoldingFinger = fingerPath;
            return 1;
        } else if(fingerPath.Delta != Vector2.zero) {
            // グリッサンド判定
            if(distance2 > Mathf.Pow(100, 2)) { return -1; }
            HoldingFinger = fingerPath;
            return 1;
        } else {
            return -1;
        }
    }
}
