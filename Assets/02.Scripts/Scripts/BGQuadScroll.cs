using UnityEngine;

public class BGQuadScroll : MonoBehaviour
{       
    public float scrollSpeed = 0.05f; // 스크롤 스피드, 별도 설정 가능

    public Vector2 direction = new Vector2(1f, -1f); // 스크롤 xy축 방향, 마찬가지로 별도 설정 가능

    private Renderer rend;
    private Vector2 offset = Vector2.zero;
    private Vector2 normDir;


    void Start()
    {
        rend = GetComponent<Renderer>();

        // 머티리얼 인스턴스 생성(공유 머티리얼 건드리지 않기 위함)
        rend.material = new Material(rend.sharedMaterial);

        if (direction == Vector2.zero) normDir = Vector2.left; // 기본값 안전장치
        else normDir = direction.normalized;
    }

    void Update()
    {        
        offset += normDir * scrollSpeed * Time.deltaTime; // deltaTime 사용해서 프레임 독립적으로 스크롤
        
        rend.material.mainTextureOffset = offset; // mainTextureOffset에 적용
    }

    void OnDestroy()
    {        
        if (rend != null && rend.material != null)
        {
            Destroy(rend.material);
        }
    }
}
