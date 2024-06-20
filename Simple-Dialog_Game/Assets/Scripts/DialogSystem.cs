using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    [SerializeField]
    // 대화에 참여하는 캐릭터들의 UI 배열
    private Speaker[] speakers;

    [SerializeField]
    // 현재 분기의 대사 목록 배열
    private DialogData[] dialogs;

    [SerializeField]
    // 자동 시작 여부
    private bool isAutoStart = true;

    // 최초 1회 호출하기 위한 변수
    private bool isFirst = true;
    // 현재 대사 순번
    private int currentDialogIndex = -1;
    // 현재 말을 하는 화자(Speaker)의 speakers 배열 순번
    private int currentSpeakerIndex = 0;
    // 텍스트 타이핑 효과의 재생 속도
    private float typingSpeed = 0.1f;
    // 텍스트 타이핑 효과를 재생중인지
    private bool isTypingEffect = false;

    private void Awake()
    {
        Setup();
    }

    private void Setup()
    {
        // 모든 대화 관련 
        for (int i = 0; i < speakers.Length; i++)
        {
            SetActiveObjects(speakers[i], false);

            speakers[i].spriteRenderer.gameObject.SetActive(true);
        }
    }

    public bool UpdateDialog()
    {
        // 대사 분기가 시작될 때 1회만 호출
        if(isFirst == true)
        {
            // 초기화. 캐릭터 이미지는 활성화하고,
            // 대사 관련 UI는 모두 비활성화
            Setup();

            // 자동 재생(isAutoStart == true)으로 설정되어 있으면
            // 첫 번째 대사 재생
            if (isAutoStart)
            {
                SetNextDialog();
            }

            isFirst = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if( isTypingEffect == true)
            {
                // 타이핑 효과를 중지하고, 현재 대사를 전체 출력한다
                isTypingEffect = false;

                StopCoroutine("OnTypingText");
                speakers[currentSpeakerIndex].textDialogue.text = dialogs[currentDialogIndex].dialogue;
                // 대사가 완료되었을 때 출력되는 커서 활성화
                speakers[currentSpeakerIndex].objectArrow.SetActive(true);

                return false;
            }

            // 대사가 남아있을 경우 다음 대사 진행
            if (dialogs.Length > currentDialogIndex + 1)
            {
                SetNextDialog();
            }
            // 대사가 더 이상 없을 경우
            // 모든 오브젝트를 비활성화하고 true 반환
            else
            {
                // 현재 대화에 참여했던 모든 캐릭터,
                // 
                for(int i = 0; i < speakers.Length; i++)
                {
                    SetActiveObjects(speakers[i], false);

                    speakers[i].spriteRenderer.gameObject.SetActive(false);
                }
                return true;
            }
        }
        return false;
    }

    private void SetNextDialog()
    {
        // 이전 화자의 대화 관련 오브젝트 비활성화
        SetActiveObjects(speakers[currentSpeakerIndex], false);

        // 다음 대사를 진행하도록
        currentDialogIndex++;

        // 현재 화자 순번 설정
        currentSpeakerIndex = dialogs[currentDialogIndex].speakerIndex;

        // 현재 화자의 대화 관련 오브젝트 활성화
        SetActiveObjects(speakers[currentSpeakerIndex], true);
        // 현재 화자 이름 텍스트 설정
        speakers[currentSpeakerIndex].textName.text = dialogs[currentDialogIndex].name;
        // 현재 화자의 대사 텍스트 설정
        StartCoroutine("OnTypingText");
    }

    private void SetActiveObjects(Speaker speaker, bool visible)
    {
        speaker.imageDialog.gameObject.SetActive(visible);
        speaker.textName.gameObject.SetActive(visible);
        speaker.textDialogue.gameObject.SetActive(visible);

        speaker.objectArrow.SetActive(false);
    }

    private IEnumerator OnTypingText()
    {
        int index = 0;

        isTypingEffect = true;

        // 텍스트를 한글자씩 타이핑치듯 재생
        while(index <= dialogs[currentDialogIndex].dialogue.Length)
        {
            speakers[currentSpeakerIndex].textDialogue.text = dialogs[currentDialogIndex].dialogue.Substring(0, index);
            
            index++;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTypingEffect = false;
        
        // 대사가 완료되었을 때 출력되는 커서 활성화
        speakers[currentSpeakerIndex].objectArrow.SetActive(true);
    }
}

[System.Serializable]
public struct Speaker
{
    // 캐릭터 이미지 (청자 / 화자 알파값 제어)
    public SpriteRenderer spriteRenderer;

    // 대화창 Image UI
    public Image imageDialog;

    // 현재 대사중인 캐릭터 이름 출력 Text UI
    public Text textName;

    public Text textDialogue;

    // 대사가 완료되었을 때 출력되는 커서 오브젝트
    public GameObject objectArrow;
}

[System.Serializable]
public struct DialogData
{
    // 이름과 대사를 출력할 현재 DialogSystem의 speaker 배열 순번
    public int speakerIndex;

    // 캐릭터 이름
    public string name;
    [TextArea(3, 5)]

    // 대사
    public string dialogue;
}