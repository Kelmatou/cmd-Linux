using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Permissions;
using Microsoft.VisualBasic.Devices;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.Threading;
using System.Net.NetworkInformation;
using NativeWifi;
using System.Threading.Tasks;

namespace cmd_Linux
{
    public static class Execution
    {
        public static string version = "16_02_27_12_38 [2.1.9]"; //SI PLUS UNE VERSION BETA, PENSER A RETIRER LA COMMANDE /feedback!!!!
        public static string editor = "Antoine CLOP (Kelmatou Apps)";

        #region APIs
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hwnd);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SetFocus(IntPtr hwnd);
        #endregion APIs //fonctions win32

        static private int execute_command(ref string[][] input, int input_number, string[] cmd, string desktop_dir, string appdata_dir, ref bool working, ref bool superuser, ref long last_result, ref List<Genius_data> genius_data, ref bool script_enable, ref bool genius_enable, ref bool private_mode, ref int refresh_timer, ref bool auto_lock, ref bool auto_log, ref bool cmd_print_user, ref bool cmd_print_path, ref bool cmd_print_time, ref string[] previous_directory, ref int previous_directory_pointer, ref int max_genius_data)
        {
            if (Interpreter.is_keyword(cmd[0]) != Interpreter.Keyword.None)
            {
                if(!private_mode)
                {
                    add_1_usage_to_genius_cmd(cmd[0], appdata_dir);
                }
                if(cmd.Length == 2 && cmd[1] == "?")
                {
                    return (execute_help(new string[] { "help", cmd[0] }, appdata_dir));
                }
                else
                {
                    switch (Interpreter.is_keyword(cmd[0]))
                    {
                        case (Interpreter.Keyword.ls):
                            return (execute_ls(cmd));
                        case (Interpreter.Keyword.cd):
                            return (execute_cd(cmd, ref previous_directory, ref previous_directory_pointer));
                        case (Interpreter.Keyword.cat):
                            return (execute_cat(cmd));
                        case (Interpreter.Keyword.touch):
                            return (execute_touch(cmd));
                        case (Interpreter.Keyword.rm):
                            return (execute_rm(cmd));
                        case (Interpreter.Keyword.rmdir):
                            return (execute_rmdir(cmd));
                        case (Interpreter.Keyword.mkdir):
                            return (execute_mkdir(cmd));
                        case (Interpreter.Keyword.pwd):
                            return (execute_pwd());
                        case (Interpreter.Keyword.clear):
                            return (execute_clear());
                        case (Interpreter.Keyword.whoami):
                            return (execute_whoami());
                        case (Interpreter.Keyword.hostname):
                            return (execute_hostname());
                        case (Interpreter.Keyword.help):
                            return (execute_help(cmd, appdata_dir));
                        case (Interpreter.Keyword.launch):
                            if(cmd.Length > 1 && cmd[1] == "/arg")
                            {
                                return (execute_launch_arg(cmd));
                            }
                            return (execute_launch(cmd));
                        case (Interpreter.Keyword.shutdown):
                            return (execute_shutdown(cmd));
                        case (Interpreter.Keyword.info):
                            return (execute_info());
                        case (Interpreter.Keyword.cp):
                            return (execute_cp(cmd));
                        case (Interpreter.Keyword.cpdir):
                            return (execute_cpdir(cmd));
                        case (Interpreter.Keyword.resize):
                            return (execute_resize(cmd));
                        case (Interpreter.Keyword.taskkill):
                            return (execute_taskkill(cmd));
                        case (Interpreter.Keyword.home):
                            return (execute_home(desktop_dir, ref previous_directory, ref previous_directory_pointer));
                        case (Interpreter.Keyword.uninstall):
                            if (superuser)
                            {
                                return (execute_uninstall(appdata_dir));
                            }
                            else
                            {
                                Console.WriteLine("> cmd Linux: uninstall: access denied!");
                                return (1);
                            }
                        case (Interpreter.Keyword.url):
                            return (execute_url(cmd));
                        case (Interpreter.Keyword.lolapp):
                            return (execute_lolapp(cmd, appdata_dir));
                        case (Interpreter.Keyword.integreted_web_site):
                            return (execute_integreted_web_site(cmd));
                        case (Interpreter.Keyword.facebook):
                            return (execute_facebook(cmd));
                        case (Interpreter.Keyword.twitter):
                            return (execute_twitter(cmd));
                        case(Interpreter.Keyword.google):
                            return (execute_google(cmd));
                        case (Interpreter.Keyword.youtube):
                            return (execute_youtube(cmd));
                        case (Interpreter.Keyword.wikipedia):
                            return (execute_wikipedia(cmd));
                        case (Interpreter.Keyword.map):
                            return (execute_map(cmd));
                        case (Interpreter.Keyword.translate):
                            return (execute_translate(cmd));
                        case (Interpreter.Keyword.bing):
                            return (execute_bing(cmd));
                        case (Interpreter.Keyword.yahoo):
                            return (execute_yahoo(cmd));
                        case (Interpreter.Keyword.skype):
                            return (execute_skype(cmd, appdata_dir));
                        case (Interpreter.Keyword.ts):
                            return (execute_ts(cmd, appdata_dir));
                        case (Interpreter.Keyword.textedit):
                            return (Textedit.execute_textedit(cmd));
                        case (Interpreter.Keyword.script):
                            return (execute_script(cmd, appdata_dir, ref input, input_number));
                        case (Interpreter.Keyword.reset):
                            if (superuser)
                            {
                                return (execute_reset(cmd, appdata_dir, ref genius_data));
                            }
                            else
                            {
                                Console.WriteLine("> cmd Linux: reset: access denied!");
                                return (1);
                            }
                        case (Interpreter.Keyword.tasklist):
                            return (execute_tasklist());
                        case (Interpreter.Keyword.mv):
                            return (execute_mv(cmd));
                        case (Interpreter.Keyword.mvdir):
                            return (execute_mvdir(cmd));
                        case (Interpreter.Keyword.sl):
                            return (execute_sl(cmd));
                        case (Interpreter.Keyword.rename):
                            return (execute_rename(cmd));
                        case (Interpreter.Keyword.password):
                            return (execute_password(superuser, appdata_dir));
                        case (Interpreter.Keyword.@lock):
                            return (execute_lock(cmd));
                        case (Interpreter.Keyword.option):
                            if (superuser)
                            {
                                return (execute_option(ref script_enable, ref refresh_timer, ref auto_lock, ref genius_enable, ref private_mode, ref auto_log, ref cmd_print_user, ref cmd_print_path, ref cmd_print_time, ref max_genius_data, appdata_dir));
                            }
                            else
                            {
                                Console.WriteLine("> cmd Linux: option: access denied!");
                                return (1);
                            }
                        case (Interpreter.Keyword.wait):
                            return (execute_wait(cmd));
                        case (Interpreter.Keyword.echo):
                            return (execute_echo(cmd));
                        case (Interpreter.Keyword.find):
                            return (execute_find(cmd));
                        case (Interpreter.Keyword.time):
                            return (execute_time(cmd, refresh_timer));
                        case (Interpreter.Keyword.seed):
                            return (execute_seed());
                        case (Interpreter.Keyword.network):
                            return (execute_network(cmd));
                        case (Interpreter.Keyword.news):
                            return (execute_news(cmd));
                        case (Interpreter.Keyword.tv):
                            return (execute_tv());
                        case (Interpreter.Keyword.breakout):
                            return (Breakout.execute_breakout());
                        case (Interpreter.Keyword.man):
                            return (execute_man(cmd));
                        case (Interpreter.Keyword.exit):
                            return (execute_exit(ref working));
                        case (Interpreter.Keyword.admin):
                            return (execute_admin(ref superuser, appdata_dir));
                        case (Interpreter.Keyword.unadmin):
                            return (execute_unadmin(ref superuser));
                    }
                }
            }
            if(Calculator.is_math_expression(cmd))
            {
                return(Calculator.execute_calculator(cmd, ref last_result));
            }
            if(cmd.Length > 0 && File.Exists(appdata_dir + "/script_files/" + cmd[0]))
            {
                if(cmd.Length == 2 && cmd[1] == "?")
                {
                    return (execute_script(new string[3] { "script", "/display", cmd[0] }, appdata_dir, ref input, input_number));
                }
                return (execute_script(new string[2] { "script", cmd[0] }, appdata_dir, ref input, input_number));
            }
            if(cmd.Length > 0 && File.Exists(Directory.GetCurrentDirectory() + "/" + cmd[0]))
            {
                return (execute_launch(new string[2] { "launch", cmd[0] }));
            }
            return (execute_unknown_cmd(cmd));
        }

        static public int execute_input(string[][] input, string desktop_dir, string appdata_dir, ref bool working, ref bool superuser, ref long last_result, ref List<Genius_data> genius_data, ref bool script_enable, ref bool genius_enable, ref bool private_mode, ref int refresh_timer, ref bool auto_lock, ref bool auto_log, ref bool cmd_print_user, ref bool cmd_print_path, ref bool cmd_print_time, ref string[] previous_directory, ref int previous_directory_pointer, ref int max_genius_data)
        {   
            for (int i = 0; i < input.GetLength(0) - 1 && working; i++ )
            {
                if(input[i].Length > 0)
                {
                    if (!private_mode)
                    {
                        update_genius_data(appdata_dir, input[i], ref genius_data, max_genius_data);
                    }
                    execute_command(ref input, i, input[i], desktop_dir, appdata_dir, ref working, ref superuser, ref last_result, ref genius_data, ref script_enable, ref genius_enable, ref private_mode, ref refresh_timer, ref auto_lock, ref auto_log, ref cmd_print_user, ref cmd_print_path, ref cmd_print_time, ref previous_directory, ref previous_directory_pointer, ref max_genius_data);
                }
            }
            if(working && input.GetLength(0) > 0 && input[input.GetLength(0) - 1].Length > 0)
            {
                if (!private_mode)
                {
                    update_genius_data(appdata_dir, input[input.GetLength(0) - 1], ref genius_data, max_genius_data);
                }
                return (execute_command(ref input, input.GetLength(0) - 1, input[input.GetLength(0) - 1], desktop_dir, appdata_dir, ref working, ref superuser, ref last_result, ref genius_data, ref script_enable, ref genius_enable, ref private_mode, ref refresh_timer, ref auto_lock, ref auto_log, ref cmd_print_user, ref cmd_print_path, ref cmd_print_time, ref previous_directory, ref previous_directory_pointer, ref max_genius_data));
            }
            else
            {
                return (0);
            }
        }

        static private int show_content(string entry)
        {
            try
            {
                if (File.Exists(entry))
                {
                    Console.WriteLine("> " + entry);
                }
                else if (Directory.Exists(entry))
                {
                    List<string> all_folders = Directory.EnumerateDirectories(entry).ToList();
                    List<string> all_files = Directory.EnumerateFiles(entry).ToList();

                    
                    for (int i = 0; i < all_folders.Count; i++)
                    {
                        Console.Write("> ");
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine(extract_shorter_path(all_folders[i]) + "/");
                        Console.ResetColor();
                    }
                    for (int i = 0; i < all_files.Count; i++)
                    {
                        Console.WriteLine("> " + extract_shorter_path(all_files[i]));
                    }
                }
                else
                {
                    Console.WriteLine("> ls: " + entry + ": No such file or directory");
                    return (1);
                }

                return (0);
            }
            catch(Exception)
            {
                Console.WriteLine("> ls: " + entry + ": access denied! (execute it with admin rights)");
                return (1);
            }
            
        } //OK

