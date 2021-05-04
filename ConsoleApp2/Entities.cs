using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace REEEE
{

    /// <summary>
    /// holds the basic functions for all entities
    /// </summary>
    abstract class EntityFramework
    {
        #region instantiation
        //data for the moves. this isn't in Weapons.cs because it's needed in Attack()
        protected static Random rnd = new Random();
        public WeaponController WeaponInterpreter = new WeaponController();
        //names, default gear and resistances
        protected readonly static int hostileWidth = 12;
        protected readonly static object[,] HostileData = Program.ReadFile("HostileData.txt");
        protected StatDictionary Stats;

        public bool Stunned = false;
        public int ID{ get; set; }
        public string Name { get; set; }

        int[,] Dot = new int[,] { { 0, 0 }, { 0, 0 } };
        /*
                         ________________
	                    | TURNS | DAMAGE |
	            ________|-------|--------|
	           | POISON |   0   |   0    |
	           |--------|-------|--------|
	           |  BLEED |   0   |   0    |
	           |________|_______|________|
         */
        //these are ways of accessing the private/protected field of the same name (with lower case).
        //the name one is expanded for demonstration.
        #endregion

        /// <summary>
        /// Make a dictionary of all the resistances the entitiy will have.<para>all but 1-3 are in percentages</para>
        /// </summary>
        /// <param name="mxhlth">Health Value Cap</param>
        /// <param name="dgd">Dodge. how likley an attack won't connect on the reciving end.<para>Be mindful of accuracy, the opposite.</para></param>
        /// <param name="phy">Physical Defense. Blunt force trauma, effective on armor</param>
        /// <param name="psn">Poison. Effective on the sanguinous</param>
        /// <param name="bld">Bleed. Effective on thing with blood</param>
        /// <param name="stn">Stun.</param>
        /// <returns></returns>
        protected static StatDictionary CompileInt(int mxhlth = 30, int dgd = 5, int phy = 10, int psn = 10, int bld = 10, int stn = 10)
        { // you can't execute the script to add the things in the instantiation phase, hense this function
            StatDictionary Stats = new StatDictionary
            {
                {"Health",mxhlth},
                {"MaxHealth",mxhlth},
                {"Dodge",dgd},
                //protection goes here. redundant, just give more health
                {"Poison",psn},
                {"Bleed",bld},
                {"Stun", stn}
            };
           
            return Stats;
        }
    
        /// <summary>
        /// name, health as an integer and percentage based health bar
        /// </summary>
        protected void Display()
        { //print({0, -10}, "ass") would be `ass-------`
            int toggle = 1;
            void Toggle()
            {
                toggle*=-1;
                if(toggle == 1) {
                    Console.ForegroundColor = ConsoleColor.White;
                } else {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                }
            }

            Toggle();
            Console.Write("\t\t _____________________________\n\t\t|");
            Toggle();
            Console.Write(" Name: {0, -21}", Name);
            Toggle();
            Console.Write(" |\n\t\t|=============================|\n\t\t|");
            Toggle();
            string healthString = " Health: "+ Stats["Health"]+ "/"+ Stats["MaxHealth"];
            Console.Write("{0, -29}", healthString);
            double hold = Math.Round(((double)Stats["Health"] / Stats["MaxHealth"]) * 10);
            Toggle();
            Console.Write("|\n\t\t|");
            Toggle();
            Console.Write(" [");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            for(int i = 0; i < hold; i++) {
                Console.Write("#");
            }
            Console.ForegroundColor = ConsoleColor.DarkGray;
            for(int i = 0; i < 10 - hold; i++) {
                Console.Write("-");
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("]");
            Toggle();
            Console.WriteLine("                |\n\t\t|_____________________________|\n");
            Toggle();
        }
   
        /// <summary>
        /// Generates and adds a weapon to the weaponinventory. Do not stack.
        /// </summary>
        /// <param name="id">The id of the weapon to generate</param>
        /// <param name="WeaponInventory">The entitiy's unique weapon inventory</param>
        /// <param name="source">Name of the entity, used for the debug log</param>
        /// <param name="fresh">If the weapon has already been generated in a merchant. overrides use of ID</param>
        /// <param name="premade">The existing weapon, for use with bool fresh</param>
        protected void AddWeapon(int id, Weapon[] WeaponInventory, string source, bool fresh = false, Weapon premade = default)
        {
            System.Diagnostics.Debug.WriteLine("ADDWEAPON adding weapon {0} to {1}", id, source);
            bool found = false;

            //cycle throug the weapon inventory
            for(int i = 0; i < WeaponInventory.Length && found == false; i++) {
                System.Diagnostics.Debug.WriteLine("ADDWEAPON checking {0} for null", i);
                if(WeaponInventory[i].Equals(default(Weapon))){ //if an open spot is found
                    found = true; //raise the flag
                    //vomit a new weapon based on the id or the premade
                    if(!fresh) {
                        WeaponInventory[i] = WeaponInterpreter.CreateWeapon(id);
                        System.Diagnostics.Debug.WriteLine("ADDWEAPON null found at {0}, created id {1}: {2}\n", i, id, WeaponInventory[i].name);
                    } else {
                        WeaponInventory[i] = premade;
                        System.Diagnostics.Debug.WriteLine("ADDWEAPON null found at {0}, used existing weapon\n", i);

                        //if it was a purchase, offer to equip it immediatly
                        string choice;
                        if(source == "merchant purchase") {
                            Program.Scroll("Equip Now?");
                            Program.Scroll("[Y / N]", 0, 0, 2, 2);
                            Console.Write("> ");
                            choice = Console.ReadLine();
                            if(choice == "Y" || choice == "y") {
                                Globals.HeldWeapon = WeaponInventory[i];
                                WeaponInterpreter.Display(Globals.HeldWeapon, true);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Add an item to the inventory, by incrementing a stack or making a new one
        /// </summary>
        /// <param name="id">The id of the item</param>
        /// <param name="stacks">If True, garantees that a new stack will be made</param>
        protected static void AddItem(int id, int[,] Inventory, bool stacks = false)
        {
            System.Diagnostics.Debug.WriteLine("AddItem ID:{0} ", id);
            bool stop = false;
            if(stacks) {
                for(int i = 0; i < Inventory.Length & stop == false; i++) {
                    System.Diagnostics.Debug.WriteLine("AddItem search for existing stack: {0} {1}", i, Inventory[i, 0]);
                    if(Inventory[i, 0] == id) {
                        stop = true;
                        Inventory[i, 1]++;
                        System.Diagnostics.Debug.WriteLine("AddItem found at {0}", Inventory[i, 0]);
                    }
                }
                if(!stop) { //the linear search didnt find the item, so its new
                    System.Diagnostics.Debug.WriteLine("AddItem stack not found. Recursing...");
                    AddItem(id, Inventory);
                }

            } else {//a new stack needs to be made

                System.Diagnostics.Debug.WriteLine("Additem new stack");
                for(int i = 0; i < Inventory.Length & stop == false; i++) {
                    System.Diagnostics.Debug.WriteLine("AddItem search for empty index: {0} {1}", i, Inventory[i, 0]);
                    if(Inventory[i, 0] == 0) {
                        System.Diagnostics.Debug.WriteLine("AddItem created new stack at {0}\n", Inventory[i, 0]);
                        stop = true;
                        Inventory[i, 0] = id;
                        Inventory[i, 1] = 1;
                    }
                }
            }
        }

        /// <summary>
        /// Deal Damage
        /// </summary>
        /// <param name="damage">base damage from the weapon. AI do not possess this attribute.</param>
        /// <param name="id">the attack ID</param>
        /// <param name="Target">the recieving entity</param>
        protected void Attack(int damage, int id, EntityFramework Target)
        {
            //should it miss -> how much damage (target.stats) -> should it stun -> reciever.damage
            if(rnd.Next(0, 100) < (int)WeaponController.AttackData[id, 2]) { //accuracy check

                int AISpacer = 0; //because how much damage the AI does is decided here, all the data has been shifted by 1 for them

                if(id < 17) {   //player attacks
                    damage *= (int)Math.Round(Convert.ToSingle(WeaponController.AttackData[id, 3]) / 100);
                    //int             double                        object          int

                    if(Globals.HeldWeapon.Durability > 0) {
                        Globals.HeldWeapon.Durability--;
                        if(Globals.HeldWeapon.Durability == 0) {
                            Program.Scroll("Your weapon crumbles in your hands...");
                        }
                    }else if(Globals.HeldWeapon.Durability == 0) {
                        Program.Scroll("Your shattered weapon is much less effective...");
                        damage = (int)Math.Ceiling(damage*0.25);
                    }
                    //reduces durability by 1 with every use, notifying when it hits 0.
                    //every use after that will do 25% damage.

                } else {        //AI attacks
                    AISpacer = 1;
                    damage = rnd.Next((int)WeaponController.AttackData[id, 3], (int)WeaponController.AttackData[id, 4]);
                }

                if(rnd.Next(0, 100) < (int)WeaponController.AttackData[id, 4 + AISpacer]) { //crit. uses the value as a % chance.
                    double hold = damage;
                    hold *= 1.5; //burner variable because *1.5 returns n.5 on odd numbers
                    damage = (int)Math.Floor(hold);
                    Program.Scroll("A critical Strike!");
                    // multiplying by a float returns a double or float, damage is an int.
                }

                int[] TransferDot = new int[3];
                int r = (int)WeaponController.AttackData[id, 5 + AISpacer];
                if (r <= 3) {
                    for(int i = 0; i < 3; i++)
                        TransferDot[i] = (int)WeaponController.AttackData[id, 5 + AISpacer + i];
                }
                //puts all the effect data into a transfer, but only if the ID is 1-3 (poison, bleed, stun)
                //[id][strength][duration]
                
                Target.Damage(damage, TransferDot);
                //taking damage is a seperate function so it's easier to access their own dodge and prot values.
                //this could plausably be done using the Using(x) function, but thats bad conpartmentalisation.

            } else {//the attack missed
                Program.Scroll("The Attack Missed!");
            }
        }

        /// <summary>
        /// Recieve Damage
        /// </summary>
        /// <param name="damage">amount of incoming damage</param>
        protected void Damage(int damage, int[] TransferDot)
        {
            //try to dodge -> get stunned -> take damage -> see if dead
            if(rnd.Next(0, 100) > Stats["Dodge"]) { //dodge check

                /*
                 * int[,] DOT = new int[,] { { 0, 0 }, { 0, 0 } };
                 * Bleed strength, Bleed Time, Poison Strength, Poison Time
                 */

                for (int i = 0; i < 2; i++) {
                    if (TransferDot[0] == i) {
                        if (rnd.Next(0, 100) < Stats.Indexer(i+4)) { //4,5,6
                            if (Dot[i, 0] != 0) {
                                double x = TransferDot[1] / 2;
                                Dot[i, 0] += (int)Math.Ceiling(x);  //if already poisoned/bled, increment strength by half what's requested (rounded up)
                            }
                            else {
                                Dot[i, 0] += TransferDot[1]; //if not already poisoned/bled, apply at full strength
                            }
                            Dot[i, 1] += TransferDot[2]; //increment the time, regardless of what already existed
                        }
                        else {
                            Program.Scroll("The "+ Stats.IndexKeys(i + 4) + " Was Resisted");
                        }
                    }
                }
                if(rnd.Next(0, 100) < Stats["Stun"] && TransferDot[0] == 3) {
                    Stunned = true;
                }

                Console.WriteLine("{0} damage done to {1}", damage, Name);
                Stats["Health"] -= damage;

                //check for death
                if(Stats["Health"] <= 0) {
                    Stats["Health"] = 0;
                    Death(); //make health always 0 rather than a negative on death, to stop weird displays
                } else {
                    Display();
                }
            } else {
                Program.Scroll("The Attack Was Dodged!");
            }
            DamageOverTime();
            //take DOT, outside the dodge because it's already there
        }

        /// <summary>
        /// applies and degrades active damage over time
        /// </summary>
        public void DamageOverTime(){
            for(int i = 0; i < 2; i++) { //only does 0 and 1, for bleed and poison
                if(Dot[i, 0] > 0) { //if active time

                    Stats["Health"] -= Dot[i, 1]; //take the damage

                    if(Stats["Health"] <= 0) { //check if dead
                        Death();
                    }

                    Dot[i, 1]--;
                    if(Dot[i, 0] > 0) { //if no more time
                        Dot[i, 1] = 0;  //remove strength
                    }
                }
            }
        }

        ///<summary>
        ///death message. This should never be seen by player, always overwritten
        ///</summary>
        protected virtual void Death()
        {
            Console.WriteLine("Entity died");
        }
    }

    class Player: EntityFramework
    {
        #region instantiation
        public static int[,] Inventory = new int[50, 2];
        //make sure this is at LEAST along enough to hold 1 of every item in the game, or figure out how to extend it

        public static Weapon[] WeaponInventory = new Weapon[2];
        //this NEEDS to be extendable
        public static int Funds { get; set; }

        public static bool metMerchant = false;
        //tracking if the player has met a merchant, as the first one gives a tutorial
        #endregion

        /// <summary>
        /// creates the player using StatGen and a cutscene for the name
        /// </summary>
        /// <returns>name, maxHealth (100), Stats (all 5)</returns>
        public void Generate()
        {
            //this code is fucking unreadable, but that's literally not my problem
            Program.Scroll("PLEASE REMEMBER THAT IT IS POSSIBLE TO QUEUE INPUTS. ", 10, 750, 0, 0);
            Program.Scroll("DO NOT ACCIDENTALLY PRESS ENTER.", 10, 750, 1, 0);
            for(int i = 0; i < Console.WindowWidth-1; i++) {
                Console.Write("-");
            }
            Program.Scroll("-", 10, 2000, 3, 0);

            Stats = CompileInt(); //generate the player using the default values
            Console.ForegroundColor = ConsoleColor.Cyan;
            Program.Scroll("Hm?", finishTime: 3000);
            Program.Scroll("Who might you be?");
            Console.ForegroundColor = ConsoleColor.White;
            Program.Scroll("[Input your name. You will not be able to change this later]", 0, 0, 2, 2);

            bool flag = false;
            //do while for entering and confirming the name entered with a boolean flag
            bool testname = false;
            string[] nameSplit = {"null", "null"};
            //because the debug name is in a try catch, any test features in it won't throw a visable exception, just continue this function
            //this is a flag *outside* the try catch that the game is in a debug state

            do//loops until the player confirmes their name
            {
                string tempname;
                Console.Write("> ");
                tempname = Console.ReadLine();
                Console.WriteLine();
                try//because of the split and parse, this might cause an error
                {
                    nameSplit = tempname.Split(' ');
                    if(nameSplit[0] == "devskip" || nameSplit[0] == "debug") {
                        testname = true;
                        //syntax is "devskip 0"
                    }
                } catch {
                    System.Diagnostics.Debug.WriteLine("GENERATE authentic input");
                }
                if (testname) {
                    Funds = 300;
                    System.Diagnostics.Debug.WriteLine("NAME {0}; {1}", nameSplit[0], nameSplit[1]);
                    AddItem(1, Inventory); //add map
                    AddWeapon(int.Parse(nameSplit[1]), WeaponInventory, "player"); //add whatever weapon
                    metMerchant = true;
                    System.Diagnostics.Debug.WriteLine("WeponInventory index 0: {0}\n", WeaponInventory[0]);
                    Name = "debug";
                    Globals.HeldWeapon = WeaponInventory[0]; //set whatever weapon was spawned in as active

                    Display();
                    WeaponInterpreter.Display(Globals.HeldWeapon, true);

                    Merchant merchant = new Merchant();
                    merchant.Generate();
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Program.Scroll("You're certian?");
                Console.ForegroundColor = ConsoleColor.White;
                Program.Scroll("[Y / N]", 0, 0, 2, 2);
                Console.Write("> ");
                string ans = Console.ReadLine();
                Console.WriteLine();

                if(ans == "Y" || ans == "y") //input validation. could add more.
                {
                    Name = tempname;
                    flag = true;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Program.Scroll("At least you know your own name...", lineBreak: 0);
                    Program.Scroll("Doing better than most rabble already.", tabs: 1);
                    Program.Scroll("Greetings, ", finishTime: 30, lineBreak: 0); Console.ForegroundColor = ConsoleColor.DarkCyan; Program.Scroll(Name + ".", finishTime: 2500, tabs: 0);
                } else {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Program.Scroll("Don't remember?", lineBreak: 0);
                    Program.Scroll("Take your time...", lineBreak: 2);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            } while(flag == false);

            //figure out how to add a wait
            Console.ForegroundColor = ConsoleColor.Cyan;
            Program.Scroll("\n\tWhat's that look for? ", finishTime: 500, lineBreak: 0, tabs: 0);
            Program.Scroll("Don't know what you're doing here?", tabs: 0);
            Program.Scroll("There used to be " + Stats.Count() + " different statistics until the developer scrapped them and made a dictionary.\n\tIt Says:");
            Console.ForegroundColor = ConsoleColor.Red;
            for(int i = 0; i < 6; i++) {
                Program.Scroll(Stats.IndexKeys(i), 50, 500, 1, 2);
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Program.Scroll("\n\tYou seem pretty average across the board; ", finishTime: 500, lineBreak: 0, tabs: 0);
            Program.Scroll("not that I know what they mean.", tabs: 0);
            Program.Scroll("Here, let me draw a map for you", lineBreak: 0);
            Program.Scroll(".", lineBreak: 0, tabs: 0);
            Program.Scroll(".", lineBreak: 0, tabs: 0);
            Program.Scroll(".", lineBreak: 2, tabs: 0);

            Console.ForegroundColor = ConsoleColor.White;
            do//loops until the player decides yes or no
            {
                Program.Scroll("[Accept Map?]", 0, 0, 2, 2);
                Program.Scroll("[Y / N]", 0, 0, 1, 2);
                Console.Write("> ");
                string ans = Console.ReadLine();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                if(ans == "Y" || ans == "y") //input validation. could add more.
                {

                    flag = true;
                    Program.Scroll("Off you go, then");
                    AddItem(1, Inventory);
                } else {
                    Program.Scroll("...", scrollTime: 300, finishTime: 2000);
                    Program.Scroll("alright then.");
                }
            } while(flag == false);

            Console.ForegroundColor = ConsoleColor.White;
            Program.Scroll("\t[You see a broken sword on the ground. It's a start.]");
            AddWeapon(0, WeaponInventory, "player");

            Globals.HeldWeapon = WeaponInventory[0];

            Display();
        }

        /// <summary>
        /// displays the 4 moves of the held weapon, then uses the selection in ef.attack();
        /// </summary>
        public void DecideAttack()
        {
            if(Stunned) {
                Program.Scroll("You are stunned!");
                return;
            }

            WeaponInterpreter.Display(Globals.HeldWeapon, true);


            string attackRawInp;
            do {
                Program.Scroll("\n1-4\n> ", lineBreak: 0);
                attackRawInp = Console.ReadLine();
            } while(attackRawInp != "1" & attackRawInp != "2" & attackRawInp != "3" & attackRawInp != "4");
            //false             true                  false                 false  
            System.Diagnostics.Debug.WriteLine("raw input collected: {0}", attackRawInp);

            int attack = (int)WeaponController.AttackData[Globals.HeldWeapon.attacks[int.Parse(attackRawInp)-1], 0];
            System.Diagnostics.Debug.WriteLine("relevant ID: {0}", attack);
            //cha cha real smooth from 1-4 as a string to the attack's ID as an int

            if (rnd.Next(0, 10) > 3) { //30% chance to not lose durability, for the memes
                Globals.HeldWeapon.Durability--;
            }

            Attack(rnd.Next(Globals.HeldWeapon.DmgUp, Globals.HeldWeapon.DmgDwn), attack, Globals.Target);
            //shipping all that good shit off to the actual attack thingie
        }

        /// <summary>
        /// Player's death message.
        /// </summary>
        protected override void Death()
        {
            Console.WriteLine("You have died!\nThese were your final stats:");
            Display();
        }
    }

    /// <summary>
    /// all attacking entities
    /// </summary>
    class Hostile: EntityFramework
    {
        #region instantiation
        readonly static int[,] Inventory = new int[50, 2];
        readonly static Weapon[] WeaponInventory = new Weapon[1];
        public int Speed;
        int? previous = null; //the previous attack used. Can't be local.

        //needs to have as many slots as the AI has phases, so they can swap weapons
        #endregion
        /// <summary>
        /// create the entity
        /// </summary>
        /// <param name="passedID">the id of the entity. aligns to HostileData</param>
        public void Generate(int passedID)
        {
            Globals.InCombat = true;
            //lock combat

            //spawning notifications
            System.Diagnostics.Debug.WriteLine("ID {0} has spawned\n", passedID);                               //the debug one
            Program.Scroll((string)HostileData[passedID, Program.rnd.Next(11, hostileWidth - 1)], scrollTime: 400); //the randomised, slow, ambient message
            Program.Scroll((string)HostileData[passedID, 10]);                                                  //the actually useful one

            ID = (int)HostileData[passedID, 0];                        //give it a name
            Name = (string)HostileData[passedID, 1];                        //give it a name
            AddWeapon((int)HostileData[passedID, 2], WeaponInventory, Name); //give it a weapon
            AddItem((int)HostileData[passedID, 3], Inventory);              //give it an item
            //instead of using a global heldweapon, just directly reference the singular inventory slot

            Stats = CompileInt((int)HostileData[passedID, 4], (int)HostileData[passedID, 5], (int)HostileData[passedID, 6], (int)HostileData[passedID, 7], (int)HostileData[passedID, 8], (int)HostileData[passedID, 9]);
            //                             health                           dodge                   physical                        posion                         bleed                               stun

            Speed = WeaponInventory[0].Speed;

            //set self as the target so it can be hit
            Globals.Target = this;
        }

        /// <summary>
        /// combat AI
        /// </summary>
        public void DecideAttack()
        {
            if(Stunned) {
                return;
            }
            /*
             * it's that time bois. Time for AI
             * best way to do this so that each entity can
             * behave as intended is a case switch based
             * on the ID. Player weapons have their attacks shuffled,
             * but AI don't (see weapon.cs line ~120)
            */
            int attackID = 0;
            int randomat = rnd.Next(1, 101);
            switch(ID) {
                case 0:                             /*Prisoner      even between 4*/
                case 1:                             /*Merchant      even between 4*/
                    attackID = rnd.Next(0, 4);
                    break;
                case 2:                             /*Skeleton      even between 4 (opens with 1)*/
                    if(previous == null) {
                            attackID = 1;
                            break;
                    }
                    goto case 1;
                case 3:                             /*The Jailer    3 with slight weight to 3rd (30|30|40)*/
                    if(randomat <= 30) {
                        attackID = 0;
                    } else if(randomat <= 60) {
                        attackID = 1;
                    } else {
                        attackID = 2;
                    }
                    break;
                case 4:                             /*Dog           2 with a heavy weight to the second (80|20)*/
                    if(randomat <= 80) {
                        attackID = 0;
                    } else {
                        attackID = 1;
                    }
                    break;
                case 5:                             /*Thrall        3 with slight weight against 3rd (40|40|20)*/
                    if(randomat <= 40) {
                        attackID = 0;
                    } else if(randomat <= 80) {
                        attackID = 1;
                    } else {
                        attackID = 2;
                    }
                    break;
                case 6:                             /*Knight        even between 3 (open with 3rd)*/
                    if(previous == null) {
                        attackID = 2;
                        break;
                    }
                    attackID = rnd.Next(0, 3);
                    if(previous == 2) {
                        goto case 6;
                    } //recur to stop this being used twice in a row
                    break;
                case 7:                             /*Priest        even between 3 (open with 4, do not repeat)*/
                    if(previous == null) {
                        attackID = 3;
                        break;
                    }
                    attackID = rnd.Next(0, 3);
                    break;
                case 8:                             /*The Gargoyle   3 with slight weight against 3rd (40|40|20)*/
                    goto case 5;
                case 9:                             /*The Entombed God      even between 2-4, use 1 last (see Death())*/
                    attackID = rnd.Next(1, 4);
                break;
                case 10:                             /*Upper Knight*/
                break;
                case 11:                             /*The Captian*/
                break;
                case 12:                             /*The King     even between 3, dont open with 3*/
                    if(previous == null){
                        attackID = rnd.Next(0, 2);
                        break;
                    }
                    attackID = rnd.Next(0, 3);
                    break;
                case 13:                             /*Bloodtinged Knight       even beween 4*/
                    goto case 1;
                case 14:                             /*The Young Drake          1-3 (40|40|20), 4 always follows 3*/
                    if (previous == 2){
                        attackID = 3;
                        break;
                    }

                    if(randomat <= 40) {
                        attackID = 0;
                    } else if(randomat <= 80) {
                        attackID = 1;
                    } else {
                        attackID = 2;
                    }
                    break;
                case 15:                             /*The Poisoned Dragon      don's open with 3, only 4 below 50%*/
                    if(Stats["Health"] > Stats["MaxHealth"] / 2)
                    {
                        goto case 12;
                    } else {
                        attackID = rnd.Next(0, 4);
                    }
                        break;
                case 16:                             /*Egg*/
                    attackID = 0;
                    break;
                case 17:                             /*Crawling Black Sludge*/
                case 18:                             /*Greater Black Sludge*/
                case 19:                             /*The Black Mass*/
                    goto case 1;
                //sludge gang
            }
            previous = attackID;
            int attack = WeaponInventory[0].attacks[attackID];
            System.Diagnostics.Debug.WriteLine("relevant ID: {0}", attack);
            //0-3 (weapon attacks) to 18-60something (real value)
            
            Attack(0, attack, Program.Player);

        }

        /// <summary>
        /// death override for custom message, drops n that
        /// </summary>
        protected override void Death()
        {
            if(ID == 9) { //the "death of a god" thing
                Attack(rnd.Next(WeaponInventory[0].DmgUp, WeaponInventory[0].DmgDwn), 0, Program.Player);
            }
            Globals.InCombat = false;
            //override for a new message
            Console.WriteLine("{0} has been vanquished", Name);
        }
    }

    /// <summary>
    /// passive merchants. becomes Hostile: EntityFramework when attacked.
    /// </summary>
    class Merchant: EntityFramework
    {
        #region instantiation
#nullable enable
        readonly Weapon[] WeaponInventory = new Weapon[4];
        //rather than this being for using like Hostile and Player, these are his merchandise

        readonly int[]? prices = new int[4];

#nullable disable //made it nullable so i can remove shit from the list without missing up the order
                  //inventory was nullable too but you cant null a struc, so we're using default(weapon)

        //int[] lootTable = new int[] { 2, 8, 10, 14, 17, 19, 25, 28, 30};
        int[] lootTable = new int[] { 0, 1, 2, 3, 4, 5, 6 };
        //the IDs of the weapons that could be for sale
        //moonlight greatsword display (7) is broken

        bool fastLeave = true;
        //flag for if the player immediatly leaves, giving an annoyed message
        #endregion

        bool active = true;

        /// <summary>
        /// put things in the WeaponInventory from the loot table
        /// </summary>
        public void Generate()
        {
            lootTable = lootTable.OrderBy(x => Program.rnd.Next()).ToArray();
            //give the lootTable a good shuffle

            for(int i = 0; i < 4; i++) {
                AddWeapon(lootTable[i], WeaponInventory, "Merchant Gen");
                prices[i] = rnd.Next(5, 20) + rnd.Next(5, 20);
            }//add the first 4 ids in the list to the inventory and shit out some prices
            // (note: AddWeapon is inherited from EntityFramework)

            if (Player.metMerchant) {
                Program.Scroll("you can hear someone muttering to themselves...\n\tA merchant presents his wares to you.");
            }
            else {
                Program.Scroll("Someone waves you over from the corner of the room.");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Program.Scroll("What, You've never met a merchant before?");
                Program.Scroll("Just trying to make a living, like the rest of us are.");
                Program.Scroll("I've got weapons, and a small smithy for repairs and upgrades if you'd be intrested.");
                if(Player.Inventory[0, 1] == 1) {
                    Program.Scroll("Is that a map you've got? How about I mark where my brothers of the trade are for you?");
                    Console.ForegroundColor = ConsoleColor.White;
                    Program.Scroll("[Accept Marking?]", 0, 0, 2, 2);
                    Program.Scroll("[Y / N]", 0, 0, 1, 2);
                    Console.Write("> ");
                    string ans = Console.ReadLine();
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    if(ans == "Y" || ans == "y") //input validation.
                    {
                        //script for marking merchants on the map
                        Console.ForegroundColor = ConsoleColor.White;
                        Map.ShowMerchants();
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Program.Scroll("No problem.");

                    } else {
                        Program.Scroll("Suit yourself.");
                    }
                     Program.Scroll("Have a look at my wares, won't you?");
                }

                Console.ForegroundColor = ConsoleColor.White;
                Player.metMerchant = true;
            }
            PurchaseLoop();
        }

        /// <summary>
        /// infinite loop of interacting with the npc until you attack him or leave
        /// </summary>
        void PurchaseLoop()
        {
            while (active){
                Console.ForegroundColor = ConsoleColor.Yellow;
                Program.Scroll("Your Funds:" + Player.Funds, lineBreak:2);
                Console.ForegroundColor = ConsoleColor.White;
                int choice = Selection();
                switch(choice){
                    case 1:                                         /*view*/
                        for(int i = 0; i < 4; i++) {
                            if(!WeaponInventory[i].Equals(default(Weapon))){
                                WeaponInterpreter.Display(WeaponInventory[i], true);
                            }
                        }
                    break;
                    case 6:                                         /*repair*/
                        int Rcost = 10;
                        int affordable = (int)Math.Floor((double)(Player.Funds/Rcost));
                        int amount = affordable++;
                        int max = (int)WeaponInterpreter.WeaponData[Globals.HeldWeapon.id, 5];
                        WeaponInterpreter.Display(Globals.HeldWeapon);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Program.Scroll("That's "+ Rcost+" gold a point.");
                        if(max == Globals.HeldWeapon.Durability) {
                            Program.Scroll("Not like your weapon seems like it needs that, though.");
                        }
                        else {
                            Console.ForegroundColor = ConsoleColor.White;
                            int available = max - Globals.HeldWeapon.Durability;
                            do {
                                Program.Scroll("[Maximum: " + available + ". Affordable: "+affordable+". 'back' to return]", lineBreak: 2);

                                Console.Write("> ");
                                string ans = Console.ReadLine();
                                Console.WriteLine();
                                if(Int32.TryParse(ans, out _)) {
                                    amount = int.Parse(ans);
                                }
                                else {
                                    PurchaseLoop();
                                    //return will end the interaction, break will end the do while. need a full recur.
                                }
                            }while(amount >= affordable & amount > 0 & amount <= max);
                            Globals.HeldWeapon.Durability += amount;
                            Player.Funds -= Rcost*amount;
                        }
                    break;
                    case 7:                                         /*upgrade*/
                        WeaponInterpreter.Display(Globals.HeldWeapon);
                        Weapon upgrade = WeaponInterpreter.UpgradeWeapon(Globals.HeldWeapon);

                        int Ucost = 100; //make this a sensible calculation
                        Program.Scroll("Your Funds:" + Player.Funds + "\t Cost: "+ Ucost, lineBreak:2);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        if (Ucost > Player.Funds) {
                            Program.Scroll("That looks a litle pricey for you");
                        }
                        else {
                            Program.Scroll("You're certian?");
                            Console.ForegroundColor = ConsoleColor.White;
                            Program.Scroll("[Y / N]", 0, 0, 2, 2);
                            Console.Write("> ");
                            string ans = Console.ReadLine();
                            Console.WriteLine();
                            if(ans == "Y" || ans == "y") {
                                Program.Scroll("Pleasure doing business");
                                Globals.HeldWeapon = upgrade;
                                Player.Funds -= Ucost;
                            }
                            else {
                                Program.Scroll("No worries.");
                            }
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    break;
                    case 8:                                         /*attack*/
                        Hostile merchant = new Hostile();
                        merchant.Generate(0);
                    return;
                    case 9:                                         /*leave*/
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        if (fastLeave) {
                        Program.Scroll("Nothing you like? You could look a little harder...");
                        } else {
                            Program.Scroll("Do come again...");
                        }
                        active = false;
                        Console.ForegroundColor = ConsoleColor.White;
                    return;
                    default:                                         /*input 2-5, purchase 1-4*/
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        if(!WeaponInventory[choice - 2].Equals(default(Weapon))){
                            if (Player.Funds >= prices[choice - 2])
                            {
                                AddWeapon(0, Player.WeaponInventory, "merchant purchase", true, WeaponInventory[choice - 2]);
                                WeaponInventory[choice - 2] = default;
                                Player.Funds -= prices[choice - 2];
                            }
                            else
                            {
                                Program.Scroll("That looks a litle pricey for you");
                            }
                        }
                        else {
                            Program.Scroll("You've already bought that");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    break;
                }
            }
        }

        int Selection()
        {
            string choice; //for the string input, then confimation
            int intChoice; //for the parsed input
            bool notnull = false;

            do { //range check, null check and confirmation
                do { //putting in a valid raw option
                    Console.WriteLine("\t1) Inspect the merchandise");
                    for(int i = 0; i < 4; i++) {
                        if(!(WeaponInventory[i].Equals(default(Weapon)))) {
                            Console.WriteLine("\t{0}) Purchase {1, -20}\t{2, -10}", i + 2, WeaponInventory[i].name, "price: " + prices[i].ToString());
                        } else {
                            Console.WriteLine("\t{0}) ###  Sold Out  ###", i + 2);
                        }
                    }
                    Console.Write("\t6) Repair your weapon\n\t7) Upgrade your weapon\n\t8) Attack the merchant\n\t9) Exit\n\n> ");
                    choice = Console.ReadLine();

                } while(!Int32.TryParse(choice, out _)); //type check. presense not needed because " " can't be string
                intChoice = int.Parse(choice);

                if(intChoice > 1 && intChoice < 6) {//check if that index of weapon is available, if its 2-5
                    notnull = !WeaponInventory[intChoice-2].Equals(default(Weapon));
                }
                if((intChoice > 1 && intChoice < 6) || intChoice == 8) { //confimation for 2-5, or 8
                    Program.Scroll("You're certian?");
                    Program.Scroll("[Y / N]", 0, 0, 2, 2);
                    Console.Write("> ");
                    choice = Console.ReadLine();
                } else {
                    choice = "y";
                }

                if(choice == "N" || choice == "n") {
                    return -1;
                } //return a vaule that will dodge the switch case, and loop round to reprinting the menu

            } while((intChoice < 1 || intChoice > 7) && (choice == "Y" || choice == "y") && notnull); //range check, null check and confirmation
            
            if(intChoice > 1 && intChoice < 6) {
                fastLeave = false;
            }

            return intChoice;
        }
    }

    /// <summary>
    /// You can't index through dictionarys or keycollections so I just made one myself
    /// </summary>
    public class StatDictionary : Dictionary<string, int>
    {

        public int Indexer(int index)
        {
            switch(index){
                case 0:
                    return this["MaxHealth"];
                case 1:
                    return this["Dodge"];
                case 2:
                    return this["Protection"];
                case 3:
                    return this["Poison"];
                case 4:
                    return this["Bleed"];
                case 5:
                    return this["Stun"];
                default:
                    throw new InvalidOperationException("Index not found.\n Possible incorrect assignment of this type of dict?");
            }
        }

        public string IndexKeys(int index)
        {
            switch(index){
                case 0:
                    return "MaxHealth";
                case 1:
                    return "Dodge";
                case 2:
                    return "Protection";
                case 3:
                    return "Poison";
                case 4:
                    return "Bleed";
                case 5:
                    return "Stun";
                default:
                    throw new InvalidOperationException("Index not found.\n Possible incorrect assignment of this type of dict?");
            }
        }
    }
}