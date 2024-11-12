using UnityEngine;

public class VisibilityController : MonoBehaviour
{
    public float holdDuration = 5f; // Mouse bas�l� tutma s�resi
    private float timer = 0f; // Zamanlay�c�
    private bool isHolding = false; // Mouse'un bas�l� tutulup tutulmad���n� kontrol et
    private GameObject currentObject = null; // �u anda etkile�imde oldu�umuz nesne

    void Update()
    {
        // FPS Kameras� �zerinden raycast g�nderiyoruz
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Raycast ile t�klanan nesne
            GameObject hitObject = hit.collider.gameObject;

            // Nesnede VisibilityController script'inin olup olmad���n� kontrol et
            VisibilityController visibilityControllerScript = hitObject.GetComponent<VisibilityController>();
            if (visibilityControllerScript == null)
            {
                // E�er bu nesnede script yoksa, i�lemi atla
                return;
            }

            // Mouse bas�l� tutuluyorsa
            if (Input.GetMouseButton(0)) // 0 = LMB (sol fare tu�u)
            {
                if (currentObject == null || currentObject != hitObject)
                {
                    // E�er farkl� bir nesneye t�klanm��sa, zamanlay�c�y� s�f�rla
                    currentObject = hitObject;
                    timer = 0f;
                    isHolding = true;
                }

                if (isHolding)
                {
                    // Zamanlay�c�y� art�r
                    timer += Time.deltaTime;

                    // 5 saniye dolmu�sa, nesnenin g�r�n�rl���n� kapat
                    if (timer >= holdDuration)
                    {
                        // Nesnenin renderer bile�enini devre d��� b�rak
                        Renderer objectRenderer = currentObject.GetComponent<Renderer>();
                        if (objectRenderer != null)
                        {
                            objectRenderer.enabled = false; // Nesne g�r�nmez olur
                        }

                        // Zamanlay�c�y� s�f�rla
                        isHolding = false;
                    }
                }
            }
            else
            {
                // Mouse tu�u b�rak�ld���nda zamanlay�c�y� s�f�rla
                isHolding = false;
                timer = 0f;
            }
        }
    }
}
