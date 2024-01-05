using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesManager : MonoBehaviour
{
    public TextAsset StageData;

    public GameObject[] NotesObject;
    public Note[] Notes;
    protected float time = 0;

    // Start is called before the first frame update
    void Start() {
        JsonLoader loader = new JsonLoader();
        loader.LoadStage(StageData);
        (NotesObject, Notes) = loader.GetNotes(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
