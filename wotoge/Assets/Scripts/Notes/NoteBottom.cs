using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class NoteBottom : Note {
    private int lane; // ノートの出るレーン。0/1/2/3
    public int Lane {
        get { return lane; }
        set {
            lane = value;
            pos.x = (int)((0.5 + lane) * bottom_size);
        }
    }

    const int bottom_bar_y = 160;
    const int bottom_size = 1080 / 4;
    const int bottom_speed = (1920 - 160) / 4;

    public NoteBottom() {
        type_str = "bottom";
        type = NoteType.Bottom;
        pos = new int2(0, 2000);
    }

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
        noteObject = CreateNoteGameObject(NoteType.Bottom);
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
        var delta = DeltaBeat();
        SetPosY();
        switch(state) {
            case NoteState.NotExisted:
                if(delta <= 4f) {
                    state = NoteState.Appeared;
                    noteObject.SetActive(true);
                }
                break;
            case NoteState.Appeared:
                noteObject.Position = new Vector3(pos.x, pos.y, 0);
                if(DeltaSecond() < time_far) {
                    state = NoteState.Ready;
                }
                break;
            case NoteState.Ready:
                noteObject.Position = new Vector3(pos.x, pos.y, 0);
                if(-DeltaSecond() > time_far) {
                    state = NoteState.Lost;
                }
                break;
            case NoteState.Hit:
                noteObject.SetActive(false);
                OnJudge(false);
                break;
            case NoteState.Lost:
                noteObject.SetActive(false);
                OnJudge(false, JudgeType.Far);
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

    void SetPosY() {
        // pos.yを設定
        if(state == NoteState.Appeared || state == NoteState.Ready) {
            pos.y = (int)(DeltaBeat() * bottom_speed + bottom_bar_y);
        } else {
            pos.y = 120;
        }
    }

    public override float CheckHit(FingerPath fingerPath) {
        // 判定可能か
        if(state != NoteState.Ready) { return -1; }
        // Bottomは押した瞬間のみ判定
        if(fingerPath.Down == false) { return -1; }
        // Bottomは位置があってれば確定
        if(lane * bottom_size <= fingerPath.Position.x && fingerPath.Position.x < (lane + 1) * bottom_size && fingerPath.Position.y < 300) {
            return 1;
        } else {
            return -1;
        }
    }
}
