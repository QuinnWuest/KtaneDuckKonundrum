using System.Collections;
using System;
using UnityEngine;
using System.Linq;
using KModkit;
using Random = UnityEngine.Random;

public class duckKonundrumScript : MonoBehaviour {  // this code is awful, continue at your own risk

    public KMBombModule Module;
    public KMBossModule BossModule;
    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMSelectable[] btnSelectables;
    public TextMesh[] btnTexts;
    public TextMesh screenText;
    public MeshRenderer[] LEDRenderers, btnRenderers;
    public Color32[] colors;
    public GameObject border, screen;

    private static int _moduleIdCounter = 1;
    private int _moduleId;
    private bool solved = false;
    private bool readyForInput = false;

    private string[] ignoredModules;
    private int stageCount, solveCount;
    private int currentStage = 0, queuedStages = 0, displayedStage = -1;
    private string stageText;

    private bool animationPlaying = false, questionsAreDisplayed = false, readyToStartSubmission = false;
    
    private int currentPos, duckPos;
    private int chairMethod;
    private int[] seatColors = { 0, 0, 0, 0, 0, 0 }; // 0 = unpainted, 1/2/3 = red/yellow/blue, 4/5/6 = orange/green/purple, 7 = brown
    private int[] bottomColors = { 0, 0, 0, 0, 0, 0 };
    private int[] backNumbers = { 0, 0, 0, 0, 0, 0 };
    private int[] backColors = { 0, 0, 0, 0, 0, 0 };
    private int[] wateringCanContents = { 0, 0, 0 }; // ordered from smallest to largest
    private int[] wateringCanColors = { 0, 0, 0 };
    private int[] bodyPartColors = { 0, 0, 0, 0, 0 }; // left foot/right foot/left hand/right hand/butt
    private int hokeyPokeyStep = -1; // step of the hokey pokey
    private int hokeyPokeyPart = 0; // body part of the hokey pokey
    private int hokeyPokeyRule = 0; // what happens while you do the hokey pokey
    private int hokeyPokeySubRule = 0; // idk
    private bool hokeyPokeyActive = false; // true if the hokey pokey is currently being done
    private string duckName = "";
    private string originalDuckName = "";
    private int whoopeeCushionPos = 69; // whoopee cushion starts on none of the chair(s)
    private bool foreheadActivated = false; // this probably sounds so stupid out of context
    private int foreheadLs = 0;
    private int foreheadRule = 0;
    private int foreheadColor = 0;
    
    private static readonly string[] possibleColors = { "no", "red", "yellow", "blue", "orange", "green", "purple", "brown" };
    private static readonly string[] bodyParts = { "your right foot", "your left foot", "your right hand", "your left hand", "your butt cheeks", "both of your feet", "both of your hands", "the duck's feet" };
    private static readonly string[] sizes = { "smallest", "medium-sized", "largest" };
    private static readonly string[] possibleChairMethods = { "Sit", "Stand", "Do a handstand" };
    private static readonly int[] colorsWithRed = { 1, 1, 4, 6, 4, 7, 6, 7 };
    private static readonly int[] colorsWithYellow = { 2, 4, 2, 5, 4, 5, 7, 7 };
    private static readonly int[] colorsWithBlue = { 3, 6, 5, 3, 7, 5, 6, 7 };
    /*
    private static readonly string[] colorSynonyms = {
        "the background of Forget Me Not", "the status light when you strike", "the left button of Simon Says",
        "the background of Forget It Not", "the right button of Simon Says", "the letters in Affine Cycle",
        "the background of Dr. Doctor", "the background of Forget The Colors", "the letters in Hill Cycle",
        "the buttons in Boolean Maze", "the Arrows module that displays arrows on its screen", "the destination screen in Polyhedral Maze",
        "the screens from Password", "the background of Forget Me Now", "the snooker ball representing 3 in Kudosudoku",
        "the background of Shell Game", "the background of The Rule", "the Arrows module with a scrambled word in the top-left corner",
        "the background of Old Fogey", "the background of Quaternions", "the snooker ball representing 4 in Kudosudoku" }; 
    */
    private static readonly string[] duckNames = {
        "Donald", "Daisy", "Drake", "Darkwing", "Flintheart", "Huey", "Louie", "Dewey", "Scrooge", "Daffy", "Howard", "Aflac", "Chica", // fictional ducks
        "Harold", "Reginald", "Victor", "Bo", "Ty", "Ed", "Zoz", // names of the ducks and the people in the original Duck Konundrum
        "Coroges", "Dogslante", "Landod", "Amciag", "Tofnen", // names of the ducks in the third Duck Konundrum, D3: The Fellowship of the Duck
        "Teddy", "Amelia", "Sasha", "Enrique", "Oscar", "Cthulu", // names of the ducks in the fifth Duck Konundrum, The Amazing Juggling Troupe of Duckkon Undrum V (and also Cthulu)
        "Dude", "Emerald", "Jordan", "Milton", "Pauletta", // names of the chefs in the sixty Duck Konundrum, The Duck Konundrum VI: Now with Way Way Way Way Way Too Much Pepper
        "Economist", "Linguist", "Chemist", "Al Capone", "Tartuffe", "Cookie Monster", "Blastoid", "Algernon", "Scotchy", // names of the puzzle solvers in the eighth Duck Konundrum, DK8: The Turducken Konundrum
        "Bagels", "Grunkle", "Steven", "Harold", "Clam Chowder", "Quackers", "Duckworth", "Quedlington", "Billiam", "Crouch", "Craig", "Goosetav", "Drew", "Henry", "Glen", "Waz",
        "Gorby", "Toodles", "Peabeater", "Xanderoth", "Mario", "Clorco", "Dicey", "Alex", "Duck Norris", "Jimothy", "Webster", "Dolan", "Jon", "JonJon", "Eric", "Cabebe", "Junior", "Doctor", "Duckra",
        "Josh", "John Cena", "Username", "Obama", "Muck", "Weedeater", "Trollface", "Baln", "Blan", "Tom Brady", "Void", "Cooldoom", "Luna", "Millie", "Rose", "Flower", "Niels", "Matthew",
        "Phillip", "Bork", "Molasses", "Quack" }; // community-submitted names
    private static readonly string[] edgeworkStrings =
    {
        "the number of batteries", "the number of battery holders", "the number of indicators", "the number of lit indicators", "the number of unlit indicators", "the number of ports", "the number of port plates",
        "the first digit of the serial number", "the last digit of the serial number", "the greatest digit in the serial number", "the lowest digit in the serial number", "the sum of all digits in the serial number",
        "the number of digits in the serial number", "the number of letters in the serial number", "the total number of modules"
    };
    private static int[] edgeworkNumbers = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    private static readonly string[] possibleHokeyPokeyRules = { "you put your penis out", "you shake your penis all about", "you turn yourself around", "you put your penis in" }; // don't worry, "your penis" will be replaced with a body part, it's just a placeholder
    private static readonly string[] possibleHokeyPokeyParts = { "your right foot", "your left foot", "your right hand", "your left hand", "your butt", "either of your feet", "either of your hands", "any painted body part", "any body part" };
    private static readonly string[] possibleForeheadRules = { "you move to a different chair", "the duck moves to a different chair", "any watering can gets painted", "paint gets added to any watering can", "a chair's seat, bottom, or back gets painted", "your body parts get painted" };

    private int randomStep, numberValue, placeholder, seatNumberValue;
    private int[] seatArray, canArray, paintArray; // used in their corresponding Generate function so that they don't override each other
    private string numberString, seatString, canString, canString2, colorString, colorString2, bodyPartString;
    private int number1 = 0, seat1 = 0, can1 = 0, can2 = 0, color1 = 0, bodyPart1 = 0, organizationIsOverrated = 0, hokey = 0, pokey = 0, okeydokey = 0, cushionRule = 0, uh = 0, oh = 0, stinky = 0;
    private int previousStep = 0; // used to prevent two of the same kinds of steps being used in a row. this doesn't actually work because some rules default to other rules, but it at least decreases the chances
    private int[] doNotUse = { 0, 1, 4, 4, 4, 5, 6, 5, 12, 69, 69, 69, 12, 13, 14, 15, 16, 17 }; // certain types of stages shouldn't be repeated/used after a different certain type of stage, e.g. you shouldn't wash any watering can immediately after painting one
    private int maxStep = 18; // used to block off the last case (hokey pokey) from being used once it's used, because it should only appear once

    private int questionNumber = 0;
    private bool[] validQuestionSubjects = { true, true, true, false, false, false, false, false, false, false, false, false, true };
    // possible question subjects are: 0 - your position / 1 - duck's position / 2 - sitting/standing/handstand / 3 - seat colors / 4 - bottom colors / 5 - back numbers/colors
    //                                 6 - watering can contents / 7 - watering can colors / 8 - body part colors / 9 - duck name / 10 - whoopee cushion position / 11 - Ls on forehead / 12 - compare two seats
    private int[] questionSubjects = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
    private string[] questionAnswers = { "", "", "", "" };
    private int correctAnswer = 0;
    private int subjectIndex = 0;

    void Start () {
        _moduleId = _moduleIdCounter++;
        Module.OnActivate += Activate;
        Init();
	}

