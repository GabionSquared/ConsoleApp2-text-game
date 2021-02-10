using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace REEEE
{
    //1275 lines

    /* GIGA BRAIN ZONE
     * 
     * https://www.text-image.com/convert/ascii.html
     * 120*30 default console size
     *      this is certianly a plan, if we were limiting ourselves to the console.
     *      that being said, we can take some artistc lisense from that
     *          https://grouflon.itch.io/veiled
     *          
     *          HEY KIDS WANNA BUY SOME HARDWRITTEN DX11
     *          
     *          https://stackoverflow.com/questions/9712932/2d-xna-game-mouse-clicking
     *          
     *  The FontStruction “7:12 Serif Italic” (https://fontstruct.com/fontstructions/show/413872) by Christian Munk is
     *  licensed under a Creative Commons Attribution Share Alike license (http://creativecommons.org/licenses/by-sa/3.0/)
     *  
     *  TODO:        
     *      > finish the attack system
     *      > special effect on moves (debuff or bonus vs type)
     *      > graphics using Monogame
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
        /// <param name="location2">G:\My Drive\Ban This Man\name.txt</param>
        /// <returns></returns>
        public static object[,] ReadFile(string location1, string location2)
        {
            #region finding the file
            string[] File;
            try { //for some reason it's [E:\] in college but [G:\] at home
                File = System.IO.File.ReadAllLines(location1);
                System.Diagnostics.Debug.WriteLine(location1.Substring(location1.IndexOf("Ban This Man") + 13), " found in Location 1");

            } catch {
                File = System.IO.File.ReadAllLines(location2);
                System.Diagnostics.Debug.WriteLine(location2.Substring(location2.IndexOf("Ban This Man") + 13), " found in Location 2:");
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
            //this gets those into their own lists
            for (int i = 0; i < width; i++) //iterate through Hold
            {
                if (int.TryParse(Hold[i], out _)) //if it's int
                {
                    break;
                }
                else if (Hold[i] == "%EOF%")
                {
                    width = i-1;
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
        //the "target" the player is in combat with. needs to be global so different objects can cha cha slide in and out.
        public static Hostile Target;
        ///<summary> names of the stats are universal </summary>

        public static Weapon HeldWeapon;

        public static bool InCombat { get; set; }
    }
}