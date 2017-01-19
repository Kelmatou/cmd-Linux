using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Permissions;
using System.Threading;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Reflection;
using Microsoft.VisualBasic.Devices;
using System.Windows.Forms;
using System.Windows;
using System.Threading.Tasks;

namespace cmd_Linux
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Security.generate_init_file("D:\\Info_project\\C#_project\\Apps\\TP16 Shell Linux\\tiny42sh\\tiny42sh\\settings"); //A METTRE EN COMMENTAIRE AVANT COMPILATION!!!!
            cmd_Linux();
        } // /!\ LIGNE A METTRE EN COMMENTAIRE!

        static void cmd_Linux()
        {
            bool active_process = true;
            Console.Title = "cmd Linux";

            while (active_process)
            {
                try
                {
                    launch_cmd();
                    active_process = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("> cmd Linux: an unhandled error occured");
                    send_email(ex);
                }
            }  
        }

        static void launch_cmd()
        {
            ShellEnvironment.init();

            if (ShellEnvironment.auto_lock || ShellEnvironment.auto_log)
            {
                if (Security.get_admin_password(ref ShellEnvironment.command, ShellEnvironment.appdata_dir))
                {
                    ShellEnvironment.command = "lock " + ShellEnvironment.command;
                    Execution.execute_input(Interpreter.parse_input(ShellEnvironment.command, ShellEnvironment.appdata_dir));
                    if (ShellEnvironment.auto_log)
                        ShellEnvironment.superuser = true;
                }
                else
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("> cmd Linux: auto_lock: session was blocked cause password was unsafe.");
                    Console.WriteLine("> cmd Linux: auto_lock: try to reinstall cmd Linux...");
                    Console.ResetColor();
                    Thread.Sleep(5000);
                    ShellEnvironment.working = false;
                }
            }

            while (ShellEnvironment.working)
            {
                print_cmd_intro_line();

                while (!(File.Exists(ShellEnvironment.appdata_dir + "/script") && ShellEnvironment.script_enable) && !Console.KeyAvailable)
                {
                    ShellEnvironment.notificationManager.triggerNotifications(DateTime.Now, true);
                    Thread.Sleep(ShellEnvironment.refresh_timer);
                }

                if (File.Exists(ShellEnvironment.appdata_dir + "/script") && ShellEnvironment.script_enable && !Console.KeyAvailable)
                {
                        try
                        {
                            StreamReader reader = new StreamReader(ShellEnvironment.appdata_dir + "/script");
                            ShellEnvironment.command = reader.ReadLine();
                            reader.Close();
                            Library.erase_script_line(ShellEnvironment.appdata_dir + "/script");
                            if (Console.CursorTop == Console.BufferHeight - 1)
                            {
                                if (Console.BufferHeight >= 32700)
                                    Console.Clear();
                                else
                                    Console.SetBufferSize(Console.BufferWidth, Console.BufferHeight + 300);
                            }
                            Console.SetCursorPosition(0, Console.CursorTop + 1);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("\n> cmd linux: script: access denied! (execute it with admin rights)");
                        }
                }
                else if (File.Exists(ShellEnvironment.appdata_dir + "/script") && ShellEnvironment.script_enable && Console.KeyAvailable)
                {
                    ConsoleKeyInfo keypressed = Console.ReadKey(true);
                            if(keypressed.Key == ConsoleKey.Escape)
                            {
                                ShellEnvironment.command = "";
                                try
                                {
                                    File.Delete(ShellEnvironment.appdata_dir + "/script");
                                    Console.WriteLine("\n> cmd linux: script execution was canceled successfully.");
                                }
                                catch (Exception)
                                {
                                    ShellEnvironment.script_enable = ShellEnvironment.superuser;
                                    Console.WriteLine("\n> cmd linux: script execution was canceled but couldn't be completely stop. (execute it with admin rights)");
                                    if (!ShellEnvironment.superuser)
                                        Console.WriteLine("> cmd linux: scripts are disabled. Log you as MASTER to enable script");
                                }
                            }
                            else
                            {
                                try
                                {
                                    StreamReader reader = new StreamReader(ShellEnvironment.appdata_dir + "/script");
                                    ShellEnvironment.command = reader.ReadLine();
                                    reader.Close();
                                    Library.erase_script_line(ShellEnvironment.appdata_dir + "/script");
                                    Console.SetCursorPosition(0, Console.CursorTop);
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("\n> cmd linux: script: access denied! (execute it with admin rights)");
                                }
                            }
                }
                else
                    ShellEnvironment.command = get_input();
                if (ShellEnvironment.command != null && ShellEnvironment.command != "")
                {
                    /*if(command.Length > 8 && command.Substring(0,9) == "/feedback")
                    {
                        Computer my_computer = new Computer();
                        send_feedback("Feedback from " + Environment.UserName + " on " + my_computer.Info.OSFullName + " with cmd Linux V." + Execution.version);
                    }
                    else
                    {
                        Execution.execute_input(Interpreter.parse_input(command, appdata_dir), desktop_dir, appdata_dir, ref working, ref superuser, ref last_result, ref all_genius_data, ref script_enable, ref genius_enable, ref private_mode, ref refresh_timer, ref auto_lock, ref auto_log, ref cmd_print_user, ref cmd_print_path, ref cmd_print_time, ref previous_directories, ref previous_directory_pointer, ref max_genius_data, ref language, ref allLinks, ref notificationManager);
                    }*/
                    Execution.execute_input(Interpreter.parse_input(ShellEnvironment.command, ShellEnvironment.appdata_dir));
                }
            }

            Console.WriteLine("> Process ended");
            Thread.Sleep(42);
        }

        static void print_directory()
        {
            string directory = Directory.GetCurrentDirectory();
            int last_stop = directory.Length - 1;
            int i = directory.Length - 1;

            while(i > 0 && directory.Length - i < 30)
            {
                if(directory[i] == '\\' || directory[i] == '/')
                    last_stop = i;
                i--;
            }

            if(i > 0)
            {
                if(last_stop == directory.Length - 1)
                    directory = "..." + directory.Substring(directory.Length - 27, 27);
                else
                    directory = directory.Substring(last_stop, directory.Length - last_stop);
            }

            Console.ResetColor();
            Console.Write("~ ");
            if (ShellEnvironment.path_color != ConsoleColor.Black)
                Console.ForegroundColor = ShellEnvironment.path_color;
            Console.Write(directory);
            Console.ResetColor();
        }

        static void print_user(bool is_admin)
        {
            if (ShellEnvironment.user_color == ConsoleColor.Black)
                Console.ResetColor();
            else
                Console.ForegroundColor = ShellEnvironment.user_color;
            Console.Write(System.Environment.UserName);
            if(is_admin)
                Console.Write(" (MASTER)");
            Console.ResetColor();
        }

        public static void print_cmd_intro_line()
        {
            Console.ResetColor();
            Console.Write("> ");
            if (ShellEnvironment.time_color != ConsoleColor.Black)
                Console.ForegroundColor = ShellEnvironment.time_color;
            if (ShellEnvironment.cmd_print_time)
                Console.Write("[" + (DateTime.Now.Hour < 10 ? "0" : "") + DateTime.Now.Hour + ":" + (DateTime.Now.Minute < 10 ? "0" : "") + DateTime.Now.Minute + ":" + (DateTime.Now.Second < 10 ? "0" : "") + DateTime.Now.Second + "]");
            Console.ResetColor();
            if (ShellEnvironment.cmd_print_user)
            {
                if (ShellEnvironment.cmd_print_time)
                    Console.Write(" ");
                print_user(ShellEnvironment.superuser);
            }
            if (ShellEnvironment.cmd_print_path)
            {
                if (ShellEnvironment.cmd_print_time || ShellEnvironment.cmd_print_user)
                    Console.Write(" ");
                print_directory();
            }
            if (ShellEnvironment.cmd_print_user || ShellEnvironment.cmd_print_path || ShellEnvironment.cmd_print_time)
                Console.Write(": ");
        }

        static void send_email(Exception ex)
        {
            string answer = "";
            char key;

            Console.Write("> cmd Linux: send feedback? (Y/N)");
            key = Console.ReadKey().KeyChar;
            
            if (key == 'Y' || key == 'y')
            {
                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com", 587);

                    Console.Write("> cmd Linux: Describe the situation just before the bug:\n> ");
                    answer = Console.ReadLine();

                    mail.From = new MailAddress(""); //SENDER GMAIL
                    mail.To.Add(""); //RECEIVER GMAIL
                    mail.Subject = "cmd Linux: Bug report";
                    mail.Body = "> cmd Linux: an unhandled error occured!\n> Description: " + answer + "\n> Bug report details: \n> " + ex + "\n> cmd Linux version: " + Execution.version;

                    SmtpServer.Credentials = new System.Net.NetworkCredential("", ""); //SENDER GMAIL, EMAIL PASSWORD
                    SmtpServer.EnableSsl = true;

                    SmtpServer.Send(mail);
                    Console.WriteLine("> cmd Linux: Report sent, thank you!");  
                    
                }
                catch (Exception)
                {
                    if(NetworkInterface.GetIsNetworkAvailable())
                        Console.WriteLine("> cmd Linux: An error occured during the sends");
                    else
                        Console.WriteLine("> cmd Linux: Connection error");
                }
            }
        }

        static void send_feedback(string message, bool display_message = true)
        {
            string answer = "auto_feedback_system";
            //string address = "";
            //string password = "";

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com", 587);

                //Console.Write("> cmd Linux: Enter your email address: ");
                //address = Console.ReadLine();
                mail.From = new MailAddress(""); //SENDER GMAIL
                // Console.Write("> cmd Linux: Enter your email password: ");
                //password = Console.ReadLine();
                mail.To.Add(""); //RECEIVER GMAIL
                mail.Subject = "cmd Linux: Feedback";
                if (display_message)
                {
                    Console.Write("> cmd Linux: Enter your message:\n> ");
                    answer = Console.ReadLine();
                }
                mail.Body = "> cmd Linux: Feedback: " + answer + "\n> cmd Linux details: \n> " + message;

                SmtpServer.Credentials = new System.Net.NetworkCredential("", ""); //SENDER GMAIL, EMAIL PASSWORD
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                if (display_message)
                {
                    Console.WriteLine("> cmd Linux: Feedback sent, thank you!");
                }
            }
            catch (Exception)
            {
                if(display_message)
                {
                    if (NetworkInterface.GetIsNetworkAvailable())
                    {
                        Console.WriteLine("> cmd Linux: An error occured during the sends");
                    }
                    else
                    {
                        Console.WriteLine("> cmd Linux: Connection error");
                    }
                }
            }
        }
        
        //KEYBOARD FUNCTION

        static string get_input()
        {
            ConsoleKeyInfo key_input = new ConsoleKeyInfo();
            string genius = "";
            string command = "";
            string last_word = "";
            int line_beginning = Console.CursorLeft;
            int line = Console.CursorTop;
            int max_cursor = Console.CursorTop;
            int input_pointer = 0;
            bool quote_mode = false;
            int delta_input = 0;

            if(line == Console.BufferHeight - 1)
            {
                if (Console.BufferHeight >= 32700)
                    Console.Clear();
                else
                    Console.SetBufferSize(Console.BufferWidth, Console.BufferHeight + 300);
            }
            while (key_input.Key != ConsoleKey.Enter && ShellEnvironment.working)
            {
                key_input = Console.ReadKey(true);
                delta_input = update_input(ref command, key_input, ref genius, ref input_pointer, ref quote_mode, ref line, last_word, ref max_cursor);
                if(delta_input != 0 || key_input.Key == ConsoleKey.UpArrow || key_input.Key == ConsoleKey.DownArrow)
                {
                    erase_genius_trace(genius, command.Length - input_pointer);
                    last_word = get_last_word(command);
                    display_update(command, line_beginning, line, input_pointer, delta_input, key_input);
                    if (key_input.Key != ConsoleKey.Enter && (genius.Length <= last_word.Length || genius.Substring(0, last_word.Length) != last_word) && key_input.KeyChar != '"' && command.Length > 0 && command[command.Length - 1] != '"' && ShellEnvironment.genius_enable)
                        genius = Interpreter.genius_cmd(last_word, Interpreter.get_arg_expected_code(command), ShellEnvironment.all_genius_data, command, ShellEnvironment.appdata_dir, ShellEnvironment.allLinks, ShellEnvironment.all_cmd);
                    else if (last_word == "")
                        genius = "";
                    if (key_input.Key != ConsoleKey.Enter && key_input.KeyChar != '"' && ShellEnvironment.genius_enable && ((command.Length > 0 && command[command.Length - 1] != '"') || quote_mode))
                        Interpreter.print_genius_cmd(genius, last_word, ref max_cursor);
                    else
                        max_cursor = Console.CursorTop;
                    cursor_replacement(command.Length - input_pointer);
                }
            }
            Console.SetCursorPosition(0, max_cursor + 1);
            if (!ShellEnvironment.private_mode)
            {
                if (ShellEnvironment.stack_pointer == 256)
                {
                    ShellEnvironment.stack_pointer--;
                    for(int i = 1; i < 256;i++)
                        ShellEnvironment.stack_input[i - 1] = ShellEnvironment.stack_input[i];
                }
                ShellEnvironment.stack_input[ShellEnvironment.stack_pointer] = command;
                ShellEnvironment.stack_pointer++;
            }
            
            return (command);
        }

        static int update_input(ref string command, ConsoleKeyInfo key_input, ref string genius, ref int input_pointer, ref bool quote_mode, ref int line, string current_input, ref int max_cursor)
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
                        erase_genius_trace(genius, command.Length - input_pointer);
                        if (key_input.Modifiers == ConsoleModifiers.Control)
                        {
                            if (get_last_word_with_quotes(command).Length == 0)
                            {
                                input_pointer--;
                                return (0);
                            }
                            else
                            {
                                if (command.Substring(0, command.Length - get_last_word_with_quotes(command).Length).Length < input_pointer)
                                    input_pointer = command.Substring(0, command.Length - get_last_word_with_quotes(command).Length).Length + 1;
                                command = command.Substring(0, command.Length - get_last_word_with_quotes(command).Length);
                            }
                            if (input_pointer < 1)
                                input_pointer = 1;
                        }
                        else if (input_pointer == 1)
                        {
                            input_pointer--;
                            return (0);
                        }
                        else
                        {
                            if (input_pointer == command.Length + 1)
                            {
                                if (command[command.Length - 1] == '"')
                                    quote_mode = !quote_mode;
                                command = command.Substring(0, command.Length - 1);
                                input_pointer--;
                            }
                            else
                            {
                                if (input_pointer >= 2)
                                {
                                    if (command[input_pointer - 2] == '"')
                                        quote_mode = !quote_mode;
                                    command = command.Substring(0, input_pointer - 2) + command.Substring(input_pointer - 1, command.Length - input_pointer + 1);
                                    input_pointer--;
                                }
                            }
                        }
                        if (command.Length == 0)
                        {
                            genius = "";
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
                    if (key_input.Modifiers == ConsoleModifiers.Control)
                    {
                        genius_linux = Interpreter.genius_linux(get_last_word(command), Interpreter.get_arg_expected_code(command), ShellEnvironment.all_genius_data, command, ShellEnvironment.appdata_dir, ShellEnvironment.allLinks, ShellEnvironment.all_cmd);
                        if (genius_linux.Length > 0 && genius_linux != get_last_word(command))
                        {
                            if (contain_space(genius_linux))
                            {
                                if (command.Length > get_last_word(command).Length && command[command.Length - 1 - get_last_word(command).Length] == '"')
                                {
                                    genius_linux = genius_linux + "\"";
                                    quote_mode = !quote_mode;
                                    input_pointer++;
                                }
                                else
                                {
                                    genius_linux = "\"" + genius_linux + "\"";
                                    input_pointer += 2;
                                }
                            }
                            if (Static_data.is_original_cmd(genius_linux) && genius_linux[genius_linux.Length - 1] != ';')
                            {
                                genius_linux = genius_linux + " ";
                                input_pointer++;
                            }
                            command = command.Substring(0, command.Length - get_last_word(command).Length) + genius_linux;
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
                        if (genius.Length > 0 && genius != get_last_word(command))
                        {
                            if (contain_space(genius))
                            {
                                if (command.Length > get_last_word(command).Length && command[command.Length - 1 - get_last_word(command).Length] == '"')
                                {
                                    genius = genius + "\"";
                                    quote_mode = !quote_mode;
                                    input_pointer++;
                                }
                                else
                                {
                                    genius = "\"" + genius + "\"";
                                    input_pointer += 2;
                                }
                            }
                            if (Static_data.is_original_cmd(genius) && genius[genius.Length - 1] != ';')
                            {
                                genius = genius + " ";
                                input_pointer++;
                            }
                            command = command.Substring(0, command.Length - get_last_word(command).Length) + genius;
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
                    erase_genius_trace(genius, command.Length - input_pointer);
                    break;
                case (ConsoleKey.A):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.B):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.C):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.D):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.E):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.F):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.G):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.H):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.I):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.J):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.K):
                    if (key_input.Modifiers == ConsoleModifiers.Control)
                    {
                        Execution.execute_input(new string[1][] { new string[1] { "lock" } });
                        input_pointer--;
                        line = 0;
                        Console.SetCursorPosition(0, 0);
                        print_cmd_intro_line();
                        Console.Write(command);
                        Interpreter.print_genius_cmd(genius, current_input, ref max_cursor);
                    }
                    else
                        add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.L):
                    if (key_input.Modifiers == ConsoleModifiers.Control)
                    {
                        Execution.execute_input(new string[1][] { new string[1] { "clear" } });
                        input_pointer--;
                        line = 0;
                        Console.SetCursorPosition(0, 0);
                        print_cmd_intro_line();
                        Console.Write(command);
                        Interpreter.print_genius_cmd(genius, current_input, ref max_cursor);
                    }
                    else
                        add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.M):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.N):
                    if (key_input.Modifiers == ConsoleModifiers.Control)
                    {
                        Execution.execute_input(new string[1][] { new string[1] { "launch" } });
                        input_pointer--;
                        return (0);
                    }
                    else
                        add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.O):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.P):
                    if (key_input.Modifiers == ConsoleModifiers.Control)
                    {
                        Execution.execute_input(new string[1][] { new string[1] { "ls" } });
                        input_pointer--;
                        return (0);
                    }
                    else
                        add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Q):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.R):
                    if (key_input.Modifiers == ConsoleModifiers.Control)
                    {
                        Execution.execute_input(new string[2][] { new string[1] { "launch" }, new string[1] { "exit" } });
                        input_pointer--;
                        return (0);
                    }
                    else
                    {
                        add_key_to_command(ref command, key_input, input_pointer);
                    }
                    break;
                case (ConsoleKey.S):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.T):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.U):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.V):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.W):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.X):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Y):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Z):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad0):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.D0):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad1):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.D1):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad2):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.D2):
                    if (key_input.KeyChar == '\0')
                    {
                        input_pointer--; //~ s'affiche uniquement sur le prochain caractère
                        return (0);
                    }
                    else
                    {
                        add_key_to_command(ref command, key_input, input_pointer);
                    }
                    break;
                case (ConsoleKey.NumPad3):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.D3):
                    add_key_to_command(ref command, key_input, input_pointer);
                    if (key_input.KeyChar == '"')
                    {
                        quote_mode = !quote_mode;
                    }
                    break;
                case (ConsoleKey.NumPad4):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.D4):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad5):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.D5):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad6):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.D6):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad7):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.D7):
                    if (key_input.KeyChar == '\0')
                    {
                        input_pointer--; //` s'affiche uniquement sur le prochain caractère
                        return (0);
                    }
                    else
                    {
                        add_key_to_command(ref command, key_input, input_pointer);
                    }
                    break;
                case (ConsoleKey.NumPad8):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.D8):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad9):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.D9):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Add):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Multiply):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Subtract):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Divide):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Spacebar):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.LeftArrow):
                    if (input_pointer > 1)
                    {
                        input_pointer -= 2;
                        cursor_replacement(1);
                    }
                    else if (input_pointer == 1)
                    {
                        input_pointer = 0;
                    }
                    return (0);
                case (ConsoleKey.RightArrow):
                    if (input_pointer > command.Length)
                    {
                        input_pointer--;
                    }
                    else
                    {
                        cursor_replacement(-1);
                    }
                    return (0);
                case (ConsoleKey.UpArrow):
                    if (ShellEnvironment.stack_pointer > 0)
                    {
                        erase_genius_trace(genius, command.Length - input_pointer);
                        ShellEnvironment.stack_pointer--;
                        command = ShellEnvironment.stack_input[ShellEnvironment.stack_pointer];
                        input_pointer = command.Length;
                        genius = "";
                    }
                    else
                    {
                        input_pointer--;
                        return (0);
                    }
                    break;
                case (ConsoleKey.DownArrow):
                    if (ShellEnvironment.stack_pointer < 255)
                    {
                        erase_genius_trace(genius, command.Length - input_pointer);
                        ShellEnvironment.stack_pointer++;
                        command = ShellEnvironment.stack_input[ShellEnvironment.stack_pointer];
                        input_pointer = command.Length;
                        genius = "";
                    }
                    else
                    {
                        input_pointer--;
                        return (0);
                    }
                    break;
                case (ConsoleKey.Escape):
                    erase_genius_trace(genius, command.Length - input_pointer);
                    input_pointer = 0;
                    if (command != "")
                    {
                        command = "";
                        genius = "";
                    }
                    else
                    {
                        return (0);
                    }
                    break;
                case (ConsoleKey.Delete):
                    if (input_pointer <= command.Length)
                    {
                        if (input_pointer == command.Length)
                        {
                            if (command[command.Length - 1] == '"')
                            {
                                quote_mode = !quote_mode;
                            }
                            command = command.Substring(0, command.Length - 1);
                        }
                        else
                        {
                            if (command[input_pointer - 1] == '"')
                            {
                                quote_mode = !quote_mode;
                            }
                            command = command.Substring(0, input_pointer - 1) + command.Substring(input_pointer, command.Length - input_pointer);
                        }
                        if (command.Length == 0)
                        {
                            erase_genius_trace(genius, command.Length - input_pointer);
                            genius = "";
                        }
                    }
                    else
                    {
                        input_pointer--;
                        return (0);
                    }
                    input_pointer--;
                    break;
                case (ConsoleKey.Insert): //devrait faire un coller --'
                    if (input_pointer <= command.Length)
                    {
                        command = command.Substring(0, input_pointer - 1) + System.Windows.Forms.Clipboard.GetText() + command.Substring(input_pointer - 1, command.Length - input_pointer + 1);
                    }
                    else
                    {
                        command = command + System.Windows.Forms.Clipboard.GetText();
                    }
                    input_pointer = command.Length;
                    break;
                case (ConsoleKey.Decimal):
                    if (mycomputer.Keyboard.CapsLock || mycomputer.Keyboard.ShiftKeyDown)
                    {
                        if (input_pointer <= command.Length)
                        {
                            if (input_pointer == command.Length)
                            {
                                if (command[command.Length - 1] == '"')
                                {
                                    quote_mode = !quote_mode;
                                }
                                command = command.Substring(0, command.Length - 1);
                            }
                            else
                            {
                                if (command[input_pointer - 1] == '"')
                                {
                                    quote_mode = !quote_mode;
                                }
                                command = command.Substring(0, input_pointer - 1) + command.Substring(input_pointer, command.Length - input_pointer);
                            }
                            if (command.Length == 0)
                            {
                                erase_genius_trace(genius, command.Length - input_pointer);
                                genius = "";
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
                        add_key_to_command(ref command, key_input, input_pointer);
                        break;
                    }
                    break;
                case (ConsoleKey.OemComma):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.OemPeriod):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.OemPlus):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.OemMinus): //- ou _ sur QWERTY
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem1):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem2):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem3):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem4):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem5):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem6):
                    if (key_input.KeyChar == '\0')
                    {
                        input_pointer--; //^ ¨ s'affichent uniquement sur le prochain caractère
                        return (0);
                    }
                    else
                    {
                        add_key_to_command(ref command, key_input, input_pointer);
                    }
                    break;
                case (ConsoleKey.Oem7):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem8):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem102):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case(ConsoleKey.F1):
                    Console.CursorVisible = false;
                    List<string> all_arg_current_command = new List<string>();
                    int i = command.Length - 1;
                    if(command.Length == 0)
                        all_arg_current_command.Add("");
                    else
                    {
                        while (i >= 0 && command[i] != ';')
                            i--;
                        if (i >= command.Length - 1)
                        {
                            i--;
                            while (i >= 0 && command[i] != ';')
                                i--;
                            all_arg_current_command = Interpreter.extract_words(command.Substring(i + 1, command.Length - i - 2));
                        }
                        else
                            all_arg_current_command = Interpreter.extract_words(command.Substring(i + 1, command.Length - i - 1));
                    }
                    if (Static_data.is_known_cmd(all_arg_current_command[0], ShellEnvironment.appdata_dir))
                        Execution.f1_help_access(ShellEnvironment.appdata_dir, all_arg_current_command[0]);
                    else
                        Execution.f1_help_access(ShellEnvironment.appdata_dir);
                    Console.CursorVisible = true;
                        input_pointer--;
                        line = 0;
                        Console.SetCursorPosition(0, 0);
                        print_cmd_intro_line();
                        Console.Write(command);
                        Interpreter.print_genius_cmd(genius, current_input, ref max_cursor);
                        break;    
                case (ConsoleKey.F4):
                        if (key_input.Modifiers == ConsoleModifiers.Alt)
                        {
                            Execution.execute_input(new string[1][] { new string[1] { "exit" } });
                            input_pointer--;
                            return (0);
                        }
                        input_pointer--;
                        break;
                default:
                    input_pointer--;
                    break;
            }

            return (command.Length - delta);
        }

        public static void display_update(string command, int line_beginning, int line, int input_pointer, int delta_input, ConsoleKeyInfo key_pressed)
        {
            Console.CursorVisible = false;
            Console.SetCursorPosition(line_beginning, line);
            if(key_pressed.Key == ConsoleKey.DownArrow || key_pressed.Key == ConsoleKey.UpArrow)
                display_up_down_arrow(command, input_pointer, delta_input);
            else if(delta_input > 0)
                display_update_adding(command, input_pointer, delta_input);
            else
                display_update_erase(command, input_pointer, delta_input);
            Console.CursorVisible = true;
        }

        static void display_up_down_arrow(string command, int input_pointer, int delta_input)
        {
            Console.Write(command);
            if(delta_input < 0)
            {
                Execution.print_n_space(-1 * delta_input);
                for (int j = 0; j < -1 * delta_input; j++)
                {
                    if (Console.CursorLeft == 0)
                    {
                        Console.SetCursorPosition(Console.WindowWidth - 1, Console.CursorTop);
                        Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - 1);
                    }
                    else
                    {
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    }
                }
            }
        }

        public static void display_update_adding(string command, int input_pointer, int delta_input)
        {
            int i = 0;

            if (input_pointer > 0 && get_last_word_with_quotes(command)[0] != '"')
            {
                while (i < input_pointer - delta_input)
                {
                    if (Console.WindowWidth - Console.CursorLeft < input_pointer - delta_input - i)
                    {
                        i = i + Console.WindowWidth - Console.CursorLeft;
                        Console.SetCursorPosition(0, Console.CursorTop + 1);
                    }
                    else
                    {
                        if (Console.CursorLeft == Console.WindowWidth - 1)
                        {
                            Console.SetCursorPosition(0, Console.CursorTop + 1);
                        }
                        else
                        {
                            Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
                        }
                        i++;
                    }
                }
                Console.Write(command.Substring(input_pointer - delta_input, command.Length - input_pointer + delta_input));
            }
            else
            {
                Console.Write(command);
            }
        }

        public static void display_update_erase(string command, int input_pointer, int delta_input)
        {
            int i = 0;

            if (input_pointer > 0)
            {
                while (i < input_pointer)
                {
                    if (Console.WindowWidth - Console.CursorLeft < input_pointer + delta_input - i)
                    {
                        i = i + Console.WindowWidth - Console.CursorLeft;
                        Console.SetCursorPosition(0, Console.CursorTop + 1);
                    }
                    else
                    {
                        if (Console.CursorLeft == Console.WindowWidth - 1)
                        {
                            Console.SetCursorPosition(0, Console.CursorTop + 1);
                        }
                        else
                        {
                            Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
                        }
                        i++;
                    }
                }
                if(input_pointer == command.Length)
                {
                    Console.Write(command.Substring(input_pointer - 1, command.Length - input_pointer));
                }
                else
                {
                    Console.Write(command.Substring(input_pointer, command.Length - input_pointer));
                }
            }
            else
            {
                Console.Write(command);
            }
            Execution.print_n_space(-1 * delta_input);
            for(int j = 0; j < -1 * delta_input; j++)
            {
                if (Console.CursorLeft == 0)
                {
                    Console.SetCursorPosition(Console.WindowWidth - 1, Console.CursorTop);
                    Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - 1);
                }
                else
                {
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                }
            }
        }

        public static void erase_genius_trace(string genius, int distance_to_end)
        {
            bool cursor_visible = Console.CursorVisible;
            Console.CursorVisible = false;
            cursor_replacement(-1 * (distance_to_end + 1));
            Execution.print_n_space(genius.Length);
            cursor_replacement(distance_to_end + 1 + genius.Length);
            Console.CursorVisible = cursor_visible;
        }

        public static void add_key_to_command(ref string command, ConsoleKeyInfo key_input, int input_pointer)
        {
            if (input_pointer <= command.Length)
            {
                command = command.Substring(0, input_pointer - 1) + key_input.KeyChar + command.Substring(input_pointer - 1, command.Length - input_pointer + 1);
            }
            else
            {
                command = command + key_input.KeyChar;
            }
        }

        public static void cursor_replacement(int delta_input_pointer_command_length)
        {
            if(delta_input_pointer_command_length <= -1) //cas spécial pour faire avancer le curseur d'un cran
            {
                for (int i = 0; i < (-1) * delta_input_pointer_command_length; i++)
                {
                    if (Console.CursorLeft == Console.WindowWidth - 1)
                        Console.SetCursorPosition(0, Console.CursorTop + 1);
                    else
                        Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
                }
            }
            for(int i = 0; i < delta_input_pointer_command_length; i++)
            {
                if (Console.CursorLeft == 0)
                {
                    Console.SetCursorPosition(Console.WindowWidth - 1, Console.CursorTop); //L'opération ne peut être optimisée car le codage de SetCursorPosition ne fait pas le positionnement simultanément
                    Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - 1);
                }
                else
                {
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                }
            }
        }

        public static string get_last_word(string command)
        {
            int i = 0;
            int last_stop;
            bool quote_mode;
            bool finished;
            int last_quote = 0;
            int first_quote = -1;
            string current_word = "";

            while(i < command.Length)
            {
                finished = false;
                quote_mode = false;
                last_stop = i;
                while (!finished)
                {
                    if (command[i] == '"')
                    {
                        if (last_quote == 0)
                        {
                            last_quote = i;
                        }
                        else
                        {
                            first_quote = last_quote;
                            last_quote = i;
                        }
                        quote_mode = !quote_mode;
                    }
                    finished = ((command[i] == ' ' || command[i] == ';') && !quote_mode) || i == command.Length - 1;
                    i++;
                }
                current_word = command.Substring(last_stop, i - last_stop);

                if(current_word[0] == '"' && current_word[current_word.Length - 1] == '"' && current_word.Length > 1)
                {
                    current_word = current_word.Substring(1, current_word.Length - 2);
                }
                else if(current_word[0] == '"')
                {
                    current_word = current_word.Substring(1, current_word.Length - 1);
                }
                else if(current_word[current_word.Length - 1] == ' ' || current_word[current_word.Length - 1] == ';')
                {
                    current_word = "";
                }
            }
            
            return (current_word);
        }

        public static string get_last_word_with_quotes(string command)
        {
            int i = 0;
            int last_stop;
            bool quote_mode;
            bool finished;
            int last_quote = 0;
            int first_quote = -1;
            string current_word = "";

            while (i < command.Length)
            {
                finished = false;
                quote_mode = false;
                last_stop = i;
                while (!finished)
                {
                    if (command[i] == '"')
                    {
                        if (last_quote == 0)
                        {
                            last_quote = i;
                        }
                        else
                        {
                            first_quote = last_quote;
                            last_quote = i;
                        }
                        quote_mode = !quote_mode;
                    }
                    finished = ((command[i] == ' ' || command[i] == ';') && !quote_mode) || i == command.Length - 1;
                    i++;
                }
                current_word = command.Substring(last_stop, i - last_stop);
            }

            return (current_word);
        }

        public static bool contain_space(string word)
        {
            int i = 0;

            while (i < word.Length && word[i] != ' ')
                i++;

            return (i < word.Length);
        }

        static string remove_char_in_string(int index, string command)
        {
            if(index >= 0 && index < command.Length - 1)
            {
                command = command.Substring(0, index) + command.Substring(index + 1, command.Length - index - 1);
            }

            return (command);
        }
    }
}
