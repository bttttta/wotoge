using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesManager : SingletonMonoBehaviour<NotesManager> {
    public TextAsset StageJson;

    public GameObject[] NotesObject { get; private set; }
    public Note[] Notes { get; private set; }
    public Event[] Events { get; private set; }
    protected float time = 0;

    // Start is called before the first frame update
    void Start() {
        StageData stageData = JsonLoader.LoadStage(StageJson);
        InstantiateNotes(stageData);
    }

    // Update is called once per frame
    void Update() {

    }

    void InstantiateNotes(StageData stageData) {
        if(stageData == null) { throw new NullReferenceException(); }

        List<GameObject> gameObjects = new List<GameObject>(stageData.Notes.Length);
        List<Note> notes = new List<Note>(stageData.Notes.Length);
        List<Event> events = new List<Event>(stageData.Notes.Length);

        float bpm = 120;
        foreach(NoteData note in stageData.Notes) {
            if(note.IsNote()) {
                GameObject go = new GameObject($"Note_{note.id}");
                switch(note.type) {
                    case "bottom":
                        NoteBottom bottom = go.AddComponent<NoteBottom>();
                        bottom.id = note.id;
                        bottom.beat = note.time;
                        bottom.Lane = note.lane;
                        bottom.bpm = bpm;
                        notes.Add(bottom);
                        break;
                    case "tap":
                        NoteTap tap = go.AddComponent<NoteTap>();
                        tap.id = note.id;
                        tap.beat = note.time;
                        tap.pos = new Unity.Mathematics.int2(note.x, note.y);
                        tap.bpm = bpm;
                        notes.Add(tap);
                        break;
                    case "flick":
                        NoteFlick flick = go.AddComponent<NoteFlick>();
                        flick.id = note.id;
                        flick.beat = note.time;
                        flick.pos = new Unity.Mathematics.int2(note.x, note.y);
                        flick.bpm = bpm;
                        flick.angle = note.angle;
                        notes.Add(flick);
                        break;
                    case "long":
                        NoteLong nLong = go.AddComponent<NoteLong>();
                        nLong.id = note.id;
                        nLong.beat = note.time;
                        nLong.pos = new Unity.Mathematics.int2(note.x, note.y);
                        nLong.length = note.length;
                        nLong.bpm = bpm;
                        notes.Add(nLong);
                        break;
                }
                go.transform.parent = transform;
                gameObjects.Add(go);
            } else {
                // Event
                GameObject go = new GameObject($"Event_{note.id}");
                Event nEvent = go.AddComponent<Event>();
                nEvent.type = note.type;
                nEvent.beat = note.time;
                nEvent.value = note.value;
                nEvent.bpm = bpm;
                switch(note.type) {
                    case "bpm":
                        bpm = note.value;
                        nEvent.bpm = bpm;
                        break;
                    default:
                        break;
                }
            }

        }

        NotesObject = gameObjects.ToArray();
        Notes = notes.ToArray();
        Events = events.ToArray();
    }
}
