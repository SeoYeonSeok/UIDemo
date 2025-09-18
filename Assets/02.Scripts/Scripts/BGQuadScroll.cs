using UnityEngine;

public class BGQuadScroll : MonoBehaviour
{       
    public float scrollSpeed = 0.05f; // ��ũ�� ���ǵ�, ���� ���� ����

    public Vector2 direction = new Vector2(1f, -1f); // ��ũ�� xy�� ����, ���������� ���� ���� ����

    private Renderer rend;
    private Vector2 offset = Vector2.zero;
    private Vector2 normDir;


    void Start()
    {
        rend = GetComponent<Renderer>();

        // ��Ƽ���� �ν��Ͻ� ����(���� ��Ƽ���� �ǵ帮�� �ʱ� ����)
        rend.material = new Material(rend.sharedMaterial);

        if (direction == Vector2.zero) normDir = Vector2.left; // �⺻�� ������ġ
        else normDir = direction.normalized;
    }

    void Update()
    {        
        offset += normDir * scrollSpeed * Time.deltaTime; // deltaTime ����ؼ� ������ ���������� ��ũ��
        
        rend.material.mainTextureOffset = offset; // mainTextureOffset�� ����
    }

    void OnDestroy()
    {        
        if (rend != null && rend.material != null)
        {
            Destroy(rend.material);
        }
    }
}
