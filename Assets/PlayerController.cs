using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public float moveSpeed = 5f;           // Hareket hýzý
    public float sprintMultiplier = 2f;    // Koþma hýzý çarpaný
    public float jumpForce = 7f;           // Zýplama gücü
    public float sizeChangeFactor = 0.5f;  // Boyut küçültme faktörü

    [Header("Mouse Look Settings")]
    public float lookSpeedX = 2.0f;        // Yatay dönüþ hýzý
    public float lookSpeedY = 2.0f;        // Dikey dönüþ hýzý
    public float upperLookLimit = -60f;    // Kamera üst sýnýrý
    public float lowerLookLimit = 60f;     // Kamera alt sýnýrý

    [Header("Camera Bobbing Settings")]
    public float walkBobAmount = 0.05f;    // Yavaþ yürürken baþ sallanma miktarý
    public float sprintBobAmount = 0.1f;   // Hýzlý koþarken baþ sallanma miktarý
    public float bobSpeed = 10f;           // Sallanma hýzýný kontrol etme

    private Rigidbody rb;
    private Camera playerCamera;
    private Vector3 originalScale;
    private bool isGrounded;
    private float rotationX = 0f;          // Dikey dönüþ miktarý
    private Vector3 lastPosition;          // Son pozisyon
    private float bobTimer = 0f;           // Sallanma zamanlayýcýsý

    void Start()
    {
        // Rigidbody ve kamera bileþenlerini al
        rb = GetComponent<Rigidbody>();
        playerCamera = Camera.main;
        originalScale = transform.localScale;
        lastPosition = transform.position;
    }

    void Update()
    {
        // Hareket, zýplama, boyut küçültme iþlemleri
        HandleMovement();
        HandleJumping();
        HandleSizeChange();

        // Kamera döndürme
        HandleMouseLook();

        // Kamera sallanma (head bobbing) iþlemleri
        HandleHeadBobbing();
    }

    private void HandleMovement()
    {
        // Kullanýcý giriþini al (W, A, S, D tuþlarý)
        float horizontal = Input.GetAxis("Horizontal");  // A/D tuþlarý
        float vertical = Input.GetAxis("Vertical");      // W/S tuþlarý

        // Koþma durumu: Shift tuþu ile hýz artýþý
        float currentMoveSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentMoveSpeed *= sprintMultiplier; // Koþma hýzýný artýr
        }

        // Hareket yönünü oluþtur
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            // Yeni pozisyonu hesapla
            Vector3 move = moveDirection * currentMoveSpeed * Time.deltaTime;

            // Rigidbody ile hareket etmek, fiziksel hesaplamalar için daha doðru sonuç verir
            rb.MovePosition(transform.position + move);
        }
    }

    private void HandleJumping()
    {
        // Zýplama iþlemi
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

    void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    private void HandleMouseLook()
    {
        // Yatay dönüþ (saða/sola)
        float mouseX = Input.GetAxis("Mouse X") * lookSpeedX;
        transform.Rotate(0, mouseX, 0);

        // Dikey dönüþ (yukarý/aþaðý)
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, upperLookLimit, lowerLookLimit); // Dikey hareket sýnýrlarý
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0); // Kamerayý dikeyde döndür
    }

    private void HandleHeadBobbing()
    {
        // Koþarken baþ hareketi (head bobbing)
        if (isGrounded && (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0))
        {
            // Yürürken baþ hareketini hesapla
            float speed = (Input.GetKey(KeyCode.LeftShift)) ? sprintBobAmount : walkBobAmount;
            bobTimer += Time.deltaTime * bobSpeed;

            // X ve Y ekseninde hareket
            float bobbingX = Mathf.Sin(bobTimer) * speed;
            float bobbingY = Mathf.Cos(bobTimer) * speed;

            // Kamerayý baþ sallanmasýyla hareket ettir
            playerCamera.transform.localPosition = new Vector3(0, bobbingY, bobbingX);
        }
        else
        {
            // Hareket yoksa baþ sallanmasý sýfýr
            playerCamera.transform.localPosition = Vector3.zero;
        }
    }

    private void HandleMovementWithMouseDirection()
    {
        // Mouse pozisyonunu kullanarak karakteri yönlendir
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition); // Kamera ile mouse pozisyonu arasýnda ýþýn gönder

        // Iþýnýn çarpma noktasýný bul
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // Mouse'un gösterdiði yere doðru yönelme
            Vector3 targetDirection = (hit.point - transform.position).normalized;

            // Karakteri bu yöne döndür
            float step = moveSpeed * Time.deltaTime; // Yavaþ yavaþ döndürme
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, step, 0f);
            transform.rotation = Quaternion.LookRotation(newDirection);

            // Hareketi karakterin ön yönünde yap
            Vector3 move = transform.forward * moveSpeed * Time.deltaTime;
            rb.MovePosition(transform.position + move);
        }
    }
}
