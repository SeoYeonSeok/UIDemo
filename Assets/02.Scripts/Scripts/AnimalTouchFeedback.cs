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
        
        if (heartEffect == null) Debug.Log($"현재 {transform.name}의 파티클 시스템을 못 찾음");
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

        if (Physics.Raycast(ray, out hit)) // 3D 오브젝트 감지 (Physics)
        {
            if (hit.collider.gameObject == gameObject) // 현재 오브젝트가 맞으면
            {                
                if (anim != null) anim.SetTrigger("Touched"); // 애니메이션 컨트롤러의 Touched 트리거 활성화
                else Debug.Log($"현재 {transform.name}의 애니메이터가 없음");

                if (heartEffect != null) heartEffect.Play();
                else Debug.Log($"현재 {transform.name}의 파티클 시스템이 없음");
            }
        }
    }    
}
