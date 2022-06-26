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
    public List<int> flowerColors = new List<int> { };
    public List<string> positionLetters = new List<string> { "ADHLNU", "DHJNQZ", "CDHNQUZ", "DHJNXZ", "ADEHNQU", "HLMUXZ", "HJMQZ", "CHMUXZ", "HJMXZ", "EHMUXZ", "ADHLQTU", "DHJTXZ", "CDHQTUZ", "DHJTXZ", "ADEHTUX" };
    public List<string> colorLetters = new List<string> { "KPR", "KOS", "KPY", "FGS", "BFP", "FI", "FSV", "W" };
    public List<int> solutionFlowers = new List<int> { };
    public List<int> pressedFlowers = new List<int> { };
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

    // Use this for initialization
    void Start()
    {
        if (CBthing.ColorblindModeActive) {
            CBactive = true;
        }

        flowerColors.Clear();
        coloredFlowers = UnityEngine.Random.Range(0, 128);
        solutionLetter = Venn[coloredFlowers];
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
            if (CBactive) {
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
                        SolveCheck();
                    }
                    else
                    {
                        GetComponent<KMBombModule>().HandleStrike();
                        Debug.LogFormat("[Flower Patch #{0}] Selected flower {1}, which is incorrect, module striked.", moduleId, n + 1);
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
    private readonly string TwitchHelpMessage = @"!{0} flower 1 12 7 [Presses the specified flower where 1-15 is each flower's position in reading order] | !{0} colorblind [Toggles colorblind mode] | !{0} reset [Removes all inputted flowers]";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        if (Regex.IsMatch(command, @"^\s*reset\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            Debug.LogFormat("[Flower Patch #{0}] TP Reset called!", moduleId);
            for (int j = 0; j < pressedFlowers.Count; j++)
            {
                solutionFlowers[pressedFlowers[j]] = 1;
            }
            yield break;
        }
        if (Regex.IsMatch(command, @"^\s*colorblind\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (CBactive) {
                CBactive = false;
                for (int i = 0; i < 15; i++)
                {
                    CBtexts[i].text = "   ";
                }
            } else {
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
