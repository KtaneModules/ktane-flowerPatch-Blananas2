using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class flowerPatchScript : MonoBehaviour
{

    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMColorblindMode CBthing;
    public KMRuleSeedable RuleSeedable;

    public KMSelectable[] flowers;
    public Material[] mats;
    public TextMesh[] CBtexts;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    //             0123456789A123456789B123456789C123456789D123456789E123456789F123456789G123456789H123456789I123456789J123456789K123456789L1234567
    string Venn = "WONBMQZALROSXEQRKJPYMSIZVUUTGTHAJGHGNMONKGIHYFPCTEKFKLJBWVJVXYYTIDSKTCOBLICJEDDCIQWXQRPEOPVDFXAUFCMLABNEAHBGZFZVMDLSRUHPNUWQWRSX";
    int coloredFlowers;
    char solutionLetter;
    int colorNow;
    string flowerString = "";
    private List<int> flowerColors = new List<int> { };
    private List<string> positionLetters = new List<string>(15) { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
    private List<string> colorLetters = new List<string>(8) { "", "", "", "", "", "", "", "" };
    private List<int> solutionFlowers = new List<int> { };
    private List<int> pressedFlowers = new List<int> { };
    bool CBactive = false;
    string CBletters = "ROYGBIVW";

    void Awake()
    {
        moduleId = moduleIdCounter++;

        foreach (KMSelectable flower in flowers)
        {
            KMSelectable pressedFlower = flower;
            flower.OnInteract += delegate () { FlowerPress(pressedFlower); return false; };
        }
    }

    public class FlowerPatchRule
    {
        public bool IsPositionRule;
        public int[] Results;
        public string LogRule;

        public FlowerPatchRule(bool isPositionRule, int[] results, string logRule)
        {
            IsPositionRule = isPositionRule;
            Results = results;
            LogRule = logRule;
        }
    }

    public FlowerPatchRule[] Rules = new FlowerPatchRule[]
    {
        // r o y g b i v w
        new FlowerPatchRule(true, new int[] {0, 4, 10, 14}, "In the corners"),
        new FlowerPatchRule(false, new int[] {4}, "That are blue"),
        new FlowerPatchRule(true, new int[] {2, 7, 12}, "In the middle column"),
        new FlowerPatchRule(true, new int[] {0, 1, 2, 3, 4, 10, 11, 12, 13, 14}, "In the 1st and 3rd rows"),
        new FlowerPatchRule(true, new int[] {4, 9, 14}, "In the eastmost column"),
        new FlowerPatchRule(false, new int[] {3, 4, 5, 6}, "That are cold colored"),
        new FlowerPatchRule(false, new int[] {3}, "That are green"),
        new FlowerPatchRule(true, new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14}, "In general; select all of them"),
        new FlowerPatchRule(false, new int[] {5}, "That are indigo"),
        new FlowerPatchRule(true, new int[] {1, 3, 6, 8, 11, 13}, "In the 2nd and 4th columns"),
        new FlowerPatchRule(false, new int[] {0, 1, 2}, "That are warm colored"),
        new FlowerPatchRule(true, new int[] {0, 5, 10}, "In the leftmost column"),
        new FlowerPatchRule(true, new int[] {5, 6, 7, 8, 9}, "In the middle row"),
        new FlowerPatchRule(true, new int[] {0, 1, 2, 3, 4}, "In the northmost row"),
        new FlowerPatchRule(false, new int[] {1}, "That are orange"),
        new FlowerPatchRule(false, new int[] {0, 2, 4}, "That are primary colored"),
        new FlowerPatchRule(true, new int[] {1, 2, 4, 6, 10, 12}, "In prime numbered positions"),
        new FlowerPatchRule(false, new int[] {0}, "That are red"),
        new FlowerPatchRule(false, new int[] {1, 3, 6}, "That are secondary colored"),
        new FlowerPatchRule(true, new int[] {10, 11, 12, 13, 14}, "In the third row"),
        new FlowerPatchRule(true, new int[] {0, 2, 4, 5, 7, 9, 10, 12, 14}, "In the 1st, 3rd and 5th columns"),
        new FlowerPatchRule(false, new int[] {6}, "That are violet"),
        new FlowerPatchRule(false, new int[] {7}, "That are white"),
        new FlowerPatchRule(true, new int[] {3, 5, 7, 8, 9, 11, 13, 14}, "In composite numbered positions"),
        new FlowerPatchRule(false, new int[] {2}, "That are yellow"),
        new FlowerPatchRule(true, new int[] {1, 2, 3, 5, 6, 7, 8, 9, 11, 12, 13}, "Not in the corners"),
        new FlowerPatchRule(true, new int[] {0, 1, 2, 3, 4, 5, 10}, "In the top row or leftmost column"),
        new FlowerPatchRule(true, new int[] {0, 1, 2, 3, 4, 9, 14}, "In the top row or rightmost column"),
        new FlowerPatchRule(true, new int[] {0, 5, 10, 11, 12, 13, 14}, "In the bottom row or leftmost column"),
        new FlowerPatchRule(true, new int[] {4, 9, 10, 11, 12, 13, 14}, "In the bottom row or rightmost column"),
        new FlowerPatchRule(true, new int[] {2, 5, 6, 7, 8, 9, 12}, "In the middle row or middle column"),
        new FlowerPatchRule(true, new int[] {1, 6, 11}, "In the second column"),
        new FlowerPatchRule(true, new int[] {3, 8, 13}, "In the fourth column"),
        new FlowerPatchRule(true, new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}, "In the top two rows"),
        new FlowerPatchRule(true, new int[] {5, 6, 7, 8, 9, 10, 11, 12, 13, 14}, "In the bottom two rows"),
        new FlowerPatchRule(true, new int[] {7}, "In the middle position"),
        new FlowerPatchRule(true, new int[] {1, 2, 3, 6, 7, 8, 11, 12, 13}, "In the 2nd, 3rd, and 4th columns"),
        new FlowerPatchRule(false, new int[] {0, 2}, "That are red or yellow"),
        new FlowerPatchRule(false, new int[] {1, 3}, "That are orange or green"),
        new FlowerPatchRule(false, new int[] {2, 4}, "That are yellow or blue"),
        new FlowerPatchRule(false, new int[] {3, 6}, "That are green or violet"),
        new FlowerPatchRule(false, new int[] {0, 4}, "That are red or blue"),
        new FlowerPatchRule(false, new int[] {1, 6}, "That are orange or violet"),
        new FlowerPatchRule(false, new int[] {5, 7}, "That are indigo or white"),
        new FlowerPatchRule(false, new int[] {0, 1, 2, 3, 4, 5, 6}, "That are not white"),
    };

    // Use this for initialization
    void Start()
    {
        var rnd = RuleSeedable.GetRNG();
        if (rnd.Seed != 1)
        {
            Debug.LogFormat("[Flower Patch #{0}] Using rule seed {1}.", moduleId, rnd.Seed);
            var arr = Venn.ToArray();
            rnd.ShuffleFisherYates(arr);
            rnd.ShuffleFisherYates(Rules);
            Venn = arr.Join("");
            Debug.LogFormat("<Flower Patch #{0}> Venn diagram: {1}", moduleId, Venn);
            var str = "";
            for (int i = 0; i < 26; i++)
                str += string.Format("\n{0}: {1}.", "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[i], Rules[i].LogRule);
            Debug.LogFormat("<Flower Patch #{0}> Rules:{1}", moduleId, str);
        }
        string alph = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        for (int i = 0; i < 26; i++)
        {
            if (Rules[i].IsPositionRule)
            {
                for (int pos = 0; pos < 15; pos++)
                    if (Rules[i].Results.Contains(pos))
                        positionLetters[pos] += alph[i];
            }
            else
            {
                for (int col = 0; col < 8; col++)
                    if (Rules[i].Results.Contains(col))
                        colorLetters[col] += alph[i];
            }
        }

        if (CBthing.ColorblindModeActive)
        {
            CBactive = true;
        }

        flowerColors.Clear();
        coloredFlowers = UnityEngine.Random.Range(0, 128);
        solutionLetter = Venn[coloredFlowers];
        var index = solutionLetter - 'A';
        
        if (coloredFlowers > 63) { flowerColors.Add(0); coloredFlowers -= 64; flowerString += "Red "; } else { flowerColors.Add(7); }
        if (coloredFlowers > 31) { flowerColors.Add(1); coloredFlowers -= 32; flowerString += "Orange "; } else { flowerColors.Add(7); }
        if (coloredFlowers > 15) { flowerColors.Add(2); coloredFlowers -= 16; flowerString += "Yellow "; } else { flowerColors.Add(7); }
        if (coloredFlowers > 7) { flowerColors.Add(3); coloredFlowers -= 8; flowerString += "Green "; } else { flowerColors.Add(7); }
        if (coloredFlowers > 3) { flowerColors.Add(4); coloredFlowers -= 4; flowerString += "Blue "; } else { flowerColors.Add(7); }
        if (coloredFlowers > 1) { flowerColors.Add(5); coloredFlowers -= 2; flowerString += "Indigo "; } else { flowerColors.Add(7); }
        if (coloredFlowers == 1) { flowerColors.Add(6); flowerString += "Violet "; } else { flowerColors.Add(7); }

        for (int j = 0; j < 8; j++)
        {
            flowerColors.Add(7);
        }
        flowerColors.Shuffle();
        for (int i = 0; i < 15; i++)
        {
            colorNow = flowerColors[i];
            flowers[i].GetComponent<MeshRenderer>().material = mats[colorNow];
            positionLetters[i] += colorLetters[colorNow];
            if (CBactive)
            {
                CBtexts[i].text = " " + CBletters[colorNow] + " ";
            }
        }

        solutionFlowers.Clear();
        for (int k = 0; k < 15; k++)
        {
            if (positionLetters[k].Contains(solutionLetter))
            {
                solutionFlowers.Add(1);
            }
            else
            {
                solutionFlowers.Add(0);
            }
        }

        Debug.LogFormat("[Flower Patch #{0}] The colored flowers are: {1}", moduleId, flowerString);
        Debug.LogFormat("[Flower Patch #{0}] The solution letter is: {1}", moduleId, solutionLetter);

        Debug.LogFormat("[Flower Patch #{0}] Press all the flowers {1}.", moduleId, Rules[index].LogRule.ToLowerInvariant());

        if ((solutionFlowers[0] + solutionFlowers[1] + solutionFlowers[2] + solutionFlowers[3] + solutionFlowers[4] + solutionFlowers[5] + solutionFlowers[6] + solutionFlowers[7] + solutionFlowers[8] + solutionFlowers[9] + solutionFlowers[10] + solutionFlowers[11] + solutionFlowers[12] + solutionFlowers[13] + solutionFlowers[14]) == 0)
        {
            solutionFlowers[7] = 1;
            Debug.LogFormat("[Flower Patch #{0}] No flowers fit the description for the letter, so the only valid flower is the middle flower.", moduleId);
        }

    }

    void FlowerPress(KMSelectable flower)
    {
        if (moduleSolved == false)
        {
            flower.AddInteractionPunch();
            GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            for (int n = 0; n < 15; n++)
            {
                if (flower == flowers[n])
                {
                    if (solutionFlowers[n] == 1)
                    {
                        pressedFlowers.Add(n);
                        solutionFlowers[n] = 0;
                        Debug.LogFormat("[Flower Patch #{0}] Selected flower {1}, which is correct.", moduleId, n + 1);
                        SolveCheck();
                    }
                    else
                    {
                        GetComponent<KMBombModule>().HandleStrike();
                        Debug.LogFormat("[Flower Patch #{0}] Selected flower {1}, which is incorrect, module striked.", moduleId, n + 1);
                        for (int j = 0; j < pressedFlowers.Count; j++)
                        {
                            solutionFlowers[pressedFlowers[j]] = 1;
                        }
                    }
                }
            }
        }

    }

    void SolveCheck()
    {
        if ((solutionFlowers[0] + solutionFlowers[1] + solutionFlowers[2] + solutionFlowers[3] + solutionFlowers[4] + solutionFlowers[5] + solutionFlowers[6] + solutionFlowers[7] + solutionFlowers[8] + solutionFlowers[9] + solutionFlowers[10] + solutionFlowers[11] + solutionFlowers[12] + solutionFlowers[13] + solutionFlowers[14]) == 0)
        {
            GetComponent<KMBombModule>().HandlePass();
            moduleSolved = true;
            Debug.LogFormat("[Flower Patch #{0}] All correct flowers selected, module solved.", moduleId);
        }
    }

    //twitch plays
    private bool numsAreValid(string s)
    {
        string[] nums = s.Split(' ');
        for (int i = 0; i < nums.Length; i++)
        {
            int temp = 0;
            bool check = int.TryParse(nums[i], out temp);
            if (check == false)
            {
                return false;
            }
            else if (temp < 1 || temp > 15)
            {
                return false;
            }
        }
        return true;
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} flower 1 12 7 [Presses the specified flower where 1-15 is each flower's position in reading order] | !{0} colorblind [Toggles colorblind mode]";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        if (Regex.IsMatch(command, @"^\s*colorblind\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (CBactive)
            {
                CBactive = false;
                for (int i = 0; i < 15; i++)
                {
                    CBtexts[i].text = "   ";
                }
            }
            else
            {
                CBactive = true;
                for (int i = 0; i < 15; i++)
                {
                    colorNow = flowerColors[i];
                    CBtexts[i].text = " " + CBletters[colorNow] + " ";
                }
            }
            yield break;
        }
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*flower\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(parameters[0], @"^\s*submit\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            if (parameters.Length >= 2)
            {
                string nums = command.Substring(7);
                if (numsAreValid(nums))
                {
                    yield return null;
                    for (int i = 1; i < parameters.Length; i++)
                    {
                        if (parameters[i].EqualsIgnoreCase("1"))
                        {
                            flowers[0].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("2"))
                        {
                            flowers[1].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("3"))
                        {
                            flowers[2].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("4"))
                        {
                            flowers[3].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("5"))
                        {
                            flowers[4].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("6"))
                        {
                            flowers[5].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("7"))
                        {
                            flowers[6].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("8"))
                        {
                            flowers[7].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("9"))
                        {
                            flowers[8].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("10"))
                        {
                            flowers[9].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("11"))
                        {
                            flowers[10].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("12"))
                        {
                            flowers[11].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("13"))
                        {
                            flowers[12].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("14"))
                        {
                            flowers[13].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("15"))
                        {
                            flowers[14].OnInteract();
                        }
                        yield return new WaitForSeconds(0.1f);
                    }
                    yield break;
                }
            }
        }
    }

    private IEnumerator TwitchHandleForcedSolve()
    {
        for (int i = 0; i < 15; i++)
        {
            if (solutionFlowers[i] == 1 && !pressedFlowers.Contains(i))
            {
                flowers[i].OnInteract();
                if (!moduleSolved)
                    yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
