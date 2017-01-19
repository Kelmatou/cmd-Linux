using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualBasic.Devices;
using System.Threading.Tasks;

namespace cmd_Linux
{
    public static class Security
    {
        //SECURITY PROTOCOL 4.0
        public static void admin_connection_protocol(ref bool superuser, string appdata_dir)
        {
            string username = "";
            string password = "";
            string answer = "";
            string answer2 = "";
            int line = Console.CursorTop;

            if (decrypt_password(ref username, ref password, appdata_dir, ref superuser) && !superuser)
            {
                Console.Write("> ");
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("Enter username: ");
                answer = Console.ReadLine();
                Console.ResetColor();
                Console.Write("> ");
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("Enter password: ");
                answer2 = get_password_protocol();
                Console.ResetColor();

                if (answer != username || answer2 != password)
                {
                    Console.Write("> ");
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Connection failed!");
                }
                else
                {
                    Console.SetCursorPosition(2, line);
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("Enter username: " + answer);
                    Console.ResetColor();
                    Console.Write("> ");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("Enter password: ");
                    for (int i = 0; i < answer2.Length; i++)
                    {
                        Console.Write("*");
                    }
                    Console.ResetColor();
                    Console.Write("\n> ");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("Connection succeed!");
                    superuser = true;
                }
                Console.ResetColor();
            }
        }

        public static bool get_admin_password(ref string password, string appdata_dir)
        {
            string userName_crypted = "";
            string password_crypted = "";
            bool boolarg = false;
            password = "";

            if(decrypt_password(ref userName_crypted, ref password_crypted, appdata_dir, ref boolarg, false))
            {
                password = password_crypted;
                return (true);
            }
            return (false);
        }

        private static bool is_good_init_code(string appdata_dir)
        {
            try
            {
                StreamReader check = new StreamReader(appdata_dir + "/settings");
                string current_line = check.ReadLine();
		//CREATE A FUNCTION THAT VERIFY THE INIT CODE
            }
            catch (Exception)
            {
                Console.WriteLine("> cmd Linux: Unable to verify valid security. Try to execute cmd linux with admin rights.");
                return (false);
            }
        }

        public static void generate_init_file(string file_path)
        {
            StreamWriter init = new StreamWriter(file_path);

            //GENERATE YOUR INIT FILE

            init.Close();
        }

        private static char encrypt_time(int time)
        {
            //ENCRYPT TIME VALUE
        }

        private static int decrypt_time(char crypted)
        {
            //DECRYPT TIME VALUE
        }

        private static string cryptage_userName(string userName, int time)
        {
            string userName_crypted = "";

            //ENCRYPT USERNAME

            return (userName_crypted);
        }

        private static string encrypt_security_code()
        {
            string security_code = DateTime.Now.Year + (DateTime.Now.Month + 100).ToString().Substring(1, 2) + (DateTime.Now.Day + 100).ToString().Substring(1, 2) + (DateTime.Now.Hour + 100).ToString().Substring(1, 2) + (DateTime.Now.Minute + 100).ToString().Substring(1, 2) + (DateTime.Now.Second + 100).ToString().Substring(1, 2);
            string encryptage = "";

	    //ENCRYPT SECURITY CODE

            return (encryptage);
        }

        private static string decrypt_security_code(string security_code)
        {
            string decrypted_security_code = "";
            
	    //DECRYPT SECURITY CODE

            return (decrypted_security_code);
        }

        public static bool encrypt_password(string new_password, string user, string appdata_dir)
        {
            string password_crypter = "";

            if (new_password.Length > 0 && user.Length > 0)
            {
                //ENCRYPT PASSWORD

                return (savePasswordFile(appdata_dir, time, password_crypter, user, rnd));
            }
            else
                return (false);
        }

