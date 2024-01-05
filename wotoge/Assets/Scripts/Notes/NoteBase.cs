using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum NoteState
{
    NotExisted, // まだ表示されていない
    Appeared, // 表示されてるけどまだ判定されない
    Ready, // 判定可能
    Hit, // 入力された瞬間
    Lost, // 見逃した瞬間
    Judged, // 判定表示してる
    Disappeared, // もう消えた
}

public abstract class Note : MonoBehaviour
{
    public float time; // 出るタイミング。秒
    public float bpm; // 出るときのBPM
    protected float current_time = 0f; // 現在の時刻
    public NoteState state = NoteState.NotExisted;
    public JudgeType judge;

    protected NoteSpriteManager noteSpriteManager;
    protected JudgeSpriteManager judgeSpriteManager;

    public float result_time = 0f; // 判定表示された時間
    public const float time_result = 1f; // 判定表示される時間
    public const float time_far = 0.5f; // Far判定の時間(半径)
    public const float time_near = 0.2f; // Near判定の時間(半径)
    public const float time_just = 0.1f; // Just判定の時間(半径)

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

    // fingerPathの指がノートに当たっているか
    // -1: 当たってない / 0~: 当たってる、高いほど優先、1以上は確定
    public abstract float CheckHit(FingerPath fingerPath);

    // fingerPathで確定したときの動作
    public void Hit(FingerPath fingerPath) {
        state = NoteState.Hit;

    }

    // timeまで何拍あるか
    protected float DeltaBeat(float? time = null) {
        float delta = this.time - (time ?? current_time);
        return delta * (bpm / 60);
    }

    // 今判定したらどの判定になるか 時刻差のみを見る
    protected JudgeType GetJudgeNow() {
        if(Mathf.Abs(time - current_time) <= time_just) {
            return JudgeType.Just;
        } else if(Mathf.Abs(time - current_time) <= time_near) {
            return JudgeType.Near;
        } else {
            return JudgeType.Far;
        }
    }

    // ノーツ表示用の子GameObjectを作成する
    protected GameObject CreateNoteGameObject(NoteType noteType) {
        GameObject result = new GameObject($"{this.name}_Note");
        SpriteRenderer spriteRenderer = result.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = noteSpriteManager.GetNoteSprite(noteType);
        result.transform.parent = this.transform;
        return result;
    }

    // タイミング表示用の子GameObjectを作成する
    protected GameObject CreateTimingGameObject(NoteType noteType) {
        GameObject result = new GameObject($"{this.name}_Timing");
        SpriteRenderer spriteRenderer = result.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = noteSpriteManager.GetTimingSprite(noteType);
        result.transform.parent = this.transform;
        return result;
    }
}
