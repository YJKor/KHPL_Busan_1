using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class NPC_Line : MonoBehaviour
{
    public Text text;
    private void Awake()
    {
        text = GetComponent<Text>();
    }
    void Start()
    {
        //text.text = "�ȳ��ϼ���";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
