using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using ZT = ZToolsKtane;
using Rnd = UnityEngine.Random;
using Math = ExMath;
using ZToolsKtane;

public class SkylanderSelection : MonoBehaviour
{

    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable[] Buttons;

    static int ModuleIdCounter = 1;
    int ModuleId;
    private bool ModuleSolved;

    void Awake()
    { //Avoid doing calculations in here regarding edgework. Just use this for setting up buttons for simplicity.
        ModuleId = ModuleIdCounter++;
        GetComponent<KMBombModule>().OnActivate += Activate;
        foreach (KMSelectable button in Buttons) {
            button.OnInteract += delegate () { PressHandler(button); return false; };
        }

    }

    void OnDestroy()
    { //Shit you need to do when the bomb ends

    }

    void Activate()
    { //Shit that should happen when the bomb arrives (factory)/Lights turn on

    }

    void Start()
    { //Shit that you calculate, usually a majority if not all of the module
        RandomizeModule();
        SetRandom();
        GetStageRules();
    }

    void Update()
    { //Shit that happens at any point after initialization

    }

    public List<Sprite> Elements = new List<Sprite>();
    public List<Sprite> Skylander = new List<Sprite>();
    public List<Sprite> Chapter = new List<Sprite>();

    public List<AudioClip> SkylanderCatchphrases = new List<AudioClip>();
    public AudioClip DarkSpyroCatchphrase = new AudioClip();

    List<Sprite> randomElements = new List<Sprite>();
    List<Sprite> randomSkylander = new List<Sprite>();
    Sprite randomChapter;

    int Stage = 1;

    void RandomizeModule()
    {
        randomElements = ZRandom.PickRandomN(Elements, 4);
        randomSkylander = ZRandom.PickRandomN(Skylander, 4);
        randomChapter = ZRandom.PickRandom(Chapter);
    }

    public SpriteRenderer SkylanderSprite1;
    public SpriteRenderer SkylanderSprite2;
    public SpriteRenderer SkylanderSprite3;
    public SpriteRenderer SkylanderSprite4;

    public SpriteRenderer ElementSprite1;
    public SpriteRenderer ElementSprite2;
    public SpriteRenderer ElementSprite3;
    public SpriteRenderer ElementSprite4;

    public SpriteRenderer ChapterSprite;

    public GameObject BigButton1;
    public GameObject BigButton2;
    public GameObject BigButton3;
    public GameObject BigButton4;

    public GameObject SmallButton1;
    public GameObject SmallButton2;
    public GameObject SmallButton3;
    public GameObject SmallButton4;

    // Saving what index the Skylander is from the list
    int SkylanderIndex1;
    int SkylanderIndex2;
    int SkylanderIndex3;
    int SkylanderIndex4;
    
    // Saving waht index the Element is from the list
    int ElementIndex1;
    int ElementIndex2;
    int ElementIndex3;
    int ElementIndex4;

    string RuleStage1;
    string RuleStage2;
    string RuleStage3;
    string RuleStage4;

    void SetRandom()
    {
        SkylanderIndex1 = Skylander.IndexOf(randomSkylander[0]);
        SkylanderIndex2 = Skylander.IndexOf(randomSkylander[1]);
        SkylanderIndex3 = Skylander.IndexOf(randomSkylander[2]);
        SkylanderIndex4 = Skylander.IndexOf(randomSkylander[3]);

        ElementIndex1 = Elements.IndexOf(randomElements[0]);
        ElementIndex2 = Elements.IndexOf(randomElements[1]);
        ElementIndex3 = Elements.IndexOf(randomElements[2]);
        ElementIndex4 = Elements.IndexOf(randomElements[3]);

        SkylanderSprite1.sprite = randomSkylander[0];
        SetButtonColor(SkylanderIndex1, BigButton1);

        SkylanderSprite2.sprite = randomSkylander[1];
        SetButtonColor(SkylanderIndex2, BigButton2);

        SkylanderSprite3.sprite = randomSkylander[2];
        SetButtonColor(SkylanderIndex3, BigButton3);

        SkylanderSprite4.sprite = randomSkylander[3];
        SetButtonColor(SkylanderIndex4, BigButton4);

        ElementSprite1.sprite = randomElements[0];
        SetButtonColor(ElementIndex1 * 4, SmallButton1);

        ChapterSprite.sprite = randomChapter;
    }

