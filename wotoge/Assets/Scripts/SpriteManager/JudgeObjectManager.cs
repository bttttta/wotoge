using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum JudgeType {
    None, Just, Near, Far,
}

public class JudgeObjectManager : SingletonMonoBehaviour<JudgeObjectManager> {
    public GameObject PrefubJust;
    public GameObject PrefubNear;
    public GameObject PrefubFar;

    public GameObject Instantiate(JudgeType type, Vector3 position, Transform parent) {
        GameObject prefub = type switch {
            JudgeType.Just => PrefubJust,
            JudgeType.Near => PrefubNear,
            JudgeType.Far => PrefubFar,
            _ => null,
        };
        GameObject gameObject = Instantiate(prefub, position, Quaternion.identity, parent);
        gameObject.name = $"{parent.name}_Judge";
        return gameObject;
    }

}
