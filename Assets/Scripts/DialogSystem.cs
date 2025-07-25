using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 다이얼로그 시스템 - 여러 캐릭터 간의 대화를 관리하는 클래스
/// </summary>
public class DialogSystem : MonoBehaviour
{
    /// <summary>
    /// 화자(Speaker) 정보를 담는 구조체
    /// </summary>
    [System.Serializable]
    public struct Speaker
    {
        public Image             imageCharacter;   // 캐릭터 이미지 (선명/흐림 상태로 구분)
        public Image             imageDialog;      // 대화창 Image UI
        public TextMeshProUGUI   textName;         // 현재 대사중인 캐릭터 이름 출력 Text UI
        public TextMeshProUGUI   textDialogue;     // 현재 대사 내용 출력 Text UI
        public GameObject        objectArrow;      // 대사가 완료되면 표시되는 커서 오브젝트
    }

    /// <summary>
    /// 대화 데이터를 담는 구조체
    /// </summary>
    [System.Serializable]
    public struct DialogData
    {
        public int      speakerIndex;   // 이 대사를 할 화자를 지정할 DialogSystem의 speakers 배열 인덱스
        public string   name;           // 캐릭터 이름
        [TextArea(3, 5)] // Inspector에서 대사 입력란을 여러 줄로 표시 (최소 3줄, 최대 5줄)
        public string   dialogue;       // 대사
    }

    [Header("화자 정보")]
    [SerializeField]
    private Speaker[] speakers;           // 대화에 참여하는 캐릭터들의 UI 배열

    [Header("대화 내용")]
    [SerializeField]
    private DialogData[] dialogs;         // 순서대로 읽어질 대화 내용 배열

    [Header("설정")]
    [SerializeField]
    private bool isAutoStart = true;      // 자동 시작 여부

    private bool           isFirst = true;             // 최초 1회만 호출하기 위한 변수
    private int            currentDialogIndex = -1;    // 현재 대화 인덱스
    private int            currentSpeakerIndex = 0;    // 현재 말을 하는 화자(Speaker)의 speakers 배열 인덱스
    private float          typingSpeed = 0.1f;         // 텍스트 타이핑 효과의 대사 속도
    private bool           isTypingEffect = false;     // 텍스트 타이핑 효과가 진행중인가

    /// <summary>
    /// 초기화
    /// </summary>
    private void Awake()
    {
        Setup();
    }

    /// <summary>
    /// 초기 설정 - 모든 대화 관련 UI를 비활성화
    /// </summary>
    private void Setup()
    {
        // 모든 대화 관련 게임오브젝트 비활성화
        for (int i = 0; i < speakers.Length; ++i)
        {
            SetActiveObjects(speakers[i], false);
            // 캐릭터 이미지만 페이드된 상태로 활성화
            speakers[i].imageCharacter.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 대화 진행을 업데이트하는 메인 함수
    /// </summary>
    /// <returns>대화가 완전히 끝났으면 true, 아니면 false</returns>
    public bool UpdateDialog()
    {
        // 대화 읽기가 시작된 후 1회만 호출
        if (isFirst == true)
        {
            // 초기화. 캐릭터 이미지를 활성화하고, 대화 관련 UI는 모두 비활성화
            Setup();

            // 자동 시작(isAutoStart=true)으로 설정되어 있으면 첫 번째 대화 시작
            if (isAutoStart)
                SetNextDialog();

            isFirst = false;
        }

        // 마우스 클릭 처리
        if (Input.GetMouseButtonDown(0))
        {
            // 텍스트 타이핑 효과가 진행중일때 마우스 왼쪽 클릭하면 타이핑 효과 중단
            if (isTypingEffect == true)
            {
                isTypingEffect = false;

                // 타이핑 효과를 중단하고, 현재 대사 전체를 출력한다
                StopCoroutine("OnTypingText");
                speakers[currentSpeakerIndex].textDialogue.text = dialogs[currentDialogIndex].dialogue;
                // 대사가 완료되면 표시되는 커서 활성화
                speakers[currentSpeakerIndex].objectArrow.SetActive(true);

                return false;
            }

            // 대사가 아직 더 있을 경우 다음 대화 진행
            if (dialogs.Length > currentDialogIndex + 1)
            {
                SetNextDialog();
            }
            // 대사가 더 이상 없을 경우 모든 오브젝트를 비활성화하고 true 반환
            else
            {
                // 현재 대화에 참여했던 모든 캐릭터, 대화 관련 UI를 화면에 보이지 않게 비활성화
                for (int i = 0; i < speakers.Length; ++i)
                {
                    SetActiveObjects(speakers[i], false);
                    // SetActiveObjects()는 캐릭터 이미지를 건드리지 않는 부분이 있어서 별도로 여기서 호출
                    speakers[i].imageCharacter.gameObject.SetActive(false);
                }

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 다음 대화로 진행
    /// </summary>
    private void SetNextDialog()
    {
        // 현재 화자의 대화 관련 오브젝트 비활성화
        SetActiveObjects(speakers[currentSpeakerIndex], false);

        // 다음 대사를 가리키도록 인덱스 증가
        currentDialogIndex++;

        // 현재 화자 인덱스 설정
        currentSpeakerIndex = dialogs[currentDialogIndex].speakerIndex;

        // 현재 화자의 대화 관련 오브젝트 활성화
        SetActiveObjects(speakers[currentSpeakerIndex], true);
        // 현재 화자 이름 텍스트 설정
        speakers[currentSpeakerIndex].textName.text = dialogs[currentDialogIndex].name;
        // 현재 화자의 대사 텍스트를 타이핑 효과로 출력 시작
        StartCoroutine("OnTypingText");
    }

    /// <summary>
    /// 특정 화자의 UI 요소들을 활성화/비활성화
    /// </summary>
    /// <param name="speaker">대상 화자</param>
    /// <param name="visible">활성화 여부</param>
    private void SetActiveObjects(Speaker speaker, bool visible)
    {
        // 대화 관련 UI 활성화/비활성화
        speaker.imageDialog.gameObject.SetActive(visible);
        speaker.textName.gameObject.SetActive(visible);
        speaker.textDialogue.gameObject.SetActive(visible);

        // 화살표는 대사가 완성되었을 때만 활성화하기 때문에 항상 false
        speaker.objectArrow.SetActive(false);

        // 캐릭터 밝기 조절 (말하는 캐릭터는 밝게, 아닌 캐릭터는 어둡게)
        Color color = speaker.imageCharacter.color;
        color.a = visible == true ? 1 : 0.2f;
        speaker.imageCharacter.color = color;
    }

    /// <summary>
    /// 텍스트 타이핑 효과 - 글자를 하나씩 나타나게 함
    /// </summary>
    /// <returns></returns>
    private IEnumerator OnTypingText()
    {
        int index = 0;
        isTypingEffect = true;

        while (index < dialogs[currentDialogIndex].dialogue.Length)
        {
            speakers[currentSpeakerIndex].textDialogue.text = dialogs[currentDialogIndex].dialogue.Substring(0, index);
            index++;
            yield return new WaitForSeconds(typingSpeed);
        }

        // 마지막 글자까지 출력
        speakers[currentSpeakerIndex].textDialogue.text = dialogs[currentDialogIndex].dialogue;

        isTypingEffect = false;
        speakers[currentSpeakerIndex].objectArrow.SetActive(true);
    }
}



