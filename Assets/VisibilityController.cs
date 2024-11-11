using System.Collections;
using UnityEngine;

public class VisibilityController : MonoBehaviour
{
    [Header("Visibility Settings")]
    [Tooltip("Fareyi basýlý tutma süresi (saniye)")]
    public float holdDuration = 5f;  // Fareyi basýlý tutma süresi (5 saniye)

    private bool isMouseDown = false;  // Mouse'un basýlý tutulup tutulmadýðýný kontrol etmek için
    private float mouseDownTime = 0f;  // Fare tuþunun ne kadar süre basýlý olduðunu takip eder

    void Update()
    {
        // Sol fare tuþuna basýldýðýnda, mouseDownTime'ý sýfýrlýyoruz
        if (Input.GetMouseButtonDown(0))  // Fare tuþuna basýldýðýnda
        {
            isMouseDown = true;
            mouseDownTime = Time.time;  // Basýlmaya baþlanan zamaný kaydediyoruz
        }

        // Sol fare tuþu býrakýldýðýnda, iþlemi sonlandýrýyoruz
        if (Input.GetMouseButtonUp(0))  // Fare tuþu býrakýldýðýnda
        {
            isMouseDown = false;
            mouseDownTime = 0f;  // Zamaný sýfýrlýyoruz
        }

        // Eðer fare basýlý tutuluyorsa
        if (isMouseDown)
        {
            // Fareyi basýlý tutma süresi, belirlenen süreyi geçtiyse nesneyi gizle
            if (Time.time - mouseDownTime >= holdDuration)
            {
                // Raycast iþlemiyle týklanan nesneyi buluyoruz
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    // Eðer týklanan nesne bu nesne ise, onu gizleriz
                    if (hit.transform == transform)
                    {
                        StartCoroutine(ToggleVisibility());  // Görünürlüðü geçici olarak kapat
                    }
                }

                isMouseDown = false;  // Týklama süresi tamamlandýktan sonra tekrar baþlatýlmasýn diye kapatýyoruz
            }
        }
    }

    // Görünürlüðü geçici olarak kapat ve sonra geri aç
    IEnumerator ToggleVisibility()
    {
        // Görünürlüðü kapat: Nesneyi aktif deðil yapýyoruz
        gameObject.SetActive(false);

        // 1 frame bekle ve ardýndan görünürlüðü geri aç
        yield return null;

        // Görünürlüðü geri aç: Nesneyi tekrar aktif yapýyoruz
        gameObject.SetActive(true);
    }
}
