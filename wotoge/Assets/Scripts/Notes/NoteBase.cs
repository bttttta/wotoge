using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum NoteState
{
    NotExisted, // �܂��\������Ă��Ȃ�
    Appeared, // �\������Ă邯�ǂ܂����肳��Ȃ�
    Ready, // ����\
    Hit, // ���͂��ꂽ�u��
    Lost, // ���������u��
    Judged, // ����\�����Ă�
    Disappeared, // ����������
}

public abstract class Note : MonoBehaviour
{
    public float time; // �o��^�C�~���O�B�b
    public float bpm; // �o��Ƃ���BPM
    protected float current_time = 0f; // ���݂̎���
    public NoteState state = NoteState.NotExisted;
    public JudgeType judge;

    protected NoteSpriteManager noteSpriteManager;
    protected JudgeSpriteManager judgeSpriteManager;

    public float result_time = 0f; // ����\�����ꂽ����
    public const float time_result = 1f; // ����\������鎞��
    public const float time_far = 0.5f; // Far����̎���(���a)
    public const float time_near = 0.2f; // Near����̎���(���a)
    public const float time_just = 0.1f; // Just����̎���(���a)

    // Start is called before the first frame update
    protected virtual void Start()
    {
        noteSpriteManager = GameObject.Find("NoteSpriteManager").GetComponent<NoteSpriteManager>();
        judgeSpriteManager = GameObject.Find("JudgeSpriteManager").GetComponent<JudgeSpriteManager>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        current_time += Time.deltaTime;
    }

    // fingerPath�̎w���m�[�g�ɓ������Ă��邩
    // -1: �������ĂȂ� / 0~: �������Ă�A�����قǗD��A1�ȏ�͊m��
    public abstract float CheckHit(FingerPath fingerPath);

    // fingerPath�Ŋm�肵���Ƃ��̓���
    public void Hit(FingerPath fingerPath) {
        state = NoteState.Hit;

    }

    // time�܂ŉ������邩
    protected float DeltaBeat(float? time = null) {
        float delta = this.time - (time ?? current_time);
        return delta * (bpm / 60);
    }

    // �����肵����ǂ̔���ɂȂ邩 �������݂̂�����
    protected JudgeType GetJudgeNow() {
        if(Mathf.Abs(time - current_time) <= time_just) {
            return JudgeType.Just;
        } else if(Mathf.Abs(time - current_time) <= time_near) {
            return JudgeType.Near;
        } else {
            return JudgeType.Far;
        }
    }

    // �m�[�c�\���p�̎qGameObject���쐬����
    protected GameObject CreateNoteGameObject(NoteType noteType) {
        GameObject result = new GameObject($"{this.name}_Note");
        SpriteRenderer spriteRenderer = result.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = noteSpriteManager.GetNoteSprite(noteType);
        result.transform.parent = this.transform;
        return result;
    }

    // �^�C�~���O�\���p�̎qGameObject���쐬����
    protected GameObject CreateTimingGameObject(NoteType noteType) {
        GameObject result = new GameObject($"{this.name}_Timing");
        SpriteRenderer spriteRenderer = result.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = noteSpriteManager.GetTimingSprite(noteType);
        result.transform.parent = this.transform;
        return result;
    }
}
