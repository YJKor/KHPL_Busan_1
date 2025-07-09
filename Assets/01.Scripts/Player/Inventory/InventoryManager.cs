using UnityEngine;
using Firebase.Firestore;
using Firebase.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private FirebaseFirestore db;
    private FirebaseAuth auth;
    private string userId;

    // Ŭ���̾�Ʈ���� �����ϴ� �κ��丮 ������ ����Ʈ
    public List<Item> inventory = new List<Item>();

    // �κ��丮 �����Ͱ� ����Ǿ��� �� UI ���� ������Ʈ�ϱ� ���� �̺�Ʈ
    public event Action OnInventoryChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Firebase �ʱ�ȭ
        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;

        // �α��� ���°� ����� ������ userId�� ������Ʈ�ϰ� �κ��丮�� �ε��մϴ�.
        FirebaseAuthManager.Instance.LoginState += HandleAuthStateChanged;
    }

    private void HandleAuthStateChanged(bool isLoggedIn)
    {
        if (isLoggedIn)
        {
            userId = FirebaseAuthManager.Instance.UserId;
            Debug.Log($"�α��� ����. ����� ID: {userId}�� �κ��丮�� �ҷ��ɴϴ�.");
            LoadInventory();
        }
        else
        {
            userId = null;
            inventory.Clear();
            OnInventoryChanged?.Invoke(); // �α׾ƿ� �� �κ��丮 UI Ŭ����
            Debug.Log("�α׾ƿ� ����. �κ��丮�� ���ϴ�.");
        }
    }

    /// <summary>
    /// Firestore���� ���� ������ �κ��丮 �����͸� �ҷ��ɴϴ�.
    /// </summary>
    public async Task LoadInventory()
    {
        if (string.IsNullOrEmpty(userId)) return;

        // ���� ������ inventory �÷��� ��θ� �����մϴ�.
        CollectionReference inventoryRef = db.Collection("users").Document(userId).Collection("inventory");

        try
        {
            QuerySnapshot snapshot = await inventoryRef.GetSnapshotAsync();
            inventory.Clear(); // ���� �κ��丮 �����͸� ���ϴ�.

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    Dictionary<string, object> itemData = document.ToDictionary();
                    Item item = Item.FromDictionary(itemData);
                    inventory.Add(item);
                }
            }
            Debug.Log("�κ��丮 �ε� ����! ������ ����: " + inventory.Count);
            OnInventoryChanged?.Invoke(); // UI ������Ʈ�� ���� �̺�Ʈ ȣ��
        }
        catch (Exception e)
        {
            Debug.LogError("�κ��丮 �ε� ����: " + e.Message);
        }
    }

    /// <summary>
    /// �κ��丮�� �������� �߰��ϰų� ������ ������Ʈ�ϰ� Firestore�� �����մϴ�.
    /// </summary>
    /// <param name="newItem">�߰��� ������</param>
    public async Task AddItem(Item newItem)
    {
        if (string.IsNullOrEmpty(userId)) return;

        // �̹� ���� �������� �ִ��� Ȯ��
        Item existingItem = inventory.Find(item => item.id == newItem.id);

        if (existingItem != null)
        {
            // �������� ������ ������ ����
            existingItem.count += newItem.count;
        }
        else
        {
            // ������ ���� �߰�
            inventory.Add(newItem);
            existingItem = newItem;
        }

        // Firestore�� ����
        DocumentReference itemRef = db.Collection("users").Document(userId).Collection("inventory").Document(existingItem.id);
        await itemRef.SetAsync(existingItem.ToDictionary());

        Debug.Log($"������ '{existingItem.name}' ({existingItem.count}��) �߰�/������Ʈ �Ϸ�.");
        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// �κ��丮���� �������� �����ϰ� Firestore�� �ݿ��մϴ�.
    /// </summary>
    /// <param name="itemId">������ �������� ID</param>
    /// <param name="countToRemove">������ ����</param>
    public async Task RemoveItem(string itemId, int countToRemove = 1)
    {
        if (string.IsNullOrEmpty(userId)) return;

        Item itemToRemove = inventory.Find(item => item.id == itemId);

        if (itemToRemove != null)
        {
            itemToRemove.count -= countToRemove;

            if (itemToRemove.count <= 0)
            {
                // ������ ������ 0 ���ϸ� ����Ʈ���� �����ϰ� Firestore������ ������ ����
                inventory.Remove(itemToRemove);
                DocumentReference itemRef = db.Collection("users").Document(userId).Collection("inventory").Document(itemId);
                await itemRef.DeleteAsync();
                Debug.Log($"������ '{itemToRemove.name}' ��� ���� �Ϸ�.");
            }
            else
            {
                // ������ ������Ʈ�ϰ� Firestore�� �ٽ� ����
                DocumentReference itemRef = db.Collection("users").Document(userId).Collection("inventory").Document(itemId);
                await itemRef.SetAsync(itemToRemove.ToDictionary());
                Debug.Log($"������ '{itemToRemove.name}' {countToRemove}�� ���. ���� ����: {itemToRemove.count}");
            }

            OnInventoryChanged?.Invoke();
        }
    }
}
