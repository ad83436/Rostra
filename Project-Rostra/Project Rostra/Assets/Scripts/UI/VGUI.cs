using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VGUI : MonoBehaviour {

    public static VGUI V_Instance;
    public bool V_SecondaryWeaponExists;
	public int V_SecondaryWeaponID;
    public int V_HeartsCount;
    public int V_HP;
	private Rect V_SecondaryWeaponRect;
    private Rect V_HeartRect;
    private Vector2 V_RectSize;
    private Texture2D V_WeaponTexture;
    private Texture2D V_HeartTexture;
    private GUIStyle V_Style;



    //There can only be one VGUI object in any given scene
    private void Awake()
    {
        if(V_Instance==null)
        {
            V_Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Start () {

        DontDestroyOnLoad(gameObject);

		V_SecondaryWeaponExists = false;
		V_SecondaryWeaponID = 0;
		V_RectSize = new Vector2(75.0f, 75.0f);
		V_SecondaryWeaponRect = new Rect(Screen.width / 2.1f, Screen.height/8.0f, V_RectSize.x, V_RectSize.y);
        V_RectSize = new Vector2(150.0f, 100.0f);
        V_HeartRect = new Rect(Screen.width / 1.5f, Screen.height / 8.0f, V_RectSize.x, V_RectSize.y);
        V_HeartTexture = Resources.Load("Textures/Heart", typeof(Texture2D)) as Texture2D;

    }


	private void OnGUI()
	{
        V_Style = new GUIStyle(GUI.skin.label);
        V_Style.fontSize = 40;
        V_Style.fontStyle = FontStyle.Bold;
        V_Style.border = new RectOffset(20, 20, 20, 20);


        GUI.Box(V_SecondaryWeaponRect,"");

        //Draw Heart, equal sign, and heart counter value
        GUI.DrawTexture(V_HeartRect, V_HeartTexture);
        GUI.Label(new Rect(Screen.width / 1.3f, Screen.height / 7.0f, V_RectSize.x, V_RectSize.y),"=",V_Style);
        GUI.Label(new Rect(Screen.width / 1.2f, Screen.height / 7.0f, V_RectSize.x, V_RectSize.y), V_HeartsCount.ToString(), V_Style);

        //HP

        //Don't show negative numbers
        if (V_HP < 0)
        {
            V_HP = 0;
        }
        GUI.Label(new Rect(Screen.width / 4.8f, Screen.height / 7.0f, V_RectSize.x, V_RectSize.y), "HP", V_Style);
        GUI.Label(new Rect(Screen.width / 3.8f, Screen.height / 7.0f, V_RectSize.x, V_RectSize.y), "=", V_Style);
        GUI.Label(new Rect(Screen.width / 3.5f, Screen.height / 7.0f, V_RectSize.x, V_RectSize.y), V_HP.ToString(), V_Style);



        if (V_SecondaryWeaponExists)
		{
			V_WeaponTexture = Resources.Load("Textures/" + V_SecondaryWeaponID, typeof(Texture2D)) as Texture2D;
			GUI.DrawTexture(V_SecondaryWeaponRect, V_WeaponTexture);
		}

	}
}
