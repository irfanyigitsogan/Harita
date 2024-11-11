using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    // Hareket, zýplama ve boyut küçültme için gerekli parametreler
    [Header("Player Settings")]
    public float moveSpeed = 5f;           // Hareket hýzý
    public float jumpForce = 7f;           // Zýplama gücü
    public float sizeChangeFactor = 0.5f;  // Boyut küçültme faktörü

    // Kamera döndürme için parametreler
    [Header("Mouse Look Settings")]
    public float lookSpeedX = 2.0f;       // Yatay dönüþ hýzý (saða/sola)
    public float lookSpeedY = 2.0f;       // Dikey dönüþ hýzý (yukarý/aþaðý)
    public float upperLookLimit = -60f;   // Kameranýn yukarýya bakma sýnýrý
    public float lowerLookLimit = 60f;    // Kameranýn aþaðýya bakma sýnýrý

    private Rigidbody rb;
    private Camera playerCamera;
    private Vector3 originalScale;
    private bool isGrounded;
    private float rotationX = 0f;          // Dikey dönüþ miktarý (kamera)

    void Start()
    {
        // Rigidbody bileþenini al
        rb = GetComponent<Rigidbody>();
        playerCamera = Camera.main; // Ana kamera
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Hareket ve zýplama iþlemleri
        HandleMovement();
        HandleJumping();
        HandleSizeChange();

        // Kamera döndürme iþlemleri
        HandleMouseLook();
    }

    private void HandleMovement()
    {
        // Kullanýcý giriþini al (W, A, S, D tuþlarý)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Hareket yönünü oluþtur
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            // Yeni pozisyonu hesapla
            Vector3 move = moveDirection * moveSpeed * Time.deltaTime;
            transform.Translate(move, Space.World); // Hareketi dünya uzayýnda yap
        }
    }

    private void HandleJumping()
    {
        // Zýplama iþlemi: Sadece yerle temas halindeyken
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void HandleSizeChange()
    {
        // Boyut küçültme iþlemi
        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.localScale = new Vector3(originalScale.x, originalScale.y * sizeChangeFactor, originalScale.z);
        }
        else
        {
            transform.localScale = originalScale;
        }
    }

    // Yerle temas kontrolü
    void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    // Mouse hareketine göre kamerayý döndürme
    private void HandleMouseLook()
    {
        // Yatay dönüþ (saða/sola) hareketi
        float mouseX = Input.GetAxis("Mouse X") * lookSpeedX;
        transform.Rotate(0, mouseX, 0);

        // Dikey dönüþ (yukarý/aþaðý) hareketi
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, upperLookLimit, lowerLookLimit); // Dikey hareket sýnýrlarý
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0); // Kamerayý dikeyde döndür
    }
}
