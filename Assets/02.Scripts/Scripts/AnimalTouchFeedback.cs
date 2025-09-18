using UnityEngine;
using UnityEngine.UIElements;

public class AnimalTouchFeedback : MonoBehaviour
{
    Animator anim;
    ParticleSystem heartEffect;


    void Start()
    {
        anim = GetComponent<Animator>();

        Debug.Log(transform.childCount);
        for (int i = 0; i < transform.childCount; i++)
        {
            Debug.Log (transform.GetChild(i).name);

            if (transform.GetChild(i).GetComponent<ParticleSystem>() != null)
            {
                heartEffect = transform.GetChild(i).GetComponent<ParticleSystem>();
                i = transform.childCount;
            }
        }
        
        if (heartEffect == null) Debug.Log($"���� {transform.name}�� ��ƼŬ �ý����� �� ã��");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            WhenTouchedOrClicked(Input.mousePosition);
        }
    }

    void WhenTouchedOrClicked(Vector2 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) // 3D ������Ʈ ���� (Physics)
        {
            if (hit.collider.gameObject == gameObject) // ���� ������Ʈ�� ������
            {                
                if (anim != null) anim.SetTrigger("Touched"); // �ִϸ��̼� ��Ʈ�ѷ��� Touched Ʈ���� Ȱ��ȭ
                else Debug.Log($"���� {transform.name}�� �ִϸ����Ͱ� ����");

                if (heartEffect != null) heartEffect.Play();
                else Debug.Log($"���� {transform.name}�� ��ƼŬ �ý����� ����");
            }
        }
    }    
}
