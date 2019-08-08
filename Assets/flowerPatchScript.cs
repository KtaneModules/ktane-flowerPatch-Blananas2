using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class flowerPatchScript : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable[] flowers;
    public Material[] mats;

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
    public List<string> positionLetters = new List<string> { "ADHLNTU", "DHJNQTZ", "CDHNQTUZ", "DHJNTXZ", "ADEHNQTU", "HLMUXZ", "HJMQZ", "CHMUXZ", "HJMXZ", "EHMUXZ", "ADHLQU", "DHJXZ", "CDHQUZ", "DHJXZ", "ADEHUX" };
    public List<string> colorLetters = new List<string> { "KPR", "KOS", "KPY", "FGS", "BFP", "FI", "FSV", "W" };
    public List<int> solutionFlowers = new List<int> {};

    void Awake () {
        moduleId = moduleIdCounter++;

        foreach (KMSelectable flower in flowers) {
            KMSelectable pressedFlower = flower;
            flower.OnInteract += delegate () { FlowerPress(pressedFlower); return false; };
        }
    }

    // Use this for initialization
    void Start () {
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
        
        for (int j=0; j<8; j++)
        {
            flowerColors.Add(7);
        }
        flowerColors.Shuffle();
        for (int i=0; i<15; i++)
        {
            colorNow = flowerColors[i];
            flowers[i].GetComponent<MeshRenderer>().material = mats[colorNow];
            positionLetters[i] += colorLetters[colorNow];
        };

        solutionFlowers.Clear();
        for (int k=0; k<15; k++)
        {
            if (positionLetters[k].Contains(solutionLetter))
            {
                solutionFlowers.Add(1);
            } else
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
	
	void FlowerPress (KMSelectable flower)
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
                        solutionFlowers[n] = 0;
                        SolveCheck();
                    } else
                    {
                        GetComponent<KMBombModule>().HandleStrike();
                        Debug.LogFormat("[Flower Patch #{0}] Selected flower {1}, which is incorrect, module striked.", moduleId, n + 1);
                    }
                }
            }
        }

    }

    void SolveCheck ()
    {
        if ((solutionFlowers[0] + solutionFlowers[1] + solutionFlowers[2] + solutionFlowers[3] + solutionFlowers[4] + solutionFlowers[5] + solutionFlowers[6] + solutionFlowers[7] + solutionFlowers[8] + solutionFlowers[9] + solutionFlowers[10] + solutionFlowers[11] + solutionFlowers[12] + solutionFlowers[13] + solutionFlowers[14]) == 0)
        {
            GetComponent<KMBombModule>().HandlePass();
            moduleSolved = true;
            Debug.LogFormat("[Flower Patch #{0}] All correct flowers selected, module solved.", moduleId);
        }
    }
}
