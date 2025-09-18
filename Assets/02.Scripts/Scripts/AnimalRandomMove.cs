using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AnimalRandomMove : MonoBehaviour
{
    NavMeshAgent agent;

    [Header("MoveRadius")]
    public float moveRadius = 1.5f; // ���� ��ġ�� ã�� �ִ� �ݰ�

    Animator moveAnim;
    private bool holdFlag = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        moveAnim = GetComponent<Animator>();

        MoveToRandomPosition(); // ���� �� ó�� �̵�
    }
    
    void Update()
    {
        // moveFlag�� false�� ��Ȳ�̰� (�������� �ʰ� �ִ� ��Ȳ)
        // + ���� NavMesh�� ��� ����� �Ϸ��߰ų� �̵����� �����̰ų�
        // + ������������ ���� �Ÿ��� ���� �Ÿ����� �۰ų� ������ Ȯ���� �� ��� �ش��Ѵٸ�
        if (!holdFlag && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(WaitAndMove()); // ��ٷȴٰ� �����̴� �ڷ�ƾ ����
        }
    }

    // ���� ��ǥ ��Ҹ� ã�� �̵��ϴ� �޼���
    public void MoveToRandomPosition()
    {
        // ������ ��Ҹ� ã�� �޼��� ����
        Vector3 randomPosition = GetRandomMovePosition(transform.position, moveRadius);

        // �����ϰ� �޾ƿ� ��ҷ� nav mesh agent�� ���� �༱�� ����
        agent.SetDestination(randomPosition);

        // (�ִϸ����Ͱ� ���������� ����Ǿ��ִٸ�) Move �ִϸ��̼��� �����ϴ� flag�� true�� �ٲ㼭 �ִϸ��̼� ����
        if (moveAnim != null) moveAnim.SetBool("isMove", true);
        else Debug.Log($"���� {transform.name} ������Ʈ�� �ִϸ����Ͱ� ����.");
    }

    // ������ ��Ҹ� ã�� �޼��� (���� ��ġ�� ��������, radius ������ ���� ������)
    Vector3 GetRandomMovePosition(Vector3 currentPos, float radius)
    {
        // 10���� �õ� �ȿ� �������� �̵��� ��Ҹ� ã��
        // Ƚ���� �ɾ���� ���� :
        // while������ ���� ��Ҹ� ã�������� ���� �ݺ��� �õ��� ���
        // ���� ������ �ɷ��� �ý����� ��������� ������ �߻��ϴ� ��찡 ����. �� ��츦 �����ϱ� ���� Ƚ�� ����
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius; // ���� ���� �� ������ ��ġ ����
            randomDirection += currentPos; // ���� ��ġ�� �������� ������ ����

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
            {
                return hit.position; // ������ ��ġ�� NavMesh ���� ����� ��ȿ�� NavMesh ��ġ ��ȯ 
            }            
        }

        // ���� ������ ��Ҹ� ã�µ� ���������� �׳� ������ �ֱ�
        Debug.Log($"{transform.name}�� ���� ��Ҹ� ã�µ� ������");
        return currentPos;
    }

    IEnumerator WaitAndMove()
    {
        holdFlag = true; // ��� ���� ON

        if (moveAnim != null) moveAnim.SetBool("isMove", false); // ���� �������� �ʰ� bool �÷��� false�� �ɾ����

        float waitTime = Random.Range(3, 9); // ������ �ִ� ��� �ð��� 3�� �̻�, 8�� ���� ���̷� �����ϰ� ����
        yield return new WaitForSeconds(waitTime);

        MoveToRandomPosition(); // ��� �ð��� ����Ǹ� ���� ��ǥ ��Ҹ� ã�� �޼��� ����
        holdFlag = false; // ��� ���� OFF
    }
}
