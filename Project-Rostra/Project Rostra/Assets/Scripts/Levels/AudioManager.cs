using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Play the music from the Audio Manager
    public static AudioManager instance;
    public AudioSource musicAudioSource;
    public AudioSource effectAudioSource;
    public AudioClip titleTheme1;
    public AudioClip worldMapMusic1;
    public AudioClip battleMusic1;
    public AudioClip bossMusic1;
    public AudioClip victoryMusic1;
    public AudioClip defeatMusic1;
    public AudioClip cutscene1;
    public AudioClip townTheme1;
    private AudioClip playThisNext;

    //UI
    public AudioClip uiScroll;
    public AudioClip uiConfirm;
    public AudioClip uiCancel;

    //Player
    public AudioClip attack;
    public AudioClip buff;
    public AudioClip deBuff;
    public AudioClip guard;

    //Fargas
    public AudioClip swordOfFury;
    public AudioClip rally;
    public AudioClip swiftStrike;
    public AudioClip sunGuard;
    public AudioClip bladeOfTheFallen;
    public AudioClip faWait;

    //Oberon

    //Frea
    public AudioClip arrowRain;
    public AudioClip doubleShot;
    public AudioClip bleedingEdge;
    public AudioClip neverAgain;
    public AudioClip piercingShot;
    public AudioClip frWait;

    //Arcelus
    public AudioClip heal;
    public AudioClip iceAge;
    public AudioClip drainEye;
    public AudioClip armageddon;
    public AudioClip hope;
    public AudioClip manaCharge;
    public AudioClip arWait;

    //Enemy shared Effects


    //Dragon
    public AudioClip earthSmash;
    //BlowSelf
    public AudioClip blowSelf;
    //Bat
    public AudioClip batWind;
    //Giant
    public AudioClip lighting;
    //RedReptile
    public AudioClip sliceAndDice;

	// Grendol
	public AudioClip Grendol_Lightning;
	public AudioClip Grendol_Fire;
	public AudioClip Grendol_Wind;
	public AudioClip Grendol_Heal;
	public AudioClip Grendol_Attack;


    private bool thisHasStartedPlaying = false;//Used to raise the volume steadily for a new piece
    private float musicMaxVolume = 0.5f; //Updated from the playerprefs

    public enum audioManagerState
    {
        notPlaying,
        playing,
        switching,
        muted
    }

    public audioManagerState currentState = audioManagerState.notPlaying;

    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if(instance == this)
        {
            instance = null;
        }
    }

    private void Start()
    {
        musicAudioSource.volume = 0.0f; //Start at volume zero
        GameManager.instance.listOfUndestroyables.Add(this.gameObject); //On start cause AM and GM start out in the same scene
    }

    private void Update()
    {
        switch(currentState)
        {
            case audioManagerState.notPlaying:
                if(thisHasStartedPlaying)
                {
                    if (musicAudioSource.volume < musicMaxVolume)
                    {
                        musicAudioSource.volume += 0.01f; //Fade in the music
                    }
                    else
                    {
                        musicAudioSource.volume = musicMaxVolume;
                        thisHasStartedPlaying = false;
                        currentState = audioManagerState.playing;
                    }
                }
                break;
            case audioManagerState.switching:
                if(!thisHasStartedPlaying) //Fade out the current music first
                {
                    if (musicAudioSource.volume > 0.0f)
                    {
                        musicAudioSource.volume -= 0.1f;
                    }
                    else
                    {
                        musicAudioSource.volume = 0.0f; //Fade out the music
                        musicAudioSource.Stop(); //Stop playing the old piece
                        musicAudioSource.clip = playThisNext; //Update the clip
                        thisHasStartedPlaying = true; //Fade in the new music
                        musicAudioSource.Play(); //Start playing the new piece                       
                    }
                }
                else
                {
                    if (musicAudioSource.volume < musicMaxVolume)
                    {
                        musicAudioSource.volume += 0.05f; //Fade in the music
                    }
                    else
                    {
                        musicAudioSource.volume = musicMaxVolume;
                        thisHasStartedPlaying = false;
                        currentState = audioManagerState.playing; //Fade in complete
                    }
                }
                break;
        }
    }

    public void PlayThisClip(string clipName)
    {
        switch (clipName) //Get the new clip 
        {
            case "TitleTheme":
                playThisNext = titleTheme1;
                break;
            case "WorldMapMusic1":
                playThisNext = worldMapMusic1;
                break;
            case "BattleMusic1":
                playThisNext = battleMusic1;
                break;
            case "BossMusic1":
                playThisNext = bossMusic1;
                break;
            case "VictoryMusic1":
                playThisNext = victoryMusic1;
                break;
            case "DefeatMusic1":
                playThisNext = defeatMusic1;
                break;
            case "Cutscene1":
                playThisNext = cutscene1;
                break;
            case "TownTheme":
                playThisNext = townTheme1;
                break;
        }

        if(currentState == audioManagerState.notPlaying) //The very first call
        {
            thisHasStartedPlaying = true;
            musicAudioSource.clip = playThisNext;
            musicAudioSource.Play();
        }
        else if(currentState == audioManagerState.playing) //If we are already playing something, go to switching
        {
            currentState = audioManagerState.switching;
            thisHasStartedPlaying = false;
        }

    }

    //Play effects. Play one shot
    public void playThisEffect(string effect)
    {
        switch(effect)
        {
            //UI
            case "uiScroll":
                effectAudioSource.PlayOneShot(uiScroll);
                break;
            case "uiConfirm":
                effectAudioSource.PlayOneShot(uiConfirm);
                break;
            case "uiCancel":
                effectAudioSource.PlayOneShot(uiCancel);
                break;

            //Generic 
            case "attack":
                effectAudioSource.PlayOneShot(attack);
                break;
            case "buff":
                effectAudioSource.PlayOneShot(buff);
                break;
            case "debuff":
                effectAudioSource.PlayOneShot(deBuff);
                break;
            case "guard":
                effectAudioSource.PlayOneShot(guard);
                break;
            //Fargas
            case "swordOfFury":
                effectAudioSource.PlayOneShot(swordOfFury);
                break;
            case "swiftStrike":
                effectAudioSource.PlayOneShot(swiftStrike);
                break;
            case "rally":
                effectAudioSource.PlayOneShot(rally);
                break;
            case "sunguard":
                effectAudioSource.PlayOneShot(sunGuard);
                break;
            case "bladeOfTheFallen":
                effectAudioSource.PlayOneShot(bladeOfTheFallen);
                break;
            case "faWait":
                effectAudioSource.PlayOneShot(faWait);
                break;

            //Frea
            case "doubleShot":
                effectAudioSource.PlayOneShot(doubleShot);
                break;
            case "bleedingEdge":
                effectAudioSource.PlayOneShot(bleedingEdge);
                break;
            case "arrowRain":
                effectAudioSource.PlayOneShot(arrowRain);
                break;
            case "neverAgain":
                effectAudioSource.PlayOneShot(neverAgain);
                break;
            case "piercingShot":
                effectAudioSource.PlayOneShot(piercingShot);
                break;
            case "frWait":
                effectAudioSource.PlayOneShot(frWait);
                break;

            //Arcelus
            case "heal":
                effectAudioSource.PlayOneShot(heal);
                break;
            case "iceAge":
                effectAudioSource.PlayOneShot(iceAge);
                break;
            case "armageddon":
                effectAudioSource.PlayOneShot(armageddon);
                break;
            case "drainEye":
                effectAudioSource.PlayOneShot(drainEye);
                break;
            case "hope":
                effectAudioSource.PlayOneShot(hope);
                break;
            case "manaCharge":
                effectAudioSource.PlayOneShot(manaCharge);
                break;
            case "arWait":
                effectAudioSource.PlayOneShot(arWait);
                break;

            //Enemy

            //Dragon
            case "EarthSmash":
                effectAudioSource.PlayOneShot(earthSmash);
                break;

            //BlowSelf
            case "Blow":
                effectAudioSource.PlayOneShot(blowSelf);
                break;

            //Bat
            case "BatWind":
                effectAudioSource.PlayOneShot(batWind);
                break;

            //Giant
            case "Lighting":
                effectAudioSource.PlayOneShot(lighting);
                break;

            //RedReptile
            case "SliceAndDice":
                effectAudioSource.PlayOneShot(sliceAndDice);
                break;


			// Grendol
			case "GLightning":
				effectAudioSource.PlayOneShot(Grendol_Lightning);
				break;
			case "GFire":
				effectAudioSource.PlayOneShot(Grendol_Fire);
				break;
			case "GHeal":
				effectAudioSource.PlayOneShot(Grendol_Heal);
				break;
			case "GWind":
				effectAudioSource.PlayOneShot(Grendol_Wind);
				break;
			case "GAttack":
				effectAudioSource.PlayOneShot(Grendol_Attack);
				break;
                

        }
    }
}
