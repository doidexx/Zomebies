using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMenu : MonoBehaviour
{
	public GameObject Menu;
	private bool on = false;

    void Start()
    {
        
    }


    void Update()
    {
       
	   if(Input.GetKeyDown("escape"))
         on = !on;
     if(on)
         Menu.SetActive(true);
     else if(!on)
         Menu.SetActive(false);
	   
	   
    }
}
