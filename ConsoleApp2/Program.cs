﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace REEEE
{
    /* 
     * 120*30 default console size
     * 
     * https://grouflon.itch.io/veiled
     * 
     * The FontStruction “7:12 Serif Italic” (https://fontstruct.com/fontstructions/show/413872) by Christian Munk is
     * licensed under a Creative Commons Attribution Share Alike license (http://creativecommons.org/licenses/by-sa/3.0/)
     *  
     *  TODO:        
     *      > finish the attack system
     *      > special effect on moves (debuff or bonus vs type)
     *      > multi-enemy encounters
     *      > .txt -> .xml
     *      > give things sensible prices
     *  
     *  Map Rework Idea:
     *      > make it based around player orentation rather than the sterotypical omniciance
     *      > "there is a door infront, behind, right and left"
     *      > no marker of where you are on the map itself
     *      > compass item to go back to nesw, pen item to mark where you've been
     *      > chance to lose orientation during combat?
     */

    class Program
    {
        public static Random rnd = new Random();

        public static Player Player = new Player();
        //make the player

        /// <summary>
        /// if you dont know what this is for, go back to codecademy<para>The main game loop</para>
        /// </summary>
        static void Main()
        {
            Map.MapData[0, 1] = "x";                //mark that the player is in the first room
            WeaponController.AssignMovesetData();   //compile the movesets
            
            Player.Generate();                      //start the opening cutscene
            do { //they call it a game loop for a reason
                if (Globals.InCombat)
                {
                    //come back to this when i have a LOT of gin
                    //Dont have gin; bought a bottle of kraken specially and walnut bought me programmer socks
                    System.Diagnostics.Debug.WriteLine("MAIN New combat turn");
                    //maybe add some speed property to decide who goes first. make it a weighted chance of being first rather than definitive > or <
                    Player.DecideAttack();
                    Globals.Target.DecideAttack();

                } else {
                    Player.DamageOverTime();
                    Map.PassiveAction();
                }
            } while(true);
        }

        /// <summary>
        /// For making strings print one character at a time
        /// </summary>
        /// <param name="msg">The string that will be printed.<para>does not work with the "value: {0}",var method, use "value: "+var</para></param>
        /// <param name="scrollTime">Time between each character being printed in thousands of a second.</param>
        /// <param name="finishTime">Time after the string is finished in thousands of a second.</param>
        /// <param name="lineBreak">How many times \n is sent AFTER msg</param>
        /// <param name="tabs">How many times \t is sent BEFORE msg</param>
        public static void Scroll(string msg, int scrollTime = 30, int finishTime = 1000, int lineBreak = 1, int tabs = 1)
        {
            for(int i = 0; i < tabs; i++) { //do the tabs
                Console.Write("\t");
            }
            for(int i = 0; i < msg.Length; i++) { //print the message
                System.Threading.Thread.Sleep(scrollTime);
                Console.Write(msg[i]);
            }
            for(int i = 0; i < lineBreak; i++) { //do the line breaks
                Console.Write("\n");
            }
            System.Threading.Thread.Sleep(finishTime); //end sleep
        }

        /// <summary>
        /// Making files into 2D arrays. Can only detect Int32 and String, seperated by |. uses `%EOF%` to find end of file automatically
        /// </summary>
        /// <param name="location1">E:\My Drive\Ban This Man\name.txt</param>
        /// <returns></returns>
        public static object[,] ReadFile(string location1)
        {

            #region finding the file
            string[] File;
            try {
                File = System.IO.File.ReadAllLines(Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString(), location1));
                //BASICALLY, GetCurrentDirectory() gives "C:\ [yadda yadda] \ConsoleApp2\bin\Debug". this get the parent twice to go to \ConsoleApp2,
                //then grafts on the actual file name from the function call to find the file. I hate it too.

                System.Diagnostics.Debug.WriteLine(location1, " found");
                //get the name of the file in question for the debug
            } catch {
                Console.WriteLine("File {0} not found", location1);
                File = new string[] {""};
                System.Threading.Thread.Sleep(4000);
                System.Environment.Exit(1);
            }
            #endregion

            #region finding EOF
            int length = 1000;
            int width = 1000;
            //finding the vertical EOF
            for (int i = 0; i < 1000; i++) //iterate through each line of the file
            {
                if(File[i] == "%EOF%")
                {
                    length = i;
                    System.Diagnostics.Debug.WriteLine("%EOF% on row {0}", i);
                    break;
                }
            }

            //finding the horizontal EOF
            string hold = File[0]; //the first line of the file as a string
            string[] Hold = hold.Split('|'); //the first line of the file as a string[]

            for (int i = 0; i < width; i++) //iterate through the string[]
            {
                if (Hold[i] == "%EOF%")
                {
                    width = i;
                    System.Diagnostics.Debug.WriteLine("%EOF% on columnbn't {0}", i);
                    break;
                }
            }

            #endregion

            object[,] Data = new object[length, width]; //the final resting place. has both int and string so types needs to be object

            for(int i = 0; i < length; i++) //iterate through each line of the file
            {
                hold = File[i]; //the hold string is that line
                Hold = hold.Split('|'); //the individual file data is seperated by /

                //this gets those into their own lists
                for(int i2 = 0; i2 < width; i2++) //iterate through that line
                {
                    bool canConvert = int.TryParse(Hold[i2], out _);
                    Data[i, i2] = canConvert == true ? int.Parse(Hold[i2]) : (object)Hold[i2];
                    //this is a "conditional assignment". basically if x is true a = b, else a = c

                    //dumps that elements characteristics into debug. by the end, this is a detailed description of all of Mapdata[,]
                    System.Diagnostics.Debug.WriteLine("[i1] Row: {0}\t[i2] column: {1}\t\t{2}\t{3}", i, i2, Data[i, i2], Data[i, i2].GetType().Name);
                }
            }
            System.Diagnostics.Debug.Write("\n=====================================================================\n");
            return Data;
        }
    }

    /// <summary>
    /// Small data thats needed all over the place and/or may change often.<para>EF target, currently held weapon, combat flag</para>
    /// </summary>
    class Globals
    {
        ///<summary>the "target" the player is in combat with. needs to be global so different objects can cha cha slide in and out.</summary>
        public static Hostile Target;
        ///<summary>weapon held by the player</summary>
        public static Weapon HeldWeapon;
        ///<summary>self explanitory</summary>
        public static bool InCombat { get; set; }
    }
}