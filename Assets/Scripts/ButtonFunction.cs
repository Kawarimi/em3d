using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System;
//using UnityEditor;

public class ButtonFunction : NetworkBehaviour {

	private Animator animator;
	private Rigidbody rb;    
	private Slider staminabar;
	private Slider healthbar;
	private Slider manabar;
	private GameObject mcam;
	private GameObject head;
	private GameObject facemask;
    private GameObject campos;
    public GameObject Camprefab;
	private GameObject uiforspells;
	public GameObject DeathSystem;
    private Button UpButton;
	private Button DownButton;
	private Button LeftButton;
	private Button RightButton;
	private Button yButton;
	private Text SelectedgameObject;
	private Dropdown effectdropdown;
	private InputField magfield;
	private Text ybuttontext;
	private float walkspeed = 20f;
	//private float speed = 0f;
	private float jumpspeed = 400f;
	private float maxspeed = 5f;
	private float stamina = 100f;
	private float staminadeprate = 0.5f;
	private float maxstamina = 100f;
	private float health = 100f;
	private float maxhealth = 100f;
	private float mana = 100f;
	private float maxmana = 100f;
	private bool walking;
	private int layerMask;
	private GameObject hitobject;
	private bool upb;
	private bool downb;
	private bool rightb;
	private bool leftb;
    private bool menuactive = false;
	private bool ytoggle = true;
	private bool maskactive = false;
    private bool inventoryactive = false;
	// Use this for initialization

	
	void Start () {

        if(isLocalPlayer)
		{
        try
		{
                campos = transform.GetChild(7).gameObject;
        facemask = transform.Find(@"BasicBandit_Mask").gameObject;
        transform.GetChild(5).gameObject.SetActive(true);
        transform.GetChild(6).gameObject.SetActive(true);
        //head = transform.Find("Head").gameObject;
        mcam = Instantiate(Camprefab);
        mcam.SendMessage("Getcampos", campos);
        mcam.SetActive(true);
        //Set unique layer
                //int outcheck;
        //int.TryParse(netId.Value.ToString(), out outcheck);
        //gameObject.layer = 8 + outcheck;
		layerMask = 1 << 8;
        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;
        mcam.GetComponent<Camera>().cullingMask = layerMask;
		//Cursor.lockState = CursorLockMode.Locked;    
		var canvasfspells = transform.GetChild(6);
        var panel = canvasfspells.transform.GetChild(0);
		var canvasforstats = transform.GetChild(5);
		UpButton = panel.transform.GetChild(0).GetComponent<Button>();
		DownButton = panel.transform.GetChild(1).GetComponent<Button>();
		LeftButton = panel.transform.GetChild(2).GetComponent<Button>();
		RightButton = panel.transform.GetChild(3).GetComponent<Button>();
		yButton = panel.transform.GetChild(8).GetComponent<Button>();
		magfield = panel.transform.GetChild(6).GetComponent<InputField>();
		effectdropdown = panel.transform.GetChild(5).GetComponent<Dropdown>();
		SelectedgameObject = panel.transform.GetChild(4).GetComponent<Text>();
        uiforspells = canvasfspells.gameObject;
		staminabar = canvasforstats.transform.GetChild(0).GetComponent<Slider>();
		healthbar = canvasforstats.transform.GetChild(1).GetComponent<Slider>();
		manabar = canvasforstats.transform.GetChild(2).GetComponent<Slider>();
		
                
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		uiforspells.SetActive(false);
        
		magfield.text = "0";
		ybuttontext = yButton.GetComponentInChildren<Text>();
		}
		catch
		{
			print("Init failed.");
		}
		}	
	}         			

