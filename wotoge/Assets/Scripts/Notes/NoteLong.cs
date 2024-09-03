using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class NoteLong : Note {
    public float switch_start_beat; // 長押しを中断したタイミング。拍
    public float switch_start_time; // 長押しを中断したタイミング。秒

    public const float switch_limit_beat = 2.5f; // Breakにならない限界の離してからの時間。拍

    public FingerPath HoldingFinger; // 長押し最中の指

    public NoteLong() {
        type_str = "long";
        type = NoteType.Long;
    }

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();

        release_beat = beat + length;
        release_time = timeManager.BeatToTime(release_beat);

        noteObject = CreateNoteGameObject(NoteType.Long);
        timingObject = CreateTimingGameObject(NoteType.Long);
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
        var delta = DeltaBeat();
        var delta_release = DeltaBeat(timeManager.music_time - release_time + time);
        switch(state) {
            case NoteState.NotExisted:
                if(delta <= 2f) {
                    state = NoteState.Appeared;
                    noteObject.SetActive(true);
                    timingObject.SetActive(true);
                }
                break;
            case NoteState.Appeared:
                timingObject.SetTimingScale(delta);
                if(DeltaSecond() < time_far) {
                    state = NoteState.Ready;
                }
                break;
            case NoteState.Ready:
                timingObject.SetTimingScale(delta);
                if(-DeltaSecond() > time_far) {
                    state = NoteState.Lost;
                }
                break;
            case NoteState.Hit:
                OnJudge(false);
                state = NoteState.Hold;
                break;
            case NoteState.Hold:
                timingObject.SetTimingScale(delta_release);
                // 判定
                if(HoldingFinger.IsActive) {
                    if(-DeltaReleaseSecond() > time_near) {
                        Debug.Log($"osippa {timeManager.music_time}/{release_time}/{time_near}");
                        // 押しっぱなしの場合
                        OnJudge(true, JudgeType.Near);
                        timingObject.SetActive(false);
                        noteObject.SetActive(false);
                    }
                } else {
                    // 離した瞬間
                    if(Mathf.Abs(DeltaReleaseSecond()) <= time_near) {
                        Debug.Log($"release {timeManager.music_time}/{release_time}/{time_near}");
                        OnJudge(true, JudgeType.Just);
                        timingObject.SetActive(false);
                        noteObject.SetActive(false);
                    } else {
                        Debug.Log($"switch {timeManager.music_time}/{release_time}/{time_near}");
                        switch_start_time = timeManager.music_time;
                        switch_start_beat = timeManager.music_beat;
                        state = NoteState.Switch;
                    }
                }
                break;
            case NoteState.Switch:
                // 押し直し判定はCheckHitで実施
                if(-DeltaReleaseSecond() > time_near) {
                    // 早Near判定より前に離して、その後押し直さなかった
                    OnJudge(false, JudgeType.Far);
                    timingObject.SetActive(false);
                    noteObject.SetActive(false);
                } else if(timeManager.music_beat - switch_start_beat > switch_limit_beat) {
                    // 押し直さないまま一定時間経過(Break)
                    OnJudge(false, JudgeType.Far);
                    timingObject.SetActive(false);
                    noteObject.SetActive(false);
                } else {
                    timingObject.SetTimingScale(delta_release);
                }
                break;
            case NoteState.Lost:
                timingObject.SetActive(false);
                OnJudge(false, JudgeType.Far);
                state = NoteState.Switch;
                switch_start_time = timeManager.music_time;
                switch_start_beat = timeManager.music_beat;
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
        float tDistance2 = Mathf.Pow(DeltaSecond(), 2);

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
