using UnityEngine;

public class DiggingController : MonoBehaviour
{
    public Camera playerCamera;  // FPS kamerasý
    public float digRadius = 1f;  // Kazma çapý
    public float digDepth = 0.5f; // Kazma derinliði
    public LayerMask diggableLayer;  // Kazýlabilir yüzeyler (örn. topraðý temsil eden layer)

    void Update()
    {
        // Mouse týklama ile kazma iþlemi
        if (Input.GetMouseButtonDown(0))  // Mouse sol tuþuna týklama
        {
            Dig();
        }
    }

    void Dig()
    {
        // Kameradan bir ýþýn gönder
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Iþýn herhangi bir objeye çarptýysa
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, diggableLayer))
        {
            // Týklanan objeye doðru bir kazma iþlemi baþlat
            Vector3 hitPoint = hit.point;

            // Kazma iþlemi (delik açma) burada yapýlacak
            CreateHole(hitPoint);
        }
    }

    void CreateHole(Vector3 position)
    {
        // Burada delik açma iþlemi yapýlacak. Örneðin, mesh deformasyonu veya terrain deformasyonu.
        Debug.Log("Digging at: " + position);

        // Örnek: Kazma çapý içinde bir delik açma
        Collider[] colliders = Physics.OverlapSphere(position, digRadius, diggableLayer);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out MeshFilter meshFilter))
            {
                Mesh mesh = meshFilter.mesh;
                Vector3[] vertices = mesh.vertices;
                for (int i = 0; i < vertices.Length; i++)
                {
                    Vector3 worldVertex = collider.transform.TransformPoint(vertices[i]);
                    float distance = Vector3.Distance(worldVertex, position);

                    // Eðer vertex týklanan nokta yakýnsa, onu aþaðýya doðru çek
                    if (distance < digRadius)
                    {
                        vertices[i] -= new Vector3(0, digDepth * (1 - distance / digRadius), 0);  // Delik açma
                    }
                }

                // Yeni vertexlerle mesh'i güncelle
                mesh.vertices = vertices;
                mesh.RecalculateNormals();  // Yeni normaller hesaplanmalý
            }
        }
    }
}