	// Update is called once per frame
	void Update ()
	{ 
		if(!isLocalPlayer)
		{
			return;
		}

	//print("spd" + speed);
    //print("sprnting " +sprinting);
	//print("stam" + stamina);
	//print("grounded" + IsGrounded());
	//mana = Mathf.SmoothDamp(mana,maxmana,ref zero1, 0.5f);
	//health = Mathf.SmoothDamp(health, maxhealth, ref zero2, 2f);
	mana = Mathf.Clamp(mana + (1.0f * Time.deltaTime), 0.0f, maxmana);
	health = Mathf.Clamp(health + (0.2f * Time.deltaTime), -Mathf.Epsilon, maxhealth);
	staminabar.value = stamina;
	healthbar.value = health;
	manabar.value = mana;
	if(health <= 0)
	{
    DeathSystem.SendMessage("Kill", gameObject.name);
	health = 98.0f;
	stamina = 15.0f;
	mana = 15.0f;
	}
	
	if(Input.GetKey(KeyCode.LeftShift) && stamina > 10.0f && walking && IsGrounded())
	{
		//print("sprint");
		maxspeed = 20;
		staminadeprate = 7.5f;
	}
	else
	{
		maxspeed = 5;
		staminadeprate = 1.0f;
	}
	if(Input.GetKeyDown(KeyCode.Space) && IsGrounded() && stamina > 10.0f)
	{
		rb.AddRelativeForce(0,jumpspeed,0);
	}
    //idling
	if(!Input.anyKey && IsGrounded()) 
	{
    Idle();
	}
	//walking
	if(Input.GetAxis("Vertical") > 0)	
	{
	rb.AddRelativeForce(0,0,walkspeed);
	Walk();
	}
	
	if(Input.GetAxis("Vertical") < 0)	
	{
	rb.AddRelativeForce(0,0,-walkspeed);
	Walk();
	}
	
	if(Input.GetAxis("Horizontal") < 0)
	{
	rb.AddRelativeForce(-walkspeed,0,0);
	Walk();
	}
	
	if(Input.GetAxis("Horizontal") > 0)
	{
	rb.AddRelativeForce(walkspeed,0,0);
	Walk();
	}
	//put mask on/off
	if(Input.GetKeyDown(KeyCode.M))
	{
		maskactive = !maskactive;
		if(facemask.activeSelf)
		{
			facemask.SetActive(false);
		}
		else
		{
			facemask.SetActive(true);
		}
	}
	//toggle menu

	if(Input.GetKeyDown(KeyCode.Q))
	{
	  
		menuactive ^= true;
        if(menuactive)
		{
			uiforspells.SetActive(true);
			
		}
		if(!menuactive)
		{
			uiforspells.SetActive(false);
		}
	}
	//toggle self select
	if(Input.GetKeyDown(KeyCode.O))
	{		
		hitobject = gameObject;
		SelectedgameObject.text = hitobject.name;				
	}
	//select object
	if(Input.GetKeyDown(KeyCode.E))
	{
        RaycastHit raycasthit;
		Physics.Raycast(mcam.transform.position,mcam.transform.forward,out raycasthit,Mathf.Infinity,layerMask);
		hitobject = raycasthit.transform.gameObject;
		SelectedgameObject.text = hitobject.name;
            if(hitobject.tag == "item")
            {
                hitobject.SendMessage("ItemTake",gameObject);
                transform.Find("Canvas/Inventorypanel").gameObject.SetActive(true);
                transform.Find("Canvas/Inventorypanel").gameObject.SendMessage("AddItem",hitobject.name);               
            }
	}
    //open inventory
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryactive ^= true;
            if (inventoryactive)
            {
                transform.Find("Canvas/Inventorypanel").gameObject.SetActive(true);
                transform.Find("Canvas/Clickout").gameObject.SetActive(true);

            }
            if (!inventoryactive)
            {
                transform.Find("Canvas/Inventorypanel").gameObject.SetActive(false);
                transform.Find("Canvas/Clickout").gameObject.SetActive(false);
            }
        }
	{
	//activateButton.onClick.AddListener(spfunc);
	UpButton.onClick.AddListener(upButton);
	DownButton.onClick.AddListener(downButton);
	RightButton.onClick.AddListener(rightButton);
	LeftButton.onClick.AddListener(leftButton);
	yButton.onClick.AddListener(yyButton);
	}
	//sets magnitude to 0
	if(Input.GetKeyDown(KeyCode.K))
	{
        magfield.text = "0";
	}
	}

	void FixedUpdate() {
	//fixupdate start
	//print("Fire3" + Input.GetAxis("Fire3"));
	transform.eulerAngles = new Vector3(0,mcam.transform.eulerAngles.y,0);
    //head.transform.eulerAngles = new Vector3(-mcam.transform.eulerAngles.x,mcam.transform.eulerAngles.y + 180,0); 
	
	//unneccesary for now
	//speed = rb.velocity.magnitude;
    if(IsGrounded())
	{
     	rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxspeed);	
	}
	//fixupdate end
	}
	
    bool IsGrounded ()
    {
        return Physics.Raycast (transform.position, - Vector3.up, 0.1f);
    }
	public void Idle ()
	{
		animator = GetComponent<Animator>();
		animator.SetBool ("Walk", false);
		stamina = Mathf.Clamp(stamina + (5.0f * Time.deltaTime), 0.0f, maxstamina);
		rb.velocity = new Vector3(0,0,0);
		//print("idling");
	}

	public void Walk ()
	{
		animator = GetComponent<Animator>();
		animator.SetBool ("Walk", true);
		stamina = Mathf.Clamp(stamina - (staminadeprate * Time.deltaTime), 0.0f, maxstamina);
		walking = true;
        //print("walking");
	}
	public void spfunc()
	{
		float magnifier = float.Parse(magfield.text);
		float manacost = Mathf.Abs(magnifier);
		if(mana > manacost)
		{
		try
		{
		if(effectdropdown.value == 0)
		{
			magnifier *= 25;
			if(upb)
			{
            hitobject.GetComponent<Rigidbody>().AddForce(Vector3.forward * magnifier); 
			}
			if(downb)
			{
            hitobject.GetComponent<Rigidbody>().AddForce(-Vector3.forward * magnifier); 
			}
			if(rightb)
			{
            hitobject.GetComponent<Rigidbody>().AddForce(Vector3.right  * magnifier); 
			}
			if(leftb)
			{
            hitobject.GetComponent<Rigidbody>().AddForce(-Vector3.right  * magnifier); 
			}
			if(ytoggle)
			{
			hitobject.GetComponent<Rigidbody>().AddForce(Vector3.up * magnifier); 
			}
			if(!ytoggle)
			{
			hitobject.GetComponent<Rigidbody>().AddForce(-Vector3.up * magnifier); 
			}			
		}
		if(effectdropdown.value == 1 && ytoggle)
		{
            magnifier *= 25;
			hitobject.GetComponent<Rigidbody>().AddTorque(Vector3.right * magnifier); 
		}
		if(effectdropdown.value == 1 && !ytoggle)
		{
            magnifier *= 25;
			hitobject.GetComponent<Rigidbody>().AddTorque(-Vector3.right * magnifier); 
		}
		if(effectdropdown.value == 2 && ytoggle)
		{
            magnifier *= 25;
			hitobject.GetComponent<Rigidbody>().AddTorque(Vector3.up * magnifier); 
		}
		if(effectdropdown.value == 2 && !ytoggle)
		{
            magnifier *= 25;
			hitobject.GetComponent<Rigidbody>().AddTorque(-Vector3.up * magnifier); 
		}
		if(effectdropdown.value == 3 && ytoggle)
		{
            magnifier *= 25;
			hitobject.GetComponent<Rigidbody>().AddTorque(Vector3.forward * magnifier); 
		}
		if(effectdropdown.value == 3 && !ytoggle)
		{
            magnifier *= 25;
			hitobject.GetComponent<Rigidbody>().AddTorque(-Vector3.forward * magnifier); 
		}
		if(effectdropdown.value == 4)
		{
			magnifier /= 75;
			hitobject.transform.localScale += new Vector3(magnifier,magnifier,magnifier);
			hitobject.GetComponent<Rigidbody>().mass += magnifier;
		}
	    }
		catch
		{
			magfield.text = "Failed";
		}
		mana -= manacost;
		}
		else
		{
			magfield.text = "Not enough mana";
		}
		if(effectdropdown.value == 5 && mana > 5)
		{
			hitobject.SendMessage("KillAura", gameObject.name);
		}
	}
	public void upButton()
	{
		upb = true;
		downb = false;
	}
	public void downButton()
	{
		upb = false;
		downb = true;
	}
	public void rightButton()
	{
		rightb = true;
		leftb = false;
	}
	public void leftButton()
	{
		rightb = false;
		leftb = true;
	}
	public void yyButton()
	{
       ytoggle = !ytoggle;
	   if(ytoggle)
	   {
       ybuttontext.text = "Up";
	   }
	   if(!ytoggle)
	   {
	   ybuttontext.text = "Down";
	   }
	}
	void GotHit(float f)
	{
    float e = 0;
	health = Mathf.SmoothDamp(health,health -= f,ref e,0.05f);
	}
	void ManaDrain(float f)
	{
	float e = 0;
	mana = Mathf.SmoothDamp(mana,mana -= f,ref e,0.05f);	
	} 
	//public void MagfieldValueChangeCheck()
	//{
	//Abandoned because of bugs
	//try
	//{
    //float foo = float.Parse(magfield.text);
	//magfieldtrue = true;
	//}
	//catch
	//{
    //magfield.textComponent. = "Cannot parse text...";
	//magfieldtrue = false;	
	//}
	//}
}