        public static bool decrypt_password(ref string userName, ref string password, string appdata_dir, ref bool superuser, bool canInitialize = true)
        {
            string userName_crypted;
            string password_crypted;
            string security_code_crypted;
            password = "";

            if (File.Exists(appdata_dir + "/settings"))
            {
                try
                {
                    StreamReader recover = new StreamReader(appdata_dir + "/settings");

                    security_code_crypted = recover.ReadLine();
                    userName_crypted = recover.ReadLine();
                    password_crypted = recover.ReadLine();

                    recover.Close();
                }
                catch (Exception)
                {
                    Console.WriteLine("> cmd linux: unable to recover password. Try to execute cmd linux with admin rights.");
                    return (false);
                }
            }
            else
            {
                Console.WriteLine("> cmd linux: unable to recover password: some data is missing.");
                return (false);
            }

            if (security_code_crypted != null && security_code_crypted[0] != ' ' && userName_crypted != null && password_crypted != null && password_crypted.Length > 1)
            {
                if (decrypt_security_code(security_code_crypted) == (File.GetLastWriteTime(appdata_dir + "/settings").Year + (File.GetLastWriteTime(appdata_dir + "/settings").Month + 100).ToString().Substring(1, 2) + (File.GetLastWriteTime(appdata_dir + "/settings").Day + 100).ToString().Substring(1, 2) + (File.GetLastWriteTime(appdata_dir + "/settings").Hour + 100).ToString().Substring(1, 2) + (File.GetLastWriteTime(appdata_dir + "/settings").Minute + 100).ToString().Substring(1, 2) + (File.GetLastWriteTime(appdata_dir + "/settings").Second + 100).ToString().Substring(1, 2)))
                {
                    //DECRYPT PASSWORD
                }
                else
                {
                    Console.WriteLine("> cmd linux: data was modified outside of the application!\n> cmd linux: For security reasons, MASTER mode is not available anymore...");
                    return (false);
                }

                userName = cryptage_userName(userName_crypted, time);
            }
            else
            {
                if (is_good_init_code(appdata_dir) && canInitialize)
                {
                    superuser = true;
                    Console.WriteLine("> cmd linux: initialisation of your password: ");
                    if (!((Execution.execute_input(new string[1][] { new string[1] { "password" } })) == 0))
                    {
                        superuser = false;
                        return (false);
                    }
                }
                else
                {
                    if (canInitialize)
                        Console.WriteLine("> cmd linux: unable to recover password: data was corrupted.");
                    else
                        Console.WriteLine("> cmd linx: password non initialized.");
                    return (false);
                }
            }

            return (true);
        }

        private static bool savePasswordFile(string appdata_dir, int time, string password_crypter, string userName, Random rnd)
        {
            try
            {
                StreamWriter save = new StreamWriter(appdata_dir + "/settings");

                //SAVE YOUR PASSWORD FILE

                save.Close();

                Console.WriteLine("> password: new password saved!");
            }
            catch (Exception)
            {
                Console.WriteLine("> password: unable to save new password. Try to execute cmd linux with admin rights");
                return (false);
            }
            return (true);
        }

        private static string safeString(char endCode, int maxLength, Random rnd)
        {
            string result = "" + (char)(rnd.Next(32, 127));
            int i = 1;
            while (result[i - 1] != endCode && i < maxLength)
            {
                if (i == maxLength - 1)
                    result = result + endCode;
                else
                    result = result + (char)(rnd.Next(32, 127));
                i++;
            }
            return (result);
        }

        public static string get_password_protocol()
        {
            ConsoleKeyInfo key_input = new ConsoleKeyInfo();
            string password = "";
            int cursor_left = Console.CursorLeft;
            int cursor_top = Console.CursorTop;
            int max_cursor_left = Console.CursorLeft;
            int max_cursor_top = Console.CursorTop;
            int input_pointer = 0;
            int delta_input = 0;


            while (key_input.Key != ConsoleKey.Enter && key_input.Key != (ConsoleKey)255)
            {
                if (cursor_top == Console.BufferHeight - 1)
                    Console.SetBufferSize(Console.BufferWidth, Console.BufferHeight + 300);
                key_input = Console.ReadKey(true);
                delta_input = update_password(ref password, key_input, ref input_pointer);
                if (delta_input != 0)
                {
                    if (delta_input > 0)
                    {
                        if (Console.CursorLeft == Console.WindowWidth - 1)
                        {
                            cursor_left = 0;
                            cursor_top++;
                        }
                        else
                            cursor_left++;
                        Console.SetCursorPosition(max_cursor_left, max_cursor_top);
                        Console.Write("*");
                        max_cursor_left = Console.CursorLeft;
                        max_cursor_top = Console.CursorTop;
                    }
                    else
                    {
                        Console.SetCursorPosition(max_cursor_left, max_cursor_top);
                        Program.cursor_replacement(-1 * delta_input);
                        max_cursor_left = Console.CursorLeft;
                        max_cursor_top = Console.CursorTop;
                        if (cursor_top > max_cursor_top)
                        {
                            cursor_top = max_cursor_top;
                            cursor_left = max_cursor_left;
                        }
                        else if (cursor_left > max_cursor_left)
                            cursor_left = max_cursor_left;
                        Execution.print_n_space(-1 * delta_input);
                    }
                    Console.SetCursorPosition(cursor_left, cursor_top);
                }
                else
                {
                    cursor_left = Console.CursorLeft;
                    cursor_top = Console.CursorTop;
                }
            }
            Console.WriteLine();
            return (password);
        }

