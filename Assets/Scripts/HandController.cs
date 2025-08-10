using System.Collections;
using UnityEngine;
public class HandController : MonoBehaviour
{
    [Header("Animation Settings")]
    public float slapDuration = 0.3f;
    public float slapDistance = 2f;

    [Header("Layer Detection")]
    public LayerMask mosquitoLayer = 8;
    public LayerMask butterflyLayer = 9;

    private Vector3 initialPosition;
    private bool isSlapping = false;
    private CircleCollider2D handCollider;

    // HAND SATES
    private IHandState currentState;
    private HandIdleState idleState;
    private HandSlapState slapState;

    private void Start()
    {
        initialPosition = transform.position;
        handCollider = GetComponent<CircleCollider2D>();

        idleState = new HandIdleState(this);
        slapState = new HandSlapState(this);

        currentState = idleState;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isSlapping) return;

        // CHECK LAYER
        int otherLayer = other.gameObject.layer;

        if (IsInLayerMask(otherLayer, mosquitoLayer))
        {
            other.GetComponent<InsectController>()?.HandleHit();
        }
        else if (IsInLayerMask(otherLayer, butterflyLayer))
        {
            other.GetComponent<InsectController>()?.HandleHit();
        }
    }

    private bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return (layerMask.value & (1 << layer)) != 0;
    }

    public void PerformSlap()
    {
        currentState.HandleSlap();
    }

    public void ChangeState(IHandState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    public void StartSlapAnimation()
    {
        if (!isSlapping)
        {
            StartCoroutine(SlapCoroutine());
        }
    }

    private IEnumerator SlapCoroutine()
    {
        isSlapping = true;

        Vector3 targetPosition = initialPosition + Vector3.down * slapDistance;
        float elapsed = 0f;

        // DOWN ANIMATION
        while (elapsed < slapDuration / 2)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / (slapDuration / 2);
            transform.position = Vector3.Lerp(initialPosition, targetPosition, progress);
            yield return null;
        }

        elapsed = 0f;

        // UP ANIMATION
        while (elapsed < slapDuration / 2)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / (slapDuration / 2);
            transform.position = Vector3.Lerp(targetPosition, initialPosition, progress);
            yield return null;
        }

        transform.position = initialPosition;
        float punishmentDelay = 0.5f; 
        yield return new WaitForSeconds(punishmentDelay);

        isSlapping = false;
        ChangeState(idleState);
    }
    // Getters
    public HandIdleState IdleState => idleState;
    public HandSlapState SlapState => slapState;
    public bool IsSlapping => isSlapping;
}