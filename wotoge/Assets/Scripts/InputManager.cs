using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerPath
{
    public bool IsActive = false;
    public bool Down = false; // �����n�߂��H
    public Vector2 Position; // ���ʍ��W(���^�b�`�������W)
    public Vector2 Delta = Vector2.zero; // �O�t���[������̈ʑ�����
    public Vector2 StartPosition;
    public int FingerId;
    public Note DeterminedNote = null; // �m��ς݂̃m�[�g
    public Dictionary<Note, float> NominatedNote = new Dictionary<Note, float>(100); // ���̃m�[�g

    public NotesManager NotesManager;

    // �^�b�`���W�����ʍ��W�ɕϊ�
    public Vector2 AdjustPosition(Vector2 position) {
        Vector2 result = new Vector2();
        // �g�嗦�̌v�Z
        float scaleWidth = (float)Screen.width / 1080.0f;
        float scaleHeight = (float)Screen.height / 1920.0f;

        if(scaleWidth < scaleHeight) { // �c�����T�C�Y�͉��ɍ��킹��
            result.x = position.x / scaleWidth;
            float y_delta = (Screen.height - 1920.0f *  scaleWidth) / 2;
            result.y = (position.y - y_delta) / scaleWidth;
        } else { // �������T�C�Y�͏c�ɍ��킹��
            // TODO
        }
        return result;
    }

    public void Activate(Vector2 position, int fingerId, NotesManager notesManager) {
        IsActive = true;
        Down = true;
        Position = AdjustPosition(position);
        StartPosition = this.Position;
        Delta = Vector2.zero;
        FingerId = fingerId;
        DeterminedNote = null;
        NotesManager = notesManager;
    }
    public void MoveTo(Vector2 position) {
        if(position == Position) { return; }
        position = AdjustPosition(position);
        Delta = position - Position;
        Position = position;
    }
    public void Release() {
        IsActive = false;
    }
    
    public void Update() {
        if(!IsActive) { return; }

        if(DeterminedNote == null) {
            // �m�[�g�m�肵�Ă��Ȃ��ꍇ
            // ���m�[�g�̌���
            foreach(var note in NotesManager.Notes) {
                float rate = note.CheckHit(this);
                if(rate >= 0) {
                    NominatedNote.Add(note, rate);
                }
            }
            // �m�[�g�m�蔻��
            float maxRate = -1;
            foreach (var (note, rate) in NominatedNote){
                // 1�ȏ�Ȃ�m��
                if(rate > maxRate && rate >= 1) {
                    DeterminedNote = note;
                    maxRate = rate;
                }
            }
            NominatedNote.Clear();

            // �m�肵���m�[�g�̏���
            if(DeterminedNote != null) {
                DeterminedNote.Hit(this);
                DeterminedNote = null;
            }
        } else {
            
        }
        Down = false;
    }
}

public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update

    public NotesManager NotesManager;

    int touchCount = 0; // fingers�Ŏg�p���Ă��鐔
    FingerPath[] fingers;

    const int maxFinger = 20; // fingers�̍ő吔=�ő哯��������

    void Start()
    {
        fingers = new FingerPath[maxFinger];
        for (int i = 0; i < fingers.Length; i++) {
            fingers[i] = new FingerPath();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // �e�w��Touch�󋵂���fingers���X�V
        for(int i = 0; i < Input.touchCount; i++) {
            Touch touch = Input.GetTouch(i);
            switch (touch.phase) {
                case TouchPhase.Began: // �����ꂽ�Ƃ�
                    // �g���Ă��Ȃ�fingers�v�f�������������ɓ����
                    for(int j = 0; j < maxFinger; j++) {
                        if(fingers[j].IsActive == false) {
                            fingers[j].Activate(touch.position, touch.fingerId, NotesManager);
                            break;
                        }
                    }
                    break;
                case TouchPhase.Moved: // ���������Ƃ�
                    for(int j=0; j < maxFinger; j++) {
                        if(fingers[j].FingerId == touch.fingerId) {
                            fingers[j].MoveTo(touch.position);
                            break;
                        }
                    }
                    break;
                case TouchPhase.Ended: // �������Ƃ�
                case TouchPhase.Canceled:
                    for(int j = 0; j < maxFinger; j++) {
                        if(fingers[j].FingerId == touch.fingerId) {
                            fingers[j].Release();
                            break;
                        }
                    }
                    break;
            }
        }

        foreach(FingerPath finger in fingers) {
            finger.Update();
        }
    }
}
