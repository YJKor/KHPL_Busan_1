using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;

[System.Serializable]
public class Dialogue
{
    public int id;
    public string character;
    public string text;
    public List<Choice> options;
}

[System.Serializable]
public class Choice
{
    public string option;
    public int nextId; // 선택지 선택 후 이동할 대사의 인덱스
}

[System.Serializable]
public class DialogueList
{
    public List<Dialogue> dialogues;
}

public class DialogueManager : MonoBehaviour
{
    public Text characterNameText;
    public Text dialogueText;
    public GameObject textPanelCanvas; // 선택지 버튼들을 담는 패널
    public Button choiceButtonPrefab; // 선택지 버튼 프리팹

    public GameObject dialoguePanel; // 대사창 전체 패널

    private DialogueList dialogueList;
    private Dictionary<int, Dialogue> dialogueDict;
    private int currentDialogueIndex = 0;

    void Start()
    {
        LoadDialogueData();
        // 처음에는 대화창 숨김
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    /// <summary>
    /// 외부에서 호출: 대화 시작 (대사창 UI On + 대화 처음부터)
    /// </summary>
    public void ShowDialoguePanel()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);
        currentDialogueIndex = 0;
        DisplayNextDialogue();
    }
    void LoadDialogueData()
    {
        // Resources 폴더에서 JSON 파일을 읽어옴
        TextAsset jsonData = Resources.Load<TextAsset>("Dialogues/dialogue_Remy");
        if (jsonData == null)
        {
            Debug.LogError("JSON 파일을 찾을 수 없습니다!");
            return;
        }
        dialogueList = JsonUtility.FromJson<DialogueList>(jsonData.text);

        // Dictionary로 변환
        dialogueDict = new Dictionary<int, Dialogue>();
        foreach (Dialogue node in dialogueList.dialogues)
        {
            dialogueDict.Add(node.id, node);
        }
    }

    public void DisplayNextDialogue()
    {
        if (currentDialogueIndex < dialogueList.dialogues.Count)
        {
            Dialogue dialogue = dialogueList.dialogues[currentDialogueIndex];
            characterNameText.text = dialogue.character;
            dialogueText.text = dialogue.text;

        if (dialogue.options != null && dialogue.options.Count > 0)
            {
                ShowChoices(dialogue.options);
            }
            else
            {
                HideChoices();
                currentDialogueIndex++;
            }
        }
        else
        {
            Debug.Log("모든 대화를 마쳤습니다.");
        }
    }

    void ShowChoices(List<Choice> choices)
    {
        textPanelCanvas.SetActive(true);
        foreach (Transform child in textPanelCanvas.transform)
        {
            Destroy(child.gameObject); // 기존 선택지 삭제
        }

        foreach (Choice choice in choices)
        {
            Button choiceButton = Instantiate(choiceButtonPrefab, textPanelCanvas.transform);
            choiceButton.GetComponentInChildren<Text>().text = choice.option;
            choiceButton.onClick.AddListener(() => OnChoiceSelected(choice.nextId));
        }
    }

    void HideChoices()
    {
        textPanelCanvas.SetActive(false);
    }

    void OnChoiceSelected(int nextIdIndex)
    {
        currentDialogueIndex = nextIdIndex;
        DisplayNextDialogue();
    }
}
