using UnityEngine;

/// <summary>
/// This player controller class will update the events from the vehicle player.
/// Standar coding documentarion can be found in 
/// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments
/// </summary>
public class PlayerController : MonoBehaviour
{


    // <value> This is the speed of the vehicle </value>
    [Range(0, 20)]
    [SerializeField] public float speed = 20.0f;

    // <value> This is the turning speed of the vehicle </value>
    [Range(0, 20)]
    [SerializeField] public float turnSpeed = 20.0f;
    public float horizontalInput;
    public float forwardInput;


    // Camera Variables
    public Camera mainCamera;
    public Camera hoodCamera;

    // Key to switch cameras
    public KeyCode switchKey;


    public string inputId;

    /// <summary>
    /// This method is called before the first frame update
    /// </summary>
    void Start()
    {

    }
    /// <summary>
    /// This method is called once per frame
    /// </summary>
    void Update()
    {
        // We get the player input
        horizontalInput = Input.GetAxis("Horizontal" + inputId);
        forwardInput = Input.GetAxis("Vertical" + inputId);

        // We move the vehicle forward
        transform.Translate(Vector3.forward * Time.deltaTime * speed * forwardInput);

        // We turn the vehicle
        transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * horizontalInput);


        //Cambio entre cámaras
    if(Input.GetKeyDown(switchKey))
    {
        mainCamera.enabled = !mainCamera.enabled;
        hoodCamera.enabled = !hoodCamera.enabled;
    }
    }
}