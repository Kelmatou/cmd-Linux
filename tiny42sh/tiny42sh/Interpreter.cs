using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace cmd_Linux
{
    public static class Interpreter
    {
        public enum Keyword
        {
            cd,
            ls,
            cat,
            touch,
            rm,
            rmdir,
            mkdir,
            pwd,
            clear,
            whoami,
            hostname,
            help,
            launch,
            shutdown,
            info,
            cp,
            cpdir,
            resize,
            taskkill,
            home,
            uninstall,
            url,
            integreted_web_site,
            facebook,
            twitter,
            google,
            youtube,
            wikipedia,
            translate,
            bing,
            yahoo,
            map,
            exit,
            lolapp,
            skype,
            ts,
            reset,
            textedit,
            script,
            tasklist,
            mv,
            mvdir,
            sl,
            rename,
            password,
            @lock,
            option,
            wait,
            echo,
            find,
            time,
            seed,
            network,
            news,
            tv,
            //breakout,
            zip,
            ascii,
            dice,
            link,
            reminder,
            tree,
            hash,
            crypto,
            @if,
            @while,
            @for,
            man,
            admin,
            unadmin,
            None
        }

        static public string[][] parse_input(string input, string appdata_dir)
        {
            List<string> dynamic_commands = extract_commands(input);
            List<string> dynamic_words = new List<string>();
            string[][] commands = new string[dynamic_commands.Count][];

            for (int i = 0; i < commands.Length; i++ )
            {
                if(i > commands.Length - 1)
                    dynamic_words = extract_words(dynamic_commands[i].Substring(0,dynamic_commands[i].Length - 1));
                else
                    dynamic_words = extract_words(dynamic_commands[i]);
                commands[i] = new string[dynamic_words.Count];
                commands[i] = copy_list(dynamic_words);
            }

            return (commands);
        }

        static public List<string> extract_commands(string input)
        {
            List<string> commands = new List<string>();
            bool quote_mode = false;
            int last_stop = 0;
            int i = 0;

            while (i < input.Length)
            {
                if (input[i] == ';' && !quote_mode)
                {
                    if(input.Substring(last_stop, i - last_stop).Length > 0)
                    {
                        commands.Add(input.Substring(last_stop, i - last_stop));
                    }
                    last_stop = i + 1;
                    while(last_stop < input.Length && input[last_stop] == ';')
                    {
                        last_stop++;
                        i++;
                    }
                }
                else if(input[i] == '"')
                {
                    quote_mode = !quote_mode;
                }
                i++;
            }
            if (last_stop < input.Length)
            {
                if (input.Substring(last_stop, i - last_stop).Length > 0)
                {
                    commands.Add(input.Substring(last_stop, i - last_stop));
                }
            }

            return (commands);
        }

        static public List<string> extract_words(string command)
        {
            int i = 0;
            int last_stop = 0;
            List<string> current_word = new List<string>();
            bool quote_mode = false;
            List<string> words = new List<string>();

            while(i < command.Length)
            {
                if(command[i] == ' ' && !quote_mode)
                {
                    if (command[last_stop] == '"')
                    {
                        if (command.Substring(last_stop + 1, i - last_stop - 2).Length > 0)
                        {
                            words.Add(command.Substring(last_stop + 1, i - last_stop - 2));
                        }
                    }
                    else
                    {
                        if (command.Substring(last_stop, i - last_stop).Length > 0)
                        {
                            words.Add(command.Substring(last_stop, i - last_stop));
                        }
                    }
                    last_stop = i + 1;
                    while(last_stop < command.Length && command[last_stop] == ' ')
                    {
                        last_stop++;
                        i++;
                    }
                }
                else if(command[i] == '"')
                    quote_mode = !quote_mode;
                i++;
            }
            if (last_stop < command.Length)
            {
                if (command[last_stop] == '"' && command.Length > last_stop + 1)
                {
                    if (command.Substring(last_stop + 1, i - last_stop - 2).Length > 0)
                    {
                        words.Add(command.Substring(last_stop + 1, i - last_stop - 2));
                        if (words[words.Count - 1].Length > 1 && words[words.Count - 1][words[words.Count - 1].Length - 1] == ';')
                            words[words.Count - 1] = words[words.Count - 1].Substring(0, words[words.Count - 1].Length - 1);
                    }
                }
                else
                {
                    if (command.Substring(last_stop, i - last_stop).Length > 0)
                    {
                        words.Add(command.Substring(last_stop, i - last_stop));
                    }
                }
            }

            return (words);
        }

        static public string[] copy_list(List<string> list)
        {
            string[] tab = new string[list.Count];

            for(int i = 0; i < list.Count; i++)
            {
                tab[i] = list[i];
            }

            return (tab);
        }

        static public void replaceLinks(ref string[] cmd, List<Link> allLinks)
        {
            for(int i = 0; i < cmd.Length; i++)
            {
                if(cmd[i].Length > 0 && cmd[i][0] == '~')
                {
                    for(int j = 0; j < allLinks.Count; j++)
                    {
                        if (allLinks[j].name == cmd[i])
                        {
                            cmd[i] = allLinks[j].target;
                            break;
                        } 
                    }
                }
            }
        }

        static public Keyword is_keyword(string word)
        {
            switch(word)
            {
                case("cd"):
                    return (Keyword.cd);
                case("ls"):
                    return (Keyword.ls);
                case ("touch"):
                    return (Keyword.touch);
                case ("rm"):
                    return (Keyword.rm);
                case ("rmdir"):
                    return (Keyword.rmdir);
                case ("mkdir"):
                    return (Keyword.mkdir);
                case ("pwd"):
                    return (Keyword.pwd);
                case ("clear"):
                    return (Keyword.clear);
                case ("whoami"):
                    return (Keyword.whoami);
                case ("hostname"):
                    return (Keyword.hostname);
                case ("cat"):
                    return (Keyword.cat);
                case("help"):
                    return (Keyword.help);
                case("launch"):
                    return (Keyword.launch);
                case ("shutdown"):
                    return (Keyword.shutdown);
                case ("info"):
                    return (Keyword.info);
                case ("cp"):
                    return (Keyword.cp);
                case ("cpdir"):
                    return (Keyword.cpdir);
                case ("resize"):
                    return (Keyword.resize);
                case ("taskkill"):
                    return (Keyword.taskkill);
                case ("home"):
                    return (Keyword.home);
                case ("uninstall"):
                    return (Keyword.uninstall);
                case ("url"):
                    return (Keyword.url);
                case ("exit"):
                    return (Keyword.exit);
                case ("facebook"):
                    return (Keyword.facebook);
                case ("twitter"):
                    return (Keyword.twitter);
                case ("youtube"):
                    return (Keyword.youtube);
                case ("google"):
                    return (Keyword.google);
                case ("lolapp"):
                    return (Keyword.lolapp);
                case ("wikipedia"):
                    return (Keyword.wikipedia);
                case ("skype"):
                    return (Keyword.skype);
                case ("ts"):
                    return (Keyword.ts);
                case ("9gag"):
                    return (Keyword.integreted_web_site);
                case ("yahoo"):
                    return (Keyword.yahoo);
                case ("dailymotion"):
                    return (Keyword.integreted_web_site);
                case ("allocine"):
                    return (Keyword.integreted_web_site);
                case ("hotmail"):
                    return (Keyword.integreted_web_site);
                case ("outlook"):
                    return (Keyword.integreted_web_site);
                case ("gmail"):
                    return (Keyword.integreted_web_site);
                case ("bing"):
                    return (Keyword.bing);
                case ("map"):
                    return (Keyword.map);
                case ("translate"):
                    return (Keyword.translate);
                case ("reset"):
                    return (Keyword.reset);
                case ("textedit"):
                    return (Keyword.textedit);
                case ("script"):
                    return (Keyword.script);
                case ("tasklist"):
                    return (Keyword.tasklist);
                case ("mv"):
                    return (Keyword.mv);
                case ("mvdir"):
                    return (Keyword.mvdir);
                case ("sl"):
                    return (Keyword.sl);
                case ("rename"):
                    return (Keyword.rename);
                case ("password"):
                    return (Keyword.password);
                case ("lock"):
                    return (Keyword.@lock);
                case ("option"):
                    return (Keyword.option);
                case ("wait"):
                    return (Keyword.wait);
                case ("echo"):
                    return (Keyword.echo);
                case ("find"):
                    return (Keyword.find);
                case ("weather"):
                    return (Keyword.integreted_web_site);
                case ("time"):
                    return (Keyword.time);
                case ("seed"):
                    return (Keyword.seed);
                case ("network"):
                    return (Keyword.network);
                case ("news"):
                    return (Keyword.news);
                case ("tv"):
                    return (Keyword.tv);
                /*case ("breakout"):
                    return (Keyword.breakout);*/
                case ("zip"):
                    return (Keyword.zip);
                case ("ascii"):
                    return (Keyword.ascii);
                case ("dice"):
                    return (Keyword.dice);
                case ("reminder"):
                    return (Keyword.reminder);
                case ("link"):
                    return (Keyword.link);
                case ("tree"):
                    return (Keyword.tree);
                case ("hash"):
                    return (Keyword.hash);
                case ("crypto"):
                    return (Keyword.crypto);
                case ("if"):
                    return (Keyword.@if);
                case ("while"):
                    return (Keyword.@while);
                case ("for"):
                    return (Keyword.@for);
                case ("man"):
                    return (Keyword.man);
                case ("/admin"):
                    return (Keyword.admin);
                case ("/unadmin"):
                    return (Keyword.unadmin);
                default:
                    return (Keyword.None);
            }
        }

        //GENIUS PART

        static public string genius_cmd(string current_input_cmd, int genius_code, List<Genius_data> all_genius_data, string command, string appdata_dir, List<Link> allLinks, List<string> all_keywords = null)
        {
            string genius_result = null;
            List<string> all_arg;
            List<string> all_folders = new List<string>();
            List<string> all_files = new List<string>();
            List<string> all_folders_seed = new List<string>();
            List<string> all_files_seed = new List<string>();
            int i = 0;
            if(current_input_cmd.Length > 0 && current_input_cmd[0] == '"')
                current_input_cmd = current_input_cmd.Substring(1, current_input_cmd.Length - 1);

            if(current_input_cmd.Length > 0)
            {
                if(current_input_cmd[0] == '~')
                {
                    while(i < allLinks.Count && genius_result == null) //search links
                    {
                        if (allLinks[i].name.Length > current_input_cmd.Length && allLinks[i].name.Substring(0, current_input_cmd.Length) == current_input_cmd)
                            genius_result = allLinks[i].name;
                        i++;
                    }
                }

                if(genius_code == 0)
                {
                    if(all_keywords != null)
                    {
                        i = 0;
                        while (i < all_keywords.Count && genius_result == null) //search command
                        {
                            if (all_keywords[i].Length > current_input_cmd.Length && all_keywords[i].Substring(0, current_input_cmd.Length) == current_input_cmd)
                                genius_result = all_keywords[i];
                            i++;
                        }
                    }

                    if (genius_result == null && Directory.Exists(appdata_dir + "/script_files/")) //add script name
                    {
                        all_files = Directory.EnumerateFiles(appdata_dir + "/script_files/").ToList();
                    }

                    i = 0;
                    while (i < all_files.Count && genius_result == null) //search scripts
                    {
                        if (extract_shorter_path(all_files[i]).Length > current_input_cmd.Length && extract_shorter_path(all_files[i]).Substring(0, current_input_cmd.Length) == current_input_cmd)
                            genius_result = extract_shorter_path(all_files[i]);
                        i++;
                    }

                    if (genius_result == null && Directory.Exists(Directory.GetCurrentDirectory() + "\\" + get_cmd_path_dyna(current_input_cmd))) //search files to execute
                    {
                        try
                        {
                            all_files = Directory.EnumerateFiles(Directory.GetCurrentDirectory() + "\\" + get_cmd_path_dyna(current_input_cmd)).ToList();
                            all_files = sort_by_length(all_files);
                        }
                        catch (Exception)
                        {

                        }
                    }

                    i = 0;
                    while (i < all_files.Count && genius_result == null) //search scripts
                    {
                        if (extract_shorter_path(all_files[i]).Length > current_input_cmd.Length && extract_shorter_path(all_files[i]).Substring(0, current_input_cmd.Length) == current_input_cmd)
                            genius_result = extract_shorter_path(all_files[i]);
                        i++;
                    }
                }
                else if(genius_code >= 1 && genius_code <= 3)
                {
                    if (Directory.Exists(Directory.GetCurrentDirectory() + "\\" + get_cmd_path_dyna(current_input_cmd))) //collect data about all files and folders
                    {
                        try
                        {
                            all_folders = Directory.EnumerateDirectories(Directory.GetCurrentDirectory() + "\\" + get_cmd_path_dyna(current_input_cmd)).ToList();
                            all_folders = sort_by_length(all_folders);

                        }
                        catch (Exception)
                        {

                        }
                        try
                        {
                            all_files = Directory.EnumerateFiles(Directory.GetCurrentDirectory() + "\\" + get_cmd_path_dyna(current_input_cmd)).ToList();
                            all_files = sort_by_length(all_files);
                        }
                        catch (Exception)
                        {

                        }
                    }
                    if (Directory.Exists(get_cmd_path_dyna(current_input_cmd))) //PATH ABSOLUTE
                    {
                        try //seed folders
                        {
                            string debug = get_cmd_path_dyna(current_input_cmd);
                            all_folders_seed = Directory.EnumerateDirectories(get_cmd_path_dyna(current_input_cmd)).ToList();
                            all_folders_seed = sort_by_length(all_folders_seed);
                        }
                        catch (Exception)
                        {
                        }

                        try //seed files
                        {
                            all_files_seed = Directory.EnumerateFiles(get_cmd_path_dyna(current_input_cmd)).ToList();
                            all_files_seed = sort_by_length(all_files_seed);
                        }
                        catch (Exception)
                        {
                        }
                    }

                    if (genius_code == 3 || genius_code == 1) //search directory then files
                    {
                        i = 0;
                        while (i < all_folders.Count && genius_result == null) //search directory
                        {
                            if (extract_shorter_path(all_folders[i]).Length > extract_shorter_path(current_input_cmd).Length && extract_shorter_path(all_folders[i]).Substring(0, extract_shorter_path(current_input_cmd).Length) == extract_shorter_path(current_input_cmd))
                                genius_result = get_cmd_path_dyna(current_input_cmd) + extract_shorter_path(all_folders[i]) + "/";
                            i++;
                        }

                        i = 0;
                        while (i < all_files.Count && genius_result == null) //search file
                        {
                            if (extract_shorter_path(all_files[i]).Length > extract_shorter_path(current_input_cmd).Length && extract_shorter_path(all_files[i]).Substring(0, extract_shorter_path(current_input_cmd).Length) == extract_shorter_path(current_input_cmd))
                                genius_result = get_cmd_path_dyna(current_input_cmd) + extract_shorter_path(all_files[i]);
                            i++;
                        }

                        i = 0;
                        while (i < all_folders_seed.Count && genius_result == null) //search directory IN ABSOLUTE
                        {
                            if (extract_shorter_path(all_folders_seed[i]).Length > extract_shorter_path(current_input_cmd).Length && extract_shorter_path(all_folders_seed[i]).Substring(0, extract_shorter_path(current_input_cmd).Length) == extract_shorter_path(current_input_cmd))
                                genius_result = get_cmd_path_dyna(current_input_cmd) + extract_shorter_path(all_folders_seed[i]) + "/";
                            i++;
                        }

                        i = 0;
                        while (i < all_files_seed.Count && genius_result == null) //search file IN ABSOLUTE
                        {
                            if (extract_shorter_path(all_files_seed[i]).Length > extract_shorter_path(current_input_cmd).Length && extract_shorter_path(all_files_seed[i]).Substring(0, extract_shorter_path(current_input_cmd).Length) == extract_shorter_path(current_input_cmd))
                                genius_result = get_cmd_path_dyna(current_input_cmd) + extract_shorter_path(all_files_seed[i]);
                            i++;
                        }
                    }
                    else if (genius_code == 2) //search file then directory
                    {
                        i = 0;
                        if (command.Length > 7 && command.Substring(0, 6) == "launch")
                        {
                            all_files.Add("/planned");
                            all_files.Add("/with");
                        }
                            

                        while (i < all_files.Count && genius_result == null) //search file
                        {
                            if (extract_shorter_path(all_files[i]).Length > extract_shorter_path(current_input_cmd).Length && extract_shorter_path(all_files[i]).Substring(0, extract_shorter_path(current_input_cmd).Length) == extract_shorter_path(current_input_cmd))
                                genius_result = get_cmd_path_dyna(current_input_cmd) + extract_shorter_path(all_files[i]);
                            i++;
                        }

                        i = 0;
                        while (i < all_folders.Count && genius_result == null) //search directory
                        {
                            if (extract_shorter_path(all_folders[i]).Length > extract_shorter_path(current_input_cmd).Length && extract_shorter_path(all_folders[i]).Substring(0, extract_shorter_path(current_input_cmd).Length) == extract_shorter_path(current_input_cmd))
                                genius_result = get_cmd_path_dyna(current_input_cmd) + extract_shorter_path(all_folders[i]) + "/";
                            i++;
                        }

                        while (i < all_files_seed.Count && genius_result == null) //search file IN ABSOLUTE
                        {
                            if (extract_shorter_path(all_files_seed[i]).Length > extract_shorter_path(current_input_cmd).Length && extract_shorter_path(all_files_seed[i]).Substring(0, extract_shorter_path(current_input_cmd).Length) == extract_shorter_path(current_input_cmd))
                                genius_result = get_cmd_path_dyna(current_input_cmd) + extract_shorter_path(all_files_seed[i]);
                            i++;
                        }

                        i = 0;
                        while (i < all_folders_seed.Count && genius_result == null) //search directory IN ABSOLUTE
                        {
                            if (extract_shorter_path(all_folders_seed[i]).Length > extract_shorter_path(current_input_cmd).Length && extract_shorter_path(all_folders_seed[i]).Substring(0, extract_shorter_path(current_input_cmd).Length) == extract_shorter_path(current_input_cmd))
                                genius_result = get_cmd_path_dyna(current_input_cmd) + extract_shorter_path(all_folders_seed[i]) + "/";
                            i++;
                        }
                    }
                }
                else if(genius_code >= 4 && genius_code <= 15) //search /xxx
                {
                    if (genius_code == 4)
                        all_arg = new List<string>() { "/a;", "/g;", "/h;", "/l;", "/p;", "/r;", "/t", "now" };
                    else if (genius_code == 5)
                        all_arg = new List<string>() { "/champion", "/chat", "/game", "/history", "/profile", "/team" };
                    else if (genius_code == 6)
                        all_arg = new List<string>() { "/genius", "/stats" };
                    else if (genius_code == 7)
                        all_arg = new List<string>() { "/drake", "/plane;", "/tank;", "/train;" };
                    else if (genius_code == 8)
                        all_arg = new List<string>() { "/calendar", "/chrono", "/delay" };
                    else if (genius_code == 9)
                        all_arg = new List<string>() { "/default", "/full" };
                    else if (genius_code == 10)
                        all_arg = new List<string>() { "/afrikaans", "/albanian", "/arabic", "/armenian", "/azeri", "/basque", "/bengali", "/belarusian", "/burmese", "/bosnian", "/bulgarian", "/catalan", "/cebuano", "/chichewa", "/chinese", "/corean", "/creole", "/croatian", "/czech", "/danish", "/dutch", "/english", "/esperanto", "/estonian", "/finnish", "/french", "/galician", "/welsh", "/georgian", "/german", "/greek", "/gujarati", "/hausa", "/hebrew", "/hmong", "/hungarian", "/icelandic", "/igbo", "/indi", "/indonesian", "/italian", "/irish", "/japanese", "/javanese", "/kannada", "/kazakh", "/khmer", "/laos", "/latin", "/latvian", "/lithuanian", "/macedonian", "/malagasy", "/malayalam", "/malaysian", "/maltese", "/maori", "/marathi", "/mongolian", "/nepalese", "/norwegian", "/persian", "/polish", "/portuguese", "/punjabi", "/romanian", "/russian", "/serbian", "/sesotho", "/singhalese", "/slovak", "/slovenian", "/spanish", "/somali", "/sundanese", "/swahili", "/swedish", "/tagalog", "/tajik", "/tamil", "/telugu", "/thai", "/turkish", "/ukrainian", "/urdu", "/uzbek", "/vietnamese", "/yiddish", "/yoruba", "/zulu" };
                    else if (genius_code == 11)
                        all_arg = new List<string>() { "/connected" };
                    else if (genius_code == 12)
                        all_arg = new List<string>() {/*"/add", */"/extract", "/mk" };
                    else if (genius_code == 13)
                        all_arg = new List<string>() { "/display", "/mk", "/rm" };
                    else if (genius_code == 14)
                        all_arg = new List<string>() { "/add", "/display", "/rm", "friday", "monday", "saturday", "sunday", "thursday", "tomorrow", "tuesday", "wednesday" };
                    else
                        all_arg = new List<string>() { "/decrypt", "/encrypt" };

                    i = 0;
                    while (i < all_arg.Count && genius_result == null)
                    {
                        if (all_arg[i].Length > current_input_cmd.Length && all_arg[i].Substring(0, current_input_cmd.Length) == current_input_cmd)
                            genius_result = all_arg[i];
                        if (all_arg[i][0] > current_input_cmd[0])
                            i = all_arg.Count;
                        i++;
                    }
                }
                else if(genius_code == 16) //search process (task)
                {
                    Process[] process_found = Process.GetProcesses();

                    i = 0;
                    while (i < process_found.Length && genius_result == null)
                    {
                        if (process_found[i].ProcessName.Length > current_input_cmd.Length && process_found[i].ProcessName.Substring(0, current_input_cmd.Length) == current_input_cmd)
                            genius_result = process_found[i].ProcessName;
                        i++;
                    }
                }
                else if (genius_code == 17) //search script
                {
                    all_files = new List<string>();
                    if (Directory.Exists(appdata_dir + "/script_files/"))
                        all_files = Directory.EnumerateFiles(appdata_dir + "/script_files/").ToList();
                    all_files.Add("/mk");
                    all_files.Add("/rm");
                    all_files.Add("/export");
                    all_files.Add("/import");
                    all_files.Add("/rename");
                    all_files.Add("/display");

                    i = 0;
                    while (i < all_files.Count - 6 && genius_result == null) //search script file
                    {
                        if (extract_shorter_path(all_files[i]).Length > current_input_cmd.Length && extract_shorter_path(all_files[i]).Substring(0, current_input_cmd.Length) == current_input_cmd)
                            genius_result = extract_shorter_path(all_files[i]);
                        i++;
                    }
                    while (i < all_files.Count && genius_result == null) //search script /rm or /mk
                    {
                        if (all_files[i].Length > current_input_cmd.Length && all_files[i].Substring(0, current_input_cmd.Length) == current_input_cmd)
                            genius_result = all_files[i];
                        i++;
                    }
                }
                else if(genius_code == 18)
                {
                    if (current_input_cmd.Length < 7 && current_input_cmd == "http://".Substring(0,current_input_cmd.Length))
                        genius_result = "http://";
                    else if (current_input_cmd.Length < 4 && current_input_cmd == "www.".Substring(0, current_input_cmd.Length))
                        genius_result = "www.";
                }
                
                if(genius_result == null && genius_code != 0) //search genius data
                {
                    i = 0;
                    while (i < all_genius_data.Count && genius_result == null)
                    {
                        if (all_genius_data[i].data.Length > current_input_cmd.Length && all_genius_data[i].data.Substring(0, current_input_cmd.Length) == current_input_cmd && (all_genius_data[i].command_usage == get_cmd_type(command) || all_genius_data[i].command_usage == "dictionary"))
                            genius_result = all_genius_data[i].data;
                        i++;
                    }
                }

                if(genius_result == null)
                    genius_result = "";
            }
            else
                return ("");

            return (genius_result);
        }

        static public string genius_linux(string current_input_cmd, int genius_code, List<Genius_data> all_genius_data, string command, string appdata_dir, List<Link> allLinks, List<string> all_keywords = null)
        {
            string longest_radical = null;
            List<string> all_arg;
            List<string> all_folders = new List<string>();
            List<string> all_files = new List<string>();
            List<string> all_folders_seed = new List<string>();
            List<string> all_files_seed = new List<string>();
            int i = 0;
            if (current_input_cmd.Length > 0 && current_input_cmd[0] == '"')
                current_input_cmd = current_input_cmd.Substring(1, current_input_cmd.Length - 1);

            if (current_input_cmd.Length > 0)
            {
                if (current_input_cmd[0] == '~')
                {
                    while (i < allLinks.Count) //search links
                    {
                        if (allLinks[i].name.Length > current_input_cmd.Length && allLinks[i].name.Substring(0, current_input_cmd.Length) == current_input_cmd)
                        {
                            if (longest_radical == null)
                                longest_radical = allLinks[i].name;
                            else
                                longest_radical = get_common_radical(allLinks[i].name, longest_radical, current_input_cmd.Length);
                        }
                        i++;
                    }
                }
                i++;

                if (genius_code == 0)
                {
                    if(all_keywords != null)
                    {
                        i = 0;
                        while (i < all_keywords.Count) //search command
                        {
                            if (all_keywords[i].Length > current_input_cmd.Length && all_keywords[i].Substring(0, current_input_cmd.Length) == current_input_cmd)
                            {
                                if (longest_radical == null)
                                    longest_radical = all_keywords[i];
                                else
                                    longest_radical = get_common_radical(all_keywords[i], longest_radical, current_input_cmd.Length);
                            }
                            i++;
                        }
                    }
                    
                    if (longest_radical == null && Directory.Exists(appdata_dir + "/script_files/")) //add script name
                    {
                        all_files = Directory.EnumerateFiles(appdata_dir + "/script_files/").ToList();
                        i = 0;
                        while (i < all_files.Count) //search scripts
                        {
                            if (extract_shorter_path(all_files[i]).Length > current_input_cmd.Length && extract_shorter_path(all_files[i]).Substring(0, current_input_cmd.Length) == current_input_cmd)
                            {
                                if (longest_radical == null)
                                    longest_radical = extract_shorter_path(all_files[i]);
                                else
                                    longest_radical = get_common_radical(extract_shorter_path(all_files[i]), longest_radical, current_input_cmd.Length);
                            }
                            i++;
                        }
                    }

                    if (longest_radical == null && Directory.Exists(Directory.GetCurrentDirectory() + "\\" + get_cmd_path_dyna(current_input_cmd)))
                    {
                        try
                        {
                            all_files = Directory.EnumerateFiles(Directory.GetCurrentDirectory() + "\\" + get_cmd_path_dyna(current_input_cmd)).ToList();
                            all_files = sort_by_length(all_files);
                            i = 0;
                            while (i < all_files.Count) //search scripts
                            {
                                if (extract_shorter_path(all_files[i]).Length > current_input_cmd.Length && extract_shorter_path(all_files[i]).Substring(0, current_input_cmd.Length) == current_input_cmd)
                                {
                                    if (longest_radical == null)
                                        longest_radical = extract_shorter_path(all_files[i]);
                                    else
                                        longest_radical = get_common_radical(extract_shorter_path(all_files[i]), longest_radical, current_input_cmd.Length);
                                }
                                i++;
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }      
                }
                else if (genius_code >= 1 && genius_code <= 3)
                {
                    if (Directory.Exists(Directory.GetCurrentDirectory() + "\\" + get_cmd_path_dyna(current_input_cmd))) //collect data about all files and folders
                    {
                        try
                        {
                            all_folders = Directory.EnumerateDirectories(Directory.GetCurrentDirectory() + "\\" + get_cmd_path_dyna(current_input_cmd)).ToList();
                            all_folders = sort_by_length(all_folders);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    if (Directory.Exists(Directory.GetCurrentDirectory() + "\\" + get_cmd_path_dyna(current_input_cmd)))
                    {
                        try
                        {
                            all_files = Directory.EnumerateFiles(Directory.GetCurrentDirectory() + "\\" + get_cmd_path_dyna(current_input_cmd)).ToList();
                            all_files = sort_by_length(all_files);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    if (Directory.Exists(get_cmd_path_dyna(current_input_cmd))) //PATH ABSOLUTE
                    {
                        try //seed folders
                        {
                            string debug = get_cmd_path_dyna(current_input_cmd);
                            all_folders_seed = Directory.EnumerateDirectories(get_cmd_path_dyna(current_input_cmd)).ToList();
                            all_folders_seed = sort_by_length(all_folders_seed);
                        }
                        catch (Exception)
                        {
                        }

                        try //seed files
                        {
                            all_files_seed = Directory.EnumerateFiles(get_cmd_path_dyna(current_input_cmd)).ToList();
                            all_files_seed = sort_by_length(all_files_seed);
                        }
                        catch (Exception)
                        {
                        }
                    }

                    if (genius_code == 3 || genius_code == 1) //search directory then files
                    {
                        i = 0;
                        while (i < all_folders.Count) //search directory
                        {
                            if (extract_shorter_path(all_folders[i]).Length > extract_shorter_path(current_input_cmd).Length && extract_shorter_path(all_folders[i]).Substring(0, extract_shorter_path(current_input_cmd).Length) == extract_shorter_path(current_input_cmd))
                            {
                                if (longest_radical == null)
                                    longest_radical = get_cmd_path_dyna(current_input_cmd) + extract_shorter_path(all_folders[i]) + "/";
                                else
                                    longest_radical = get_common_radical(get_cmd_path_dyna(current_input_cmd) + extract_shorter_path(all_folders[i]) + "/", longest_radical, current_input_cmd.Length);
                            }
                            i++;
                        }

                        i = 0;
                        while (i < all_files.Count) //search file
                        {
                            if (extract_shorter_path(all_files[i]).Length > extract_shorter_path(current_input_cmd).Length && extract_shorter_path(all_files[i]).Substring(0, extract_shorter_path(current_input_cmd).Length) == extract_shorter_path(current_input_cmd))
                            {
                                if (longest_radical == null)
                                    longest_radical = get_cmd_path_dyna(current_input_cmd) + extract_shorter_path(all_files[i]);
                                else
                                    longest_radical = get_common_radical(get_cmd_path_dyna(current_input_cmd) + extract_shorter_path(all_files[i]), longest_radical, current_input_cmd.Length);
                            }
                            i++;
                        }

                        i = 0;
                        while (i < all_folders_seed.Count) //search directory IN ABSOLUTE
                        {
                            if (extract_shorter_path(all_folders_seed[i]).Length > extract_shorter_path(current_input_cmd).Length && extract_shorter_path(all_folders_seed[i]).Substring(0, extract_shorter_path(current_input_cmd).Length) == extract_shorter_path(current_input_cmd))
                            {
                                if (longest_radical == null)
                                    longest_radical = get_cmd_path_dyna(current_input_cmd) + extract_shorter_path(all_folders_seed[i]) + "/";
                                else
                                    longest_radical = get_common_radical(get_cmd_path_dyna(current_input_cmd) + extract_shorter_path(all_folders_seed[i]) + "/", longest_radical, current_input_cmd.Length);
                            }
                            i++;
                        }

                        i = 0;
                        while (i < all_files_seed.Count) //search file IN ABSOLUTE
                        {
                            if (extract_shorter_path(all_files_seed[i]).Length > extract_shorter_path(current_input_cmd).Length && extract_shorter_path(all_files_seed[i]).Substring(0, extract_shorter_path(current_input_cmd).Length) == extract_shorter_path(current_input_cmd))
                            {
                                if (longest_radical == null)
                                    longest_radical = get_cmd_path_dyna(current_input_cmd) + extract_shorter_path(all_files_seed[i]);
                                else
                                    longest_radical = get_common_radical(get_cmd_path_dyna(current_input_cmd) + extract_shorter_path(all_files_seed[i]), longest_radical, current_input_cmd.Length);
                            }
                            i++;
                        }
                    }
                    else if (genius_code == 2) //search file then directory
                    {
                        i = 0;
                        if (command.Length > 7 && command.Substring(0, 6) == "launch")
                        {
                            all_files.Add("/planned");
                            all_files.Add("/with");
                        }

                        while (i < all_files.Count) //search file
                        {
                            if (extract_shorter_path(all_files[i]).Length > extract_shorter_path(current_input_cmd).Length && extract_shorter_path(all_files[i]).Substring(0, extract_shorter_path(current_input_cmd).Length) == extract_shorter_path(current_input_cmd))
                            {
                                if (longest_radical == null)
                                    longest_radical = get_cmd_path_dyna(current_input_cmd) + extract_shorter_path(all_files[i]);
                                else
                                    longest_radical = get_common_radical(get_cmd_path_dyna(current_input_cmd) + extract_shorter_path(all_files[i]), longest_radical, current_input_cmd.Length);
                            }
                            i++;
                        }

                        i = 0;
                        while (i < all_folders.Count) //search directory
                        {
                            if (extract_shorter_path(all_folders[i]).Length > extract_shorter_path(current_input_cmd).Length && extract_shorter_path(all_folders[i]).Substring(0, extract_shorter_path(current_input_cmd).Length) == extract_shorter_path(current_input_cmd))
                            {
                                if (longest_radical == null)
                                    longest_radical = get_cmd_path_dyna(current_input_cmd) + extract_shorter_path(all_folders[i]) + "/";
                                else
                                    longest_radical = get_common_radical(get_cmd_path_dyna(current_input_cmd) + extract_shorter_path(all_folders[i]) + "/", longest_radical, current_input_cmd.Length);
                            }
                            i++;
                        }

                        i = 0;
                        while (i < all_files_seed.Count) //search file IN ABSOLUTE
                        {
                            if (extract_shorter_path(all_files_seed[i]).Length > extract_shorter_path(current_input_cmd).Length && extract_shorter_path(all_files_seed[i]).Substring(0, extract_shorter_path(current_input_cmd).Length) == extract_shorter_path(current_input_cmd))
                            {
                                if (longest_radical == null)
                                    longest_radical = get_cmd_path_dyna(current_input_cmd) + extract_shorter_path(all_files_seed[i]);
                                else
                                    longest_radical = get_common_radical(get_cmd_path_dyna(current_input_cmd) + extract_shorter_path(all_files_seed[i]), longest_radical, current_input_cmd.Length);
                            }
                            i++;
                        }

                        i = 0;
                        while (i < all_folders_seed.Count) //search directory IN ABSOLUTE
                        {
                            if (extract_shorter_path(all_folders_seed[i]).Length > extract_shorter_path(current_input_cmd).Length && extract_shorter_path(all_folders_seed[i]).Substring(0, extract_shorter_path(current_input_cmd).Length) == extract_shorter_path(current_input_cmd))
                            {
                                if (longest_radical == null)
                                    longest_radical = get_cmd_path_dyna(current_input_cmd) + extract_shorter_path(all_folders_seed[i]) + "/";
                                else
                                    longest_radical = get_common_radical(get_cmd_path_dyna(current_input_cmd) + extract_shorter_path(all_folders_seed[i]) + "/", longest_radical, current_input_cmd.Length);
                            }
                            i++;
                        }
                    }
                }
                else if (genius_code >= 4 && genius_code <= 15) //search /xxx
                {
                    if (genius_code == 4)
                        all_arg = new List<string>() { "/a;", "/g;", "/h;", "/l;", "/p;", "/r;", "/t", "now" };
                    else if (genius_code == 5)
                        all_arg = new List<string>() { "/champion", "/chat", "/game", "/history", "/profile", "/team" };
                    else if (genius_code == 6)
                        all_arg = new List<string>() { "/genius", "/stats" };
                    else if (genius_code == 7)
                        all_arg = new List<string>() { "/drake", "/plane;", "/tank;", "/train;" };
                    else if (genius_code == 8)
                        all_arg = new List<string>() { "/calendar", "/chrono", "/delay" };
                    else if (genius_code == 9)
                        all_arg = new List<string>() { "/default", "/full" };
                    else if (genius_code == 10)
                        all_arg = new List<string>() { "/afrikaans", "/albanian", "/arabic", "/armenian", "/azeri", "/basque", "/bengali", "/belarusian", "/burmese", "/bosnian", "/bulgarian", "/catalan", "/cebuano", "/chichewa", "/chinese", "/corean", "/creole", "/croatian", "/czech", "/danish", "/dutch", "/english", "/esperanto", "/estonian", "/finnish", "/french", "/galician", "/welsh", "/georgian", "/german", "/greek", "/gujarati", "/hausa", "/hebrew", "/hmong", "/hungarian", "/icelandic", "/igbo", "/indi", "/indonesian", "/italian", "/irish", "/japanese", "/javanese", "/kannada", "/kazakh", "/khmer", "/laos", "/latin", "/latvian", "/lithuanian", "/macedonian", "/malagasy", "/malayalam", "/malaysian", "/maltese", "/maori", "/marathi", "/mongolian", "/nepalese", "/norwegian", "/persian", "/polish", "/portuguese", "/punjabi", "/romanian", "/russian", "/serbian", "/sesotho", "/singhalese", "/slovak", "/slovenian", "/spanish", "/somali", "/sundanese", "/swahili", "/swedish", "/tagalog", "/tajik", "/tamil", "/telugu", "/thai", "/turkish", "/ukrainian", "/urdu", "/uzbek", "/vietnamese", "/yiddish", "/yoruba", "/zulu" };
                    else if (genius_code == 11)
                        all_arg = new List<string>() { "/connected" };
                    else if (genius_code == 12)
                        all_arg = new List<string>() { /*"/add", */"/extract", "/mk" };
                    else if (genius_code == 13)
                        all_arg = new List<string>() { "/display", "/mk", "/rm" };
                    else if (genius_code == 14)
                        all_arg = new List<string>() { "/add", "/display", "/rm", "friday", "monday", "saturday", "sunday", "thursday", "tomorrow", "tuesday", "wednesday" };
                    else
                        all_arg = new List<string>() { "/decrypt", "/encrypt" };

                    i = 0;
                    while (i < all_arg.Count)
                    {
                        if (all_arg[i].Length > current_input_cmd.Length && all_arg[i].Substring(0, current_input_cmd.Length) == current_input_cmd)
                        {
                            if (longest_radical == null)
                                longest_radical = all_arg[i];
                            else
                                longest_radical = get_common_radical(all_arg[i], longest_radical, current_input_cmd.Length);
                        }
                        if (all_arg[i][0] > current_input_cmd[0])
                            i = all_arg.Count;
                        i++;
                    }
                }
                else if (genius_code == 16) //search process (task)
                {
                    Process[] process_found = Process.GetProcesses();

                    i = 0;
                    while (i < process_found.Length)
                    {
                        if (process_found[i].ProcessName.Length > current_input_cmd.Length && process_found[i].ProcessName.Substring(0, current_input_cmd.Length) == current_input_cmd)
                        {
                            if (longest_radical == null)
                                longest_radical = process_found[i].ProcessName;
                            else
                                longest_radical = get_common_radical(process_found[i].ProcessName, longest_radical, current_input_cmd.Length);
                        }
                        i++;
                    }
                }
                else if (genius_code == 17) //search script
                {
                    all_files = new List<string>();
                    if (Directory.Exists(appdata_dir + "/script_files/"))
                        all_files = Directory.EnumerateFiles(appdata_dir + "/script_files/").ToList();
                    all_files.Add("/mk");
                    all_files.Add("/rm");
                    all_files.Add("/export");
                    all_files.Add("/import");
                    all_files.Add("/rename");
                    all_files.Add("/display");

                    i = 0;
                    while (i < all_files.Count - 6) //search script file
                    {
                        if (extract_shorter_path(all_files[i]).Length > current_input_cmd.Length && extract_shorter_path(all_files[i]).Substring(0, current_input_cmd.Length) == current_input_cmd)
                        {
                            if (longest_radical == null)
                                longest_radical = extract_shorter_path(all_files[i]);
                            else
                                longest_radical = get_common_radical(extract_shorter_path(all_files[i]), longest_radical, current_input_cmd.Length);
                        }
                        i++;
                    }
                    while (i < all_files.Count) //search script /rm or /mk...
                    {
                        if (all_files[i].Length > current_input_cmd.Length && all_files[i].Substring(0, current_input_cmd.Length) == current_input_cmd)
                        {
                            if (longest_radical == null)
                                longest_radical = all_files[i];
                            else
                                longest_radical = get_common_radical(all_files[i], longest_radical, current_input_cmd.Length);
                        }
                        i++;
                    }
                }
                else if (genius_code == 18)
                {
                    if (current_input_cmd.Length < 7 && current_input_cmd == "http://".Substring(0, current_input_cmd.Length))
                        longest_radical = "http://";
                    else if (current_input_cmd.Length < 4 && current_input_cmd == "www.".Substring(0, current_input_cmd.Length))
                        longest_radical = "www.";
                }

                if (longest_radical == null && genius_code != 0) //search genius data
                {
                    i = 0;
                    while (i < all_genius_data.Count)
                    {
                        if (all_genius_data[i].data.Length > current_input_cmd.Length && all_genius_data[i].data.Substring(0, current_input_cmd.Length) == current_input_cmd && (all_genius_data[i].command_usage == get_cmd_type(command) || all_genius_data[i].command_usage == "dictionary"))
                        {
                            
                            if (longest_radical == null)
                                longest_radical = all_genius_data[i].data;
                            else
                                longest_radical = get_common_radical(all_genius_data[i].data, longest_radical, current_input_cmd.Length);
                        }
                        i++;
                    }
                }

                if (longest_radical == null)
                    longest_radical = "";
            }
            else
                return ("");

            return (longest_radical);
        }

        static public void print_genius_cmd(string genius, string current_input, ref int max_cursor)
        {
            int cursorL = Console.CursorLeft;
            int cursorT = Console.CursorTop;

            if (genius.Length > 1 && genius[genius.Length - 1] == '"')
            {
                genius = genius.Substring(1, genius.Length - 2);
            }
            if (genius.Length > 0 && current_input.Length < genius.Length)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write(genius.Substring(current_input.Length, genius.Length - current_input.Length));
                Console.ResetColor();
            }

            max_cursor = Console.CursorTop;
            Console.SetCursorPosition(cursorL, cursorT);
        }

        static public string extract_shorter_path(string full_path)
        {
            int i = full_path.Length - 1;

            while (i >= 0 && full_path[i] != '\\' && full_path[i] != '/')
            {
                i--;
            }

            if (i < 0)
            {
                return (full_path);
            }
            else
            {
                return (full_path.Substring(i + 1, full_path.Length - i - 1));
            }
        }

        static private string get_cmd_path_dyna(string current_cmd)
        {
            int i = current_cmd.Length - 1;

            while(i >= 0 && current_cmd[i] != '\\' && current_cmd[i] != '/')
            {
                i--;
            }

            if(i >= 0)
            {
                return (current_cmd.Substring(0, i+1));
            }
            else
            {
                return ("");
            }
        }

        static public int get_arg_expected_code(string command)
        {
            /* CODE:
              -1    =   libre
               0    =   mot de commande
               1    =   directory
               2    =   file then directory (for functions who prefer file arg)
               3    =   directory then file (for functions who prefer directory arg)
               4    =   parametre de commande shutdown
               5    =   parametre de commande lolapp
               6    =   parametre de commande reset
               7    =   parametre de commande sl
               8    =   parametre de commande time
               9    =   parametre de commande resize
               10   =   parametre de commande language
               11   =   parametre de commande network
               12   =   parametre de commande zip
               13   =   parametre de commande link
               14   =   parametre de commande reminder
               15   =   parametre de commande crypto
               16   =   task
               17   =   script name
               18   =   http://*/
            int i = command.Length - 1;
            List<string> all_arg_current_command = new List<string>();

            if(command.Length > 0)
            {
                while (i >= 0 && command[i] != ';')
                    i--;

                command = command.Substring(i + 1, command.Length - i - 1);
                all_arg_current_command = extract_words(command);
                if(all_arg_current_command.Count > 0)
                    all_arg_current_command.Remove(all_arg_current_command[all_arg_current_command.Count - 1]);

                if (all_arg_current_command.Count == 0 || all_arg_current_command[0] == "help" || all_arg_current_command[0] == "man" || (all_arg_current_command.Count > 1 && all_arg_current_command[0] == "reset") || all_arg_current_command[0] == "if" || all_arg_current_command[0] == "while" || all_arg_current_command[0] == "for")
                    return (0);
                else if ((all_arg_current_command[0] == "cd" && all_arg_current_command.Count == 1) || all_arg_current_command[0] == "rmdir" || (all_arg_current_command[0] == "cpdir" && (all_arg_current_command.Count < 3)) || (all_arg_current_command[0] == "cp" && all_arg_current_command.Count == 2) || (all_arg_current_command[0] == "mv" && all_arg_current_command.Count == 2) || (all_arg_current_command[0] == "mvdir" && all_arg_current_command.Count < 3))
                    return (1);
                else if (all_arg_current_command[0] == "rm" || all_arg_current_command[0] == "cat" || all_arg_current_command[0] == "launch" || (all_arg_current_command[0] == "cp" && all_arg_current_command.Count == 1) || (all_arg_current_command[0] == "mv" && all_arg_current_command.Count == 1) || (all_arg_current_command[0] == "textedit" && all_arg_current_command.Count == 1) || (all_arg_current_command.Count >= 2 && all_arg_current_command[0] == "script" && all_arg_current_command[1] == "/import") || (((all_arg_current_command.Count == 1 && command.Length > 5 && command[5] != '/') || all_arg_current_command.Count == 2) && all_arg_current_command[0] == "link") || all_arg_current_command[0] == "hash" || (all_arg_current_command.Count > 1 && all_arg_current_command[0] == "crypto"))
                    return (2);
                else if (all_arg_current_command[0] == "ls" || all_arg_current_command[0] == "tree" || all_arg_current_command[0] == "touch" || (all_arg_current_command[0] == "rename" && all_arg_current_command.Count == 1) || all_arg_current_command[0] == "find" || (all_arg_current_command.Count > 1 && all_arg_current_command[0] == "zip"))
                    return (3);
                else if (all_arg_current_command.Count == 1 && all_arg_current_command[0] == "shutdown")
                    return (4);
                else if (all_arg_current_command.Count == 1 && all_arg_current_command[0] == "lolapp")
                    return (5);
                else if (all_arg_current_command.Count == 1 && all_arg_current_command[0] == "reset")
                    return (6);
                else if(all_arg_current_command.Count == 1 && all_arg_current_command[0] == "sl")
                    return (7);
                else if (all_arg_current_command.Count == 1 && all_arg_current_command[0] == "time")
                    return (8);
                else if (all_arg_current_command.Count == 1 && all_arg_current_command[0] == "resize")
                    return (9);
                else if ((all_arg_current_command.Count == 1 || all_arg_current_command.Count == 2) && (all_arg_current_command[0] == "translate" || all_arg_current_command[0] == "wikipedia"))
                    return (10);
                else if (all_arg_current_command.Count == 1 && all_arg_current_command[0] == "network")
                    return (11);
                else if (all_arg_current_command.Count == 1 && all_arg_current_command[0] == "zip")
                    return (12);
                else if (all_arg_current_command.Count == 1 && all_arg_current_command[0] == "link")
                    return (13);
                else if ((all_arg_current_command.Count == 1 || all_arg_current_command.Count == 3) && all_arg_current_command[0] == "reminder")
                    return (14);
                else if (all_arg_current_command.Count == 1 && all_arg_current_command[0] == "crypto")
                    return (15);
                else if(all_arg_current_command[0] == "taskkill")
                    return (16);
                else if(all_arg_current_command[0] == "script")
                    return (17);
                else if(all_arg_current_command[0] == "url")
                    return (18);
            }

            return (-1);
        }

        static public string get_cmd_type(string command)
        {
            int i = command.Length - 1;
            int last_space = command.Length - 1;

            if(command.Length > 0 || command[command.Length - 1] == ';')
            {
                while(i >= 0 && command[i] != ';')
                {
                    if(command[i] == ' ')
                    {
                        last_space = i;
                    }
                    i--;
                }

                return (command.Substring(i + 1, command.Length - (i + 1) - (command.Length - last_space)));
            }
            else
            {
                return("");
            }
            
        }

        static public string get_common_radical(string arg1, string arg2, int begin = 0)
        {
            while(begin < arg1.Length && begin < arg2.Length && arg1[begin] == arg2[begin])
            {
                begin++;
            }

            return (arg1.Substring(0, begin));
        }

        static public List<string> sort_by_length(List<string> original_list)
        {
            bool modification = true;
            string swap;

            while(modification)
            {
                modification = false;
                for(int i = 0; i < original_list.Count - 1; i++)
                {
                    if(original_list[i].Length > original_list[i + 1].Length)
                    {
                        swap = original_list[i];
                        original_list[i] = original_list[i + 1];
                        original_list[i + 1] = swap;
                        modification = true;
                    }
                }
            }

            return (original_list);
        }
    }
}
