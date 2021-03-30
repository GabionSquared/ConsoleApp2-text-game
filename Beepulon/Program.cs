using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Beepulon
{
    class Program
    {
        //The purpose of beepulon is to automatically change data in a way that i cannot be bothered to do manually.
        //As this is a devtool, I've made it in a seperate project.

        static int length = 1000;
        static int width = 1000;
        //these values arent local to readfile anymore so we can use them ~elsewhere~ (mainly in for loops)

        static object[,] Data = Program.ReadFile("AttackData.txt");

        static void Main(string[] args)
        {
            #region making a new one
            string fileName = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString()).ToString()).ToString() + @"\ConsoleApp2\BeepulonOutput.txt";
            if(File.Exists(fileName)) {
                File.Delete(fileName);
            }
            Console.WriteLine(fileName);
            #endregion

            for(int i = 0; i < length; i++) {
                Console.Write("was: {0}", Data[i, 0]);
                Data[i, 0] = i;
                Console.WriteLine("    is:{0} ", Data[i, 0]);
            }

            #region writing the shit
            using(StreamWriter file = new StreamWriter(fileName)) {
                Console.WriteLine("ree");
                for(int y = 0; y < length; y++) {
                    for(int x = 0; x < width; x++) {
                        file.Write(Data[y, x]);
                        if(x < width-1) {
                            file.Write("|");
                        }
                    }
                    file.Write("\n");
                }
            }
            #endregion
        }

        public static object[,] ReadFile(string location1)
        {

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
