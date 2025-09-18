using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    public Camera mainCam;
    public Transform animalSpawnPos; // ���װ� ��ġ�Ǹ� ������ �����ϴ� Transform
    public GameObject uiMainMenus; // ���� ��ư�� ������ ������� ���� �޴� ui ��ҵ�
    public GameObject uiInFields; // ���� ��ġ �� �����ϴ� ui ��ҵ�
    public LogoTween logoTween; // �ΰ� ����� Tween ��ũ��Ʈ

    private int gainedAnimalNum; // ���� ���� ������ ����
    private int gainedGold; // ���� ���� ����
    private int goldCurrent = 0; // ���ݱ��� ���� ���� ����
    private int animalCurrent = 0; // ���ݱ��� ���� ���� ����

    [Header("Egg's Elements")]
    public Button btnEggHatchBtn; // ���� ��ư
    public GameObject Egg;
    public Animator animEgg;
    public TextMeshProUGUI txtHatchMsg;
    public GameObject imgHatchMsg;
    public ParticleSystem ptcEggShakeEffect;
    public ParticleSystem ptcEggHatchEffect;
    private float eggShakeSpeed = 1f;
    private GameObject hatchedAnimal;

    [Header("Animals")]
    public GameObject[] goAnimals; // ���� �𵨵� (�̵��̳� �ǵ��� ���� �κе��� ���� �𵨵�)
    public GameObject[] goAnimalsMoveable;
    public Sprite[] animalIcon;
    public Transform TrMovableAnimalSpawnPos;

    [Header("Circular Wipe Effect")]
    public Image img_circularWipeImg; // ���� ȭ�� ��ȯ ȿ��    

    [Header("Field's Elements")]
    public GameObject goField;
    public GameObject AlertPanel; // �˸� �̹��� �̿��� �ٸ� ������Ʈ���� Ŭ��/��ġ �Է��� ���� ���� ��׶��� �� �׷�ȭ ���� �θ� ������Ʈ
    public Image alert; // �˸� �̹���
    public TextMeshProUGUI TxtCurrentGold; // ���� ���� ���� ��� ���� ǥ���ϴ� �ؽ�Ʈ
    public TextMeshProUGUI TxtCurrentAnimal; // ���� ���� ���� ���� ���� ǥ���ϴ� �ؽ�Ʈ
    public TextMeshProUGUI TxtAlertPanel1; // ���� �˸�â�� �ִ� TMP ������Ʈ
    public TextMeshProUGUI TxtAlertPanel2; // ���� �˸�â�� �ִ� TMP ������Ʈ  
    public Image ImgAlertPanel; // ���� �˸�â�� �ִ� �̹��� ������Ʈ

    // ���� ��ġ ��ư ������� ����Ǵ� �޼���
    public void PressEggHatchBtn()
    {
        // �ϴ� Egg Hatch ��ư ��� �Ͻ��� ��Ȱ��ȭ (�ߺ����� ������ ���� �߻��� ���� ����)
        btnEggHatchBtn.enabled = false;
        // Egg Hatch ��ư�� ��Ȱ��ȭ�� �ð������� �ǵ�����ִ� �Ҹ� Tween ����
        btnEggHatchBtn.gameObject.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.Linear);

        // Egg�� ����Ǿ��ִ� �ִϸ��̼� ����
        animEgg.SetBool("Shake", true);

        // Egg�� �����̸鼭 ��ƼŬ ����Ʈ ����
        ptcEggShakeEffect.Play();

        // �ִϸ������� �ӵ��� 1���� 3���� 5�� ���� Tween ���� ����
        DOTween.To(() => eggShakeSpeed, x =>
        {
            eggShakeSpeed = x;
            animEgg.speed = eggShakeSpeed;
        },3f, 3f).SetEase(Ease.Linear)
        .OnComplete(() =>
        {            
            // ����� �����̼�, �ִϸ��̼� �ӵ�, �ִϸ��̼� �÷��� ���� �ʱ갪���� �������� �Ͻ��� ��Ȱ��ȭ�ϱ�
            animEgg.SetBool("Shake", false);
            eggShakeSpeed = 1f;
            Egg.transform.localRotation = Quaternion.identity;
            Egg.SetActive(false);
            
            // ��ƼŬ ����Ʈ ���� / ���� ��
            ptcEggShakeEffect.Stop();
            ptcEggHatchEffect.Play();

            // ���� ��ȯ �޼��� ����
            SpawnAnimals();
        });
    }

    // ���� ��ġ �޼���
    private void SpawnAnimals()
    {
        // �����ϰ� ���� �� Ư�� ��ġ�� ����
        gainedAnimalNum = Random.Range(0, goAnimals.Length);
        hatchedAnimal = Instantiate(goAnimals[gainedAnimalNum], animalSpawnPos);

        // ��� ȹ��
        gainedGold = Random.Range(1, 6) * 5;
        goldCurrent += gainedGold;

        // �ʵ忡�� ��� ���� ������ ��ȯ�ϱ�
        GameObject go = Instantiate(goAnimalsMoveable[gainedAnimalNum], TrMovableAnimalSpawnPos.position, Quaternion.identity);
        go.GetComponent<AnimalRandomMove>().enabled = true;

        // !!! ���⿡�� �ؽ�Ʈ ���⵵ �ֱ� !!!
        imgHatchMsg.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);
        txtHatchMsg.text = $"��! {GetAnimalName(gainedAnimalNum)} ȹ���ߴ�!";

        // ���� �� ���� �� ũ�� xyz�� ���� 0.1�� �ʱ�ȭ
        hatchedAnimal.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        // �������� Tween �帧 ����
        Sequence sq = DOTween.Sequence();
        sq.Append(hatchedAnimal.transform.DOScale(Vector3.one, 0.75f).SetEase(Ease.OutBack));
        sq.Join(hatchedAnimal.transform.DOLocalRotate(new Vector3(0f, 580f, 0f), 0.75f, RotateMode.FastBeyond360));

        // 2���� ���� �� ȭ�� ��ȯ ȿ�� ���� (���� ������ ����)
        sq.AppendInterval(2f);
        sq.AppendCallback(() => CircularWipeEffectOn());        
    }

    // ���� �̸� �ؽ�Ʈ�� �޾ƿ���
    private string GetAnimalName(int animal)
    {
        if (animal == 0) return "<color=purple><bounce>������</bounce></color>��";
        else if (animal == 1) return "<color=red><bounce>�ϸ�</bounce></color>��";
        else if (animal == 2) return "<color=purple><bounce>����ġ</bounce></color>��";
        else if (animal == 3) return "<color=blue><bounce>����</bounce></color>��";
        else if (animal == 4) return "<color=blue><bounce>�ڳ���</bounce></color>��";
        else if (animal == 5) return "<color=purple><bounce>�⸰</bounce></color>��";
        else if (animal == 6) return "<color=red><bounce>��</bounce></color>��";
        else if (animal == 7) return "<color=purple><bounce>��</bounce></color>��";
        else return "NULL";
    }

    // ���� ȭ�� ��ȯ ȿ�� �޼���
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

    // ���� ȭ�� ��ȯ ȿ�� ���� �޼���
    private void CircularWipeEffectOff()
    {
        // �˿��� ��ȭ�� ���� ���ֱ� 
        Destroy(hatchedAnimal);

        // ���θ޴� UI�� active false�ϰ�
        logoTween.EndTween(); // Ȥ�� �� ��쿡 ����� �ΰ��� ���� ���� Ʈ���� �����س���
        imgHatchMsg.transform.localScale = Vector3.zero;
        txtHatchMsg.text = "";
        animalSpawnPos.gameObject.SetActive(false);
        uiMainMenus.SetActive(false);

        // �ʵ� active true �س���
        goField.SetActive(true);

        // ������ 1 : ���� ȭ�� ��ȯ ȿ�� ���ֱ�
        float imgFill = 1f;

        Sequence sq1 = DOTween.Sequence();
        sq1.Append(DOTween.To(() => imgFill, x =>
        {
            imgFill = x;
            img_circularWipeImg.fillAmount = imgFill;
        }, 0f, 1f).SetEase(Ease.Linear).OnComplete(() => img_circularWipeImg.gameObject.SetActive(false)));

        // ������ 2 : ī�޶� �ܾ� ȿ�� ���� FOV �� ���� -> �˸� UI ���
        float camFov = 90f;

        Sequence sq2 = DOTween.Sequence();        
        sq2.Append(DOTween.To(() => camFov, x =>
        {
            camFov = x;
            mainCam.fieldOfView = camFov;
        }, 60f, 1f).SetEase(Ease.Linear).OnComplete(() => InFieldUITweens()));
        // ���� ȭ�� ��ȯ ȿ�� & ����Ʈ ����� �Ŀ� �� UI�� ���� ���� �޼��� ����               
    }

    // ���ʵ� UI���� ���� + �˸� �޽��� ����ϱ�
    private void InFieldUITweens()
    {
        UITweens[] tweens = new UITweens[uiInFields.transform.childCount];

        for (int i = 0; i < tweens.Length; i++)
        {
            tweens[i] = uiInFields.transform.GetChild(i).GetComponent<UITweens>();
            tweens[i].transform.gameObject.SetActive(true);
            tweens[i].ActivateTween();
        }

        // ������ ���� & UI�� �ݿ��ϴ� �۾� ���� (��� & ���� ���� ���� ���� �� ����)        
        TxtCurrentGold.text = (goldCurrent).ToString();
        TxtCurrentAnimal.text = (++animalCurrent).ToString();

        // ���� �˸� â ��� �޼��� ����
        ShowAlert();
    }

    // ���ʵ忡�� ���� �޴��� ���ư���
    public void PressBack2HatchBtn()
    {
        // �켱 ���� ȭ���� UI���� ���� �ٱ����� �������� Ʈ�� �����ϱ�
        UITweens[] tweens = new UITweens[uiInFields.transform.childCount];

        for (int i = 0; i < uiInFields.transform.childCount; i++)
        {
            tweens[i] = uiInFields.transform.GetChild(i).GetComponent<UITweens>();
            tweens[i].ActivateReverseTween();
        }

        // �������� ���� �޴� UI�鵵 �ٽ� activew true�� �ǵ����� ����� �ΰ� Ʈ�� ������ϱ�
        uiMainMenus.SetActive(true);
        logoTween.RestartTween();

        // Egg Hatch ��ư�� ������ �Ͻ��� ��Ȱ��ȭ���ѵ״� Egg Hatch ��ư�� �ٽ� enable �ϱ�
        btnEggHatchBtn.enabled = true;
        // Egg Hatch ��ư���� ��Ȱ��ȭ �ð��� �ǵ���� �ִ� Tween �ֱ�
        btnEggHatchBtn.gameObject.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);

        // Egg �̹����� �ٽ� ��Ȱ��ȭ�ϸ鼭 �ð��� �ǵ�� �ֱ�
        Egg.SetActive(true);
        Egg.transform.localScale = Vector3.zero;
        Egg.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);

        // Egg Hatch�Ҷ� ������ ����Ʈ�鵵 �ٽ� ��Ȱ��ȭ
        animalSpawnPos.gameObject.SetActive(true);
    }

    // ���� �˸� â �⎶ �޼���
    private void ShowAlert()
    {
        // ���� �˸� UI ��� �غ�
        AlertPanel.SetActive(true); // �˸� �̹��� ��׶��� active true�� ���� �ٸ� �Է� ����
        AlertPanel.GetComponent<Image>().color = new Color(255f, 255f, 255f, 0.25f); // �˸� �̹��� ��׶���� ��¦ �������ϰ� ���� �ܺ� ������ Ŭ�� �Ұ����ϴٴ� �λ� �ɾ��ֱ�
        alert.transform.localScale = Vector3.zero; // �˸� �̹��� ������ 0���� �˱�ȭ�ϱ�
        alert.transform.DOScale((Vector3.one), 0.5f); // �˸� �̹��� ũ�� 1�� �ø���

        // ���� �˸� UI�� �ؽ�Ʈ �� �̹��� ����
        TxtAlertPanel1.text = $"{GetAnimalName(gainedAnimalNum)} ���忡 �ʴ��ߴ�!";
        ImgAlertPanel.sprite = animalIcon[gainedAnimalNum];
        TxtAlertPanel2.text = $"<color=#C39B3C><bounce f=2>{gainedGold}���</color></bounce>�� ȹ���ߴ�!";        
    }

    public void PressExitAlertBtn()
    {
        AlertPanel.GetComponent<Image>().color = new Color(255f, 255f, 255f, 0f);
        alert.transform.DOScale((Vector3.zero), 1f)
            .SetEase(Ease.InBack)
            .OnComplete(() => AlertPanel.SetActive(false));
    }
}
