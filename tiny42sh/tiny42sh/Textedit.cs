using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace cmd_Linux
{
    public static class Textedit
    {
        static public int execute_textedit(string[] cmd)
        {
            List<string> content = new List<string>();
            string line = "";
            
            if(cmd.Length == 1)
            {

                Console.Write("> ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("textedit: Write you text, '/save' in empty line to save it");
                Console.ResetColor();
                Console.Write("> ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("or use /leave to leave textedit.");
                Console.ResetColor();
                while (line != "/save" && line != "/leave")
                {
                    Console.Write("> ");
                    line = Console.ReadLine();
                    if (line != "/save" && line != "/leave")
                    {
                        content.Add(line);
                    }
                }

                if (line == "/leave")
                {
                    return (0);
                }
                else
                {
                    do
                    {
                        Console.Write("> ");
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write("textedit: Enter the name of your text file (/leave to cancel): ");
                        Console.ResetColor();
                        line = Console.ReadLine();
                        if (!Execution.is_valid_name(line) && line != "/leave")
                        {
                            Console.Write("> ");
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine("textedit: " + line + ": Invalid file name!");
                            Console.ResetColor();
                        }
                    } while (!Execution.is_valid_name(line) && line != "/leave");

                    if (line != "/leave")
                    {
                        return (save_file(line, content));
                    }
                }
            }
            else if (cmd.Length == 2)
            {
                Console.Write("> ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("textedit: Write you text, '/save' in empty line to save it");
                Console.ResetColor();
                Console.Write("> ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("or use /leave to leave textedit.");
                Console.ResetColor();
                if (File.Exists(cmd[1]))
                {
                    content = extract_file_content(cmd[1]);
                }

                for (int j = 0; j < content.Count; j++)
                {
                    Console.WriteLine("> " + content[j]);
                }

                while (line != "/save" && line != "/leave")
                {
                    Console.Write("> ");
                    line = Console.ReadLine();
                    if (line != "/save" && line != "/leave")
                    {
                        content.Add(line);
                    }
                }

                if (line != "/leave" && content.Count > 0)
                {
                    return (save_file(cmd[1], content));
                }
            }
            else
            {
                Console.WriteLine("> textedit: Invalid number of argument");
                return (1);
            }

            return (0);
        }

        static private int save_file(string path, List<string> content)
        {
            Console.Write("> ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            try
            {
                StreamWriter saver = new StreamWriter(path);

                for(int i = 0; i < content.Count; i++)
                {
                    saver.WriteLine(content[i]);
                }

                saver.Close();
                Console.WriteLine("textedit: " + path + ": saved.");
                Console.ResetColor();
                return (0);
            }
            catch(Exception)
            {
                Console.WriteLine("textedit: " + Execution.extract_shorter_path(Directory.GetCurrentDirectory()) + ": access denied! (execute it with admin rights)");
                Console.ResetColor();
                return (1);
            }
        }

        static private List<string> extract_file_content(string file)
        {
            List<string> file_content = new List<string>();

            if(File.Exists(file))
            {
                    try
                    {
                        StreamReader extractor = new StreamReader(file);
                        string line = extractor.ReadLine();

                        while (line != null)
                        {
                            file_content.Add(line);
                            line = extractor.ReadLine();
                        }

                        extractor.Close();
                    }
                    catch(Exception)
                    {
                        Console.WriteLine("> textedit: " + Execution.extract_shorter_path(file) + ": access denied! (execute it with admin rights)");
                    }
            }
            else if(Directory.Exists(file))
            {
                Console.WriteLine("> textedit: " + Execution.extract_shorter_path(file) + ": Is a directory");
            }
            else
            {
                Console.WriteLine("> textedit: " + Execution.extract_shorter_path(file) + ": No such file or directory");
            }

            return (file_content);
        }
    }
}
