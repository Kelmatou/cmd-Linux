using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualBasic.Devices;
using System.Threading.Tasks;

namespace cmd_Linux
{
    public static class Textedit
    {
        static public int execute_textedit(string[] cmd, string appdata_dir, string language = "en")
        {
            List<string> content = new List<string>();
            int lineNumber = 0;
            int argNumber = cmd.Length;
            string fileName = "";
            if(cmd.Length > 1 && cmd[1].Length == 3 && cmd[1][0] == '/')
            {
                argNumber--;
                if (isSupportedLanguage(cmd[1].Substring(1)))
                    language = cmd[1].Substring(1);
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("> " + cmd[0] + ": " + cmd[1].Substring(1) + ": is not a supported language.");
                    Console.WriteLine("> english (/en) was set by default.");
                    Console.ResetColor();
                }
            }
            List<Genius_data> dictionary = loadDictionary(language, appdata_dir); //all suggested words
            if (cmd.Length > 2)
                fileName = cmd[2];
            else if(cmd.Length == 2)
                fileName = cmd[1];
            
            if(argNumber == 1)
            {
                if (!get_text(ref content, ref dictionary, appdata_dir, lineNumber, language))
                    return (0);
                else
                {
                    do
                    {
                        Console.Write("> ");
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write(cmd[0] + ": Enter the name of your text file (/leave to cancel): ");
                        Console.ResetColor();
                        fileName = Console.ReadLine();
                        if (!Execution.is_valid_name(fileName) && fileName != "/leave")
                        {
                            Console.Write("> ");
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine(cmd[0] + ": " + fileName + ": Invalid file name!");
                            Console.ResetColor();
                        }
                    } while (!Execution.is_valid_name(fileName) && fileName != "/leave");

                    if (fileName != "/leave")
                        return (save_file(fileName, content, appdata_dir, ref dictionary, language));
                }
            }
            else if (argNumber == 2)
            {
                if (!Execution.is_valid_name(fileName))
                {
                    Console.WriteLine("> " + cmd[0] + ": " + fileName + ": Invalid name");
                    return (1);
                }

                if (File.Exists(fileName))
                {
                    content = extract_file_content(fileName);
                    for (int j = 0; j < content.Count; j++)
                    {
                        Console.Write("> " + content[j]);
                        if (j < content.Count - 1)
                            Console.WriteLine();
                    }
                    lineNumber = (content.Count == 0 ? 0 : content.Count - 1);
                }

                if (get_text(ref content, ref dictionary, appdata_dir, lineNumber, language, fileName) && content.Count > 0)
                    return (save_file(fileName, content, appdata_dir, ref dictionary, language));
            }
            else
            {
                Console.WriteLine("> textedit: Invalid number of argument");
                return (1);
            }

            return (0);
        }

        static private int save_file(string path, List<string> content, string appdata_dir, ref List<Genius_data> dictionary, string language = "en", bool silent = false)
        {
            updateDictionary(appdata_dir, content, ref dictionary, language);
            if(!silent)
                Console.Write("> ");
            if(Execution.is_valid_name(Library.extractShorterPath(path)))
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                try
                {
                    StreamWriter saver = new StreamWriter(path);

                    for (int i = 0; i < content.Count; i++)
                        saver.WriteLine(content[i]);

                    saver.Close();
                    if(!silent)
                        Console.WriteLine("textedit: " + path + ": saved.");
                    Console.ResetColor();
                    return (0);
                }
                catch (Exception)
                {
                    if(!silent)
                        Console.WriteLine("textedit: " + Library.extractShorterPath(Directory.GetCurrentDirectory()) + ": access denied! (execute it with admin rights)");
                    Console.ResetColor();
                    return (1);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                if (!silent)
                    Console.WriteLine("textedit: " + Library.extractShorterPath(path) + ": Invalid name");
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
                        Console.WriteLine("> textedit: " + Library.extractShorterPath(file) + ": access denied! (execute it with admin rights)");
                    }
            }
            else if(Directory.Exists(file))
            {
                Console.WriteLine("> textedit: " + Library.extractShorterPath(file) + ": Is a directory");
            }
            else
            {
                Console.WriteLine("> textedit: " + Library.extractShorterPath(file) + ": No such file or directory");
            }

