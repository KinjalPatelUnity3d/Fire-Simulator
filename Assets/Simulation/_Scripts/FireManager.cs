using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class FireManager : MonoBehaviour
{
    public static FireManager Instance;
    public DragObject DragObjectInstance;
    public Transform CameraObj;
    public Transform CameraInitial;
    public Transform CameraZoom1;
    public Transform CameraZoomAtPin;
    public Transform CameraZoomAtFire;
    public GameObject FireExtenguisherObj;
    public GameObject PinHandObj;
    public bool IsAtCorrectPos;
    public GameObject PinObj;
    public GameObject Left_Hand;
    public GameObject Right_Hand;
    public GameObject IntroScreen;

    [Header("#### INSTRUCTION ####")]
    public GameObject InstructionPanel;
    public TextMeshProUGUI InstructionText;
    public GameObject WarningPanel;
    public TextMeshProUGUI WarningPanelText;
    public GameObject CompletionPanel;

    [Header("#### ARROWS ####")]
    public GameObject Exting_Grab_Arrow;
    public GameObject Green_Arrow;
    public GameObject PinPull_Arrow;
    public GameObject Safe_DistanceObj;

    [Header("#### SQUEEZE ####")]
    public bool CanSqueeze;
    public bool IsSqueezed;
    public GameObject Thumb1_r;
    public GameObject Thumb1_r_squeezed;
    public GameObject Thumb1_r_Unsqueezed;
    public GameObject Lever;
    public GameObject LeverSqueezed;
    public GameObject LeverUnSqueezed;
    public float speed;
    public GameObject SqueezeArrow;
    //public int SqueezeCounter = 0;

    [Header("#### NOZZLE ####")]
    public bool CanSpray;
    public Slider RotationSlider;
    public GameObject NozzleObj;
    public ParticleSystem SprayParticle;
    public ParticleSystem FireParticle;
    public float FireParticleStartSize;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {

    }

    public void MoveCameraToTheFire()
    {
        IntroScreen.SetActive(false);
        Sequence s = DOTween.Sequence();
        s.Append(CameraObj.DOLocalMove(CameraZoom1.localPosition, 3f).SetEase(Ease.Linear)).OnComplete(OnCamReached);
    }

    public void OnCamReached()
    {
        print("OnCamReached");
        DragObjectInstance.enabled = true;
        FireExtenguisherObj.GetComponent<BoxCollider>().enabled = true;
        Safe_DistanceObj.SetActive(true);
        Exting_Grab_Arrow.SetActive(true);
        ShowInstructionPanel();
        Invoke("HideInstructionPanel",5);
    }

    public void OnFireExtinguisherGrabbed()
    {
        CancelInvoke("HideInstructionPanel");
        Exting_Grab_Arrow.SetActive(false);
        Green_Arrow.SetActive(true);
        ShowInstructionPanel();
        InstructionText.text = "Drag the fire extinguisher and drop near the green arrow";
    }

    public void OnFireExtinguisherDropped()
    {
        DragObjectInstance.gameObject.GetComponent<BoxCollider>().enabled = false;
        DragObjectInstance.enabled = false;
        Green_Arrow.SetActive(false);
        FireExtenguisherObj.transform.localPosition = new Vector3(0.7f, -0.1f, 6);
        MoveCameraToPin();
    }

    void MoveCameraToPin()
    {
        InstructionText.text = "Great.. Now pull the pin";
        Sequence s = DOTween.Sequence();
        s.Append(CameraObj.DOLocalMove(CameraZoomAtPin.localPosition, 4f).SetEase(Ease.Linear)).OnComplete(OnReachedAtPin);

        Sequence Rotate_s = DOTween.Sequence();
        Rotate_s.Append(CameraObj.DOLocalRotate(CameraZoomAtPin.localEulerAngles, 4f).SetEase(Ease.Linear));
    }

    void OnReachedAtPin()
    {
        print("OnReachedAtPin");
        PinPull_Arrow.SetActive(true);
        PinHandObj.SetActive(true);
        PinObj.GetComponent<BoxCollider>().enabled = true;
    }

    public void OnPinPulled()
    {
        PinPull_Arrow.SetActive(false);
        PinHandObj.SetActive(false);
        MoveCameraToFire();
    }

    void MoveCameraToFire()
    {
        InstructionText.text = "Awesome..Now lets press the handle to spray";
        Sequence s = DOTween.Sequence();
        s.Append(CameraObj.DOLocalMove(CameraZoomAtFire.localPosition, 4f).SetEase(Ease.Linear)).OnComplete(OnReachedAtFire);

        Sequence Rotate_s = DOTween.Sequence();
        Rotate_s.Append(CameraObj.DOLocalRotate(CameraZoomAtFire.localEulerAngles, 4f).SetEase(Ease.Linear));
    }

    void OnReachedAtFire()
    {
        print("OnReachedAtFire");
        Left_Hand.SetActive(true);
        Right_Hand.SetActive(true);
        SqueezeArrow.SetActive(true);
        Invoke("ShowSqueezInstruction", 2);
    }

    void ShowSqueezInstruction()
    {
        InstructionText.text = "Click on handle to Squeeze / Unsqueeze";
        CanSqueeze = true;
    }

    void ShowSprayInstruction()
    {
        InstructionText.text = "Drag nozzle up or down to aim at fire.Fire will go off slowly as you point correctly";
        CanSpray = true;
        RotationSlider.gameObject.SetActive(true);
    }


    public void ShowInstructionPanel()
    {
        InstructionPanel.SetActive(true);
    }

    public void HideInstructionPanel()
    {
        InstructionPanel.SetActive(false);
    }

    private void Update()
    {

        Vector3 FireExtenguisherPos = FireExtenguisherObj.transform.localPosition;
        if (FireExtenguisherPos.x<1 && FireExtenguisherPos.x > 0.2f)
        {
            IsAtCorrectPos = true;
        }
        else
        {
            IsAtCorrectPos = false;
        }

        if (FireExtenguisherPos.x < 0.6f)
        {
            WarningPanel.SetActive(true);
        }
        else
        {
            WarningPanel.SetActive(false);
        }


        if (CanSpray)
        {
            RotationSlider.gameObject.SetActive(true);
            Vector3 tmpAngle = NozzleObj.transform.localEulerAngles;
            tmpAngle.y = 0;
            tmpAngle.z = 0;
            tmpAngle.x = RotationSlider.value;
            NozzleObj.transform.localEulerAngles = tmpAngle;
            if(RotationSlider.value>200 && IsSqueezed)
            {
                if(FireParticleStartSize>0)
                {
                    FireParticleStartSize = FireParticleStartSize - (Time.deltaTime*0.2f);
                    FireParticle.gameObject.transform.Translate(Vector3.back *( Time.deltaTime*0.1f));
                    var main = FireParticle.main;
                    main.startSize = FireParticleStartSize;
                    if(FireParticleStartSize<=0)
                    {
                        FireParticle.gameObject.transform.parent.gameObject.SetActive(false);
                        InstructionPanel.SetActive(false);
                        CompletionPanel.SetActive(true);
                        Safe_DistanceObj.SetActive(false);
                    }
                }                    
            }

        }
    }

    public void Squeeze_UnSqueeze()
    {
        if (CanSqueeze)
        {
            IsSqueezed = !IsSqueezed;
            SqueezeArrow.SetActive(false);
            if (IsSqueezed)
            {
                Sequence Squeeze_s = DOTween.Sequence();
                Squeeze_s.Append(Lever.transform.DOLocalRotate(LeverSqueezed.transform.localEulerAngles,0.2f).
                                SetEase(Ease.Linear)).OnComplete(OnSqueezeCompleted);


                Sequence Squeeze_sT = DOTween.Sequence();
                Squeeze_sT.Append(Thumb1_r.transform.DOLocalRotate(Thumb1_r_squeezed.transform.localEulerAngles, 0.2f).SetEase(Ease.Linear));
                SprayParticle.Play();
                ShowSprayInstruction();
            }
            else
            {
                Sequence UnSqueeze_s = DOTween.Sequence();
                UnSqueeze_s.Append(Lever.transform.DOLocalRotate(LeverUnSqueezed.transform.localEulerAngles, 0.2f).
                                SetEase(Ease.Linear)).OnComplete(OnUnSqueezeCompleted);

                Sequence UnSqueeze_sT = DOTween.Sequence();
                UnSqueeze_sT.Append(Thumb1_r.transform.DOLocalRotate(Thumb1_r_Unsqueezed.transform.localEulerAngles, 0.2f).SetEase(Ease.Linear));
                SprayParticle.Stop();
            }
        }
    }

    void OnSqueezeCompleted()
    {

    }

    void OnUnSqueezeCompleted()
    {

    }

    public void RestartScene()
    {
        DOTween.KillAll();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainGameScene");
    }


}
