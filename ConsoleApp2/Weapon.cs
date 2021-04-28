using System;
using System.IO;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REEEE
{
    class WeaponController
    {
        public static readonly object[,] AttackData = Program.ReadFile("AttackData.txt");
        public readonly int finalAddress = (AttackData.Length / 9) -1; // .length on a 2d array will give the ENTIRE LENGTH (length*width) so we're dividing by the width to get the last row

        public object[,] WeaponData = Program.ReadFile("WeaponData.txt");
        //the weapon names, text and attached moveset
        public static int[][] MovesetData = new int[32][];
        public string Flavour { get; set; }
#pragma warning disable IDE0044 // Add readonly modifier
        string[] BrokenFlavour = new string[4];
#pragma warning restore IDE0044 // Add readonly modifier

        public readonly int[] attacks = new int[4];
        //all the data a weapon has, in order of how it's stored on the files

        public Dictionary<int, String> WeaponCatagories = new Dictionary<int, String>
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
            //turn this into external data when you can
            MovesetData[0] = new int[] { 0, 1, 2, 3 };               //shortsword  //grapple, hew, zealous plunge, bludgeon
            MovesetData[1] = new int[] { 4, 5, 6, 7, 8, 9 };         //dagger      //parry, puncture, guardbreak, lacerate, feint, throw?
            MovesetData[2] = new int[] { 10, 1, 11, 12, 2 };         //greatsword  //withstand, hew, intimidate, crush, zealous plunge
            MovesetData[3] = new int[] { 10, 1, 11, 12, 3, 13, 14 }; //UGreatsword //withstand, hew, intimidate, crush, bludgeon, charge, sweep
            MovesetData[4] = new int[] { 4, 1, 7, 8 };               //curvedsword //parry, hew, lacerate, feint
            MovesetData[5] = new int[] { 4, 5, 2, 8  };              //rapier      //parry, puncture, zealous plunge, feint
            MovesetData[6] = new int[] { 0, 1, 3, 11 };              //axe         //grapple, hew, bludgeon, intimidate
            MovesetData[7] = new int[] { 10, 3, 15, 16 };            //hammer      //withstand, bludgeon, shatter, blunt (blunt the enemy weapon)
            MovesetData[8] = new int[] { 10, 3, 15, 14 };            //greathammer //withstand, bludgeon, shatter, sweep
            MovesetData[9] = new int[] { 0, 3, 7, 8 };               //fist/claw   //grapple, bludgeon, lacerate, feint
            MovesetData[10] = new int[] { 10, 5, 2, 11 };            //spear       //withstand, puncture, zealous plunge, intimidate
            MovesetData[11] = new int[] { 10, 5, 2, 14 };            //halberd     //withstand, puncture, zealous plunge, intimidate, sweep
            MovesetData[12] = new int[] { 10, 5, 7, 14 };            //scythe      //withstand, puncture, lacerate, intimidate, sweep

            MovesetData[13] = new int[] { 17, 18, 19, 20 };          //0Prisoner
            MovesetData[14] = new int[] { 21, 22, 23, 24 };          //2Skeleton
            MovesetData[15] = new int[] { 25, 26, 27 };              //3The Jailer
            MovesetData[16] = new int[] { 28, 29 };                  //4Dog
            MovesetData[17] = new int[] { 30, 31, 32 };              //5Thrall
            MovesetData[18] = new int[] { 33, 34, 35 };              //6Knight
            MovesetData[19] = new int[] { 36, 37, 38, 39 };          //7Priest
            MovesetData[20] = new int[] { 39, 40, 41 };              //8The Gargoyle
            MovesetData[21] = new int[] { 55, 56, 57, 58 };          //9The Entombed God
            MovesetData[22] = new int[] { 40, 41, 42, 43 };          //10Upper Knight
            MovesetData[23] = new int[] { 44, 45, 46, 47, 48 };      //11The Captian
            MovesetData[24] = new int[] { 52, 53, 54 };              //12The King
            MovesetData[25] = new int[] { 59, 60, 61, 62 };          //13Bloodtinged Knight
            MovesetData[26] = new int[] { 63, 64, 66, 65 };          //14The Young Drake
            MovesetData[27] = new int[] { 67, 68, 69, 70 };          //15The Poisoned Dragon
            MovesetData[28] = new int[] { 71 };                      //16Egg
            MovesetData[29] = new int[] { 72, 73, 74, 75 };          //17Crawling Black Sludge
            MovesetData[30] = new int[] { 76, 77, 78, 79 };          //18Greater Black Sludge
            MovesetData[31] = new int[] { 80, 81, 82, 83 };          //19The Black Mass
        }

        /// <summary>
        /// defining all the variables using WeaponData and some bell curves
        /// </summary>
        /// <param name="passedID">The id that WeaponController.CreateWeapon, used for indexing WeaponData</param>
        public Weapon CreateWeapon(int passedID)
        {
            Weapon weapon = new Weapon {
                id = passedID,
                name = Convert.ToString(WeaponData[passedID, 1]),
                type = Convert.ToInt32(WeaponData[passedID, 2]),
                Durability = Program.rnd.Next(Convert.ToInt32(WeaponData[passedID, 3]), Convert.ToInt32(WeaponData[passedID, 4])) + Program.rnd.Next(Convert.ToInt32(WeaponData[passedID, 3]), Convert.ToInt32(WeaponData[passedID, 4])),
                DmgUp = Convert.ToInt32(WeaponData[passedID, 6]),
                DmgDwn = Convert.ToInt32(WeaponData[passedID, 7]),
                Crit = Convert.ToInt32(WeaponData[passedID, 8]),
                Speed = Convert.ToInt32(WeaponData[passedID, 9]),
                Flavour = Convert.ToString(WeaponData[passedID, 10])
            };

            System.Diagnostics.Debug.WriteLine("\n\n" + weapon.Flavour);
            weapon.BrokenFlavour = BreakFlavour(weapon.Flavour);

            int[] attacks = new int[4];
            //up to ID 30 is player weapons, and beyond that are AI weapons.
            //dont shuffle AI weapons so its easier to program their choices.
            if (passedID < 31)
            {
                //assign the first one
                attacks[0] = MovesetData[weapon.type][0];
                //remove the first one (by putting it into another array, not touching the master)
                int[] ShuffledMoveSet = MovesetData[weapon.type].Where((source, index) => index != 0).ToArray();
                //shuffle the new shorter array
                ShuffledMoveSet = ShuffledMoveSet.OrderBy(x => Program.rnd.Next()).ToArray();

                attacks[1] = ShuffledMoveSet[0];
                attacks[2] = ShuffledMoveSet[1];
                attacks[3] = ShuffledMoveSet[2];
                //is 3 too few for a for loop?
            }
            else {
                attacks[0] = MovesetData[weapon.type][0];
                attacks[1] = MovesetData[weapon.type][1];
                attacks[2] = MovesetData[weapon.type][2];
                attacks[3] = MovesetData[weapon.type][3];
            }
            weapon.attacks = attacks;

            return weapon;
        }

        #region Flavourtext shenanigans

        /// <summary>
        /// size tiers of description: 80 (20), 120 (30) 200 (50)
        /// </summary>
        /// <returns></returns>
        int FlavourBracket(string Flavour)
        {
            int flavourBracket;
            System.Diagnostics.Debug.WriteLine("len = {0}", Flavour.Length);
            if(true) {
                flavourBracket = 50;
                System.Diagnostics.Debug.WriteLine("max length\tbracket: {0}", flavourBracket);
            }
            if(Flavour.Length <= 120) {
                flavourBracket = 36;
                System.Diagnostics.Debug.WriteLine("mid length\tbracket: {0}", flavourBracket);
            }
            if(Flavour.Length < 80) {
                flavourBracket = 20;
                System.Diagnostics.Debug.WriteLine("smallest length\tbracket: {0}", flavourBracket);
            }
            return flavourBracket;
        }

        /// <summary>
        /// splits the string into 3 arrays
        /// </summary>
        /// <param name="debug">if the debug log is shown</param>
        /// <returns></returns>
        string[] BreakFlavour(string Flavour, bool debug = false)
        {
            string[] BrokenFlavour = new string[4];
            int flavourBracket = FlavourBracket(Flavour);
            System.Diagnostics.Debug.WriteLine("\nBREAKFLAVOUR");
            string[] Hold;
            string[] Punctuation = { ".", ",", ";", ":", "-", "(", ")", "!", "?" };

            //size tiers of description: 80 (20), 120 (30) 200 (50)

            
            #region bit between the bracket making and using
            //this is the character limit per line based on how long the whole thing is

            int counter = 0; //how many of string[] Hold we've found a place for
            Hold = Flavour.Split(' '); //the flavor is now broken into words
            for (int i = 0; i < Hold.Length; i++) { System.Diagnostics.Debug.Write(Hold[i]); }
            System.Diagnostics.Debug.WriteLine("");

            //we can only know the line is full once we go over the limit, so we BITES ZA DASTO that shit and go on with our day
            for (int set = 0; set < 4; set++)
            {                          //for each line we're talkin
                if (debug) { System.Diagnostics.Debug.WriteLine("Set {0}", set); }
                while (true)
                {                                           //hey kids wanna buy an infinite for loop?
                    try
                    {
                        if (counter == Hold.Length)
                        {                     //check if we're at the end
                            throw new Exception("Finished");
                        }
                        if (debug) { System.Diagnostics.Debug.WriteLine("Word: {0}", Hold[counter]); }
                        if (Punctuation.Any(Hold[counter].Contains))
                        {   //check if the word is actually just punctuation
                            BrokenFlavour[set] += Hold[counter];                      //put the next punctuation into the line
                            if (debug) { System.Diagnostics.Debug.WriteLine("Word is punc"); }
                        }
                        else
                        {
                            BrokenFlavour[set] += " " + Hold[counter];                //put the next word into the line
                            if (debug) { System.Diagnostics.Debug.WriteLine("Word is not punc."); }
                        }
                        if (debug) { System.Diagnostics.Debug.WriteLine("So Far: {0}", BrokenFlavour[set]); }
                        if (BrokenFlavour[set].Length > flavourBracket)
                        {             //check if it's gone too far
                            BrokenFlavour[set] = BrokenFlavour[set].Remove(BrokenFlavour[set].Length - (Hold[counter].Length + 1), Hold[counter].Length + 1);   //trim that shit off [remove(how much you're keeping, how much after that you're not)]
                            throw new Exception("Line Full");               //KILLA QUEEN DAISAN NO BAKUDAN
                        }
                        else
                        {
                            if (debug) { System.Diagnostics.Debug.WriteLine("\tmoving to next word, counter {0}", counter + 1); }
                            counter++;                                      //move on to the next word
                        }
                    }
                    catch
                    {
                        if (debug) { System.Diagnostics.Debug.WriteLine("\tmoving to next set, counter {0}", counter + 1); }
                        break;                                          //move on to the next line
                    }
                }
            }
            if (debug) { System.Diagnostics.Debug.WriteLine(""); }
            for (int i = 0; i < BrokenFlavour.Length; i++) { System.Diagnostics.Debug.WriteLine(BrokenFlavour[i]); }
            System.Diagnostics.Debug.WriteLine("");

            return BrokenFlavour;
        }

        /// <summary>
        /// prints the weapons data in a box
        /// </summary>
        /// <param name="weapon">what weapon it is</param>
        /// <param name="moves">if the moves are shown too by autoactivating void DisplayMoves()</param>
        /// <param name="debug">if the debus is shown</param>
        public void Display(Weapon weapon, bool moves = false, bool debug = false)
        {
            #region Thinking space
            /*
             * current method:
             *      1 -> 4
             *      print each piece of data in the id
             *      
             * this is good when dumping them one after the other, but not for
             * lining them as as it's only looking at one ID at a time.
             * 
             * .attacks is a 4 long list of the attack IDs
             * 
             * for dataIndex (1 -> 13){ //thats how many pieces of data there are
             *      for attackIndex (1 -> 4){ //to go through the attacks array
             *          printLine("{0}: {1}\n"AttackData[finalAddress, dataIndex], AttackData[attacks[], dataIndex])
             *          
             *          //this should print like:
             *          //Name: ass    Name: ass2   Name: ass3  Name: ass4
             *          //thrust: 100    thrust: 100    thrust: 100 thrust: 100
             *            
             *          //and so on
             *      }
             * }
            */
            #endregion
            if (debug)
            {
                #region the vertical display method used by debug

                for (int attackIndex = 0; attackIndex < 4; attackIndex++) //for each of the 4 attacks
                {
                    int attackID = attacks[attackIndex]; //get the ID of the current one

                    for (int dataIndex = 0; dataIndex < 7; dataIndex++) //for the all of the horizontal data present.
                    {
                        try
                        {
                            if ((int)AttackData[attacks[attackIndex], dataIndex] > 0) //if the data is string, this will throw an exception.
                            {
                                System.Diagnostics.Debug.WriteLine("\t{0}:\t{1}\t\t{2}", attackIndex, AttackData[finalAddress, dataIndex], AttackData[attackID, dataIndex]);
                                //Index:    Name       Value
                            }
                        }
                        catch
                        {
                            System.Diagnostics.Debug.WriteLine("\t{0}:\t{1}\t\t{2}", attackIndex, AttackData[finalAddress, dataIndex], AttackData[attackID, dataIndex]);
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine("\n");

                #endregion
            }

            int totalWidth = 24 + FlavourBracket(weapon.Flavour);
            //size tiers of description: 80 (20), 120 (30), 200 (50)

            Tabgroup();
            Console.Write(" ");
            for (int i = 0; i < totalWidth; i++)
            {
                Console.Write("_");
            }
            Console.Write("\n");

            #endregion
            switch (FlavourBracket(weapon.Flavour))
            {
                case 20:
                    Tabgroup(); Console.Write("| {0,-22}|", weapon.name); //name
                    Tabgroup(); Console.Write("| {0,-22}|", "(" + WeaponCatagories[weapon.type] + ")"); //type
                    Tabgroup(); Console.Write("|");

                /* what is that -22 you ask? let me tell you, you'll be seeing them a lot in this function.
                * it creates a virtual field. let's say i wanted a {0, 10}. 0 is still the next available peramater,
                * but the 10 will take up space. 10 characters worth of space will be taken up, regardless of
                * what the parameter is.
                * print({0, 10}, "ass") would be like `-------ass`, and print({0, -10}, "ass") would be `ass-------`
                * the - are visual aid, obviously. they'd be blank characters.
                * a problem with this system is that you can't use variables in them, so everything has a character limit.
                * That's why this switchcase exists: it uses different spacing depending on how long the flavour text is.
                */

                    for(int i = 0; i < totalWidth; i++)
                    {
                        Console.Write("-");
                    }
                    Console.Write("|\n");
                    Tabgroup(); Console.WriteLine("|{0,11}:\t  {1,-5}|{2,-39} |", "Durability", weapon.Durability, weapon.BrokenFlavour[0]); //durability
                    Tabgroup(); Console.WriteLine("|{0,11}:\t {1,2}-{2,-3}|{3,-39} |", "DMG Base", weapon.DmgDwn, weapon.DmgUp, weapon.BrokenFlavour[1]); //damage
                    Tabgroup(); Console.WriteLine("|{0,11}:\t  {1,-5}|{2,-39} |", "CRT base", weapon.Crit, weapon.BrokenFlavour[2]); //crit
                    Tabgroup(); Console.WriteLine("|{0,11}:\t  {1,-5}|{2,-39} |", "SPD base", weapon.Speed, weapon.BrokenFlavour[3]); //speed
                    break;
                case 36:
                    Tabgroup(); Console.WriteLine("| {0,-59}|", weapon.name); //name
                    Tabgroup(); Console.WriteLine("| {0,-59}|", "(" + WeaponCatagories[weapon.type] + ")"); //type
                    Tabgroup(); Console.Write("|");
                    for (int i = 0; i < totalWidth; i++)
                    {
                        Console.Write("-");
                    }
                    Console.Write("|\n");
                    Tabgroup(); Console.WriteLine("|{0,11}:\t  {1,-5}|{2,-36} |", "Durability", weapon.Durability, weapon.BrokenFlavour[0]); //durability
                    Tabgroup(); Console.WriteLine("|{0,11}:\t {1,2}-{2,-3}|{3,-36} |", "DMG Base", weapon.DmgDwn, weapon.DmgUp, weapon.BrokenFlavour[1]); //damage
                    Tabgroup(); Console.WriteLine("|{0,11}:\t  {1,-5}|{2,-36} |", "CRT base", weapon.Crit, weapon.BrokenFlavour[2]); //crit
                    Tabgroup(); Console.WriteLine("|{0,11}:\t  {1,-5}|{2,-36} |", "SPD base", weapon.Speed, weapon.BrokenFlavour[3]); //speed
                    break;
                case 50:
                    Tabgroup(); Console.WriteLine("| {0,-73}|", weapon.name); //name
                    Tabgroup(); Console.WriteLine("| {0,-73}|", "(" + WeaponCatagories[weapon.type] + ")"); //type
                    Tabgroup(); Console.Write("|");
                    for (int i = 0; i < totalWidth; i++)
                    {
                        Console.Write("-");
                    }
                    Console.Write("|\n");
                    Tabgroup(); Console.WriteLine("|{0,11}:\t  {1,-5}|{2,-50} |", "Durability", weapon.Durability, weapon.BrokenFlavour[0]); //durability
                    Tabgroup(); Console.WriteLine("|{0,11}:\t {1,2}-{2,-3}|{3,-50} |", "DMG Base", weapon.DmgDwn, weapon.DmgUp, weapon.BrokenFlavour[1]); //damage
                    Tabgroup(); Console.WriteLine("|{0,11}:\t  {1,-5}|{2,-50} |", "CRT base", weapon.Crit, weapon.BrokenFlavour[2]); //crit
                    Tabgroup(); Console.WriteLine("|{0,11}:\t  {1,-5}|{2,-50} |", "SPD base", weapon.Speed, weapon.BrokenFlavour[3]); //speed
                    break;
            }

            Tabgroup(); Console.Write("|");
            for (int i = 0; i < totalWidth; i++)
            {
                Console.Write("_");
            }
            Console.Write("|\n");
            Console.Write("\n");

            if (moves)
            {
                DisplayMoves();
            }
        }
        #endregion

        void DisplayMoves()
        {
            //System.Diagnostics.Debug.WriteLine("\t DisplayMoves");
            Console.Write("\t ____________________    ____________________    ____________________    ____________________\n");
            //top of the boxes
            for (int dataIndex = 1; dataIndex < 8; dataIndex++) //each data piece. starting from one to dodge printing the attack ID
            {
                Console.Write("\t");
                System.Diagnostics.Debug.WriteLine("ATTACK new dataIndex {0}/12", dataIndex);
                for (int attackIndex = 0; attackIndex < 4; attackIndex++) //each attack
                {
                    //System.Diagnostics.Debug.WriteLine("\tATTACK new attackIndex {0}/3", attackIndex);
                    try
                    {
                        int hold = (int)AttackData[attacks[attackIndex], dataIndex];
                        //if the data is string, this will throw an exception.
                        Console.Write("|{0,11}:\t{1,-5}|\t", AttackData[finalAddress, dataIndex], AttackData[attackIndex, dataIndex] + "%");
                    } catch //it wasn't an int; and is therefore the name
                    {
                        Console.Write("| {0,-19}|\t", AttackData[attackIndex, dataIndex]);
                        //unique print so it isn't `|Name:   name|` and get the name from the id
                        //(was having issues with it printing the names in the order they're in on the list, even though they're randomised on the weapon) 

                        if (attackIndex == 3) //the last attack. print a blank line for spacing
                        {
                            Console.Write("\n\t|\t\t     |\t|\t\t     |\t|\t\t     |\t|\t\t     |");
                        }
                    }
                }
                Console.Write("\n");
            }
            //bottom of the boxes
            Console.Write("\t|____________________|  |____________________|  |____________________|  |____________________|\n");
        }

        /// <summary>
        /// I dont remember why i decided i needed to do this
        /// </summary>
        void Tabgroup()
        {
            Console.Write("\t");
        }
    }

    struct Weapon
    {
        #region weapon data
        public int id;
        public string name;
        public int type;
        public int Durability { get; set; }
        public int DmgUp { get; set; }
        public int DmgDwn { get; set; }
        public int Crit { get; set; }
        public int Speed { get; set; }
        public string Flavour { get; set; }
        public string[] BrokenFlavour;

        public int[] attacks;
        //all the data a weapon has, in order of how it's stored on the files
        #endregion

    }
}
