using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkills : MonoBehaviour
{
    public static EnemySkills Instance;
    public static bool usedSkill = false;
    public bool tellMeWhatYouKnow;
    public Player[] playerRef = new Player[4];
    [SerializeField] int[] backRow = new int[2];
    [SerializeField] int[]frontRow = new int[2];

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            RowThePlayers();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        tellMeWhatYouKnow = usedSkill;
    }

    public int[] GroundSmashSkill()
    {
        int randomValue = Random.Range(0, 50);
        int[] rowSelected = new int[2];

        if (randomValue >= 25)
        {
            Debug.Log("Attacked front Row");
            rowSelected[0] = frontRow[0];
            rowSelected[1] = frontRow[1];
            print(rowSelected[0] + "" + rowSelected[1]);
            return rowSelected;
        }

        else
        {
            Debug.Log("Attacked Back Row");
            rowSelected[0] = backRow[0];
            rowSelected[1] = backRow[1];
            print(rowSelected[0] + "" + rowSelected[1]);
            return rowSelected;

        }
    }

    //sorts the players in rows by index
    void RowThePlayers()
    {
        if (!playerRef[0].dead)
        {
            frontRow[0] = playerRef[0].playerIndex; //Fargas index
        }

        if (!playerRef[3].dead)
        {
            frontRow[1] = playerRef[3].playerIndex; // Arcelus index
        }

        if (!playerRef[1].dead)
        {
            backRow[0] = playerRef[1].playerIndex; //Oberon index
        }

        if (!playerRef[2].dead)
        {
            backRow[1] = playerRef[2].playerIndex; //frea index
        }
    }

}



