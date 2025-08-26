using System.Collections.Generic;
using UnityEngine;

public class MovementSystemManager : MonoBehaviour
{
    private static MovementSystemManager instance;
    public static MovementSystemManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<MovementSystemManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("MovementSystemManager");
                    instance = go.AddComponent<MovementSystemManager>();
                }
            }
            return instance;
        }
    }

    [Header("Global Settings")]
    [SerializeField] private bool globalPause = false;
    [SerializeField] private List<PlayerController> registeredControllers = new List<PlayerController>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterController(PlayerController controller)
    {
        if (!registeredControllers.Contains(controller))
        {
            registeredControllers.Add(controller);
        }
    }

    public void UnregisterController(PlayerController controller)
    {
        registeredControllers.Remove(controller);
    }

    public void PauseAllControllers()
    {
        globalPause = true;
        foreach (var controller in registeredControllers)
        {
            if (controller != null)
                controller.Pause();
        }
    }

    public void ResumeAllControllers()
    {
        globalPause = false;
        foreach (var controller in registeredControllers)
        {
            if (controller != null)
                controller.Resume();
        }
    }

    public void SwitchToResponsiveMovement()
    {
        foreach (var controller in registeredControllers)
        {
            if (controller != null)
                controller.SetMovementBehavior(new ResponsiveMovementBehavior());
        }
    }

    public void SwitchToFloatyMovement()
    {
        foreach (var controller in registeredControllers)
        {
            if (controller != null)
                controller.SetMovementBehavior(new FloatyMovementBehavior());
        }
    }
}