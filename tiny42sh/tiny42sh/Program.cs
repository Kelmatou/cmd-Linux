﻿using System;
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
            //generate_init_file("D:\\Info_project\\C#_project\\Apps\\TP16 Shell Linux\\tiny42sh\\tiny42sh\\settings"); //A METTRE EN COMMENTAIRE AVANT COMPILATION!!!!
            cmd_Linux();
        } // /!\ A LIGNE A METTRE EN COMMENTAIRE!

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
                    Console.WriteLine("> cmd Linux: an unhandle error occured");
                    send_email(ex);
                }
            }  
        }

        static void launch_cmd()
        {
            #region variables
            bool working = true;
            bool superuser = false;
            bool script_enable = true;
            bool genius_enable = true;
            bool private_mode = false;
            bool auto_lock = false;
            bool auto_log = false;
            bool cmd_print_time = false;
            bool cmd_print_user = true;
            bool cmd_print_path = true;
            int max_genius_data = 10000; // = unlimited
            int refresh_timer = 50;
            string command = "";
            Directory.SetCurrentDirectory(find_desktop());
            string desktop_dir = Directory.GetCurrentDirectory();
            string appdata_dir = find_appdata();
            string[] previous_directories = new string[256];
            string[] stack_input = new string[256];
            for (int i = 0; i < 256; i++)
            {
                stack_input[i] = "";
                previous_directories[i] = "";
            }
            previous_directories[0] = Directory.GetCurrentDirectory();
            get_settings_option(ref script_enable, ref refresh_timer, ref auto_lock, ref genius_enable, ref private_mode, ref auto_log, ref cmd_print_user, ref cmd_print_path, ref cmd_print_time, ref max_genius_data, appdata_dir);
            List<Genius_data> all_genius_data = extract_genius_data(appdata_dir, max_genius_data);
            int stack_pointer = 0;
            int previous_directory_pointer = -1;
            long last_result = 0;
            List<string> all_cmd = init_genius_cmd(appdata_dir);
            #endregion
            
            if(auto_lock || auto_log)
            {
                if(get_admin_password(ref command, appdata_dir) == 0)
                {
                    command = "lock " + command;
                    Execution.execute_input(Interpreter.parse_input(command, appdata_dir), desktop_dir, appdata_dir, ref working, ref superuser, ref last_result, ref all_genius_data, ref script_enable, ref genius_enable, ref private_mode, ref refresh_timer, ref auto_lock, ref auto_log, ref cmd_print_user, ref cmd_print_path, ref cmd_print_time, ref previous_directories, ref previous_directory_pointer, ref max_genius_data);
                    if(auto_log)
                    {
                        superuser = true;
                    }
                }
                else
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("> cmd Linux: auto_lock: session was blocked cause password was unsafe.");
                    Console.WriteLine("> cmd Linux: auto_lock: try to reinstall cmd Linux...");
                    Console.ResetColor();
                    Thread.Sleep(5000);
                    working = false;
                }
            }

            while(working)
            {
                print_cmd_intro_line(cmd_print_time, cmd_print_user, cmd_print_path, superuser);

                while(!(File.Exists(appdata_dir + "/script") && script_enable) && !Console.KeyAvailable)
                {
                    Thread.Sleep(refresh_timer);
                }

                if (File.Exists(appdata_dir + "/script") && script_enable && !Console.KeyAvailable)
                {
                    try
                        {
                            StreamReader reader = new StreamReader(appdata_dir + "/script");
                            command = reader.ReadLine();
                            reader.Close();
                            erase_script_line(appdata_dir + "/script");
                            string[][] words = Interpreter.parse_input(command, appdata_dir);
                            if (Console.CursorTop == Console.BufferHeight - 1)
                            {
                                if (Console.BufferHeight >= 32700)
                                {
                                    Console.Clear();
                                }
                                else
                                {
                                    Console.SetBufferSize(Console.BufferWidth, Console.BufferHeight + 300);
                                }
                            }
                            Console.SetCursorPosition(0, Console.CursorTop + 1);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("\n> cmd linux: script: access denied! (execute it with admin rights)");
                        }
                }
                else if(File.Exists(appdata_dir + "/script") && script_enable && Console.KeyAvailable)
                {
                    ConsoleKeyInfo keypressed = Console.ReadKey(true);
                            if(keypressed.Key == ConsoleKey.Escape)
                            {
                                command = "";
                                try
                                {
                                    File.Delete(appdata_dir + "/script");
                                    Console.WriteLine("\n> cmd linux: script execution was canceled successfully.");
                                }
                                catch (Exception)
                                {
                                    script_enable = superuser;
                                    Console.WriteLine("\n> cmd linux: script execution was canceled but couldn't be completely stop. (execute it with admin rights)");
                                    if(!superuser)
                                    {
                                        Console.WriteLine("> cmd linux: scripts are disabled. Log you as MASTER to enable script");
                                    }
                                }
                            }
                            else
                            {
                                try
                                {
                                    StreamReader reader = new StreamReader(appdata_dir + "/script");
                                    command = reader.ReadLine();
                                    reader.Close();
                                    erase_script_line(appdata_dir + "/script");
                                    Console.SetCursorPosition(0, Console.CursorTop);
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("\n> cmd linux: script: access denied! (execute it with admin rights)");
                                }
                            }
                }
                else
                {
                    command = get_input(superuser, ref stack_input, ref stack_pointer, ref all_genius_data, appdata_dir, genius_enable, private_mode, all_cmd, ref working, cmd_print_time, cmd_print_user, cmd_print_path);
                }
                if(command != "")
                {
                    /*if(command.Length > 8 && command.Substring(0,9) == "/feedback")
                    {
                        Computer my_computer = new Computer();
                        send_feedback("Feedback from " + Environment.UserName + " on " + my_computer.Info.OSFullName + " with cmd Linux V." + Execution.version);
                    }
                    else
                    {
                        Execution.execute_input(Interpreter.parse_input(command, appdata_dir), desktop_dir, appdata_dir, ref working, ref superuser, ref last_result, ref all_genius_data, ref script_enable, ref genius_enable, ref private_mode, ref refresh_timer, ref auto_lock, ref auto_log, ref cmd_print_user, ref cmd_print_path, ref cmd_print_time, ref previous_directories, ref previous_directory_pointer);
                    }*/
                    Execution.execute_input(Interpreter.parse_input(command, appdata_dir), desktop_dir, appdata_dir, ref working, ref superuser, ref last_result, ref all_genius_data, ref script_enable, ref genius_enable, ref private_mode, ref refresh_timer, ref auto_lock, ref auto_log, ref cmd_print_user, ref cmd_print_path, ref cmd_print_time, ref previous_directories, ref previous_directory_pointer, ref max_genius_data);
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
                {
                    last_stop = i;
                }
                i--;
            }

            if(i > 0)
            {
                if(last_stop == directory.Length - 1)
                {
                    directory = "..." + directory.Substring(directory.Length - 27, 27);
                }
                else
                {
                    directory = directory.Substring(last_stop, directory.Length - last_stop);
                }
            }

            Console.Write("~ ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(directory);
            Console.ResetColor();
        }

        static void print_user(bool is_admin)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write(System.Environment.UserName);
            if(is_admin)
            {
                Console.Write(" (MASTER)");
            }
            Console.ResetColor();
        }

        static void print_cmd_intro_line(bool cmd_print_time, bool cmd_print_user, bool cmd_print_path, bool superuser)
        {
            Console.Write("> ");
            if (cmd_print_time)
            {
                Console.Write("[" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "]");
            }
            if (cmd_print_user)
            {
                if (cmd_print_time)
                {
                    Console.Write(" ");
                }
                print_user(superuser);
            }
            if (cmd_print_path)
            {
                if (cmd_print_time || cmd_print_user)
                {
                    Console.Write(" ");
                }
                print_directory();
            }
            if (cmd_print_user || cmd_print_path || cmd_print_time)
            {
                Console.Write(": ");
            }
        }

        public static void admin_connection_protocol(ref bool superuser, string appdata_dir)
        {
            string username = Environment.UserName;
            string username_password = "";
            string password = "";
            string answer = "";
            string answer2 = "";
            int line = Console.CursorTop;

            if(decrypt_password(ref username_password,ref password, appdata_dir, ref superuser) && !superuser)
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

                if (answer != username || answer2 != password || username != username_password)
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

        static void send_email(Exception ex)
        {
            string answer = "";

            Console.Write("> cmd Linux: send feedback? (Y/N)\n> ");
            answer = Console.ReadLine();
            
            if (answer == "Y" || answer == "y")
            {
                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com", 587);

                    Console.Write("> cmd Linux: Describe the situation just before the bug:\n> ");
                    answer = Console.ReadLine();

                    mail.From = new MailAddress("1magnum78630@gmail.com");
                    mail.To.Add("magnum78630@gmail.com");
                    mail.Subject = "cmd Linux: Bug report";
                    mail.Body = "> cmd Linux: an unhandle error occured!\n> Description: " + answer + "\n> Bug report details: \n> " + ex + "\n> cmd Linux version: " + Execution.version;

                    SmtpServer.Credentials = new System.Net.NetworkCredential("1magnum78630@gmail.com", "oceandrive");
                    SmtpServer.EnableSsl = true;

                    SmtpServer.Send(mail);
                    Console.WriteLine("> cmd Linux: Report sent, thank you!");  
                    
                }
                catch (Exception)
                {
                    if(NetworkInterface.GetIsNetworkAvailable())
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
                mail.From = new MailAddress("1magnum78630@gmail.com");
                // Console.Write("> cmd Linux: Enter your email password: ");
                //password = Console.ReadLine();
                mail.To.Add("magnum78630@gmail.com");
                mail.Subject = "cmd Linux: Feedback";
                if (display_message)
                {
                    Console.Write("> cmd Linux: Enter your message:\n> ");
                    answer = Console.ReadLine();
                }
                mail.Body = "> cmd Linux: Feedback: " + answer + "\n> cmd Linux details: \n> " + message;

                SmtpServer.Credentials = new System.Net.NetworkCredential("1magnum78630@gmail.com", "oceandrive");
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

        //INIT FUNCTION

        static string find_desktop()
        {
            char racine = 'A';
            int i = 0;

            while (i < 26 && !Directory.Exists(racine + ":/Users/" + Environment.UserName + "/Desktop/"))
            {
                i++;
                racine = (char)(racine + 1);
            }

            if(26 == i)
            {
                return (Directory.GetCurrentDirectory());
            }
            else
            {
                return (racine + ":/Users/" + Environment.UserName + "/Desktop/");
            }
        }

        static string find_appdata()
        {
            char racine = 'A';
            int i = 0;

            while (i < 26 && !Directory.Exists(racine + ":/Users/" + Environment.UserName + "/AppData/Roaming/Kelmatou Apps/cmd Linux/"))
            {
                i++;
                racine = (char)(racine + 1);
            }

            if (26 == i)
            {
                racine = 'A';
                i = 0;
                while (i < 26 && !Directory.Exists(racine + ":/"))
                {
                    i++;
                    racine = (char)(racine + 1);
                }
                
                if(i == 26)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("> cmd Linux: FATAL ERROR: can't find application ressources!");
                    Console.Write("> cmd Linux: try to reinstall the application");
                    return ("");
                }
                else
                {
                    try
                    {
                        Directory.CreateDirectory(racine + ":/Users/" + Environment.UserName + "/AppData/Roaming/Kelmatou Apps/cmd Linux/");
                        Directory.CreateDirectory(racine + ":/Users/" + Environment.UserName + "/AppData/Roaming/Kelmatou Apps/cmd Linux/genius_data");
                        string appdata_dir = racine + ":/Users/" + Environment.UserName + "/AppData/Roaming/Kelmatou Apps/cmd Linux/";
                        StreamWriter eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_bing");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_cat");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_cd");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_cp");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_cpdir");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_echo");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_facebook");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_google");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_launch");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_lolapp");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_ls");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_map");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_mkdir");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_rm");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_mv");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_mvdir");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_news");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_rename");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_rmdir");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_search");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_textedit");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_time");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_touch");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_translate");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_twitter");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_url");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_wait");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_wikipedia");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_yahoo");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_youtube");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/skype_location");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/ts_location");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/lolapp_location");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/shutdown.bat");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/cmd.bat");
                        eraser.Close();
                        return (appdata_dir);
                    }
                    catch(Exception)
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write("> cmd Linux: FATAL ERROR: can't find application ressources!");
                        Console.Write("> cmd Linux: try to reinstall the application");
                        return ("");
                    }
                }
            }
            else
            {
                return (racine + ":/Users/" + Environment.UserName + "/AppData/Roaming/Kelmatou Apps/cmd Linux/");
            }
        }

        static void get_settings_option(ref bool script_enable, ref int refresh_timer, ref bool auto_lock, ref bool genius_enable, ref bool private_mode, ref bool auto_log, ref bool cmd_print_user, ref bool cmd_print_path, ref bool cmd_print_time, ref int max_genius_data, string appdata_dir)
        {
            List<string> content = new List<string>();
            string line;

            if (File.Exists(appdata_dir + "/option"))
            {
                StreamReader reader = new StreamReader(appdata_dir + "/option");
                line = reader.ReadLine();

                while (line != null && content.Count < 10)
                {
                    content.Add(line);
                    line = reader.ReadLine();
                }

                reader.Close();
            }

            if (content.Count == 10)
            {
                if (content[0].Length >= 11)
                {
                    try
                    {
                        refresh_timer = Convert.ToInt32(content[0].Substring(8, content[0].Length - 10));
                    }
                    catch (Exception)
                    {
                        refresh_timer = 50;
                    }
                }
                if (content[1].Length > 12)
                {
                    cmd_print_user = content[1][12] == 'T';
                }
                if (content[2].Length > 17)
                {
                    cmd_print_path = content[2][17] == 'T';
                }
                if (content[3].Length > 12)
                {
                    cmd_print_time = content[3][12] == 'T';
                }
                if (content[4].Length > 15)
                {
                    script_enable = content[4][15] == 'T';
                }
                if (content[5].Length > 11)
                {
                    auto_lock = content[5][11] == 'T';
                }
                if (content[6].Length > 10)
                {
                    auto_log = content[6][10] == 'T';
                }
                if (content[7].Length > 15)
                {
                    genius_enable = content[7][15] == 'T';
                }
                if (content[8].Length >= 18)
                {
                    try
                    {
                        max_genius_data = Convert.ToInt32(content[8].Substring(17, content[8].Length - 17));
                    }
                    catch (Exception)
                    {
                        max_genius_data = 10000;
                    }
                }
                if (content[9].Length > 14)
                {
                    private_mode = content[8][14] == 'T';
                }
            }
        }

        static List<string> init_genius_cmd(string appdata_dir)
        {
            List<string> all_cmd = Static_data.cmd_linux_commands();
            List<int> all_cmd_usage = new List<int>();
            List<string> all_content = new List<string>();
            for(int i = 0; i < all_cmd.Count; i++)
            {
                all_cmd_usage.Add(-1);
            }
            string line;
            int start_index;

            if(File.Exists(appdata_dir + "/genius_data/genius_data_cmd"))
            {
                try
                {
                    StreamReader data_reader = new StreamReader(appdata_dir + "/genius_data/genius_data_cmd");
                    line = data_reader.ReadLine();

                    while(line != null)
                    {
                        all_content.Add(line);
                        line = data_reader.ReadLine();
                    }

                    data_reader.Close();

                    for (int i = 0; i < all_content.Count; i++)
                    {
                        start_index = 1;
                        while(all_content[i][start_index] != ':')
                        {
                            start_index++;
                        }
                        start_index++;

                        Static_data.match_cmd_with_cmd_linux_command(ref all_cmd_usage, all_content[i].Substring(0, start_index - 1), Convert.ToInt32(all_content[i].Substring(start_index, all_content[i].Length - start_index)));
                    }

                    if(!all_genius_cmd_found(ref all_cmd_usage))
                    {
                        reinit_file_genius_data_cmd(appdata_dir, all_cmd, all_cmd_usage);
                    }

                    all_cmd = sort_genius_cmd(all_cmd, all_cmd_usage);
                    return (all_cmd);
                }
                catch(Exception)
                {

                }
            }

            all_genius_cmd_found(ref all_cmd_usage);
            reinit_file_genius_data_cmd(appdata_dir, all_cmd, all_cmd_usage);

            return (all_cmd);
        }
        
        //KEYBOARD FUNCTION

        static string get_input(bool superuser, ref string[] stack_input, ref int stack_pointer, ref List<Genius_data> all_genius_data, string appdata_dir, bool genius_enable, bool private_mode, List<string> all_keywords, ref bool working, bool cmd_print_time, bool cmd_print_user, bool cmd_print_path)
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
                {
                    Console.Clear();
                }
                else
                {
                    Console.SetBufferSize(Console.BufferWidth, Console.BufferHeight + 300);
                }
            }
            while(key_input.Key != ConsoleKey.Enter && working)
            {
                key_input = Console.ReadKey(true);
                delta_input = update_input(ref command, key_input, ref stack_input, ref stack_pointer, ref genius, ref input_pointer, ref quote_mode, ref working, ref line, cmd_print_time, cmd_print_user, cmd_print_path, superuser, all_genius_data, appdata_dir, all_keywords, last_word, ref max_cursor);
                if(delta_input != 0 || key_input.Key == ConsoleKey.UpArrow || key_input.Key == ConsoleKey.DownArrow)
                {
                    erase_genius_trace(genius, command.Length - input_pointer);
                    last_word = get_last_word(command);
                    display_update(command, line_beginning, line, input_pointer, delta_input, key_input);
                    if (key_input.Key != ConsoleKey.Enter && (genius.Length <= last_word.Length || genius.Substring(0, last_word.Length) != last_word) && key_input.KeyChar != '"' && command.Length > 0 && command[command.Length - 1] != '"' && genius_enable)
                    {
                        genius = Interpreter.genius_cmd(last_word, Interpreter.get_arg_expected_code(command), all_genius_data, command, appdata_dir, all_keywords);
                    }
                    else if (last_word == "")
                    {
                        genius = "";
                    }
                    if (key_input.Key != ConsoleKey.Enter && key_input.KeyChar != '"' && genius_enable && ((command.Length > 0 && command[command.Length - 1] != '"') || quote_mode))
                    {
                        Interpreter.print_genius_cmd(genius, last_word, ref max_cursor);
                    }
                    else
                    {
                        max_cursor = Console.CursorTop;
                    }
                    cursor_replacement(command.Length - input_pointer);
                }
            }
            Console.SetCursorPosition(0, max_cursor + 1);
            if(!private_mode)
            {
                if (stack_pointer == 256)
                {
                    stack_pointer--;
                    for(int i = 1; i < 256;i++)
                    {
                        stack_input[i - 1] = stack_input[i];
                    }
                }
                stack_input[stack_pointer] = command;
                stack_pointer++;
            }
            
            return (command);
        }

        static int update_input(ref string command, ConsoleKeyInfo key_input, ref string[] stack_input, ref int stack_pointer, ref string genius, ref int input_pointer, ref bool quote_mode, ref bool working, ref int line, bool cmd_print_time, bool cmd_print_user, bool cmd_print_path, bool superuser, List<Genius_data> all_genius_data, string appdata_dir, List<string> all_keywords, string current_input, ref int max_cursor)
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
                                {
                                    input_pointer = command.Substring(0, command.Length - get_last_word_with_quotes(command).Length).Length + 1;
                                }
                                command = command.Substring(0, command.Length - get_last_word_with_quotes(command).Length);
                            }
                            if (input_pointer < 1)
                            {
                                input_pointer = 1;
                            }
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
                                {
                                    quote_mode = !quote_mode;
                                }
                                command = command.Substring(0, command.Length - 1);
                                input_pointer--;
                            }
                            else
                            {
                                if (input_pointer >= 2)
                                {
                                    if (command[input_pointer - 2] == '"')
                                    {
                                        quote_mode = !quote_mode;
                                    }
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
                        genius_linux = Interpreter.genius_linux(get_last_word(command), Interpreter.get_arg_expected_code(command), all_genius_data, command, appdata_dir, all_keywords);
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
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.L):
                    if (key_input.Modifiers == ConsoleModifiers.Control)
                    {
                        bool arg_bool = true;
                        long arg_long = 0;
                        List<Genius_data> arg_genius_data = new List<Genius_data>();
                        int arg_int = 0;
                        string[] arg_tab = new string[0];
                        Execution.execute_input(new string[1][] { new string[1] { "lock" } }, "", "", ref working, ref arg_bool, ref arg_long, ref arg_genius_data, ref arg_bool, ref arg_bool, ref arg_bool, ref arg_int, ref arg_bool, ref arg_bool, ref arg_bool, ref arg_bool, ref arg_bool, ref arg_tab, ref arg_int, ref arg_int);
                        input_pointer--;
                        line = 0;
                        Console.SetCursorPosition(0, 0);
                        print_cmd_intro_line(cmd_print_time, cmd_print_user, cmd_print_path, superuser);
                        Console.Write(command);
                        Interpreter.print_genius_cmd(genius, current_input, ref max_cursor);
                    }
                    else
                    {
                        add_key_to_command(ref command, key_input, input_pointer);
                    }
                    break;
                case (ConsoleKey.M):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.N):
                    if (key_input.Modifiers == ConsoleModifiers.Control)
                    {
                        bool arg_bool = true;
                        long arg_long = 0;
                        List<Genius_data> arg_genius_data = new List<Genius_data>();
                        int arg_int = 0;
                        string[] arg_tab = new string[0];
                        Execution.execute_input(new string[1][] { new string[1] { "launch" } }, "", "", ref working, ref arg_bool, ref arg_long, ref arg_genius_data, ref arg_bool, ref arg_bool, ref arg_bool, ref arg_int, ref arg_bool, ref arg_bool, ref arg_bool, ref arg_bool, ref arg_bool, ref arg_tab, ref arg_int, ref arg_int);
                        input_pointer--;
                        return (0);
                    }
                    else
                    {
                        add_key_to_command(ref command, key_input, input_pointer);
                    }
                    break;
                case (ConsoleKey.O):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.P):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.Q):
                    add_key_to_command(ref command, key_input, input_pointer);
                    break;
                case (ConsoleKey.R):
                    if (key_input.Modifiers == ConsoleModifiers.Control)
                    {
                        bool arg_bool = true;
                        long arg_long = 0;
                        List<Genius_data> arg_genius_data = new List<Genius_data>();
                        int arg_int = 0;
                        string[] arg_tab = new string[0];
                        Execution.execute_input(new string[2][] { new string[1] { "launch" }, new string[1] { "exit" } }, "", "", ref working, ref arg_bool, ref arg_long, ref arg_genius_data, ref arg_bool, ref arg_bool, ref arg_bool, ref arg_int, ref arg_bool, ref arg_bool, ref arg_bool, ref arg_bool, ref arg_bool, ref arg_tab, ref arg_int, ref arg_int);
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
                    if (command.Length == 0 || (input_pointer == 1 || (input_pointer > 1 && command[input_pointer - 2] == ';')))
                    {
                        input_pointer--;
                        return (0);
                    }
                    else
                    {
                        add_key_to_command(ref command, key_input, input_pointer);
                    }
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
                    if (stack_pointer > 0)
                    {
                        erase_genius_trace(genius, command.Length - input_pointer);
                        stack_pointer--;
                        command = stack_input[stack_pointer];
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
                    if (stack_pointer < 255)
                    {
                        erase_genius_trace(genius, command.Length - input_pointer);
                        stack_pointer++;
                        command = stack_input[stack_pointer];
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
                    if (key_input.KeyChar == ';' && (input_pointer == 1 || (input_pointer > 1 && command[input_pointer - 2] == ';')))
                    {
                        input_pointer--;
                        return (0);
                    }
                    else
                    {
                        add_key_to_command(ref command, key_input, input_pointer);
                    }
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
                    {
                        all_arg_current_command.Add("");
                    }
                    else
                    {
                        while (i >= 0 && command[i] != ';')
                        {
                            i--;
                        }
                        if (i >= command.Length - 1)
                        {
                            i--;
                            while (i >= 0 && command[i] != ';')
                            {
                                i--;
                            }
                            all_arg_current_command = Interpreter.extract_words(command.Substring(i + 1, command.Length - i - 2));
                        }
                        else
                        {
                            all_arg_current_command = Interpreter.extract_words(command.Substring(i + 1, command.Length - i - 1));
                        }
                    }
                    if(Static_data.is_known_cmd(all_arg_current_command[0], appdata_dir))
                    {
                        Execution.f1_help_access(appdata_dir, all_arg_current_command[0]);
                    }
                    else
                    {
                            Execution.f1_help_access(appdata_dir);
                    }
                    Console.CursorVisible = true;
                        input_pointer--;
                        line = 0;
                        Console.SetCursorPosition(0, 0);
                        print_cmd_intro_line(cmd_print_time, cmd_print_user, cmd_print_path, superuser);
                        Console.Write(command);
                        Interpreter.print_genius_cmd(genius, current_input, ref max_cursor);
                        break;    
                case (ConsoleKey.F4):
                        if (key_input.Modifiers == ConsoleModifiers.Alt)
                        {
                            bool arg_bool = true;
                            long arg_long = 0;
                            List<Genius_data> arg_genius_data = new List<Genius_data>();
                            int arg_int = 0;
                            string[] arg_tab = new string[0];
                            Execution.execute_input(new string[1][] { new string[1] { "exit" } }, "", "", ref working, ref arg_bool, ref arg_long, ref arg_genius_data, ref arg_bool, ref arg_bool, ref arg_bool, ref arg_int, ref arg_bool, ref arg_bool, ref arg_bool, ref arg_bool, ref arg_bool, ref arg_tab, ref arg_int, ref arg_int);
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

        static void display_update(string command, int line_beginning, int line, int input_pointer, int delta_input, ConsoleKeyInfo key_pressed)
        {
            Console.CursorVisible = false;
            Console.SetCursorPosition(line_beginning, line);
            if(key_pressed.Key == ConsoleKey.DownArrow || key_pressed.Key == ConsoleKey.UpArrow)
            {
                display_up_down_arrow(command, input_pointer, delta_input);
            }
            else if(delta_input > 0)
            {
                display_update_adding(command, input_pointer, delta_input);
            }
            else
            {
                display_update_erase(command, input_pointer, delta_input);
            }
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

        static void display_update_adding(string command, int input_pointer, int delta_input)
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

        static void display_update_erase(string command, int input_pointer, int delta_input)
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

        static void erase_genius_trace(string genius, int distance_to_end)
        {
            bool cursor_visible = Console.CursorVisible;
            Console.CursorVisible = false;
            cursor_replacement(-1 * (distance_to_end + 1));
            Execution.print_n_space(genius.Length);
            cursor_replacement(distance_to_end + 1 + genius.Length);
            Console.CursorVisible = cursor_visible;
        }

        static void add_key_to_command(ref string command, ConsoleKeyInfo key_input, int input_pointer)
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

        static void cursor_replacement(int delta_input_pointer_command_length)
        {
            if(delta_input_pointer_command_length <= -1) //cas spécial pour faire avancer le curseur d'un cran
            {
                for (int i = 0; i < (-1) * delta_input_pointer_command_length; i++)
                {
                    if (Console.CursorLeft == Console.WindowWidth - 1)
                    {
                        Console.SetCursorPosition(0, Console.CursorTop + 1);
                    }
                    else
                    {
                        Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
                    }
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

        static string get_last_word(string command)
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

        static string get_last_word_with_quotes(string command)
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

        static bool contain_space(string word)
        {
            int i = 0;

            while (i < word.Length && word[i] != ' ')
            {
                i++;
            }

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

        static List<Genius_data> extract_genius_data(string appdata_dir, int max_genius_data)
        {
            List<Genius_data> all_genius_data = new List<Genius_data>();
            string[] all_genius_files = new string[30] {"genius_data_bing", "genius_data_cat", "genius_data_cd", "genius_data_cp", "genius_data_cpdir", "genius_data_echo", "genius_data_facebook", "genius_data_find", "genius_data_google", "genius_data_launch", "genius_data_lolapp", "genius_data_ls", "genius_data_map", "genius_data_mkdir", "genius_data_mv", "genius_data_mvdir", "genius_data_news", "genius_data_rename", "genius_data_rm", "genius_data_rmdir", "genius_data_textedit", "genius_data_time", "genius_data_touch", "genius_data_translate", "genius_data_twitter", "genius_data_url", "genius_data_wait", "genius_data_wikipedia", "genius_data_yahoo", "genius_data_youtube" };

            for (int i = 0; i < all_genius_files.Length; i++)
            {
                if (File.Exists(appdata_dir + "/genius_data/" + all_genius_files[i]))
                {
                    StreamReader data_reader = new StreamReader(appdata_dir + "/genius_data/" + all_genius_files[i]);
                    string current_word = data_reader.ReadLine();

                    while (current_word != null)
                    {
                        all_genius_data.Add(new Genius_data(all_genius_files[i].Substring(12,all_genius_files[i].Length - 12),current_word));
                        current_word = data_reader.ReadLine();
                    }

                    data_reader.Close();
                }
            }  
            
            return (all_genius_data);
        }

        public static void erase_script_line(string script_path)
        {
            List<string> script_content = new List<string>();
            StreamReader reader = new StreamReader(script_path);
            string next_line = reader.ReadLine();
            
            while(next_line != null)
            {
                script_content.Add(next_line);
                next_line = reader.ReadLine();
            }

            reader.Close();
            
            if(script_content.Count == 1)
            {
                File.Delete(script_path);
            }
            else
            {
                StreamWriter writer = new StreamWriter(script_path);

                for(int i = 1; i < script_content.Count; i++)
                {
                    writer.WriteLine(script_content[i]);
                }

                writer.Close();
            }
        }

        //SECURITY FUNCTION

        public static bool is_good_init_code(string appdata_dir)
        {
            try
            {
                StreamReader check = new StreamReader(appdata_dir + "/settings");
                string current_line = check.ReadLine();
                int i = 0;
                
                while(i < Execution.version.Length && current_line != null)
                {
                    if(Char.IsNumber(Execution.version[i]))
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
                        {
                            i = Execution.version.Length + 1;
                        }
                    }
                    i++;
                }

                check.Close();
                while(i < Execution.version.Length && !(Char.IsNumber(Execution.version[i])))
                {
                    i++;
                }

                return (i == Execution.version.Length && current_line == null);
            }
            catch(Exception)
            {
                Console.WriteLine("> cmd Linux: Unable to verify valid security. Try to execute cmd linux with admin rights.");
                return (false);
            }
        }

        private static void generate_init_file(string file_path)
        {
            StreamWriter init = new StreamWriter(file_path);

            for(int i = 0; i < Execution.version.Length; i++)
            {
                if(Char.IsNumber(Execution.version[i]))
                {
                    for (int j = 0; j < Convert.ToInt32(Execution.version[i].ToString()); j++)
                    {
                        if(j>4)
                        {
                            init.Write("\t");
                        }
                        else
                        {
                            init.Write(" ");
                        }
                    }
                    init.WriteLine();
                }
            }

            init.Close();
        }

        public static char encrypt_time(int time)
        {
            switch(time)
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
                    return(' ');
            }
        }

        public static int decrypt_time(char crypted)
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

        public static string cryptage_userName(string userName, int time)
        {
            string userName_crypted = "";

            for(int i = 0; i < userName.Length; i++)
            {
                userName_crypted = userName_crypted + (char)(userName[i]^time);
            }

            return (userName_crypted);
        }

        public static string encrypt_security_code()
        {
            string security_code = DateTime.Now.Year + (DateTime.Now.Month + 100).ToString().Substring(1, 2) + (DateTime.Now.Day + 100).ToString().Substring(1, 2) + (DateTime.Now.Hour + 100).ToString().Substring(1, 2) + (DateTime.Now.Minute + 100).ToString().Substring(1, 2) + (DateTime.Now.Second + 100).ToString().Substring(1, 2);
            string encryptage = "";
            Random rnd = new Random();
            int key = rnd.Next(0, 10);

            for(int i = 0; i < security_code.Length; i++)
            {
                encryptage = encryptage + ((int)security_code[i] + security_code.Length).ToString();
            }

            for (int i = 0; i < key; i++)
            {
                encryptage = encryptage[encryptage.Length - 1] + encryptage.Substring(0, encryptage.Length - 1);
            }
            encryptage = encryptage + (char)(key + encryptage.Length + 42);

            return (encryptage);
        }

        public static string decrypt_security_code(string security_code)
        {
            string decrypted_security_code = "";
            int key = (int)(security_code[security_code.Length - 1] - 41 - security_code.Length);
            security_code = security_code.Substring(0, security_code.Length - 1);

            for(int i = 0; i < key; i++)
            {
                security_code = security_code.Substring(1, security_code.Length - 1) + security_code[0];
            }

            for(int i = 0;  i < security_code.Length / 2; i++)
            {
                try
                {
                    decrypted_security_code = decrypted_security_code + (char)(Convert.ToInt32(security_code.Substring(i * 2, 2)) - security_code.Length / 2);
                }
                catch(Exception)
                {
                    Console.WriteLine("> cmd Linux: Invalid security code");
                    security_code = "";
                    decrypted_security_code = "";
                }
            }

            return (decrypted_security_code);
        }

        public static bool encrypt_password(string new_password, string appdata_dir)
        {
            string password_crypter = "";
            int time = DateTime.Now.Minute % 10;

            if(new_password.Length > 0)
            {
                for (int i = 0; i < new_password.Length; i++)
                {
                    if (new_password[i] <= 126 && new_password[i] >= 33)
                    {
                        password_crypter = password_crypter + (char)(((new_password[i] + i + time) % 94) + 33);
                    }
                    else
                    {
                        password_crypter = password_crypter + new_password[i];
                    }
                }
                password_crypter = password_crypter + encrypt_time(time);

                try
                {
                    StreamWriter save = new StreamWriter(appdata_dir + "/settings");

                    save.WriteLine(encrypt_security_code());
                    save.WriteLine(cryptage_userName(Environment.UserName, time));
                    save.WriteLine(password_crypter);

                    save.Close();

                    Console.WriteLine("> password: new password saved!");
                }
                catch (Exception)
                {
                    Console.WriteLine("> password: unable to save new password. Try to execute cmd linux with admin rights");
                    return (false);
                }
            }
            else
            {
                return (false);
            }

            return (true);
        }

        public static bool decrypt_password(ref string userName, ref string password, string appdata_dir, ref bool superuser)
        {
            string userName_crypted;
            string password_crypted;
            string security_code_crypted;
            int time = 0;
            int new_char;
            password = "";

            if(File.Exists(appdata_dir + "/settings"))
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

            if(security_code_crypted != null && security_code_crypted[0] != ' ' && userName_crypted != null && password_crypted != null && password_crypted.Length > 1)
            {
                if (decrypt_security_code(security_code_crypted) == (File.GetLastWriteTime(appdata_dir + "/settings").Year + (File.GetLastWriteTime(appdata_dir + "/settings").Month + 100).ToString().Substring(1, 2) + (File.GetLastWriteTime(appdata_dir + "/settings").Day + 100).ToString().Substring(1, 2) + (File.GetLastWriteTime(appdata_dir + "/settings").Hour + 100).ToString().Substring(1, 2) + (File.GetLastWriteTime(appdata_dir + "/settings").Minute + 100).ToString().Substring(1, 2) + (File.GetLastWriteTime(appdata_dir + "/settings").Second + 100).ToString().Substring(1, 2)))
                {
                    time = decrypt_time(password_crypted[password_crypted.Length - 1]);
                    if (time > -1)
                    {
                        for (int i = 0; i < password_crypted.Length - 1; i++)
                        {
                            if (password_crypted[i] <= 126 && password_crypted[i] >= 33)
                            {
                                new_char = password_crypted[i] - 33;
                                if (new_char - i - time < 33)
                                {
                                    new_char = 94 + new_char;
                                }
                                new_char = new_char - i - time;
                                password = password + (char)new_char;
                            }
                            else
                            {
                                password = password + password_crypted[i];
                            }
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
                if(is_good_init_code(appdata_dir))
                {
                    long arg_needed = 0;
                    int arg_needed3 = 0;
                    string[] arg_needed4 = new string[0];
                    superuser = true;
                    List<Genius_data> arg_needed2 = new List<Genius_data>();
                    Console.WriteLine("> cmd linux: initialisation of your password: ");
                    if(!((Execution.execute_input(new string[1][] { new string[1] { "password" } }, "", appdata_dir, ref superuser, ref superuser, ref arg_needed, ref arg_needed2, ref superuser, ref superuser, ref superuser, ref arg_needed3, ref superuser, ref superuser, ref superuser, ref superuser, ref superuser, ref arg_needed4, ref arg_needed3, ref arg_needed3)) == 0))
                    {
                        superuser = false;
                        return (false);
                    }
                }
                else
                {
                    Console.WriteLine("> cmd linux: unable to recover password: data was corrupted.");
                    return (false);
                }
            }

            return (true);
        }

        private static int get_admin_password(ref string password, string appdata_dir)
        {
            /*code:
                0 = OK
                1 = access denied by OS
                2 = file removed
                3 = file modified
                4 = password non initialized
            */
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
                    return (1);
                }
            }
            else
            {
                Console.WriteLine("> cmd linux: unable to recover password: some data is missing.");
                return (2);
            }

            if (security_code_crypted != null && security_code_crypted[0] != ' ' && userName_crypted != null && password_crypted != null && password_crypted.Length > 1)
            {
                if (decrypt_security_code(security_code_crypted) == (File.GetLastWriteTime(appdata_dir + "/settings").Year + (File.GetLastWriteTime(appdata_dir + "/settings").Month + 100).ToString().Substring(1, 2) + (File.GetLastWriteTime(appdata_dir + "/settings").Day + 100).ToString().Substring(1, 2) + (File.GetLastWriteTime(appdata_dir + "/settings").Hour + 100).ToString().Substring(1, 2) + (File.GetLastWriteTime(appdata_dir + "/settings").Minute + 100).ToString().Substring(1, 2) + (File.GetLastWriteTime(appdata_dir + "/settings").Second + 100).ToString().Substring(1, 2)))
                {
                    time = decrypt_time(password_crypted[password_crypted.Length - 1]);
                    if (time > -1)
                    {
                        for (int i = 0; i < password_crypted.Length - 1; i++)
                        {
                            if (password_crypted[i] <= 126 && password_crypted[i] >= 33)
                            {
                                new_char = password_crypted[i] - 33;
                                if (new_char < 33)
                                {
                                    new_char = 94 + new_char;
                                }
                                new_char = new_char - i - time;
                                password = password + (char)new_char;
                            }
                            else
                            {
                                password = password + password_crypted[i];
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("> cmd linux: unable to recover password: data was corrupted.");
                        return (3);
                    }
                }
                else
                {
                    Console.WriteLine("> cmd linux: data was modified outside of the application!\n> cmd linux: For security reasons, MASTER mode is not available anymore...");
                    return (3);
                }
            }
            else
            {
                if (is_good_init_code(appdata_dir))
                {
                    return (4);
                }
                else
                {
                    Console.WriteLine("> cmd linux: unable to recover password: data was corrupted.");
                    return (3);
                }
            }

            return (0);
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


            if (cursor_top == Console.BufferHeight - 1)
            {
                Console.SetBufferSize(Console.BufferWidth, Console.BufferHeight + 300);
            }
            while (key_input.Key != ConsoleKey.Enter && key_input.Key != (ConsoleKey)255)
            {
                key_input = Console.ReadKey(true);
                delta_input = update_password(ref password, key_input, ref input_pointer);
                if(delta_input != 0)
                {
                    if(delta_input > 0)
                    {
                        if(Console.CursorLeft == Console.WindowWidth - 1)
                        {
                            cursor_left = 0;
                            cursor_top++;
                        }
                        else
                        {
                            cursor_left++;
                        }
                        Console.SetCursorPosition(max_cursor_left, max_cursor_top);
                        Console.Write("*");
                        max_cursor_left = Console.CursorLeft;
                        max_cursor_top = Console.CursorTop;
                    }
                    else
                    {
                        Console.SetCursorPosition(max_cursor_left, max_cursor_top);
                        cursor_replacement(-1 * delta_input);
                        max_cursor_left = Console.CursorLeft;
                        max_cursor_top = Console.CursorTop;
                        if (cursor_top > max_cursor_top)
                        {
                            cursor_top = max_cursor_top;
                            cursor_left = max_cursor_left;
                        }
                        else if (cursor_left > max_cursor_left)
                        {
                            cursor_left = max_cursor_left;
                        }
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

        public static int update_password(ref string password, ConsoleKeyInfo key_input, ref int input_pointer)
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
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Enter):
                    break;
                case (ConsoleKey.A):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.B):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.C):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.D):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.E):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.F):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.G):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.H):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.I):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.J):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.K):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.L):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.M):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.N):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.O):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.P):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Q):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.R):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.S):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.T):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.U):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.V):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.W):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.X):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Y):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Z):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad0):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.D0):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad1):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.D1):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad2):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.D2):
                    if (key_input.KeyChar == '\0')
                    {
                        input_pointer--; //~ s'affiche uniquement sur le prochain caractère
                        return (0);
                    }
                    else
                    {
                        add_key_to_command(ref password, key_input, input_pointer);
                    }
                    break;
                case (ConsoleKey.NumPad3):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.D3):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad4):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.D4):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad5):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.D5):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad6):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.D6):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad7):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.D7):
                    if (key_input.KeyChar == '\0')
                    {
                        input_pointer--; //` s'affiche uniquement sur le prochain caractère
                        return (0);
                    }
                    else
                    {
                        add_key_to_command(ref password, key_input, input_pointer);
                    }
                    break;
                case (ConsoleKey.NumPad8):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.D8):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.NumPad9):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.D9):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Add):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Multiply):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Subtract):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Divide):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Spacebar):
                    add_key_to_command(ref password, key_input, input_pointer);
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
                    if (input_pointer > password.Length)
                    {
                        input_pointer--;
                    }
                    else
                    {
                        cursor_replacement(-1);
                    }
                    return (0);
                case (ConsoleKey.UpArrow):
                    input_pointer--;
                    return (0);
                case (ConsoleKey.DownArrow):
                    input_pointer--;
                    return (0);
                case (ConsoleKey.Escape):
                    input_pointer = 0;
                    if(password == "")
                    {
                        return (0);
                    }
                    else
                    {
                        password = "";
                    }
                    break;
                case (ConsoleKey.Delete):
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
                        add_key_to_command(ref password, key_input, input_pointer);
                        break;
                    }
                    break;
                case (ConsoleKey.OemComma):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.OemPeriod):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.OemPlus):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.OemMinus): //- ou _ sur QWERTY
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem1):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem2):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem3):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem4):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem5):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem6):
                    if (key_input.KeyChar == '\0')
                    {
                        input_pointer--; //^ ¨ n'affichent uniquement sur le prochain caractère
                        return (0);
                    }
                    else
                    {
                        add_key_to_command(ref password, key_input, input_pointer);
                    }
                    break;
                case (ConsoleKey.Oem7):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem8):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                case (ConsoleKey.Oem102):
                    add_key_to_command(ref password, key_input, input_pointer);
                    break;
                default:
                    input_pointer--;
                    return (0);
            }

            return (password.Length - delta_input);
        }

        //CMD FUNCTION
        private static List<string> sort_genius_cmd(List<string> data, List<int> data_usage)
        {
            int move = 1;
            int last_elt = 0;
            string data_swap;

            while(move > 0)
            {
                move = 0;
                for(int i = data_usage.Count - 1; i > last_elt; i--)
                {
                    if(data_usage[i] > data_usage[i-1])
                    {
                        move++;
                        data_usage[i] = data_usage[i - 1] + data_usage[i];
                        data_usage[i - 1] = data_usage[i] - data_usage[i - 1];
                        data_usage[i] = data_usage[i] - data_usage[i - 1];
                        data_swap = data[i];
                        data[i] = data[i - 1];
                        data[i - 1] = data_swap;
                    }
                }
                last_elt++;
            }

            return (data);
        }

        public static void reinit_file_genius_data_cmd(string appdata_dir, List<string> all_cmd, List<int> all_cmd_usage = null)
        {
            try
            {
                StreamWriter recover = new StreamWriter(appdata_dir + "/genius_data/genius_data_cmd");

                for (int i = 0; i < all_cmd.Count; i++)
                {
                    if(all_cmd_usage != null)
                    {
                        if (all_cmd[i][all_cmd[i].Length - 1] == ';')
                        {
                            recover.WriteLine(all_cmd[i].Substring(0, all_cmd[i].Length - 1) + ":" + all_cmd_usage[i]);
                        }
                        else
                        {
                            recover.WriteLine(all_cmd[i] + ":" + all_cmd_usage[i]);
                        }
                    }
                    else
                    {
                        if (all_cmd[i][all_cmd[i].Length - 1] == ';')
                        {
                            recover.WriteLine(all_cmd[i].Substring(0, all_cmd[i].Length - 1) + ":0");
                        }
                        else
                        {
                            recover.WriteLine(all_cmd[i] + ":0");
                        }
                    }
                }

                recover.Close();
            }
            catch (Exception)
            {

            }
        }

        private static bool all_genius_cmd_found(ref List<int> all_cmd_usage)
        {
            bool result = true;

            for(int i = 0; i < all_cmd_usage.Count; i++)
            {
                if(all_cmd_usage[i] == -1)
                {
                    all_cmd_usage[i] = 0;
                    result = false;
                }
            }

            return (result);
        }
    }
}