            return (file_content);
        }

        static bool get_text(ref List<string> content, ref List<Genius_data> dictionary, string appdata_dir, int lineNumber, string language = "en", string fileName = "")
        {
            int windowWidth = Console.WindowWidth; //window size must be constant
            if (Console.CursorTop == Console.BufferHeight - 1)
            {
                if (Console.BufferHeight >= 32700)
                    Console.Clear();
                else
                    Console.SetBufferSize(Console.BufferWidth, Console.BufferHeight + 300);
            }
            if(Console.CursorLeft == 0)
                Console.Write("> ");
            if (content.Count == 0)
                content.Add("");

            ConsoleKeyInfo key_input = new ConsoleKeyInfo();
            int line_beginning = 2; //remembers first left position on current line
            int line = Console.CursorTop - ((content[lineNumber].Length + 2) / Console.WindowWidth); //remembers first top position on current line
            int max_cursor = Console.CursorTop; //remember max top position reached
            int input_pointer = content[lineNumber].Length; //remember index in current string input
            int delta_input = 0; //used for optimisation in input
            string genius = ""; //used for genius suggestions
            string last_word = ""; //last word in current string

            while (key_input.Modifiers != ConsoleModifiers.Control || (key_input.Key != ConsoleKey.X && key_input.Key != ConsoleKey.Q))
            {
                key_input = Console.ReadKey(true);
                if (Console.WindowWidth != windowWidth)
                    Console.WindowWidth = windowWidth;
                if(key_input.Modifiers != ConsoleModifiers.Control || (key_input.Key != ConsoleKey.X && key_input.Key != ConsoleKey.Q && key_input.Key != ConsoleKey.W))
                {
                    string curText = content[lineNumber];
                    delta_input = update_input(ref curText, key_input, ref input_pointer, ref line, dictionary, appdata_dir, ref max_cursor, ref genius, ref lineNumber, ref line_beginning, ref content);
                    content[lineNumber] = curText;
                    if (delta_input != 0)
                    {
                        Program.erase_genius_trace(genius, content[lineNumber].Length - input_pointer);
                        last_word = Program.get_last_word(content[lineNumber]);
                        display_update(content[lineNumber], line_beginning, line, input_pointer, delta_input);
                        if (key_input.Key != ConsoleKey.Enter && (genius.Length <= last_word.Length || genius.Substring(0, last_word.Length) != last_word) && content[lineNumber].Length > 0)
                            genius = Interpreter.genius_cmd(last_word, -1, dictionary, content[lineNumber], appdata_dir, new List<Link>());
                        else if (last_word == "")
                            genius = "";
                        if (key_input.Key != ConsoleKey.Enter && (content[lineNumber].Length > 0 && content[lineNumber][content[lineNumber].Length - 1] != '"'))
                            Interpreter.print_genius_cmd(genius, last_word, ref max_cursor);
                        else
                            max_cursor = Console.CursorTop;
                        Program.cursor_replacement(content[lineNumber].Length - input_pointer);
                    }
                }
                if (key_input.Modifiers == ConsoleModifiers.Control && key_input.Key == ConsoleKey.W && fileName != "")
                    save_file(fileName, content, appdata_dir, ref dictionary, language, true);
            }
            Console.SetCursorPosition(0, max_cursor + 1);

            return (key_input.Key == ConsoleKey.X);
        }

        static int update_input(ref string command, ConsoleKeyInfo key_input, ref int input_pointer, ref int line, List<Genius_data> dictionary, string appdata_dir, ref int max_cursor, ref string genius, ref int lineNumber, ref int lineBegin, ref List<string> content)
        {
            Computer mycomputer = new Computer();
            input_pointer++;
            int delta = command.Length;
            string genius_linux = "";

            switch (key_input.Key)
            {
                case (ConsoleKey.Backspace):
                    if (command.Length > 0)
                    {
                        Program.erase_genius_trace(genius, command.Length - input_pointer);
                        if (key_input.Modifiers == ConsoleModifiers.Control)
                        {
                            if (command.Substring(0, command.Length - Program.get_last_word_with_quotes(command).Length).Length < input_pointer)
                                input_pointer = command.Substring(0, command.Length - Program.get_last_word_with_quotes(command).Length).Length + 1;
                            command = command.Substring(0, command.Length - Program.get_last_word_with_quotes(command).Length);
                            if (input_pointer < 1)
                                input_pointer = 1;
                        }
                        else if (input_pointer == 1)
                        {
                            if (lineNumber > 0)
                                removeLineBackspace(ref command, ref content, ref input_pointer, ref lineNumber, ref line, ref max_cursor);
                            else
                                input_pointer--;
                            return (0);
                        }
                        else
                        {
                            if (input_pointer == command.Length + 1)
                            {
                                command = command.Substring(0, command.Length - 1);
                                input_pointer--;
                            }
                            else
                            {
                                if (input_pointer >= 2)
                                {
                                    command = command.Substring(0, input_pointer - 2) + command.Substring(input_pointer - 1, command.Length - input_pointer + 1);
                                    input_pointer--;
                                }
                            }
                        }
                        if (command.Length == 0)
                            genius = "";
                    }
                    else
                    {
                        if (lineNumber > 0)
                            removeLineBackspace(ref command, ref content, ref input_pointer, ref lineNumber, ref line, ref max_cursor);
                        else
                            input_pointer--;
                        return (0);
                    }
                    input_pointer--;
                    break;
                case (ConsoleKey.Tab):
                    if (key_input.Modifiers == ConsoleModifiers.Control)
                    {
                        genius_linux = Interpreter.genius_linux(Program.get_last_word(command), Interpreter.get_arg_expected_code(command), dictionary, command, appdata_dir, new List<Link>());
                        if (genius_linux.Length > 0 && genius_linux != Program.get_last_word(command))
                        {
                            command = command.Substring(0, command.Length - Program.get_last_word(command).Length) + genius_linux;
                            input_pointer = command.Length;
                        }
                        else
                        {
                            input_pointer--;
                            return (0);
                        }
                    }
                    else
                    {
                        if (genius.Length > 0 && genius != Program.get_last_word(command))
                        {
                            genius += " ";
                            command = command.Substring(0, command.Length - Program.get_last_word(command).Length) + genius;
                            genius = "";
                            input_pointer = command.Length;
                        }
                        else
                        {
                            input_pointer--;
                            return (0);
                        }
                    }
                    break;
                case (ConsoleKey.Enter):
                    Program.erase_genius_trace(genius, command.Length - input_pointer);
                    createLineSpace(ref command, ref content, ref input_pointer, ref lineNumber, ref line, ref max_cursor, ref lineBegin, ref genius);
                    return (0);
                case (ConsoleKey.A):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.B):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.C):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.D):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.E):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.F):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.G):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.H):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.I):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.J):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.K):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.L):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.M):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.N):
                    if (key_input.Modifiers == ConsoleModifiers.Control)
                    {
                        Execution.execute_input(new string[1][] { new string[1] { "launch" } });
                        input_pointer--;
                        return (0);
                    }
                    else
                    {
                        Program.add_key_to_command(ref command, key_input, input_pointer);
                    }
                    break;
                case (ConsoleKey.O):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.P):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Q):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.R):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.S):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.T):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.U):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.V):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.W):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.X):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Y):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Z):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad0):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.D0):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad1):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.D1):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad2):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.D2):
                    if (key_input.KeyChar == '\0')
                    {
                        input_pointer--; //~ s'affiche uniquement sur le prochain caractère
                        return (0);
                    }
                    else
                        Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad3):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.D3):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad4):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.D4):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad5):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.D5):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad6):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.D6):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad7):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.D7):
                    if (key_input.KeyChar == '\0')
                    {
                        input_pointer--; //` s'affiche uniquement sur le prochain caractère
                        return (0);
                    }
                    else
                        Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad8):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.D8):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad9):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.D9):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Add):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Multiply):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Subtract):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Divide):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Spacebar):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.LeftArrow):
                    if (input_pointer > 1)
                    {
                        input_pointer -= 2;
                        Program.cursor_replacement(1);
                    }
                    else if (input_pointer == 1)
                        input_pointer = 0;
                    return (0);
                case (ConsoleKey.RightArrow):
                    if (input_pointer > command.Length)
                        input_pointer--;
                    else
                        Program.cursor_replacement(-1);
                    return (0);
                case (ConsoleKey.UpArrow):
                    input_pointer--;
                    if (lineNumber > 0 || input_pointer >= Console.WindowWidth - 2)
                    {
                        Program.erase_genius_trace(genius, command.Length - input_pointer);
                        if(input_pointer >= Console.WindowWidth - 2)
                        {
                            if (input_pointer < Console.WindowWidth * 2 - 2 && Console.CursorLeft < 2)
                            {
                                Console.CursorLeft = 2;
                                input_pointer = 0;
                            }
                            else
                                input_pointer -= Console.WindowWidth;
                        }
                        else
                        {
                            lineNumber--;
                            command = content[lineNumber];
                            line = line - 1 - ((command.Length + 2) / Console.WindowWidth);
                            if ((command.Length + 2) % Console.WindowWidth < Console.CursorLeft)
                            {
                                input_pointer = command.Length;
                                Console.CursorLeft = (input_pointer + 2) % Console.WindowWidth;
                            }
                            else
                                input_pointer = command.Length - (command.Length + 2) % Console.WindowWidth + Console.CursorLeft;
                        }
                        Console.CursorTop--;
                        genius = "";
                    }
                    return (0);
                case (ConsoleKey.DownArrow):
                    input_pointer--;
                    if (lineNumber < content.Count - 1 || (int)((input_pointer + 2) / Console.WindowWidth) < (int)((content[lineNumber].Length + 1) / Console.WindowWidth))
                    {
                        Program.erase_genius_trace(genius, command.Length - input_pointer);
                        if((int)((input_pointer + 2) / Console.WindowWidth) < (int)((content[lineNumber].Length + 1) / Console.WindowWidth))
                        {
                            if (input_pointer + Console.WindowWidth > content[lineNumber].Length)
                            {
                                Console.CursorLeft = (content[lineNumber].Length + 2) % Console.WindowWidth;
                                input_pointer = content[lineNumber].Length;
                            }
                            else
                                input_pointer += Console.WindowWidth;
                        }
                        else
                        {
                            lineNumber++;
                            command = content[lineNumber];
                            line = Console.CursorTop + 1;
                            if (Console.CursorLeft < 2)
                            {
                                Console.CursorLeft = 2;
                                input_pointer = 0;
                            }
                            else if (Console.CursorLeft > command.Length + 2)
                            {
                                Console.CursorLeft = command.Length + 2;
                                input_pointer = command.Length;
                            }
                            else
                                input_pointer = Console.CursorLeft - 2;
                        }
                        Console.CursorTop++;
                        genius = "";
                    }
                    return (0);
                case (ConsoleKey.Escape):
                    Program.erase_genius_trace(genius, command.Length - input_pointer);
                    input_pointer = 0;
                    if (command != "")
                    {
                        command = "";
                        genius = "";
                    }
                    else
                        return (0);
                    break;
                case (ConsoleKey.Delete)://here
                    if (input_pointer <= command.Length)
                    {
                        if (input_pointer == command.Length)
                            command = command.Substring(0, command.Length - 1);
                        else
                            command = command.Substring(0, input_pointer - 1) + command.Substring(input_pointer, command.Length - input_pointer);
                        if (command.Length == 0)
                        {
                            Program.erase_genius_trace(genius, command.Length - input_pointer);
                            genius = "";
                        }
                    }
                    else
                    {
                        removeLineDelete(ref command, ref content, ref input_pointer, ref lineNumber, ref max_cursor);
                        return (0);
                    }
                    input_pointer--;
                    break;
                case (ConsoleKey.Insert): //devrait faire un coller --'
                    if (input_pointer <= command.Length)
                        command = command.Substring(0, input_pointer - 1) + System.Windows.Forms.Clipboard.GetText() + command.Substring(input_pointer - 1, command.Length - input_pointer + 1);
                    else
                        command = command + System.Windows.Forms.Clipboard.GetText();
                    input_pointer = command.Length;
                    break;
                case (ConsoleKey.Decimal):
                    if (mycomputer.Keyboard.CapsLock || mycomputer.Keyboard.ShiftKeyDown)
                    {
                        if (input_pointer <= command.Length)
                        {
                            if (input_pointer == command.Length)
                                command = command.Substring(0, command.Length - 1);
                            else
                                command = command.Substring(0, input_pointer - 1) + command.Substring(input_pointer, command.Length - input_pointer);
                            if (command.Length == 0)
                            {
                                Program.erase_genius_trace(genius, command.Length - input_pointer);
                                genius = "";
                            }
                        }
                        else
                        {
                            removeLineDelete(ref command, ref content, ref input_pointer, ref lineNumber, ref max_cursor);
                            return (0);
                        }
                        input_pointer--;
                    }
                    else
                    {
                        Program.add_key_to_command(ref command, key_input, input_pointer);
                        break;
                    }
                    break;
                case (ConsoleKey.OemComma):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.OemPeriod):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.OemPlus):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.OemMinus): //- ou _ sur QWERTY
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem1):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem2):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem3):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem4):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem5):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem6):
                    if (key_input.KeyChar == '\0')
                    {
                        input_pointer--; //^ ¨ s'affichent uniquement sur le prochain caractère
                        return (0);
                    }
                    else
                        Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem7):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem8):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem102):
                    Program.add_key_to_command(ref command, key_input, input_pointer);
                    break;
                default:
                    input_pointer--;
                    break;
            }

            return (command.Length - delta);
        }

        static void display_update(string command, int line_beginning, int line, int input_pointer, int delta_input)
        {
            Console.CursorVisible = false;
            Console.SetCursorPosition(line_beginning, line);
            if (delta_input > 0)
                Program.display_update_adding(command, input_pointer, delta_input);
            else
                Program.display_update_erase(command, input_pointer, delta_input);
            Console.CursorVisible = true;
        }

        static List<Genius_data> loadDictionary(string language, string appdata_dir)
        {
            List<Genius_data> dictionary = new List<Genius_data>();
            List<Genius_data> personalDictionary = new List<Genius_data>();

            if(Directory.Exists(appdata_dir + "dictionary"))
            {
                if (!Directory.Exists(appdata_dir + "dictionary/personal"))
                    Directory.CreateDirectory(appdata_dir + "dictionary/personal");
                switch (language)
                {
                    case ("en"):
                        if (File.Exists(appdata_dir + "dictionary/en"))
                            dictionary = convertDictionaryToGenius(Library.getFileContent(appdata_dir + "dictionary/en"));
                        if (File.Exists(appdata_dir + "dictionary/personal/en"))
                            personalDictionary = convertDictionaryToGenius(Library.getFileContent(appdata_dir + "dictionary/personal/en"));
                        break;
                    case ("fr"):
                        if (File.Exists(appdata_dir + "dictionary/fr"))
                            dictionary = convertDictionaryToGenius(Library.getFileContent(appdata_dir + "dictionary/fr"));
                        if (File.Exists(appdata_dir + "dictionary/personal/fr"))
                            personalDictionary = convertDictionaryToGenius(Library.getFileContent(appdata_dir + "dictionary/personal/fr"));
                        break;
                    default:
                        if (File.Exists(appdata_dir + "dictionary/en"))
                            dictionary = convertDictionaryToGenius(Library.getFileContent(appdata_dir + "dictionary/en"));
                        if (File.Exists(appdata_dir + "dictionary/personal/en"))
                            personalDictionary = convertDictionaryToGenius(Library.getFileContent(appdata_dir + "dictionary/personal/en"));
                        break;
                }

                for (int i = personalDictionary.Count - 1; i >= 0; i--)
                    dictionary.Insert(0, personalDictionary[i]);
            }
            else
            {
                Directory.CreateDirectory(appdata_dir + "dictionary");
                Directory.CreateDirectory(appdata_dir + "dictionary/personal");
            }

            return (dictionary);
        }

        static List<Genius_data> convertDictionaryToGenius(List<string> words)
        {
            List<Genius_data> dictionary = new List<Genius_data>();
            for (int i = 0; i < words.Count; i++)
                dictionary.Add(new Genius_data("dictionary", getWordValue(words[i])));
            return (dictionary);
        }

        static string getWordValue(string word)
        {
            int i = 0;
            while (i < word.Length && word[i] != ':')
                i++;
            if (i == word.Length)
                return (word);
            else
                return (word.Substring(0, i));
        }

        static int getWordOccurence(string word)
        {
            int i = word.Length - 1;
            while (i >= 0 && Char.IsNumber(word[i]))
                i--;
            if (i < 0 || i == word.Length - 1)
                return (0);
            else
            {
                try
                {
                    return (Convert.ToInt32(word.Substring(i + 1, word.Length - i - 1)));
                }
                catch(Exception)
                {
                    return (Int32.MaxValue);
                }
            }
        }

        static bool isSupportedLanguage(string language)
        {
            return (language == "en" || language == "fr");
        }

        static void updateDictionary(string appdata_dir, List<string> content, ref List<Genius_data> dictionary, string language = "en")
        {
            string[] words;
            List<string> dictionaryFile = Library.getFileContent(appdata_dir + "dictionary/" + language);
            List<int> dictionaryOccurence = new List<int>();
            bool dictionaryModified = false;
            List<string> dictionaryPerso = Library.getFileContent(appdata_dir + "dictionary/personal/" + language);
            bool persoModified = false;
            List<int> persoOccurence = new List<int>();
            setDictionaryLists(ref dictionaryFile, ref dictionaryOccurence, ref dictionaryPerso, ref persoOccurence);

            for(int i = 0; i < content.Count; i++)
            {
                words = content[i].Split(' ');
                for(int j = 0; j < words.Length; j++)
                {
                    if(words[j].Length > 1)
                    {
                        if (dictionaryFile.Contains(words[j]))
                        {
                            dictionaryOccurence[dictionaryFile.IndexOf(words[j])]++;
                            dictionaryModified = true;
                        }
                        else if (dictionaryPerso.Contains(words[j]))
                        {
                            persoOccurence[dictionaryPerso.IndexOf(words[j])]++;
                            persoModified = true;
                        }    
                        else
                        {
                            dictionary.Insert(0, new Genius_data("dictionary", words[j]));
                            dictionaryPerso.Add(words[j]);
                            persoOccurence.Add(1);
                            persoModified = true;
                        }
                    }
                }
            }

            if(dictionaryModified)
            {
                sortDictionary(ref dictionaryFile, ref dictionaryOccurence);
                try
                {
                    StreamWriter save = new StreamWriter(appdata_dir + "dictionary/" + language);
                    for (int i = 0; i < dictionaryFile.Count; i++)
                        save.WriteLine(dictionaryFile[i] + ":" + dictionaryOccurence[i]);
                    save.Close();
                }
                catch(Exception)
                {
                }
            }

            if (persoModified)
            {
                sortDictionary(ref dictionaryPerso, ref persoOccurence);
                try
                {
                    StreamWriter save = new StreamWriter(appdata_dir + "dictionary/personal/" + language);
                    for (int i = 0; i < dictionaryPerso.Count; i++)
                        save.WriteLine(dictionaryPerso[i] + ":" + persoOccurence[i]);
                    save.Close();
                }
                catch (Exception)
                {
                }
            }
        }

        static void setDictionaryLists(ref List<string> dictionaryFile, ref List<int> dictionaryOccurence, ref List<string> dictionaryPerso, ref List<int> persoOccurence)
        {
            for(int i = 0; i < dictionaryFile.Count; i++)
            {
                dictionaryOccurence.Add(getWordOccurence(dictionaryFile[i]));
                dictionaryFile[i] = getWordValue(dictionaryFile[i]);
            }
            for (int i = 0; i < dictionaryPerso.Count; i++)
            {
                persoOccurence.Add(getWordOccurence(dictionaryPerso[i]));
                dictionaryPerso[i] = getWordValue(dictionaryPerso[i]);
            }
        }

        static void sortDictionary(ref List<string> dictionary, ref List<int> occurence)
        {
            int move = 1;
            string swap = "";
            while(move > 0)
            {
                move = 0;
                for(int i = 0; i < occurence.Count - 1; i++)
                {
                    if(occurence[i] < occurence[i + 1])
                    {
                        swap = dictionary[i];
                        dictionary[i] = dictionary[i + 1];
                        dictionary[i + 1] = swap;
                        occurence[i + 1] += occurence[i];
                        occurence[i] = occurence[i + 1] - occurence[i];
                        occurence[i + 1] = occurence[i + 1] - occurence[i];
                        move++;
                    }
                }
            }
        }

        static void removeLineBackspace(ref string command, ref List<string> content, ref int input_pointer, ref int lineNumber, ref int line, ref int max_cursor)
        {
            //Assumes that it's not the first line (lineNumber > 0)
            //Assumes that you're at the beginning of the line to remove
            int newTopCursorPos = Console.CursorTop - 1;
            int newLeftCursorPos = (content[lineNumber - 1].Length + 2) % Console.WindowWidth;
            int cursorTopReplacement = 0;
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, Console.CursorTop);
            for (int i = lineNumber; i < content.Count; i++)
            {
                if (i < content.Count - 1)
                {
                    Console.Write("> " + content[i + 1]);
                    cursorTopReplacement = Console.CursorTop + 1;
                    Library.print_n_space(content[i].Length - content[i + 1].Length);
                    Library.print_n_space(Console.WindowWidth - Console.CursorLeft);
                    Console.SetCursorPosition(0, cursorTopReplacement);
                }
                else
                {
                    max_cursor = Console.CursorTop - 1;
                    Library.print_n_space(content[i].Length + 2);
                    Console.SetCursorPosition(newLeftCursorPos, newTopCursorPos);
                }
            }
            Console.CursorVisible = true; 
            lineNumber--;
            line = Console.CursorTop - (content[lineNumber].Length + 1) / Console.WindowWidth;
            command = content[lineNumber] + content[lineNumber + 1];
            input_pointer = content[lineNumber].Length;
            Console.Write(content[lineNumber + 1]);
            Program.cursor_replacement(content[lineNumber + 1].Length);
            content.RemoveAt(lineNumber + 1);
        }

        static void removeLineDelete(ref string command, ref List<string> content, ref int input_pointer, ref int lineNumber, ref int max_cursor)
        {
            //Assumes that you're at the end of the line
            if(lineNumber < content.Count - 1)
            {
                int newTopCursorPos = Console.CursorTop;
                int newLeftCursorPos = Console.CursorLeft;
                int cursorTopReplacement = 0;
                Console.CursorVisible = false;
                Console.SetCursorPosition(0, Console.CursorTop - ((content[lineNumber].Length + 1) / Console.WindowWidth));
                for (int i = lineNumber; i < content.Count; i++)
                {
                    if (i == lineNumber)
                    {
                        Console.Write("> " + content[i] + content[i + 1]);
                        cursorTopReplacement = Console.CursorTop + 1;
                        Library.print_n_space(Console.WindowWidth - Console.CursorLeft);
                        Console.SetCursorPosition(0, cursorTopReplacement);
                    }
                    else if (i < content.Count - 1)
                    {
                        Console.Write("> " + content[i + 1]);
                        cursorTopReplacement = Console.CursorTop + 1;
                        Library.print_n_space(content[i].Length - content[i + 1].Length);
                        Library.print_n_space(Console.WindowWidth - Console.CursorLeft);
                        Console.SetCursorPosition(0, cursorTopReplacement);
                    }
                    else
                    {
                        max_cursor = Console.CursorTop - 1;
                        Library.print_n_space(content[i].Length + 2);
                        Console.SetCursorPosition(newLeftCursorPos, newTopCursorPos);
                    }
                }
                Console.CursorVisible = true;
                command = content[lineNumber] + content[lineNumber + 1];
                content.RemoveAt(lineNumber + 1);
            }
            input_pointer--;
            
        }

        static void createLineSpace(ref string command, ref List<string> content, ref int input_pointer, ref int lineNumber, ref int line, ref int max_cursor, ref int lineBegin, ref string genius)
        {
            if (line == Console.BufferHeight - 2)
            {
                if (Console.BufferHeight >= 32700)
                    Console.Clear();
                else
                    Console.SetBufferSize(Console.BufferWidth, Console.BufferHeight + 300);
            }
            int cursorTopReplacement = 0;
            lineNumber++;
            if (input_pointer - 1 < content[lineNumber - 1].Length)
            {
                content.Insert(lineNumber, content[lineNumber - 1].Substring(input_pointer - 1));
                content[lineNumber - 1] = content[lineNumber - 1].Substring(0, input_pointer - 1);
                Console.SetCursorPosition(lineBegin, line);
                Console.Write(content[lineNumber - 1]);
                line = Console.CursorTop;
                lineBegin = Console.CursorLeft;
                Library.print_n_space(content[lineNumber].Length);
                Console.SetCursorPosition(lineBegin, line);
            }
            else
                content.Insert(lineNumber, "");
            line = Console.CursorTop + 1;
            lineBegin = 2;
            Console.CursorVisible = false;
            for (int i = lineNumber; i < content.Count; i++)
            {
                Console.SetCursorPosition(0, Console.CursorTop + 1);
                Console.Write("> " + content[i]);
                cursorTopReplacement = Console.CursorTop;
                if (i < content.Count - 1)
                {
                    Library.print_n_space(content[i + 1].Length - content[i].Length);
                    Console.SetCursorPosition(0, cursorTopReplacement);
                }

            }
            Console.CursorVisible = true;
            max_cursor = Console.CursorTop;
            Console.SetCursorPosition(lineBegin, line);
            genius = "";
            command = content[lineNumber];
            input_pointer = 0;
        }
    }
}
