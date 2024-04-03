using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public NotesManager notesManager; // BPMイベント監視用
    public AudioSource MusicPlayer; // 曲開始用

    AudioSource SE_metronome; // メトロノーム音を鳴らす用の Audio Source
    Event[] bpmEvents; // BPM変更イベントのリスト
    float currentBpm; // 現在のBPM
    float startBpm; // 開始時のBPM

    const float intro_beats = 6; // シーン開始から曲開始までの時間。拍

    public float scene_time { get; private set; } // シーンが始まってからの時間。秒
    public float music_time { get; private set; } // 曲が始まってからの時間。秒
    public float music_beat { get; private set; } // 曲が始まってからの時間。拍

    // Start is called before the first frame update
    void Start()
    {
        SE_metronome = GetComponent<AudioSource>();
        bpmEvents = notesManager.Events.Where(e => e.type == "bpm").ToArray();
        startBpm = currentBpm = (bpmEvents.Length > 0) ? (bpmEvents[0].value) : 120;
        scene_time = 0;
        music_time = -intro_beats * BeatLength(currentBpm);
    }

    // Update is called once per frame
    void Update()
    {
        if(music_time < 0 && music_time + Time.deltaTime >= 0) {
            MusicPlayer.Play();
        }
        for(int i = 1; i <= 4;  i++) {
            float introTime = -BeatLength(currentBpm) * i;
            if(music_time < introTime && music_time + Time.deltaTime >= introTime) {
                SE_metronome.Play();
            }
        }
        scene_time += Time.deltaTime;
        music_time += Time.deltaTime;
        music_beat += Time.deltaTime / BeatLength(currentBpm);
    }

    // 1拍が何秒か
    public static float BeatLength(float bpm) {
        return 60 / bpm;
    }

    // 拍から秒に変換する
    public float BeatToTime(float beat) {
        float ret = 0;
        float bpm = startBpm;
        float lastBeat = 0;
        foreach (Event bpmEvent in bpmEvents) {
            if(beat < bpmEvent.beat) { break; }
            ret += BeatLength(bpm) * (bpmEvent.beat - lastBeat);
            lastBeat = bpmEvent.beat;
            bpm = bpmEvent.value;
        }
        ret += BeatLength(bpm) * (beat - lastBeat);
        return ret;
    }
}
