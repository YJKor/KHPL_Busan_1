using System;
using System.Collections.Generic;

// �� Ŭ������ �κ��丮�� ����� ������ �ϳ��� �����͸� �����մϴ�.
// Firestore�� �����͸� �ְ�ޱ� ���� ����ȭ(Serializable) �����ؾ� �մϴ�.
[Serializable]
public class Item
{
    public string id;       // ������ ���� ID (��: "sword_001", "potion_red")
    public string name;     // ������ �̸� (��: "��ö ��", "���� ����")
    public int count;       // ������ ����
    public string description; // ������ ����

    // Firestore�� ��ü�� ���� �������� ���ϹǷ�, Dictionary ���·� ��ȯ�ϴ� �Լ��� �ʿ��մϴ�.
    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            { "id", id },
            { "name", name },
            { "count", count },
            { "description", description }
        };
    }

    // Dictionary ���¿��� Item ��ü�� ��ȯ�ϴ� ���� �Լ��Դϴ�.
    public static Item FromDictionary(Dictionary<string, object> dict)
    {
        return new Item
        {
            id = dict["id"].ToString(),
            name = dict["name"].ToString(),
            // Firestore���� ���ڴ� Int64(long)���� ��޵� �� �����Ƿ� ��ȯ���ݴϴ�.
            count = Convert.ToInt32(dict["count"]),
            description = dict["description"].ToString()
        };
    }
}
