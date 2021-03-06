﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REEEE
{
    class Map
    {
        static int currentLocation = 0;

#pragma warning disable IDE0044 // Add readonly modifier
        public static object[,] MapData = Program.ReadFile("MapData.txt");
#pragma warning restore IDE0044 // Add readonly modifier
        static Dictionary<int, string> Nesw = new Dictionary<int, string>
        {
            {0,"North"},
            {1,"East"},
            {2,"South"},
            {3,"West"},
        };

        static int?[] nesw = new int?[4];

        static string previousMarker = " ";

        /// <summary>
        /// prints the map.<para>What did you expect?</para>
        /// </summary>
        static void DisplayMap(bool direction = false)
        {
            System.Diagnostics.Debug.WriteLine("DISPLAYMAP check if map is held : Player.inventory[0, 1] = {0}\n", Player.Inventory[0, 1]);
            if(Player.Inventory[0, 1] == 1) {
                
                Console.WriteLine("____________________________________________________________________\n\\                                                                   \\\n \\    ___________________________________________________________    \\\n /   |           |     |           |     |           |     |     |   /\n/    |           |     |   {24}       |  {29}              |  {34}        |  /\n|    |     {0}     |  {22}  |      __ __|__ __|     {30}     |__ __|  {35}  |  |\n\\    |           |     |     |     |     |           |     |     |  \\\n |   |           |     |     |  {26}     {28}  |           |  {33}  |     |   |\n |   |__ ________|__ __|__ __|_____|_____|___________|__ __|__ __|   |\n \\   |     |     |     |     |     |     |           |     |     |   \\\n  |  |  {1}     {2}  |  {21}     {23}     {25}     {27}        {31}        {32}     {36}  |    |\n /   |__ __|_____|__ __|_____|_____|__ __|___________|_____|_____|   /\n |   |     |     |     |           |     |     |     |     |     |   |\n/    |  {3}     {4}  |  {20}  |           |  {44}     {45}     {46}     {38}        |  /\n|    |_____|__ __|__ __|     {43}     |__ __|_____|__ __|__ __|  {37}  |  |\n\\    |     |     |     |           |     |     |     |     |     |  \\\n |   |  {5}     {6}  |  {19}  |              {47}     {48}  |  {49}  |  {39}  |     |   |\n |   |__ __|__ __|__ __|___________|_____|_____|__ __|__ __|_____|   |\n \\   |     |     |     |     |     |                 |           |   \\\n  |  |  {7}     {8}  |  {16}     {17}     {18}  |                 |           |    |\n /   |_____|__ __|__ __|__ __|__ __|                 |     {40}     |   /\n |   |           |     |     |     |                 |           |  |\n /   |           |  {13}     {14}     {15}  |        {50}        |           | /\n|    |     {9}     |__ __|__ __|__ __|                 |________ __| |\n\\    |           |     |     |     |                 |     |     | \\\n |   |              {10}     {11}     {12}  |                 |  {41}     {42}  |  |\n \\   |___________|_____|_____|_____|_________________|_____|_____|  \\\n  |__________________________________________________________________|\n", MapData[0, 1], MapData[1, 1], MapData[2, 1], MapData[3, 1], MapData[4, 1], MapData[5, 1], MapData[6, 1], MapData[7, 1], MapData[8, 1], MapData[9, 1], MapData[10, 1], MapData[11, 1], MapData[12, 1], MapData[13, 1], MapData[14, 1], MapData[15, 1], MapData[16, 1], MapData[17, 1], MapData[18, 1], MapData[19, 1], MapData[20, 1], MapData[21, 1], MapData[22, 1], MapData[23, 1], MapData[24, 1], MapData[25, 1], MapData[26, 1], MapData[27, 1], MapData[28, 1], MapData[29, 1], MapData[30, 1], MapData[31, 1], MapData[32, 1], MapData[33, 1], MapData[34, 1], MapData[35, 1], MapData[36, 1], MapData[37, 1], MapData[38, 1], MapData[39, 1], MapData[40, 1], MapData[41, 1], MapData[42, 1], MapData[43, 1], MapData[44, 1], MapData[45, 1], MapData[46, 1], MapData[47, 1], MapData[48, 1], MapData[49, 1], MapData[50, 1]);
            }
            if (direction) {
                #region print nesw options
                Console.ForegroundColor = ConsoleColor.Green;
                Program.Scroll(" ____________________________", scrollTime:5, finishTime:20);
                //string str;
                for(int i = 0; i < 4; i++) {
                    if (nesw[i].HasValue) {
                        Program.Scroll("| ", scrollTime:20, lineBreak: 0, finishTime:20);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("{0,-26}", "There is a "+Nesw[i]+" door open");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Program.Scroll(" |", scrollTime:20, tabs: 0, finishTime:20);
                    }
                }
                Program.Scroll("|____________________________|", scrollTime:5);
                #endregion
            }
        }

        /// <summary>
        /// detects which directions are available from a given room
        /// </summary>
        /// <returns>if there is a valid door to north, east, south, west. Null if not.</returns>
        public static int?[] GetDirection()
        {
            //iterate through entire graph information
            for (int i = 2; i < 6; i++) //get the ways that have open doors and add them to int?[]nesw
            {//this for loop does 2,3,4,5 (-2 for 0,1,2,3) as graph index and nesw don't align

                nesw[i - 2] = (int)MapData[currentLocation, i] != -1 ? (int)MapData[currentLocation, i] : (int?)null;
                //if connectiondata != -1 -> connector = connectiondata.  else, connector = (int?)null
                //this line is so fucking hard to read
            }
            //Print direction availability to debug
            for (int i = 0; i < 4; i++)
            {
                System.Diagnostics.Debug.WriteLine("GETDIRECTION nesw[i] : {0}",nesw[i]-1);
            }
            System.Diagnostics.Debug.WriteLine("");

            System.Diagnostics.Debug.WriteLine("PASSIVEACTION Current room ID: {0}\n", currentLocation);
            //gets all the availale directions in a nullable int list



            //for loop printing all the available direction, using the dictionary for the sentance
            //if the value is null on int?[]nesw, it's skipped as there's no doorway in that direction
            #region print nesw options
            Console.ForegroundColor = ConsoleColor.Green;
            Program.Scroll(" ____________________________", scrollTime: 5, finishTime: 20);
            string hold;
            for (int i = 0; i < 4; i++)
            {
                if (nesw[i].HasValue)
                {
                    Program.Scroll("| ", scrollTime: 20, lineBreak: 0, finishTime: 20);
                    Console.ForegroundColor = ConsoleColor.White;
                    hold = "There is a " + Nesw[i] + " Door open";
                    Console.Write("{0, -26}", hold);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Program.Scroll(" |", scrollTime: 20, tabs: 0, finishTime: 20);
                }
            }
            Program.Scroll("|____________________________|", scrollTime: 5);
            #endregion

            return nesw;
        }

        /// <summary>
        /// move the x on the map to where the player is, and check enemy spawn
        /// </summary>
        /// <param name="goTo">the room number of the destination</param>
        static void SetLocation(int goTo)
        {
            MapData[currentLocation, 1] = previousMarker; //remove the x marker from the map
            previousMarker = (string)MapData[goTo, 1];
            MapData[goTo, 1] = "x"; //put it in the new place

            currentLocation = goTo; //make it so you've moved more than graphically

            //check for events of enemy and item (prints that an event is occuring in the debug)
            //the enemy one
            System.Diagnostics.Debug.WriteLine("SETLOCATION enemy spawn ID: {0}", MapData[currentLocation,6]);
            if((int)MapData[currentLocation, 6] >= 0)
            {
                System.Diagnostics.Debug.WriteLine("SETLOCATION enemy present\n", (int)MapData[currentLocation, 6]);
                        
                Hostile hostile = new Hostile();
                hostile.Generate((int)MapData[currentLocation, 6]);
                MapData[currentLocation, 6] = -1;
                //generates a hostile AI using the mapdata as an ID

            }else{
                System.Diagnostics.Debug.WriteLine("SETLOCATION no enemy spawned\n");
            }
        }

        /// <summary>
        /// choosing what to do (when not in combat)
        /// </summary>
        /// <param name="nesw">the output of GetDirection()</param>
        public static void PassiveAction()
        {
            int?[] nesw = GetDirection();

            
            //ask if they want to move, see the map, or inspect
            bool flag = true; //only set to true if the menu wont re-appear that cycle, on move.
            do {

                #region printing the menu
                int toggle = 1;
                void Toggle()
                {
                    toggle*=-1;
                    if(toggle == 1) {
                        Console.ForegroundColor = ConsoleColor.White;
                    } else {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                }
                Toggle();
                Program.Scroll("\n\t ________________________", scrollTime:10, finishTime:30, tabs: 0);
                          Program.Scroll("\t| 1:", 10, 30, 0, 0);
                Toggle(); Program.Scroll("Map              ", 10, 30, 0);
                Toggle(); Console.WriteLine("|"); Program.Scroll("| 2:", 10, 30, 0);
                Toggle(); Program.Scroll("Move             ", 10, 30, 0);
                Toggle(); Console.WriteLine("|"); Program.Scroll("| 3:", 10, 30, 0);
                Toggle(); Program.Scroll("Inspect the Room ", 10, 30, 0);
                Toggle(); Console.WriteLine("|"); Program.Scroll("| 4:", 10, 30, 0);
                Toggle(); Program.Scroll("Inventory        ", 10, 30, 0);
                Toggle(); Console.WriteLine("|"); Program.Scroll("| 5:", 10, 30, 0);
                Toggle(); Program.Scroll("Save             ", 10, 30, 0);
                Toggle(); Console.WriteLine("|"); Program.Scroll("|________________________|", 10, 30);
                Console.ForegroundColor = ConsoleColor.White;
                #endregion

                Console.Write("> ");
                string ans = Console.ReadLine();

                switch(ans) {

                    case "1": //show map
                    DisplayMap();
                    _ = GetDirection();
                    break;

                    case "2": //move
                    flag = false;
                    Move(nesw);
                    break;

                    case "3": //inspect
                    InspectionEvent();
                    break;

                    case "4": //inventory
                    Program.Scroll("[This feature is under construction]");
                    Console.WriteLine("Item: {0}\tqtt: {1}",Player.Inventory[0, 0], Player.Inventory[0, 1]);
                    Console.WriteLine("Item: {0}\tqtt: {1}", Player.Inventory[1, 0], Player.Inventory[1, 1]);
                    break;

                    case "5": //save
                    Program.Scroll("[This feature is under construction]");
                        //this will probably work by vomiting all of mapdata and the player data onto a .txt
                    break;
                }
            } while(flag);
        }

        /// <summary>
        /// uses GetDirection and SetLocation to move the player
        /// </summary>
        /// <param name="nesw">the output of GetDirection()</param>
        static void Move(int?[] nesw)
        {
            //invalid input causes the function to recur
            Program.Scroll("\n\tWhich way do you walk?\t\t[back to return]", lineBreak:2);
            Console.Write("> ");
            string ans = Console.ReadLine();
            string[][] PossibleAnswers = new string[4][];
            
            PossibleAnswers[0] = new string[] { "North", "north", "N", "n", "Up", "up", "u" };
            PossibleAnswers[1] = new string[] { "East", "east", "E", "e", "Right", "right", "r" };
            PossibleAnswers[2] = new string[] { "South", "south", "S", "s", "Down", "down", "d" };
            PossibleAnswers[3] = new string[] { "West", "west", "W", "w", "Left", "left", "l" };

            if (PossibleAnswers[0].Contains(ans) & nesw[0].HasValue) {
                Program.Scroll("You walk through the north door");
                SetLocation(nesw[0].Value - 1);
            } else if (PossibleAnswers[1].Contains(ans) & nesw[1].HasValue)
            {
                Program.Scroll("You walk through the east door");
                SetLocation(nesw[1].Value - 1);
            } else if (PossibleAnswers[2].Contains(ans) & nesw[2].HasValue)
            {
                Program.Scroll("You walk through the south door");
                SetLocation(nesw[2].Value - 1);
            } else if (PossibleAnswers[3].Contains(ans) & nesw[3].HasValue)
            {
                Program.Scroll("You walk through the west door");
                SetLocation(nesw[3].Value - 1);
            } else if (ans == "back")
            {
                PassiveAction();
            } else
            {
                Move(nesw);
            }
        }

        /// <summary>
        /// Checks the mapdata for what happens on inspection.<para> Seperate function so it can be easily tied to the passive action menu</para>
        /// </summary>
        static void InspectionEvent()
        {
            if((int)MapData[currentLocation, 7] == -1)
            {
                MapData[currentLocation, 7] = Program.rnd.Next(1, 10);
            }
            //this should be hard-written into mapdata, but i have more press matters. 7, 8 & 9 will give no detail.

            int randomat = Program.rnd.Next(1, 101);
            bool selection()
            {
                Console.ForegroundColor = ConsoleColor.White;
                Program.Scroll("Interact?");
                Program.Scroll("[Y / N]", 0, 0, 2, 2);
                Console.Write("> ");
                string ans = Console.ReadLine();
                Console.WriteLine();

                if(ans == "Y" || ans == "y") //input validation. could add more.
                {
                    return true;
                }
                else{
                    return false;
                }
            }

            System.Diagnostics.Debug.WriteLine("INSPECTEVENT inspect event {0} present", (int)MapData[currentLocation, 6]);

            switch ((int)MapData[currentLocation, 7]) {
                case -2:
                    Program.BasicBox("", message:"You've already scavinged this room.");
                    break;
                case 0:
                    Merchant merchant = new Merchant();
                    merchant.Generate();
                    break;
                case 1:
                    Program.BasicBox("", message:"There is a crate in the corner.");
                    if (selection()) {
                        if(randomat < 26)
                        {
                            Program.BasicBox("", message:"The crate is empty.");
                        } else if(randomat < 46)
                        {
                            Program.BasicBox("", message:"The crate contains supplies.");
                            if(Player.Health > (Player.MaxHealth * 0.66))
                            {
                                Player.MaxHealth = (int)Math.Floor(Player.MaxHealth* 1.2);
                            }
                            else {
                                Player.Health = (int)Math.Floor(Player.Health*1.3);
                            }
                            Program.Player.Display();
                        }else
                        {
                            Program.BasicBox("", message:"You cut your finger in the crumbled wood.");
                            int[] DOT = new int[] {1, 2, 3};
                            Program.Player.Damage(0, DOT);
                        }
                        MapData[currentLocation, 7] = -2;
                    }
                    break;
                case 2:
                    if (selection()) {
                        if(randomat < 61)
                        {
                            Program.BasicBox("", message:"The bag is empty.");
                        }else
                        {
                            Program.BasicBox("", message:"The bag contains supplies!");
                            int gold = Program.rnd.Next(10, 12) + Program.rnd.Next(10, 12);
                            Player.Funds += gold;
                            Player.Health = (int)Math.Floor(Player.Health* 1.25);
                            if (Player.Health> Player.MaxHealth)
                            {
                                Player.Health = Player.MaxHealth;
                            }
                        }
                        MapData[currentLocation, 7] = -2;
                    }
                    break;
                case 3:
                    Program.BasicBox("", message:"A large, iron chest stands to one side.");
                    if (selection()) {
                        if(randomat < 31)
                        {
                            Program.BasicBox("", message:"Treasure!");
                            int gold = Program.rnd.Next(15, 30) + Program.rnd.Next(15, 30);
                            Player.Funds += gold;
                        }else if(randomat < 66)
                        {
                            Program.BasicBox("", message:"The chest is trapped!");
                            int[] DOT = new int[] {1, 2, 3};
                            Program.Player.Damage(0, DOT);
                        }
                        else {
                            Program.BasicBox("", message:"The chest is trapped!");
                            int[] DOT = new int[] {2, 2, 3};
                            Program.Player.Damage(0, DOT);
                        }
                        MapData[currentLocation, 7] = -2;
                    }
                    break;
                case 4:
                    Program.BasicBox("", message:"The brightly lit room provides minor relaxation for your weary bones.");
                    Player.Health = (int)Math.Floor(Player.Health* 1.25);
                    if (Player.Health> Player.MaxHealth)
                    {
                        Player.Health = Player.MaxHealth;
                    }
                    Player.Dodge = (int)Math.Floor(Player.Dodge* 1.3);
                    MapData[currentLocation, 7] = -2;
                    break;
                case 5:
                    Program.BasicBox("", message:"A great statue of an angel stands in the center.");
                    if (selection()) {
                        if(randomat < 50)
                        {
                            Program.BasicBox("", message:"It gazes deep into your soul.");
                            Player.Dodge = (int)Math.Floor(Player.Dodge* 0.6);
                        }
                        else {
                            Program.BasicBox("", message:"you feel envigorated!");
                            Player.Dodge = (int)Math.Floor(Player.Dodge* 1.3);
                        }
                        MapData[currentLocation, 7] = -2;
                    }
                    break;
                case 6:
                    Program.BasicBox("", message:"The room is decorated with large urns.");
                    if (selection()) {
                        if(randomat < 21)
                        {
                            Program.BasicBox("", message:"They're all empty.");
                        }
                        else if(randomat < 41)
                        {
                            Program.BasicBox("", message:"They're filled with offerings to the dead. better off in your hands.");
                            int gold = Program.rnd.Next(15, 25) + Program.rnd.Next(15, 25);
                            Player.Funds += gold;
                        }else {
                            int[] DOT = new int[] {1, 2, 3};
                            Program.Player.Damage(0, DOT);
                        }
                        MapData[currentLocation, 7] = -2;
                    }
                    break;
                default:
                    Program.BasicBox("", message:"The room is devoid of detail.");
                    break;
            }
        }

        /// <summary>
        /// Writes all merchant locations to the map graphic my detecting their inspection event
        /// </summary>
        public static void ShowMerchants()
        {
            for(int i = 0; i < (MapData.Length/8); i++) {
                if((int)MapData[i,7] == 0) {
                    MapData[i,1] = "M";
                }
            }
            previousMarker = "M";
            DisplayMap();
        }
        
    }
}