    void GetStageRules()
    {
        bool airOrFire = false;
        bool earthOrUndead = false;
        bool lifeOrWater = false;
        bool magicOrTech = false;

        if (ElementIndex1 == 0 || ElementIndex1 == 2)
            airOrFire = true;
        if (ElementIndex1 == 1 || ElementIndex1 == 6)
            earthOrUndead = true;
        if (ElementIndex1 == 3 || ElementIndex1 == 7)
            lifeOrWater = true;
        if (ElementIndex1 == 4 || ElementIndex1 == 5)
            magicOrTech = true;

        RuleStage1 = VennCalc(airOrFire, earthOrUndead, lifeOrWater, magicOrTech);

        if (ElementIndex2 == 0 || ElementIndex2 == 2)
            airOrFire = true;
        if (ElementIndex2 == 1 || ElementIndex2 == 6)
            earthOrUndead = true;
        if (ElementIndex2 == 3 || ElementIndex2 == 7)
            lifeOrWater = true;
        if (ElementIndex2 == 4 || ElementIndex2 == 5)
            magicOrTech = true;

        RuleStage2 = VennCalc(airOrFire, earthOrUndead, lifeOrWater, magicOrTech);

        if (ElementIndex3 == 0 || ElementIndex3 == 2)
            airOrFire = true;
        if (ElementIndex3 == 1 || ElementIndex3 == 6)
            earthOrUndead = true;
        if (ElementIndex3 == 3 || ElementIndex3 == 7)
            lifeOrWater = true;
        if (ElementIndex3 == 4 || ElementIndex3 == 5)
            magicOrTech = true;

        RuleStage3 = VennCalc(airOrFire, earthOrUndead, lifeOrWater, magicOrTech);

        if (ElementIndex4 == 0 || ElementIndex4 == 2)
            airOrFire = true;
        if (ElementIndex4 == 1 || ElementIndex4 == 6)
            earthOrUndead = true;
        if (ElementIndex4 == 3 || ElementIndex4 == 7)
            lifeOrWater = true;
        if (ElementIndex4 == 4 || ElementIndex4 == 5)
            magicOrTech = true;

        RuleStage4 = VennCalc(airOrFire, earthOrUndead, lifeOrWater, magicOrTech);
    }

    string VennCalc(bool aof, bool eou, bool low, bool mot)
    {
        int output = 0;
        if (aof) output |= 1 << 0;
        if (eou) output |= 1 << 1;
        if (low) output |= 1 << 2;
        if (mot) output |= 1 << 3;

        switch (output)
        {
            case 1:
                return "B";
            case 2:
                return "I";
            case 3:
                return "E";
            case 4:
                return "F";
            case 5:
                return "K";
            case 6:
                return "O";
            case 7:
                return "M";
            case 8:
                return "A";
            case 9:
                return "C";
            case 10:
                return "L";
            case 11:
                return "H";
            case 12:
                return "D";
            case 13:
                return "G";
            case 14:
                return "N";
            case 15:
                return "J";
        }
        return "This never gets called lol";
    }

    void SetNewStage()
    {
        switch (Stage)
        {
            case 1:
                Stage = 2;

                ElementSprite2.sprite = randomElements[1];
                SetButtonColor(ElementIndex2 * 4, SmallButton2);
                break;
            case 2:
                Stage = 3;

                ElementSprite3.sprite = randomElements[2];
                SetButtonColor(ElementIndex3 * 4, SmallButton3);
                break;
            case 3:
                Stage = 4;

                ElementSprite4.sprite = randomElements[3];
                SetButtonColor(ElementIndex4 * 4, SmallButton4);
                break;
        }
    }

    void SetButtonColor(int Offset, GameObject button)
    {
        switch (Offset)
        {
            //Air
            case 0:
            case 1:
            case 2:
            case 3:
                button.GetComponent<MeshRenderer>().material.color = new Color(0f, 0.43137f, 0.68627f);
                break;
            //Earth
            case 4:
            case 5:
            case 6:
            case 7:
                button.GetComponent<MeshRenderer>().material.color = new Color(0.49020f, 0.19608f, 0f);
                break;
            //Fire
            case 8:
            case 9:
            case 10:
            case 11:
                button.GetComponent<MeshRenderer>().material.color = new Color(0.68627f, 0f, 0f);
                break;
            //Life
            case 12:
            case 13:
            case 14:
            case 15:
                button.GetComponent<MeshRenderer>().material.color = new Color(0f, 0.35294f, 0f);
                break;
            //Magic
            case 16:
            case 17:
            case 18:
            case 19:
                button.GetComponent<MeshRenderer>().material.color = new Color(0.13725f, 0f, 0.19608f);
                break;
            //Tech
            case 20:
            case 21:
            case 22:
            case 23:
                button.GetComponent<MeshRenderer>().material.color = new Color(0.74510f, 0.31373f, 0f);
                break;
            //Undead
            case 24:
            case 25:
            case 26:
            case 27:
                button.GetComponent<MeshRenderer>().material.color = new Color(0.23529f, 0.23529f, 0.29412f);
                break;
            //Water
            case 28:
            case 29:
            case 30:
            case 31:
                button.GetComponent<MeshRenderer>().material.color = new Color(0f, 0.11765f, 0.25490f);
                break;
        }
    }

    void PressHandler(KMSelectable button)
    {
        int buttonIndex = Array.IndexOf(Buttons, button);

        Debug.Log(buttonIndex);
    }

    void Solve()
    {
        GetComponent<KMBombModule>().HandlePass();
    }

    void Strike()
    {
        GetComponent<KMBombModule>().HandleStrike();
    }
    /* Delete this if you dont want TP integration
#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use !{0} to do something.";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string Command)
    {
        yield return null;
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        yield return null;
    }*/
}
