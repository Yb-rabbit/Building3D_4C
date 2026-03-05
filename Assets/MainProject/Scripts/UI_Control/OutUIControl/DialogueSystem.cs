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
    private List<string> dialogueHistory = new List<string>();
    private const int MaxHistoryCount = 2;
    
    private int currentMaxNodeId = 0;
    private int currentActiveNodeId = 1;
    
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    public UnityEvent onDialogueCompleted;

    void Start()
    {
        if (currentData != null)
        {
            CalculateMaxNodeId();
            StartDialogue(startNodeId);
        }
    }

    void CalculateMaxNodeId()
    {
        currentMaxNodeId = 1;
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

        currentActiveNodeId = nodeId;

        // 1. 更新进度条
        UpdateProgressBar(nodeId);

        // 2. 检查是否到达末端
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

        // 3. 文本显示逻辑（只显示旧历史，最新的打字机输出）
        string currentLine = "";
        if (!string.IsNullOrEmpty(playerChoice))
        {
            currentLine += "> " + playerChoice + "\n";
        }
        currentLine += ">> " + node.terminalText;
        
        dialogueHistory.Add(currentLine);
        
        while (dialogueHistory.Count > MaxHistoryCount)
        {
            dialogueHistory.RemoveAt(0);
        }

        terminalText.text = "";
        
        // 显示旧历史
        for (int i = 0; i < dialogueHistory.Count - 1; i++)
        {
            terminalText.text += dialogueHistory[i] + "\n";
        }
        
        // 最新的使用打字机输出
        typingCoroutine = StartCoroutine(AppendText(currentLine));

        // 4. 生成选项
        CreateChoices(node.choices);
    }

    void UpdateProgressBar(int currentNodeId)
    {
        if (progressBar != null && currentMaxNodeId > 0)
        {
            float progress = (float)currentNodeId / (float)currentMaxNodeId;
            progress = Mathf.Clamp01(progress);
            progressBar.size = progress;
        }
    }

    IEnumerator AppendText(string text)
    {
        isTyping = true;
        foreach (char c in text)
        {
            terminalText.text += c;
            yield return new WaitForSeconds(0.05f);
        }
        
        UpdateContentHeight();
        isTyping = false;
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
        dialogueHistory.Clear();

        if (scrollRect != null)
        {
            RectTransform contentRect = scrollRect.content;
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, 0);
        }

        StartDialogue(currentActiveNodeId, "");
    }
}
