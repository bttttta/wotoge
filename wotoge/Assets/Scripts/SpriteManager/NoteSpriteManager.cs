using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public enum NoteType
{
    Bottom, Tap, Flick, Long, Slide,
}

public class NoteSpriteManager : MonoBehaviour
{
    public Sprite SpriteBottom;
    public Sprite SpriteTap;
    public Sprite SpriteFlick;
    public Sprite SpriteLong;
    public Sprite SpriteSlide;

    public Sprite SpriteTapTiming;
    public Sprite SpriteFlickTiming;
    public Sprite SpriteLongTiming;
    public Sprite SpriteSlideTiming;

    public Sprite GetNoteSprite(NoteType type) {
        return type switch {
            NoteType.Bottom => SpriteBottom,
            NoteType.Tap => SpriteTap,
            NoteType.Flick => SpriteFlick,
            NoteType.Long => SpriteLong,
            NoteType.Slide => SpriteSlide,
            _ => null,
        };
    }

    public Sprite GetTimingSprite(NoteType type) {
        return type switch {
            NoteType.Bottom => null,
            NoteType.Tap => SpriteTapTiming,
            NoteType.Flick => SpriteFlickTiming,
            NoteType.Long => SpriteLongTiming,
            NoteType.Slide => SpriteSlideTiming,
            _ => null,
        };
    }
}
