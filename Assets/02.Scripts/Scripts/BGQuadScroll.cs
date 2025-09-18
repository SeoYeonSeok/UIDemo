using UnityEngine;

public class BGQuadScroll : MonoBehaviour
{
    public float scrollSpeed = 0.05f;
    public Vector2 direction = new Vector2(1f, -1f);

    private Material mat;
    private Vector2 offset;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        offset += direction.normalized * scrollSpeed * Time.deltaTime;
        mat.mainTextureOffset = offset;
    }
}
