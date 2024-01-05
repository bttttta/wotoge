using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBottom : Note
{
    public int lane; // ノートの出るレーン。0/1/2/3

    GameObject noteObject;
    Transform noteTransform;
    SpriteRenderer spriteRenderer;
    Vector3 position;

    const int bottom_bar_y = 160;
    const int bottom_size = 1080 / 4;
    const int bottom_speed = (1920 - 160) / 4;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        noteObject = CreateNoteGameObject(NoteType.Bottom);
        noteTransform = noteObject.transform;
        spriteRenderer = noteObject.GetComponent<SpriteRenderer>();
        position = new Vector3((float)((0.5 + lane) * bottom_size), 2000, 0);
        noteTransform.position = position;
        noteObject.SetActive(false);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        var delta = DeltaBeat();
        switch(state) {
            case NoteState.NotExisted:
                if(delta <= 4f) {
                    state = NoteState.Appeared;
                    noteObject.SetActive(true);
                }
                break;
            case NoteState.Appeared:
                position.y = DeltaBeat() * bottom_speed + bottom_bar_y;
                noteTransform.position = position;
                if(time - current_time < time_far) {
                    state = NoteState.Ready;
                }
                break;
            case NoteState.Ready:
                position.y = DeltaBeat() * bottom_speed + bottom_bar_y;
                noteTransform.position = position;
                if(current_time - time > time_far) {
                    state = NoteState.Lost;
                }
                break;
            case NoteState.Hit:
                noteTransform.localScale = Vector3.one;
                noteTransform.position = new Vector3(position.x, 120, position.z);
                judge = GetJudgeNow();
                spriteRenderer.sprite = judgeSpriteManager.GetSprite(judge);
                state = NoteState.Judged;
                break;
            case NoteState.Lost:
                noteTransform.localScale = Vector3.one;
                noteTransform.position = new Vector3(position.x, 120, position.z);
                judge = JudgeType.Far;
                spriteRenderer.sprite = judgeSpriteManager.GetSprite(judge);
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
