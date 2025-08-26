using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private MovementConfig movementConfig;
    [SerializeField] private bool useTrackball = true;

    [Header("Trackball Settings")]
    [SerializeField] private bool invertHorizontal = false;
    [SerializeField] private bool invertVertical = true;
    [Tooltip("Sensibilidad del trackball")]
    [Range(0.5f, 3f)]
    [SerializeField] private float trackballSensitivity = 1f;

    [Header("Debug")]
    [SerializeField] private bool debugMode = false;
    [SerializeField] private Vector2 currentInput;
    [SerializeField] private string currentStateName;

    public IInputStrategy InputStrategy { get; private set; }
    public IMovementBehavior MovementBehavior { get; private set; }
    public Rigidbody2D Rigidbody { get; private set; }

    private IControllerState currentState;

    public event Action<Vector2> OnMovementInput;
    public event Action OnPrimaryAction;
    public event Action OnSecondaryAction;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        ConfigureRigidbody();
        InitializeStrategies();
    }

    private void Start()
    {
        ChangeState(new IdleState());
    }

    private void ConfigureRigidbody()
    {
        if (Rigidbody != null)
        {
            Rigidbody.gravityScale = 0f; 
            Rigidbody.linearDamping = 1f; 
            Rigidbody.freezeRotation = true; 
        }
    }

    private void InitializeStrategies()
    {
        InputStrategy = useTrackball ?
            new TrackballInputStrategy(movementConfig, invertHorizontal, invertVertical, trackballSensitivity) :
            new KeyboardInputStrategy();

        MovementBehavior = new FloatyMovementBehavior();
        MovementBehavior.Configure(movementConfig);
    }

    public void UpdateTrackballSettings(bool invertH, bool invertV, float sensitivity)
    {
        invertHorizontal = invertH;
        invertVertical = invertV;
        trackballSensitivity = sensitivity;

        if (useTrackball)
        {
            InputStrategy = new TrackballInputStrategy(movementConfig, invertHorizontal, invertVertical, trackballSensitivity);
        }
    }

    private void Update()
    {
        if (InputStrategy.GetPrimaryAction())
        {
            OnPrimaryAction?.Invoke();
            HandlePrimaryAction();
        }

        if (InputStrategy.GetSecondaryAction())
        {
            OnSecondaryAction?.Invoke();
            HandleSecondaryAction();
        }

        if (debugMode)
        {
            currentInput = InputStrategy.GetMovementInput();
            currentStateName = currentState?.GetType().Name ?? "None";
        }
    }

    private void FixedUpdate()
    {
        currentState?.Execute(this);

        Vector2 input = InputStrategy.GetMovementInput();
        if (input != Vector2.zero)
        {
            OnMovementInput?.Invoke(input);
        }
    }

    public void ChangeState(IControllerState newState)
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState?.Enter(this);
    }

    public void SetMovementBehavior(IMovementBehavior newBehavior)
    {
        MovementBehavior = newBehavior;
        MovementBehavior.Configure(movementConfig);
    }

    public void SetInputStrategy(IInputStrategy newStrategy)
    {
        InputStrategy = newStrategy;
    }

    public void Pause()
    {
        ChangeState(new PausedState());
    }

    public void Resume()
    {
        ChangeState(new IdleState());
    }

    private void HandlePrimaryAction()
    {

        Debug.Log("Primary Action (Left Click)");
    }

    private void HandleSecondaryAction()
    {
        Debug.Log("Secondary Action (Right Click)");
    }

    private void OnDrawGizmos()
    {
        if (!debugMode) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, (Vector3)currentInput * 2f);

        if (Rigidbody != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, (Vector3)Rigidbody.linearVelocity.normalized * 3f);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying && useTrackball)
        {
            UpdateTrackballSettings(invertHorizontal, invertVertical, trackballSensitivity);
        }
    }
#endif
}