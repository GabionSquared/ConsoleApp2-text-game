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


        static object[,] Data = Program.ReadFile("AttackData.txt");

        static void Main(string[] args)
        {
            string fileName = Path.Combine(Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString(), "BeepulonOutput.txt"));
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            Console.WriteLine(fileName);
            using FileStream fs = File.Create(fileName);

            for (int i = 0; i > Data.Length; i++)
            {
                Data[i, 0] = i;
            }

            // Create a new file     
            fs.Write(Data);

        }

        public static object[,] ReadFile(string location1)
        {

            #region finding the file
            string[] File;
            try
            {
                File = System.IO.File.ReadAllLines(Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString(), location1));
                //BASICALLY, GetCurrentDirectory() gives "C:\ [yadda yadda] \ConsoleApp2\bin\Debug". this get the parent twice to go to \ConsoleApp2,
                //then grafts on the actual file name from the function call to find the file. I hate it too.

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
            int length = 1000;
            int width = 1000;
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
