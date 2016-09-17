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
                int i = 0;

                while (i < Execution.version.Length && current_line != null)
                {
                    if (Char.IsNumber(Execution.version[i]))
                    {
                        if (Convert.ToInt32(Execution.version[i].ToString()) == current_line.Length)
                        {
                            for (int j = 0; j < Convert.ToInt32(Execution.version[i].ToString()); j++)
                            {
                                if ((j > 4 && (current_line.Length <= j || current_line[j] != '\t')) || (j < 5 && (current_line.Length <= j || current_line[j] != ' ')))
                                {
                                    i = Execution.version.Length + 1;
                                    break;
                                }
                            }
                            current_line = check.ReadLine();
                        }
                        else
                            i = Execution.version.Length + 1;
                    }
                    i++;
                }

                check.Close();
                while (i < Execution.version.Length && !(Char.IsNumber(Execution.version[i])))
                    i++;

                return (i == Execution.version.Length && current_line == null);
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

            for (int i = 0; i < Execution.version.Length; i++)
            {
                if (Char.IsNumber(Execution.version[i]))
                {
                    for (int j = 0; j < Convert.ToInt32(Execution.version[i].ToString()); j++)
                    {
                        if (j > 4)
                            init.Write("\t");
                        else
                            init.Write(" ");
                    }
                    init.WriteLine();
                }
            }

            init.Close();
        }

        private static char encrypt_time(int time)
        {
            switch (time)
            {
                case (0):
                    return ('>');
                case (1):
                    return ('c');
                case (2):
                    return ('m');
                case (3):
                    return ('d');
                case (4):
                    return ('_');
                case (5):
                    return ('l');
                case (6):
                    return ('i');
                case (7):
                    return ('n');
                case (8):
                    return ('u');
                case (9):
                    return ('x');
                default:
                    return (' ');
            }
        }

        private static int decrypt_time(char crypted)
        {
            switch (crypted)
            {
                case ('>'):
                    return (0);
                case ('c'):
                    return (1);
                case ('m'):
                    return (2);
                case ('d'):
                    return (3);
                case ('_'):
                    return (4);
                case ('l'):
                    return (5);
                case ('i'):
                    return (6);
                case ('n'):
                    return (7);
                case ('u'):
                    return (8);
                case ('x'):
                    return (9);
                default:
                    return (-1);
            }
        }

        private static string cryptage_userName(string userName, int time)
        {
            string userName_crypted = "";

            for (int i = 0; i < userName.Length; i++)
                userName_crypted = userName_crypted + (char)(userName[i] ^ time);

            return (userName_crypted);
        }

        private static string encrypt_security_code()
        {
            string security_code = DateTime.Now.Year + (DateTime.Now.Month + 100).ToString().Substring(1, 2) + (DateTime.Now.Day + 100).ToString().Substring(1, 2) + (DateTime.Now.Hour + 100).ToString().Substring(1, 2) + (DateTime.Now.Minute + 100).ToString().Substring(1, 2) + (DateTime.Now.Second + 100).ToString().Substring(1, 2);
            string encryptage = "";
            Random rnd = new Random();
            int key = rnd.Next(0, 10);

            for (int i = 0; i < security_code.Length; i++)
                encryptage = encryptage + ((int)security_code[i] + security_code.Length).ToString();

            for (int i = 0; i < key; i++)
                encryptage = encryptage[encryptage.Length - 1] + encryptage.Substring(0, encryptage.Length - 1);
            encryptage = encryptage + (char)(key + encryptage.Length + 42);

            return (encryptage);
        }

        private static string decrypt_security_code(string security_code)
        {
            string decrypted_security_code = "";
            int key = (int)(security_code[security_code.Length - 1] - 41 - security_code.Length);
            security_code = security_code.Substring(0, security_code.Length - 1);

            for (int i = 0; i < key; i++)
                security_code = security_code.Substring(1, security_code.Length - 1) + security_code[0];

            for (int i = 0; i < security_code.Length / 2; i++)
            {
                try
                {
                    decrypted_security_code = decrypted_security_code + (char)(Convert.ToInt32(security_code.Substring(i * 2, 2)) - security_code.Length / 2);
                }
                catch (Exception)
                {
                    Console.WriteLine("> cmd Linux: Invalid security code");
                    security_code = "";
                    decrypted_security_code = "";
                }
            }

            return (decrypted_security_code);
        }

        public static bool encrypt_password(string new_password, string user, string appdata_dir)
        {
            string password_crypter = "";
            int time = DateTime.Now.Minute % 10;
            Random rnd = new Random();

            if (new_password.Length > 0 && user.Length > 0)
            {
                for (int i = 0; i < new_password.Length; i++)
                {
                    if (new_password[i] <= 126 && new_password[i] >= 33)
                        password_crypter = password_crypter + (char)(((new_password[i] + i + time) % 94) + 33);
                    else
                        password_crypter = password_crypter + new_password[i];
                }
                password_crypter = password_crypter + encrypt_time(time);

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
            int time = 0;
            int new_char;
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
                    userName_crypted = Library.extractString(userName_crypted, '¿', '¡');
                    password_crypted = Library.extractString(password_crypted, '¡', '¿');
                    time = decrypt_time(password_crypted[password_crypted.Length - 1]);
                    if (time > -1)
                    {
                        for (int i = 0; i < password_crypted.Length - 1; i++)
                        {
                            if (password_crypted[i] <= 126 && password_crypted[i] >= 33)
                            {
                                new_char = password_crypted[i] - 33;
                                if (new_char - i - time < 33)
                                    new_char = 94 + new_char;
                                new_char = new_char - i - time;
                                password = password + (char)new_char;
                            }
                            else
                                password = password + password_crypted[i];
                        }
                    }
                    else
                    {
                        Console.WriteLine("> cmd linux: unable to recover password: data was corrupted.");
                        return (false);
                    }
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
                    long arg_needed = 0;
                    int arg_needed3 = 0;
                    string arg_needed5 = "";
                    string[] arg_needed4 = new string[0];
                    superuser = true;
                    List<Genius_data> arg_needed2 = new List<Genius_data>();
                    List<Link> arg_link = new List<Link>();
                    NotificationManager arg_notif = new NotificationManager("");
                    Console.WriteLine("> cmd linux: initialisation of your password: ");
                    if (!((Execution.execute_input(new string[1][] { new string[1] { "password" } }, "", appdata_dir, ref superuser, ref superuser, ref arg_needed, ref arg_needed2, ref superuser, ref superuser, ref superuser, ref arg_needed3, ref superuser, ref superuser, ref superuser, ref superuser, ref superuser, ref arg_needed4, ref arg_needed3, ref arg_needed3, ref arg_needed5, ref arg_link, ref arg_notif)) == 0))
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

                save.WriteLine(encrypt_security_code());
                save.WriteLine(safeString('¿', 128, rnd) + cryptage_userName(userName, time) + '¡' + safeString('A', 128, rnd));
                save.WriteLine(safeString('¡', 128, rnd) + password_crypter + '¿' + safeString('R', 128, rnd));

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
