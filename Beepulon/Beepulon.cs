using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Beepulon
{

    class Beepulon
    {
        //The purpose of beepulon is to automatically change data in a way that i cannot be bothered to do manually.
        //As this is a devtool, I've made it in a seperate project.
        protected static Random rnd = new Random();
        static int length = 1000;
        static int width = 1000;
        //these values arent local to readfile anymore so we can use them ~elsewhere~ (mainly in for loops)

        static object[,] Data = ReadFile("AttackData.txt");
        //[0]>50, 0 < [5] < 30, rnd (2->7)*2
        //[2]<100, rnd (38->50)*2
        //[3] = cieling(log[0])
        //[4] = cieling(cieling(log[0])*1.2)

        static void Main()
        {
            #region making a new one
            string fileName = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString()).ToString()).ToString() + @"\ConsoleApp2\BeepulonOutput.txt";
            Console.WriteLine(fileName);
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            Console.WriteLine(fileName);
            #endregion

            #region writing the shit
            using (StreamWriter file = new StreamWriter(fileName))
            {
                int[] notHit = new int[] { 74,  78, 82, 71, 69, 70, 63, 66, 61, 62, 58, 52, 54};

                for(int y = 0; y < length; y++) {
                    file.Write("");
                    int id = (int)Data[y,0];
                    for(int x = 0; x < width; x++) {
                        #region processing
                        switch (x) {
                            case 5:
                                if(id>50 && (int)Data[y,5]>0 && (int)Data[y, 5] < 30) {
                                    Data[y,5] = rnd.Next(2, 7) + rnd.Next(2, 7);
                                }
                            break;
                            case 2:
                                if((int)Data[y,2]<100) {
                                    Data[y,2] = rnd.Next(38, 50) + rnd.Next(38, 50);
                                }
                            break;
                            case 3:
                                if (id > 51 & !notHit.Contains(id)) {
                                    Data[y,3] = Math.Ceiling(Math.Log((double)Data[y,0]));
                                }
                            break;
                            case 4:
                                if (id > 51 & !notHit.Contains(id)) {
                                Data[y,4] = Math.Ceiling((Math.Log((double)Data[y,0]))*1.2);
                                }
                            break;
                        }
                        //intList.IndexOf(intVariable) != -1;

                        #endregion
                        file.Write(Data[y, x]);
                        if(x < width-1) {
                            file.Write("|");
                        }
                    }
                    file.Write("\n");
                }
            }
            Console.WriteLine("Finished.");
            #endregion

            

        }

        static object[,] ReadFile(string location1)
        {
            Console.WriteLine(Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString()).ToString()).ToString() + @"\ConsoleApp2\" + location1);
            #region finding the file
            string[] File;
            try
            {

                File = System.IO.File.ReadAllLines(Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString()).ToString()).ToString() + @"\ConsoleApp2\" + location1);
                //this has been modified from the one in main consoleapp so it can navigate out of /beepulon/ and into /consoleapp/

                System.Diagnostics.Debug.WriteLine(location1, " found");
                //get the name of the file in question for the debug
            }
            catch
            {
                Console.WriteLine("File {0} not found", location1);
                File = new string[] { "" };
                System.Threading.Thread.Sleep(4000);
                System.Environment.Exit(1);
            }
            #endregion

            #region finding EOF
            //finding the vertical EOF
            for (int i = 0; i < 1000; i++) //iterate through each line of the file
            {
                if (File[i] == "%EOF%")
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

            for (int i = 0; i < length; i++) //iterate through each line of the file
            {
                hold = File[i]; //the hold string is that line
                Hold = hold.Split('|'); //the individual file data is seperated by /

                //this gets those into their own lists
                for (int i2 = 0; i2 < width; i2++) //iterate through that line
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
}
