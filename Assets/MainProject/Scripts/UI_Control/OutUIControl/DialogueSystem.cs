using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI 引用")]
    public Text terminalText;
    public Transform buttonContainer;
    public GameObject buttonPrefab;
    public ScrollRect scrollRect;
    public Button clearButton;

    [Header("进度条设置")]
    public Scrollbar progressBar;
    public GameObject endButton;

    [Header("对话数据")]
    public DialogueData currentData;
    public int startNodeId = 1;

    [Header("显示设置")]
    [Tooltip("玩家选项的颜色")]
    public Color playerColor = Color.yellow;
    [Tooltip("系统/剧情文本的颜色")]
    public Color systemColor = Color.white;

    // --- 内部变量 ---
    private List<string> dialogueHistory = new List<string>(); // 修复：声明dialogueHistory
    private const int MaxHistoryCount = 2;
    private int currentMaxNodeId = 0; // 修复：声明currentMaxNodeId
    private int currentActiveNodeId = 1; // 修复：声明currentActiveNodeId
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool skipTyping = false; // 点击加速标记

    public UnityEvent onDialogueCompleted;

    void Start()
    {
        if (currentData != null)
        {
            CalculateMaxNodeId();
            StartDialogue(startNodeId);
        }

        if (clearButton != null)
        {
            clearButton.onClick.AddListener(ClearHistory);
        }
    }

    void Update()
    {
        // 检测鼠标左键点击，跳过打字机效果
        if (isTyping && Input.GetMouseButtonDown(0))
        {
            skipTyping = true;
        }
    }

    void CalculateMaxNodeId()
    {
        currentMaxNodeId = 1; // 使用已声明的变量
        if (currentData != null && currentData.dialogueList != null)
        {
            foreach (var node in currentData.dialogueList)
            {
                if (node.nodeId > currentMaxNodeId)
                    currentMaxNodeId = node.nodeId;
            }
        }
        else
        {
            currentMaxNodeId = 1;
        }
    }

    public void StartDialogue(int nodeId, string playerChoice = "")
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            isTyping = false;
        }

        DialogueNode node = currentData.GetNodeById(nodeId);
        if (node == null) return;

        currentActiveNodeId = nodeId; // 使用已声明的变量
        UpdateProgressBar(nodeId);

        bool isEnd = (node.choices == null || node.choices.Count == 0);
        if (isEnd)
        {
            if (onDialogueCompleted != null) onDialogueCompleted.Invoke();
            if (endButton != null) endButton.SetActive(true);
        }
        else
        {
            if (endButton != null) endButton.SetActive(false);
        }

        // 1. 生成纯文本（不带标签）
        string pureText = "";
        if (!string.IsNullOrEmpty(playerChoice))
        {
            pureText += $"> {playerChoice}\n"; // 玩家选项（纯文本）
        }
        pureText += $">> {node.terminalText}"; // 系统文本（纯文本）

        // 2. 清除旧内容（只显示最新一行）
        terminalText.text = "";
        dialogueHistory.Clear(); // 使用已声明的变量

        // 3. 启动打字机显示纯文本
        typingCoroutine = StartCoroutine(AppendText(pureText));

        // 4. 打字机结束后，添加颜色标签
        StartCoroutine(AddColorTagsAfterTyping(pureText));

        CreateChoices(node.choices);
    }

    // 【核心修改1】纯文本打字机（不带标签）
    IEnumerator AppendText(string pureText)
    {
        isTyping = true;
        skipTyping = false;
        int i = 0;

        while (i < pureText.Length)
        {
            if (skipTyping)
            {
                terminalText.text += pureText.Substring(i);
                break;
            }

            terminalText.text += pureText[i];
            i++;
            yield return new WaitForSeconds(0.03f); // 打字速度可调整
        }

        isTyping = false;
    }

    // 【核心修改2】打字机结束后，添加颜色标签
    IEnumerator AddColorTagsAfterTyping(string pureText)
    {
        // 等待打字机结束
        while (isTyping)
        {
            yield return null;
        }

        // 生成带标签的文本
        string coloredText = "";
        if (pureText.Contains("\n"))
        {
            string[] lines = pureText.Split('\n');
            if (lines.Length > 0)
            {
                // 玩家选项（第一行）
                if (!string.IsNullOrEmpty(lines[0]))
                {
                    coloredText += $"<color=#{ColorUtility.ToHtmlStringRGBA(playerColor)}>{lines[0]}</color>\n";
                }
                // 系统文本（第二行）
                if (lines.Length > 1 && !string.IsNullOrEmpty(lines[1]))
                {
                    coloredText += $"<color=#{ColorUtility.ToHtmlStringRGBA(systemColor)}>{lines[1]}</color>";
                }
            }
        }
        else
        {
            // 如果没有换行，直接添加系统文本标签
            coloredText = $"<color=#{ColorUtility.ToHtmlStringRGBA(systemColor)}>{pureText}</color>";
        }

        // 更新终端文本为带标签的文本（瞬间生效）
        terminalText.text = coloredText;

        UpdateContentHeight();
    }

    void UpdateProgressBar(int currentNodeId)
    {
        if (progressBar != null && currentMaxNodeId > 0) // 使用已声明的变量
        {
            float progress = (float)currentNodeId / (float)currentMaxNodeId;
            progress = Mathf.Clamp01(progress);
            progressBar.size = progress;
        }
    }

    void UpdateContentHeight()
    {
        Vector2 textSize = terminalText.rectTransform.rect.size;
        TextGenerationSettings settings = terminalText.GetGenerationSettings(textSize);
        settings.font = terminalText.font;
        terminalText.cachedTextGenerator.Populate(terminalText.text, settings);
        
        float textHeight = terminalText.cachedTextGenerator.rectExtents.y * 2;
        RectTransform contentRect = scrollRect.content;
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, textHeight);
        scrollRect.verticalNormalizedPosition = 0f;
    }

    void CreateChoices(List<ChoiceOption> choices)
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        if (choices == null) return;

        foreach (var choice in choices)
        {
            GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
            Button btn = btnObj.GetComponent<Button>();
            Text btnText = btn.GetComponentInChildren<Text>();
            
            if (btnText) btnText.text = "▶ " + choice.optionText;
            
            int targetId = choice.nextNodeId;
            btn.onClick.AddListener(() => StartDialogue(targetId, choice.optionText));
        }
    }

    public void ClearHistory()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        isTyping = false;

        terminalText.text = "";
        dialogueHistory.Clear(); // 使用已声明的变量

        if (scrollRect != null)
        {
            RectTransform contentRect = scrollRect.content;
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, 0);
        }

        StartDialogue(currentActiveNodeId, ""); // 使用已声明的变量
    }
}
