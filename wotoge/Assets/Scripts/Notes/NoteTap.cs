using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Android;

public class NoteTap : Note
{
    Transform noteTransform;
    SpriteRenderer noteSpriteRenderer;
    Vector3 notePosition;

    public NoteTap(){
        type_str = "tap";
        type = NoteType.Tap;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        noteObject = CreateNoteGameObject(NoteType.Tap);
        noteTransform = noteObject.transform;
        noteTransform.position = new Vector3(pos.x, pos.y, 0);
        noteSpriteRenderer = noteObject.GetComponent<SpriteRenderer>();
        noteObject.SetActive(false);
        timingObject = CreateTimingGameObject(NoteType.Tap);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        var delta = DeltaBeat();
        switch(state) {
            case NoteState.NotExisted:
                if (delta <= 2f) {
                    state = NoteState.Appeared;
                    noteObject.SetActive(true);
                    timingObject.SetActive(true);
                }
                break;
            case NoteState.Appeared:
                timingObject.SetTimingScale(delta);
                if (time - timeManager.music_time < time_far) {
                    state = NoteState.Ready;
                }
                break;
            case NoteState.Ready:
                timingObject.SetTimingScale(delta);
                if(timeManager.music_time - time > time_far) {
                    state = NoteState.Lost;
                }
                break;
            case NoteState.Hit:
                noteObject.SetActive(false);
                timingObject.SetActive(false);
                judge = GetJudgeNow();
                CreateJudgeGameObject(judge);
                state = NoteState.Judged;
                break;
            case NoteState.Lost:
                noteObject.SetActive(false);
                timingObject.SetActive(false);
                judge = JudgeType.Far;
                CreateJudgeGameObject(judge);
                state = NoteState.Judged;
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
        if(state != NoteState.Ready) { return -1; }
        // 距離の計算
        float distance2 = Mathf.Pow(fingerPath.Position.x - pos.x, 2)  + Mathf.Pow(fingerPath.Position.y - pos.y, 2);
        float tDistance2 = Mathf.Pow(time - timeManager.music_time, 2);

        if(fingerPath.Down) {
            // 押し判定
            if(distance2 > Mathf.Pow(400, 2)) { return -1; }
            return 1;
        } else if(fingerPath.Delta != Vector2.zero) {
            // グリッサンド判定
            if(distance2 > Mathf.Pow(100, 2)) { return -1; }
            return 1;
        } else {
            return -1;
        }
    }
}
