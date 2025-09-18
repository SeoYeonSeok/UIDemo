using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AnimalRandomMove : MonoBehaviour
{
    NavMeshAgent agent;

    [Header("MoveRadius")]
    public float moveRadius = 1.5f; // 랜덤 위치를 찾을 최대 반경

    Animator moveAnim;
    private bool holdFlag = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        moveAnim = GetComponent<Animator>();

        MoveToRandomPosition(); // 시작 시 처음 이동
    }
    
    void Update()
    {
        // moveFlag가 false인 상황이고 (움직이지 않고 있는 상황)
        // + 현재 NavMesh가 경로 계산을 완료했거나 이동중인 상태이거나
        // + 목적지까지의 남은 거리가 정지 거리보다 작거나 같은지 확인한 뒤 모두 해당한다면
        if (!holdFlag && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(WaitAndMove()); // 기다렸다가 움직이는 코루틴 실행
        }
    }

    // 다음 목표 장소를 찾아 이동하는 메서드
    public void MoveToRandomPosition()
    {
        // 랜덤한 장소를 찾는 메서드 실행
        Vector3 randomPosition = GetRandomMovePosition(transform.position, moveRadius);

        // 랜덤하게 받아온 장소로 nav mesh agent의 다음 행선지 설정
        agent.SetDestination(randomPosition);

        // (애니메이터가 정상적으로 내장되어있다면) Move 애니메이션을 실행하는 flag를 true로 바꿔서 애니메이션 실행
        if (moveAnim != null) moveAnim.SetBool("isMove", true);
        else Debug.Log($"현재 {transform.name} 오브젝트에 애니메이터가 없음.");
    }

    // 랜덤한 장소를 찾는 메서드 (현재 위치를 기준으로, radius 변수의 범위 내에서)
    Vector3 GetRandomMovePosition(Vector3 currentPos, float radius)
    {
        // 10번의 시도 안에 다음으로 이동할 장소를 찾기
        // 횟수를 걸어놓은 이유 :
        // while문으로 다음 장소를 찾을때까지 무한 반복을 시도할 경우
        // 무한 루프에 걸려서 시스템이 멈춰버리는 문제가 발생하는 경우가 있음. 그 경우를 방지하기 위해 횟수 제한
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius; // 원형 범위 내 무작위 위치 생성
            randomDirection += currentPos; // 현재 위치를 기준으로 오프셋 적용

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
            {
                return hit.position; // 무작위 위치가 NavMesh 범위 내라면 유효한 NavMesh 위치 반환 
            }            
        }

        // 다음 랜덤한 장소를 찾는데 실패했으면 그냥 가만히 있기
        Debug.Log($"{transform.name}이 다음 장소를 찾는데 실패함");
        return currentPos;
    }

    IEnumerator WaitAndMove()
    {
        holdFlag = true; // 대기 상태 ON

        if (moveAnim != null) moveAnim.SetBool("isMove", false); // 아직 움직이지 않게 bool 플래그 false로 걸어놓기

        float waitTime = Random.Range(3, 9); // 가만히 있는 대기 시간을 3초 이상, 8초 이하 사이로 랜덤하게 받음
        yield return new WaitForSeconds(waitTime);

        MoveToRandomPosition(); // 대기 시간이 종료되면 다음 목표 장소를 찾는 메서드 실행
        holdFlag = false; // 대기 상태 OFF
    }
}
