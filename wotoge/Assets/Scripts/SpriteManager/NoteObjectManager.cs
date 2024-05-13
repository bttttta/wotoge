using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public enum NoteType {
    Bottom, Tap, Flick, Long, Slide,
}

public class NoteObjectManager : SingletonMonoBehaviour<NoteObjectManager> {
    public ObjectNote PrefubNoteBottom;
    public ObjectNote PrefubNoteTap;
    public ObjectNote PrefubNoteFlick;
    public ObjectNote PrefubNoteLong;
    public ObjectNote PrefubNoteSlide;

    public ObjectNote Instantiate(NoteType type, Vector3 position, Transform parent) {
        ObjectNote prefub = type switch {
            NoteType.Bottom => PrefubNoteBottom,
            NoteType.Tap => PrefubNoteTap,
            NoteType.Flick => PrefubNoteFlick,
            NoteType.Long => PrefubNoteLong,
            NoteType.Slide => PrefubNoteSlide,
            _ => null,
        };
        ObjectNote gameObject = Instantiate(prefub, position, Quaternion.identity, parent);
        gameObject.name = $"{parent.name}_Note";
        return gameObject;
    }

}
