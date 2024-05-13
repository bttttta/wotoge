using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class TimingObjectManager : SingletonMonoBehaviour<TimingObjectManager> {
    public ObjectTiming PrefubTimingTap;
    public ObjectTiming PrefubTimingFlick;
    public ObjectTiming PrefubTimingLong;
    public ObjectTiming PrefubTimingSlide;

    public ObjectTiming Instantiate(NoteType type, Vector3 position, Transform parent) {
        ObjectTiming prefub = type switch {
            NoteType.Bottom => null,
            NoteType.Tap => PrefubTimingTap,
            NoteType.Flick => PrefubTimingFlick,
            NoteType.Long => PrefubTimingLong,
            NoteType.Slide => PrefubTimingSlide,
            _ => null,
        };
        ObjectTiming gameObject = Instantiate(prefub, position, Quaternion.identity, parent);
        gameObject.name = $"{parent.name}_Timing";
        return gameObject;
    }

}
