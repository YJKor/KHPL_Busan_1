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
    public int nextId; // ������ ���� �� �̵��� ����� �ε���
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
    public GameObject textPanelCanvas; // ������ ��ư���� ��� �г�
    public Button choiceButtonPrefab; // ������ ��ư ������

    public GameObject dialoguePanel; // ���â ��ü �г�

    private DialogueList dialogueList;
    private Dictionary<int, Dialogue> dialogueDict;
    private int currentDialogueIndex = 0;

    void Start()
    {
        LoadDialogueData();
        // ó������ ��ȭâ ����
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    /// <summary>
    /// �ܺο��� ȣ��: ��ȭ ���� (���â UI On + ��ȭ ó������)
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
        // Resources �������� JSON ������ �о��
        TextAsset jsonData = Resources.Load<TextAsset>("Dialogues/dialogue_Remy");
        if (jsonData == null)
        {
            Debug.LogError("JSON ������ ã�� �� �����ϴ�!");
            return;
        }
        dialogueList = JsonUtility.FromJson<DialogueList>(jsonData.text);

        // Dictionary�� ��ȯ
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
            Debug.Log("��� ��ȭ�� ���ƽ��ϴ�.");
        }
    }

    void ShowChoices(List<Choice> choices)
    {
        textPanelCanvas.SetActive(true);
        foreach (Transform child in textPanelCanvas.transform)
        {
            Destroy(child.gameObject); // ���� ������ ����
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
