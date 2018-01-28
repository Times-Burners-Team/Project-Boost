using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Rocket : MonoBehaviour {

	[SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;
    Rigidbody rigidBody;
	AudioSource audioSource;

    enum State { Alive, Dying, Transcending };
    State state = State.Alive;

    bool collisionDisabled = false;

	// Use this for initialization
	void Start () 
    {
		rigidBody = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {	
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        if (Debug.isDebugBuild)
        {
        RespondToDebugKeys();
        }

    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionDisabled = !collisionDisabled; // toogle
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || !collisionDisabled) { return; }
        switch (collision.gameObject.tag)
        {
            case "Friendly":
            // do nothing
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        mainEngineParticles.Stop();
        successParticles.Play();
        Invoke("LoadNextScene", levelLoadDelay);
		if (SceneManager.GetActiveScene ().buildIndex == LevelManager.countUnlockedLevel) {
			LevelManager.countUnlockedLevel++;
		}
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("LoadFirstLevel", 2f);
    }

    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextScenceIndex = 0;
        if (nextScenceIndex == SceneManager.sceneCountInBuildSettings)
		{
            nextScenceIndex = 1;
			// loop back to start
        }
        SceneManager.LoadScene(nextScenceIndex);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(1); 
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))    // can thrust while rotating
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    private void RespondToRotateInput()
    {
		rigidBody.freezeRotation = true;    // take manual control of rotation

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
		rigidBody.freezeRotation = false;   // resume physics control of rotation
    }
}
