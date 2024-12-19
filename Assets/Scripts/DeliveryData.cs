using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "DeliveryItems/SpawnManagerScriptableObject", order = 1)]
public class DeliveryData: ScriptableObject
{
    public GameObject prefab;
    public string name;

    public GameObject deliveryNpc;
    
    public NPCDialogOptions NpcVoiceLines;
    
    public float deliveryTime;
}

[System.Serializable]
public class NPCDialogOptions
{
    public AudioClip StartDialogue;
    public AudioClip LateDialogue;
    public AudioClip DoneDialogue;
    public AudioClip FailedDialogue;
}