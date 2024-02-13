using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class NoteFlick : Note
{
    public int2 pos; // ノートの位置
    public float angle; // 角度(degree) 0で上、90で左

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
        noteObject = CreateNoteGameObject(NoteType.Flick);
        noteTransform = noteObject.transform;
        noteTransform.position = new Vector3(pos.x, pos.y, 0);
        noteTransform.eulerAngles = new Vector3(0, 0, angle);
        noteSpriteRenderer = noteObject.GetComponent<SpriteRenderer>();
        noteObject.SetActive(false);
        timingObject = CreateTimingGameObject(NoteType.Flick);
        timingTransform = timingObject.transform;
        timingTransform.position = new Vector3(pos.x, pos.y, 0);
        timingTransform.localScale = Vector3.one * 4f;
        timingTransform.eulerAngles = noteTransform.eulerAngles;
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
                timingTransform.localScale = Vector3.one * (Mathf.Abs(delta) * 2f);
                if(current_time - time > time_far) {
                    state = NoteState.Lost;
                }
                break;
            case NoteState.Hit:
                timingObject.SetActive(false);
                noteTransform.localScale = Vector3.one;
                noteTransform.eulerAngles = Vector3.zero;
                judge = GetJudgeNow();
                noteSpriteRenderer.sprite = judgeSpriteManager.GetSprite(judge);
                state = NoteState.Judged;
                break;
            case NoteState.Lost:
                timingObject.SetActive(false);
                noteTransform.localScale = Vector3.one;
                noteTransform.eulerAngles = Vector3.zero;
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

        if(distance2 > Mathf.Pow(400, 2)) { return -1; }

        if(fingerPath.Delta != Vector2.zero) {
            // 角度の計算
            // fingerPathの角度
            float fingerAngle = Mathf.Atan2(-fingerPath.Delta.x, fingerPath.Delta.y) * Mathf.Rad2Deg;
            if(fingerAngle < 0) { // fingerAngleは-180~180度。これを0~360に変換
                fingerAngle = 360 + fingerAngle;
            }
            // このノーツの角度とfingerPathの角度との差分
            float deltaAngle = Mathf.Abs(angle - fingerAngle);
            if(deltaAngle > 180) {
                deltaAngle = 360 - deltaAngle;
            }

            if(deltaAngle < 45) {
                return 1;
            }
        }


        return -1;
    }
}