    void Activate()
    {
        for (int i = 0; i < 4; i++)
        {
            int j = i;
            btnSelectables[i].OnInteract += delegate ()
            {
                if (!solved)
                    PressButton(j);
                Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, btnSelectables[j].transform);
                btnSelectables[j].AddInteractionPunch();
                return false;
            };
        }
    }

    void Init()
    {
        if (ignoredModules == null)
            ignoredModules = BossModule.GetIgnoredModules("Duck Konundrum", new string[]{
                "14",
                "A>N<D",
                "Bamboozling Time Keeper",
                "Brainf---",
                "Busy Beaver",
                "Duck Konundrum",
                "Everything",
                "Forget Enigma",
                "Forget Everything",
                "Forget It Not",
                "Forget Me Later",
                "Forget Me Not",
                "Forget Perspective",
                "Forget Them All",
                "Forget This",
                "Forget Us Not",
                "Iconic",
                "Kugelblitz",
                "OmegaForget",
                "Organization",
                "RPS Judging",
                "Simon Forgets",
                "Simon's Stages",
                "Souvenir",
                "Tallordered Keys",
                "The Time Keeper",
                "Timing is Everything",
                "The Troll",
                "The Twin",
                "Turn The Key",
                "Übermodule",
                "Ültimate Custom Night",
                "The Very Annoying Button"
            });

        stageCount = Bomb.GetSolvableModuleNames().Where(a => !ignoredModules.Contains(a)).ToList().Count;

        edgeworkNumbers[0] = Bomb.GetBatteryCount();
        edgeworkNumbers[1] = Bomb.GetBatteryHolderCount();
        edgeworkNumbers[2] = Bomb.GetIndicators().Count();
        edgeworkNumbers[3] = Bomb.GetOnIndicators().Count();
        edgeworkNumbers[4] = Bomb.GetOffIndicators().Count();
        edgeworkNumbers[5] = Bomb.GetPortCount();
        edgeworkNumbers[6] = Bomb.GetPortPlateCount();
        edgeworkNumbers[7] = Bomb.GetSerialNumberNumbers().First();
        edgeworkNumbers[8] = Bomb.GetSerialNumberNumbers().Last();
        edgeworkNumbers[9] = Bomb.GetSerialNumberNumbers().Max();
        edgeworkNumbers[10] = Bomb.GetSerialNumberNumbers().Min();
        edgeworkNumbers[11] = Bomb.GetSerialNumberNumbers().Sum();
        edgeworkNumbers[12] = Bomb.GetSerialNumberNumbers().Count();
        edgeworkNumbers[13] = Bomb.GetSerialNumberLetters().Count();
        edgeworkNumbers[14] = Bomb.GetModuleIDs().Count();

        if (stageCount <= 1)
        {
            DebugMsg("Cannot generate stages; autosolving module.");
            StartCoroutine("SolveAnimation");
        }
        
        screenText.text = "";
        StartCoroutine(PrepForStage(displayedStage));
        StartCoroutine(CheckForSolves());
    }

    void GenerateStage()
    {
        if (currentStage == 0)
        {
            currentPos = Random.Range(0, 6);
            duckPos = Random.Range(0, 6);
            stageText = "You should sit on the ";
            if (currentPos == 0)
                stageText += "armchair. The duck should sit on the ";
            else
                stageText += "chair " + currentPos + " chair(s) clockwise from the armchair. The duck should sit on the ";
            if (duckPos == currentPos)
                stageText += "same chair as you. Please hold the duck very carefully.";
            else if (duckPos == 0)
                stageText += "armchair.";
            else if (Random.Range(0, 2) == 0)
                stageText += "chair " + duckPos + " chair(s) clockwise from the armchair.";
            else if (duckPos - currentPos > 0)
                stageText += "chair " + (duckPos - currentPos) + " chair(s) clockwise from you.";
            else
                stageText += "chair " + (currentPos - duckPos) + " chair(s) counter-clockwise from you.";
            StartCoroutine(DisplayStage(stageText));
            for (int i = 0; i < 3; i++)
                btnTexts[i+1].text = "";
        }

        else if (displayedStage + 1 == stageCount)
        {
            stageText = "Press any button to enter submission mode.";
            for (int i = 0; i < 3; i++)
                btnTexts[i + 1].text = "";
            StartCoroutine(DisplayStage(stageText));
        }

        else
        {
            if (stageCount - displayedStage > 5)
                randomStep = Random.Range(0, maxStep + 15) % maxStep; // higher chance of being one of the first 15 kinds of steps
            else
                randomStep = Random.Range(0, 15);

            while (randomStep == doNotUse[previousStep] || randomStep == previousStep)
                randomStep = Random.Range(0, maxStep + 15) % maxStep;
            previousStep = randomStep;
            numberString = canString = colorString = "";
            placeholder = 0; // used for whenever it's needed

            Debug.LogFormat("debug message (step). randomstep = {0}.", randomStep);

            switch (randomStep)
            {
                case 0: // Move to a different chair.
                    seat1 = GenerateSeat(4);
                    while (seat1 == currentPos)
                        seat1 = GenerateSeat(4);
                    currentPos = seat1;
                    stageText = "Move to " + seatString + ".";
                    DebugMsg("You are now in the chair " + currentPos + " chair(s) clockwise from the armchair.");
                    if (currentPos == whoopeeCushionPos)
                        SitOnWhoopeeCushion();
                    if (foreheadActivated && foreheadRule == 0)
                    {
                        foreheadLs++;
                        DebugMsg("You paint another L on your forehead. There are now " + foreheadLs + " Ls on your forehead.");
                    }
                    break;
                case 1: // Move the duck to a different chair.
                    seat1 = GenerateSeat(4);
                    while (seat1 == duckPos)
                        seat1 = GenerateSeat(4);
                    duckPos = seat1;
                    stageText = "Move the duck to " + seatString + ".";
                    if (Random.Range(0, 5) == 0)
                        stageText += " Please be careful with the duck.";
                    DebugMsg("The duck is now in the chair " + duckPos + " chair(s) clockwise from the armchair.");
                    if (duckPos == whoopeeCushionPos)
                        SitOnWhoopeeCushion();
                    if (foreheadActivated && foreheadRule == 1)
                    {
                        foreheadLs++;
                        DebugMsg("You paint another L on your forehead. There are now " + foreheadLs + " Ls on your forehead.");
                    }
                    break;
                case 2: // Fill a watering can with paint.
                    color1 = GeneratePaint(true);
                    can1 = GenerateCan();
                    while (wateringCanContents[can1] == color1)
                    {
                        color1 = GeneratePaint(true);
                        can1 = GenerateCan();
                    }
                    wateringCanContents[can1] = MixColors(wateringCanContents[can1], color1);
                    stageText = "Fill " + canString + " with " + colorString + ".";
                    DebugMsg("The " + sizes[can1] + " watering can is now filled with " + possibleColors[wateringCanContents[can1]] + " paint.");
                    if (foreheadActivated && foreheadRule == 3)
                    {
                        foreheadLs++;
                        DebugMsg("You paint another L on your forehead. There are now " + foreheadLs + " Ls on your forehead.");
                    }
                    break;
                case 3: // Fill a watering can with paint from a watering can.
                    if (wateringCanContents.Count(a => a.Equals(0)) == 2 || wateringCanContents.Distinct().Count() == 1 || (wateringCanContents.Distinct().Count() == 2 && wateringCanContents.Count(a => a.Equals(0)) == 1))
                        goto case 2; // if only one of the cans have paint or there is only one unique paint color throughout all cans, default to case 2=
                    else
                    {
                        can2 = GenerateCan();
                        canString2 = canString;
                        can1 = GenerateCan();
                        while (can1 == can2 || wateringCanContents[can1] == 0 || wateringCanContents[can1] == wateringCanContents[can2])
                            can1 = GenerateCan();
                        wateringCanContents[can2] = MixColors(wateringCanContents[can1], wateringCanContents[can2]);
                        stageText = "Fill " + canString2 + " with some paint from " + canString + " (if that can is empty, do nothing).";
                        DebugMsg("The " + sizes[can2] + " watering can is now filled with " + possibleColors[wateringCanContents[can2]] + " paint.");
                        if (foreheadActivated && foreheadRule == 3 && wateringCanContents[can1] != 0)
                        {
                            foreheadLs++;
                            DebugMsg("You paint another L on your forehead. There are now " + foreheadLs + " Ls on your forehead.");
                        }
                    }

                    break;
                case 4: // Empty a watering can.
                    if (wateringCanContents.Count(a => a.Equals(0)) == 3 || wateringCanContents.Count(a => a.Equals(0)) == 2) // if all watering cans are empty/only one with paint, paint a can with a random paint
                        goto case 7;
                    else
                    {
                        can1 = GenerateCan();
                        while (wateringCanContents[can1] == 0)
                            can1 = GenerateCan();
                        wateringCanContents[can1] = 0;

                        switch (Random.Range(0, 3))
                        {
                            case 0:
                                stageText = "Throw the duck at " + canString + " to knock it over and empty it of all its paint. Then pick the duck back up, put it back where it was, and apologize.";
                                break;
                            case 1:
                                stageText = "Kick " + canString + " to knock it over and empty it of all its paint.";
                                break;
                            case 2:
                                stageText = "Drink all of the paint in " + canString + ".";
                                break;
                        }
                        DebugMsg("The " + sizes[can1] + " watering can is now filled with " + possibleColors[wateringCanContents[can1]] + " paint.");
                    }
                    break;
                case 5: // Wash a watering can.
                    if (wateringCanColors.Count(a => a.Equals(0)) == 3 || wateringCanColors.Count(a => a.Equals(0)) == 2) // if all watering cans are unpainted/only one painted, paint the can with a random paint
                        goto case 7;
                    else
                    {
                        can1 = GenerateCan();
                        while (wateringCanColors[can1] == 0)
                            can1 = GenerateCan();
                        wateringCanColors[can1] = 0;
                        stageText = "Wash the paint off of " + canString + ".";
                        DebugMsg("The " + sizes[can1] + " watering can is now " + possibleColors[wateringCanColors[can1]] + "-colored.");
                    }
                    break;
                case 6: // Pour all of the paint from one watering can to another.
                    if (wateringCanContents.Count(a => a.Equals(0)) == 3) // if none of the cans have paint
                        goto case 2;
                    else
                    {
                        can2 = GenerateCan();
                        canString2 = canString;
                        while (wateringCanContents[can2] == 0)
                        {
                            can2 = GenerateCan();
                            canString2 = canString;
                        }
                        can1 = GenerateCan();
                        while (can1 == can2)
                            can1 = GenerateCan();
                        wateringCanContents[can1] = MixColors(wateringCanContents[can1], wateringCanContents[can2]);
                        wateringCanContents[can2] = 0;
                        stageText = "Dump all of the paint from " + canString2 + " into " + canString + ".";
                        if (foreheadActivated && foreheadRule == 3)
                        {
                            foreheadLs++;
                            DebugMsg("You paint another L on your forehead. There are now " + foreheadLs + " Ls on your forehead.");
                        }
                    }
                    
                    DebugMsg("The " + sizes[can1] + " watering can is now filled with " + possibleColors[wateringCanContents[can1]] + " paint.");
                    break;
                case 7: // Paint a watering can.
                    color1 = GeneratePaint(false);
                    can1 = GenerateCan();
                    while (color1 == wateringCanColors[can1])
                    {
                        color1 = GeneratePaint(false);
                        can1 = GenerateCan();
                    }
                    wateringCanColors[can1] = color1;
                    stageText = "Using " + colorString + ", paint " + canString + ".";
                    DebugMsg("The " + sizes[can1] + " watering can is now " + possibleColors[color1] + "-colored.");
                    if (foreheadActivated && foreheadRule == 2)
                    {
                        foreheadLs++;
                        DebugMsg("You paint another L on your forehead. There are now " + foreheadLs + " Ls on your forehead.");
                    }
                    break;
                case 8: // Paint one of your body parts
                    color1 = GeneratePaint(false);
                    bodyPart1 = GenerateBodyPart();

                    while ((bodyPart1 < 5 && bodyPartColors[bodyPart1] == color1) || (bodyPart1 == 5 && bodyPartColors[0] == color1 && bodyPartColors[1] == color1) || (bodyPart1 == 6 && bodyPartColors[1] == color1 && bodyPartColors[2] == color1))
                    {
                        color1 = GeneratePaint(false);
                        bodyPart1 = GenerateBodyPart();
                    }

                    if (bodyPart1 < 5)
                        bodyPartColors[bodyPart1] = color1;
                    else if (bodyPart1 == 5)
                        bodyPartColors[0] = bodyPartColors[1] = color1;
                    else
                        bodyPartColors[2] = bodyPartColors[3] = color1;

                    stageText = "Paint " + bodyPartString + " with " + colorString + ".";
                    DebugMsg("Now " + bodyPartString + " is/are " + possibleColors[color1] + "-colored.");
                    if (foreheadActivated && foreheadRule == 5)
                    {
                        foreheadLs++;
                        DebugMsg("You paint another L on your forehead. There are now " + foreheadLs + " Ls on your forehead.");
                    }
                    break;
                case 9: // Paint the seat of a chair.
                    color1 = GeneratePaint(false);
                    seat1 = GenerateSeat();
                    while (color1 == seatColors[seat1 % 6])
                    {
                        color1 = GeneratePaint(false);
                        seat1 = GenerateSeat();
                    }

                    seatColors[seat1 % 6] = color1;
                    stageText = "Using " + colorString + ", paint the seat of " + seatString + ".";
                    DebugMsg("The seat of the chair " + (seat1 % 6) + " chair(s) clockwise from the armchair is now " + possibleColors[color1] + "-colored.");
                    if (foreheadActivated && foreheadRule == 4)
                    {
                        foreheadLs++;
                        DebugMsg("You paint another L on your forehead. There are now " + foreheadLs + " Ls on your forehead.");
                    }
                    break;
                case 10: // Paint the bottom of a chair.
                    color1 = GeneratePaint(false);
                    seat1 = GenerateSeat();
                    while (color1 == bottomColors[seat1 % 6])
                    {
                        color1 = GeneratePaint(false);
                        seat1 = GenerateSeat();
                    }

                    bottomColors[seat1 % 6] = color1;
                    stageText = "Using " + colorString + ", paint the bottom of " + seatString + ".";
                    DebugMsg("The bottom of the chair " + (seat1 % 6) + " chair(s) clockwise from the armchair is now " + possibleColors[color1] + "-colored.");
                    if (foreheadActivated && foreheadRule == 4)
                    {
                        foreheadLs++;
                        DebugMsg("You paint another L on your forehead. There are now " + foreheadLs + " Ls on your forehead.");
                    }
                    break;
                case 11: // Paint a number on the back of a chair.
                    stageText = "asdfsjld";
                    while (stageText == "asdfsjld" || stageText.Length > 320) // make sure stage text does not go off screen
                    {
                        color1 = GeneratePaint(false);
                        seat1 = GenerateSeat();
                        number1 = GenerateNumber(6);
                        while (number1 == 0 || number1 == backNumbers[seat1])
                            number1 = GenerateNumber(6);
                        stageText = "Using " + colorString + ", paint the number " + numberString + " on the back of " + seatString + ".";
                    }

                    backNumbers[seat1 % 6] = number1;
                    backColors[seat1 % 6] = color1;
                    DebugMsg("The back of the chair " + (seat1 % 6) + " chair(s) clockwise from the armchair now has a " + possibleColors[color1] + "-colored " + number1 + " on it.");
                    if (foreheadActivated && foreheadRule == 4)
                    {
                        foreheadLs++;
                        DebugMsg("You paint another L on your forehead. There are now " + foreheadLs + " Ls on your forehead.");
                    }
                    break;
                case 12: // Wash a body part.
                    if (bodyPartColors.Count(a => a.Equals(0)) == 5)
                        goto case 8;
                    else
                    {
                        bodyPart1 = Random.Range(0, 5);
                        while (bodyPartColors[bodyPart1] == 0)
                            bodyPart1 = Random.Range(0, 5);
                        bodyPartColors[bodyPart1] = 0;

                        stageText = "Wash the paint off of " + bodyParts[bodyPart1] + ".";
                        DebugMsg("Now " + bodyParts[bodyPart1] + " is/are unpainted.");
                    }
                    break;
                case 13: // Sit/stand/do a handstand on the chair.
                    placeholder = Random.Range(1, 3);
                    chairMethod = (chairMethod + placeholder) % 3;
                    validQuestionSubjects[2] = true;

                    stageText = possibleChairMethods[chairMethod] + " on your chair.";

                    break;
                case 14: // Name the duck/change the duck's name.
                    if (duckName == "") // if the duck has no name, get new name
                    {
                        duckName = duckNames[Random.Range(0, duckNames.Length)];
                        originalDuckName = duckName;
                        stageText = "Let's name the duck " + duckName + ".";
                        DebugMsg("The duck's name is " + duckName + ".");
                    }

                    else // if the duck already has a name, change the name
                    {
                        switch (Random.Range(0, 3))
                        {
                            case 0: // add Jr. to the end of its name
                                if (duckName.EndsWith("Jr"))
                                    goto default;
                                duckName += " Jr";
                                stageText = "Add Jr. to the end of the duck's name.";
                                DebugMsg("The duck's name is " + duckName + ".");
                                break;
                            case 1: // add Dr to the start of its name
                                if (duckName.StartsWith("Dr"))
                                    goto default;
                                duckName = "Dr " + duckName;
                                stageText = "Add Dr. to the start of the duck's name.";
                                DebugMsg("The duck's name is " + duckName + ".");
                                break;
                            case 2: // remove vowels from the duck's name
                                duckName = duckName.ToLowerInvariant();
                                if (!duckName.Contains("a") && !duckName.Contains("e") && !duckName.Contains("i") && !duckName.Contains("o") && !duckName.Contains("u")) // there's probably a better way of doing this but uhhhhhh oh well
                                    goto default;
                                duckName = duckName.Replace("a", string.Empty).Replace("e", string.Empty).Replace("i", string.Empty).Replace("o", string.Empty).Replace("u", string.Empty);
                                duckName = duckName.First().ToString().ToUpperInvariant() + duckName.Substring(1); // capitalize first letter
                                stageText = "Remove all vowels from the duck's name as punishment. (Y is not considered to be a vowel.)";
                                DebugMsg("The duck's name is " + duckName + ".");
                                break;
                            default: // default to painting a body part
                                color1 = GeneratePaint(false);
                                bodyPart1 = GenerateBodyPart();

                                while ((bodyPart1 < 5 && bodyPartColors[bodyPart1] == color1) || (bodyPart1 == 5 && bodyPartColors[0] == color1 && bodyPartColors[1] == color1) || (bodyPart1 == 6 && bodyPartColors[1] == color1 && bodyPartColors[2] == color1))
                                {
                                    color1 = GeneratePaint(false);
                                    bodyPart1 = GenerateBodyPart();
                                }

                                if (bodyPart1 < 5)
                                    bodyPartColors[bodyPart1] = color1;
                                else if (bodyPart1 == 5)
                                    bodyPartColors[0] = bodyPartColors[1] = color1;
                                else
                                    bodyPartColors[2] = bodyPartColors[3] = color1;

                                stageText = "Paint " + bodyPartString + " with " + colorString + ".";
                                DebugMsg("Now " + bodyPartString + " is/are " + possibleColors[color1] + "-colored.");
                                if (foreheadActivated && foreheadRule == 3)
                                {
                                    foreheadLs++;
                                    DebugMsg("You paint another L on your forehead. There are now " + foreheadLs + " Ls on your forehead.");
                                }
                                break;
                        }
                    }

                    break;
                case 15: // Put a whoopee cushion on a chair/move the whoopee cushion/redefine the cushion condition.
                    if (whoopeeCushionPos == 69) // if whoopee cushion hasn't been placed yet
                    {
                        whoopeeCushionPos = GenerateSeat(7);
                        while (whoopeeCushionPos == duckPos || whoopeeCushionPos == currentPos)
                            whoopeeCushionPos = GenerateSeat(7);
                        stageText = "Find a whoopee cushion, and put it on " + seatString + ". Every time you or the duck move to the chair with the whoopee cushion, ";

                        cushionRule = Random.Range(0, 5);

                        switch (cushionRule) // basically generate a stage but limited to more simple options
                        {
                            case 0: // move to a different chair!!!
                                uh = Random.Range(1, 6);
                                oh = Random.Range(0, 2);
                                if (uh == 3)
                                    stageText += "move to the chair across from you.";
                                else if (oh == 0)
                                    stageText += "move " + uh + " chair(s) counter-clockwise.";
                                else
                                    stageText += "move " + uh + " chair(s) clockwise.";
                                break;
                            case 1: // move the duck to a different chair :OOOO
                                uh = Random.Range(1, 6);
                                oh = Random.Range(0, 2);
                                if (uh == 3)
                                    stageText += "move the duck to the chair across from it.";
                                if (oh == 0)
                                    stageText += "move the duck " + uh + " chair(s) counter-clockwise.";
                                else
                                    stageText += "move the duck " + uh + " chair(s) clockwise.";
                                break;
                            case 2: // fill a can with paint from another can! poggers!
                                uh = Random.Range(0, 3);
                                stageText += "fill the " + sizes[uh] + " watering can with some paint from the ";
                                oh = Random.Range(1, 3);
                                stageText += sizes[(uh + oh) % 3] + " watering can. (If that can is empty, do nothing.)";
                                break;
                            case 3: // paint a seat! wow!
                                uh = Random.Range(0, 2);
                                oh = Random.Range(0, 6);
                                stinky = Random.Range(1, 8);
                                if (uh == 0)
                                {
                                    if (oh == 0)
                                        stageText += "paint the seat of your chair with " + possibleColors[stinky] + " paint.";
                                    else
                                        stageText += "paint the seat of the chair " + oh.ToString() + " chair(s) clockwise from your chair with " + possibleColors[stinky] + " paint.";
                                }
                                else
                                {
                                    if (oh == 0)
                                        stageText += "paint the seat of the duck's chair with " + possibleColors[stinky] + " paint.";
                                    else
                                        stageText += "paint the seat of the chair " + oh.ToString() + " chair(s) clockwise from the duck's chair with " + possibleColors[stinky] + " paint.";
                                }
                                break;
                            case 4: // paint a bottom! woah!
                                uh = Random.Range(0, 2);
                                oh = Random.Range(0, 6);
                                stinky = Random.Range(1, 8);
                                if (uh == 0)
                                {
                                    if (oh == 0)
                                        stageText += "paint the bottom of your chair with " + possibleColors[stinky] + " paint.";
                                    else
                                        stageText += "paint the bottom of the chair " + oh.ToString() + " chair(s) clockwise from your chair with " + possibleColors[stinky] + " paint.";
                                }
                                else
                                {
                                    if (oh == 0)
                                        stageText += "paint the bottom of the duck's chair with " + possibleColors[stinky] + " paint.";
                                    else
                                        stageText += "paint the bottom of the chair " + oh.ToString() + " chair(s) clockwise from the duck's chair with " + possibleColors[stinky] + " paint.";
                                }
                                break;
                        }
                        
                        DebugMsg("The whoopee cushion is on the chair " + whoopeeCushionPos + " chair(s) clockwise from the armchair.");
                    }
                    else if (Random.Range(0, 2) == 0) // move the cushion
                    {
                        whoopeeCushionPos = GenerateSeat();
                        stageText = "Move the whoopee cushion to " + seatString + ". If this causes the whoopee cushion to move to your/the duck's chair, activate the whoopee cushion's rule.";
                        if (whoopeeCushionPos == currentPos || whoopeeCushionPos == duckPos)
                            SitOnWhoopeeCushion();
                    }
                    else
                    {
                        stageText = "Now every time you or the duck move to the chair with the whoopee cushion, ";

                        placeholder = Random.Range(0, 6);
                        while (cushionRule == placeholder)
                            placeholder = Random.Range(0, 6);
                        cushionRule = placeholder;

                        switch (cushionRule) // basically generate a stage but limited to more simple options
                        {
                            case 0: // move to a different chair!!!
                                uh = Random.Range(1, 6);
                                oh = Random.Range(0, 2);
                                if (uh == 3)
                                    stageText += "move to the chair across from you instead.";
                                else if (oh == 0)
                                    stageText += "move " + uh + " chair(s) counter-clockwise instead.";
                                else
                                    stageText += "move " + uh + " chair(s) clockwise instead.";
                                break;
                            case 1: // move the duck to a different chair :OOOO
                                uh = Random.Range(1, 6);
                                oh = Random.Range(0, 2);
                                if (uh == 3)
                                    stageText += "move the duck to the chair across from it instead.";
                                if (oh == 0)
                                    stageText += "move the duck " + uh + " chair(s) counter-clockwise instead.";
                                else
                                    stageText += "move the duck " + uh + " chair(s) clockwise instead.";
                                break;
                            case 2: // fill a can with paint from another can! poggers!
                                uh = Random.Range(0, 3);
                                stageText += "fill the " + sizes[uh] + " watering can with some paint from the ";
                                oh = Random.Range(1, 3);
                                stageText += sizes[(uh + oh) % 3] + " watering can instead. (If that can is empty, do nothing.)";
                                break;
                            case 3: // paint a seat! wow!
                                uh = Random.Range(0, 2);
                                oh = Random.Range(0, 6);
                                stinky = Random.Range(1, 8);
                                if (uh == 0)
                                {
                                    if (oh == 0)
                                        stageText += "paint the seat of your chair with " + possibleColors[stinky] + " paint instead.";
                                    else
                                        stageText += "paint the seat of the chair " + oh.ToString() + " chair(s) clockwise from your chair with " + possibleColors[stinky] + " paint instead.";
                                }
                                else
                                {
                                    if (oh == 0)
                                        stageText += "paint the seat of the duck's chair with " + possibleColors[stinky] + " paint instead.";
                                    else
                                        stageText += "paint the seat of the chair " + oh.ToString() + " chair(s) clockwise from the duck's chair with " + possibleColors[stinky] + " paint instead.";
                                }
                                break;
                            case 4: // paint a bottom! woah!
                                uh = Random.Range(0, 2);
                                oh = Random.Range(0, 6);
                                stinky = Random.Range(1, 8);
                                if (uh == 0)
                                {
                                    if (oh == 0)
                                        stageText += "paint the bottom of your chair with " + possibleColors[stinky] + " paint instead.";
                                    else
                                        stageText += "paint the bottom of the chair " + oh.ToString() + " chair(s) clockwise from your chair with " + possibleColors[stinky] + " paint instead.";
                                }
                                else
                                {
                                    if (oh == 0)
                                        stageText += "paint the bottom of the duck's chair with " + possibleColors[stinky] + " paint instead.";
                                    else
                                        stageText += "paint the bottom of the chair " + oh.ToString() + " chair(s) clockwise from the duck's chair with " + possibleColors[stinky] + " paint instead.";
                                }
                                break;
                            case 5: // paint a body part with paint from a can! super epic!
                                uh = Random.Range(0, 4);
                                oh = Random.Range(0, 3);
                                stageText += "dip " + bodyParts[uh] + " in the " + sizes[oh] + " watering can to paint it with whatever paint is in that can (unless the can is empty, then nothing happens).";
                                break;
                        }
                        
                        DebugMsg("The whoopee cushion is on the chair " + whoopeeCushionPos + " chair(s) clockwise from the armchair.");
                    }

                    break;
                case 16: // Paint Ls on your forehead.
                    if (!foreheadActivated)
                    {
                        foreheadColor = Random.Range(1, 8);
                        foreheadRule = Random.Range(0, 6);
                        foreheadActivated = true;
                        stageText = "From now on, every time " + possibleForeheadRules[foreheadRule] + ", paint an L on your forehead with " + possibleColors[foreheadColor] + ".";
                    }

                    else
                    {
                        if (foreheadLs > 1 && Random.Range(0, 3) == 0)
                        {
                            placeholder = Random.Range(1, foreheadLs);
                            foreheadLs -= placeholder;
                            stageText = "Wash " + placeholder + " of the Ls off of your forehead.";
                        }

                        else
                        {
                            placeholder = GenerateNumber(12);
                            foreheadLs += placeholder;
                            stageText = "Paint " + numberString + " Ls on your forehead, in the color that you're supposed to be painting them with.";
                        }
                    }
                    
                    break;
                default: // Hokey pokey!!!
                    hokeyPokeyActive = true;
                    hokeyPokeyRule = Random.Range(0, 4);
                    hokeyPokeySubRule = Random.Range(0, 9);
                    organizationIsOverrated = Random.Range(0, 6);
                    stageText = "Do the Hokey Pokey very, very slowly. To be precise, one step between each stage. Every time " + possibleHokeyPokeyRules[hokeyPokeyRule].Replace("your penis", possibleHokeyPokeyParts[hokeyPokeySubRule]) + ", ";

                    switch (organizationIsOverrated) // basically generate a stage but limited to more simple options
                    {
                        case 0: // move to a different chair!!!
                            hokey = Random.Range(1, 6);
                            pokey = Random.Range(0, 2);
                            if (hokey == 3)
                                stageText += "move to the chair across from you.";
                            else if (pokey == 0)
                                stageText += "move " + hokey + " chair(s) counter-clockwise.";
                            else
                                stageText += "move " + hokey + " chair(s) clockwise.";
                            break;
                        case 1: // move the duck to a different chair :OOOO
                            hokey = Random.Range(1, 6);
                            pokey = Random.Range(0, 2);
                            if (hokey == 3)
                                stageText += "move the duck to the chair across from it.";
                            else if (pokey == 0)
                                stageText += "move the duck " + hokey + " chair(s) counter-clockwise.";
                            else
                                stageText += "move the duck " + hokey + " chair(s) clockwise.";
                            break;
                        case 2: // fill a can with paint from another can! poggers!
                            hokey = Random.Range(0, 3);
                            stageText += "fill the " + sizes[hokey] + " watering can with some paint from the ";
                            pokey = Random.Range(1, 3);
                            stageText += sizes[(hokey + pokey) % 3] + " watering can. (If that can is empty, do nothing.)";
                            break;
                        case 3: // paint a seat! wow!
                            hokey = Random.Range(0, 2);
                            pokey = Random.Range(0, 6);
                            okeydokey = Random.Range(1, 8);
                            if (hokey == 0)
                            {
                                if (pokey == 0)
                                    stageText += "paint the seat of your chair with " + possibleColors[okeydokey] + " paint.";
                                else
                                    stageText += "paint the seat of the chair " + pokey.ToString() + " chair(s) clockwise from your chair with " + possibleColors[okeydokey] + " paint.";
                            }
                            else
                            {
                                if (pokey == 0)
                                    stageText += "paint the seat of the duck's chair with " + possibleColors[okeydokey] + " paint.";
                                else
                                    stageText += "paint the seat of the chair " + pokey.ToString() + " chair(s) clockwise from the duck's chair with " + possibleColors[okeydokey] + " paint.";
                            }
                            break;
                        case 4: // paint a bottom! woah!
                            hokey = Random.Range(0, 2);
                            pokey = Random.Range(0, 6);
                            okeydokey = Random.Range(1, 8);
                            if (hokey == 0)
                            {
                                if (pokey == 0)
                                    stageText += "paint the bottom of your chair with " + possibleColors[okeydokey] + " paint.";
                                else
                                    stageText += "paint the bottom of the chair " + pokey.ToString() + " chair(s) clockwise from your chair with " + possibleColors[okeydokey] + " paint.";
                            }
                            else
                            {
                                if (pokey == 0)
                                    stageText += "paint the bottom of the duck's chair with " + possibleColors[okeydokey] + " paint.";
                                else
                                    stageText += "paint the bottom of the chair " + pokey.ToString() + " chair(s) clockwise from the duck's chair with " + possibleColors[okeydokey] + " paint.";
                            }
                            break;
                        case 5: // paint a body part with paint from a can! super epic!
                            hokey = Random.Range(0, 4);
                            pokey = Random.Range(0, 3);
                            stageText += "dip " + bodyParts[hokey] + " in the " + sizes[pokey] + " watering can to paint it with whatever paint is in that can (unless the can is empty, then nothing happens).";
                            break;
                    }

                    maxStep--; // prevent this kind of step from happening again.
                    break;
            }

            if (hokeyPokeyActive)
            {
                hokeyPokeyStep++;
                if (hokeyPokeyStep > 3 && hokeyPokeyPart > 3)
                {
                    hokeyPokeyActive = false;
                    hokeyPokeyStep = hokeyPokeyPart = 0;
                }
                else if (hokeyPokeyStep > 3)
                {
                    hokeyPokeyStep = 0;
                    hokeyPokeyPart++;
                }

                if ((hokeyPokeyRule == 3 && hokeyPokeyStep != 2 && hokeyPokeyStep != 3) || (hokeyPokeyRule != 3 && hokeyPokeyStep == hokeyPokeyRule))
                    if (hokeyPokeyRule == 2 || (hokeyPokeySubRule < 5 && hokeyPokeyPart == hokeyPokeySubRule) || (hokeyPokeySubRule == 5 && hokeyPokeyPart < 2) || (hokeyPokeySubRule == 6 && (hokeyPokeyPart == 2 || hokeyPokeyPart == 3)) || (hokeyPokeySubRule == 7 && bodyPartColors[hokeyPokeyPart] != 0) || hokeyPokeySubRule == 8)
                    {
                        switch (organizationIsOverrated) // basically generate a stage but limited to more simple options
                        {
                            case 0: // move to a different chair!!!
                                if (pokey == 0)
                                    currentPos = (currentPos + (hokey - 6) * -1) % 6;
                                else
                                    currentPos = (currentPos + hokey) % 6;
                                DebugMsg("The Hokey Pokey rule activated! You are now in the chair " + currentPos + " chair(s) clockwise from the armchair.");
                                if (currentPos == whoopeeCushionPos)
                                    SitOnWhoopeeCushion();
                                if (foreheadActivated && foreheadRule == 0)
                                {
                                    foreheadLs++;
                                    DebugMsg("You paint another L on your forehead. There are now " + foreheadLs + " Ls on your forehead.");
                                }
                                break;
                            case 1: // move the duck to a different chair :OOOO
                                if (pokey == 0)
                                    duckPos = (duckPos + (hokey - 6) * -1) % 6;
                                else
                                    duckPos = (duckPos + hokey) % 6;
                                DebugMsg("The Hokey Pokey rule activated! The duck is now in the chair " + duckPos + " chair(s) clockwise from the armchair.");
                                if (duckPos == whoopeeCushionPos)
                                    SitOnWhoopeeCushion();
                                if (foreheadActivated && foreheadRule == 1)
                                {
                                    foreheadLs++;
                                    DebugMsg("You paint another L on your forehead. There are now " + foreheadLs + " Ls on your forehead.");
                                }
                                break;
                            case 2: // fill a can with paint from another can! poggers!
                                wateringCanContents[hokey] = MixColors(wateringCanContents[hokey], wateringCanContents[(hokey + pokey) % 3]);
                                DebugMsg("The Hokey Pokey rule activated! The " + sizes[hokey] + " watering can is now filled with " + possibleColors[wateringCanContents[hokey]] + " paint.");
                                if (foreheadActivated && foreheadRule == 3 && wateringCanContents[(hokey + pokey) % 3] != 0)
                                {
                                    foreheadLs++;
                                    DebugMsg("You paint another L on your forehead. There are now " + foreheadLs + " Ls on your forehead.");
                                }
                                break;
                            case 3: // paint a seat! wow!
                                if (hokey == 0)
                                {
                                    seatColors[(currentPos + pokey) % 6] = okeydokey;
                                    DebugMsg("The Hokey Pokey rule activated! The seat of the chair " + ((currentPos + pokey) % 6).ToString() + " chair(s) clockwise from the armchair is now " + possibleColors[okeydokey] + ".");
                                }
                                else
                                {
                                    seatColors[(duckPos + pokey) % 6] = okeydokey;
                                    DebugMsg("The Hokey Pokey rule activated! The seat of the chair " + ((duckPos + pokey) % 6).ToString() + " chair(s) clockwise from the armchair is now " + possibleColors[okeydokey] + ".");
                                }
                                if (foreheadActivated && foreheadRule == 3)
                                {
                                    foreheadLs++;
                                    DebugMsg("You paint another L on your forehead. There are now " + foreheadLs + " Ls on your forehead.");
                                }
                                break;
                            case 4: // paint a bottom! woah!
                                if (hokey == 0)
                                {
                                    bottomColors[(currentPos + pokey) % 6] = okeydokey;
                                    DebugMsg("The Hokey Pokey rule activated! The bottom of the chair " + ((currentPos + pokey) % 6).ToString() + " chair(s) clockwise from the armchair is now " + possibleColors[okeydokey] + ".");
                                }
                                else
                                {
                                    bottomColors[(duckPos + pokey) % 6] = okeydokey;
                                    DebugMsg("The Hokey Pokey rule activated! The bottom of the chair " + ((duckPos + pokey) % 6).ToString() + " chair(s) clockwise from the armchair is now " + possibleColors[okeydokey] + ".");
                                }
                                if (foreheadActivated && foreheadRule == 3)
                                {
                                    foreheadLs++;
                                    DebugMsg("You paint another L on your forehead. There are now " + foreheadLs + " Ls on your forehead.");
                                }
                                break;
                            case 5: // paint a body part with paint from a can! super epic!
                                if (wateringCanContents[pokey] != 0)
                                    bodyPartColors[hokey] = wateringCanContents[pokey];
                                DebugMsg("The Hokey Pokey rule activated! Now " + bodyParts[hokey] + " is painted with " + possibleColors[bodyPartColors[hokey]] + " paint.");
                                if (foreheadActivated && foreheadRule == 5)
                                {
                                    foreheadLs++;
                                    DebugMsg("You paint another L on your forehead. There are now " + foreheadLs + " Ls on your forehead.");
                                }
                                break;
                        }
                    }
                Debug.LogFormat("debug message (hokey pokey). hokey pokey is happening. step is {0}. part is {1}.", hokeyPokeyStep, hokeyPokeyPart);
            }
            
            StartCoroutine(DisplayStage(stageText));
        }
    }

    int MixColors(int color1, int color2)
    {
        switch (color1)
        {
            case 0: // if unpainted
                return color2;
            case 1: // if red
                return colorsWithRed[color2];
            case 2: // if yellow
                return colorsWithYellow[color2];
            case 3: // if blue
                return colorsWithBlue[color2];
            case 4: // if orange
                if (color2 == 3 || color2 == 5 || color2 == 6 || color2 == 7)
                    return 7;
                else
                    return 4;
            case 5: // if green
                if (color2 == 1 || color2 == 4 || color2 == 6 || color2 == 7)
                    return 7;
                else
                    return 5;
            case 6: // if purple
                if (color2 == 2 || color2 == 4 || color2 == 5 || color2 == 7)
                    return 7;
                else
                    return 6;
            default: // if brown
                return 7;
        }
    }
    void SitOnWhoopeeCushion()
    {
        switch (cushionRule) // basically generate a stage but limited to more simple options
        {
            case 0: // move to a different chair!!!
                if (oh == 0)
                    currentPos = (currentPos + (uh - 6) * -1) % 6;
                else
                    currentPos = (currentPos + uh) % 6;
                if (currentPos == whoopeeCushionPos)
                    SitOnWhoopeeCushion();
                DebugMsg("The whoopee cushion rule activated! You are now in the chair " + currentPos + " chair(s) clockwise from the armchair.");
                if (foreheadActivated && foreheadRule == 0)
                {
                    foreheadLs++;
                    DebugMsg("You paint another L on your forehead. There are now " + foreheadLs + " Ls on your forehead.");
                }
                break;
            case 1: // move the duck to a different chair :OOOO
                if (oh == 0)
                    duckPos = (duckPos + (uh - 6) * -1) % 6;
                else
                    duckPos = (duckPos + uh) % 6;
                if (duckPos == whoopeeCushionPos)
                    SitOnWhoopeeCushion();
                DebugMsg("The whoopee cushion rule activated! The duck is now in the chair " + duckPos + " chair(s) clockwise from the armchair.");
                if (foreheadActivated && foreheadRule == 1)
                {
                    foreheadLs++;
                    DebugMsg("You paint another L on your forehead. There are now " + foreheadLs + " Ls on your forehead.");
                }
                break;
            case 2: // fill a can with paint from another can! poggers!
                wateringCanContents[uh] = MixColors(wateringCanContents[uh], wateringCanContents[(uh + oh) % 3]);
                DebugMsg("The whoopee cushion rule activated! The " + sizes[uh] + " watering can is now filled with " + possibleColors[wateringCanContents[uh]] + " paint.");
                if (foreheadActivated && foreheadRule == 3 && wateringCanContents[(hokey + pokey) % 3] != 0)
                {
                    foreheadLs++;
                    DebugMsg("You paint another L on your forehead. There are now " + foreheadLs + " Ls on your forehead.");
                }
                break;
            case 3: // paint a seat! wow!
                if (uh == 0)
                {
                    seatColors[(currentPos + oh) % 6] = stinky;
                    DebugMsg("The whoopee cushion rule activated! The seat of the chair " + ((currentPos + oh) % 6).ToString() + " chair(s) clockwise from the armchair is now " + possibleColors[stinky] + ".");
                }
                else
                {
                    seatColors[(duckPos + oh) % 6] = stinky;
                    DebugMsg("The whoopee cushion rule activated! The seat of the chair " + ((duckPos + oh) % 6).ToString() + " chair(s) clockwise from the armchair is now " + possibleColors[stinky] + ".");
                }
                if (foreheadActivated && foreheadRule == 3)
                {
                    foreheadLs++;
                    DebugMsg("You paint another L on your forehead. There are now " + foreheadLs + " Ls on your forehead.");
                }
                break;
            case 4: // paint a bottom! woah!
                if (uh == 0)
                {
                    bottomColors[(currentPos + oh) % 6] = stinky;
                    DebugMsg("The whoopee cushion rule activated! The bottom of the chair " + ((currentPos + oh) % 6).ToString() + " chair(s) clockwise from the armchair is now " + possibleColors[stinky] + ".");
                }
                else
                {
                    bottomColors[(duckPos + oh) % 6] = stinky;
                    DebugMsg("The whoopee cushion rule activated! The bottom of the chair " + ((duckPos + oh) % 6).ToString() + " chair(s) clockwise from the armchair is now " + possibleColors[stinky] + ".");
                }
                if (foreheadActivated && foreheadRule == 3)
                {
                    foreheadLs++;
                    DebugMsg("You paint another L on your forehead. There are now " + foreheadLs + " Ls on your forehead.");
                }
                break;
            case 5: // paint a body part with paint from a can! super epic!
                if (wateringCanContents[oh] != 0)
                    bodyPartColors[uh] = wateringCanContents[oh];
                DebugMsg("The Hokey Pokey rule activated! Now " + bodyParts[uh] + " is painted with " + possibleColors[bodyPartColors[uh]] + " paint.");
                if (foreheadActivated && foreheadRule == 5)
                {
                    foreheadLs++;
                    DebugMsg("You paint another L on your forehead. There are now " + foreheadLs + " Ls on your forehead.");
                }
                break;
        }
    } // activate whoopee cushion rule

    // always use GeneratePaint before GenerateSeat/GenerateCan
    // always use GenerateSeat before GenerateNumber

    int GenerateNumber(int ignoredRule1 = 69, int ignoredRule2 = 69)
    {
        numberValue = Random.Range(5, 13); // 1-5 = 1-5, 6 = number on back of a chair, 7 = number of chair(s) with paint on their seats,
                                           // 8 = number of your body parts that are painted, 9 = number of letters in the duck's name, 10 = sum/digital root of all numbers on the backs of chair(s),
                                           // 11 = number of chair(s) with paint on their bottoms (why did i forget to implement this one), 12 = number of Ls on your forehead
        while (numberValue == ignoredRule1 || numberValue == ignoredRule2)
            numberValue = Random.Range(5, 13);

        Debug.LogFormat("debug message (number). numbervalue = {0}.", numberValue);

        switch (numberValue)
        {
            case 5:
                numberValue = Random.Range(1, 6);
                break;
            case 6:
                if (backNumbers.Count(a => a.Equals(0)) == 6)
                    numberValue = Random.Range(1, 6); // if no numbers on backs of chair(s), default to 1-5
                else
                {
                    int[] shuffled = new int[6] { 0, 1, 2, 3, 4, 5 }.Shuffle().ToArray();
                    foreach (int uhhhhhhhhhhh in shuffled)
                    {
                        if (backNumbers[uhhhhhhhhhhh] != 0)
                        {
                            switch (Random.Range(0, 4)) // 0 = ...chair(s) clockwise from the armchair // 1 = ...chair(s) clockwise from you // 2 = ...chair(s) clockwise from the duck // 3 = ...written in [color]
                            {
                                case 0:
                                    if (uhhhhhhhhhhh == 0)
                                        numberString = "N (where N is the number written on the back of the armchair)";
                                    else
                                        numberString = "N (where N is the number written on the back of the chair " + uhhhhhhhhhhh + " chair(s) clockwise from the armchair)";
                                    break;
                                case 1:
                                    if (currentPos == uhhhhhhhhhhh)
                                        numberString = "N (where N is the number written on the back of the chair you're on)";
                                    else
                                        numberString = "N (where N is the number written on the back of the chair " + Mathf.Abs(currentPos - uhhhhhhhhhhh) + " chair(s) clockwise from you)";
                                    break;
                                case 2:
                                    if (duckPos == uhhhhhhhhhhh)
                                        numberString = "N (where N is the number written on the back of the duck's chair)";
                                    else
                                        numberString = "N (where N is the number written on the back of the chair " + Mathf.Abs(duckPos - uhhhhhhhhhhh) + " chair(s) clockwise from the duck's chair)";
                                    break;
                                case 3:
                                    if (backColors.Count(a => a.Equals(backColors[uhhhhhhhhhhh])) == 1)
                                        numberString = "N (where N is the only number written on the back of a chair in " + possibleColors[backColors[uhhhhhhhhhhh]] + ") ";
                                    else
                                        numberValue = Random.Range(1, 6);
                                    break;
                            }
                            return backNumbers[uhhhhhhhhhhh];
                        }
                    }
                }
                break;
            case 7:
                if (seatColors.Count(a => a.Equals(0)) == 6)
                    numberValue = Random.Range(1, 6); // if no painted seats, default to 1-5
                else
                {
                    numberString = "N (where N is the number of chair(s) with paint on their seats)";
                    return seatColors.Count(a => !a.Equals(0));
                }
                break;
            case 8:
                if (bodyPartColors.Count(a => a.Equals(0)) == 5)
                    numberValue = Random.Range(1, 6); // if none of your body parts are painted, default to 1-5
                else
                {
                    numberString = "N (where N is the number of your body parts with paint on them, not including the forehead)";
                    return bodyPartColors.Count(a => !a.Equals(0));
                }
                break;
            case 9:
                if (duckName == "")
                    numberValue = Random.Range(1, 6); // if the duck has no name, default to 1-5
                else if (Random.Range(0, 2) == 0)
                {
                    numberString = "N (where N is the number of letters in the duck's name, modulo 10)";
                    return duckName.Replace(" ", "").Length % 10;
                }
                else
                {
                    string[] alphabet = "A B C D E F G H I J K L M N O P Q R S T U V W X Y Z".Split(' ');
                    numberString = "N (where N is the position of the first letter of the duck's name in the alphabet)";
                    for (int i = 0; i < alphabet.Length; i++)
                        if (duckName.Substring(0, 1) == alphabet[i])
                            return (i + 1);
                }
                break;
            case 10:
                if (backNumbers.Count(a => a.Equals(0)) == 6)
                    numberValue = Random.Range(1, 6);
                else if (Random.Range(0, 2) == 0)
                {
                    numberString = "N (where N is the sum of all the numbers on the backs of chair(s))";
                    return backNumbers.Sum();
                }
                else
                {
                    numberString = "N (where N is the digital root of all the numbers on the backs of chair(s))";
                    return backNumbers.Sum() % 9;
                }
                break;
            case 11:
                if (bottomColors.Count(a => a.Equals(0)) == 6)
                    numberValue = Random.Range(1, 6); // if no painted seats, default to 1-5
                else
                {
                    numberString = "N (where N is the number of chair(s) with paint on their bottoms)";
                    return bottomColors.Count(a => !a.Equals(0));
                }
                break;
            case 12:
                if (!foreheadActivated)
                    numberValue = Random.Range(1, 6); // if no forehead, default to 1-5
                else
                {
                    numberString = "N (where N is the number of Ls on your forehead)";
                    return foreheadLs;
                }
                break;
        }
        
        if (ignoredRule1 == 5 || Random.Range(0, 4) == 0)
        {
            numberValue = Random.Range(0, 15);
            numberString = "N (where N is " + edgeworkStrings[numberValue] + ")";
            numberValue = edgeworkNumbers[numberValue];
        }

        else
            numberString = numberValue.ToString();
        return numberValue;
    }
    int GenerateSeat(int ignoredRule = 69, bool dontGenerateNumberPlease = false)
    {
        number1 = GenerateNumber() % 6;
        numberValue = Random.Range(0, 8);
        if (ignoredRule == 420)
            while (numberValue == 5 || numberValue == 6)
                numberValue = Random.Range(0, 8);
        else
            while (numberValue == ignoredRule)
                numberValue = Random.Range(0, 8);
        if (dontGenerateNumberPlease)
        {
            number1 = Random.Range(0, 6);
            numberString = number1.ToString();
        }

        seatNumberValue = numberValue;

        Debug.LogFormat("debug message (seat). numbervalue = {0}.", numberValue);
        switch (numberValue)
        {
            case 0: // X chair(s) clockwise from your chair.
                if (ignoredRule == 0 && ignoredRule == 1)
                    goto case 4;
                else if (ignoredRule == 0)
                    goto case 1;
                if (number1 % 6 == 0)
                    seatString = "your chair";
                else if (number1 % 6 == 3)
                    seatString = "the chair opposite from your chair";
                else if (Random.Range(0, 2) == 0)
                {
                    seatString = "the chair " + numberString + " chair(s) counter-clockwise from your chair";
                    number1 = (number1 % 6 - 6) * -1;
                }
                else
                    seatString = "the chair " + numberString + " chair(s) clockwise from your chair";
                return (currentPos + number1) % 6;
            case 1: // X chair(s) clockwise from the duck's chair.
                if (ignoredRule == 4 && ignoredRule == 1)
                    goto case 0;
                else if (ignoredRule == 1)
                    goto case 4;
                if (number1 % 6 == 0)
                    seatString = "the duck's chair";
                else if (number1 % 6 == 3)
                    seatString = "the chair opposite from the duck's chair";
                else if (Random.Range(0, 2) == 0)
                {
                    seatString = "the chair " + numberString + " chair(s) counter-clockwise from the duck's chair";
                    number1 = (number1 % 6 - 6) * -1;
                }
                else
                    seatString = "the chair " + number1 + " chair(s) clockwise from the duck's chair";
                return (duckPos + number1) % 6;
            case 2: // the chair with a [color] seat. if there are multiple, use the first one clockwise from you/the duck.
                if (seatColors.Count(a => a.Equals(0)) == 6 && ignoredRule != 0) // if all seats are unpainted, default to case 0.
                    goto case 0;
                else if (seatColors.Count(a => a.Equals(0)) == 6)
                    goto case 1;
                else
                {
                    seatArray = new int[7] { 1, 2, 3, 4, 5, 6, 7 }.Shuffle().ToArray();
                    foreach (int bluh in seatArray)
                    {
                        if (seatColors.Contains(bluh))
                        {
                            placeholder = Random.Range(0, 2); // decides whether it uses you/duck
                            for (int i = 0; i < 6; i++)
                            {
                                if (placeholder == 0 && seatColors[(currentPos + i) % 6] == bluh)
                                {
                                    seatString = "the chair whose seat is painted with " + possibleColors[bluh] + " paint (if there are multiple, use the first one clockwise from you)";
                                    return (currentPos + i) % 6;
                                }
                                else if (placeholder == 1 && seatColors[(duckPos + i) % 6] == bluh)
                                {
                                    seatString = "the chair whose seat is painted with " + possibleColors[bluh] + " paint (if there are multiple, use the first one clockwise from the duck)";
                                    return (duckPos + i) % 6;
                                }
                            }
                        }
                    }
                }
                return 69420; // should never be returned
            case 3: // the chair with a [color] bottom. if there are multiple, use the first one clockwise from you/the duck.
                if (bottomColors.Count(a => a.Equals(0)) == 6 && ignoredRule != 1) // if all bottoms are unpainted, default to case 1.
                    goto case 1;
                else if (seatColors.Count(a => a.Equals(0)) == 6)
                    goto case 0;
                else
                {
                    seatArray = new int[7] { 1, 2, 3, 4, 5, 6, 7 }.Shuffle().ToArray();
                    foreach (int bluh in seatArray)
                    {
                        if (bottomColors.Contains(bluh))
                        {
                            placeholder = Random.Range(0, 2); // decides whether it uses you/duck
                            for (int i = 0; i < 6; i++)
                            {
                                if (placeholder == 0 && bottomColors[(currentPos + i) % 6] == bluh)
                                {
                                    seatString = "the chair whose bottom is painted with " + possibleColors[bluh] + " paint (if there are multiple, use the first one clockwise from you)";
                                    return (currentPos + i) % 6;
                                }
                                else if (placeholder == 1 && bottomColors[(duckPos + i) % 6] == bluh)
                                {
                                    seatString = "the chair whose bottom is painted with " + possibleColors[bluh] + " paint (if there are multiple, use the first one clockwise from the duck)";
                                    return (duckPos + i) % 6;
                                }
                            }
                        }
                    }
                }
                return 69420; // should never be returned
            case 4: // the chair X chair(s) clockwise from the armchair
                if (ignoredRule == 4)
                    goto case 0;
                if (number1 % 6 == 0)
                    seatString = "the armchair";
                else if (number1 % 6 == 3)
                    seatString = "the chair opposite from the armchair";
                else if (Random.Range(0, 2) == 0)
                {
                    seatString = "the chair " + numberString + " chair(s) counter-clockwise from the armchair";
                    number1 = (number1 % 6 - 6) * -1;
                }
                else
                    seatString = "the chair " + numberString + " chair(s) clockwise from the armchair";
                return number1;
            case 5: // the chair with the number N on the back
                seatArray = new int[6] { 0, 1, 2, 3, 4, 5 }.Shuffle().ToArray();
                for (int i = 0; i < 6; i++)
                {
                    if (seatArray[i] != 0 && backNumbers.Count(a => a.Equals(backNumbers[seatArray[i]])) == 1) // if there is only one of that number
                    {
                        seatString = "the chair with " + backNumbers[seatArray[i]] + " painted on the back of it"; // i could probably make this another GenerateNumber() but sometimes you just have to know when to stop
                        return seatArray[i];
                    }
                }

                if (ignoredRule != 4) // default to case 4 if it's not ignored
                    goto case 4;

                else if (ignoredRule != 0) // if it is ignored, default to case 0
                    goto case 0;

                else
                    goto case 1;
            case 6: // the chair with a [color]-painted number on the back
                seatArray = new int[6] { 0, 1, 2, 3, 4, 5 }.Shuffle().ToArray();
                for (int i = 0; i < 6; i++)
                {
                    if (seatArray[i] != 0 && backColors.Count(a => a.Equals(backColors[seatArray[i]])) == 1) // if there is only one of that color number
                    {
                        seatString = "the chair with a number painted with " + possibleColors[backColors[seatArray[i]]] + " on the back of it";
                        return seatArray[i];
                    }
                }

                if (ignoredRule != 4) // default to case 4 if it's not ignored
                    goto case 4;

                else if (ignoredRule != 1) // if it is ignored, default to case 1
                    goto case 1;

                else
                    goto case 0;
            case 7: // the chair X chair(s) clockwise from the whoopee cushion
                if (whoopeeCushionPos == 69)
                    goto case 0;
                else if (number1 % 6== 0)
                    seatString = "the chair with the whoopee cushion";
                else if (number1 % 6 == 3)
                    seatString = "the chair across from the whoopee cushion";
                else if (Random.Range(0, 2) == 0)
                {
                    seatString = "the chair " + numberString + " chair(s) counter-clockwise from the whoopee cushion";
                    number1 = (number1 % 6 - 6) * -1;
                }
                else
                    seatString = "the chair " + numberString + " chair(s) clockwise from the whoopee cushion";
                return (whoopeeCushionPos + number1) % 6;
            default:
                goto case 1; // should never be returned
        }
    }
    int GenerateCan(int ignoredRule = 69)
    {
        numberValue = Random.Range(2, 6); // 0/1/2 = smallest/medium/largest, 3 = the only paint-filled/empty can, 4 = the only painted/unpainted can, 5 = the [color] watering can
        while (numberValue == ignoredRule)
            numberValue = Random.Range(2, 6);
        Debug.LogFormat("debug message (can). numbervalue = {0}.", numberValue);
        switch (numberValue)
        {
            case 2:
                numberValue = Random.Range(0, 3);
                break;
            case 3:
                if (wateringCanContents.Count(a => a.Equals(0)) == 2) // if there is one can with paint in it, use the paint-filled can
                {
                    canString = "the only watering can with paint in it";
                    for (int i = 0; i < 3; i++)
                        if (wateringCanContents[i] != 0)
                            return i;
                }
                else if (wateringCanContents.Count(a => a.Equals(0)) == 1) // if there is one empty can, use the empty can
                {
                    canString = "the only empty watering can";
                    return Array.IndexOf(wateringCanContents, 0);
                }
                else
                {
                    numberValue = Random.Range(0, 3);
                }
                break;
            case 4:
                if (wateringCanColors.Count(a => a.Equals(0)) == 2) // if there is one painted, use the painted can
                {
                    canString = "the only painted watering can";
                    for (int i = 0; i < 3; i++)
                        if (wateringCanColors[i] != 0)
                            return i;
                }
                else if (wateringCanColors.Count(a => a.Equals(0)) == 1) // if there is one unpainted can, use the unpainted can
                {
                    canString = "the only unpainted watering can";
                    return Array.IndexOf(wateringCanColors, 0);
                }
                else
                {
                    numberValue = Random.Range(0, 3);
                }
                break;
            case 5:
                if (ignoredRule == 5)
                    goto case 2;
                canArray = new int[3] { 0, 1, 2 }.Shuffle().ToArray();
                for (int i = 0; i < 3; i++)
                {
                    if (wateringCanColors[canArray[i]] != 0 && wateringCanColors.Count(a => a.Equals(wateringCanColors[canArray[i]])) == 1)
                    {
                        colorString2 = colorString; // save the colorstring from generatepaint
                        placeholder = wateringCanColors[canArray[i] % 3];
                        colorString = possibleColors[placeholder] + " paint";
                        canString = "the watering can that's painted with " + colorString + "";
                        colorString = colorString2; // set colorstring back to what it should be
                        return Array.LastIndexOf(wateringCanColors, wateringCanColors[canArray[i]]);
                    }
                }
                numberValue = Random.Range(0, 3); // if the above doesn't work because 1) all cans are unpainted or 2) no unique paint colors, default to sizes
                break;
        }

        canString = "the " + sizes[numberValue] + " watering can";
        return numberValue;
    }
    int GeneratePaint(bool primaryOnlyPlease, int ignoredRule = 69) // primaryOnlyPlease only affects case 0/6
    {
        numberValue = Random.Range(0, 8);
        while (numberValue == ignoredRule)
            numberValue = Random.Range(0, 8);
        Debug.LogFormat("debug message (color). numbervalue = {0}.", numberValue);

        switch (numberValue)
        {
            case 1: // color on the seat of a chair
                if (seatColors.Count(a => a.Equals(0)) == 6) // default to case 0
                    break;
                placeholder = GenerateSeat(2);
                while (seatColors[placeholder % 6] == 0)
                    placeholder = GenerateSeat(2);
                colorString = "the paint on the seat of " + seatString;
                return seatColors[placeholder % 6];
            case 2: // color on the bottom of a chair
                if (bottomColors.Count(a => a.Equals(0)) == 6) // default to case 0
                    break;
                placeholder = GenerateSeat(3);
                while (bottomColors[placeholder % 6] == 0)
                    placeholder = GenerateSeat(3);
                colorString = "the paint on the bottom of " + seatString;
                return bottomColors[placeholder % 6];
            case 3: // color on a watering can
                if (wateringCanColors.Count(a => a.Equals(0)) == 3) // default to case 0
                    break;
                placeholder = GenerateCan(5);
                while (wateringCanColors[placeholder] == 0)
                    placeholder = GenerateCan(5);
                colorString = "the paint on " + canString;
                return wateringCanColors[placeholder];
            case 4: // color in a watering can
                if (wateringCanContents.Count(a => a.Equals(0)) == 3) // default to case 0
                    break;
                placeholder = GenerateCan(5);
                while (wateringCanContents[placeholder] == 0)
                    placeholder = GenerateCan(5);
                colorString = "the paint inside " + canString;
                return wateringCanContents[placeholder];
            case 5: // color on one of your body parts
                if (bodyParts.Count(a => a.Equals(0)) == 5) // default to case 0
                    break;
                paintArray = new int[5] { 0, 1, 2, 3, 4 }.Shuffle().ToArray();
                for (int i = 0; i < 5; i++)
                {
                    if (bodyPartColors[paintArray[i]] != 0)
                    {
                        colorString = "paint that's the same color as " + bodyParts[paintArray[i]];
                        return bodyPartColors[paintArray[i]];
                    }
                }
                break;
            case 6: // color on back of chair
                if (backColors.Count(a => a.Equals(0)) == 6) // default to case 0
                    break;
                placeholder = GenerateSeat(6);
                while (backColors[placeholder] == 0)
                    placeholder = GenerateSeat(6);
                colorString = "the paint of the number written on the back of " + seatString;
                return backColors[placeholder];
            case 7: // color of your forehead
                if (foreheadLs == 0)
                    break;
                colorString = "the color of the Ls on your forehead";
                return foreheadColor;
        }

        if (primaryOnlyPlease) // makeshift case 0
            placeholder = Random.Range(1, 4); // primary colors
        else
            placeholder = Random.Range(1, 8); // primary and secondary colors and also brown

        colorString = possibleColors[placeholder] + " paint";
        return placeholder;
    }
    int GenerateBodyPart()
    {
        numberValue = Random.Range(0, 3);
        Debug.LogFormat("debug message (can). numbervalue = {0}.", numberValue);
        switch (numberValue)
        {
            case 1:
                if (bodyPartColors.Count(a => a.Equals(0)) == 1) // the only unpainted body part
                {
                    bodyPartString = "your only unpainted body part";
                    return Array.IndexOf(bodyPartColors, 0);
                }
                else if (bodyPartColors.Count(a => a.Equals(0)) == 4) // the only painted body part
                {
                    bodyPartString = "your only painted body part";
                    for (int i = 0; i < 5; i++)
                        if (bodyPartColors[i] != 0)
                            return i;
                    break;
                }
                else
                {
                    if (Random.Range(0, 2) == 0) // either default to case 0 or 2
                        break;
                    bodyPartString = "all of your body parts that are touching the chair";
                    if (chairMethod == 0)
                        return 4;
                    else if (chairMethod == 1)
                        return 5;
                    else
                        return 6;
                }
            case 2: // the body part(s) that are touching the chair
                bodyPartString = "all of your body parts that are touching the chair";
                if (chairMethod == 0)
                    return 4;
                else if (chairMethod == 1)
                    return 5;
                else
                    return 6;
        }

        placeholder = Random.Range(0, 7);
        bodyPartString = bodyParts[placeholder];
        return placeholder;
    }

    void PressButton(int btnNumber)
    {
        if (readyToStartSubmission)
        {
            DebugMsg("Ready for input!");
            StartCoroutine(PrepForSubmission());
            readyToStartSubmission = false;
        }
        else if (!readyForInput)
            Module.HandleStrike();
        else if (!animationPlaying)
        {
            if (correctAnswer == btnNumber)
            {
                Audio.PlaySoundAtTransform("glitch" + Random.Range(1, 12).ToString(), Module.transform);
                if (questionNumber > 0)
                    LEDRenderers[questionNumber - 1].material.color = colors[1];
                btnRenderers[btnNumber].material.color = colors[1];
                subjectIndex++;
                GenerateQuestion();
            }
            else if (btnTexts[btnNumber].text != "")
            {
                Audio.PlaySoundAtTransform("glitch" + Random.Range(1, 12).ToString(), Module.transform);
                Module.HandleStrike();
                DebugMsg("You pressed " + btnTexts[btnNumber].text + ". That was not correct.");
                while (btnTexts[btnNumber].text != "")
                    btnTexts[btnNumber].text = btnTexts[btnNumber].text.Substring(0, btnTexts[btnNumber].text.Length - 1);
            }
            else
            {
                // play a sound or something idfk
            }
        }
    }

    void GenerateQuestion()
    {
        for (int i = 0; i < 4; i++)
            btnTexts[i].text = "";
        if (questionNumber > 2)
            StartCoroutine(SolveAnimation());
        else
        {
            while (!validQuestionSubjects[questionSubjects[subjectIndex]])
                subjectIndex = (subjectIndex + 1) % questionSubjects.Length;
            correctAnswer = Random.Range(0, 4);
            questionAnswers = new string[4] { "", "", "", "" };

            switch (questionSubjects[subjectIndex])
            {
                case 0: // question based on your current position
                    seat1 = GenerateSeat(0) % 6;
                    if (Random.Range(0, 2) == 0) // X is the same as Y
                    {
                        while (seat1 != currentPos)
                            seat1 = GenerateSeat(0) % 6;
                        correctAnswer = 0;
                    }

                    else // X is not the same as Y
                    {
                        while (seat1 == currentPos)
                            seat1 = GenerateSeat(0) % 6;
                        correctAnswer = 1;
                    }

                    stageText = "Is the chair that you're sitting in the same as " + seatString + "?";
                    questionAnswers[0] = "yes";
                    questionAnswers[1] = "no";
                    break;
                case 1: // question based on the duck's position
                    seat1 = GenerateSeat(1) % 6;
                    if (Random.Range(0, 2) == 0) // X is the same as Y
                    {
                        while (seat1 != duckPos)
                            seat1 = GenerateSeat(1) % 6;
                        correctAnswer = 0;
                    }

                    else // X is not the same as Y
                    {
                        while (seat1 == duckPos)
                            seat1 = GenerateSeat(1) % 6;
                        correctAnswer = 1;
                    }

                    stageText = "Is the chair that the duck is sitting in the same as " + seatString + "?";
                    questionAnswers[0] = "yes";
                    questionAnswers[1] = "no";
                    break;
                case 2: // question based on whether you're sitting/standing/doing a handstand
                    string[] betterChairMethods = { "sitting", "standing", "a handstand", "tap dancing" };
                    stageText = "What are you doing?";
                    questionAnswers[correctAnswer] = betterChairMethods[chairMethod];
                    for (int i = 1; i < 4; i++)
                    {
                        placeholder = Random.Range(0, 4);
                        while (questionAnswers.Contains(betterChairMethods[placeholder]))
                            placeholder = Random.Range(0, 4);
                        questionAnswers[(correctAnswer + i) % 4] = betterChairMethods[placeholder];
                    }
                    break;
                case 3: // question based on seat colors
                    seat1 = GenerateSeat(2) % 6;
                    if (Random.Range(0, 4) == 0) // if 25% chance goes through, ask about whether or not a seat is painted instead
                    {
                        if (Random.Range(0, 2) == 0 || !seatColors.Contains(0)) // seat is painted
                        {
                            while (seatColors[seat1] == 0)
                                seat1 = GenerateSeat(2) % 6;
                            correctAnswer = 0;
                        }

                        else // seat is not painted
                        {
                            while (seatColors[seat1] != 0)
                                seat1 = GenerateSeat(2) % 6;
                            correctAnswer = 1;
                        }

                        stageText = "Is there paint on the seat of " + seatString + "?";
                        questionAnswers[0] = "yes";
                        questionAnswers[1] = "no";
                    }

                    else
                    {
                        if (Random.Range(0, 4) != 0) // 25% chance of the seat generated being allowed to be unpainted
                            while (seatColors[seat1] == 0)
                                seat1 = GenerateSeat(2) % 6;
                        stageText = "What color is the seat of " + seatString + "?";
                        questionAnswers[correctAnswer] = possibleColors[seatColors[seat1]].Replace("no", "unpainted");
                        for (int i = 1; i < 4; i++)
                        {
                            placeholder = Random.Range(0, 8);
                            while (questionAnswers.Contains(possibleColors[placeholder].Replace("no", "unpainted")))
                                placeholder = Random.Range(0, 8);
                            questionAnswers[(correctAnswer + i) % 4] = possibleColors[placeholder].Replace("no", "unpainted");
                        }
                    }
                    break;
                case 4: // question based on bottom colors
                    seat1 = GenerateSeat(3) % 6;
                    if (Random.Range(0, 4) == 0) // if 25% chance goes through, ask about whether or not a bottom is painted instead
                    {
                        if (Random.Range(0, 2) == 0 || !bottomColors.Contains(0)) // bottom is painted
                        {
                            while (bottomColors[seat1] == 0)
                                seat1 = GenerateSeat(3) % 6;
                            correctAnswer = 0;
                        }

                        else // bottom is not painted
                        {
                            while (bottomColors[seat1] != 0)
                                seat1 = GenerateSeat(3) % 6;
                            correctAnswer = 1;
                        }

                        stageText = "Is there paint on the bottom of " + seatString + "?";
                        questionAnswers[0] = "yes";
                        questionAnswers[1] = "no";
                    }

                    else
                    {
                        if (Random.Range(0, 4) != 0) // 25% chance of the bottom generated being allowed to be unpainted
                            while (bottomColors[seat1] == 0)
                                seat1 = GenerateSeat(3) % 6;
                        stageText = "What color is the bottom of " + seatString + "?";
                        questionAnswers[correctAnswer] = possibleColors[bottomColors[seat1]].Replace("no", "unpainted");
                        for (int i = 1; i < 4; i++)
                        {
                            placeholder = Random.Range(0, 8);
                            while (questionAnswers.Contains(possibleColors[placeholder].Replace("no", "unpainted")))
                                placeholder = Random.Range(0, 8);
                            questionAnswers[(correctAnswer + i) % 4] = possibleColors[placeholder].Replace("no", "unpainted");
                        }
                    }
                    break;
                case 5: // question based on back numbers/colors
                    seat1 = GenerateSeat(420) % 6;
                    if (Random.Range(0, 4) == 0) // if 25% chance goes through, ask about whether or not a chair has a number instead
                    {
                        if (Random.Range(0, 2) == 0 || !backColors.Contains(0)) // chair has a number
                        {
                            Debug.LogFormat("chair has a number.");
                            while (backColors[seat1] == 0)
                                seat1 = GenerateSeat(420) % 6;
                            correctAnswer = 0;
                        }

                        else // chair does not have a number
                        {
                            Debug.LogFormat("chair does not have a number.");
                            while (backColors[seat1] != 0)
                                seat1 = GenerateSeat(420) % 6;
                            correctAnswer = 1;
                        }

                        stageText = "Is there a number painted on the back of " + seatString + "?";
                        questionAnswers[0] = "yes";
                        questionAnswers[1] = "no";
                    }

                    else if (Random.Range(0, 2) == 0) // ask about the number
                    {
                        while (backNumbers[seat1] == 0)
                            seat1 = GenerateSeat(5) % 6;
                        stageText = "What number is painted on the back of " + seatString + "?";
                        questionAnswers[correctAnswer] = backNumbers[seat1].ToString();
                        for (int i = 1; i < 4; i++)
                        {
                            placeholder = Random.Range(1, 10);
                            while (questionAnswers.Contains(placeholder.ToString()))
                                placeholder = Random.Range(1, 10);
                            questionAnswers[(correctAnswer + i) % 4] = placeholder.ToString();
                        }
                    }

                    else // ask about the color
                    {
                        while (backColors[seat1] == 0)
                            seat1 = GenerateSeat(6) % 6;
                        stageText = "What color is the number painted on the back of " + seatString + "?";
                        questionAnswers[correctAnswer] = possibleColors[backColors[seat1]];
                        for (int i = 1; i < 4; i++)
                        {
                            placeholder = Random.Range(1, 8);
                            while (questionAnswers.Contains(possibleColors[placeholder]))
                                placeholder = Random.Range(1, 8);
                            questionAnswers[(correctAnswer + i) % 4] = possibleColors[placeholder];
                        }
                    }
                    break;
                case 6: // question based on watering can contents
                    can1 = GenerateCan();
                    while (canString == "the only empty watering can")
                        can1 = GenerateCan();
                    stageText = "What paint is inside " + canString + "?";
                    questionAnswers[correctAnswer] = possibleColors[wateringCanContents[can1]] + " paint";
                    for (int i = 1; i < 4; i++)
                    {
                        placeholder = Random.Range(0, 8);
                        while (questionAnswers.Contains(possibleColors[placeholder] + " paint"))
                            placeholder = Random.Range(0, 8);
                        questionAnswers[(correctAnswer + i) % 4] = possibleColors[placeholder] + " paint";
                    }
                    break;
                case 7: // question based on watering can colors
                    can1 = GenerateCan(5);
                    while (canString == "the only unpainted watering can")
                        can1 = GenerateCan(5);
                    stageText = "What paint is " + canString + " painted with?";
                    questionAnswers[correctAnswer] = possibleColors[wateringCanColors[can1]] + " paint";
                    for (int i = 1; i < 4; i++)
                    {
                        placeholder = Random.Range(0, 8);
                        while (questionAnswers.Contains(possibleColors[placeholder] + " paint"))
                            placeholder = Random.Range(0, 8);
                        questionAnswers[(correctAnswer + i) % 4] = possibleColors[placeholder] + " paint";
                    }
                    break;
                case 8: // question based on body part colors
                    bodyPart1 = Random.Range(0, 5);
                    if (Random.Range(0, 4) != 0)
                        while (bodyPartColors[bodyPart1] == 0)
                            bodyPart1 = Random.Range(0, 5);
                    stageText = "What is the color of " + bodyParts[bodyPart1] + "?";
                    questionAnswers[correctAnswer] = possibleColors[bodyPartColors[bodyPart1]];
                    for (int i = 1; i < 4; i++)
                    {
                        placeholder = Random.Range(1, 8);
                        while (questionAnswers.Contains(possibleColors[placeholder]))
                            placeholder = Random.Range(1, 8);
                        questionAnswers[(correctAnswer + i) % 4] = possibleColors[placeholder];
                    }
                    break;
                case 9: // question based on the duck's name
                    if (duckName.Length > 12)
                    {
                        stageText = "How many letters are in the duck's name?";
                        questionAnswers[correctAnswer] = duckName.Replace(" ", "").Length.ToString();
                    }
                    else
                    {
                        stageText = "What is the duck's name?";
                        questionAnswers[correctAnswer] = duckName;
                    }
                    if (duckNames.Contains(duckName)) // if the duck's name has not been edited yet
                    {
                        for (int i = 1; i < 4; i++)
                        {
                            placeholder = Random.Range(0, duckNames.Length);
                            while (questionAnswers.Contains(duckNames[placeholder]))
                                placeholder = Random.Range(0, duckNames.Length);
                            questionAnswers[(correctAnswer + i) % 4] = duckNames[placeholder];
                        }
                    }
                    else if (duckName.Length > 12) // if the question is how many letters are in the duck's name
                    {
                        for (int i = 1; i < 4; i++)
                        {
                            placeholder = Random.Range(3, 16);
                            while (questionAnswers.Contains(placeholder.ToString()))
                                placeholder = Random.Range(3, 16);
                            questionAnswers[(correctAnswer + i) % 4] = placeholder.ToString();
                        }
                    }

                    else
                    {
                        string[] uhhhhhhhhhhhhhhh = { originalDuckName, originalDuckName + " Jr", "Dr " + originalDuckName, "Dr " + originalDuckName + " Jr", originalDuckName.Replace("a", string.Empty).Replace("e", string.Empty).Replace("i", string.Empty).Replace("o", string.Empty).Replace("u", string.Empty), originalDuckName.Replace("a", string.Empty).Replace("e", string.Empty).Replace("i", string.Empty).Replace("o", string.Empty).Replace("u", string.Empty) + " Jr", "Dr " + originalDuckName.Replace("a", string.Empty).Replace("e", string.Empty).Replace("i", string.Empty).Replace("o", string.Empty).Replace("u", string.Empty), "Dr " + originalDuckName.Replace("a", string.Empty).Replace("e", string.Empty).Replace("i", string.Empty).Replace("o", string.Empty).Replace("u", string.Empty) + " Jr" };
                        uhhhhhhhhhhhhhhh = uhhhhhhhhhhhhhhh.Shuffle().ToArray();
                        int uhhhhhhhhhhhhhhhIndex = 0;

                        for (int i = 1; i < 4; i++)
                        {
                            while (questionAnswers.Contains(uhhhhhhhhhhhhhhh[uhhhhhhhhhhhhhhhIndex]) || uhhhhhhhhhhhhhhh[uhhhhhhhhhhhhhhhIndex].Length > 12)
                                uhhhhhhhhhhhhhhhIndex++;
                            questionAnswers[(correctAnswer + i) % 4] = uhhhhhhhhhhhhhhh[uhhhhhhhhhhhhhhhIndex].Substring(0, 1).ToUpperInvariant() + uhhhhhhhhhhhhhhh[uhhhhhhhhhhhhhhhIndex].Substring(1);
                        }
                    }
                    break;
                case 10: // question based on the whoopee cushion
                    if (Random.Range(0, 2) == 0) // X is the same as Y
                    {
                        seat1 = GenerateSeat(0) % 6;
                        while (seat1 != whoopeeCushionPos)
                            seat1 = GenerateSeat(0) % 6;
                        correctAnswer = 0;
                    }

                    else // X is not the same as Y
                    {
                        seat1 = GenerateSeat(0) % 6;
                        while (seat1 == whoopeeCushionPos)
                            seat1 = GenerateSeat(0) % 6;
                        correctAnswer = 1;
                    }

                    stageText = "Is the chair that the whoopee cushion is on the same as " + seatString + "?";
                    questionAnswers[0] = "yes";
                    questionAnswers[1] = "no";
                    break;
                case 11: // question based on the Ls on your forehead
                    stageText = "How many Ls are on your forehead?";
                    questionAnswers[correctAnswer] = foreheadLs.ToString();
                    for (int i = 1; i < 4; i++)
                    {
                        placeholder = Random.Range(foreheadLs - 5, foreheadLs + 5);
                        while (placeholder < 0 || questionAnswers.Contains(placeholder.ToString()))
                            placeholder += 5;
                        while (questionAnswers.Contains(placeholder.ToString()))
                            placeholder = Random.Range(foreheadLs - 5, foreheadLs + 5);
                        questionAnswers[(correctAnswer + i) % 4] = placeholder.ToString();
                    }
                    break;
                case 12: // compare two seats
                    int seat2 = GenerateSeat();
                    string seat2String = seatString;
                    seat1 = GenerateSeat(seatNumberValue);
                    if (Random.Range(0, 2) == 0) // X is the same as Y
                    {
                        while (seat1 != seat2)
                            seat1 = GenerateSeat(seatNumberValue);
                        correctAnswer = 0;
                    }
                    else // X is not the same as Y
                    {
                        while (seat1 == seat2)
                            seat1 = GenerateSeat(seatNumberValue);
                        correctAnswer = 1;
                    }
                    stageText = "Is " + seatString + " the same as " + seat2String + "?";
                    questionAnswers[0] = "yes";
                    questionAnswers[1] = "no";
                    break;
            }

            DebugMsg("The question was '" + stageText + "'. The correct answer is " + questionAnswers[correctAnswer] + ".");
            StartCoroutine(DisplayQuestion());
            questionNumber++;
        }
    }

    IEnumerator CheckForSolves()
    {
        while (!solved)
        {
            if (ignoredModules != null)
            {
                solveCount = Bomb.GetSolvedModuleNames().Where(a => !ignoredModules.Contains(a)).ToList().Count;
                if (solveCount > currentStage)
                {
                    currentStage++;
                    StartCoroutine(PrepForStage(currentStage));
                }
            }

            yield return new WaitForSeconds(.1f);
        }
    }
    IEnumerator PrepForStage(int thisDisplayedStage)
    {
        //DebugMsg("OIGFDLJLFKGJDFLGF");
        if (stageCount == 0)
            yield return null;
        else
        {
            //DebugMsg("Woahgh!");
            queuedStages++;

            while (animationPlaying)
            {
                yield return new WaitForSeconds(.1f);
                btnTexts[2].text = "(" + queuedStages + " stage(s)";
                btnTexts[3].text = "are queued.)";
            }

            GenerateStage();
        }
    }
    IEnumerator DisplayStage(string textToDisplay)
    {
        animationPlaying = true;
        displayedStage++;
        queuedStages--;
        btnTexts[2].text = "(" + queuedStages + " stage(s)";
        btnTexts[3].text = "are queued.)";
        btnTexts[0].text = "Loading...";
        if (queuedStages == 0)
            btnTexts[2].text = btnTexts[3].text = "";
        string line = "";
        DebugMsg("Displying Stage " + displayedStage + ". It says: " + textToDisplay);

        if (displayedStage >= stageCount)
            readyToStartSubmission = true;

        while (screenText.text != "")
        {
            screenText.text = screenText.text.Substring(0, screenText.text.Length - 1);
            yield return new WaitForSeconds(.002f);
        }

        for (int i = 0; i < textToDisplay.Length; i++)
        {
            line += textToDisplay[i];
            screenText.text += textToDisplay[i];

            if (line.Length > 33 && !line.EndsWith(" "))
            {
                screenText.text = screenText.text.Substring(0, screenText.text.LastIndexOf(' '));
                line = line.Substring(line.LastIndexOf(' ') + 1);
                screenText.text += "\n" + line;
            }
            yield return new WaitForSeconds(.002f);
        }

        btnTexts[0].text = "Stage " + displayedStage;

        yield return new WaitForSeconds(2);
        
        // currentStage++;
        animationPlaying = false;

    }
    IEnumerator PrepForSubmission()
    {
        if (displayedStage >= stageCount)
            yield return null;
        Debug.LogFormat("bluh");
        readyForInput = true;
        while (animationPlaying || currentStage + 1 <= stageCount)
            yield return new WaitForSeconds(.1f);
        animationPlaying = true;
        questionsAreDisplayed = true;
        Audio.PlaySoundAtTransform("glitch" + Random.Range(1, 12).ToString(), Module.transform);
        btnTexts[0].text = btnTexts[1].text = btnTexts[2].text = btnTexts[3].text = "";

        while (screenText.text != "")
        {
            screenText.text = screenText.text.Substring(0, screenText.text.Length - 1);
            yield return new WaitForSeconds(.01f);
        }

        if (seatColors.Count(a => a.Equals(0)) < 6) 
            validQuestionSubjects[3] = true;
        if (bottomColors.Count(a => a.Equals(0)) < 6)
            validQuestionSubjects[4] = true;
        if (backColors.Count(a => a.Equals(0)) < 6)
            validQuestionSubjects[5] = true;
        if (wateringCanContents.Count(a => a.Equals(0)) < 3)
            validQuestionSubjects[6] = true;
        if (wateringCanColors.Count(a => a.Equals(0)) < 3)
            validQuestionSubjects[7] = true;
        if (bodyPartColors.Count(a => a.Equals(0)) < 5)
            validQuestionSubjects[8] = true;
        if (duckName != "")
            validQuestionSubjects[9] = true;
        if (whoopeeCushionPos != 69)
            validQuestionSubjects[10] = true;
        if (foreheadActivated)
            validQuestionSubjects[11] = true;
        questionSubjects = questionSubjects.Shuffle().ToArray();

        string asfawmroemgsfoifdgoijsfb = "";
        for (int i = 2; i < 12; i++)
            asfawmroemgsfoifdgoijsfb += validQuestionSubjects[i].ToString() + " ";
        Debug.LogFormat("debug message. valid subjects: True True {0}", asfawmroemgsfoifdgoijsfb);
        animationPlaying = true;
        GenerateQuestion();
        StartCoroutine(GlitchAnimations());
    }
    IEnumerator DisplayQuestion()
    {
        animationPlaying = true;
        string line = "";

        while (screenText.text != "")
        {
            screenText.text = screenText.text.Substring(0, screenText.text.Length - 1);
            yield return new WaitForSeconds(.01f);
        }

        for (int i = 0; i < stageText.Length; i++)
        {
            line += stageText[i];
            screenText.text += stageText[i];

            if (line.Length > 33 && !line.EndsWith(" "))
            {
                screenText.text = screenText.text.Substring(0, screenText.text.LastIndexOf(' '));
                line = line.Substring(line.LastIndexOf(' ') + 1);
                screenText.text += "\n" + line;
            }
            yield return new WaitForSeconds(.01f);
        }

        btnTexts[0].text = "";
        btnRenderers[0].material.color = btnRenderers[1].material.color = btnRenderers[2].material.color = btnRenderers[3].material.color = colors[0];

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < questionAnswers[i].Length; j++)
            {
                btnTexts[i].text += questionAnswers[i][j];

                yield return new WaitForSeconds(.01f);
            }
        }
        animationPlaying = false;
    }
    IEnumerator SolveAnimation()
    {
        btnRenderers[0].material.color = btnRenderers[1].material.color = btnRenderers[2].material.color = btnRenderers[3].material.color = colors[0];
        LEDRenderers[0].material.color = LEDRenderers[1].material.color = LEDRenderers[2].material.color = colors[1];
        screenText.text = "";
        yield return new WaitForSeconds(.15f);
        for (int i = 0; i < 4; i++)
        {
            btnTexts[i].text = "";
            yield return new WaitForSeconds(.15f);
        }

        DebugMsg("Module solved! Poggers!");
        screenText.text = "SYSTEM OVERLOAD...";
        Audio.PlaySoundAtTransform("solveSound", Module.transform);

        for (int i = 0; i < 50; i++)
        {
            placeholder = Random.Range(0, 4);
            randomStep = Random.Range(0, 3); // don't feel like making another variable just for this
            btnRenderers[placeholder].material.color = colors[1];
            LEDRenderers[randomStep].material.color = colors[0];
            yield return new WaitForSeconds(.2f);
            btnRenderers[placeholder].material.color = colors[0];
            LEDRenderers[randomStep].material.color = colors[1];
            if (Bomb.GetTime() < 5) // make sure the module solves if the bomb is going to blow up before the solve animation can go through
                Module.HandlePass();
        }
        
        btnRenderers[0].material.color = btnRenderers[1].material.color = btnRenderers[2].material.color = btnRenderers[3].material.color = colors[1];
        Module.HandlePass();
        screenText.text = "Poggers!";
        solved = true;
    }
    IEnumerator GlitchAnimations()
    {
        while (!solved)
        {
            int randomButton = Random.Range(0, 4);
            if (screenText.text != "SYSTEM OVERLOAD...")
                yield return new WaitForSeconds(Random.Range(3 + (2 - questionNumber) * 4, 6 + (2 - questionNumber) * 4) * .1f);
            switch (Random.Range(0, 8))
            {
                case 0: // the border twitches
                    border.transform.Rotate(Vector3.down, 5);
                    for (int i = 0; i < 10; i++)
                    {
                        yield return new WaitForSeconds(.01f);
                        border.transform.Rotate(Vector3.down, -.5f);
                    }
                    break;
                case 1: // the border twitches again
                    border.transform.Rotate(Vector3.up, 5);
                    for (int i = 0; i < 10; i++)
                    {
                        yield return new WaitForSeconds(.01f);
                        border.transform.Rotate(Vector3.up, -.5f);
                    }
                    break;
                case 2: // led flickers
                    int randomLED = Random.Range(0, 3);
                    if (LEDRenderers[randomLED].material.color == colors[1])
                    {
                        LEDRenderers[randomLED].material.color = colors[0];
                        yield return new WaitForSeconds(Random.Range(1, 30) * .01f);
                        LEDRenderers[randomLED].material.color = colors[1];
                        yield return new WaitForSeconds(Random.Range(1, 30) * .01f);
                        LEDRenderers[randomLED].material.color = colors[0];
                        yield return new WaitForSeconds(Random.Range(1, 30) * .01f);
                        LEDRenderers[randomLED].material.color = colors[1];
                    }
                    else
                    {
                        LEDRenderers[randomLED].material.color = colors[1];
                        yield return new WaitForSeconds(Random.Range(1, 30) * .01f);
                        LEDRenderers[randomLED].material.color = colors[0];
                        yield return new WaitForSeconds(Random.Range(1, 30) * .01f);
                        LEDRenderers[randomLED].material.color = colors[1];
                        yield return new WaitForSeconds(Random.Range(1, 30) * .01f);
                        LEDRenderers[randomLED].material.color = colors[0];
                    }
                    break;
                case 3: // button flickers green
                    btnRenderers[randomButton].material.color = colors[1];
                    yield return new WaitForSeconds(Random.Range(1, 30) * .01f);
                    btnRenderers[randomButton].material.color = colors[0];
                    break;
                case 4: // button text flickers off
                    screenText.color = new Color32(0, 0, 0, 0);
                    yield return new WaitForSeconds(Random.Range(1, 30) * .01f);
                    screenText.color = new Color32(0, 255, 0, 255);
                    break;
                case 5: // screen text flickers off
                    screenText.color = new Color32(0, 0, 0, 0);
                    yield return new WaitForSeconds(Random.Range(1, 30) * .01f);
                    screenText.color = new Color32(0, 255, 0, 255);
                    break;
                case 6: // screen twitches
                    screen.transform.Rotate(Vector3.down, 5);
                    for (int i = 0; i < 10; i++)
                    {
                        yield return new WaitForSeconds(.01f);
                        screen.transform.Rotate(Vector3.down, -.5f);
                    }
                    break;
                case 7: // screen twitches again
                    screen.transform.Rotate(Vector3.up, 5);
                    for (int i = 0; i < 10; i++)
                    {
                        yield return new WaitForSeconds(.01f);
                        screen.transform.Rotate(Vector3.up, -.5f);
                    }
                    break;
            }
        }

        yield return new WaitForSeconds(.1f);
    }
    

    void DebugMsg(string message)
    {
        Debug.LogFormat("[Duck Konundrum #{0}] {1}", _moduleId, message);
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = "Use !{0} press 1/2/3/4 to press the 1st/2nd/3rd/4th buttons respectively.";
    #pragma warning disable 414

    IEnumerator ProcessTwitchCommand(string cmd)
    {
        string[] numbers = { "1", "2", "3", "4" };
        cmd = cmd.ToLowerInvariant();
        if (!questionsAreDisplayed)
        {
            yield return "sendtochaterror Duck Konundrum is not ready for submission yet!";
            yield break;
        }
        if (cmd.StartsWith("press "))
        {
            cmd = cmd.Substring(6);
            if (numbers.Contains(cmd))
                PressButton(Array.IndexOf(numbers, cmd));
            else
                yield return "sendtochaterror That's not a number 1-4.";
            yield break;
        }
        
        yield return "sendtochaterror W-what?";
        yield break;
    }
}