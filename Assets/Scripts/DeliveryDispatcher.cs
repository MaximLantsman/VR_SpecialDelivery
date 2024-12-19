using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DeliveryDispatcher : MonoBehaviour
{
    public int score=0;
    [Space(10)]
    
    [SerializeField, Anywhere] private GameObject player;
    [SerializeField, Anywhere] private DeliveryEndPoint DeliveryPointPrefab;
    [SerializeField, Anywhere] private Transform locationNPC;
    [SerializeField, Anywhere] private AudioSource audioNPC;
    [SerializeField, Anywhere] private Image timerSlider;
    
    [SerializeField, Anywhere] private GameObject npcTalkWindow;

    [SerializeField] private List<DeliveryData> deliveredObjectList;
    [SerializeField] private List<Transform> deliveryLocations;

    [SerializeField] private float timeAfterSuccess = 3f;
    [SerializeField] private float itsGettingLate = 0.4f;


    private float deliveryTimer;
    private GameObject spawnedDelivery;

    private GameObject spawnedNPC;
    private AudioClip deliveryStartNPCSound;
    private AudioClip deliveryLateNPCSound;
    private AudioClip deliveryDoneNPCSound;
    private AudioClip deliveryFailedNPCSound;

    private CancellationTokenSource timerCts; 

    private const int Zero = 0;
    private const int One = 1;
    

    public async void InstantiateNewDelivery()
    {
        npcTalkWindow.SetActive(true);
        
        int randomGift = Random.Range(Zero, deliveredObjectList.Count);
        spawnedDelivery = Instantiate(deliveredObjectList[randomGift].prefab, player.transform.position, Quaternion.identity);

        int randomLocation = Random.Range(Zero, deliveryLocations.Count);
        DeliveryPointPrefab.transform.position = deliveryLocations[randomLocation].position;
        DeliveryPointPrefab.currentPresent = spawnedDelivery;
        DeliveryPointPrefab.OnDeliveryFinished.AddListener(NextDelivery);

        spawnedNPC = Instantiate(deliveredObjectList[randomGift].deliveryNpc, locationNPC.position, Quaternion.identity, locationNPC);
        spawnedNPC.transform.Rotate(Zero,180,Zero);

        //FIX THE NUMBERS- DO SOMETHING ~~ MAYBE NOT LIST BUT VARIABLES FOR LINES
        deliveryStartNPCSound = deliveredObjectList[randomGift].NpcVoiceLines.StartDialogue;
        deliveryLateNPCSound = deliveredObjectList[randomGift].NpcVoiceLines.LateDialogue;
        deliveryFailedNPCSound = deliveredObjectList[randomGift].NpcVoiceLines.FailedDialogue;
        deliveryDoneNPCSound = deliveredObjectList[randomGift].NpcVoiceLines.DoneDialogue;

        audioNPC.resource = deliveryStartNPCSound;
        audioNPC.Play();
        float currAudioTime = deliveryLateNPCSound.length;
        
        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken npcTalksScreenToken = cts.Token;
        
        await AwaitAfterLateWindow(currAudioTime ,npcTalksScreenToken);
        
        timerCts = new CancellationTokenSource();
        CancellationToken token = timerCts.Token;
        
        await DeliveryTimer(deliveredObjectList[randomGift].deliveryTime,token);
    }
    
    private async UniTask DeliveryTimer(float duration,CancellationToken token)
    {
        for (int i = Zero; i < duration; i++)
        {
            token.ThrowIfCancellationRequested();
            await UniTask.Delay(TimeSpan.FromSeconds(One), cancellationToken: token); 
            timerSlider.fillAmount = One - i / duration;

            if (timerSlider.fillAmount == itsGettingLate)
            {
                DeliveryGettingLate();
            }
        }

        DeliveryCompletion(false);
    }

    private async void DeliveryGettingLate()
    {
        Destroy(spawnedDelivery);
        
        
        audioNPC.resource = deliveryLateNPCSound;
        float currAudioTime = deliveryLateNPCSound.length;
        audioNPC.Play();
        
       CancellationTokenSource cts = new CancellationTokenSource();
       CancellationToken token = cts.Token;
        
    await AwaitAfterLateWindow(currAudioTime ,token);
    }
    
    private async UniTask AwaitAfterLateWindow(float duration, CancellationToken token)
    {
        float delay = 3f; //CHANGE LATER IF WORKS WELL TO GLOBAL
        await UniTask.Delay(TimeSpan.FromSeconds(duration + delay), cancellationToken: token);

        npcTalkWindow.gameObject.SetActive(false);
    }
    
    private async void DeliveryCompletion(bool isSuccess)
    {
        Destroy(spawnedDelivery);

        float currAudioTime;
        if (isSuccess)
        {
            audioNPC.resource = deliveryDoneNPCSound;
            currAudioTime = deliveryDoneNPCSound.length;

            score += 10;
        }
        else
        {
            audioNPC.resource = deliveryFailedNPCSound;
            currAudioTime = deliveryFailedNPCSound.length;
            
            score -= 10;
        }
        
        audioNPC.Play();
        
        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;
        
        await AwaitBeforeNextDelivery(currAudioTime ,token);
    }

    private async UniTask AwaitBeforeNextDelivery(float duration, CancellationToken token)
    {
        float delay = 3f; //CHANGE LATER IF WORKS WELL TO GLOBAL
        await UniTask.Delay(TimeSpan.FromSeconds(duration+ delay), cancellationToken: token);

        NextDelivery();
    }

    private void DeliveryCompletionSuccess()
    {
        
    }
    
    private async void NextDelivery()
    {
        timerCts.Cancel();
        
        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;
        
        await UniTask.WhenAny(DeliverySuccess(timeAfterSuccess,token));
    }
    
    private async UniTask DeliverySuccess(float duration,CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: token);
        
        ResetDelivery();

        InstantiateNewDelivery();
    }

    private void ResetDelivery()
    {
        Destroy(spawnedDelivery);
        Destroy(spawnedNPC);
    }




    
    
    
}
