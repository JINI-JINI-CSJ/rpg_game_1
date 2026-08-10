using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 대화(다이얼로그) 타이핑 출력 시스템
/// - 시간차로 한 글자씩 출력
/// - 외부 입력이 들어오면 현재 문장 즉시 전부 출력
/// - 문장 다 출력되면 마지막에 점멸 캐럿 표시
/// - string 리스트를 순서대로 단계적으로 처리
/// - TextMeshPro(TMP_Text)와 유니티 기본 UI.Text 둘 다 지원
///   (둘 중 사용할 쪽에만 참조를 연결하면 됩니다. 둘 다 비워두면 안 됩니다.)
/// </summary>
public class DialogueTyper : MonoBehaviour
{
    [Header("본문 텍스트 - 둘 중 하나만 연결")]
    [SerializeField] private TMP_Text tmpDialogueText;
    [SerializeField] private Text legacyDialogueText;

    [Header("캐럿 텍스트 - 둘 중 하나만 연결 (본문과 같은 타입 사용 권장)")]
    [SerializeField] private TMP_Text tmpCaretText;
    [SerializeField] private Text legacyCaretText;

    [Header("옵션")]
    [SerializeField] private float charInterval = 0.03f;      // 글자 하나당 딜레이
    [SerializeField] private float caretBlinkInterval = 0.5f; // 캐럿 점멸 주기
    [SerializeField] private string caretChar = "▌";
    [SerializeField] private bool autoStartOnEnable = false;

    private List<string> lines = new List<string>();
    private int currentIndex = -1;

    private Coroutine typingRoutine;
    private Coroutine caretRoutine;

    private bool isTyping = false;       // 현재 한 글자씩 출력 중인지
    private bool isLineFinished = false; // 현재 문장이 완전히 출력됐는지
    public bool IsDialogueActive { get; private set; } = false;

    // 필요하면 다이얼로그 종료 시 구독해서 처리
    public System.Action OnDialogueFinished;
    public System.Action<int, string> OnLineStarted; // (인덱스, 문장)

    private void Awake()
    {
        ValidateReferences();
        SetCaretGameObjectActive(false);
    }

    private void ValidateReferences()
    {
        if (tmpDialogueText == null && legacyDialogueText == null)
            Debug.LogError("[DialogueTyper] tmpDialogueText 또는 legacyDialogueText 중 하나는 반드시 연결해야 합니다.", this);

        if (tmpCaretText == null && legacyCaretText == null)
            Debug.LogWarning("[DialogueTyper] 캐럿 텍스트가 연결되지 않았습니다. 캐럿 점멸이 표시되지 않습니다.", this);
    }

    private void Update()
    {
        if (!IsDialogueActive) return;

        // 외부 입력 감지 (원하는 입력 방식으로 교체 가능)
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            OnInput();
        }
    }

    /// <summary>
    /// 새 대화 시작. 메시지 리스트를 넘기면 처음부터 순차 재생.
    /// </summary>
    public void StartDialogue(List<string> messages)
    {
        StopAllCoroutines();
        lines = messages;
        currentIndex = -1;
        IsDialogueActive = true;

        SetCaretGameObjectActive(false);

        NextLine();
    }

    /// <summary>
    /// 외부 입력 처리 진입점.
    /// - 타이핑 중이면: 즉시 전체 출력
    /// - 타이핑 끝난 상태면: 다음 문장으로
    /// </summary>
    public void OnInput()
    {
        if (isTyping)
        {
            CompleteLineInstantly();
        }
        else if (isLineFinished)
        {
            NextLine();
        }
    }

    private void NextLine()
    {
        currentIndex++;

        if (currentIndex >= lines.Count)
        {
            EndDialogue();
            return;
        }

        OnLineStarted?.Invoke(currentIndex, lines[currentIndex]);

        if (typingRoutine != null) StopCoroutine(typingRoutine);
        typingRoutine = StartCoroutine(TypeLine(lines[currentIndex]));
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        isLineFinished = false;
        SetCaretGameObjectActive(false);

        SetDialogueDisplay("");
        string buffer = "";
        foreach (char c in line)
        {
            buffer += c;
            SetDialogueDisplay(buffer);
            yield return new WaitForSeconds(charInterval);
        }

        FinishLine(line);
    }

    /// <summary>
    /// 입력이 들어왔을 때 타이핑 중이던 문장을 즉시 전부 출력.
    /// </summary>
    private void CompleteLineInstantly()
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        FinishLine(lines[currentIndex]);
    }

    private void FinishLine(string line)
    {
        SetDialogueDisplay(line);
        isTyping = false;
        isLineFinished = true;
        SetCaretGameObjectActive(true);
    }

    private void EndDialogue()
    {
        Debug.Log( "다이얼로그 EndDialogue " );
        IsDialogueActive = false;
        isTyping = false;
        isLineFinished = false;
        SetCaretGameObjectActive(false);
        OnDialogueFinished?.Invoke();
    }

    // ---------- 본문 텍스트 출력 (TMP / 기본 UI 공용 처리) ----------

    private void SetDialogueDisplay(string text)
    {
        if (tmpDialogueText != null) tmpDialogueText.text = text;
        if (legacyDialogueText != null) legacyDialogueText.text = text;
    }

    // ---------- 캐럿 점멸 처리 (TMP / 기본 UI 공용 처리) ----------

    private GameObject CaretGameObject =>
        tmpCaretText != null ? tmpCaretText.gameObject :
        legacyCaretText != null ? legacyCaretText.gameObject : null;

    private void SetCaretGameObjectActive(bool active)
    {
        var go = CaretGameObject;
        if (go == null) return;

        if (caretRoutine != null)
        {
            StopCoroutine(caretRoutine);
            caretRoutine = null;
        }

        if (active)
        {
            if (tmpCaretText != null) tmpCaretText.text = caretChar;
            if (legacyCaretText != null) legacyCaretText.text = caretChar;
            go.SetActive(true);
            caretRoutine = StartCoroutine(BlinkCaret(go));
        }
        else
        {
            go.SetActive(false);
        }
    }

    private IEnumerator BlinkCaret(GameObject caretGO)
    {
        while (true)
        {
            caretGO.SetActive(!caretGO.activeSelf);
            yield return new WaitForSeconds(caretBlinkInterval);
        }
    }

    private void OnEnable()
    {
        if (autoStartOnEnable && lines.Count > 0)
            StartDialogue(lines);
    }

    // 커브 판넬 닫기 이벤트
    public void ClosePopup_EndAni()
    {
        Debug.Log( "ClosePopup_EndAni====================================" );
        if(tmpDialogueText != null)     tmpDialogueText.text = "";
        if(legacyDialogueText != null)  legacyDialogueText.text = "";
    }
}
