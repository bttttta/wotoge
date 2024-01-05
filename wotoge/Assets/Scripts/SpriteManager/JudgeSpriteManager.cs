using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JudgeType
{
    Just, Near, Far,
}

public class JudgeSpriteManager : MonoBehaviour
{
    public Sprite SpriteJust;
    public Sprite SpriteNear;
    public Sprite SpriteFar;

    public Sprite GetSprite(JudgeType type) {
        return type switch {
            JudgeType.Just => SpriteJust,
            JudgeType.Near => SpriteNear,
            JudgeType.Far => SpriteFar,
            _ => null,
        };
    }

}
