using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    public Camera mainCam;
    public Transform animalSpawnPos; // 에그가 해치되면 동물이 등장하는 Transform
    public GameObject uiMainMenus; // 시작 버튼을 누르면 사라지는 메인 메뉴 ui 요소들
    public GameObject uiInFields; // 에그 해치 후 등장하는 ui 요소들
    public LogoTween logoTween; // 로고에 내장된 Tween 스크립트

    private int gainedAnimalNum; // 현재 얻은 동물의 숫자
    private int gainedGold; // 현재 얻은 코인
    private int goldCurrent = 0; // 지금까지 보유 중인 코인
    private int animalCurrent = 0; // 지금까지 보유 중인 동물

    [Header("Egg's Elements")]
    public Button btnEggHatchBtn; // 시작 버튼
    public GameObject Egg;
    public Animator animEgg;
    public TextMeshProUGUI txtHatchMsg;
    public GameObject imgHatchMsg;
    public ParticleSystem ptcEggShakeEffect;
    public ParticleSystem ptcEggHatchEffect;
    private float eggShakeSpeed = 1f;
    private GameObject hatchedAnimal;

    [Header("Animals")]
    public GameObject[] goAnimals; // 동물 모델들 (이동이나 피드백과 같은 부분들은 없는 모델들)
    public GameObject[] goAnimalsMoveable;
    public Sprite[] animalIcon;
    public Transform TrMovableAnimalSpawnPos;

    [Header("Circular Wipe Effect")]
    public Image img_circularWipeImg; // 원형 화면 전환 효과    

    [Header("Field's Elements")]
    public GameObject goField;
    public GameObject AlertPanel; // 알림 이미지 이외의 다른 오브젝트들의 클릭/터치 입력을 막기 위한 백그라운드 겸 그룹화 목적 부모 오브젝트
    public Image alert; // 알림 이미지
    public TextMeshProUGUI TxtCurrentGold; // 현재 보유 중인 골드 수를 표현하는 텍스트
    public TextMeshProUGUI TxtCurrentAnimal; // 현재 보유 중인 동물 수를 표현하는 텍스트
    public TextMeshProUGUI TxtAlertPanel1; // 보상 알림창에 있는 TMP 컴포넌트
    public TextMeshProUGUI TxtAlertPanel2; // 보상 알림창에 있는 TMP 컴포넌트  
    public Image ImgAlertPanel; // 보상 알림창에 있는 이미지 컴포넌트

    // 에그 해치 버튼 누를경우 실행되는 메서드
    public void PressEggHatchBtn()
    {
        // 일단 Egg Hatch 버튼 기능 일시적 비활성화 (중복으로 눌러서 버그 발생을 막기 위함)
        btnEggHatchBtn.enabled = false;
        // Egg Hatch 버튼의 비활성화를 시각적으로 피드백해주는 소멸 Tween 실행
        btnEggHatchBtn.gameObject.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.Linear);

        // Egg에 내장되어있는 애니메이션 실행
        animEgg.SetBool("Shake", true);

        // Egg가 움직이면서 파티클 이펙트 실행
        ptcEggShakeEffect.Play();

        // 애니메이터의 속도를 1에서 3까지 5초 동안 Tween 선형 증가
        DOTween.To(() => eggShakeSpeed, x =>
        {
            eggShakeSpeed = x;
            animEgg.speed = eggShakeSpeed;
        },3f, 3f).SetEase(Ease.Linear)
        .OnComplete(() =>
        {            
            // 계란의 로테이션, 애니메이션 속도, 애니메이션 플래그 전부 초깃값으로 돌려놓고 일시적 비활성화하기
            animEgg.SetBool("Shake", false);
            eggShakeSpeed = 1f;
            Egg.transform.localRotation = Quaternion.identity;
            Egg.SetActive(false);
            
            // 파티클 이펙트 종료 / 실행 후
            ptcEggShakeEffect.Stop();
            ptcEggHatchEffect.Play();

            // 동물 소환 메서드 실행
            SpawnAnimals();
        });
    }

    // 동물 배치 메서드
    private void SpawnAnimals()
    {
        // 랜덤하게 동물 모델 특정 위치에 생성
        gainedAnimalNum = Random.Range(0, goAnimals.Length);
        hatchedAnimal = Instantiate(goAnimals[gainedAnimalNum], animalSpawnPos);

        // 골드 획득
        gainedGold = Random.Range(1, 6) * 5;
        goldCurrent += gainedGold;

        // 필드에도 방금 잡은 동물을 소환하기
        GameObject go = Instantiate(goAnimalsMoveable[gainedAnimalNum], TrMovableAnimalSpawnPos.position, Quaternion.identity);
        go.GetComponent<AnimalRandomMove>().enabled = true;

        // !!! 여기에다 텍스트 연출도 넣기 !!!
        imgHatchMsg.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);
        txtHatchMsg.text = $"와! {GetAnimalName(gainedAnimalNum)} 획득했다!";

        // 동물 모델 생성 후 크기 xyz축 전부 0.1로 초기화
        hatchedAnimal.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        // 시퀀스로 Tween 흐름 관리
        Sequence sq = DOTween.Sequence();
        sq.Append(hatchedAnimal.transform.DOScale(Vector3.one, 0.75f).SetEase(Ease.OutBack));
        sq.Join(hatchedAnimal.transform.DOLocalRotate(new Vector3(0f, 580f, 0f), 0.75f, RotateMode.FastBeyond360));

        // 2초의 지연 후 화면 전환 효과 실행 (동일 씬에서 실행)
        sq.AppendInterval(2f);
        sq.AppendCallback(() => CircularWipeEffectOn());        
    }

    // 동물 이름 텍스트로 받아오기
    private string GetAnimalName(int animal)
    {
        if (animal == 0) return "<color=purple><bounce>강아지</bounce></color>를";
        else if (animal == 1) return "<color=red><bounce>하마</bounce></color>를";
        else if (animal == 2) return "<color=purple><bounce>고슴도치</bounce></color>를";
        else if (animal == 3) return "<color=blue><bounce>사자</bounce></color>를";
        else if (animal == 4) return "<color=blue><bounce>코끼리</bounce></color>를";
        else if (animal == 5) return "<color=purple><bounce>기린</bounce></color>을";
        else if (animal == 6) return "<color=red><bounce>말</bounce></color>을";
        else if (animal == 7) return "<color=purple><bounce>소</bounce></color>를";
        else return "NULL";
    }

    // 원형 화면 전환 효과 메서드
    private void CircularWipeEffectOn()
    {
        img_circularWipeImg.gameObject.SetActive(true);

        float imgFill = 0f;

        Sequence sq = DOTween.Sequence();
        sq.Append(DOTween.To(() => imgFill, x =>
        {
            imgFill = x;
            img_circularWipeImg.fillAmount = imgFill;
        }, 1f, 1f).SetEase(Ease.Linear));

        sq.AppendInterval(2f);
        sq.AppendCallback(() => CircularWipeEffectOff());
    }

    // 원형 화면 전환 효과 종료 메서드
    private void CircularWipeEffectOff()
    {
        // 알에서 부화된 동물 없애기 
        Destroy(hatchedAnimal);

        // 메인메뉴 UI들 active false하고
        logoTween.EndTween(); // 혹시 모를 경우에 대비해 로고에서 실행 중인 트윈은 종료해놓기
        imgHatchMsg.transform.localScale = Vector3.zero;
        txtHatchMsg.text = "";
        animalSpawnPos.gameObject.SetActive(false);
        uiMainMenus.SetActive(false);

        // 필드 active true 해놓기
        goField.SetActive(true);

        // 시퀀스 1 : 원형 화면 전환 효과 없애기
        float imgFill = 1f;

        Sequence sq1 = DOTween.Sequence();
        sq1.Append(DOTween.To(() => imgFill, x =>
        {
            imgFill = x;
            img_circularWipeImg.fillAmount = imgFill;
        }, 0f, 1f).SetEase(Ease.Linear).OnComplete(() => img_circularWipeImg.gameObject.SetActive(false)));

        // 시퀀스 2 : 카메라 줌업 효과 위해 FOV 값 변경 -> 알림 UI 출력
        float camFov = 90f;

        Sequence sq2 = DOTween.Sequence();        
        sq2.Append(DOTween.To(() => camFov, x =>
        {
            camFov = x;
            mainCam.fieldOfView = camFov;
        }, 60f, 1f).SetEase(Ease.Linear).OnComplete(() => InFieldUITweens()));
        // 원형 화면 전환 효과 & 이펙트 종료와 후에 새 UI들 등장 연출 메서드 실행               
    }

    // 인필드 UI들을 띄우기 + 알림 메시지 출력하기
    private void InFieldUITweens()
    {
        UITweens[] tweens = new UITweens[uiInFields.transform.childCount];

        for (int i = 0; i < tweens.Length; i++)
        {
            tweens[i] = uiInFields.transform.GetChild(i).GetComponent<UITweens>();
            tweens[i].transform.gameObject.SetActive(true);
            tweens[i].ActivateTween();
        }

        // 데이터 누적 & UI에 반영하는 작업 시행 (골드 & 현재 보유 중인 동물 수 증가)        
        TxtCurrentGold.text = (goldCurrent).ToString();
        TxtCurrentAnimal.text = (++animalCurrent).ToString();

        // 보상 알림 창 출력 메서드 실행
        ShowAlert();
    }

    // 인필드에서 메인 메뉴로 돌아가기
    public void PressBack2HatchBtn()
    {
        // 우선 현재 화면의 UI들을 전부 바깥으로 내보내는 트윈 실행하기
        UITweens[] tweens = new UITweens[uiInFields.transform.childCount];

        for (int i = 0; i < uiInFields.transform.childCount; i++)
        {
            tweens[i] = uiInFields.transform.GetChild(i).GetComponent<UITweens>();
            tweens[i].ActivateReverseTween();
        }

        // 숨겨졌던 메인 메뉴 UI들도 다시 activew true로 되돌리고 종료된 로고 트윈 재실행하기
        uiMainMenus.SetActive(true);
        logoTween.RestartTween();

        // Egg Hatch 버튼을 누를때 일시적 비활성화시켜뒀던 Egg Hatch 버튼도 다시 enable 하기
        btnEggHatchBtn.enabled = true;
        // Egg Hatch 버튼에도 재활성화 시각적 피드백을 주는 Tween 주기
        btnEggHatchBtn.gameObject.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);

        // Egg 이미지도 다시 재활성화하면서 시각적 피드백 주기
        Egg.SetActive(true);
        Egg.transform.localScale = Vector3.zero;
        Egg.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);

        // Egg Hatch할때 나오는 이펙트들도 다시 재활성화
        animalSpawnPos.gameObject.SetActive(true);
    }

    // 보상 알림 창 출렧 메서드
    private void ShowAlert()
    {
        // 보상 알림 UI 출력 준비
        AlertPanel.SetActive(true); // 알림 이미지 백그라운드 active true로 만들어서 다른 입력 막기
        AlertPanel.GetComponent<Image>().color = new Color(255f, 255f, 255f, 0.25f); // 알림 이미지 백그라운드는 살짝 불투명하게 만들어서 외부 지점은 클릭 불가능하다는 인상 심어주기
        alert.transform.localScale = Vector3.zero; // 알림 이미지 사이즈 0으로 촉기화하기
        alert.transform.DOScale((Vector3.one), 0.5f); // 알림 이미지 크기 1로 늘리기

        // 보상 알림 UI의 텍스트 및 이미지 수정
        TxtAlertPanel1.text = $"{GetAnimalName(gainedAnimalNum)} 농장에 초대했다!";
        ImgAlertPanel.sprite = animalIcon[gainedAnimalNum];
        TxtAlertPanel2.text = $"<color=#C39B3C><bounce f=2>{gainedGold}골드</color></bounce>를 획득했다!";        
    }

    public void PressExitAlertBtn()
    {
        AlertPanel.GetComponent<Image>().color = new Color(255f, 255f, 255f, 0f);
        alert.transform.DOScale((Vector3.zero), 1f)
            .SetEase(Ease.InBack)
            .OnComplete(() => AlertPanel.SetActive(false));
    }
}
