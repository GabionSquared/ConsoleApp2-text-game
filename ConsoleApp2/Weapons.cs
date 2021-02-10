using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REEEE
{
    class WeaponController
    {
        public static object[,] WeaponData = Program.ReadFile(@"E:\My Drive\Ban This Man\WeaponData.txt", @"G:\My Drive\Ban This Man\WeaponData.txt", 11);
        //the weapon names, text and attached moveset
        public static int[][] MovesetData = new int[13][];

        /*scaling letters to numbers dictionary
        Dictionary<char?, int> ScalingHidden = new Dictionary<char?, int>
        {//scaling will be displayed to the player (and in the .txt) as letter grades. This is what that actually means.
            {'S',6},
            {'A',5},
            {'B',4},
            {'C',3},
            {'D',2},
            {'E',1},
            {null,0},
        };
        */

        public static Dictionary<int, String> WeaponCatagories = new Dictionary<int, String>
        {
            {0,"Straight Sword"},
            {1,"Dagger"},
            {2,"Greatsword"},
            {3,"Ultra Greatsword"},
            {4,"Curved Sword"},
            {5,"Rapier"},
            {6,"Axe"},
            {7,"Hammer"},
            {8,"Great Hammer"},
            {9,"Fist/Claw"},
            {10,"Spear"},
            {11,"Halberd"},
            {12,"Scythe"},
        };

        //assigns the moveset arrays into MovesetData. Jagged array (array of arrays), NOT a 2d array or list bs
        public static void AssignMovesetData()
        {//basic moveset is dependant on weapon class. 1 defense and 3 attack (first two will never be replaced)
            MovesetData[0]  = new int[] { 0, 1, 2, 3 };               //shortsword  //grapple, hew, zealous plunge, bludgeon
            MovesetData[1]  = new int[] { 4, 5, 6, 7, 8, 9 };         //dagger      //parry, puncture, guardbreak, lacerate, feint, throw?
            MovesetData[2]  = new int[] { 10, 1, 11, 12, 2 };         //greatsword  //withstand, hew, intimidate, crush, zealous plunge
            MovesetData[3]  = new int[] { 10, 1, 11, 12, 3, 13, 14 }; //UGreatsword //withstand, hew, intimidate, crush, bludgeon, charge, sweep
            MovesetData[4]  = new int[] { 4, 1, 7, 8 };               //curvedsword //parry, hew, lacerate, feint
            MovesetData[5]  = new int[] { 4, 5, 2 };                  //rapier      //parry, puncture, zealous plunge
            MovesetData[6]  = new int[] { 0, 1, 3, 11 };              //axe         //grapple, hew, bludgeon, intimidate
            MovesetData[7]  = new int[] { 10, 3, 15, 16 };            //hammer      //withstand, bludgeon, shatter, blunt (blunt the enemy weapon)
            MovesetData[8]  = new int[] { 10, 3, 15, 14 };            //greathammer //withstand, bludgeon, shatter, sweep
            MovesetData[9]  = new int[] { 0, 3, 7, 8 };               //fist/claw   //grapple, bludgeon, lacerate, feint
            MovesetData[10] = new int[] { 10, 5, 2, 11 };             //spear       //withstand, puncture, zealous plunge, intimidate
            MovesetData[11] = new int[] { 10, 5, 2, 14 };             //halberd     //withstand, puncture, zealous plunge, intimidate, sweep
            MovesetData[12] = new int[] { 10, 5, 7, 14 };             //scythe      //withstand, puncture, lacerate, intimidate, sweep
        }

        /// <summary>
        /// make a new weapon object from id <para>This summary block is longer than the actual function</para>
        /// </summary>
        /// <param name="id">ID for the weapon being generated</param>
        /// <returns>weapon from id as Weapon type</returns>
        public static Weapon CreateWeapon(int id = 0)
        {
            Weapon weapon = new Weapon();
            weapon.Assign(id);
            return weapon;
        }
    }

    class Weapon
    {
        //all the data a weapon has, in order of how it's stored on the files
        public int id;
        public string name;
        public int type;
        public int Durability { get; set; }
        public int DmgUp { get; set; }
        public int DmgDwn { get; set; }
        public int Crit { get; set; }
        public int Speed { get; set; }
        
        string flavour;
        public int flavourBracket;
        public string[] BrokenFlavour = new string[4];

        public readonly int[] attacks = new int[4];

        /// <summary>
        /// defining all the variables using WeaponData and some bell curves
        /// </summary>
        /// <param name="passedID">The id that WeaponController.CreateWeapon, used for indexing WeaponData</param>
        public void Assign(int passedID = 0)
        {
            id = passedID;
            name = Convert.ToString(WeaponController.WeaponData[id, 1]);
            type = Convert.ToInt32(WeaponController.WeaponData[id, 2]);
            Durability = Program.rnd.Next(Convert.ToInt32(WeaponController.WeaponData[id, 3]), Convert.ToInt32(WeaponController.WeaponData[id, 4])) + Program.rnd.Next(Convert.ToInt32(WeaponController.WeaponData[id, 3]), Convert.ToInt32(WeaponController.WeaponData[id, 4]));
            DmgUp = Convert.ToInt32(WeaponController.WeaponData[id, 6]);
            DmgDwn = Convert.ToInt32(WeaponController.WeaponData[id, 7]);
            Crit = Convert.ToInt32(WeaponController.WeaponData[id, 8]);
            Speed = Convert.ToInt32(WeaponController.WeaponData[id, 9]);
            flavour = Convert.ToString(WeaponController.WeaponData[id, 10]);

            //assign the first one
            attacks[0] = WeaponController.MovesetData[type][0];
            //remove the first one (by putting it into another array, not fucking the master)
            int[] ShuffledMoveSet = WeaponController.MovesetData[type].Where((source, index) => index != 0).ToArray();
            //shuffle the new shorter array
            ShuffledMoveSet = ShuffledMoveSet.OrderBy(x => Program.rnd.Next()).ToArray();

            attacks[1] = ShuffledMoveSet[0];
            attacks[2] = ShuffledMoveSet[1];
            attacks[3] = ShuffledMoveSet[2];

            BreakFlavour();
        }

        void BreakFlavour()
        {
            System.Diagnostics.Debug.WriteLine("\nBREAKFLAVOUR");
            //BrokenFlavour[4]
            //flavourBracket
            string[] Hold;
            string[] Punctuation = {".", ",", ";", ":", "-", "(", ")", "!", "?" };

            //size tiers of description: 44 (11), 120 (30) 200 (50)

            if(flavour.Length <= 156) {
                flavourBracket = 39;
            }else if(flavour.Length <= 120) {
                flavourBracket = 30;
            }else if(flavour.Length <= 44) {
                flavourBracket = 11;
            }
            //this is the character limit per line based on how long the whole thing is

            int counter = 0; //how many of string[] Hold we've found a place for
            Hold = flavour.Split(' '); //the flavor is now broken into words

            //we can only know the line is full once we go over the limit, so we BITES ZA DASTO that shit and go on with our day
            for(int set = 0; set < 4; set++) {                          //for each line we're talkin
                System.Diagnostics.Debug.WriteLine("Set {0}", set);
                while(true) {                                           //hey kids wanna buy an infinite for loop?
                    System.Diagnostics.Debug.WriteLine("Word: {0}", Hold[counter]);
                    try {
                        if(counter == Hold.Length) {                     //check if we're at the end
                            throw new Exception("Finished");
                        }
                        if(Punctuation.Any(Hold[counter].Contains)) {   //check if the word is actually just punctuation
                            BrokenFlavour[set] += Hold[counter];                      //put the next punctuation into the line
                            System.Diagnostics.Debug.WriteLine("Word is punc");
                        } else {
                            BrokenFlavour[set] += " " + Hold[counter];                //put the next word into the line
                            System.Diagnostics.Debug.WriteLine("Word is not punc.");
                        }
                        System.Diagnostics.Debug.WriteLine("So Far: {0}", BrokenFlavour[set]);
                        if(BrokenFlavour[set].Length > flavourBracket) {             //check if it's gone too far
                            BrokenFlavour[set] = BrokenFlavour[set].Remove(BrokenFlavour[set].Length-(Hold[counter].Length+1), Hold[counter].Length + 1);   //trim that shit off [remove(how much you're keeping, how much after that you're not)]
                            throw new Exception("Line Full");               //KILLA QUEEN DAISAN NO BAKUDAN
                        } else {
                            System.Diagnostics.Debug.WriteLine("\tmoving to next word, counter {0}", counter+1);
                            counter++;                                      //move on to the next word
                        }
                    } catch {
                        System.Diagnostics.Debug.WriteLine("\tmoving to next set, counter {0}", counter + 1);
                        break;                                          //move on to the next line
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine("");
        }
        public void Display()
        {
            int totalWidth = 23 + flavourBracket;
            //size tiers of description: 44 (11), 120 (30) 156 (39)
            Tabgroup();
            Console.Write(" ");
            for(int i = 0; i < totalWidth; i++) {
                Console.Write("_");
            }
            Console.Write("\n");

            switch(flavourBracket) {
                case 11:
                    /*0*/ Tabgroup(); Console.WriteLine("| {0,-21}|", name); //name
                    /*1*/ Tabgroup(); Console.WriteLine("| {0,-21}|", "(" + WeaponController.WeaponCatagories[Globals.HeldWeapon.type] + ")"); //type
                    /*2*/ Tabgroup(); Console.Write("|");
                        Tabgroup(); for (int i = 0; i < totalWidth; i++) {
                            Console.Write("-");
                        }
                        Console.Write("|\n");
                    /*3*/ Tabgroup(); Console.WriteLine("|{0,11}:\t  {1,-5}|{2,-39}|", "Durability", Globals.HeldWeapon.Durability, Globals.HeldWeapon.BrokenFlavour[0]); //durability
                    /*4*/ Tabgroup(); Console.WriteLine("|{0,11}:\t {1,2}-{2,-3}|{3,-39}|", "DMG Base", Globals.HeldWeapon.DmgDwn, Globals.HeldWeapon.DmgUp, Globals.HeldWeapon.BrokenFlavour[1]); //damage
                    /*5*/ Tabgroup(); Console.WriteLine("|{0,11}:\t  {1,-5}|{2,-39}|", "CRT base", Globals.HeldWeapon.Crit, Globals.HeldWeapon.BrokenFlavour[2]); //crit
                    /*6*/ Tabgroup(); Console.WriteLine("|{0,11}:\t  {1,-5}|{2,-39}|", "SPD base", Globals.HeldWeapon.Speed, Globals.HeldWeapon.BrokenFlavour[3]); //speed
                    break;
                case 30:
                    Tabgroup(); Console.WriteLine("| {0,-52}|", name); //name
                    Tabgroup(); Console.WriteLine("| {0,-52}|", "(" + WeaponController.WeaponCatagories[Globals.HeldWeapon.type] + ")"); //type
                    Tabgroup(); Console.Write("|");
                    for(int i = 0; i < totalWidth; i++) {
                        Console.Write("-");
                    }
                    Console.Write("|\n");
                    Tabgroup(); Console.WriteLine("|{0,11}:\t  {1,-5}|{2,-30}|", "Durability", Globals.HeldWeapon.Durability, Globals.HeldWeapon.BrokenFlavour[0]); //durability
                    Tabgroup(); Console.WriteLine("|{0,11}:\t {1,2}-{2,-3}|{3,-30}|", "DMG Base", Globals.HeldWeapon.DmgDwn, Globals.HeldWeapon.DmgUp, Globals.HeldWeapon.BrokenFlavour[1]); //damage
                    Tabgroup(); Console.WriteLine("|{0,11}:\t  {1,-5}|{2,-30}|", "CRT base", Globals.HeldWeapon.Crit, Globals.HeldWeapon.BrokenFlavour[2]); //crit
                    Tabgroup(); Console.WriteLine("|{0,11}:\t  {1,-5}|{2,-30}|", "SPD base", Globals.HeldWeapon.Speed, Globals.HeldWeapon.BrokenFlavour[3]); //speed
                break;
                case 39:
                    Tabgroup(); Console.WriteLine("| {0,-61}|", name); //name
                    Tabgroup(); Console.WriteLine("| {0,-61}|", "(" + WeaponController.WeaponCatagories[Globals.HeldWeapon.type] + ")"); //type
                    Tabgroup(); Console.Write("|");
                    for(int i = 0; i < totalWidth; i++) {
                        Console.Write("-");
                    }
                    Console.Write("|\n");
                    Tabgroup(); Console.WriteLine("|{0,11}:\t  {1,-5}|{2,-39}|", "Durability", Globals.HeldWeapon.Durability, Globals.HeldWeapon.BrokenFlavour[0]); //durability
                    Tabgroup(); Console.WriteLine("|{0,11}:\t {1,2}-{2,-3}|{3,-39}|", "DMG Base", Globals.HeldWeapon.DmgDwn, Globals.HeldWeapon.DmgUp, Globals.HeldWeapon.BrokenFlavour[1]); //damage
                    Tabgroup(); Console.WriteLine("|{0,11}:\t  {1,-5}|{2,-39}|", "CRT base", Globals.HeldWeapon.Crit, Globals.HeldWeapon.BrokenFlavour[2]); //crit
                    Tabgroup(); Console.WriteLine("|{0,11}:\t  {1,-5}|{2,-39}|", "SPD base", Globals.HeldWeapon.Speed, Globals.HeldWeapon.BrokenFlavour[3]); //speed
                break;
            }

            Tabgroup(); Console.Write("|");
            for(int i = 0; i < totalWidth; i++) {
                Console.Write("_");
            }
            Console.Write("|\n");
            Console.Write("\n");
        }

        void Tabgroup()
        {
            Console.WriteLine("\t");
        }
    }
}
