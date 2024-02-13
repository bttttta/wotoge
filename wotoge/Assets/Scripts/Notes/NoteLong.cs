using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class NoteLong : Note
{
    public int2 pos; // ノートの位置。
    public float length; // 長押しの時間

    public FingerPath HoldingFinger; // 長押し最中の指

    GameObject noteObject;
    GameObject timingObject;
    Transform noteTransform;
    Transform timingTransform;
    SpriteRenderer noteSpriteRenderer;
    SpriteRenderer timingSpriteRenderer;
    Vector3 notePosition;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
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
                if(time - current_time < time_far) {
                    state = NoteState.Ready;
                }
                break;
            case NoteState.Ready:
                timingTransform.localScale = Vector3.one * (delta * 2f);
                if(current_time - time > time_far) {
                    state = NoteState.Lost;
                }
                break;
            case NoteState.Hit:
                timingObject.SetActive(false);
                noteTransform.localScale = Vector3.one;
                judge = GetJudgeNow();
                noteSpriteRenderer.sprite = judgeSpriteManager.GetSprite(judge);
                state = NoteState.Hold;
                break;
            case NoteState.Hold:

                break;
            case NoteState.Lost:
                timingObject.SetActive(false);
                noteTransform.localScale = Vector3.one;
                judge = JudgeType.Far;
                noteSpriteRenderer.sprite = judgeSpriteManager.GetSprite(judge);
                state = NoteState.Judged;
                break;
            case NoteState.Judged:
                result_time += Time.deltaTime;
                if(result_time >= time_result) {
                    noteObject.SetActive(false);
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
        float distance2 = Mathf.Pow(fingerPath.Position.x - pos.x, 2) + Mathf.Pow(fingerPath.Position.y - pos.y, 2);
        float tDistance2 = Mathf.Pow(time - current_time, 2);

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
