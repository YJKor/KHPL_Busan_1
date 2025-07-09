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

    // 클라이언트에서 관리하는 인벤토리 데이터 리스트
    public List<Item> inventory = new List<Item>();

    // 인벤토리 데이터가 변경되었을 때 UI 등을 업데이트하기 위한 이벤트
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
        // Firebase 초기화
        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;

        // 로그인 상태가 변경될 때마다 userId를 업데이트하고 인벤토리를 로드합니다.
        FirebaseAuthManager.Instance.LoginState += HandleAuthStateChanged;
    }

    private void HandleAuthStateChanged(bool isLoggedIn)
    {
        if (isLoggedIn)
        {
            userId = FirebaseAuthManager.Instance.UserId;
            Debug.Log($"로그인 성공. 사용자 ID: {userId}의 인벤토리를 불러옵니다.");
            LoadInventory();
        }
        else
        {
            userId = null;
            inventory.Clear();
            OnInventoryChanged?.Invoke(); // 로그아웃 시 인벤토리 UI 클리어
            Debug.Log("로그아웃 상태. 인벤토리를 비웁니다.");
        }
    }

    /// <summary>
    /// Firestore에서 현재 유저의 인벤토리 데이터를 불러옵니다.
    /// </summary>
    public async Task LoadInventory()
    {
        if (string.IsNullOrEmpty(userId)) return;

        // 현재 유저의 inventory 컬렉션 경로를 지정합니다.
        CollectionReference inventoryRef = db.Collection("users").Document(userId).Collection("inventory");

        try
        {
            QuerySnapshot snapshot = await inventoryRef.GetSnapshotAsync();
            inventory.Clear(); // 기존 인벤토리 데이터를 비웁니다.

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    Dictionary<string, object> itemData = document.ToDictionary();
                    Item item = Item.FromDictionary(itemData);
                    inventory.Add(item);
                }
            }
            Debug.Log("인벤토리 로딩 성공! 아이템 개수: " + inventory.Count);
            OnInventoryChanged?.Invoke(); // UI 업데이트를 위해 이벤트 호출
        }
        catch (Exception e)
        {
            Debug.LogError("인벤토리 로딩 실패: " + e.Message);
        }
    }

    /// <summary>
    /// 인벤토리에 아이템을 추가하거나 개수를 업데이트하고 Firestore에 저장합니다.
    /// </summary>
    /// <param name="newItem">추가할 아이템</param>
    public async Task AddItem(Item newItem)
    {
        if (string.IsNullOrEmpty(userId)) return;

        // 이미 같은 아이템이 있는지 확인
        Item existingItem = inventory.Find(item => item.id == newItem.id);

        if (existingItem != null)
        {
            // 아이템이 있으면 개수만 증가
            existingItem.count += newItem.count;
        }
        else
        {
            // 없으면 새로 추가
            inventory.Add(newItem);
            existingItem = newItem;
        }

        // Firestore에 저장
        DocumentReference itemRef = db.Collection("users").Document(userId).Collection("inventory").Document(existingItem.id);
        await itemRef.SetAsync(existingItem.ToDictionary());

        Debug.Log($"아이템 '{existingItem.name}' ({existingItem.count}개) 추가/업데이트 완료.");
        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// 인벤토리에서 아이템을 제거하고 Firestore에 반영합니다.
    /// </summary>
    /// <param name="itemId">제거할 아이템의 ID</param>
    /// <param name="countToRemove">제거할 개수</param>
    public async Task RemoveItem(string itemId, int countToRemove = 1)
    {
        if (string.IsNullOrEmpty(userId)) return;

        Item itemToRemove = inventory.Find(item => item.id == itemId);

        if (itemToRemove != null)
        {
            itemToRemove.count -= countToRemove;

            if (itemToRemove.count <= 0)
            {
                // 아이템 개수가 0 이하면 리스트에서 제거하고 Firestore에서도 문서를 삭제
                inventory.Remove(itemToRemove);
                DocumentReference itemRef = db.Collection("users").Document(userId).Collection("inventory").Document(itemId);
                await itemRef.DeleteAsync();
                Debug.Log($"아이템 '{itemToRemove.name}' 모두 제거 완료.");
            }
            else
            {
                // 개수만 업데이트하고 Firestore에 다시 저장
                DocumentReference itemRef = db.Collection("users").Document(userId).Collection("inventory").Document(itemId);
                await itemRef.SetAsync(itemToRemove.ToDictionary());
                Debug.Log($"아이템 '{itemToRemove.name}' {countToRemove}개 사용. 남은 개수: {itemToRemove.count}");
            }

            OnInventoryChanged?.Invoke();
        }
    }
}