        private static int update_password(ref string password, ConsoleKeyInfo key_input, ref int input_pointer)
        {
            Computer mycomputer = new Computer();
            int delta_input = password.Length;
            input_pointer++;

            switch (key_input.Key)
            {
                case (ConsoleKey.Backspace):
                    if (password.Length > 0)
                    {
                        if (input_pointer == password.Length + 1)
                        {
                            password = password.Substring(0, password.Length - 1);
                            input_pointer--;
                        }
                        else
                        {
                            if (input_pointer >= 2)
                            {
                                password = password.Substring(0, input_pointer - 2) + password.Substring(input_pointer - 1, password.Length - input_pointer + 1);
                                input_pointer--;
                            }
                        }
                    }
                    else
                    {
                        input_pointer--;
                        return (0);
                    }
                    input_pointer--;
                    break;
                case (ConsoleKey.Tab):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Enter):
                    break;
                case (ConsoleKey.A):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.B):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.C):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.D):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.E):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.F):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.G):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.H):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.I):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.J):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.K):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.L):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.M):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.N):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.O):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.P):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Q):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.R):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.S):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.T):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.U):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.V):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.W):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.X):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Y):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Z):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad0):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.D0):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad1):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.D1):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad2):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.D2):
                    if (key_input.KeyChar == '\0')
                    {
                        input_pointer--; //~ s'affiche uniquement sur le prochain caractère
                        return (0);
                    }
                    else
                        Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad3):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.D3):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad4):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.D4):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad5):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.D5):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad6):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.D6):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad7):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.D7):
                    if (key_input.KeyChar == '\0')
                    {
                        input_pointer--; //` s'affiche uniquement sur le prochain caractère
                        return (0);
                    }
                    else
                        Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad8):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.D8):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad9):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.D9):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Add):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Multiply):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Subtract):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Divide):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Spacebar):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
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
                    if (input_pointer > password.Length)
                    {
                        input_pointer--;
                    }
                    else
                        Program.cursor_replacement(-1);
                    return (0);
                case (ConsoleKey.UpArrow):
                    input_pointer--;
                    return (0);
                case (ConsoleKey.DownArrow):
                    input_pointer--;
                    return (0);
                case (ConsoleKey.Escape):
                    input_pointer = 0;
                    if (password == "")
                        return (0);
                    else
                        password = "";
                    break;
                case (ConsoleKey.Delete):
                    if (input_pointer <= password.Length)
                    {
                        if (input_pointer == password.Length)
                            password = password.Substring(0, password.Length - 1);
                        else
                            password = password.Substring(0, input_pointer - 1) + password.Substring(input_pointer, password.Length - input_pointer);
                    }
                    else
                    {
                        input_pointer--;
                        return (0);
                    }
                    input_pointer--;
                    break;
                case (ConsoleKey.Insert):
                    input_pointer--;
                    return (0);
                case (ConsoleKey.Decimal):
                    if (mycomputer.Keyboard.CapsLock || mycomputer.Keyboard.ShiftKeyDown)
                    {
                        if (input_pointer <= password.Length)
                        {
                            if (input_pointer == password.Length)
                            {
                                password = password.Substring(0, password.Length - 1);
                            }
                            else
                            {
                                password = password.Substring(0, input_pointer - 1) + password.Substring(input_pointer, password.Length - input_pointer);
                            }
                        }
                        else
                        {
                            input_pointer--;
                            return (0);
                        }
                        input_pointer--;
                    }
                    else
                    {
                        Program.add_key_to_command(ref password, key_input, input_pointer);
                        break;
                    }
                    break;
                case (ConsoleKey.OemComma):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.OemPeriod):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.OemPlus):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.OemMinus): //- ou _ sur QWERTY
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem1):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem2):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem3):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem4):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem5):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem6):
                    if (key_input.KeyChar == '\0')
                    {
                        input_pointer--; //^ ¨ n'affichent uniquement sur le prochain caractère
                        return (0);
                    }
                    else
                    {
                        Program.add_key_to_command(ref password, key_input, input_pointer);
                    }
                    break;
                case (ConsoleKey.Oem7):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem8):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem102):
                    Program.add_key_to_command(ref password, key_input, input_pointer);
                    break;
                default:
                    input_pointer--;
                    return (0);
            }

            return (password.Length - delta_input);
        }
    }
}
