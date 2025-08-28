using UnityEngine;
using UnityEngine.Pool;

public class PlayerCannonController : MonoBehaviour
{
    [Header("Cannon Settings")]
    [SerializeField] private Transform cannonPivot;
    [SerializeField] private Transform firePoint;   
    [SerializeField] private float rotationSpeed = 180f;

    [Header("Controls")]
    [SerializeField] private KeyCode rotateClockwiseKey = KeyCode.F;   
    [SerializeField] private KeyCode shootKey = KeyCode.Space;         

    [Header("Shooting")]
    [SerializeField] private ObjectPoolMaxi projectilePool;   
    [SerializeField] private float projectileSpeed = 15f;
    [SerializeField] private float recoilForce = 10f;
    [SerializeField] private float fireCooldown = 0.5f;

    private float cooldownTimer = 0f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>(); 
    }

    private void Update()
    {
        HandleRotation();
        HandleShooting();
    }

    private void HandleRotation()
    {
        float rotationInput = 0f;

        if (Input.GetKey(rotateClockwiseKey)) rotationInput = -1f;

        if (rotationInput != 0f)
        {
            cannonPivot.Rotate(Vector3.forward, rotationInput * rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleShooting()
    {
        cooldownTimer -= Time.deltaTime;
        if (Input.GetKeyDown(shootKey) && cooldownTimer <= 0f)
        {
            Shoot();
            cooldownTimer = fireCooldown;
        }
    }

    private void Shoot()
    {
        if (projectilePool == null)
        {
            Debug.LogWarning("No ObjectPool");
            return;
        }
        GameObject projectile = projectilePool.GetObject();
        projectile.transform.position = firePoint.position;
        projectile.transform.rotation = cannonPivot.rotation;
        projectile.SetActive(true);

        Rigidbody2D projRb = projectile.GetComponent<Rigidbody2D>();
        if (projRb != null)
        {
            projRb.linearVelocity = cannonPivot.up * projectileSpeed;
        }
        Vector2 recoilDir = -(cannonPivot.up).normalized;
        rb.AddForce(recoilDir * recoilForce, ForceMode2D.Impulse);
    }
}