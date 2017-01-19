using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace cmd_Linux
{
    public static class ShellEnvironment
    {
        public static bool working = true;
        public static bool superuser = false;
        public static bool script_enable = true;
        public static bool genius_enable = true;
        public static bool private_mode = false;
        public static bool auto_lock = false;
        public static bool auto_log = false;
        public static bool cmd_print_time = false;
        public static bool cmd_print_user = true;
        public static bool cmd_print_path = true;
        public static ConsoleColor time_color = ConsoleColor.Black;
        public static ConsoleColor user_color = ConsoleColor.DarkGreen;
        public static ConsoleColor path_color = ConsoleColor.DarkYellow;
        public static string language = "en";
        public static int max_genius_data = 10000; // = unlimited
        public static int refresh_timer = 50;
        public static string command = "";
        public static string desktop_dir = Directory.GetCurrentDirectory();
        public static string appdata_dir = "";
        public static string[] previous_directories = new string[256];
        public static string[] stack_input = new string[256];
        public static List<Genius_data> all_genius_data = new List<Genius_data>();
        public static List<Link> allLinks = new List<Link>();
        public static NotificationManager notificationManager;
        public static int stack_pointer = 0;
        public static int previous_directory_pointer = -1;
        public static long last_result = 0;
        public static List<string> all_cmd = new List<string>();

        public static void init()
        {
            Directory.SetCurrentDirectory(find_desktop());
            ShellEnvironment.desktop_dir = Directory.GetCurrentDirectory();
            ShellEnvironment.appdata_dir = find_appdata();
            for (int i = 0; i < 256; i++)
            {
                stack_input[i] = "";
                previous_directories[i] = "";
            }
            previous_directories[0] = Directory.GetCurrentDirectory();
            get_settings_option(ref script_enable, ref refresh_timer, ref auto_lock, ref genius_enable, ref private_mode, ref auto_log, ref cmd_print_user, ref cmd_print_path, ref cmd_print_time, ref max_genius_data, ref language, appdata_dir);
            ShellEnvironment.all_genius_data = extract_genius_data(appdata_dir, max_genius_data);
            ShellEnvironment.notificationManager = new NotificationManager(appdata_dir + "/notification");
            ShellEnvironment.all_cmd = init_genius_cmd(appdata_dir);
        }

        private static string find_desktop()
        {
            char racine = 'A';
            int i = 0;

            while (i < 26 && !Directory.Exists(racine + ":/Users/" + Environment.UserName + "/Desktop/"))
            {
                i++;
                racine = (char)(racine + 1);
            }

            if (26 == i)
                return (Directory.GetCurrentDirectory());
            else
                return (racine + ":/Users/" + Environment.UserName + "/Desktop/");
        }

        private static string find_appdata()
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

                if (i == 26)
                {
                    Console.Clear();
                    Console.Write("> ");
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("cmd Linux: FATAL ERROR: can't find application ressources!");
                    Console.ResetColor();
                    Console.Write("> ");
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("cmd Linux: try to reinstall the application");
                    Console.ResetColor();
                    return ("");
                }
                else
                {
                    try
                    {
                        Directory.CreateDirectory(racine + ":/Users/" + Environment.UserName + "/AppData/Roaming/Kelmatou Apps/cmd Linux/");
                        Directory.CreateDirectory(racine + ":/Users/" + Environment.UserName + "/AppData/Roaming/Kelmatou Apps/cmd Linux/genius_data");
                        string appdata_dir = racine + ":/Users/" + Environment.UserName + "/AppData/Roaming/Kelmatou Apps/cmd Linux/";
                        StreamWriter eraser;
                        for (int j = 0; j < Static_data.cmd_linux_commands().Count; j++)
                        {
                            if (Static_data.has_genius_data_file(Static_data.cmd_linux_commands()[j]))
                            {
                                if (Static_data.cmd_linux_commands()[j][Static_data.cmd_linux_commands()[j].Length - 1] == ';')
                                    eraser = new StreamWriter(appdata_dir + "genius_data/" + Static_data.cmd_linux_commands()[j].Substring(0, Static_data.cmd_linux_commands()[j].Length - 1));
                                else
                                    eraser = new StreamWriter(appdata_dir + "genius_data/" + Static_data.cmd_linux_commands()[j]);
                                eraser.Close();
                            }
                        }
                        eraser = new StreamWriter(appdata_dir + "/skype_location");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/ts_location");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/lolapp_location");
                        eraser.Close();
                        eraser = new StreamWriter(appdata_dir + "/cmd.bat");
                        eraser.Close();
                        return (appdata_dir);
                    }
                    catch (Exception)
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

        private static void get_settings_option(ref bool script_enable, ref int refresh_timer, ref bool auto_lock, ref bool genius_enable, ref bool private_mode, ref bool auto_log, ref bool cmd_print_user, ref bool cmd_print_path, ref bool cmd_print_time, ref int max_genius_data, ref string language, string appdata_dir)
        {
            List<string> content = new List<string>();

            if (File.Exists(appdata_dir + "/option"))
                content = Library.getFileContent(appdata_dir + "/option");

            if (content.Count == 14)
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
                    cmd_print_user = content[1][12] == 'T';
                if (content[2].Length > 17)
                    cmd_print_path = content[2][17] == 'T';
                if (content[3].Length > 12)
                    cmd_print_time = content[3][12] == 'T';
                if (content[4].Length > 14)
                    user_color = Library.get_color_name(content[4].Substring(12));
                if (content[5].Length > 14)
                    path_color = Library.get_color_name(content[5].Substring(12));
                if (content[6].Length > 14)
                    time_color = Library.get_color_name(content[6].Substring(12));
                if (content[7].Length > 19)
                {
                    language = content[7].Substring(19);
                    if (Execution.getLanguageName(language) == "Unknown")
                        language = "en";
                }
                if (content[8].Length > 15)
                    script_enable = content[8][15] == 'T';
                if (content[9].Length > 11)
                    auto_lock = content[9][11] == 'T';
                if (content[10].Length > 10)
                    auto_log = content[10][10] == 'T';
                if (content[11].Length > 15)
                    genius_enable = content[11][15] == 'T';
                if (content[12].Length >= 18)
                {
                    try
                    {
                        max_genius_data = Convert.ToInt32(content[12].Substring(17, content[12].Length - 17));
                    }
                    catch (Exception)
                    {
                        max_genius_data = 10000;
                    }
                }
                if (content[13].Length > 14)
                    private_mode = content[13][14] == 'T';
            }
        }

        private static List<string> init_genius_cmd(string appdata_dir)
        {
            List<string> all_cmd = Static_data.cmd_linux_commands();
            List<int> all_cmd_usage = new List<int>();
            List<string> all_content = new List<string>();
            for (int i = 0; i < all_cmd.Count; i++)
                all_cmd_usage.Add(-1);
            int start_index;

            if (File.Exists(appdata_dir + "/genius_data/genius_data_cmd"))
            {
                try
                {
                    all_content = Library.getFileContent(appdata_dir + "/genius_data/genius_data_cmd");

                    for (int i = 0; i < all_content.Count; i++)
                    {
                        start_index = 1;
                        while (all_content[i][start_index] != ':')
                            start_index++;
                        start_index++;

                        Static_data.match_cmd_with_cmd_linux_command(ref all_cmd_usage, all_content[i].Substring(0, start_index - 1), Convert.ToInt32(all_content[i].Substring(start_index, all_content[i].Length - start_index)));
                    }

                    if (!all_genius_cmd_found(ref all_cmd_usage))
                        reinit_file_genius_data_cmd(appdata_dir, all_cmd, all_cmd_usage);

                    all_cmd = sort_genius_cmd(all_cmd, all_cmd_usage);
                    return (all_cmd);
                }
                catch (Exception)
                {
                }
            }

            all_genius_cmd_found(ref all_cmd_usage);
            reinit_file_genius_data_cmd(appdata_dir, all_cmd, all_cmd_usage);

            return (all_cmd);
        }

        private static List<Genius_data> extract_genius_data(string appdata_dir, int max_genius_data)
        {
            List<Genius_data> all_genius_data = new List<Genius_data>();
            string[] all_genius_files = new string[40] { "genius_data_ascii", "genius_data_bing", "genius_data_cat", "genius_data_cd", "genius_data_cp", "genius_data_cpdir", "genius_data_dice", "genius_data_echo", "genius_data_facebook", "genius_data_find", "genius_data_for", "genius_data_google", "genius_data_hash", "genius_data_if", "genius_data_launch", "genius_data_link", "genius_data_lolapp", "genius_data_ls", "genius_data_map", "genius_data_mkdir", "genius_data_mv", "genius_data_mvdir", "genius_data_news", "genius_data_reminder", "genius_data_rename", "genius_data_rm", "genius_data_rmdir", "genius_data_textedit", "genius_data_time", "genius_data_touch", "genius_data_translate", "genius_data_tree", "genius_data_twitter", "genius_data_url", "genius_data_wait", "genius_data_while", "genius_data_wikipedia", "genius_data_yahoo", "genius_data_youtube", "genius_data_zip" };

            for (int i = 0; i < all_genius_files.Length; i++)
            {
                if (File.Exists(appdata_dir + "/genius_data/" + all_genius_files[i]))
                {
                    StreamReader data_reader = new StreamReader(appdata_dir + "/genius_data/" + all_genius_files[i]);
                    string current_word = data_reader.ReadLine();

                    while (current_word != null)
                    {
                        all_genius_data.Add(new Genius_data(all_genius_files[i].Substring(12, all_genius_files[i].Length - 12), current_word));
                        current_word = data_reader.ReadLine();
                    }

                    data_reader.Close();
                }
            }

            return (all_genius_data);
        } //si nouveau fichier genius data, loaded here

        private static List<string> sort_genius_cmd(List<string> data, List<int> data_usage)
        {
            int move = 1;
            int last_elt = 0;
            string data_swap;

            while (move > 0)
            {
                move = 0;
                for (int i = data_usage.Count - 1; i > last_elt; i--)
                {
                    if (data_usage[i] > data_usage[i - 1])
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
                    if (all_cmd_usage != null)
                    {
                        if (all_cmd[i][all_cmd[i].Length - 1] == ';')
                            recover.WriteLine(all_cmd[i].Substring(0, all_cmd[i].Length - 1) + ":" + all_cmd_usage[i]);
                        else
                            recover.WriteLine(all_cmd[i] + ":" + all_cmd_usage[i]);
                    }
                    else
                    {
                        if (all_cmd[i][all_cmd[i].Length - 1] == ';')
                            recover.WriteLine(all_cmd[i].Substring(0, all_cmd[i].Length - 1) + ":0");
                        else
                            recover.WriteLine(all_cmd[i] + ":0");
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

            for (int i = 0; i < all_cmd_usage.Count; i++)
            {
                if (all_cmd_usage[i] == -1)
                {
                    all_cmd_usage[i] = 0;
                    result = false;
                }
            }

            return (result);
        }
    }
}
