using Paarl.Utility;

namespace Paarl
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Print.WriteInit("Paarl v1.0.0");
            Print.WriteInit("Auto LODs all the models in your GDT");
            Print.WriteWarning("Please note, this tool is intended for mass setting up your models. Manually checking and editing your models will still be required");
            Print.WriteInit("To use, drag and drop any GDT file onto Paarl's executable");
            Print.WriteInit("Created by Rayjiun");
            Console.WriteLine();

            if(args.Length == 0)
            {
                Print.WriteWarning("No file was given.");
            }

            foreach (var file in args)
            {
                var GDT = Path.GetFileName(file);
                if(!GDT.Contains(".gdt"))
                {
                    Print.WriteWarning($"{file} is not a .gdt");
                    continue;
                }

                Print.WriteLog($"Parsing {GDT}");
                LOD.Parse(file);
            }

            Console.WriteLine();
            Print.WriteQuestion("Please enter any key to exit Paarl");
            Console.ReadKey();
        }
    }
}
