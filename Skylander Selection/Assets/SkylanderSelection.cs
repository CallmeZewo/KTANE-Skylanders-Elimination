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
using static Enums;

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
        MakeSkylander();
        MakeElements();
        MakeChapter();
        RandomizeModule();
        SetRandom();
        GetStageRules();
    }

    void Update()
    { //Shit that happens at any point after initialization

    }

    static readonly string[] VennTable =
    {
        "", // Not used
        "B", "I", "E", "F",
        "K", "O", "M", "A",
        "C", "L", "H", "D",
        "G", "N", "J"
    };

    // Lists holding sprites

    public List<Sprite> ElementSprites = new List<Sprite>();
    public List<Sprite> SkylanderSprites = new List<Sprite>();
    public List<Sprite> ChapterSprites = new List<Sprite>();

    // Catchphrases

    public List<AudioClip> SkylanderCatchphrases = new List<AudioClip>();
    public AudioClip DarkSpyroCatchphrase = new AudioClip();

    // Lists of pre made classes

    List<SkylanderClass> SkylanderList = new List<SkylanderClass>();
    List<ElementClass> ElementList = new List<ElementClass>();
    List<ChapterClass> ChapterList = new List<ChapterClass>();

    //randomly picked selection

    List<ElementClass> randomElements = new List<ElementClass>();
    List<SkylanderClass> randomSkylander = new List<SkylanderClass>();
    ChapterClass randomChapter;

    int Stage = 1;

    void MakeSkylander()
    {
        for (int i = 0; i < SkylanderSprites.Count; i++)
        {
            SkylanderList.Add(new SkylanderClass(SkylanderSprites[i], (ElementEnum)(i / 4), SkylanderCatchphrases[i]));
        }
    }

    void MakeElements()
    {
        for (int i = 0;i < ElementSprites.Count; i++)
        {
            ElementList.Add(new ElementClass(ElementSprites[i], (ElementEnum)i));
        }
    }

    void MakeChapter()
    {
        for(int i = 0;i < ChapterSprites.Count; i++)
        {
            ChapterList.Add(new ChapterClass(ChapterSprites[i], i));
        }
    }

    void RandomizeModule()
    {
        randomElements = ZRandom.PickRandomN(ElementList, 4);
        randomSkylander = ZRandom.PickRandomN(SkylanderList, 4);
        randomChapter = ZRandom.PickRandom(ChapterList);
    }

    // Sprites

    public SpriteRenderer SkylanderSprite1;
    public SpriteRenderer SkylanderSprite2;
    public SpriteRenderer SkylanderSprite3;
    public SpriteRenderer SkylanderSprite4;

    public SpriteRenderer ElementSprite1;
    public SpriteRenderer ElementSprite2;
    public SpriteRenderer ElementSprite3;
    public SpriteRenderer ElementSprite4;

    public SpriteRenderer ChapterSprite;

    // Button objects

    public GameObject BigButton1;
    public GameObject BigButton2;
    public GameObject BigButton3;
    public GameObject BigButton4;

    public GameObject SmallButton1;
    public GameObject SmallButton2;
    public GameObject SmallButton3;
    public GameObject SmallButton4;

    string RuleStage1;
    string RuleStage2;
    string RuleStage3;
    string RuleStage4;

    void SetRandom()
    {

        SkylanderSprite1.sprite = randomSkylander[0].Sprite;
        SetButtonColor(randomSkylander[0].Element, BigButton1);

        SkylanderSprite2.sprite = randomSkylander[1].Sprite;
        SetButtonColor(randomSkylander[1].Element, BigButton2);

        SkylanderSprite3.sprite = randomSkylander[2].Sprite;
        SetButtonColor(randomSkylander[2].Element, BigButton3);

        SkylanderSprite4.sprite = randomSkylander[3].Sprite;
        SetButtonColor(randomSkylander[3].Element, BigButton4);

        ElementSprite1.sprite = randomElements[0].Sprite;
        SetButtonColor(randomElements[0].Element, SmallButton1);

        ChapterSprite.sprite = randomChapter.Sprite;
    }

    void GetStageRules()
    {
        bool airOrFire = false;
        bool earthOrUndead = false;
        bool lifeOrWater = false;
        bool magicOrTech = false;

        SetElement(randomElements[0].Element, ref airOrFire, ref earthOrUndead, ref lifeOrWater, ref magicOrTech);
        RuleStage1 = VennCalc(airOrFire, earthOrUndead, lifeOrWater, magicOrTech);

        SetElement(randomElements[1].Element, ref airOrFire, ref earthOrUndead, ref lifeOrWater, ref magicOrTech);
        RuleStage2 = VennCalc(airOrFire, earthOrUndead, lifeOrWater, magicOrTech);

        SetElement(randomElements[2].Element, ref airOrFire, ref earthOrUndead, ref lifeOrWater, ref magicOrTech);
        RuleStage3 = VennCalc(airOrFire, earthOrUndead, lifeOrWater, magicOrTech);

        SetElement(randomElements[3].Element, ref airOrFire, ref earthOrUndead, ref lifeOrWater, ref magicOrTech);
        RuleStage4 = VennCalc(airOrFire, earthOrUndead, lifeOrWater, magicOrTech);

        Debug.LogFormat(
        "[Skylander Selection #{0}] Rules: {1}, {2}, {3}, {4}",
        ModuleId, RuleStage1, RuleStage2, RuleStage3, RuleStage4
        );
    }

    void SetElement(ElementEnum e, ref bool aof, ref bool eou, ref bool low, ref bool mot)
    {
        switch (e)
        {
            case ElementEnum.Air:
            case ElementEnum.Fire:
                aof = true;
                break;
            case ElementEnum.Earth:
            case ElementEnum.Undead:
                eou = true;
                break;
            case ElementEnum.Life:
            case ElementEnum.Water:
                low = true;
                break;
            case ElementEnum.Magic:
            case ElementEnum.Tech:
                mot = true;
                break;

        }
    }

    string VennCalc(bool aof, bool eou, bool low, bool mot)
    {
        int output = 0;
        if (aof) output |= 1 << 0;
        if (eou) output |= 1 << 1;
        if (low) output |= 1 << 2;
        if (mot) output |= 1 << 3;

        string result = VennTable[output];

        return result;
    }

    void SetNewStage()
    {
        switch (Stage)
        {
            case 1:
                Stage = 2;

                ElementSprite2.sprite = randomElements[1].Sprite;
                SetButtonColor(randomElements[1].Element, SmallButton2);
                break;
            case 2:
                Stage = 3;

                ElementSprite3.sprite = randomElements[2].Sprite;
                SetButtonColor(randomElements[2].Element, SmallButton3);
                break;
            case 3:
                Stage = 4;

                ElementSprite4.sprite = randomElements[3].Sprite;
                SetButtonColor(randomElements[3].Element, SmallButton4);
                break;
        }
    }

    void SetButtonColor(ElementEnum element, GameObject button)
    {
        switch (element)
        {
            //Air
            case ElementEnum.Air:
                button.GetComponent<MeshRenderer>().material.color = new Color(0f, 0.43137f, 0.68627f);
                break;
            //Earth
            case ElementEnum.Earth:
                button.GetComponent<MeshRenderer>().material.color = new Color(0.49020f, 0.19608f, 0f);
                break;
            //Fire
            case ElementEnum.Fire:
                button.GetComponent<MeshRenderer>().material.color = new Color(0.68627f, 0f, 0f);
                break;
            //Life
            case ElementEnum.Life:
                button.GetComponent<MeshRenderer>().material.color = new Color(0f, 0.35294f, 0f);
                break;
            //Magic
            case ElementEnum.Magic:
                button.GetComponent<MeshRenderer>().material.color = new Color(0.13725f, 0f, 0.19608f);
                break;
            //Tech
            case ElementEnum.Tech:
                button.GetComponent<MeshRenderer>().material.color = new Color(0.74510f, 0.31373f, 0f);
                break;
            //Undead
            case ElementEnum.Undead:
                button.GetComponent<MeshRenderer>().material.color = new Color(0.23529f, 0.23529f, 0.29412f);
                break;
            //Water
            case ElementEnum.Water:
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