        static private int execute_ls(string[] cmd)
        {
            if(cmd.Length == 1)
            {
                return (show_content(Directory.GetCurrentDirectory()));
            }
            else if(cmd.Length == 2)
            {
                return (show_content(cmd[1]));
            }
            else
            {
                for (int i = 1; i < cmd.Length - 1; i++)
                {
                    Console.Write("> ");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("ls " + cmd[i]);
                    Console.ResetColor();
                    show_content(cmd[i]);
                }
                Console.Write("> ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("ls " + cmd[cmd.Length - 1]);
                Console.ResetColor();
                return (show_content(cmd[cmd.Length - 1]));
            }
        } //OK

        static private int execute_cd(string[] cmd, ref string[] previous_directory, ref int previous_directory_pointer)
        {
            if(cmd.Length == 2)
            {
                if (Directory.Exists(cmd[1]))
                {
                    try
                    {
                        Directory.SetCurrentDirectory(cmd[1]);
                        if(previous_directory_pointer == 254)
                        {
                            previous_directory_pointer--;
                            for(int i = 1; i < 256; i++)
                            {
                                previous_directory[i - 1] = previous_directory[i];
                            }
                        }
                        previous_directory_pointer++;
                        previous_directory[previous_directory_pointer + 1] = Directory.GetCurrentDirectory();
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("> cd: " + extract_longer_path(cmd[1]) + ": access denied! (execute it with admin rights)");
                    }
                }
                else if (cmd[1] == "-")
                {
                    if (previous_directory_pointer >= 0 && Directory.Exists(previous_directory[previous_directory_pointer]))
                    {
                        try
                        {
                            Directory.SetCurrentDirectory(previous_directory[previous_directory_pointer]);
                            previous_directory_pointer--;
                        }
                        catch
                        {
                            Console.WriteLine("> cd: " + previous_directory[previous_directory_pointer] + ": access denied! (execute it with admin rights)");
                            return (1);
                        }
                    }
                    else
                    {
                        Console.WriteLine("> cd: No previous directory");
                        return (1);
                    }
                }
                else if (cmd[1] == "+")
                {
                    if (previous_directory_pointer < 254 && previous_directory[previous_directory_pointer + 2] != "" && Directory.Exists(previous_directory[previous_directory_pointer + 2]))
                    {
                        try
                        {
                            Directory.SetCurrentDirectory(previous_directory[previous_directory_pointer + 2]);
                            previous_directory_pointer++;
                        }
                        catch
                        {
                            Console.WriteLine("> cd: " + previous_directory[previous_directory_pointer + 2] + ": access denied! (execute it with admin rights)");
                            return (1);
                        }
                    }
                    else
                    {
                        Console.WriteLine("> cd: No next directory");
                        return (1);
                    }
                }
                else if (File.Exists(cmd[1]))
                {
                    Console.WriteLine("> cd: " + cmd[1] + ": Is a file");
                    return (1);
                }
                else
                {
                    Console.WriteLine("> cd: " + cmd[1] + ": No such file or directory");
                    return (1);
                }
            }
            else
            {
                Console.WriteLine("> cd: Invalid number of argument");
                return (1);
            }
            return (0);
        } //OK

        static private int execute_cat(string[] cmd)
        {
            if(cmd.Length > 1)
            {
                for (int i = 1; i < cmd.Length; i++ )
                {
                    if (File.Exists(cmd[i]))
                    {
                        try
                        {
                            StreamReader read_file = new StreamReader(cmd[i]);
                            string line = read_file.ReadLine();

                            while (line != null)
                            {
                                Console.WriteLine("> " + line);
                                line = read_file.ReadLine();
                            }

                            read_file.Close();
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("> "+ cmd[0] + ": " + extract_longer_path(cmd[i]) + ": access denied! (execute it with admin rights)");
                            if (i == cmd.Length - 1)
                            {
                                return (1);
                            }
                        }
                    }
                    else if (Directory.Exists(cmd[i]))
                    {
                        Console.WriteLine("> " + cmd[0] + ": " + cmd[i] + ": is a directory");
                        if (i == cmd.Length - 1)
                        {
                            return (1);
                        }
                    }
                    else
                    {
                        Console.WriteLine("> " + cmd[0] + ": " + cmd[i] + ": No such file or directory");
                        if (i == cmd.Length - 1)
                        {
                            return (1);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("> cat: Invalid number of arguments");
                return (1);
            }
            return (0);
        } //OK

        static private int execute_touch(string[] cmd)
        {
            if (cmd.Length > 1)
            {
                for (int i = 1; i < cmd.Length; i++)
                {
                    if(is_valid_name(cmd[i]))
                    {
                        try
                        {
                            if (File.Exists(cmd[i]))
                            {
                                File.SetLastAccessTime(cmd[i], DateTime.Now);
                            }
                            else if (Directory.Exists(cmd[i]))
                            {
                                Directory.SetLastAccessTime(cmd[i], DateTime.Now);
                            }
                            else
                            {
                                if(cmd[i].Length <= 260)
                                {
                                    FileStream creat_file = new FileStream(cmd[i], FileMode.Create);
                                    creat_file.Close();
                                }
                                else
                                {
                                    Console.WriteLine("> " + cmd[0] + ": " + cmd[i] + ": File name is too long (max 260 char)");
                                    if (i == cmd.Length - 1)
                                    {
                                        return (1);
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            if (extract_longer_path(cmd[i]) == cmd[i])
                            {
                                Console.WriteLine("> " + cmd[0] + ": " + Directory.GetCurrentDirectory() + ": access denied! (execute it with admin rights)");
                            }
                            else
                            {
                                Console.WriteLine("> " + cmd[0] + ": " + extract_longer_path(cmd[i]) + ": access denied! (execute it with admin rights)");
                            }
                            if (i == cmd.Length - 1)
                            {
                                return (1);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("> " + cmd[0] + ": " + cmd[i] + ": Invalid name");
                        if (i == cmd.Length - 1)
                        {
                            return (1);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("> touch: Invalid number of arguments");
                return (1);
            }

            return (0);
        } //OK

        static private int execute_rm(string[] cmd)
        {
            if(cmd.Length > 1)
            {
                for(int i = 1; i < cmd.Length; i++)
                {
                    if(File.Exists(cmd[i]))
                    {
                        try
                        {
                            File.Delete(cmd[i]);
                        }
                        catch(Exception)
                        {
                            Console.WriteLine("> " + cmd[0] + ": " + cmd[i] + ": inaccessible for security reasons");
                            if (i == cmd.Length - 1)
                            {
                                return (1);
                            }
                        }
                    }
                    else if(Directory.Exists(cmd[i]))
                    {
                        Console.WriteLine("> " + cmd[0] + ": " + cmd[i] + " is a directory");
                        if (i == cmd.Length - 1)
                        {
                            return (1);
                        }
                    }
                    else
                    {
                        Console.WriteLine("> " + cmd[0] + ": " + cmd[i] + ": No such file or directory");
                        if (i == cmd.Length - 1)
                        {
                            return (1);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("> rm: Invalid number of arguments");
                return (1);
            }

            return (0);
        } //OK

        static private int execute_rmdir(string[] cmd)
        {
            if (cmd.Length > 1)
            {
                for (int i = 1; i < cmd.Length; i++)
                {
                    if (Directory.Exists(cmd[i]))
                    {
                        try
                        {
                            rm_full_directory(cmd[i]);
                            Directory.Delete(cmd[i]);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("> " + cmd[0] + ": " + cmd[i] + ": inaccessible for security reasons");
                            if (i == cmd.Length - 1)
                            {
                                return (1);
                            }
                        }
                    }
                    else if (File.Exists(cmd[i]))
                    {
                        Console.WriteLine("> " + cmd[0] + ": " + cmd[i] + " is a file");
                        if (i == cmd.Length - 1)
                        {
                            return (1);
                        }
                    }
                    else
                    {
                        Console.WriteLine("> " + cmd[0] + ": " + cmd[i] + ": No such file or directory");
                        if (i == cmd.Length - 1)
                        {
                            return (1);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("> rmdir: Invalid number of arguments");
                return (1);
            }

            return (0);
        } //OK

        static private int execute_mkdir(string[] cmd)
        {
            if (cmd.Length > 1)
            {
                for (int i = 1; i < cmd.Length; i++)
                {
                    if (Directory.Exists(cmd[i]))
                    {
                        Console.WriteLine("> " + cmd[0] + ": " + cmd[i] + ": Directory already exists");
                        if (i == cmd.Length - 1)
                        {
                            return (1);
                        }
                    }
                    else if (File.Exists(cmd[i]))
                    {
                        Console.WriteLine("> " + cmd[0] + ": " + cmd[i] + ": File already exists");
                        if (i == cmd.Length - 1)
                        {
                            return (1);
                        }
                    }
                    else
                    {
                        if (is_valid_name(cmd[i]) && (Directory.GetCurrentDirectory() + cmd[i]).Length <= 248)
                        {
                            try
                            {
                                Directory.CreateDirectory(cmd[i]);
                            }
                            catch (Exception)
                            {
                                if (extract_longer_path(cmd[i]) == cmd[i])
                                {
                                    Console.WriteLine("> " + cmd[0] + ": " + Directory.GetCurrentDirectory() + ": access denied! (execute it with admin rights)");
                                }
                                else
                                {
                                    Console.WriteLine("> " + cmd[0] + ": " + extract_longer_path(cmd[i]) + ": access denied! (execute it with admin rights)");
                                }
                                if (i == cmd.Length - 1)
                                {
                                    return (1);
                                }
                            }
                        }
                        else
                        {
                            if ((Directory.GetCurrentDirectory() + cmd[i]).Length <= 248)
                            {
                                Console.WriteLine("> " + cmd[0] + ": " + cmd[i] + ": Invalid name");
                                if (i == cmd.Length - 1)
                                {
                                    return (1);
                                }
                            }
                            else
                            {
                                Console.WriteLine("> " + cmd[0] + ": " + cmd[i] + ": Directory name is too long (max 248 char)");
                                if (i == cmd.Length - 1)
                                {
                                    return (1);
                                }
                            }
                            
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("> mkdir: Invalid number of arguments");
                return (1);
            }

            return (0);
        } //OK

        static private int execute_cp(string[] cmd)
        {
            int attempts = 1;
            string new_name;

            if (cmd.Length == 2 || cmd.Length == 3)
            {
                new_name = get_name_file(cmd[1]) + get_format_file(cmd[1]);
                if (File.Exists(cmd[1]))
                {
                    if (cmd.Length == 2)
                    {
                        while (File.Exists(Directory.GetCurrentDirectory() + "/" + new_name) || Directory.Exists(Directory.GetCurrentDirectory() + "/" + new_name))
                        {
                            new_name = get_name_file(cmd[1]) + "(" + attempts + ")" + get_format_file(cmd[1]);
                            attempts++;
                        }

                        try
                        {
                            File.Copy(cmd[1], Directory.GetCurrentDirectory() + "/" + new_name);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("> " + cmd[0] + ": " + Directory.GetCurrentDirectory() + ": access denied! (execute it with admin rights)");
                            return (1);
                        }
                        
                    }
                    else if (Directory.Exists(cmd[2]))
                    {
                        while (File.Exists(cmd[2] + "/" + new_name) || Directory.Exists(cmd[2] + "/" + new_name))
                        {
                            new_name = get_name_file(cmd[1]) + "(" + attempts + ")" + get_format_file(cmd[1]);
                            attempts++;
                        }

                        try
                        {
                            File.Copy(cmd[1], cmd[2] + "/" + new_name);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("> " + cmd[0] + ": " + cmd[2] + ": access denied! (execute it with admin rights)");
                            return (1);
                        }
                    }
                    else if (File.Exists(cmd[2]))
                    {
                        Console.WriteLine("> " + cmd[0] + ": " + cmd[2] + "(Destination): is not a directory");
                        return (1);
                    }
                    else
                    {
                        Console.WriteLine("> " + cmd[0] + ": " + cmd[2] + "(Destination): No such file or directory");
                        return (1);

                    }
                }
                else if (Directory.Exists(cmd[1]))
                {
                    Console.WriteLine("> " + cmd[0] + ": " + cmd[1] + "(Source): is a directory");
                    return (1);
                }
                else
                {
                    Console.WriteLine("> " + cmd[0] + ": " + cmd[1] + "(Source): No such file or directory");
                    return (1);

                }
            }
            else
            {
                Console.WriteLine("> cp: Invalid number of argument");
                return (1);
            }
            return (0);
        }

        static private int execute_mv(string[] cmd)
        {
            if(cmd.Length == 3)
            {
                if (File.Exists(cmd[2] + "/" + extract_shorter_path(cmd[1])) || Directory.Exists(cmd[2] + "/" + extract_shorter_path(cmd[1])))
                {
                    Console.WriteLine("> " + cmd[0] + ": " + extract_shorter_path(cmd[1]) + ": File already exists in " + cmd[2]);
                    return (1);
                }
                else
                {
                    if (execute_cp(new string[3] { "mv", cmd[1], cmd[2] }) == 0)
                    {
                        File.Delete(cmd[1]);
                    }
                }
            }
            else
            {
                Console.WriteLine("> mv: Invalid number of argument");
                return (1);
            }

            return (0);
        }

        static private int execute_cpdir(string[] cmd)
        {
            int attempts = 1;
            string new_name;

            if (cmd.Length == 2 || cmd.Length == 3)
            {
                if (cmd[1].Length > 0 && (cmd[1][cmd[1].Length - 1] == '/' || cmd[1][cmd[1].Length - 1] == '\\'))
                {
                    cmd[1] = cmd[1].Substring(0, cmd[1].Length - 1);
                }
                new_name = extract_shorter_path(cmd[1]);
                if(Directory.Exists(cmd[1]))
                {
                    if(cmd.Length == 2)
                    {
                        if (target_is_copy_folder(cmd[1], Directory.GetCurrentDirectory()))
                        {
                            Console.WriteLine("> "+ cmd[0] + ": Exeption: Can't copy into the source directory");
                            return (1);
                        }
                        else
                        {
                            while (Directory.Exists(Directory.GetCurrentDirectory() + "/" + new_name) || File.Exists(Directory.GetCurrentDirectory() + "/" + new_name))
                            {
                                new_name = cmd[1] + "(" + attempts + ")";
                                attempts++;
                            }
                            try
                            {
                                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/" + new_name);
                                copy_all_folder_content(cmd[1], Directory.GetCurrentDirectory() + "/" + new_name);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("> " + cmd[0] + ": " + Directory.GetCurrentDirectory() + ": access denied! (execute it with admin rights)");
                                return (1);
                            }
                        }
                    }
                    else
                    {
                        if(target_is_copy_folder(cmd[1],cmd[2]))
                        {
                            Console.WriteLine("> "+ cmd[0] + ": Exeption: Can't copy into the source directory");
                            return (1);
                        }
                        else if (Directory.Exists(cmd[2]))
                        {
                            while (Directory.Exists(cmd[2] + "/" + new_name) || File.Exists(cmd[2] + "/" + new_name))
                            {
                                new_name = cmd[1] + "(" + attempts + ")";
                                attempts++;
                            }
                            try
                            {
                                Directory.CreateDirectory(cmd[2] + "/" + new_name);
                                copy_all_folder_content(cmd[1], cmd[2] + "/" + new_name);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("> " + cmd[0] + ": " + cmd[2] + ": access denied! (execute it with admin rights)");
                                return (1);
                            }
                        }
                        else if (File.Exists(cmd[2]))
                        {
                            Console.WriteLine("> " + cmd[0] + ": " + cmd[2] + "(Destination): is a file");
                            return (1);
                        }
                        else
                        {
                            Console.WriteLine("> " + cmd[0] + ": " + cmd[2] + "(Destination): No such file or directory");
                            return (1);
                        }
                    } 
                }
                else if(File.Exists(cmd[1]))
                {
                    Console.WriteLine("> " + cmd[0] + ": " + cmd[1] + "(Source): is not a directory");
                    return (1);
                }
                else
                {
                    Console.WriteLine("> " + cmd[0] + ": " + cmd[1] + "(Source): No such file or directory");
                    return (1);     
                }   
            }
            else
            {
                Console.WriteLine("> cpdir: Invalid number of argument");
                return (1);
            }
            return (0);
        } //OK

        static private int execute_mvdir(string[] cmd)
        {
            if (cmd.Length == 3)
            {
                if (cmd[1][cmd[1].Length - 1] == '/' || cmd[1][cmd[1].Length - 1] == '\\')
                {
                    cmd[1] = cmd[1].Substring(0, cmd[1].Length - 1);
                }
                if (Directory.Exists(cmd[2] + "/" + extract_shorter_path(cmd[1])) || File.Exists(cmd[2] + "/" + extract_shorter_path(cmd[1])))
                {
                    Console.WriteLine("> " + cmd[0] + ": " + cmd[1] + ": Directory already exists in " + cmd[2]);
                    return (1);
                }
                else
                {
                    if (execute_cpdir(new string[3] { "mvdir", cmd[1], cmd[2] }) == 0)
                    {
                        execute_rmdir(new string[2] { "mvdir", cmd[1] });
                    }
                }
            }
            else
            {
                Console.WriteLine("> mvdiv: Invalid number of argument");
                return (1);
            }

            return (0);
        }

        static private int execute_pwd()
        {
            Console.WriteLine("> " + Path.GetFullPath(Directory.GetCurrentDirectory()));
            return (0);
        } //OK

        static private int execute_clear()
        {
            Console.Clear();
            return (0);
        } //OK

        static private int execute_whoami()
        {
            Console.WriteLine("> " + Environment.UserName);

            return (0);
        } //OK

        static private int execute_hostname()
        {
            Console.WriteLine("> " + Environment.MachineName);

            return (0);
        } //OK

        static private int execute_launch(string[] cmd)
        {
            if(cmd.Length > 1)
            {
                ProcessStartInfo pStart = new ProcessStartInfo();

                for (int i = 1; i < cmd.Length; i++ )
                {
                    if (Directory.Exists(cmd[i]) || File.Exists(cmd[i]) || File.Exists("." + cmd[i]) || Directory.Exists("." + cmd[i]))
                    {
                        try
                        {
                            if(cmd[i][0] == '/')
                            {
                                if(File.Exists(cmd[i]) || Directory.Exists(cmd[i]))
                                {
                                    pStart.FileName = Path.GetFullPath(cmd[i]);
                                }
                                else if (File.Exists("." + cmd[i]) || Directory.Exists("." + cmd[i]))
                                {
                                    pStart.FileName = Path.GetFullPath("." + cmd[i]);
                                }
                                else
                                {
                                    pStart.FileName = Path.GetFullPath(cmd[i]);
                                }
                            }
                            else
                            {
                                pStart.FileName = Path.GetFullPath(cmd[i]);
                            }
                            pStart.RedirectStandardOutput = false;
                            pStart.RedirectStandardError = false;
                            pStart.RedirectStandardInput = false;
                            pStart.UseShellExecute = true;
                            pStart.CreateNoWindow = true;
                            Process p = Process.Start(pStart);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("> " + cmd[0] + ": " + cmd[i] + ": failed to execute");
                            if (i == cmd.Length - 1)
                            {
                                return (1);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("> " + cmd[0] + ": " + cmd[i] + ": No such file or directory");
                        if(i == cmd.Length - 1)
                        {
                            return (1);
                        }
                    }
                }
            }
            else if(cmd.Length == 1)
            {
                ProcessStartInfo pStart = new ProcessStartInfo();

                try
                {
                    pStart.FileName = Assembly.GetExecutingAssembly().Location;
                    pStart.RedirectStandardOutput = false;
                    pStart.RedirectStandardError = false;
                    pStart.RedirectStandardInput = false;
                    pStart.UseShellExecute = true;
                    pStart.CreateNoWindow = true;
                    Process p = Process.Start(pStart);
                }
                catch (Exception)
                {
                    Console.WriteLine("> "+ cmd[0] + ": failed to open new cmd Linux session");
                    return (1);
                }
            }
            else
            {
                Console.WriteLine("> launch: Invalid number of arguments");
                return (1);
            }

            return (0);
        } //OK

        static public int execute_launch_arg(string[] cmd)
        {
            if (cmd.Length > 2)
            {
                ProcessStartInfo pStart = new ProcessStartInfo();

                if (Directory.Exists(cmd[2]) || File.Exists(cmd[2]) || File.Exists("." + cmd[2]) || Directory.Exists("." + cmd[2]))
                {
                    try
                    {
                        if (cmd[2][0] == '/')
                        {
                            if (File.Exists(cmd[2]) || Directory.Exists(cmd[2]))
                            {
                                pStart.FileName = Path.GetFullPath(cmd[2]);
                            }
                            else if (File.Exists("." + cmd[2]) || Directory.Exists("." + cmd[2]))
                            {
                                pStart.FileName = Path.GetFullPath("." + cmd[2]);
                            }
                            else
                            {
                                pStart.FileName = Path.GetFullPath(cmd[2]);
                            }
                        }
                        else
                        {
                            pStart.FileName = Path.GetFullPath(cmd[2]);
                        }
                        for (int i = 3; i < cmd.Length; i++)
                        {
                            if(i > 3)
                                pStart.Arguments = pStart.Arguments + " " + cmd[i];
                            else
                                pStart.Arguments = cmd[i];
                        }
                        pStart.RedirectStandardOutput = false;
                        pStart.RedirectStandardError = false;
                        pStart.RedirectStandardInput = false;
                        pStart.UseShellExecute = true;
                        pStart.CreateNoWindow = true;
                        Process p = Process.Start(pStart);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            pStart.UseShellExecute = false;
                            Process p = Process.Start(pStart);
                        }
                        catch(Exception)
                        {
                            Console.WriteLine("> " + cmd[0] + ": " + cmd[2] + ": failed to execute");
                            return (1);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("> " + cmd[0] + ": " + cmd[2] + ": No such file or directory");
                    return (1);
                }
            }
            else
            {
                Console.WriteLine("> launch: Argument missing");
                return (1);
            }

            return (0);
        } //OK

        static private int execute_taskkill(string[] cmd)
        {
            Process[] process_found;
            bool no_error = true;

            if (cmd.Length > 1)
            {
                for (int i = 1; i < cmd.Length; i++)
                {
                    process_found = Process.GetProcessesByName(cmd[i]);
                    if (process_found.Length > 0)
                    {
                        no_error = true;
                        Console.Write("> " + cmd[0] + ": " + cmd[i] + ": (" + process_found.Length + ") process found - ");
                        for(int j = 0; j < process_found.Length; j++)
                        {
                            try
                            {
                                process_found[j].CloseMainWindow();
                                if (j == process_found.Length - 1 && Process.GetProcessesByName(process_found[j].ProcessName).Length != 0)
                                {
                                    Thread.Sleep(500);
                                    if (j == process_found.Length - 1 && Process.GetProcessesByName(process_found[j].ProcessName).Length != 0)
                                    {
                                        try
                                        {
                                            process_found[j].Kill();
                                        }
                                        catch(Exception)
                                        {
                                            Console.WriteLine("closure failure");
                                            no_error = false;
                                            if (i == cmd.Length - 1 && j == process_found.Length - 1)
                                            {
                                                return (1);
                                            }
                                        }
                                    }
                                }
                            }
                            catch(Exception)
                            {
                                try
                                {
                                    process_found[j].Kill();
                                }
                                catch(Exception)
                                {
                                    Console.WriteLine("closure failure");
                                    no_error = false;
                                    if (i == cmd.Length - 1 && j == process_found.Length - 1)
                                    {
                                        return (1);
                                    }
                                }
                            }
                        }
                        if (no_error)
                        {
                            Console.WriteLine("closed successfuly");
                        }
                    }
                    else
                    {
                        Console.WriteLine("> " + cmd[0] + ": " + cmd[i] + ": process not found");
                        if (i == cmd.Length - 1)
                        {
                            return (1);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("> taskkill: Invalid number of arguments");
                return (1);
            }

            return (0);
        } //OK

        static private int execute_tasklist()
        {
            Process[] process_found;
            process_found = sort_alpha(Process.GetProcesses());
            List<string> column1 = new List<string>();
            List<string> column2 = new List<string>();
            List<string> column3 = new List<string>();
            int columnX = 1;

            for (int i = 0; i < process_found.Length; i++)
            {
                switch(columnX)
                {
                    case(1):
                        column1.Add(process_found[i].ProcessName);
                        columnX = 2;
                        break;
                    case (2):
                        column2.Add(process_found[i].ProcessName);
                        columnX = 3;
                        break;
                    case (3):
                        column3.Add(process_found[i].ProcessName);
                        columnX = 1;
                        break;
                }
            }

            if(columnX == 2)
            {
                column2.Add("");
                column3.Add("");
            }
            else if(columnX == 3)
            {
                column3.Add("");
            }

            Console.WriteLine("> Process found: " + process_found.Length);
            for (int i = 0; i < column1.Count; i++)
            {
                Console.Write("> " + column1[i]);
                print_n_space(max_length_list(column1) + 1 - column1[i].Length);
                Console.Write(column2[i]);
                print_n_space(max_length_list(column2) + 1 - column2[i].Length);
                Console.WriteLine(column3[i]);
            }

                return (0);
        } //OK

        static private int execute_shutdown(string[] cmd)
        {
            string path = "";
            if(File.Exists("C:/Windows/System32/shutdown.exe"))
            {
                path = "C:/Windows/System32/shutdown.exe";
            }

            if(path != "")
            {
                if (cmd.Length == 1)
                {
                    return (execute_launch_arg(new string[] { "launch", "/arg", path, "-s -t 5" }));
                }
                else if (cmd.Length == 2 || cmd.Length == 3)
                {
                    switch (cmd[1])
                    {
                        case ("now"):
                            return (execute_launch_arg(new string[] { "launch", "/arg", path, "-s -t 0" }));
                        case ("/r"):
                            return (execute_launch_arg(new string[] { "launch", "/arg", path, "-r -t 5" }));
                        case ("/g"):
                            return (execute_launch_arg(new string[] { "launch", "/arg", path, "-g -t 5" }));
                        case ("/a"):
                            return (execute_launch_arg(new string[] { "launch", "/arg", path, "-a" }));
                        case ("/l"):
                            return (execute_launch_arg(new string[] { "launch", "/arg", path, "-l" }));
                        case ("/h"):
                            return (execute_launch_arg(new string[] { "launch", "/arg", path, "-h" }));
                        case ("/p"):
                            return (execute_launch_arg(new string[] { "launch", "/arg", path, "-p" }));
                        case ("/t"):
                            if (cmd.Length == 3)
                            {
                                int time = -1;

                                try
                                {
                                    time = Convert.ToInt32(cmd[2]);
                                }
                                catch (Exception)
                                {
                                    long calc = 0;
                                    if (Calculator.is_math_expression(new string[1] { cmd[2] }) && Calculator.get_result(new string[1] { cmd[2] }, ref calc) == 0)
                                    {
                                        try
                                        {
                                            time = Convert.ToInt32(calc);
                                        }
                                        catch (Exception)
                                        {
                                            Console.WriteLine("> " + cmd[0] + ": " + cmd[2] + ": Too hight number");
                                            return (1);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("> " + cmd[0] + ": " + cmd[2] + ": Invalid time argument");
                                        return (1);
                                    }
                                }

                                if (time >= 0 && time < 315360000)
                                {
                                    return (execute_launch_arg(new string[] { "launch", "/arg", path, "-s -t " + time}));
                                }
                                else
                                {
                                    Console.WriteLine("> " + cmd[0] + ": " + time + ": Out of range [0;315360000[");
                                    return (1);
                                }
                            }
                            else
                            {
                                Console.WriteLine("> shutdown /t: Invalid number of arguments");
                                return (1);
                            }
                        default:
                            Console.WriteLine("> " + cmd[0] + ": " + cmd[1] + ": Invalid argument");
                            return (1);
                    }
                }
                else
                {
                    Console.WriteLine("> shutdown: Invalid number of arguments");
                    return (1);
                }
            }
            else
            {
                Console.WriteLine("> shutdown: File missing.");
                return (1);
            }
            
        } //OK

        static private int execute_resize(string[] cmd)
        {
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            bool full_size = false;

            if(cmd.Length == 2)
            {
                if(cmd[1] == "/default")
                {
                    try
                    {
                        Console.SetWindowSize(80, 25);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("> resize: unable to set this size");
                    }
                }
                else if(cmd[1] == "/full")
                {
                    while(!full_size)
                    {
                        width += 8;
                        height += 3;
                        try
                        {
                            Console.SetWindowSize(width, height);
                        }
                        catch(Exception)
                        {
                            full_size = true;
                        }
                    }
                    width -= 8;
                    height -= 3;
                    full_size = false;
                    while (!full_size)
                    {
                        width += 4;
                        try
                        {
                            Console.SetWindowSize(width, height);
                        }
                        catch (Exception)
                        {
                            full_size = true;
                        }
                    }
                    width -= 4;
                    full_size = false;
                    while (!full_size)
                    {
                        height += 3;
                        try
                        {
                            Console.SetWindowSize(width, height);
                        }
                        catch (Exception)
                        {
                            try
                            {
                                Console.SetWindowSize(width, height - 1);
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    Console.SetWindowSize(width, height - 2);
                                }
                                catch (Exception)
                                {

                                }
                            }
                            full_size = true;
                        }
                    }
                    Console.SetWindowSize(width - 3, Console.WindowHeight);
                }
                else
                {
                    Console.WriteLine("> " + cmd[0] + ": " + cmd[1] + ": Invalid argument");
                    return (1);
                }
            }
            else if(cmd.Length == 3)
            {
                try
                {
                    width = Convert.ToInt32(cmd[1]);
                    try
                    {
                        height = Convert.ToInt32(cmd[2]);
                        try
                        {
                            Console.SetWindowSize(width, height);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("> resize: (" + width + "x" + height + ") unable to set this size");
                        }
                    }
                    catch (Exception)
                    {
                        long calc = 0;
                        if (Calculator.is_math_expression(new string[1] { cmd[2] }) && Calculator.get_result(new string[1] { cmd[2] }, ref calc) == 0)
                        {
                            try
                            {
                                height = Convert.ToInt32(calc);
                                try
                                {
                                    Console.SetWindowSize(width, height);
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("> resize: " + width + "x" + height + ": unable to set this size");
                                }
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("> " + cmd[0] + ": " + cmd[2] + ": (height) Too hight number");
                                return (1);
                            }
                        }
                        else
                        {
                            Console.WriteLine("> " + cmd[0] + ": " + cmd[2] + ": (height) wrong format");
                            return (1);
                        }
                    }
                }
                catch(Exception)
                {
                    long calc = 0;
                    if (Calculator.is_math_expression(new string[1] { cmd[1] }) && Calculator.get_result(new string[1] { cmd[1] }, ref calc) == 0)
                    {
                        try
                        {
                            width = Convert.ToInt32(calc);
                            try
                            {
                                height = Convert.ToInt32(cmd[2]);
                                try
                                {
                                    Console.SetWindowSize(width, height);
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("> resize: " + width + "x" + height + ": unable to set this size");
                                }
                            }
                            catch (Exception)
                            {
                                calc = 0;
                                if (Calculator.is_math_expression(new string[1] { cmd[2] }) && Calculator.get_result(new string[1] { cmd[2] }, ref calc) == 0)
                                {
                                    try
                                    {
                                        height = Convert.ToInt32(calc);
                                        try
                                        {
                                            Console.SetWindowSize(width, height);
                                        }
                                        catch (Exception)
                                        {
                                            Console.WriteLine("> resize: (" + width + "x" + height + ") unable to set this size");
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        Console.WriteLine("> " + cmd[0] + ": " + cmd[2] + ": (height) Too hight number");
                                        return (1);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("> " + cmd[0] + ": " + cmd[2] + ": (height) wrong format");
                                    return (1);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("> " + cmd[0] + ": " + cmd[1] + ": (width) Too hight number");
                            return (1);
                        }
                    }
                    else
                    {
                        Console.WriteLine("> " + cmd[0] + ": " + cmd[1] + ": (width) wrong format");
                        return (1);
                    }
                } 
            }
            else
            {
                Console.WriteLine("> resize: Invalid number of arguments");
                return (1);
            }

            Console.SetBufferSize(Console.WindowWidth, Console.BufferHeight);
            return (0);
        } //OK

        static private int execute_home(string desktop_dir, ref string[] previous_directories, ref int previous_directories_pointer)
        {
            return (execute_cd(new string[2] { "home", desktop_dir }, ref previous_directories, ref previous_directories_pointer));
        } //OK

        static private int execute_uninstall(string appdata_dir)
        {
            if(appdata_dir != "")
            {
                List<string> all_files = new List<string>();

                if(Directory.Exists(appdata_dir + "/script_files/"))
                {
                    all_files = Directory.EnumerateFiles(appdata_dir + "/script_files/").ToList();
                }

                for (int i = 0; i < all_files.Count; i++)
                {
                    File.Delete(all_files[i]);
                }

                try
                {
                    StreamWriter bat = new StreamWriter(appdata_dir + "/cmd.bat");

                    bat.WriteLine("control");

                    bat.Close();
                    if (execute_launch(new string[2] { "uninstall", appdata_dir + "/cmd.bat" }) == 0)
                    {
                        Console.WriteLine("> uninstall: use the unistall windows tools to complete uninstalling");
                    }

                    return (0);
                }
                catch (Exception)
                {
                    Console.WriteLine("> uninstall: access denied! (execute it with admin rights)");
                    return (1);
                }
            }
            else
            {
                Console.WriteLine("> uninstall: file not found, try to reinstall cmd Linux");
                return (1);
            }
            
        } //OK

        static private int execute_url(string[] cmd)
        {
            if (cmd.Length > 1)
            {
                ProcessStartInfo pStart = new ProcessStartInfo();

                for (int i = 1; i < cmd.Length; i++)
                {
                    try
                    {
                        if(is_valid_url(cmd[i]))
                        {
                            Process p = Process.Start(cmd[i]);
                        }
                        else
                        {
                            Console.WriteLine("> " + cmd[0] + ": " + cmd[i] + ": failed to load this url");
                            if (i == cmd.Length - 1)
                            {
                                return (1);
                            }
                        }
                    }
                    catch(Exception)
                    {
                        Console.WriteLine("> " + cmd[0] + ": " + cmd[i] + ": URL format expected");
                        if (i == cmd.Length - 1)
                        {
                            return (1);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("> url: Invalid number of arguments");
                return (1);
            }

            return (0);
        } //OK

        static private int execute_integreted_web_site(string[] cmd)
        {
            return(execute_url(new string[2] { cmd[0], "http://" + cmd[0] + ".com" }));
        } //OK
        
        static private int execute_facebook(string[] cmd)
        {
            if(cmd.Length == 1)
            {
                return (execute_url(new string[2] { cmd[0], "http://" + cmd[0] + ".com" }));
            }
            else
            {
                for (int i = 1; i < cmd.Length - 1; i++)
                {
                    execute_url(new string[2] { cmd[0], "https://www." + cmd[0] + ".com/search/results/?init=quick&q=" + Web.convert_key_to_url_search_format(cmd[i]) });
                }
                return (execute_url(new string[2] { cmd[0], "https://www." + cmd[0] + ".com/search/results/?init=quick&q=" + Web.convert_key_to_url_search_format(cmd[cmd.Length - 1]) }));
            }
        } //OK

        static private int execute_twitter(string[] cmd)
        {
            if (cmd.Length == 1)
            {
                return (execute_url(new string[2] { cmd[0], "http://" + cmd[0] + ".com" }));
            }
            else
            {
                for (int i = 1; i < cmd.Length - 1; i++)
                {
                    execute_url(new string[2] { cmd[0], "https://" + cmd[0] + ".com/search?src=typd&q=" + Web.convert_key_to_url_search_format(cmd[i]) });
                }
                return (execute_url(new string[2] { cmd[0], "https://" + cmd[0] + ".com/search?src=typd&q=" + Web.convert_key_to_url_search_format(cmd[cmd.Length - 1]) }));
            }
        } //OK

        static private int execute_google(string[] cmd)
        {
            if(cmd.Length == 1)
            {
                return (execute_url(new string[2] { cmd[0], "http://" + cmd[0] + ".com" }));
            }
            else
            {
                for (int i = 1; i < cmd.Length - 1; i++)
                {
                    execute_url(new string[2] { cmd[0], "http://" + cmd[0] + ".com/#q=" + Web.convert_key_to_url_search_format(cmd[i]) });
                }
                return (execute_url(new string[2] { cmd[0], "http://" + cmd[0] + ".com/#q=" + Web.convert_key_to_url_search_format(cmd[cmd.Length - 1]) }));
            }
        } //OK

        static private int execute_news(string[] cmd)
        {
            if (cmd.Length == 1)
            {
                return (execute_url(new string[2] { cmd[0], "http://news.google.com" }));
            }
            else
            {
                for (int i = 1; i < cmd.Length - 1; i++)
                {
                    execute_url(new string[2] { cmd[0], "http://www.google.fr/search?tbm=nws&q=" + Web.convert_key_to_url_search_format(cmd[i]) });
                }
                return (execute_url(new string[2] { cmd[0], "http://www.google.fr/search?tbm=nws&q=" + Web.convert_key_to_url_search_format(cmd[cmd.Length - 1]) }));
            }
        } //OK

        static private int execute_youtube(string[] cmd)
        {
            if (cmd.Length == 1)
            {
                return (execute_url(new string[2] { cmd[0], "http://" + cmd[0] + ".com" }));
            }
            else
            {
                for (int i = 1; i < cmd.Length - 1; i++)
                {
                    execute_url(new string[2] { cmd[0], "http://" + cmd[0] + ".com/results?search_query=" + Web.convert_key_to_url_search_format(cmd[i]) });
                }
                return (execute_url(new string[2] { cmd[0], "http://" + cmd[0] + ".com/results?search_query=" + Web.convert_key_to_url_search_format(cmd[cmd.Length - 1]) }));
            }
        } //OK

        static private int execute_map(string[] cmd)
        {
            if (cmd.Length == 1)
            {
                return (execute_url(new string[2] { cmd[0], "http://google.com/maps/" }));
            }
            else
            {
                for (int i = 1; i < cmd.Length - 1; i++)
                {
                    execute_url(new string[2] { cmd[0], "http://google.com/maps/search/" + Web.convert_key_to_url_search_format(cmd[i]) });
                }
                return (execute_url(new string[2] { cmd[0], "http://google.com/maps/search/" + Web.convert_key_to_url_search_format(cmd[cmd.Length - 1]) }));
            }
            
        } //OK

        static private int execute_wikipedia(string[] cmd)
        {
            if (cmd.Length == 1)
            {
                return (execute_url(new string[2] { cmd[0], "http://wikipedia.com/" }));
            }
            else if(cmd.Length > 1)
            {
                if (Web.web_language_code(cmd[1].Substring(1, cmd[1].Length - 1)) != "")
                {
                    for (int i = 2; i < cmd.Length - 1; i++)
                    {
                        execute_url(new string[2] { cmd[0], "http://" + Web.web_language_code(cmd[1].Substring(1, cmd[1].Length - 1)) + ".wikipedia.com/w/index.php?search=" + Web.convert_key_to_url_search_format(cmd[i]) });
                    }
                    return (execute_url(new string[2] { cmd[0], "http://" + Web.web_language_code(cmd[1].Substring(1, cmd[1].Length - 1)) + ".wikipedia.com/w/index.php?search=" + Web.convert_key_to_url_search_format(cmd[cmd.Length - 1]) }));
                }
                else
                {
                    for (int i = 1; i < cmd.Length - 1; i++)
                    {
                        execute_url(new string[2] { cmd[0], "http://en.wikipedia.com/w/index.php?search=" + Web.convert_key_to_url_search_format(cmd[i]) });
                    }
                    return (execute_url(new string[2] { cmd[0], "http://en.wikipedia.com/w/index.php?search=" + Web.convert_key_to_url_search_format(cmd[cmd.Length - 1]) }));
                }  
            }
            else
            {
                Console.WriteLine("> wikipedia: Invalid number of argument");
                return (1);
            }

        } //OK

        static private int execute_translate(string[] cmd)
        {
            if (cmd.Length == 1)
            {
                return (execute_url(new string[2] { "translate", "http://translate.google.com" }));
            }
            else if(cmd.Length > 2)
            {
                if (Web.web_language_code_google_trad(cmd[1].Substring(1, cmd[1].Length - 1)) != "")
                {
                    if (Web.web_language_code_google_trad(cmd[2].Substring(1, cmd[2].Length - 1)) != "")
                    {
                        if(cmd.Length > 3)
                        {
                            for (int i = 3; i < cmd.Length - 1; i++)
                            {
                                execute_url(new string[2] { "translate", "http://translate.google.com/#" + Web.web_language_code_google_trad(cmd[1].Substring(1, cmd[1].Length - 1)) + "/" + Web.web_language_code_google_trad(cmd[2].Substring(1, cmd[2].Length - 1)) + "/" + Web.convert_key_to_url_search_format(cmd[i]) });
                            }
                            return (execute_url(new string[2] { "translate", "http://translate.google.com/#" + Web.web_language_code_google_trad(cmd[1].Substring(1, cmd[1].Length - 1)) + "/" + Web.web_language_code_google_trad(cmd[2].Substring(1, cmd[2].Length - 1)) + "/" + Web.convert_key_to_url_search_format(cmd[cmd.Length - 1]) }));
                        }
                        else
                        {
                            return (execute_url(new string[2] { "translate", "http://translate.google.com/#" + Web.web_language_code_google_trad(cmd[1].Substring(1, cmd[1].Length - 1)) + "/" + Web.web_language_code_google_trad(cmd[2].Substring(1, cmd[2].Length - 1)) }));
                        }
                    }
                    else
                    {
                        Console.WriteLine("> " + cmd[0] + ": " + cmd[2] + ": Unknown language");
                    }
                }
                else
                {
                    Console.WriteLine("> " + cmd[0] + ": " + cmd[1] + ": Unknown language");
                }
            }
            else
            {
                Console.WriteLine("> translate: Invalid number of argument");
            }

            return (1);
        } //OK

        static private int execute_bing(string[] cmd)
        {
            if (cmd.Length == 1)
            {
                return (execute_url(new string[2] { cmd[0], "http://" + cmd[0] + ".com" }));
            }
            else
            {
                for (int i = 1; i < cmd.Length - 1; i++)
                {
                    execute_url(new string[2] { cmd[0], "http://" + cmd[0] + ".com/search?q=" + Web.convert_key_to_url_search_format(cmd[i]) });
                }
                return (execute_url(new string[2] { cmd[0], "http://" + cmd[0] + ".com/search?q=" + Web.convert_key_to_url_search_format(cmd[cmd.Length - 1]) }));
            }
        } //OK

        static private int execute_yahoo(string[] cmd)
        {
            if (cmd.Length == 1)
            {
                return (execute_url(new string[2] { cmd[0], "http://" + cmd[0] + ".com" }));
            }
            else
            {
                for (int i = 1; i < cmd.Length - 1; i++)
                {
                    execute_url(new string[2] { cmd[0], "http://search." + cmd[0] + ".com/search?p=" + Web.convert_key_to_url_search_format(cmd[i]) });
                }
                return (execute_url(new string[2] { cmd[0], "http://search." + cmd[0] + ".com/search?p=" + Web.convert_key_to_url_search_format(cmd[cmd.Length - 1]) }));
            }
        } //OK

        static private int execute_tv()
        {
            return (execute_url(new string[2] { "tv", "http://www.programme-tv.net/" }));
        } //OK

        static private int execute_lolapp(string[] cmd, string appdata_dir)
        {
            string answer = "";
            Process[] process_found;

            if(cmd.Length == 4 || (cmd.Length == 3 && cmd[1] == "/champion"))
            {
                create_lolapp_script(cmd, appdata_dir);
                execute_cp(new string[3] { "lolapp", appdata_dir + "/script", Directory.GetParent(Directory.GetParent(appdata_dir).ToString()) + "/lolapp" });
                execute_rm(new string[2] { "lolapp", appdata_dir + "/script" });
            }
            process_found = Process.GetProcessesByName("LoLapp");
            if(process_found.Length == 0 || cmd.Length == 1)
            {
                if (File.Exists(appdata_dir + "/lolapp_location"))
                {
                    StreamReader reader = new StreamReader(appdata_dir + "lolapp_location");
                    answer = reader.ReadLine();
                    reader.Close();
                }
                if (File.Exists(Directory.GetParent(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName).FullName + "/LoLapp/lolapp.exe"))
                {
                    return (execute_launch(new string[2] { "lolapp", Directory.GetParent(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName).FullName + "/LoLapp/lolapp.exe" }));
                }
                else if (answer != null && answer != "")
                {
                    return (execute_launch(new string[2] { "lolapp", answer }));
                }
                else
                {
                    Console.WriteLine("> lolapp: can't find the application");
                    Console.Write("> lolapp: would you like to tell us where it is? (Y/N)");
                    answer = Console.ReadLine();
                    if (answer == "Y" || answer == "y")
                    {
                        Console.Write("> lolapp: enter the path of LoLapp (or drag LoLapp in this window): ");
                        answer = Console.ReadLine();
                        if (answer[0] == '"' && answer[answer.Length - 1] == '"')
                        {
                            answer = answer.Substring(1, answer.Length - 2);
                        }
                        else if (answer[0] == '"')
                        {
                            answer = answer.Substring(1, answer.Length - 1);
                        }
                        else if (answer[answer.Length - 1] == '"')
                        {
                            answer = answer.Substring(0, answer.Length - 1);
                        }
                        if (File.Exists(answer) && (extract_shorter_path(answer) == "LoLapp.exe" || extract_shorter_path(answer) == "LoLapp.lnk"))
                        {
                            StreamWriter writer = new StreamWriter(appdata_dir + "/lolapp_location");
                            writer.WriteLine(answer);
                            writer.Close();
                            Console.WriteLine("> lolapp: LoLapp location saved!");
                            return (execute_launch(new string[2] { "lolapp", answer }));
                        }
                        else if (extract_shorter_path(answer) != "LoLapp.exe" && extract_shorter_path(answer) != "LoLapp.lnk")
                        {
                            Console.WriteLine("> lolapp: " + extract_shorter_path(answer) + ": incorrect file or application");
                        }
                        else
                        {
                            Console.WriteLine("> lolapp: " + extract_shorter_path(answer) + ": file not found");
                        }
                    }
                    return (1);
                }
            }
            return (0);
        } //OK

        static private int execute_skype(string[] cmd, string appdata_dir)
        {
            string answer = "";

            if (File.Exists(appdata_dir + "/skype_location"))
            {
                StreamReader reader = new StreamReader(appdata_dir + "skype_location");
                answer = reader.ReadLine();
                reader.Close();
            }
            if (File.Exists(find_main_seed() + ":/Program Files (x86)/Skype/Phone/Skype.exe")) //par défaut sur 64bits
            {
                return (execute_launch(new string[2] { "skype", find_main_seed() + ":/Program Files (x86)/Skype/Phone/Skype.exe" }));
            }
            else if (answer != null && answer != "")
            {
                return (execute_launch(new string[2] { "skype", answer }));
            }
            else
            {
                Console.WriteLine("> skype: can't find the application");
                Console.Write("> skype: would you like to tell us where it is? (Y/N)");
                answer = Console.ReadLine();
                if (answer[0] == '"' && answer[answer.Length - 1] == '"')
                {
                    answer = answer.Substring(1, answer.Length - 2);
                }
                else if (answer[0] == '"')
                {
                    answer = answer.Substring(1, answer.Length - 1);
                }
                else if (answer[answer.Length - 1] == '"')
                {
                    answer = answer.Substring(0, answer.Length - 1);
                }
                if (answer == "Y" || answer == "y")
                {
                    Console.Write("> skype: enter the path of Skype (or drag Skype in this window): ");
                    answer = Console.ReadLine();
                    if (File.Exists(answer) && (extract_shorter_path(answer) == "Skype.exe" || extract_shorter_path(answer) == "Skype.lnk"))
                    {
                        StreamWriter writer = new StreamWriter(appdata_dir + "/skype_location");
                        writer.WriteLine(answer);
                        writer.Close();
                        Console.WriteLine("> skype: Skype location saved!");
                        return (execute_launch(new string[2] { "skype", answer }));
                    }
                    else if (extract_shorter_path(answer) != "Skype.exe" && extract_shorter_path(answer) != "Skype.lnk")
                    {
                        Console.WriteLine("> skype: " + extract_shorter_path(answer) + ": incorrect file or application");
                    }
                    else
                    {
                        Console.WriteLine("> skype: " + extract_shorter_path(answer) + ": file not found");
                    }
                }
                return (1);
            }
        } //OK

        static private int execute_ts(string[] cmd, string appdata_dir)
        {
            string answer = "";

            if (File.Exists(appdata_dir + "/ts_location"))
            {
                StreamReader reader = new StreamReader(appdata_dir + "ts_location");
                answer = reader.ReadLine();
                reader.Close();
            }
            if (File.Exists(find_main_seed() + ":/Users/" + Environment.UserName + "/AppData/Local/TeamSpeak 3 Client/ts3client_win32.exe"))
            {
                return (execute_launch(new string[2] { "ts", find_main_seed() + ":/Users/" + Environment.UserName + "/AppData/Local/TeamSpeak 3 Client/ts3client_win32.exe" }));
            }
            else if (answer != null && answer != "")
            {
                return (execute_launch(new string[2] { "ts", answer }));
            }
            else
            {
                Console.WriteLine("> ts: can't find the application");
                Console.Write("> ts: would you like to tell us where it is? (Y/N)");
                answer = Console.ReadLine();
                if (answer[0] == '"' && answer[answer.Length - 1] == '"')
                {
                    answer = answer.Substring(1, answer.Length - 2);
                }
                else if (answer[0] == '"')
                {
                    answer = answer.Substring(1, answer.Length - 1);
                }
                else if (answer[answer.Length - 1] == '"')
                {
                    answer = answer.Substring(0, answer.Length - 1);
                }
                if (answer == "Y" || answer == "y")
                {
                    Console.Write("> ts: enter the path of Team Speak (or drag Team Speak in this window): ");
                    answer = Console.ReadLine();
                    if (File.Exists(answer) && (extract_shorter_path(answer) == "ts3client_win32.exe" || extract_shorter_path(answer) == "TeamSpeak 3 Client.lnk"))
                    {
                        StreamWriter writer = new StreamWriter(appdata_dir + "/ts_location");
                        writer.WriteLine(answer);
                        writer.Close();
                        Console.WriteLine("> ts: Team Speak location saved!");
                        return (execute_launch(new string[2] { "ts", answer }));
                    }
                    else if (extract_shorter_path(answer) != "ts3client_win32.exe" && extract_shorter_path(answer) != "TeamSpeak 3 Client.lnk")
                    {
                        Console.WriteLine("> ts: " + extract_shorter_path(answer) + ": incorrect file or application");
                    }
                    else
                    {
                        Console.WriteLine("> ts: " + extract_shorter_path(answer) + ": file not found");
                    }
                }
                return (1);
            }
        } //OK

        static private int execute_reset(string[] cmd, string appdata_dir, ref List<Genius_data> genius_data)
        {
            int error = 0;

            if (cmd.Length == 1)
            {
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
                Program.reinit_file_genius_data_cmd(appdata_dir, Static_data.cmd_linux_commands());
            }
            else if (cmd.Length == 2 && cmd[1] == "/stats")
            {
                Program.reinit_file_genius_data_cmd(appdata_dir, Static_data.cmd_linux_commands());
            }
            else if (cmd.Length >= 2)
            {
                if(cmd[1] == "/genius")
                {
                    if(cmd.Length >= 3)
                    {
                        for (int i = 2; i < cmd.Length; i++)
                        {
                            if (File.Exists(appdata_dir + "/genius_data/genius_data_" + cmd[i]))
                            {
                                StreamWriter eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_" + cmd[i]);
                                eraser.Close();
                                error = 0;
                            }
                            else
                            {
                                error = 1;
                            }
                        }
                    }
                    else
                    {
                        List<string> all_cmd = Static_data.cmd_linux_commands();
                        for (int i = 0; i < all_cmd.Count; i++)
                        {
                            if (File.Exists(appdata_dir + "/genius_data/genius_data_" + all_cmd[i]))
                            {
                                StreamWriter eraser = new StreamWriter(appdata_dir + "/genius_data/genius_data_" + all_cmd[i]);
                                eraser.Close();
                                error = 0;
                            }
                            else
                            {
                                error = 1;
                            }
                        }
                    }
                }
                else if(cmd[1] == "/stats")
                {
                    Console.WriteLine("> " + cmd[0] + " /stats: Invalid number of argument");
                    return (1);
                }
                else
                {
                    Console.WriteLine("> " + cmd[0] + ": " + cmd[1] + ": Invalid argument");
                    return (1);
                }
            }
            else
            {
                Console.WriteLine("> reset: Invalid number of argument");
                return (1);
            }
            return (error);
        } //OK

        static private int execute_script(string[] cmd, string appdata_dir, ref string[][] input, int input_pointer)
        {
            int error = 0;
            List<string> script_instructions = new List<string>();
            string line_read;

            if (cmd.Length > 1)
            {
                if(cmd[1] == "/mk")
                {
                    #region mk_code
                    if (cmd.Length > 2)
                    {
                        if (!Directory.Exists(appdata_dir + "/script_files/"))
                        {
                            Directory.CreateDirectory(appdata_dir + "/script_files/");
                        }

                        for (int i = 2; i < cmd.Length; i++)
                        {
                            Console.Write("> ");
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            if (is_valid_name(cmd[i]))
                            {
                                Console.WriteLine("Edit the script \"" + cmd[i] + "\":");
                                Console.ResetColor();
                                Console.Write("> ");
                                Console.ForegroundColor = ConsoleColor.DarkGreen;
                                Console.WriteLine("(/save in empty line to save your script or /leave to cancel)");
                                Console.ResetColor();
                                script_instructions = new List<string>();
                                Console.Write("> ");
                                line_read = Console.ReadLine();
                                while (line_read != "/save" && line_read != "/leave")
                                {
                                    script_instructions.Add(line_read);
                                    Console.Write("> ");
                                    line_read = Console.ReadLine();
                                }

                                if (line_read == "/save")
                                {
                                    try
                                    {
                                        StreamWriter script_creator = new StreamWriter(appdata_dir + "/script_files/" + cmd[i]);

                                        for (int j = 0; j < script_instructions.Count; j++)
                                        {
                                            script_creator.WriteLine(script_instructions[j]);
                                        }

                                        script_creator.Close();
                                        Console.Write("> ");
                                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                                        Console.WriteLine("Script \"" + cmd[i] + "\" saved!");
                                    }
                                    catch (Exception)
                                    {
                                        Console.WriteLine("> " + cmd[0] + ": " + cmd[i] + ": unable to save");
                                        error = 1;
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine(cmd[0] + ": " + cmd[i] + ": Invalid script name");
                            }
                            if (i < cmd.Length - 1)
                            {
                                Console.ResetColor();
                                Console.WriteLine(">");
                            }
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        Console.WriteLine("> script: /mk: Invalid number of argument");
                        return (1);
                    }
                    #endregion
                    
                }
                else if(cmd[1] == "/rm")
                {
                    #region rm_code
                    if (cmd.Length > 2)
                    {
                        for (int i = 2; i < cmd.Length; i++)
                        {
                            if (File.Exists(appdata_dir + "/script_files/" + cmd[i]))
                            {
                                try
                                {
                                    File.Delete(appdata_dir + "/script_files/" + cmd[i]);
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("> " + cmd[0] + ": " + cmd[i] + ": unable to remove");
                                    error = 1;
                                }

                            }
                            else
                            {
                                Console.WriteLine("> " + cmd[0] + ": " + cmd[i] + ": Script not found");
                                error = 1;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("> script: /rm: Invalid number of argument");
                        return (1);
                    }
                    #endregion

                }
                else if(cmd[1] == "/rename")
                {
                    #region rename_code
                    if (cmd.Length == 4)
                    {
                        if (File.Exists(appdata_dir + "/script_files/" + cmd[2]))
                        {
                            try
                            {
                                File.Move(appdata_dir + "/script_files/" + cmd[2], appdata_dir + "/script_files/" + cmd[3]);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("> " + cmd[0] + ": " + cmd[2] + ": unable to rename");
                                error = 1;
                            }

                        }
                        else
                        {
                            Console.WriteLine("> " + cmd[0] + ": " + cmd[2] + ": Script not found");
                            error = 1;
                        }
                    }
                    else
                    {
                        Console.WriteLine("> script: /rename: Invalid number of argument");
                        return (1);
                    }
                    #endregion
                }
                else if(cmd[1] == "/display")
                {
                    #region display_code
                    if (cmd.Length == 2)
                    {
                        if (Directory.Exists(appdata_dir + "/script_files/"))
                        {
                            List<string> all_files = new List<string>();
                            try
                            {
                                all_files = Directory.EnumerateFiles(appdata_dir + "/script_files/").ToList();
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("> script: /display: Unable to display scripts");
                                return (1);
                            }

                            for (int i = 0; i < all_files.Count; i++)
                            {
                                Console.WriteLine("> " + extract_shorter_path(all_files[i]));
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory(appdata_dir + "/script_files/");
                        }
                    }
                    else
                    {
                        for (int i = 2; i < cmd.Length; i++)
                        {
                            Console.Write("> ");
                            if (File.Exists(appdata_dir + "/script_files/" + cmd[i]))
                            {
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.WriteLine("Script " + cmd[i] + ":");
                                Console.ResetColor();
                                execute_cat(new string[2] { "script", appdata_dir + "/script_files/" + cmd[i] });
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.WriteLine("Script " + cmd[i] + ": Script not found");
                                Console.ResetColor();
                            }
                        }
                    }
                    #endregion  
                }
                else if(cmd[1] == "/import")
                {
                    #region import_code
                    int elt_added = 0;
                    int elt_replaced = 0;
                    int elt_refused = 0;
                    int i = 2;
                    string answer;

                    if (cmd.Length > 3 && (cmd[2] == "/f" || cmd[2] == "/d") || (cmd.Length > 2 && cmd[2] != "/f" && cmd[2] != "/d"))
                    {
                        if (cmd[2] == "/f" || cmd[2] == "/d")
                        {
                            i++;
                        }
                        for (; i < cmd.Length; i++)
                        {
                            if (File.Exists(cmd[i]))
                            {
                                if (is_script_format(extract_shorter_path(cmd[i])))
                                {
                                    if (File.Exists(appdata_dir + "/script_files/" + extract_shorter_path(cmd[i])))
                                    {
                                        if (cmd[2] == "/f")
                                        {
                                            answer = "Y";
                                        }
                                        else if (cmd[2] == "/d")
                                        {
                                            answer = "N";
                                        }
                                        else
                                        {
                                            Console.Write("> script: /import: " + cmd[i] + ": Script already exists, replace it? (Y/N): ");
                                            answer = Console.ReadLine();
                                        }
                                        if (answer == "y" || answer == "Y")
                                        {
                                            execute_rm(new string[2] { "script", appdata_dir + "/script_files/" + extract_shorter_path(cmd[i]) });
                                            execute_cp(new string[3] { "script", cmd[i], appdata_dir + "/script_files/" });
                                            Console.WriteLine("> script: /import: " + cmd[i] + " replaced");
                                            elt_replaced++;
                                            elt_added++;
                                        }
                                        else
                                        {
                                            Console.WriteLine("> script: /import: " + cmd[i] + ": Abandon");
                                            elt_refused++;
                                        }
                                    }
                                    else
                                    {
                                        execute_cp(new string[3] { "script", cmd[i], appdata_dir + "/script_files/" });
                                        Console.WriteLine("> script: /import: " + cmd[i] + " added");
                                        elt_added++;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("> script: /import: " + extract_shorter_path(cmd[i]) + ": Wrong script format");
                                    elt_refused++;
                                }
                            }
                            else if (Directory.Exists(cmd[i]))
                            {
                                List<string> all_files = new List<string>();
                                try
                                {
                                    all_files = Directory.EnumerateFiles(cmd[i]).ToList();
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("> script: /import: " + extract_shorter_path(cmd[i]) + ": Unable to access content");
                                }

                                for (int j = 0; j < all_files.Count; j++)
                                {
                                    if (is_script_format(extract_shorter_path(all_files[j])))
                                    {
                                        if (File.Exists(appdata_dir + "/script_files/" + extract_shorter_path(all_files[j])))
                                        {
                                            if (cmd[2] == "/f")
                                            {
                                                answer = "Y";
                                            }
                                            else if (cmd[2] == "/d")
                                            {
                                                answer = "N";
                                            }
                                            else
                                            {
                                                Console.Write("> script: /import: " + extract_shorter_path(all_files[j]) + ": Script already exists, replace it? (Y/N): ");
                                                answer = Console.ReadLine();
                                            }
                                            if (answer == "y" || answer == "Y")
                                            {
                                                execute_rm(new string[2] { "script", appdata_dir + "/script_files/" + extract_shorter_path(all_files[j]) });
                                                execute_cp(new string[3] { "script", all_files[j], appdata_dir + "/script_files/" });
                                                Console.WriteLine("> script: /import: " + extract_shorter_path(all_files[j]) + " replaced");
                                                elt_replaced++;
                                                elt_added++;
                                            }
                                            else
                                            {
                                                Console.WriteLine("> script: /import: " + extract_shorter_path(cmd[i]) + "/" + extract_shorter_path(all_files[j]) + ": Abandon");
                                                elt_refused++;
                                            }
                                        }
                                        else
                                        {
                                            execute_cp(new string[3] { "script", all_files[j], appdata_dir + "/script_files/" });
                                            Console.WriteLine("> script: /import: " + extract_shorter_path(all_files[j]) + " added");
                                            elt_added++;
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("> script: /import: " + extract_shorter_path(all_files[j]) + ": Wrong script format");
                                        elt_refused++;
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("> script: /import: " + cmd[i] + ": No such file or directory");
                            }
                        }
                        Console.Write("> ");
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("script: /import: " + elt_added + " script(s) added");
                        Console.ResetColor();
                        Console.Write("> ");
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("script: /import: " + elt_replaced + " script(s) replaced");
                        Console.ResetColor();
                        Console.Write("> ");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("script: /import: " + elt_refused + " script(s) refused");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine("> script: /import: Invalid number of argument");
                        return (1);
                    }
                    #endregion
                }
                else if (cmd[1] == "/export")
                {
                    #region export_code
                    return (execute_cpdir(new string[3] { "script", appdata_dir + "/script_files" , Directory.GetCurrentDirectory()}));
                    #endregion
                }
                else
                {
                    #region script_execution
                    for (int i = 1; i < cmd.Length; i++)
                    {
                        if (File.Exists(appdata_dir + "/script_files/" + cmd[i]))
                        {
                            StreamReader reader;
                            if (File.Exists(appdata_dir + "script"))
                            {
                                try
                                {
                                    reader = new StreamReader(appdata_dir + "script");
                                    line_read = reader.ReadLine();

                                    while (line_read != null)
                                    {
                                        script_instructions.Add(line_read);
                                        line_read = reader.ReadLine();
                                    }

                                    reader.Close();
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("> " + cmd[0] + ": unable to recover script (execute it with admin rights)");
                                    return (1);
                                }

                            }

                            try
                            {
                                reader = new StreamReader(appdata_dir + "/script_files/" + cmd[i]);
                                line_read = reader.ReadLine();

                                while (line_read != null)
                                {
                                    script_instructions.Add(line_read);
                                    line_read = reader.ReadLine();
                                }

                                reader.Close();
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("> " + cmd[0] + ": unable to launch script (execute it with admin rights)");
                                return (1);
                            }

                        }
                        else
                        {
                            Console.WriteLine("> " + cmd[0] + ": " + cmd[i] + ": Script not found");
                            error = 1;
                        }
                    }

                    if (script_instructions.Count > 0)
                    {
                        for (int i = input_pointer + 1; i < input.GetLength(0); i++)
                        {
                            line_read = "";
                            for (int j = 0; j < input[i].Length; j++)
                            {
                                line_read = line_read + "\"" + input[i][j] + "\" ";
                            }
                            script_instructions.Add(line_read);
                        }
                        input = new string[0][];

                        try
                        {
                            StreamWriter script_creator = new StreamWriter(appdata_dir + "script");

                            for (int i = 0; i < script_instructions.Count; i++)
                            {
                                script_creator.Write(script_instructions[i] + ";");
                            }

                            script_creator.Close();
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("> " + cmd[0] + ": unable to launch script (execute it with admin rights)");
                            return (1);
                        }

                    }
                
                    #endregion
                }   
            }
            else
            {
                Console.WriteLine("> script: Invalid number of argument");
                return (1);
            }

            return (error);
        } //OK

        static private int execute_sl(string[] cmd)
        {
            if(cmd.Length == 1)
            {
                Random random = new Random();

                switch (random.Next(0, 4))
                {
                    case (0):
                        train_animation();
                        break;
                    case (1):
                        plane_animation();
                        break;
                    case (2):
                        tank_animation();
                        break;
                    case (3):
                        drake_animation();
                        break;
                }
            }
            else if(cmd.Length == 2)
            {
                switch(cmd[1])
                {
                    case("/train"):
                        train_animation();
                        break;
                    case("/plane"):
                        plane_animation();
                        break;
                    case("/tank"):
                        tank_animation();
                        break;
                    case ("/drake"):
                        drake_animation();
                        break;
                    default:
                        Console.WriteLine("> " + cmd[0] + ": " + cmd[1] + ": Invalid argument");
                        return (1);
                }
            }
            else
            {
                Console.WriteLine("> sl: Invalid number of argument");
                return (1);
            }
            
            return (0);
        } //OK

        static private int execute_rename(string[] cmd)
        {
            if(cmd.Length == 3)
            {
                if(File.Exists(cmd[1]))
                {
                    #region rename_file
                    if (is_valid_name(cmd[2]))
                    {
                        if(!File.Exists(cmd[2]))
                        {
                            if(!Directory.Exists(cmd[2]))
                            {
                                if (cmd[2].Length <= 260)
                                {
                                    try
                                    {
                                        if (extract_longer_path(cmd[1]) != cmd[1])
                                        {
                                            File.Move(cmd[1], extract_longer_path(cmd[1]) + cmd[2]);
                                        }
                                        else
                                        {
                                            File.Move(cmd[1], cmd[2]);
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        Console.WriteLine("> " + cmd[0] + ": " + extract_shorter_path(cmd[1]) + ": access denied! (execute it with admin rights)");
                                        return (1);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("> " + cmd[0] + ": " + cmd[2] + ": File name is too long (max 260 char)");
                                    return (1);
                                }
                            }
                            else
                            {
                                Console.WriteLine("> " + cmd[0] + ": " + cmd[2] + ": Directory already exists");
                                return (1);
                            }
                            
                        }
                        else
                        {
                            Console.WriteLine("> " + cmd[0] + ": " + cmd[2] + ": File already exists");
                            return (1);
                        }
                    }
                    else
                    {
                        Console.WriteLine("> " + cmd[0] + ": " + cmd[2] + ": Invalid name");
                        return (1);
                    }
                    #endregion
                    
                }
                else if(Directory.Exists(cmd[1]))
                {
                    #region rename_folder
                    if (is_valid_name(cmd[2]))
                    {
                        if (!File.Exists(cmd[2]))
                        {
                            if (!Directory.Exists(cmd[2]))
                            {
                                if (cmd[2].Length <= 260)
                                {
                                    try
                                    {
                                        if (extract_longer_path(cmd[1]) != cmd[1])
                                        {
                                            Directory.Move(cmd[1], extract_longer_path(cmd[1]) + cmd[2]);
                                        }
                                        else
                                        {
                                            Directory.Move(cmd[1], cmd[2]);
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        Console.WriteLine("> " + cmd[0] + ": " + extract_shorter_path(cmd[1]) + ": access denied! (execute it with admin rights)");
                                        return (1);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("> " + cmd[0] + ": " + extract_shorter_path(cmd[2]) + ": File name is too long (max 260 char)");
                                    return (1);
                                }
                            }
                            else
                            {
                                Console.WriteLine("> " + cmd[0] + ": " + extract_shorter_path(cmd[2]) + ": Directory already exists");
                                return (1);
                            }

                        }
                        else
                        {
                            Console.WriteLine("> " + cmd[0] + ": " + extract_shorter_path(cmd[2]) + ": File already exists");
                            return (1);
                        }
                    }
                    else
                    {
                        if (cmd[2][cmd[2].Length - 1] == '/' || cmd[2][cmd[2].Length - 1] == '\\')
                        {
                            Console.WriteLine("> " + cmd[0] + ": " + extract_shorter_path(cmd[2].Substring(0, cmd[2].Length - 1)) + "/: Invalid name");
                        }
                        else
                        {
                            Console.WriteLine("> " + cmd[0] + ": " + extract_shorter_path(cmd[2]) + ": Invalid name");
                        }
                        return (1);
                    }
                    #endregion
                }
                else
                {
                    Console.WriteLine("> " + cmd[0] + ": " + extract_shorter_path(cmd[1]) + ": No such file or directory");
                    return (1);
                }
            }
            else
            {
                Console.WriteLine("> rename: Invalid number of argument");
                return (1);
            }

            return (0);
        } //OK

        static private int execute_password(bool superuser, string appdata_dir)
        {
            bool init = false;
            string new_password = "";
            string old_password = "";
            string User = Environment.UserName;

            try
            {
                StreamReader reader = new StreamReader(appdata_dir + "/settings");

                init = reader.ReadLine()[0] == ' ';

                reader.Close();
            }
            catch(Exception)
            {

            }

            if(superuser || init)
            {
                if(!init)
                {
                    if(Program.decrypt_password(ref User,ref old_password, appdata_dir, ref superuser))
                    {
                        Console.Write("> ");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write("Enter password: ");
                        new_password = Program.get_password_protocol();
                    }
                    Console.ResetColor();
                }
                if(new_password == old_password)
                {
                    Console.Write("> ");
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("Enter your new password: ");
                    new_password = Console.ReadLine();
                    Console.ResetColor();
                    if (Program.encrypt_password(new_password, appdata_dir))
                    {
                        return (0);
                    }
                    else
                    {
                        Console.WriteLine("> password: Invalid password");
                    }
                }
                else
                {
                    Console.WriteLine("> password: authentication failed!");
                }
            }
            else
            {
                Console.WriteLine("> password: access denied!");
            }

            return (1);
        } //OK

        static private int execute_lock(string[] cmd)
        {
            string password;
            int start_cursor_left;
            int start_cursor_top;
            float batPercentage = SystemInformation.PowerStatus.BatteryLifePercent;
            bool charging = SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online;
            Stopwatch clock = new Stopwatch();

            Console.Title = "[LOCK] cmd Linux [LOCK]";
            Console.Clear();
            if(Console.WindowWidth / 2 - 12 < 0 || Console.WindowHeight / 2 - 5 < 0)
            {
                Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight / 2);
            }
            else
            {
                Console.SetCursorPosition(Console.WindowWidth / 2 - 12, Console.WindowHeight / 2 - 5);
            }
            Console.Write("[LOCK] cmd Linux [LOCK]");
            if(Console.CursorLeft >= 19)
            {
                Console.SetCursorPosition(Console.CursorLeft - 19, Console.CursorTop + 3);
            }
            else
            {
                Console.SetCursorPosition(0, Console.CursorTop + 3);
            }
            Console.Write("Enter password");
            if (Console.CursorLeft >= 14)
            {
                Console.SetCursorPosition(Console.CursorLeft - 14, Console.CursorTop + 1);
            }
            else
            {
                Console.SetCursorPosition(0, Console.CursorTop + 1);
            }
            start_cursor_top = Console.CursorTop;
            start_cursor_left = Console.CursorLeft;

            Console.SetCursorPosition(1, 1);
            Console.Write(get_battery_status());

            if(cmd.Length == 2)
            {
                password = cmd[1] + '_';
                while(password != cmd[1])
                {
                    Console.CursorVisible = false;
                    while (!Console.KeyAvailable)
                    {
                        if(batPercentage != SystemInformation.PowerStatus.BatteryLifePercent || charging != (SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online))
                        {
                            batPercentage = SystemInformation.PowerStatus.BatteryLifePercent;
                            charging = (SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online);
                            Console.SetCursorPosition(1, 1);
                            print_n_space(Console.WindowWidth);
                            Console.SetCursorPosition(1, 1);
                            Console.Write(get_battery_status());
                        }
                        if (Console.WindowWidth - 1 - DateTime.Now.ToString().Length >= 0)
                        {
                            Console.SetCursorPosition(Console.WindowWidth - 1 - DateTime.Now.ToString().Length, 1);
                        }
                        else
                        {
                            Console.SetCursorPosition(0, 1);
                        }
                        Console.Write(DateTime.Now);
                        Thread.Sleep(1000);
                    }
                    Console.CursorVisible = true;
                    Console.SetCursorPosition(start_cursor_left, start_cursor_top);
                    password = Program.get_password_protocol();
                    Console.SetCursorPosition(start_cursor_left, start_cursor_top);
                    print_n_space(password.Length);
                    Console.SetCursorPosition(start_cursor_left, start_cursor_top);
                }
            }
            else
            {
                password = "cmd_linux";
                Console.CursorVisible = false;
                while (Console.CursorVisible == false)
                {
                    if(Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                        clock.Start();
                        while (clock.ElapsedMilliseconds < 200 && password == "cmd_linux")
                        {
                            if (Console.KeyAvailable)
                            {
                                Console.ReadKey(true);
                                password = "you_shall_not_pass";
                                clock.Reset();
                            }
                        }
                        if(clock.ElapsedMilliseconds >= 200 && password == "cmd_linux")
                        {
                            Console.CursorVisible = true;
                        }
                        else
                        {
                            password = "cmd_linux";
                        }
                    }

                    if(Console.CursorVisible == false)
                    {
                        if (batPercentage != SystemInformation.PowerStatus.BatteryLifePercent || charging != (SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online))
                        {
                            batPercentage = SystemInformation.PowerStatus.BatteryLifePercent;
                            charging = (SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online);
                            Console.SetCursorPosition(1, 1);
                            print_n_space(Console.WindowWidth);
                            Console.SetCursorPosition(1, 1);
                            Console.Write(get_battery_status());
                        }
                        if (Console.WindowWidth - 1 - DateTime.Now.ToString().Length >= 0)
                        {
                            Console.SetCursorPosition(Console.WindowWidth - 1 - DateTime.Now.ToString().Length, 1);
                        }
                        else
                        {
                            Console.SetCursorPosition(0, 1);
                        }
                        Console.Write(DateTime.Now);
                        Thread.Sleep(1000);
                    }
                }
            }
            Console.Clear();

            Console.Title = "cmd Linux";
            return (0);
        } //OK

        static private int execute_option(ref bool script_enable, ref int refresh_timer, ref bool auto_lock, ref bool genius_enable, ref bool private_mode, ref bool auto_log, ref bool cmd_print_user, ref bool cmd_print_path, ref bool cmd_print_time, ref int max_genius_data, string appdata_dir)
        {
            bool running = true;
            int cursor_first_line = Console.CursorTop;
            int line_select = 1;
            ConsoleKeyInfo action;
            Console.CursorVisible = false;
            
            while(running)
            {
                print_option_menu(cursor_first_line, line_select, script_enable, refresh_timer, auto_lock, genius_enable, private_mode, auto_log, cmd_print_user, cmd_print_path, cmd_print_time, max_genius_data);
                action = Console.ReadKey(true);
                apply_option_menu_action(action, ref line_select, ref script_enable, ref refresh_timer, ref auto_lock, ref genius_enable, ref private_mode, ref running, ref auto_log, ref cmd_print_user, ref cmd_print_path, ref cmd_print_time, ref max_genius_data);
            }

            erase_option_menu(cursor_first_line);
            save_option(script_enable, refresh_timer, auto_lock, genius_enable, private_mode, auto_log, cmd_print_user, cmd_print_path, cmd_print_time, max_genius_data, appdata_dir);
            Console.CursorVisible = true;
            return (0);
        } //OK

        static private int execute_wait(string[] cmd)
        {
            if(cmd.Length == 2)
            {
                try
                {
                    Console.CursorVisible = false;
                    Thread.Sleep(Convert.ToInt32(cmd[1]) * 1000);
                    Console.CursorVisible = true;
                }
                catch(Exception)
                {
                    long calc = 0;
                    if (Calculator.is_math_expression(new string[1] { cmd[1] }) && Calculator.get_result(new string[1] { cmd[1] }, ref calc) == 0)
                    {
                        try
                        {
                            Console.CursorVisible = false;
                            Thread.Sleep(Convert.ToInt32(calc) * 1000);
                            Console.CursorVisible = true;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("> " + cmd[0] + ": " + cmd[1] + ": Too hight number");
                            return (1);
                        }
                    }
                    else
                    {
                        Console.WriteLine("> " + cmd[0] + ": " + cmd[1] + ": Invalid time argument");
                        return (1);
                    }
                }
            }
            else
            {
                Console.WriteLine("> wait: Invalid number of argument");
                return (1);
            }
            return (0);
        } //OK

        static private int execute_echo(string[] cmd)
        {
            for (int i = 1; i < cmd.Length; i++)
            {
                Console.WriteLine("> " + cmd[i]);
            }

            return (0);
        } //OK

        static private int execute_find(string[] cmd)
        {
            bool cancel = false;
            bool first_found = false;
            List<List<string>> results = new List<List<string>>();
            for (int i = 2; i < cmd.Length; i++)
            {
                results.Add(new List<string>());
            }
            
            if(cmd.Length > 1)
            {
                Console.CursorVisible = false;
                Console.Write("> ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write(cmd[0] + ": waiting for results...");
                Console.ResetColor();
                Console.SetCursorPosition(0, Console.CursorTop);
                if(!search_file_or_folder(cmd, ref results, Directory.GetCurrentDirectory(), ref cancel, ref first_found))
                {
                    print_n_space(cmd[0].Length + 26);
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write("> ");
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(cmd[0] + ": " + cmd[1] + ": No result found");
                    Console.ResetColor();
                }
                if(!cancel)
                {
                    for (int i = 0; i < results.Count; i++)
                    {
                        Console.Write("> ");
                        if (results[i].Count > 0)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine(cmd[0] + ": " + cmd[i + 2] + ":");
                            Console.ResetColor();
                            for (int j = 0; j < results[i].Count; j++)
                            {
                                Console.WriteLine("> " + results[i][j]);
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine(cmd[0] + ": " + cmd[i + 2] + ": No result found");
                            Console.ResetColor();
                        }
                    }
                }
                Console.CursorVisible = true;
            }
            else
            {
                Console.WriteLine("> find: Invalid number of argument");
                return (1);
            }
            return (0);
        } //OK

        static private int execute_time(string[] cmd, int refresh_timer)
        {
            Stopwatch clock = new Stopwatch();
            ConsoleKeyInfo key_pressed = new ConsoleKeyInfo();

            if(cmd.Length == 1)
            {
                Console.WriteLine("> " + DateTime.Now);
            }
            else if(cmd.Length == 2)
            {
                if (cmd[1] == "/chrono")
                {
                    Console.CursorVisible = false;
                    Console.Write("> press \"Enter\" to start chrono: " + clock.Elapsed);
                    do
                    {
                        key_pressed = Console.ReadKey(true);
                    }while(key_pressed.Key != ConsoleKey.Enter);
                    Console.SetCursorPosition(0,Console.CursorTop);
                    print_n_space(41);
                    Console.SetCursorPosition(0,Console.CursorTop);
                    Console.Write("> press \"Escape\" to stop chrono or \"Space\" to pause/play: " + clock.Elapsed);
                    clock.Start();
                    do
                    {
                        Thread.Sleep(refresh_timer);
                        Console.SetCursorPosition(58, Console.CursorTop);
                        print_n_space(16);
                        Console.SetCursorPosition(58, Console.CursorTop);
                        Console.Write((clock.Elapsed.Hours + 100).ToString().Substring(1, 2) + ":" + (clock.Elapsed.Minutes + 100).ToString().Substring(1, 2) + ":" + (clock.Elapsed.Seconds + 100).ToString().Substring(1, 2) + "." + (clock.Elapsed.Milliseconds));
                        if (Console.KeyAvailable)
                        {
                            key_pressed = Console.ReadKey(true);
                            if(key_pressed.Key == ConsoleKey.Spacebar)
                            {
                                clock.Stop();
                                do
                                {
                                    key_pressed = Console.ReadKey(true);
                                } while (key_pressed.Key != ConsoleKey.Spacebar && key_pressed.Key != ConsoleKey.Escape) ;
                                if(key_pressed.Key == ConsoleKey.Spacebar)
                                {
                                    clock.Start();
                                }
                            }
                        }
                    }while(key_pressed.Key != ConsoleKey.Escape);
                    clock.Stop();
                    Console.SetCursorPosition(0, Console.CursorTop);
                    print_n_space(Console.WindowWidth - 1);
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.WriteLine("> chrono stopped: " + (clock.Elapsed.Hours + 100).ToString().Substring(1, 2) + ":" + (clock.Elapsed.Minutes + 100).ToString().Substring(1, 2) + ":" + (clock.Elapsed.Seconds + 100).ToString().Substring(1, 2) + "." + (clock.Elapsed.Milliseconds));
                    Console.CursorVisible = true;
                }
                else if(cmd[1] == "/calendar")
                {
                    print_calendar();
                }
                else
                {
                    Console.WriteLine("> " + cmd[0] + ": " + cmd[1] + ": Invalid argument");
                    return (1);
                }
            }
            else if(cmd.Length == 3)
            {
                if(cmd[1] == "/delay")
                {
                    int total_time = 0;
                    int endtime_s = 0;
                    int endtime_m = 0;
                    int endtime_h = 0;

                    try
                    {
                        total_time = Convert.ToInt32(cmd[2]);
                    }
                    catch (Exception)
                    {
                        long calc = 0;
                        if (Calculator.is_math_expression(new string[1] { cmd[2] }) && Calculator.get_result(new string[1] { cmd[2] }, ref calc) == 0)
                        {
                            try
                            {
                                total_time = Convert.ToInt32(calc);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("> " + cmd[0] + " /delay: " + cmd[2] + ": Too hight number");
                                return (1);
                            }
                        }
                        else
                        {
                            Console.WriteLine("> " + cmd[0] + " /delay: " + cmd[2] + ": Invalid time argument");
                            return (1);
                        }
                    }
                    if(total_time >= 0)
                    {
                        endtime_h = total_time / 3600;
                        endtime_s = total_time % 3600;
                        endtime_m = endtime_s / 60;
                        endtime_s = endtime_s % 60;
                        Console.CursorVisible = false;
                        Console.Write("> press \"Enter\" to start: " + get_time_format(endtime_h, endtime_m, endtime_s));
                        do
                        {
                            key_pressed = Console.ReadKey(true);
                        } while (key_pressed.Key != ConsoleKey.Enter);
                        Console.SetCursorPosition(0, Console.CursorTop);
                        print_n_space(38);
                        Console.SetCursorPosition(0, Console.CursorTop);
                        Console.Write("> press \"Escape\" to stop or \"Space\" to pause/play: " + get_time_format(endtime_h, endtime_m, endtime_s));
                        clock.Start();
                        while ((endtime_s > 0 || endtime_m > 0 || endtime_h > 0) && key_pressed.Key != ConsoleKey.Escape)
                        {
                            endtime_s = total_time - ((int)clock.ElapsedMilliseconds / 1000);
                            endtime_h = endtime_s / 3600;
                            endtime_s = endtime_s % 3600;
                            endtime_m = endtime_s / 60;
                            endtime_s = endtime_s % 60;
                            Console.SetCursorPosition(51, Console.CursorTop);
                            print_n_space(12);
                            Console.SetCursorPosition(51, Console.CursorTop);
                            if(endtime_s >= 0 && endtime_m >= 0 && endtime_h >= 0)
                            {
                                Console.Write(get_time_format(endtime_h, endtime_m, endtime_s));
                            }
                            if(Console.KeyAvailable)
                            {
                                key_pressed = Console.ReadKey(true);
                                if (key_pressed.Key == ConsoleKey.Spacebar)
                                {
                                    clock.Stop();
                                    do
                                    {
                                        key_pressed = Console.ReadKey(true);
                                    } while (key_pressed.Key != ConsoleKey.Spacebar && key_pressed.Key != ConsoleKey.Escape);
                                    if (key_pressed.Key == ConsoleKey.Spacebar)
                                    {
                                        clock.Start();
                                    }
                                }
                            }
                            if(key_pressed.Key != ConsoleKey.Escape && (endtime_s > 0 || endtime_m > 0 || endtime_h > 0))
                            {
                                Thread.Sleep(1000);
                            }
                        }
                        clock.Stop();
                        Console.SetCursorPosition(0, Console.CursorTop);
                        print_n_space(Console.WindowWidth - 1);
                        Console.SetCursorPosition(0, Console.CursorTop);
                        if (endtime_s <= 0)
                        {
                            ShowApp(Assembly.GetExecutingAssembly().Location);
                            Console.WriteLine("> delay ended at " + get_time_format(DateTime.Now.Hour,DateTime.Now.Minute,DateTime.Now.Second));
                            Console.Beep(1000, 1);
                        }
                        else
                        {
                            if (endtime_s >= 0 && endtime_m >= 0 && endtime_h >= 0)
                            {
                                Console.WriteLine("> delay was canceled " + get_time_format(endtime_h, endtime_m, endtime_s) + " before end");
                            }
                        }
                        Console.CursorVisible = true;
                    }
                    else
                    {
                        Console.WriteLine("> " + cmd[0] + ": /delay: " + cmd[2] + ": Negative time argument");
                        return (1);
                    }
                }
                else if (cmd[1] == "/calendar")
                {
                    int time_arg = 0;

                    try
                    {
                        time_arg = Convert.ToInt32(cmd[2]);
                    }
                    catch(Exception)
                    {
                        long calc = 0;
                        if (Calculator.is_math_expression(new string[1] { cmd[2] }) && Calculator.get_result(new string[1] { cmd[2] }, ref calc) == 0)
                        {
                            try
                            {
                                time_arg = Convert.ToInt32(calc);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("> " + cmd[0] + " /calendar: " + cmd[2] + ": Too hight number");
                                return (1);
                            }
                        }
                        else
                        {
                            Console.WriteLine("> " + cmd[0] + " /calendar: " + cmd[2] + ": Invalid time argument");
                            return (1);
                        }
                    }
                    if (time_arg >= 1)
                    {
                        print_calendar(time_arg);
                    }
                    else
                    {
                        Console.WriteLine("> " + cmd[0] + " /calendar: " + cmd[2] + ": Too low number");
                        return (1);
                    }
                }
                else
                {
                    Console.WriteLine("> " + cmd[0] + ": " + cmd[1] + ": Invalid argument");
                    return (1);
                }
            }
            else
            {
                Console.WriteLine("> time: Invalid number of argument");
                return (1);
            }

            return (0);
        } //OK

        static private int execute_seed()
        {
            DriveInfo[] my_drive = DriveInfo.GetDrives();
            List<string> root_path = new List<string>();
            List<string> root_name = new List<string>();
            List<string> root_type = new List<string>();
            List<string> root_total_space = new List<string>();
            List<string> root_available_space = new List<string>();
            int nbr_space_path_name;
            int nbr_space_name_type;
            int nbr_space_type_total;
            int nbr_space_total_available;

            Console.CursorVisible = false;
            for (int i = 0; i < my_drive.Length; i++)
            {
                try
                {
                    root_path.Add(my_drive[i].Name);
                }
                catch (Exception) { root_path.Add(""); }
                try
                {
                    root_name.Add(my_drive[i].VolumeLabel);
                }
                catch (Exception) { root_name.Add(""); }
                try
                {
                    root_type.Add(my_drive[i].DriveType.ToString());
                }
                catch (Exception) { root_type.Add(""); }
                try
                {
                    root_total_space.Add(Math.Round((double)my_drive[i].TotalSize / (1 << 30), 2) + "Go");
                }
                catch (Exception) { root_total_space.Add(""); }
                try
                {
                    root_available_space.Add(Math.Round((double)my_drive[i].AvailableFreeSpace / (1 << 30), 2) + "Go (" + (my_drive[i].AvailableFreeSpace * 100 / my_drive[i].TotalSize * 100) / 100 + "%)");
                }
                catch (Exception) { root_available_space.Add(""); }
            }
            nbr_space_path_name = get_longest_elt(root_path, 4); //4 = longueur de "root"
            nbr_space_name_type = get_longest_elt(root_name, 4); //4 = longueur de "name"
            nbr_space_type_total = get_longest_elt(root_type, 8); //8 = longueur de "hardware"
            nbr_space_total_available = get_longest_elt(root_total_space, 4); //4 = longueur de "size"
            Console.Write("> ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("Root");
            print_n_space(nbr_space_path_name - 3);
            Console.Write("Name");
            print_n_space(nbr_space_name_type - 3);
            Console.Write("Hardware");
            print_n_space(nbr_space_type_total - 7);
            Console.Write("Size");
            print_n_space(nbr_space_total_available - 3);
            Console.WriteLine("Available");
            Console.ResetColor();
            for (int i = 0; i < my_drive.Length; i++)
            {
                Console.Write("> " + root_path[i]);
                print_n_space(nbr_space_path_name + 1 - root_path[i].Length);
                Console.Write(root_name[i]);
                print_n_space(nbr_space_name_type + 1 - root_name[i].Length);
                Console.Write(root_type[i]);
                print_n_space(nbr_space_type_total + 1 - root_type[i].Length);
                Console.Write(root_total_space[i]);
                print_n_space(nbr_space_total_available + 1 - root_total_space[i].Length);
                Console.WriteLine(root_available_space[i]);
            }

            Console.CursorVisible = true;

            return (0);
        } //OK

        static private int execute_network(string[] cmd)
        {
            List<string> all_networks_name = new List<string>();
            List<string> all_networks_signal = new List<string>();
            List<string> all_networks_cryptage = new List<string>();
            List<string> all_networks_security = new List<string>();
            List<string> all_networks_status = new List<string>();
            int nbr_space_name_signal;
            int nbr_space_signal_cryptage;
            int nbr_space_cryptage_security;
            int attempts = 0;

            if(cmd.Length > 0 && cmd.Length < 3)
            {
                if(cmd.Length == 1 || (cmd.Length == 2 && cmd[1] == "/connected"))
                {
                    while(attempts < 10)
                    {
                        try
                        {
                            WlanClient client = new WlanClient();

                            foreach (WlanClient.WlanInterface wlanInterface in client.Interfaces)
                            {
                                Wlan.WlanAvailableNetwork[] networks = wlanInterface.GetAvailableNetworkList(0);
                                foreach (Wlan.WlanAvailableNetwork network in networks)
                                {
                                    if (cmd.Length == 1 || (cmd[1] == "/connected" && network.flags.ToString() != "0"))
                                    {
                                        ListViewItem item = new ListViewItem(Encoding.ASCII.GetString(network.dot11Ssid.SSID, 0, (int)network.dot11Ssid.SSIDLength));
                                        all_networks_name.Add(Encoding.ASCII.GetString(network.dot11Ssid.SSID, 0, (int)network.dot11Ssid.SSIDLength));
                                        all_networks_signal.Add(network.wlanSignalQuality + "%");
                                        all_networks_cryptage.Add(network.dot11DefaultCipherAlgorithm.ToString());
                                        all_networks_security.Add(network.securityEnabled.ToString());
                                        all_networks_status.Add(network.flags.ToString());
                                        if (all_networks_name[all_networks_name.Count - 1] == "")
                                        {
                                            all_networks_name[all_networks_name.Count - 1] = "Private Network";
                                        }
                                    }
                                }
                            }
                            attempts = 11;
                        }
                        catch
                        {
                            attempts++;
                            if(attempts < 10)
                            {
                                Thread.Sleep(500);
                            }
                        }
                        if(attempts < 10 && Console.KeyAvailable)
                        {
                            if(Console.ReadKey(true).Key == ConsoleKey.Escape)
                            {
                                attempts = 12;
                            }
                        }
                    }
                    
                    try
                    {
                        if (NetworkInterface.GetIsNetworkAvailable())
                        {
                            NetworkInterface[] all_networkdevices = NetworkInterface.GetAllNetworkInterfaces();
                            for (int i = 0; i < all_networkdevices.Length; i++)
                            {
                                if (all_networkdevices[i].OperationalStatus == OperationalStatus.Up && (all_networkdevices[i].NetworkInterfaceType == NetworkInterfaceType.Ethernet || all_networkdevices[i].NetworkInterfaceType == NetworkInterfaceType.Ethernet3Megabit || all_networkdevices[i].NetworkInterfaceType == NetworkInterfaceType.FastEthernetFx || all_networkdevices[i].NetworkInterfaceType == NetworkInterfaceType.FastEthernetT))
                                {
                                    if (cmd.Length == 1 || cmd[1] == "/connected")
                                    {
                                        all_networks_name.Add("Wired Connection");
                                        all_networks_signal.Add("100%");
                                        all_networks_cryptage.Add("None");
                                        all_networks_security.Add("False");
                                        all_networks_status.Add("Connected");
                                    }
                                }
                            }
                        }
                    }
                    catch(Exception)
                    {

                    }
                    
                    if (all_networks_name.Count > 0)
                    {
                        nbr_space_name_signal = get_longest_elt(all_networks_name, 4); //4 = "Name".length
                        nbr_space_signal_cryptage = get_longest_elt(all_networks_signal, 6); //6 = "Signal".length
                        nbr_space_cryptage_security = get_longest_elt(all_networks_cryptage, 10); //10 = "Encryption".length

                        Console.Write("> ");
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("Name");
                        print_n_space(nbr_space_name_signal - 3);
                        Console.Write("Signal");
                        print_n_space(nbr_space_signal_cryptage - 5);
                        Console.Write("Encryption");
                        print_n_space(nbr_space_cryptage_security - 9);
                        Console.WriteLine("Secured");
                        Console.ResetColor();

                        sort_by_connection_status(ref all_networks_name, ref all_networks_signal, ref all_networks_cryptage, ref all_networks_security, ref all_networks_status);

                        for (int i = 0; i < all_networks_name.Count; i++)
                        {
                            Console.Write("> ");
                            if (all_networks_status[i] != "0")
                            {
                                Console.ForegroundColor = ConsoleColor.DarkGreen;
                            }
                            Console.Write(all_networks_name[i]);
                            print_n_space(nbr_space_name_signal + 1 - all_networks_name[i].Length);
                            Console.Write(all_networks_signal[i]);
                            print_n_space(nbr_space_signal_cryptage + 1 - all_networks_signal[i].Length);
                            Console.Write(all_networks_cryptage[i]);
                            print_n_space(nbr_space_cryptage_security + 1 - all_networks_cryptage[i].Length);
                            Console.WriteLine(all_networks_security[i]);
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        Console.WriteLine("> network: no signal found");
                    }
                }
                else
                {
                    Console.WriteLine("> " + cmd[0] + ": " + cmd[1] + ": Invalid argument");
                    return (1);
                }
            }
            else
            {
                Console.WriteLine("> network: Invalid number of argument");
                return (1);
            }

            return (0);
        } //OK

        static private int execute_exit(ref bool working)
        {
            working = false;
            return (0);
        } //OK

        static private int execute_unknown_cmd(string[] cmd)
        {
            Console.WriteLine("> " + cmd[0] + ": Unknown command");
            return (1);
        } //OK

        static private int execute_info()
        {
            Computer my_computer = new Computer();
            DriveInfo[] my_drive = DriveInfo.GetDrives();
            long total_drive_space = 0;
            long total_free_drive_space = 0;

            Console.CursorVisible = false;
            for (int i = 0; i < my_drive.Length; i++)
            {
                try
                {
                    total_drive_space = total_drive_space + my_drive[i].TotalSize;
                    total_free_drive_space = total_free_drive_space + my_drive[i].TotalFreeSpace;
                }
                catch (Exception)
                {

                }
            }
            Console.CursorVisible = true;

            Console.WriteLine("> USER:             " + Environment.UserName);
            Console.WriteLine("> COMPUTER:         " + Environment.MachineName);
            Console.Write("> OPERATING SYSTEM: " + my_computer.Info.OSFullName);
            if(Environment.Is64BitOperatingSystem)
            {
                Console.WriteLine(" 64bits (" + my_computer.Info.OSVersion + ")");
            }
            else
            {
                Console.WriteLine(" 32bits (" + my_computer.Info.OSVersion + ")");
            }
            Console.WriteLine("> MEMORY:           " + Math.Round((double)total_drive_space / (1 << 30), 2) + "Go");
            Console.WriteLine("> AVAILABLE SPACE:  " + Math.Round((double)total_free_drive_space / (1 << 30), 2) + "Go (" + (total_free_drive_space * 100 / total_drive_space * 100) / 100 + "%)");
            Console.WriteLine("> RAM:              " + Math.Round((double)my_computer.Info.TotalPhysicalMemory / (1 << 30), 2) + "Go");
            Console.WriteLine("> SHELL VERSION:    " + version.Substring(15, version.Length - 15));
            Console.WriteLine("> EDITOR:           " + editor);
            return (0);
        } //OK

        static private int execute_admin(ref bool superuser, string appdata_dir) //OK
        {
            if (!superuser)
            {
                Program.admin_connection_protocol(ref superuser, appdata_dir);
            }
            else
            {
                Console.WriteLine("> /admin: already logged as admin");
            }
            return (0);
        }

        static private int execute_unadmin(ref bool superuser)
        {
            superuser = false;
            return (0);
        } //OK

        static private int execute_help(string[] cmd, string appdata_dir)
        {
            if(cmd.Length == 1)
            {
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("9gag"); Console.ResetColor(); Console.WriteLine(": loads a 9gag page in default browser.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("allocine"); Console.ResetColor(); Console.WriteLine(": loads an allocine page in default browser.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("bing"); Console.ResetColor(); Console.WriteLine(": loads a bing page or research in default browser.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("cat"); Console.ResetColor(); Console.WriteLine(": displays content of the file.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("cd"); Console.ResetColor(); Console.WriteLine(": travels to a directory.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("clear"); Console.ResetColor(); Console.WriteLine(": clears console.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("cp"); Console.ResetColor(); Console.WriteLine(": copies a file.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("cpdir"); Console.ResetColor(); Console.WriteLine(": copies a folder");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("dailymotion"); Console.ResetColor(); Console.WriteLine(": loads a dailymotion page in default browser.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("echo"); Console.ResetColor(); Console.WriteLine(": rewrites arguments in cmd Linux.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("exit"); Console.ResetColor(); Console.WriteLine(": leaves cmd Linux.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("facebook"); Console.ResetColor(); Console.WriteLine(": loads a facebook page in default browser.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("find"); Console.ResetColor(); Console.WriteLine(": displays all file or directory who match with arguments.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("gmail"); Console.ResetColor(); Console.WriteLine(": loads a gmail page in default browser.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("google"); Console.ResetColor(); Console.WriteLine(": loads a google page or research in default browser.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("home"); Console.ResetColor(); Console.WriteLine(": travels to the start folder.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("hostname"); Console.ResetColor(); Console.WriteLine(": gives the name of the computer.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("hotmail"); Console.ResetColor(); Console.WriteLine(": loads a hotmail page in default browser.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("info"); Console.ResetColor(); Console.WriteLine(": displays system informations.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("launch"); Console.ResetColor(); Console.WriteLine(": starts an application.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("lock"); Console.ResetColor(); Console.WriteLine(": locks cmd linux.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("lolapp"); Console.ResetColor(); Console.WriteLine(": executes LoLapp (Kelmatou apps).");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("ls"); Console.ResetColor(); Console.WriteLine(": displays content of the directory.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("man"); Console.ResetColor(); Console.WriteLine(": gives detailed information about commands.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("map"); Console.ResetColor(); Console.WriteLine(": loads a google map page or research in default browser.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("mv"); Console.ResetColor(); Console.WriteLine(": copies a file and remove the original file.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("mvdir"); Console.ResetColor(); Console.WriteLine(": copies a directory and remove the original directory.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("mkdir"); Console.ResetColor(); Console.WriteLine(": creates a new directory.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("network"); Console.ResetColor(); Console.WriteLine(": displays network information.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("news"); Console.ResetColor(); Console.WriteLine(": loads a google news page or research in default browser.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("option"); Console.ResetColor(); Console.WriteLine(": changes setting of cmd Linux [MASTER].");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("outlook"); Console.ResetColor(); Console.WriteLine(": loads an outlook page in default browser.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("password"); Console.ResetColor(); Console.WriteLine(": sets a new password [MASTER].");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("pwd"); Console.ResetColor(); Console.WriteLine(": displays current path.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("rename"); Console.ResetColor(); Console.WriteLine(": renames a file or a directory.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("reset"); Console.ResetColor(); Console.WriteLine(": erases all data saved from cmd linux [MASTER].");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("resize"); Console.ResetColor(); Console.WriteLine(": resizes console.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("rm"); Console.ResetColor(); Console.WriteLine(": removes the file.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("rmdir"); Console.ResetColor(); Console.WriteLine(": removes the directory.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("script"); Console.ResetColor(); Console.WriteLine(": executes a script made before.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("seed"); Console.ResetColor(); Console.WriteLine(": displays all devices.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("shutdown"); Console.ResetColor(); Console.WriteLine(": shutdowns your computer.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("skype"); Console.ResetColor(); Console.WriteLine(": launches Skype.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("sl"); Console.ResetColor(); Console.WriteLine(": don't fail writing \"ls\"!");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("taskkill"); Console.ResetColor(); Console.WriteLine(": leaves an application.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("tasklist"); Console.ResetColor(); Console.WriteLine(": shows all processes running.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("textedit"); Console.ResetColor(); Console.WriteLine(": edit your own text file.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("time"); Console.ResetColor(); Console.WriteLine(": shows current time.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("touch"); Console.ResetColor(); Console.WriteLine(": creates the file if it doesn't exist.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("translate"); Console.ResetColor(); Console.WriteLine(": loads a google traduction page in default browser.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("ts"); Console.ResetColor(); Console.WriteLine(": launches Team Speak.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("tv"); Console.ResetColor(); Console.WriteLine(": loads a tv program page in default browser.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("twitter"); Console.ResetColor(); Console.WriteLine(": loads a twitter page in default browser.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("uninstall"); Console.ResetColor(); Console.WriteLine(": opens the control center for apps [MASTER].");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("url"); Console.ResetColor(); Console.WriteLine(": loads an url in default browser.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("wait"); Console.ResetColor(); Console.WriteLine(": disables cmd Linux for a duration in seconds.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("weather"); Console.ResetColor(); Console.WriteLine(": loads a weather.com page in default browser.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("whoami"); Console.ResetColor(); Console.WriteLine(": gives the name of the user.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("wikipedia"); Console.ResetColor(); Console.WriteLine(": loads a wikipedia page or research in default browser.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("yahoo"); Console.ResetColor(); Console.WriteLine(": loads a yahoo page or research in default browser.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("youtube"); Console.ResetColor(); Console.WriteLine(": loads a youtube page or research in default browser.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("/admin"); Console.ResetColor(); Console.WriteLine(": logs you as MASTER.");
                Console.Write("> "); Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("/unadmin"); Console.ResetColor(); Console.WriteLine(": logs you out of MASTER.");

            }
            else if(cmd.Length > 1)
            {
                for(int i = 1; i < cmd.Length; i++)
                {
                    switch(cmd[i])
                    {
                        case ("cd"):
                            Console.WriteLine("> cd: travels to a directory.\n>       . to go to the current directory.\n>       .. to go to the parent directory.\n>       - to go to the previous directory.\n>       + to go to the next directory.");
                            Console.WriteLine("> Ex: cd C:/Users/...");
                            break;
                        case ("ls"):
                            Console.WriteLine("> ls: shows content of the directory.");
                            Console.WriteLine("> Ex: ls C:/Users/...");
                            break;
                        case ("touch"):
                            Console.WriteLine("> touch: creates the file if it doesn't exist.");
                            Console.WriteLine("> Ex: touch C:/Users/.../file");
                            break;
                        case ("rm"):
                            Console.WriteLine("> rm: removes the file.");
                            Console.WriteLine("> Ex: rm C:/Users/.../file");
                            break;
                        case ("rmdir"):
                            Console.WriteLine("> rmdir: removes the directory.");
                            Console.WriteLine("> Ex: rmdir C:/Users/.../folder/");
                            break;
                        case ("mkdir"):
                            Console.WriteLine("> mkdir: creates a new directory.");
                            Console.WriteLine("> Ex: mkdir \"New Folder\"");
                            break;
                        case ("pwd"):
                            Console.WriteLine("> pwd: displays current path.");
                            Console.WriteLine("> Ex: pwd");
                            break;
                        case ("clear"):
                            Console.WriteLine("> clear: clears console.");
                            Console.WriteLine("> Ex: clear");
                            break;
                        case ("whoami"):
                            Console.WriteLine("> whoami: gives the name of the user.");
                            Console.WriteLine("> Ex: whoami");
                            break;
                        case ("hostname"):
                            Console.WriteLine("> hostname: gives the name of the computer.");
                            Console.WriteLine("> Ex: hostname");
                            break;
                        case ("cat"):
                            Console.WriteLine("> cat: shows content of the file.");
                            Console.WriteLine("> Ex: cat C:/Users/.../file");
                            break;
                        case("help"):
                            Console.WriteLine("> help: gives tips about commands.");
                            Console.WriteLine("> Ex: help cd");
                            break;
                        case("exit"):
                            Console.WriteLine("> exit: leaves cmd Linux.");
                            Console.WriteLine("> Ex: exit");
                            break;
                        case ("/admin"):
                            Console.WriteLine("> /admin: logs you as MASTER.");
                            Console.WriteLine("> Ex: /admin");
                            break;
                        case ("/unadmin"):
                            Console.WriteLine("> /unadmin: logs you out of MASTER.");
                            Console.WriteLine("> Ex: /unadmin");
                            break;
                        case ("launch"):
                            Console.WriteLine("> launch: starts an application.");
                            Console.WriteLine("> Ex: launch \"cmd Linux\"");
                            break;
                        case ("shutdown"):
                            Console.WriteLine("> shutdown: shutdowns your computer.\n>       /a to cancel.\n>       /g to restart with app launched.\n>       /h to hibernate.\n>       /l to leave session.\n>       /p shutdown without notification.\n>       /r to restart.\n>       /t XX (s) to set a delay.\n>       now to shutdown without delay.");
                            Console.WriteLine("> Ex: shutdown");
                            break;
                        case ("info"):
                            Console.WriteLine("> info: displays system informations.");
                            Console.WriteLine("> Ex: info");
                            break;
                        case ("cp"):
                            Console.WriteLine("> cp: copies a file.");
                            Console.WriteLine("> Ex: cp C:/Users/.../file C:/Users/.../folder");
                            break;
                        case ("cpdir"):
                            Console.WriteLine("> cpdir: copies a folder.");
                            Console.WriteLine("> Ex: cpdir C:/Users/.../folder C:/Users/.../folder2");
                            break;
                        case ("resize"):
                            Console.WriteLine("> resize: resizes console.\n>       /default to reset size.\n>       /full to set maximum size.");
                            Console.WriteLine("> Ex: resize 42 42; resize (width height)");
                            break;
                        case ("taskkill"):
                            Console.WriteLine("> taskkill: leaves an application.");
                            Console.WriteLine("> Ex: taskkill \"cmd Linux.exe\"");
                            break;
                        case ("home"):
                            Console.WriteLine("> home: travels to the start folder.");
                            Console.WriteLine("> Ex: home");
                            break;
                        case ("url"):
                            Console.WriteLine("> url: loads an url in default browser.");
                            Console.WriteLine("> Ex: url http//website.com");
                            break;
                        case ("facebook"):
                            Console.WriteLine("> facebook: loads a facebook page in default browser.");
                            Console.WriteLine("> Ex: facebook");
                            break;
                        case ("twitter"):
                            Console.WriteLine("> twitter: loads a twitter page in default browser.");
                            Console.WriteLine("> Ex: twitter");
                            break;
                        case ("google"):
                            Console.WriteLine("> google: loads a google page or research in default browser.");
                            Console.WriteLine("> Ex: google;google \"how to use cmd Linux\"");
                            break;
                        case ("youtube"):
                            Console.WriteLine("> youtube: loads a youtube page or research in default browser.");
                            Console.WriteLine("> Ex: youtube;youtube \"tuto: how to use cmd Linux\"");
                            break;
                        case ("wikipedia"):
                            Console.WriteLine("> wikipedia: loads a wikipedia page or research in default browser.\n>       /language research (default is english).");
                            Console.WriteLine("> Ex: wikipedia;wikipedia \"Eiffel Tower\";wikipedia /french \"Eiffel Tower\"");
                            break;
                        case ("9gag"):
                            Console.WriteLine("> 9gag: loads a 9gag page in default browser.");
                            Console.WriteLine("> Ex: 9gag");
                            break;
                        case ("gmail"):
                            Console.WriteLine("> gmail: loads a gmail page in default browser.");
                            Console.WriteLine("> Ex: gmail");
                            break;
                        case ("map"):
                            Console.WriteLine("> map: loads a google maps page or research in default browser.");
                            Console.WriteLine("> Ex: map;map Paris");
                            break;
                        case ("allocine"):
                            Console.WriteLine("> allocine: loads a allocine page in default browser.");
                            Console.WriteLine("> Ex: allocine");
                            break;
                        case ("dailymotion"):
                            Console.WriteLine("> dailymotion: loads a dailymotion page in default browser.");
                            Console.WriteLine("> Ex: dailymotion");
                            break;
                        case ("yahoo"):
                            Console.WriteLine("> yahoo: loads a yahoo page or research in default browser.");
                            Console.WriteLine("> Ex: yahoo");
                            break;
                        case ("outlook"):
                            Console.WriteLine("> outlook: loads a outlook page in default browser.");
                            Console.WriteLine("> Ex: outlook");
                            break;
                        case ("hotmail"):
                            Console.WriteLine("> hotmail: loads a hotmail page in default browser.");
                            Console.WriteLine("> Ex: hotmail");
                            break;
                        case ("translate"):
                            Console.WriteLine("> translate: loads a google traduction page in default browser.\n>       /language1 /language2 expression_to_translate.");
                            Console.WriteLine("> Ex: translate /english /french \"hello word!\"");
                            break;
                        case ("bing"):
                            Console.WriteLine("> bing: loads a bing page or research in default browser.");
                            Console.WriteLine("> Ex: bing");
                            break;
                        case ("lolapp"):
                            Console.WriteLine("> lolapp: executes LoLapp (Kelmatou apps).\n>       /champion champion\n>       /game summoner region.\n>       /history summoner region.\n>       /profile summoner region.\n>       /team summoner region.");
                            Console.WriteLine("> Ex: lolapp; lolapp /profile Faker Kr");
                            break;
                        case ("skype"):
                            Console.WriteLine("> skype: launches Skype.");
                            Console.WriteLine("> Ex: skype");
                            break;
                        case ("ts"):
                            Console.WriteLine("> ts: launches Team Speak.");
                            Console.WriteLine("> Ex: ts");
                            break;
                        case ("script"):
                            Console.WriteLine("> script: executes a script made before.\n>       /display to show script content.\n>       /export to export scripts.\n>       /import to import scripts.\n>          /d to don't replace same name scripts.\n>          /f to replace all same name scripts.\n>       /mk to creat a new script.\n>       /rename to change a script name.\n>       /rm to remove a script.");
                            Console.WriteLine("> Ex: script my_script;script /mk new_script;script /rm old_script");
                            break;
                        case ("textedit"):
                            Console.WriteLine("> textedit: edit your own text file.");
                            Console.WriteLine("> Ex: textedit");
                            break;
                        case ("uninstall"):
                            Console.WriteLine("> uninstall: opens the control center for apps [MASTER].");
                            Console.WriteLine("> Ex: uninstall");
                            break;
                        case ("reset"):
                            Console.WriteLine("> reset: erases all data saved from cmd linux [MASTER].\n>       /genius \"cmd1\" \"cmd2\" to remove propositions of a command.\n>       /stats to reset genius stats.");
                            Console.WriteLine("> Ex: reset; reset /genius cd touch");
                            break;
                        case ("tasklist"):
                            Console.WriteLine("> tasklist: shows all processes running.");
                            Console.WriteLine("> Ex: tasklist");
                            break;
                        case ("mv"):
                            Console.WriteLine("> mv: copies a file and remove the original file.");
                            Console.WriteLine("> Ex: mv file C:/.../");
                            break;
                        case ("mvdir"):
                            Console.WriteLine("> mvdir: copies a directory and remove the original directory.");
                            Console.WriteLine("> Ex: mvdir: C:/.../folder C:/.../");
                            break;
                        case ("sl"):
                            Console.WriteLine("> sl: don't fail writing \"ls\"!\n>       /drake to get a dragon.\n>       /plane to get a plane.\n>       /tank to get a tank.\n>       /train to get a train.");
                            Console.WriteLine("> Ex: sl");
                            break;
                        case ("rename"):
                            Console.WriteLine("> rename: renames a file or a directory.");
                            Console.WriteLine("> Ex: rename oldname newname");
                            break;
                        case ("password"):
                            Console.WriteLine("> password: sets a new password [MASTER].");
                            Console.WriteLine("> Ex: password");
                            break;
                        case ("lock"):
                            Console.WriteLine("> lock: locks cmd linux.");
                            Console.WriteLine("> Ex: lock password");
                            break;
                        case ("option"):
                            Console.WriteLine("> option: changes setting of cmd Linux [MASTER].");
                            Console.WriteLine("> Ex: option");
                            break;
                        case ("wait"):
                            Console.WriteLine("> wait: disables cmd Linux for a duration in seconds.");
                            Console.WriteLine("> Ex: wait 42");
                            break;
                        case ("echo"):
                            Console.WriteLine("> echo: rewrites arguments in cmd Linux.");
                            Console.WriteLine("> Ex: echo nomy");
                            break;
                        case ("find"):
                            Console.WriteLine("> find: displays all file or directory who match with arguments.");
                            Console.WriteLine("> Ex: find LoLapp.exe LoLapp_folder");
                            break;
                        case ("weather"):
                            Console.WriteLine("> weather: loads a weather.com page in default browser.");
                            Console.WriteLine("> Ex: weather");
                            break;
                        case ("time"):
                            Console.WriteLine("> time: shows current time.\n>       /calendar [xx<) to show calendar.\n>       /chrono to set a chrono.\n>       /delay XX (s) to set an account rebour.");
                            Console.WriteLine("> Ex: time;time /delay 42");
                            break;
                        case ("seed"):
                            Console.WriteLine("> seed: displays all devices.");
                            Console.WriteLine("> Ex: seed");
                            break;
                        case ("network"):
                            Console.WriteLine("> network: displays network information.\n>       /connected only displays connected network.");
                            Console.WriteLine("> Ex: network");
                            break;
                        case ("news"):
                            Console.WriteLine("> news: loads a google news page or research in default browser.");
                            Console.WriteLine("> Ex: news;news Paris");
                            break;
                        case ("tv"):
                            Console.WriteLine("> tv: : loads a tv program page in default browser.");
                            Console.WriteLine("> Ex: tv");
                            break;
                        case ("man"):
                            Console.WriteLine("> man: gives detailed information about commands.");
                            Console.WriteLine("> Ex: man man");
                            break;
                        default:
                            if(File.Exists(appdata_dir + "/script_files/" + cmd[i]))
                            {
                                string[][] arg = null;
                                execute_script(new string[3] { "script", "/display", cmd[i] }, appdata_dir, ref arg, -1);
                            }
                            else
                            {
                                Console.WriteLine("> " + cmd[i] + ": Unknown command.");
                            }
                            break;
                            
                    }
                }
            }
            else
            {
                Console.WriteLine("> help: Invalid number of arguments");
                return (1);
            }

            return (0);
        } //OK NEEDS MAJ EVERYTIME

        static private int execute_man(string[] cmd)
        {
            if (cmd.Length > 1)
            {
                Console.CursorVisible = false;
                for (int i = 1; i < cmd.Length; i++)
                {
                    Console.Clear();
                    Console.Write(">\n>\n>");
                    switch (cmd[i])
                    {
                        case ("/admin"):
                            print_n_space(Console.WindowWidth / 2 - 8);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN /ADMIN <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": change user status as MASTER of cmd Linux.\n>              That allows you to use some restricted commands.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": /admin");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("/unadmin"):
                            print_n_space(Console.WindowWidth / 2 - 8);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN /UNADMIN <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": change user status as USER of cmd Linux.\n>              You won't be able to use MASTER commands.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": /unadmin");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("9gag"):
                            print_n_space(Console.WindowWidth / 2 - 7);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN 9GAG <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": launch default browser and go to 9gag.com.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": 9gag");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("allocine"):
                            print_n_space(Console.WindowWidth / 2 - 9);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN ALLOCINE <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": launch default browser and go to allocine.com.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": allocine");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("bing"):
                            print_n_space(Console.WindowWidth / 2 - 7);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN BING <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": launch default browser and go to bing.com.\n>              You can also search on bing by adding arguments.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": bing [KEYWORDS*]");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("cat"):
                            print_n_space(Console.WindowWidth / 2 - 6);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN CAT <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": display content of a file.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": cat {FILE*}");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("cd"):
                            print_n_space(Console.WindowWidth / 2 - 6);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN CD <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": change working directory.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": cd {-+ ; ++ ; DIRECTORY+}");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       . to go to the current directory\n>       .. to go to the parent directory\n>       - to go to the previous directory\n>       + to go to the next directory");
                            break;
                        case ("clear"):
                            print_n_space(Console.WindowWidth / 2 - 7);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN CLEAR <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": clear cmd Linux's buffer.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": clear");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("cp"):
                            print_n_space(Console.WindowWidth / 2 - 6);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN CP <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": copy file.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": cp {FILE [FOLDER]}");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       FILE argument is file to copy\n>       FOLDER is destination (default is current).\n>       if directory or file of the same name exists, it will change its name.");
                            break;
                        case ("cpdir"):
                            print_n_space(Console.WindowWidth / 2 - 7);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN CPDIR <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": copy folder.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": cpdir {FOLDER [FOLDER]}");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       first FOLDER argument is folder to copy\n>       second FOLDER is destination (default is current).\n>       prevent copying folder into itself.\n>       if directory or file of the same name exists, it will change its name.");
                            break;
                        case ("dailymotion"):
                            print_n_space(Console.WindowWidth / 2 - 10);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN DAILYMOTION <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": launch default browser and go to dailymotion.com.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": dailymotion");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("echo"):
                            print_n_space(Console.WindowWidth / 2 - 7);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN ECHO <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": write in cmd Linux argument(s).");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": echo [KEYWORD*]");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       go back to line beginning for each argument.");
                            break;
                        case ("exit"):
                            print_n_space(Console.WindowWidth / 2 - 7);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN EXIT <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": stop cmd Linux's current process.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": exit");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("facebook"):
                            print_n_space(Console.WindowWidth / 2 - 9);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN FACEBOOK <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": launch default browser and go to facebook.com.\n>              You can also search on facebook by adding arguments.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": facebook [KEYWORD*]");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("find"):
                            print_n_space(Console.WindowWidth / 2 - 7);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN FIND <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": find files or folders which have the same name\n>              into current directory and subdirectories.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": find {KEYWORD*}");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       pressing ESCAPE key will cancel search.");
                            break;
                        case ("gmail"):
                            print_n_space(Console.WindowWidth / 2 - 7);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN GMAIL <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": launch default browser and go to gmail.com.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": gmail");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("google"):
                            print_n_space(Console.WindowWidth / 2 - 8);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN GOOGLE <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": launch default browser and go to google.com.\n>              You can also search on google by adding arguments.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": google [KEYWORD*]");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("help"):
                            print_n_space(Console.WindowWidth / 2 - 7);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN HELP <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": five simple tips about commands.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": help [COMMAND* ; SCRIPT*]");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       help without argument will display short help for all commands.\n>       using '?' after a command or script will display help.\n>       script help shows content of this script.");
                            break;
                        case ("home"):
                            print_n_space(Console.WindowWidth / 2 - 7);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN HOME <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": return to desktop.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": home");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       if desktop doesn't exist, it will go to cmd Linux application folder.");
                            break;
                        case ("hostname"):
                            print_n_space(Console.WindowWidth / 2 - 9);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN HOSTNAME <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": display user name of the current session.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": hostname");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("hotmail"):
                            print_n_space(Console.WindowWidth / 2 - 8);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN HOTMAIL <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": launch default browser and go to hotmail.com.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": hotmail");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("info"):
                            print_n_space(Console.WindowWidth / 2 - 7);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN INFO <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": display diverse information.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": info");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("launch"):
                            print_n_space(Console.WindowWidth / 2 - 8);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN LAUNCH <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": execute file or folder.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": launch [FILE* ; FOLDER*]");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       launch without argument starts a new cmd Linux window.\n>       launch a folder will open the graphic version in explorer.");
                            break;
                        case ("lock"):
                            print_n_space(Console.WindowWidth / 2 - 7);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN LOCK <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": lock cmd Linux.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": lock [KEYWORD]");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       lock without argument can be unlock by pressing one key and waiting.\n>       KEYWORD argument will be the password to unlock.");
                            break;
                        case ("lolapp"):
                            print_n_space(Console.WindowWidth / 2 - 8);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN LOLAPP <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": use LoLapp (Kelmatou Apps)\n>              to give information about League of Legends.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": lolapp [LOL_PARAMETER {KEYWORD KEYWORD}]");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       first KEYWORD must be summoner name, second must be region.\n>       /champion: champion information.\n>       /game: current game information.\n>       /history: history of summoner.\n>       /profile: summoner profile.\n>       /team: summoner team.");
                            break;
                        case ("ls"):
                            print_n_space(Console.WindowWidth / 2 - 6);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN LS <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": display all content of a directory");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": ls [FOLDER* ; FILE*]");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       FILE argument will only display file if it exists.");
                            break;
                        case ("man"):
                            print_n_space(Console.WindowWidth / 2 - 6);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN MAN <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": gives detailed information about commands.\n>              It provides syntax, arguments and option you can use.\n>              Scripts are not taken into account.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": man {COMMAND*}");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       * means that you can repeat argument type an unlimited amount of time.\n>       + means that argument must be present at least once.\n>       {} is the required part of command's arguments.\n>       [] is the optionnal part of command's arguments.\n>       ; separates different type of argument.");
                            break;
                        case ("map"):
                            print_n_space(Console.WindowWidth / 2 - 6);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN MAP <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": launch default browser and go to google.map.com.\n>              You can also search on google map by adding arguments.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": map [KEYWORD*]");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("mkdir"):
                            print_n_space(Console.WindowWidth / 2 - 7);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN MKDIR <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": create new folder(s).");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": mkdir {KEYWORD*}");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("mv"):
                            print_n_space(Console.WindowWidth / 2 - 6);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN MV <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": move file's place.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": mv {FILE FOLDER}");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       FILE argument is file to move, FOLDER is destination.\n>       if file or folder of the same name exists, it will change its name.");
                            break;
                        case ("mvdir"):
                            print_n_space(Console.WindowWidth / 2 - 7);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN MVDIR <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": move folder's place.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": mvdir {FOLDER FOLDER}");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       first FOLDER argument is folder to move, second is destination.\n>       prevent moving folder into itself.\n>       if directory or file of the same name exists, it will change its name.");
                            break;
                        case ("network"):
                            print_n_space(Console.WindowWidth / 2 - 8);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN NETWORK <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": display all networks (wired or not).");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": network [NETWORK_PARAMETER]");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       /connected: only display connected network");
                            break;
                        case ("news"):
                            print_n_space(Console.WindowWidth / 2 - 7);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN NEWS <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": launch default browser and go to google.news.com.\n>              You can also search on google news by adding arguments.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": news [KEYWORD*]");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("option"):
                            print_n_space(Console.WindowWidth / 2 - 8);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN OPTION <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": open option menu.\n>              then you can change settings of cmd Linux. [MASTER]");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": option");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("outlook"):
                            print_n_space(Console.WindowWidth / 2 - 8);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN OUTLOOK <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": launch default browser and go to outlook.com.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": outlook");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("password"):
                            print_n_space(Console.WindowWidth / 2 - 9);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN PASSWORD <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": change or set new password. [MASTER]");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": password");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       MASTER mode is not required for initialization.");
                            break;
                        case ("pwd"):
                            print_n_space(Console.WindowWidth / 2 - 6);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN PWD <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": print current directory.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": pwd");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("rename"):
                            print_n_space(Console.WindowWidth / 2 - 8);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN RENAME <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": rename file or folder.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": rename {{FILE ; FOLDER} KEYWORD}");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       new file or folder name can't be over 260 characters");
                            break;
                        case ("reset"):
                            print_n_space(Console.WindowWidth / 2 - 7);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN RESET <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": reset data from cmd Linux. [MASTER]");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": reset [RESET_PARAMETER [COMMAND*]]");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       without argument remove genius propositions and usage statistics.\n>       /genius: remove genius propositions of COMMAND argument.\n>          used without argument will reset all genius data.\n>       /stats: remove usage statistics.");
                            break;
                        case ("resize"):
                            print_n_space(Console.WindowWidth / 2 - 8);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN RESIZE <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": resize cmd Linux's window.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": resize {NUMBER NUMBER ; RESIZE_PARAMETER}");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       first NUMBER argument is WIDTH, second is HEIGHT.\n>       /default reset size to default size.\n>       /full set size to its maximum size.");
                            break;
                        case ("rm"):
                            print_n_space(Console.WindowWidth / 2 - 6);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN RM <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": remove a file.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": rm {FILE*}");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("rmdir"):
                            print_n_space(Console.WindowWidth / 2 - 7);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN RMDIR <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": remove a folder.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": rmdir {FOLDER*}");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("script"):
                            print_n_space(Console.WindowWidth / 2 - 8);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN SCRIPT <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": launch script, which is a file containing commands.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": script {SCRIPT ; SCRIPT_PARAMETER [SCRIPT* ; KEYWORD* ; FOLDER*]}");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       if SCRIPT argument, command will execute it.\n>       /display:\n>          without SCRIPT argument, list all scripts.\n>          with SCRIPT argument, display content of SCRIPT.\n>       /export: create new folder in current directory with all scripts.\n>       /import: add all scripts from a folder or script file.\n>       /mk: create a new script in editor.\n>       /rename: change the name of a script.\n>       /rm: remove script.");
                            break;
                        case ("seed"):
                            print_n_space(Console.WindowWidth / 2 - 7);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN SEED <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": display all devices and information about them.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": seed");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("shutdown"):
                            print_n_space(Console.WindowWidth / 2 - 9);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN SHUTDOWN <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": shutdown computer.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": shutdown [SHUTDOWN_PARAMETER [TIME_IN_SECOND]]");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       shutdown without argument will shutdown it in 5 seconds.\n>       /a: cancel shutdown.\n>       /g: restart computer with app launched.\n>       /h: hibernate computer.\n>       /l: leave current session.\n>       /p: shutdown immediatly without notify user.\n>       /r: restart computer.\n>       /t: shutdown computer in TIME_IN_SECOND seconds.\n>       now: shutdown computer immeditly.");
                            break;
                        case ("skype"):
                            print_n_space(Console.WindowWidth / 2 - 7);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN SKYPE <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": launch Skype.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": skype");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       if Skype is not found, it will ask for its location.");
                            break;
                        case ("sl"):
                            print_n_space(Console.WindowWidth / 2 - 6);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN SL <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": don't fail writing ls.\n>              something will go through the screen!");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": sl [SL_PARAMETER]");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       sl without argument will randomly choose what's coming.\n>       /drake: a dragon will go through the screen.\n>       /plane: a plane wil go through the screen.\n>       /tank: a tank will go through the screen.\n>       /train: a train will go through the screen.");
                            break;
                        case ("taskkill"):
                            print_n_space(Console.WindowWidth / 2 - 9);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN TASKKILL <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": stop running process.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": taskkill {TASK_NAME*}");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("tasklist"):
                            print_n_space(Console.WindowWidth / 2 - 9);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN TASKLIST <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": display all running process.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": tasklist");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("textedit"):
                            print_n_space(Console.WindowWidth / 2 - 9);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN TEXTEDIT <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": create, edit text files.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": textedit [FILE]");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       create FILE argument if it doesn't exist.");
                            break;
                        case ("time"):
                            print_n_space(Console.WindowWidth / 2 - 7);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN TIME <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": display information about time.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": time [TIME_PARAMETER {TIME_IN_SECOND ; [NUMBER]}]");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       time with no argument display current time\n>       /calendar: display calendar to NUMBER of next week(s).\n>          default value is 5 weeks.\n>          will stop on 31/12/9999.\n>       /chrono: create a chrono of TIME_IN_SECOND seconds.\n>       /delay: create a delay of TIME_IN_SECOND seconds.");
                            break;
                        case ("touch"):
                            print_n_space(Console.WindowWidth / 2 - 7);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN TOUCH <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": create new file. if argument is a folder or a already created\n>              file, touch will update its last access time.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": touch {FILE* ; FOLDER*}");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       new file name can't be over 260 characters");
                            break;
                        case ("translate"):
                            print_n_space(Console.WindowWidth / 2 - 9);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN TRANSLATE <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": launch default brower and go to translate.google.com\n>              and will translate arguments.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": translate [LANGUAGE LANGUAGE [KEYWORD*]]");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       default language is the previous one used on google traduction.\n>       first language argument is SOURCE, second is DESTINATION.");
                            break;
                        case ("ts"):
                            print_n_space(Console.WindowWidth / 2 - 6);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN TS <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": launch Team Speak.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": ts");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       if Team Speak is not found, it will ask for its location.");
                            break;
                        case ("tv"):
                            print_n_space(Console.WindowWidth / 2 - 6);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN TV <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": launch default browser and go to programme-tv.net.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": tv");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("twitter"):
                            print_n_space(Console.WindowWidth / 2 - 8);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN TWITTER <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": launch default browser and go to twitter.com.\n>              You can also search on twitter by adding arguments.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": twitter [KEYWORD*]");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("uninstall"):
                            print_n_space(Console.WindowWidth / 2 - 9);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN UNINSTALL <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": prepare cmd Linux to be uninstalled then run Control Panel.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": uninstall");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       before running Control Panel, all scripts will be removed!");
                            break;
                        case ("url"):
                            print_n_space(Console.WindowWidth / 2 - 6);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN URL <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": launch default browser and go to the url(s) argument(s).");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": url {URL*}");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       url argument begins by:\n>          http://\n>          https://\n>          www.");
                            break;
                        case ("wait"):
                            print_n_space(Console.WindowWidth / 2 - 7);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN WAIT <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": set into pause mode cmd Linux.\n>              The process is completly asleep during this time.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": wait {TIME_IN_SECOND}");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("weather"):
                            print_n_space(Console.WindowWidth / 2 - 8);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN WEATHER <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": launch default browser and go to weather.com.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": weather");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("whoami"):
                            print_n_space(Console.WindowWidth / 2 - 8);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN WHOAMI <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": display user name of the current session.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": whoami");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("wikipedia"):
                            print_n_space(Console.WindowWidth / 2 - 9);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN WIKIPEDIA <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": launch default browser and go to wikipedia.com.\n>              You can also add arguments to search directly on Wikipedia.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": wikipedia [{LANGUAGE} {KEYWORD*} ; KEYWORD*]");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            Console.WriteLine(">       default language is english (/english)");
                            break;
                        case ("yahoo"):
                            print_n_space(Console.WindowWidth / 2 - 7);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN YAHOO <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": launch default browser and go to yahoo.com.\n>              You can also search on yahoo by adding arguments.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": yahoo [KEYWORDS*]");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        case ("youtube"):
                            print_n_space(Console.WindowWidth / 2 - 8);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("> MAN YOUTUBE <");
                            Console.ResetColor();
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("DESCRIPTION");
                            Console.ResetColor();
                            Console.WriteLine(": launch default browser and go to youtube.com.\n>              You can also search on youtube by adding arguments.");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write("SYNTAX");
                            Console.ResetColor();
                            Console.WriteLine(": youtube [KEYWORD*]");
                            Console.Write(">\n> ");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("NOTES");
                            Console.ResetColor();
                            break;
                        default:
                            print_n_space(Console.WindowWidth / 2 - 8 - (cmd[i].Length / 2));
                            Console.WriteLine(cmd[i] + ": Unknown command");
                            break;

                    }
                    Console.Write(">\n>\n>");
                    print_n_space(Console.WindowWidth / 2 - 13);
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine("PRESS ANY KEY TO CONTINUE");
                    Console.ResetColor();
                    Console.ReadKey(true);
                }
            }
            else
            {
                Console.WriteLine("> man: Invalid number of arguments");
                return (1);
            }

            Console.Clear();
            Console.CursorVisible = true;
            return (0);
        } //OK NEEDS MAJ EVERYTIME

        //FONCTION TOOLS

        static public void ShowApp(string app_path)
        {
            IntPtr h = Process.GetCurrentProcess().MainWindowHandle;
            ShowWindow(h, 5);
            SetForegroundWindow(h);
            SetFocus(h);
            System.Diagnostics.Debug.WriteLine(h);
        }

        static public int f1_help_access(string appdata_dir, string argument = "")
        {
            Console.Clear();
            if(argument != "")
            {
                if(File.Exists(appdata_dir + "/script_files/" + argument))
                {
                    execute_help(new string[2] { "help", argument}, appdata_dir);
                    Console.WriteLine(">\n> press any key to continue...");
                    Console.ReadKey(true);
                }
                else
                {
                    execute_man(new string[2] { "man", argument });
                }
            }
            else
            {
                execute_help(new string[1] { "help", }, appdata_dir);
                Console.WriteLine(">\n> press any key to continue...");
                Console.ReadKey(true);
            }
            Console.Clear();
            return (0);
        }

        static public string extract_shorter_path(string full_path)
        {
            int i = full_path.Length - 1;

            while(i >= 0 && full_path[i] != '\\' && full_path[i] != '/')
            {
                i--;
            }

            if(i < 0)
            {
                
                return (full_path);
            }
            else
            {
                return (full_path.Substring(i + 1, full_path.Length - i - 1));
            }
        }

        static public string extract_longer_path(string full_path)
        {
            int i;

            if(full_path.Length == 0)
            {
                return ("");
            }

            if (full_path[full_path.Length - 1] == '/' || full_path[full_path.Length - 1] == '\\')
            {
                i = full_path.Length - 2;
            }
            else
            {
                i = full_path.Length - 1;
            }

            while (i >= 0 && full_path[i] != '/' && full_path[i] != '\\')
            {
                i--;
            }
            
            if (i < 0)
            {
                return (full_path);
            }
            else
            {
                return (full_path.Substring(0, i + 1));
            }
        }

        static public bool is_valid_name(string name)
        {
            int i = 0;
            bool is_not_only_spaces = false;

            while (i < name.Length && name[i] != '"' && name[i] != '\\' && name[i] != '/' && name[i] != '*' && name[i] != '?' && name[i] != '|' && name[i] != '<' && name[i] != '>' && name[i] != ':')
            {
                if(name[i] != ' ')
                {
                    is_not_only_spaces = true;
                }
                if(i < name.Length - 1)
                {
                    if(name[i] == '.' && name[i+1] == '.')
                    {
                        i = name.Length;
                    }
                }
                i++;
            }

            return (i == name.Length && ((name.Length == 1 && name[0] != '.') || name.Length != 1) && is_not_only_spaces);
        }

        static public bool is_valid_url(string url)
        {
            return ((url.Length > 7 && url.Substring(0, 7) == "http://") || (url.Length > 8 && url.Substring(0, 8) == "https://") || (url.Length > 4 && url.Substring(0, 4) == "www."));
        }

        static private void copy_all_folder_content(string parent_directory, string target_directory)
        {
            List<string> all_folders = Directory.EnumerateDirectories(parent_directory).ToList();
            List<string> all_files = Directory.EnumerateFiles(parent_directory).ToList();

            if(all_files.Count > 0 || all_folders.Count > 0)
            {
                while(all_files.Count > 0)
                {
                    execute_cp(new string[3] { "cp", all_files[all_files.Count - 1], target_directory });
                    all_files.Remove(all_files[all_files.Count - 1]);
                }

                while (all_folders.Count > 0)
                {
                    Directory.CreateDirectory(target_directory + "/" + extract_shorter_path(all_folders[all_folders.Count - 1]));
                    copy_all_folder_content(all_folders[all_folders.Count - 1], target_directory + "/" + extract_shorter_path(all_folders[all_folders.Count - 1]));
                    all_folders.Remove(all_folders[all_folders.Count - 1]);
                }
            }
        }

        static private bool target_is_copy_folder(string folder_directory, string target_directory)
        {
            bool is_same = true;
            int i = 0;

            while(folder_directory.Length - 1 >= i && target_directory.Length - 1 >= i && is_same)
            {
                is_same = folder_directory[i] == target_directory[i];
                if(is_same)
                {
                    i++;
                }
            }

            return (is_same && folder_directory.Length - 1 < i);
        }

        static private string get_format_file(string file)
        {
            int i = file.Length - 1;

            while(i >= 0 && file[i] != '.')
            {
                i--;
            }

            if(i < 0)
            {
                return ("");
            }
            else
            {
                return (file.Substring(i, file.Length - i));
            }
        }

        static private string get_name_file(string file)
        {
            int i = file.Length - 1;
            string name = "";

            while (i >= 0 && file[i] != '/' && file[i] != '\\')
            {
                name = file[i] + name;
                if(file[i] == '.')
                {
                    name = "";
                }
                i--;
            }

            return (name);
        }

        static private void rm_full_directory(string parent_directory)
        {
            List<string> all_folders = Directory.EnumerateDirectories(parent_directory).ToList();
            List<string> all_files = Directory.EnumerateFiles(parent_directory).ToList();

            while(all_files.Count > 0)
            {
                File.Delete(all_files[all_files.Count - 1]);
                all_files.Remove(all_files[all_files.Count - 1]);
            }

            while (all_folders.Count > 0)
            {
                rm_full_directory(all_folders[all_folders.Count - 1]);
                Directory.Delete(all_folders[all_folders.Count - 1]);
                all_folders.Remove(all_folders[all_folders.Count - 1]);
            }
        }

        static private List<string> extract_file_content(string file_name)
        {
            if(File.Exists(file_name))
            {
                StreamReader reader = new StreamReader(file_name);
                List<string> file_content = new List<string>();
                string line = reader.ReadLine();
               
                while(line != null)
                {
                    file_content.Add(line);
                    line = reader.ReadLine();
                }

                reader.Close();
                return (file_content);
            }

            return (null);
        }

        static private void save_file_content(string file_name, List<string> content)
        {
            StreamWriter writer = new StreamWriter(file_name);

            for (int i = 0; i < content.Count; i++)
            {
                writer.WriteLine(content[i]);
            }

            writer.Close();
        }

        static private bool create_lolapp_script(string[] cmd, string appdata_dir)
        {
            StreamWriter writer = new StreamWriter(appdata_dir + "/script");

            switch(cmd[1])
            {
                case ("/profile"):
                    writer.WriteLine(2);
                    break;
                case ("/champion"):
                    writer.WriteLine(3);
                    break;
                case ("/game"):
                    writer.WriteLine(4);
                    break;
                case ("/history"):
                    writer.WriteLine(5);
                    break;
                case ("/team"):
                    writer.WriteLine(6);
                    break;
                default:
                    return (false);
            }

            writer.WriteLine(cmd[2]);
            if(cmd.Length == 4)
            {
                writer.WriteLine(cmd[3]);
            }
            
            writer.Close();
            return (true);
        }

        static public char find_main_seed()
        {
            char racine = 'A';
            int i = 0;

            while (i < 26 && !Directory.Exists(racine + ":/Program Files (x86)/"))
            {
                i++;
                racine = (char)(racine + 1);
            }

            if (i < 26)
            {
                return (racine);
            }
            else
            {
                return (' ');
            }
        }

        static public void update_genius_data(string appdata_dir, string[] cmd, ref List<Genius_data> all_genius_data, int max_genius_data)
        {
            int i;
            List<string> file_content = new List<string>();
            string file_line;

            if (cmd.Length > 0 && File.Exists(appdata_dir + "/genius_data/genius_data_" + cmd[0]))
            {
                StreamReader get_content = new StreamReader(appdata_dir + "/genius_data/genius_data_" + cmd[0]);
                file_line = get_content.ReadLine();

                while (file_line != null)
                {
                    file_content.Add(file_line);
                    file_line = get_content.ReadLine();
                }

                get_content.Close();

                for (int j = 1; j < cmd.Length; j++)
                {
                    if(cmd[j].Length > 1 && cmd[j][0] != '/')
                    {
                        i = 0;
                        while (i < file_content.Count && file_content[i] != cmd[j])
                        {
                            i++;
                        }
                        all_genius_data.Insert(0, new Genius_data(cmd[0], cmd[j]));
                        if (i < file_content.Count)
                        {
                            file_content.Remove(cmd[j]);
                        }
                        file_content.Insert(0, cmd[j]); //on rajoute la data au début (plus rapidement suggérée)
                    }
                }

                StreamWriter updater = new StreamWriter(appdata_dir + "/genius_data/genius_data_" + cmd[0]);

                for (i = 0; i < file_content.Count && i < max_genius_data; i++)
                {
                    updater.WriteLine(file_content[i]);
                }

                updater.Close();
            }
        }

        static public void print_n_space(int n)
        {
            for(int i = 0; i < n; i++)
            {
                Console.Write(" ");
            }
        }

        static public int max_length_list(List<string> list)
        {
            int max_length = 0;

            for(int i = 0; i < list.Count; i++)
            {
                if(list[i].Length > max_length)
                {
                    max_length = list[i].Length;
                }
            }

            return (max_length);
        }

        static public Process[] sort_alpha(Process[] list)
        {
            bool modification = true;
            Process swap;

            while(modification)
            {
                modification = false;
                for(int i = 0; i < list.Length - 1; i++)
                {
                    if(!string1_is_before_string2(list[i].ProcessName, list[i + 1].ProcessName))
                    {
                        swap = list[i + 1];
                        list[i + 1] = list[i];
                        list[i] = swap;
                        modification = true;
                    }
                }
            }

            return (list);
        }

        static private bool string1_is_before_string2(string string1, string string2)
        {
            int i = 0;

            while(i < string1.Length && i < string2.Length && string1[i] == string2[i])
            {
                i++;
            }

            if(i == string1.Length && i < string2.Length)
            {
                return (true);
            }
            if(i == string2.Length && i < string1.Length)
            {
                return (false);
            }
            if(i == string1.Length && i == string2.Length)
            {
                return (true);
            }
            return (string1[i] < string2[i]);
        }

        static private void train_animation()
        {
            Console.Clear();
            string line1 = "                      ====        ________                ___________             ";
            string line2 = "  _D _|  |_______/        \\__I_I_____===__|_________|                             ";
            string line3 = "   |(_)---  |   H\\________/ |   |        =|___ ___|      _________________        ";
            string line4 = "   /     |  |   H  |  |     |   |         ||_| |_||     _|                \\_____A ";
            string line5 = "  |      |  |   H  |__--------------------| [___] |   =|                        | ";
            string line6 = "  | ________|___H__/__|_____/[][]~\\_______|       |   -|                        | ";
            string line7 = "  |/ |   |-----------I_____I [][] []  D   |=======|____|________________________|_";
            string line8 = "__/ =| o |=-~~\\  /~~\\  /~~\\  /~~\\ ____Y___________|__|__________________________|_";
            string line9 = " |/-=|___||    ||    ||    ||    |_____/~\\___/          |_D__D__D_|  |_D__D__D_|  ";
            string line10 = "  \\_/      \\__/  \\__/  \\__/  \\__/      \\_/               \\_/   \\_/    \\_/   \\_/   ";
            int duration = line1.Length + Console.WindowWidth;

            for (int i = 0; i < Console.WindowWidth; i++)
            {
                line1 = " " + line1;
                line2 = " " + line2;
                line3 = " " + line3;
                line4 = " " + line4;
                line5 = " " + line5;
                line6 = " " + line6;
                line7 = " " + line7;
                line8 = " " + line8;
                line9 = " " + line9;
                line10 = " " + line10;
            }

            for (int i = 0; i < duration && !Console.KeyAvailable; i++)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                if (line1.Length + 1 > Console.WindowWidth)
                {
                    Console.WriteLine(line1.Substring(0, Console.WindowWidth - 1));
                    Console.WriteLine(line2.Substring(0, Console.WindowWidth - 1));
                    Console.WriteLine(line3.Substring(0, Console.WindowWidth - 1));
                    Console.WriteLine(line4.Substring(0, Console.WindowWidth - 1));
                    Console.WriteLine(line5.Substring(0, Console.WindowWidth - 1));
                    Console.WriteLine(line6.Substring(0, Console.WindowWidth - 1));
                    Console.WriteLine(line7.Substring(0, Console.WindowWidth - 1));
                    Console.WriteLine(line8.Substring(0, Console.WindowWidth - 1));
                    Console.WriteLine(line9.Substring(0, Console.WindowWidth - 1));
                    Console.WriteLine(line10.Substring(0, Console.WindowWidth - 1));
                }
                else
                {
                    Console.WriteLine(line1);
                    Console.WriteLine(line2);
                    Console.WriteLine(line3);
                    Console.WriteLine(line4);
                    Console.WriteLine(line5);
                    Console.WriteLine(line6);
                    Console.WriteLine(line7);
                    Console.WriteLine(line8);
                    Console.WriteLine(line9);
                    Console.WriteLine(line10);
                }
                if (line1.Length > 1)
                {
                    line1 = line1.Substring(1, line1.Length - 1);
                    line2 = line2.Substring(1, line2.Length - 1);
                    line3 = line3.Substring(1, line3.Length - 1);
                    line4 = line4.Substring(1, line4.Length - 1);
                    line5 = line5.Substring(1, line5.Length - 1);
                    line6 = line6.Substring(1, line6.Length - 1);
                    line7 = line7.Substring(1, line7.Length - 1);
                    line8 = line8.Substring(1, line8.Length - 1);
                    line9 = line9.Substring(1, line9.Length - 1);
                    line10 = line10.Substring(1, line10.Length - 1);
                }
                Thread.Sleep(50);
                Console.Clear();
            }
        }

        static private void plane_animation()
        {
            Console.Clear();
            string line1 = "                      \\ \\                                ";
            string line2 = "                       \\ `\\                              ";
            string line3 = "    ___                 \\  \\                             ";
            string line4 = "   |    \\                \\  `\\                           ";
            string line5 = "   |_____\\                \\    \\                         ";
            string line6 = "   |______\\                \\    `\\                       ";
            string line7 = "   |       \\                \\     \\                      ";
            string line8 = "   |      __\\__---------------------------------._.      ";
            string line9 = " __|---~~~__o_o_o_o_o_o_o_o_o_o_o_o_o_o_o_o_o_o_[][\\__   ";
            string line10 = "|___                         /~      )                \\__";
            string line11 = "    ~~~---..._______________/      ,/_________________/  ";
            string line12 = "                           /      /                      ";
            string line13 = "                          /     ,/                       ";
            string line14 = "                         /     /                         ";
            string line15 = "                        /    ,/                          ";
            string line16 = "                       /    /                            ";
            string line17 = "                      //  ,/                             ";
            string line18 = "                     //  /                               ";
            string line19 = "                    // ,/                                ";
            string line20 = "                   //_/                                  ";
            int duration = line1.Length + Console.WindowWidth - 1;

            for (int i = 1; i < duration && !Console.KeyAvailable; i++)
            {
                Console.WriteLine();
                Console.WriteLine();
                if (i < 57)
                {
                    Console.WriteLine(line1.Substring(line1.Length - i, i));
                    Console.WriteLine(line2.Substring(line2.Length - i, i));
                    Console.WriteLine(line3.Substring(line3.Length - i, i));
                    Console.WriteLine(line4.Substring(line4.Length - i, i));
                    Console.WriteLine(line5.Substring(line5.Length - i, i));
                    Console.WriteLine(line6.Substring(line6.Length - i, i));
                    Console.WriteLine(line7.Substring(line7.Length - i, i));
                    Console.WriteLine(line8.Substring(line8.Length - i, i));
                    Console.WriteLine(line9.Substring(line9.Length - i, i));
                    Console.WriteLine(line10.Substring(line10.Length - i, i));
                    Console.WriteLine(line11.Substring(line11.Length - i, i));
                    Console.WriteLine(line12.Substring(line12.Length - i, i));
                    Console.WriteLine(line13.Substring(line13.Length - i, i));
                    Console.WriteLine(line14.Substring(line14.Length - i, i));
                    Console.WriteLine(line15.Substring(line15.Length - i, i));
                    Console.WriteLine(line16.Substring(line16.Length - i, i));
                    Console.WriteLine(line17.Substring(line17.Length - i, i));
                    Console.WriteLine(line18.Substring(line18.Length - i, i));
                    Console.WriteLine(line19.Substring(line19.Length - i, i));
                    Console.WriteLine(line20.Substring(line20.Length - i, i));
                }
                else
                {
                    Console.WriteLine(line1);
                    Console.WriteLine(line2);
                    Console.WriteLine(line3);
                    Console.WriteLine(line4);
                    Console.WriteLine(line5);
                    Console.WriteLine(line6);
                    Console.WriteLine(line7);
                    Console.WriteLine(line8);
                    Console.WriteLine(line9);
                    Console.WriteLine(line10);
                    Console.WriteLine(line11);
                    Console.WriteLine(line12);
                    Console.WriteLine(line13);
                    Console.WriteLine(line14);
                    Console.WriteLine(line15);
                    Console.WriteLine(line16);
                    Console.WriteLine(line17);
                    Console.WriteLine(line18);
                    Console.WriteLine(line19);
                    Console.WriteLine(line20);
                }
                if (i > 57)
                {
                    if(line1.Length < Console.WindowWidth - 1)
                    {
                        line1 = " " + line1;
                        line2 = " " + line2;
                        line3 = " " + line3;
                        line4 = " " + line4;
                        line5 = " " + line5;
                        line6 = " " + line6;
                        line7 = " " + line7;
                        line8 = " " + line8;
                        line9 = " " + line9;
                        line10 = " " + line10;
                        line11 = " " + line11;
                        line12 = " " + line12;
                        line13 = " " + line13;
                        line14 = " " + line14;
                        line15 = " " + line15;
                        line16 = " " + line16;
                        line17 = " " + line17;
                        line18 = " " + line18;
                        line19 = " " + line19;
                        line20 = " " + line20;
                    }
                    else
                    {
                        line1 = " " + line1.Substring(0,line1.Length - 1);
                        line2 = " " + line2.Substring(0, line2.Length - 1);
                        line3 = " " + line3.Substring(0, line3.Length - 1);
                        line4 = " " + line4.Substring(0, line4.Length - 1);
                        line5 = " " + line5.Substring(0, line5.Length - 1);
                        line6 = " " + line6.Substring(0, line6.Length - 1);
                        line7 = " " + line7.Substring(0, line7.Length - 1);
                        line8 = " " + line8.Substring(0, line8.Length - 1);
                        line9 = " " + line9.Substring(0, line9.Length - 1);
                        line10 = " " + line10.Substring(0, line10.Length - 1);
                        line11 = " " + line11.Substring(0, line11.Length - 1);
                        line12 = " " + line12.Substring(0, line12.Length - 1);
                        line13 = " " + line13.Substring(0, line13.Length - 1);
                        line14 = " " + line14.Substring(0, line14.Length - 1);
                        line15 = " " + line15.Substring(0, line15.Length - 1);
                        line16 = " " + line16.Substring(0, line16.Length - 1);
                        line17 = " " + line17.Substring(0, line17.Length - 1);
                        line18 = " " + line18.Substring(0, line18.Length - 1);
                        line19 = " " + line19.Substring(0, line19.Length - 1);
                        line20 = " " + line20.Substring(0, line20.Length - 1);
                    }
                }
                Thread.Sleep(30);
                Console.Clear();
            }
        }

        static private void tank_animation()
        {
            Console.Clear();
            string line1 = "         \\                         ";
            string line2 = "          \\                        ";
            string line3 = "         __\\________               ";
            string line4 = "        /            \\             ";
            string line5 = "       /              \\========    ";
            string line6 = "   ___|________________\\______     ";
            string line7 = " /                             \\   ";
            string line8 = "/ _____________________________ \\  ";
            string line9 = "\\/ _=========================_ \\/  ";
            string line10 = " \\                              \\  ";
            string line11 = "  \"-==========================-\"   ";
            int duration = line1.Length + Console.WindowWidth - 1;

            for (int i = 1; i < duration && !Console.KeyAvailable; i++)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                if (i < 35)
                {
                    Console.WriteLine(line1.Substring(line1.Length - i, i));
                    Console.WriteLine(line2.Substring(line2.Length - i, i));
                    Console.WriteLine(line3.Substring(line3.Length - i, i));
                    Console.WriteLine(line4.Substring(line4.Length - i, i));
                    Console.WriteLine(line5.Substring(line5.Length - i, i));
                    Console.WriteLine(line6.Substring(line6.Length - i, i));
                    Console.WriteLine(line7.Substring(line7.Length - i, i));
                    Console.WriteLine(line8.Substring(line8.Length - i, i));
                    Console.WriteLine(line9.Substring(line9.Length - i, i));
                    Console.WriteLine(line10.Substring(line10.Length - i, i));
                    Console.WriteLine(line11.Substring(line11.Length - i, i));
                }
                else
                {
                    Console.WriteLine(line1);
                    Console.WriteLine(line2);
                    Console.WriteLine(line3);
                    Console.WriteLine(line4);
                    Console.WriteLine(line5);
                    Console.WriteLine(line6);
                    Console.WriteLine(line7);
                    Console.WriteLine(line8);
                    Console.WriteLine(line9);
                    Console.WriteLine(line10);
                    Console.WriteLine(line11);
                }
                if (i > 35)
                {
                    if (line1.Length < Console.WindowWidth - 1)
                    {
                        line1 = " " + line1;
                        line2 = " " + line2;
                        line3 = " " + line3;
                        line4 = " " + line4;
                        line5 = " " + line5;
                        line6 = " " + line6;
                        line7 = " " + line7;
                        line8 = " " + line8;
                        line9 = " " + line9;
                        line10 = " " + line10;
                        line11 = " " + line11;
                    }
                    else
                    {
                        line1 = " " + line1.Substring(0, line1.Length - 1);
                        line2 = " " + line2.Substring(0, line2.Length - 1);
                        line3 = " " + line3.Substring(0, line3.Length - 1);
                        line4 = " " + line4.Substring(0, line4.Length - 1);
                        line5 = " " + line5.Substring(0, line5.Length - 1);
                        line6 = " " + line6.Substring(0, line6.Length - 1);
                        line7 = " " + line7.Substring(0, line7.Length - 1);
                        line8 = " " + line8.Substring(0, line8.Length - 1);
                        line9 = " " + line9.Substring(0, line9.Length - 1);
                        line10 = " " + line10.Substring(0, line10.Length - 1);
                        line11 = " " + line11.Substring(0, line11.Length - 1);
                    }
                }
                Thread.Sleep(60);
                Console.Clear();
            }
        }

        static private void drake_animation()
        {
            Console.Clear();

            string line1 = "                                                             /===-_---~~~~~~~~~------____";
            string line2 = "                                                |===-~___                _,-'            ";
            string line3 = "                 -==\\\\                         `//~\\\\   ~~~~`---.___.-~~                 ";
            string line4 = "             ______-==|                         | |  \\\\           _-~`                   ";
            string line5 = "       __--~~~  ,-/-==\\\\                        | |   `\\        ,'                       ";
            string line6 = "    _-~       /'    |  \\\\                      / /      \\      /                         ";
            string line7 = "  .'        /       |   \\\\                   /' /        \\   /'                          ";
            string line8 = " /  ____  /         |    \\`\\.__/-~~ ~ \\ _ _/'  /          \\/'                            ";
            string line9 = "/-'~    ~~~~~---__  |     ~-/~         ( )   /'        _--~`                             ";
            string line10 = "                  \\_|      /        _)   ;  ),   __--~~                                  ";
            string line11 = "                    '~~--_/      _-~/-  / \\   '-~ \\                                      ";
            string line12 = "                   {\\__--_/}    / \\\\_>- )<__\\      \\                                     ";
            string line13 = "                   /'   (_/  _-~  | |__>--<__|      |                                    ";
            string line14 = "                  |0  0 _/) )-~     | |__>--<__|      |                                  ";
            string line15 = "                  / /~ ,_/       / /__>---<__/      |                                    ";
            string line16 = "                 o o _//        /-~_>---<__-~      /                                     ";
            string line17 = "                 (^(~          /~_>---<__-      _-~                                      ";
            string line18 = "                ,/|           /__>--<__/     _-~                                         ";
            string line19 = "             ,//('(          |__>--<__|     /                  .----_                    ";
            string line20 = "            ( ( '))          |__>--<__|    |                 /' _---_~\\                  ";
            string line21 = "         `-)) )) (           |__>--<__|    |               /'  /     ~\\`\\                ";
            string line22 = "        ,/,'//( (             \\__>--<__\\    \\            /'  //        ||                ";
            string line23 = "      ,( ( ((, ))              ~-__>--<_~-_  ~--____---~' _/'/        /'                 ";
            string line24 = "    `~/  )` ) ,/|                 ~-_~>--<_/-__       __-~ _/                            ";
            string line25 = "  ._-~//( )/ )) `                    ~~-'_/_/ /~~~~~~~__--~                              ";
            string line26 = "   ;'( ')/ ,)(                              ~~~~~~~~~~                                   ";
            string line27 = "  ' ') '( (/                                                                             ";
            string line28 = "    '   '  `                                                                             ";

            string line29 = "                              /__>--<__/     _-~                                         ";
            string line30 = "                             |__>--<__|     /                  .----_                    ";
            string line31 = "                             |__>--<__|    |                 /' _---_~\\                  ";
            string line32 = "                             |__>--<__|    |               /'  /     ~\\`\\                ";
            string line33 = "                              \\__>--<__\\    \\            /'  //        ||                ";
            string line34 = "                               ~-__>--<_~-_  ~--____---~' _/'/        /'                 ";
            string line35 = "                                  ~-_~>--<_/-__       __-~ _/                            ";
            string line36 = "                                     ~~-'_/_/ /~~~~~~~__--~                              ";
            string line37 = "                                            ~~~~~~~~~~                                   ";
            string line38 = "                                                                                         ";
            string line39 = "                                                                                         ";
            int duration = line1.Length + Console.WindowWidth;

            for (int i = 0; i < Console.WindowWidth; i++)
            {
                line1 = " " + line1;
                line2 = " " + line2;
                line3 = " " + line3;
                line4 = " " + line4;
                line5 = " " + line5;
                line6 = " " + line6;
                line7 = " " + line7;
                line8 = " " + line8;
                line9 = " " + line9;
                line10 = " " + line10;
                line11 = " " + line11;
                line12 = " " + line12;
                line13 = " " + line13;
                line14 = " " + line14;
                line15 = " " + line15;
                line16 = " " + line16;
                line17 = " " + line17;
                line18 = " " + line18;
                line19 = " " + line19;
                line20 = " " + line20;
                line21 = " " + line21;
                line22 = " " + line22;
                line23 = " " + line23;
                line24 = " " + line24;
                line25 = " " + line25;
                line26 = " " + line26;
                line27 = " " + line27;
                line28 = " " + line28;
                line29 = " " + line29;
                line30 = " " + line30;
                line31 = " " + line31;
                line32 = " " + line32;
                line33 = " " + line33;
                line34 = " " + line34;
                line35 = " " + line35;
                line36 = " " + line36;
                line37 = " " + line37;
                line38 = " " + line38;
                line39 = " " + line39;
            }

            for (int i = 0; i < duration && !Console.KeyAvailable; i++)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                if (line1.Length + 1 > Console.WindowWidth)
                {
                    Console.WriteLine(line1.Substring(0, Console.WindowWidth - 1));
                    Console.WriteLine(line2.Substring(0, Console.WindowWidth - 1));
                    Console.WriteLine(line3.Substring(0, Console.WindowWidth - 1));
                    Console.WriteLine(line4.Substring(0, Console.WindowWidth - 1));
                    Console.WriteLine(line5.Substring(0, Console.WindowWidth - 1));
                    Console.WriteLine(line6.Substring(0, Console.WindowWidth - 1));
                    Console.WriteLine(line7.Substring(0, Console.WindowWidth - 1));
                    Console.WriteLine(line8.Substring(0, Console.WindowWidth - 1));
                    Console.WriteLine(line9.Substring(0, Console.WindowWidth - 1));
                    Console.WriteLine(line10.Substring(0, Console.WindowWidth - 1));
                    Console.WriteLine(line11.Substring(0, Console.WindowWidth - 1));
                    Console.WriteLine(line12.Substring(0, Console.WindowWidth - 1));
                    Console.WriteLine(line13.Substring(0, Console.WindowWidth - 1));
                    Console.WriteLine(line14.Substring(0, Console.WindowWidth - 1));
                    Console.WriteLine(line15.Substring(0, Console.WindowWidth - 1));
                    Console.WriteLine(line16.Substring(0, Console.WindowWidth - 1));
                    Console.WriteLine(line17.Substring(0, Console.WindowWidth - 1));

                    if(i < 35) //dragon sans flamme
                    {
                        Console.WriteLine(line29.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line30.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line31.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line32.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line33.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line34.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line35.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line36.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line37.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line38.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line39.Substring(0, Console.WindowWidth - 1));
                    }
                    else if (i >= 35 && i <= 40) //dragon debut petite flamme
                    {
                        Console.WriteLine(line18.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line19.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line20.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line32.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line33.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line34.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line35.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line36.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line37.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line38.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line39.Substring(0, Console.WindowWidth - 1));
                    }
                    else if (i >= 41 && i <= 46) //dragon moyenne flamme
                    {
                        Console.WriteLine(line18.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line19.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line20.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line21.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line22.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line23.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line24.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line36.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line37.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line38.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line39.Substring(0, Console.WindowWidth - 1));
                    }
                    else if (i >= 47 && i <= 89) //dragon grande flamme
                    {
                        Console.WriteLine(line18.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line19.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line20.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line21.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line22.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line23.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line24.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line25.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line26.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line27.Substring(0, Console.WindowWidth - 1));
                        Console.WriteLine(line28.Substring(0, Console.WindowWidth - 1));
                    }
                    
                }
                else
                {
                    Console.WriteLine(line1);
                    Console.WriteLine(line2);
                    Console.WriteLine(line3);
                    Console.WriteLine(line4);
                    Console.WriteLine(line5);
                    Console.WriteLine(line6);
                    Console.WriteLine(line7);
                    Console.WriteLine(line8);
                    Console.WriteLine(line9);
                    Console.WriteLine(line10);
                    Console.WriteLine(line11);
                    Console.WriteLine(line12);
                    Console.WriteLine(line13);
                    Console.WriteLine(line14);
                    Console.WriteLine(line15);
                    Console.WriteLine(line16);
                    Console.WriteLine(line17);

                    if (i >= 102 && i <= 114) //dragon sans flamme
                    {
                        Console.WriteLine(line29);
                        Console.WriteLine(line30);
                        Console.WriteLine(line31);
                        Console.WriteLine(line32);
                        Console.WriteLine(line33);
                        Console.WriteLine(line34);
                        Console.WriteLine(line35);
                        Console.WriteLine(line36);
                        Console.WriteLine(line37);
                        Console.WriteLine(line38);
                        Console.WriteLine(line39);
                    }
                    else if (i >= 115 && i <= 120) //dragon debut petite flamme
                    {
                        Console.WriteLine(line18);
                        Console.WriteLine(line19);
                        Console.WriteLine(line20);
                        Console.WriteLine(line32);
                        Console.WriteLine(line33);
                        Console.WriteLine(line34);
                        Console.WriteLine(line35);
                        Console.WriteLine(line36);
                        Console.WriteLine(line37);
                        Console.WriteLine(line38);
                        Console.WriteLine(line39);
                    }
                    else if (i >= 121 && i <= 126) //dragon moyenne flamme
                    {
                        Console.WriteLine(line18);
                        Console.WriteLine(line19);
                        Console.WriteLine(line20);
                        Console.WriteLine(line21);
                        Console.WriteLine(line22);
                        Console.WriteLine(line23);
                        Console.WriteLine(line24);
                        Console.WriteLine(line36);
                        Console.WriteLine(line37);
                        Console.WriteLine(line38);
                        Console.WriteLine(line39);
                    }
                    else if(i >= 90 && i <= 95) //dragon debut fin flamme
                    {
                        Console.WriteLine(line29);
                        Console.WriteLine(line30);
                        Console.WriteLine(line31);
                        Console.WriteLine(line21);
                        Console.WriteLine(line22);
                        Console.WriteLine(line23);
                        Console.WriteLine(line24);
                        Console.WriteLine(line25);
                        Console.WriteLine(line26);
                        Console.WriteLine(line27);
                        Console.WriteLine(line28);
                    }
                    else if(i >= 96 && i <= 101) //dragon milieu fin flamme
                    {
                        Console.WriteLine(line29);
                        Console.WriteLine(line30);
                        Console.WriteLine(line31);
                        Console.WriteLine(line32);
                        Console.WriteLine(line33);
                        Console.WriteLine(line34);
                        Console.WriteLine(line35);
                        Console.WriteLine(line25);
                        Console.WriteLine(line26);
                        Console.WriteLine(line27);
                        Console.WriteLine(line28);
                    }
                    else //dragon grande flamme
                    {
                        Console.WriteLine(line18);
                        Console.WriteLine(line19);
                        Console.WriteLine(line20);
                        Console.WriteLine(line21);
                        Console.WriteLine(line22);
                        Console.WriteLine(line23);
                        Console.WriteLine(line24);
                        Console.WriteLine(line25);
                        Console.WriteLine(line26);
                        Console.WriteLine(line27);
                        Console.WriteLine(line28);
                    }
                }
                if (line1.Length > 1)
                {
                    line1 = line1.Substring(1, line1.Length - 1);
                    line2 = line2.Substring(1, line2.Length - 1);
                    line3 = line3.Substring(1, line3.Length - 1);
                    line4 = line4.Substring(1, line4.Length - 1);
                    line5 = line5.Substring(1, line5.Length - 1);
                    line6 = line6.Substring(1, line6.Length - 1);
                    line7 = line7.Substring(1, line7.Length - 1);
                    line8 = line8.Substring(1, line8.Length - 1);
                    line9 = line9.Substring(1, line9.Length - 1);
                    line10 = line10.Substring(1, line10.Length - 1);
                    line11 = line11.Substring(1, line11.Length - 1);
                    line12 = line12.Substring(1, line12.Length - 1);
                    line13 = line13.Substring(1, line13.Length - 1);
                    line14 = line14.Substring(1, line14.Length - 1);
                    line15 = line15.Substring(1, line15.Length - 1);
                    line16 = line16.Substring(1, line16.Length - 1);
                    line17 = line17.Substring(1, line17.Length - 1);
                    line18 = line18.Substring(1, line18.Length - 1);
                    line19 = line19.Substring(1, line19.Length - 1);
                    line20 = line20.Substring(1, line20.Length - 1);
                    line21 = line21.Substring(1, line21.Length - 1);
                    line22 = line22.Substring(1, line22.Length - 1);
                    line23 = line23.Substring(1, line23.Length - 1);
                    line24 = line24.Substring(1, line24.Length - 1);
                    line25 = line25.Substring(1, line25.Length - 1);
                    line26 = line26.Substring(1, line26.Length - 1);
                    line27 = line27.Substring(1, line27.Length - 1);
                    line28 = line28.Substring(1, line28.Length - 1);

                    line29 = line29.Substring(1, line29.Length - 1);
                    line30 = line30.Substring(1, line30.Length - 1);
                    line31 = line31.Substring(1, line31.Length - 1);
                    line32 = line32.Substring(1, line32.Length - 1);
                    line33 = line33.Substring(1, line33.Length - 1);
                    line34 = line34.Substring(1, line34.Length - 1);
                    line35 = line35.Substring(1, line35.Length - 1);
                    line36 = line36.Substring(1, line36.Length - 1);
                    line37 = line37.Substring(1, line37.Length - 1);
                    line38 = line38.Substring(1, line38.Length - 1);
                    line39 = line39.Substring(1, line39.Length - 1);
                }
                Thread.Sleep(30);
                Console.Clear();
            }
        }

        static private void print_option_menu(int first_line_position, int current_line, bool script_enable, int refresh_timer, bool auto_lock, bool genius_enable, bool private_mode, bool auto_log, bool cmd_print_user, bool cmd_print_path, bool cmd_print_time, int max_genius_data)
        {
            Console.SetCursorPosition(0, first_line_position + current_line - 1);
            print_n_space(Console.WindowWidth - 1);
            Console.SetCursorPosition(0, first_line_position);
            if (current_line == 1)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("> UPDATE:            " + refresh_timer + "ms");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("> UPDATE:            " + refresh_timer + "ms");
            }
            if (current_line == 2)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("> PRINT USER:        " + cmd_print_user);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("> PRINT USER:        " + cmd_print_user);
            }
            if (current_line == 3)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("> PRINT DIRECTORY:   " + cmd_print_path);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("> PRINT DIRECTORY:   " + cmd_print_path);
            }
            if (current_line == 4)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("> PRINT TIME:        " + cmd_print_time);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("> PRINT TIME:        " + cmd_print_time);
            }
            if(current_line == 5)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("> SCRIPT ENABLE:     " + script_enable);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("> SCRIPT ENABLE:     " + script_enable);
            }
            if (current_line == 6)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("> AUTO LOCK:         " + auto_lock);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("> AUTO LOCK:         " + auto_lock);
            }
            if (current_line == 7)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("> AUTO LOG:          " + auto_log);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("> AUTO LOG:          " + auto_log);
            }
            if (current_line == 8)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("> GENIUS ENABLE:     " + genius_enable);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("> GENIUS ENABLE:     " + genius_enable);
            }
            if (current_line == 9)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("> MAX GENIUS DATA:   ");
                if(max_genius_data == 10000)
                {
                    Console.WriteLine("Unlimited");
                }
                else
                {
                    Console.WriteLine(max_genius_data);
                }
                Console.ResetColor();
            }
            else
            {
                Console.Write("> MAX GENIUS DATA:   ");
                if (max_genius_data == 10000)
                {
                    Console.WriteLine("Unlimited");
                }
                else
                {
                    Console.WriteLine(max_genius_data);
                }
            }
            if (current_line == 10)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("> PRIVATE MODE:      " + private_mode);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("> PRIVATE MODE:      " + private_mode);
            }
            if (current_line == 11)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("> EXIT");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("> EXIT");
            }
        }

        static private void erase_option_menu(int first_line_position)
        {
            Console.SetCursorPosition(0, first_line_position);
            for(int i = 0; i < 11; i++)
            {
                print_n_space(Console.WindowWidth - 1);
                Console.WriteLine();
            }
            Console.SetCursorPosition(0, first_line_position);
        }

        static private void apply_option_menu_action(ConsoleKeyInfo action, ref int current_line, ref bool script_enable, ref int refresh_timer, ref bool auto_lock, ref bool genius_enable, ref bool private_mode, ref bool running, ref bool auto_log, ref bool cmd_print_user, ref bool cmd_print_path, ref bool cmd_print_time, ref int max_genius_data)
        {
            switch(action.Key)
            {
                case (ConsoleKey.UpArrow):
                    if(current_line == 1)
                    {
                        current_line = 11;
                    }
                    else
                    {
                        current_line--;
                    }
                    break;
                case (ConsoleKey.DownArrow):
                    if (current_line == 11)
                    {
                        current_line = 1;
                    }
                    else
                    {
                        current_line++;
                    }
                    break;
                case (ConsoleKey.LeftArrow):
                    switch(current_line)
                    {
                        case (1):
                            if(refresh_timer >= 5)
                            {
                                refresh_timer -= 5;
                            }
                            break;
                        case (2):
                            cmd_print_user = !cmd_print_user;
                            break;
                        case (3):
                            cmd_print_path = !cmd_print_path;
                            break;
                        case (4):
                            cmd_print_time = !cmd_print_time;
                            break;
                        case (5):
                            script_enable = !script_enable;
                            break;
                        case (6):
                            auto_lock = !auto_lock;
                            break;
                        case (7):
                            auto_log = !auto_log;
                            break;
                        case (8):
                            genius_enable = !genius_enable;
                            break;
                        case (9):
                            if(max_genius_data == 10000)
                            {
                                max_genius_data = 100;
                            }
                            else
                            {
                                max_genius_data -= 5;
                                if(max_genius_data == 0)
                                {
                                    max_genius_data = 10000;
                                }
                            }
                            break;
                        case (10):
                            private_mode = !private_mode;
                            break;
                    }
                    break;
                case (ConsoleKey.RightArrow):
                    switch (current_line)
                    {
                        case (1):
                            if(refresh_timer <= 2147483642)
                            {
                                refresh_timer += 5;
                            }
                            break;
                        case (2):
                            cmd_print_user = !cmd_print_user;
                            break;
                        case (3):
                            cmd_print_path = !cmd_print_path;
                            break;
                        case (4):
                            cmd_print_time = !cmd_print_time;
                            break;
                        case (5):
                            script_enable = !script_enable;
                            break;
                        case (6):
                            auto_lock = !auto_lock;
                            break;
                        case (7):
                            auto_log = !auto_log;
                            break;
                        case (8):
                            genius_enable = !genius_enable;
                            break;
                        case (9):
                            if (max_genius_data == 100)
                            {
                                max_genius_data = 10000;
                            }
                            else
                            {
                                if(max_genius_data == 10000)
                                {
                                    max_genius_data = 0;
                                }
                                max_genius_data += 5;
                            }
                            break;
                        case (10):
                            private_mode = !private_mode;
                            break;
                    }
                    break;
                case (ConsoleKey.Escape):
                    if(current_line == 11)
                    {
                        running = false;
                    }
                    else
                    {
                        current_line = 11;
                    }
                    break;
                case (ConsoleKey.Enter):
                    if(current_line == 11)
                    {
                        running = false;
                    }
                    break;
            }
        }

        static private void save_option(bool script_enable, int refresh_timer, bool auto_lock, bool genius_enable, bool private_mode, bool auto_log, bool cmd_print_user, bool cmd_print_path, bool cmd_print_time, int max_genius_data, string appdata_dir)
        {
            try
            {
                StreamWriter saver = new StreamWriter(appdata_dir + "/option");
                saver.WriteLine("UPDATE: " + refresh_timer + "ms");
                saver.WriteLine("PRINT USER: " + cmd_print_user);
                saver.WriteLine("PRINT DIRECTORY: " + cmd_print_path);
                saver.WriteLine("PRINT TIME: " + cmd_print_time);
                saver.WriteLine("SCRIPT ENABLE: " + script_enable);
                saver.WriteLine("AUTO LOCK: " + auto_lock);
                saver.WriteLine("AUTO LOG: " + auto_log);
                saver.WriteLine("GENIUS ENABLE: " + genius_enable);
                saver.WriteLine("MAX GENIUS DATA: " + max_genius_data);
                saver.WriteLine("PRIVATE MODE: " + private_mode);
                saver.Close();
                Console.WriteLine("> option: options saved!");
            }
            catch(Exception)
            {
                Console.WriteLine("> option: can't save options, access denied! (execute it with admin rights)");
            }
        }

        static private bool search_file_or_folder(string[] cmd, ref List<List<string>> results, string searching_directory, ref bool cancel, ref bool first_found)
        {
            List<string> all_folders = new List<string>();

            if(Console.KeyAvailable)
            {
                ConsoleKeyInfo key_pressed = Console.ReadKey(true);
                if(key_pressed.Key == ConsoleKey.Escape)
                {
                    print_n_space(26 + cmd[0].Length);
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write("> ");
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("search: process canceled");
                    Console.ResetColor();
                    cancel = true;
                    first_found = true;
                }
            }

            if(!cancel)
            {
                try
                {
                    for (int i = 1; i < cmd.Length; i++)
                    {
                        if (File.Exists(searching_directory + "/" + cmd[i]))
                        {
                            if(i == 1)
                            {
                                if(!first_found)
                                {
                                    print_n_space(cmd[0].Length + 26);
                                    Console.SetCursorPosition(0, Console.CursorTop);
                                    Console.Write("> ");
                                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                                    Console.WriteLine(cmd[0] + ": " + cmd[1] + ": ");
                                    Console.ResetColor();
                                }
                                Console.WriteLine("> " + searching_directory + "\\" + cmd[i]);
                                first_found = true;
                            }
                            else
                            {
                                results[i - 2].Add(searching_directory + "\\" + cmd[i]);
                            }
                        }
                        if (Directory.Exists(searching_directory + "/" + cmd[i]))
                        {
                            if (cmd[i][cmd[i].Length - 1] == '/')
                            {
                                if (i == 1)
                                {
                                    if (!first_found)
                                    {
                                        print_n_space(cmd[0].Length + 26);
                                        Console.SetCursorPosition(0, Console.CursorTop);
                                        Console.Write("> ");
                                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                                        Console.WriteLine(cmd[0] + ": " + cmd[1] + ": ");
                                        Console.ResetColor();
                                    }
                                    Console.WriteLine("> " + searching_directory + "\\" + cmd[i]);
                                    first_found = true;
                                }
                                else
                                {
                                    results[i - 2].Add(searching_directory + "\\" + cmd[i]);
                                }
                            }
                            else
                            {
                                if (i == 1)
                                {
                                    if (!first_found)
                                    {
                                        print_n_space(cmd[0].Length + 26);
                                        Console.SetCursorPosition(0, Console.CursorTop);
                                        Console.Write("> ");
                                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                                        Console.WriteLine(cmd[0] + ": " + cmd[1] + ": ");
                                        Console.ResetColor();
                                    }
                                    Console.WriteLine("> " + searching_directory + "\\" + cmd[i] + "/");
                                    first_found = true;
                                }
                                else
                                {
                                    results[i - 2].Add(searching_directory + "\\" + cmd[i] + "/");
                                }
                            }
                        }
                    } 
                }
                catch (Exception)
                {
                    
                }

                try
                {
                    all_folders = Directory.EnumerateDirectories(searching_directory).ToList();
                }
                catch (Exception)
                {

                }

                if (all_folders.Count > 0)
                {
                    for (int i = 0; i < all_folders.Count && !cancel; i++)
                    {
                        first_found = search_file_or_folder(cmd, ref results, all_folders[i], ref cancel, ref first_found) || first_found;
                    }
                }
            }

            return (first_found);
        }

        static public void add_1_usage_to_genius_cmd(string cmd, string appdata_dir)
        {
            List<string> content = new List<string>();
            string line;
            int i = 0;
            int usage = 0;

            if (File.Exists(appdata_dir + "/genius_data/genius_data_cmd") && Static_data.is_known_cmd(cmd, appdata_dir))
            {
                try
                {
                    StreamReader content_extractor = new StreamReader(appdata_dir + "/genius_data/genius_data_cmd");
                    line = content_extractor.ReadLine();

                    while (line != null)
                    {
                        content.Add(line);
                        line = content_extractor.ReadLine();
                    }

                    content_extractor.Close();

                    while (i < content.Count)
                    {
                        if (content[i].Length >= cmd.Length && content[i].Substring(0, cmd.Length) == cmd)
                        {
                            i = content.Count + i + 1;
                        }
                        else
                        {
                            i++;
                        }
                    }

                    usage = Convert.ToInt32(content[i - content.Count - 1].Substring(cmd.Length + 1, content[i - content.Count - 1].Length - cmd.Length - 1)) + 1;

                    if (i > content.Count)
                    {
                        content[i - content.Count - 1] = cmd + ":" + usage;
                    }

                    StreamWriter content_writer = new StreamWriter(appdata_dir + "/genius_data/genius_data_cmd");

                    for (i = 0; i < content.Count; i++)
                    {
                        content_writer.WriteLine(content[i]);
                    }

                    content_writer.Close();

                }
                catch (Exception)
                {

                }
            }
        }

        static private string get_time_format(int hours, int minutes, int seconds)
        {
            if(hours > 99)
            {
                return(hours + ":" + (minutes + 100).ToString().Substring(1,2) + ":" + (seconds + 100).ToString().Substring(1,2));
            }
            else
            {
                return ((hours + 100).ToString().Substring(1,2) + ":" + (minutes + 100).ToString().Substring(1,2) + ":" + (seconds + 100).ToString().Substring(1,2));
            }
        }

        static private void print_calendar(int nbr_week = 5)
        {
            DateTime day = DateTime.Now;
            int mounth = DateTime.Now.Month;
            int year = DateTime.Now.Year;

            Console.Write("> ");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(" M  T  W  T  F  S  S");

            for(int i = get_day_number(day); i > 0; i--)
            {
                day = day.AddDays(-1);
            }

            for (int i = 0; i < nbr_week; i++)
            {
                Console.ResetColor();
                Console.Write("> ");
                for (int j = 0; j < 7 && nbr_week > 0; j++)
                {
                    Console.ResetColor();
                    if (i == 0 && day.Day == DateTime.Now.Day)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                    }

                    if (day.Day < 10)
                    {
                        Console.Write(" " + day.Day + " ");
                    }
                    else
                    {
                        Console.Write(day.Day + " ");
                    }

                    if(day.Year < 9999 || day.Month < 12 || day.Day < 31)
                    {
                        day = day.AddDays(1);
                    }
                    else
                    {
                        nbr_week = 0;
                    }
                }

                if(nbr_week > 0)
                {
                    if (day.AddDays(-1).DayOfWeek == DayOfWeek.Sunday && day.AddDays(-1).Day <= 7)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write(get_month_name(day.AddDays(-1).Month) + " " + day.AddDays(-1).Year);
                        Console.ResetColor();
                    }
                    else if (i == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write(get_month_name(day.AddDays(-1).Month) + " " + day.AddDays(-1).Year);
                        Console.ResetColor();
                    }
                }
                Console.WriteLine();
            }    
        }

        static private int get_day_number(DateTime date)
        {
            switch(date.DayOfWeek)
            {
                case(DayOfWeek.Monday):
                    return (0);
                case (DayOfWeek.Tuesday):
                    return (1);
                case (DayOfWeek.Wednesday):
                    return (2);
                case (DayOfWeek.Thursday):
                    return (3);
                case (DayOfWeek.Friday):
                    return (4);
                case (DayOfWeek.Saturday):
                    return (5);
                case (DayOfWeek.Sunday):
                    return (6);
                default:
                    return (0);
            }
        }

        static private string get_month_name(int mounth)
        {
            switch(mounth)
            {
                case (1):
                    return ("January");
                case (2):
                    return ("February");
                case (3):
                    return ("March");
                case (4):
                    return ("April");
                case (5):
                    return ("May");
                case (6):
                    return ("June");
                case (7):
                    return ("July");
                case (8):
                    return ("August");
                case (9):
                    return ("September");
                case (10):
                    return ("October");
                case (11):
                    return ("November");
                case (12):
                    return ("December");
                default:
                    return ("Unknown");
            }
        }

        static private int get_longest_elt(List<string> list, int start_len = 0)
        {
            for(int i = 0; i < list.Count; i++)
            {
                if(list[i].Length > start_len)
                {
                    start_len = list[i].Length;
                }
            }

            return (start_len);
        }

        static private void sort_by_connection_status(ref List<string> all_networks_name, ref List<string> all_networks_signal, ref List<string> all_networks_cryptage, ref List<string> all_networks_security, ref List<string> all_networks_status)
        {
            int pos_connected = 0;
            string swap;

            for(int i = 0; i < all_networks_status.Count; i++)
            {
                if(all_networks_status[i] != "0")
                {
                    swap = all_networks_name[pos_connected];
                    all_networks_name[pos_connected] = all_networks_name[i];
                    all_networks_name[i] = swap;

                    swap = all_networks_signal[pos_connected];
                    all_networks_signal[pos_connected] = all_networks_signal[i];
                    all_networks_signal[i] = swap;

                    swap = all_networks_cryptage[pos_connected];
                    all_networks_cryptage[pos_connected] = all_networks_cryptage[i];
                    all_networks_cryptage[i] = swap;

                    swap = all_networks_security[pos_connected];
                    all_networks_security[pos_connected] = all_networks_security[i];
                    all_networks_security[i] = swap;

                    swap = all_networks_status[pos_connected];
                    all_networks_status[pos_connected] = all_networks_status[i];
                    all_networks_status[i] = swap;

                    pos_connected++;
                }
            }
        }
    
        static private bool is_script_format(string file_name)
        {
            int i = file_name.Length - 1;

            while(i >= 0 && file_name[i] != '.')
            {
                i--;
            }

            return (i < 0);
        }

        static private string get_battery_status()
        {
            string batteryStatus = "";

            if (SystemInformation.PowerStatus.BatteryChargeStatus == BatteryChargeStatus.NoSystemBattery || SystemInformation.PowerStatus.BatteryChargeStatus == BatteryChargeStatus.Unknown)
            {
                return ("No Battery");
            }

            batteryStatus = "Battery: " + SystemInformation.PowerStatus.BatteryLifePercent * 100 + "%";
            if (SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online)
            {
                batteryStatus = batteryStatus + " (Charging)";
            }
            else
            {
                if (SystemInformation.PowerStatus.BatteryLifeRemaining > -1)
                {
                    batteryStatus = batteryStatus + " (" + convert_second_to_time(SystemInformation.PowerStatus.BatteryLifeRemaining) + ")";
                }
            }

            return (batteryStatus);
        }

        static private string convert_second_to_time(int second)
        {
            string result = "";
            
            if(second / 3600 > 0)
            {
                result = result + second / 3600 + "h";
                second = second % 3600;
            }
            if(second / 60 > 0)
            {
                if (second / 60 >= 10 || result == "")
                {
                    result = result + second / 60 + "m";
                }
                else
                {
                    result = result + "0" + second / 60 + "m";
                }
                second = second % 60;
            }
            if(second > 0)
            {
                if (second >= 10 || result == "")
                {
                    result = result + second + "s";
                }
                else
                {
                    result = result + "0" + second + "s";
                }
            }

            return (result);
        }
    }
}
