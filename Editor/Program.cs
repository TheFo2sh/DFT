using System;
using System.IO;

namespace Editor
{
    class Program
    {
        static void Main(string[] args)
        {
           var streamReader=new StreamReader("C:\\Users\\UWP\\Desktop\\twitter.csv");
           var streamWriter = new StreamWriter("C:\\Users\\UWP\\Desktop\\twitter.json");
            string line;
           while ( (line=streamReader.ReadLine())!=null)
           {
               line = "new Point {x =" + line.Replace(",", ",y=") + "},";
               streamWriter.WriteLine(line);
               Console.WriteLine(line);
           }
           streamWriter.Flush();
           streamWriter.Close();
           streamReader.Close();
        }
    }
}
