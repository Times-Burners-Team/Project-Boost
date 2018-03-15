using UnityEngine;
using UnityEngine.SceneManagement;

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

    bool collisionDisabled = true;

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
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        mainEngineParticles.Stop();
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("LoadLastLevel", 2f);
    }

    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextScenceIndex = currentSceneIndex + 1;
        if (nextScenceIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextScenceIndex = 0; // loop back to start
        }
        SceneManager.LoadScene(nextScenceIndex);
    }

    private void LoadLastLevel()
    {   
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int LastScenceIndex = currentSceneIndex - 1; 
        if (LastScenceIndex <= 0)
        {
            LastScenceIndex = 0;
        }
        SceneManager.LoadScene(LastScenceIndex);
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
