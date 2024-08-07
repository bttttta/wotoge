using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class NoteFlick : Note {
    public float angle; // äpìx(degree) 0Ç≈è„ÅA90Ç≈ç∂

    public NoteFlick() {
        type_str = "flick";
        type = NoteType.Flick;
    }

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
        noteObject = CreateNoteGameObject(NoteType.Flick);
        noteObject.EulerAngles = new Vector3(0, 0, angle);
        timingObject = CreateTimingGameObject(NoteType.Flick);
        timingObject.EulerAngles = noteObject.EulerAngles;
    }

    // Update is called once per frame
    protected override void Update() {
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
                timingObject.SetTimingScale(delta);
                if(time - timeManager.music_time < time_far) {
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
                OnJudge(false);
                break;
            case NoteState.Lost:
                noteObject.SetActive(false);
                timingObject.SetActive(false);
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

    public override float CheckHit(FingerPath fingerPath) {
        // îªíËâ¬î\Ç©
        if(state != NoteState.Ready) { return -1; }
        // ãóó£ÇÃåvéZ
        float distance2 = Mathf.Pow(fingerPath.Position.x - pos.x, 2) + Mathf.Pow(fingerPath.Position.y - pos.y, 2);
        float tDistance2 = Mathf.Pow(time - timeManager.music_time, 2);

        if(distance2 > Mathf.Pow(400, 2)) { return -1; }

        if(fingerPath.Delta != Vector2.zero) {
            // äpìxÇÃåvéZ
            // fingerPathÇÃäpìx
            float fingerAngle = Mathf.Atan2(-fingerPath.Delta.x, fingerPath.Delta.y) * Mathf.Rad2Deg;
            if(fingerAngle < 0) { // fingerAngleÇÕ-180~180ìxÅBÇ±ÇÍÇ0~360Ç…ïœä∑
                fingerAngle = 360 + fingerAngle;
            }
            // Ç±ÇÃÉmÅ[ÉcÇÃäpìxÇ∆fingerPathÇÃäpìxÇ∆ÇÃç∑ï™
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
