using UnityEngine;

public class VisibilityController : MonoBehaviour
{
    public float holdDuration = 5f; // Mouse basýlý tutma süresi
    private float timer = 0f; // Zamanlayýcý
    private bool isHolding = false; // Mouse'un basýlý tutulup tutulmadýðýný kontrol et
    private GameObject currentObject = null; // Þu anda etkileþimde olduðumuz nesne

    void Update()
    {
        // FPS Kamerasý üzerinden raycast gönderiyoruz
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Raycast ile týklanan nesne
            GameObject hitObject = hit.collider.gameObject;

            // Nesnede VisibilityController script'inin olup olmadýðýný kontrol et
            VisibilityController visibilityControllerScript = hitObject.GetComponent<VisibilityController>();
            if (visibilityControllerScript == null)
            {
                // Eðer bu nesnede script yoksa, iþlemi atla
                return;
            }

            // Mouse basýlý tutuluyorsa
            if (Input.GetMouseButton(0)) // 0 = LMB (sol fare tuþu)
            {
                if (currentObject == null || currentObject != hitObject)
                {
                    // Eðer farklý bir nesneye týklanmýþsa, zamanlayýcýyý sýfýrla
                    currentObject = hitObject;
                    timer = 0f;
                    isHolding = true;
                }

                if (isHolding)
                {
                    // Zamanlayýcýyý artýr
                    timer += Time.deltaTime;

                    // 5 saniye dolmuþsa, nesnenin görünürlüðünü kapat
                    if (timer >= holdDuration)
                    {
                        // Nesnenin renderer bileþenini devre dýþý býrak
                        Renderer objectRenderer = currentObject.GetComponent<Renderer>();
                        if (objectRenderer != null)
                        {
                            objectRenderer.enabled = false; // Nesne görünmez olur
                        }

                        // Zamanlayýcýyý sýfýrla
                        isHolding = false;
                    }
                }
            }
            else
            {
                // Mouse tuþu býrakýldýðýnda zamanlayýcýyý sýfýrla
                isHolding = false;
                timer = 0f;
            }
        }
    }
}
