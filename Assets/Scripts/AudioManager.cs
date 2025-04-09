using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;
    private EventInstance musicEventInstance;
    public static AudioManager instance {get; private set; }
    [SerializeField] private EventReference levelMusic;
    private StudioEventEmitter emitter;

    [SerializeField] private GameObject blackScreen;



    private void Awake()
    {
        if (instance != null) 
        {
            Debug.Log("Found more than one Audio Manager in the scene.");
        }
        instance = this;
        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();
    }

    private void Start()
    {
        emitter = GetComponent<StudioEventEmitter>();
        Debug.Log(SceneManager.GetActiveScene().buildIndex);
        if(SceneManager.GetActiveScene().buildIndex == 2 ||
        SceneManager.GetActiveScene().buildIndex == 4 ||
        SceneManager.GetActiveScene().buildIndex == 6)
        {
            //AudioManager.instance.InitializeMusic(levelMusic);
        }
    }

    void Update()
    {
        if(blackScreen.activeSelf)
        {
            emitter.Stop();
            CleanUp();
        }
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos) 
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateEventInstance(EventReference eventReference) 
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
    }

    public void SetMusicIntensity(float intensity) 
    {
        emitter.SetParameter("Intensity", intensity);
    }

    public void InitializeMusic(EventReference musicEventReference) 
    {
        musicEventInstance = CreateEventInstance(musicEventReference);
        musicEventInstance.start();
    }

    private void CleanUp()
    {
        //stop and release any created instances
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }

        //stop all of the event emitters, because if we don't they may hang around in other scenes
        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
        Debug.Log("Destroyed");
        CleanUp();   
    }
}
