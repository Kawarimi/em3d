using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour {
    int index = 0; //6 RH, 7 LH
    GameObject[] inventorybuttons = new GameObject[7];
	// Use this for initialization
	void Start () {
        for (int i = 0; i < 7; i++)
            inventorybuttons[i] = transform.GetChild(i).gameObject;
        gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void AddItem(string name)
    {
        while(true)
        {          
            if (inventorybuttons[index].transform.GetChild(0).GetComponent<Text>().text == "None")
            {
                inventorybuttons[index].transform.GetChild(0).GetComponent<Text>().text = name;
                break;
            }
            else
            {
                if(index < 8)
                {
                    index++;
                }
                else
                {
                    index = 0;
                    print("no space in inv");
                    break;
                }
                
            }
            
        }
        gameObject.SetActive(false);
    }
}
