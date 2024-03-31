using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesManager : MonoBehaviour
{
    public TextAsset StageData;

    public GameObject[] NotesObject;
    public Note[] Notes;
    public Event[] Events;
    protected float time = 0;

    // Start is called before the first frame update
    void Start() {
        JsonLoader loader = new JsonLoader();
        loader.LoadStage(StageData);
        (NotesObject, Notes, Events) = loader.GetNotes(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
