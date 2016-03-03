using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace cmd_Linux
{
    public static class Static_data
    {
        //DATA

        public static List<string> cmd_linux_commands()
        {
            return (new List<string>() { "/admin;", "/unadmin;", "9gag;", "allocine;", "bing", "breakout", "cat", "cd", "clear;", "cp", "cpdir", "dailymotion;", "echo", "exit;", "facebook", "find", "gmail;", "google", "help", "home;", "hostname;", "hotmail;", "info;", "launch", "lock", "lolapp", "ls", "man", "map", "mkdir", "mv", "mvdir", "network", "news", "option;", "outlook;", "password;", "pwd;", "rename", "reset", "resize", "rm", "rmdir", "script", "seed;", "shutdown", "skype;", "sl", "taskkill", "tasklist;", "textedit", "time", "touch", "translate", "ts;", "tv;", "twitter", "uninstall;", "url", "wait", "weather;", "whoami;", "wikipedia", "yahoo", "youtube" });
        }

        //DATA FUNCTION

        public static bool is_known_cmd(string cmd, string appdata_dir)
        {
            return (Interpreter.is_keyword(cmd) != Interpreter.Keyword.None || Interpreter.is_keyword(cmd + ";") != Interpreter.Keyword.None  || File.Exists(appdata_dir + "/script_files/" + cmd));
        }

        public static bool is_original_cmd(string cmd)
        {
            return (Interpreter.is_keyword(cmd) != Interpreter.Keyword.None);
        }

        public static bool match_cmd_with_cmd_linux_command(ref List<int> all_cmd_usage, string cmd, int usage)
        {
            switch (cmd)
            {
                case ("/admin"):
                    all_cmd_usage[0] = usage;
                    break;
                case ("/unadmin"):
                    all_cmd_usage[1] = usage;
                    break;
                case ("9gag"):
                    all_cmd_usage[2] = usage;
                    break;
                case ("allocine"):
                    all_cmd_usage[3] = usage;
                    break;
                case ("bing"):
                    all_cmd_usage[4] = usage;
                    break;
                case ("breakout"):
                    all_cmd_usage[5] = usage;
                    break;
                case ("cat"):
                    all_cmd_usage[6] = usage;
                    break;
                case ("cd"):
                    all_cmd_usage[7] = usage;
                    break;
                case ("clear"):
                    all_cmd_usage[8] = usage;
                    break;
                case ("cp"):
                    all_cmd_usage[9] = usage;
                    break;
                case ("cpdir"):
                    all_cmd_usage[10] = usage;
                    break;
                case ("dailymotion"):
                    all_cmd_usage[11] = usage;
                    break;
                case ("echo"):
                    all_cmd_usage[12] = usage;
                    break;
                case ("exit"):
                    all_cmd_usage[13] = usage;
                    break;
                case ("facebook"):
                    all_cmd_usage[14] = usage;
                    break;
                case("find"):
                    all_cmd_usage[15] = usage;
                    break;
                case ("gmail"):
                    all_cmd_usage[16] = usage;
                    break;
                case ("google"):
                    all_cmd_usage[17] = usage;
                    break;
                case ("help"):
                    all_cmd_usage[18] = usage;
                    break;
                case ("home"):
                    all_cmd_usage[19] = usage;
                    break;
                case ("hostname"):
                    all_cmd_usage[20] = usage;
                    break;
                case ("hotmail"):
                    all_cmd_usage[21] = usage;
                    break;
                case ("info"):
                    all_cmd_usage[22] = usage;
                    break;
                case ("launch"):
                    all_cmd_usage[23] = usage;
                    break;
                case ("lock"):
                    all_cmd_usage[24] = usage;
                    break;
                case ("lolapp"):
                    all_cmd_usage[25] = usage;
                    break;
                case ("ls"):
                    all_cmd_usage[26] = usage;
                    break;
                case ("man"):
                    all_cmd_usage[27] = usage;
                    break;
                case ("map"):
                    all_cmd_usage[28] = usage;
                    break;
                case ("mkdir"):
                    all_cmd_usage[29] = usage;
                    break;
                case ("mv"):
                    all_cmd_usage[30] = usage;
                    break;
                case ("mvdir"):
                    all_cmd_usage[31] = usage;
                    break;
                case ("network"):
                    all_cmd_usage[32] = usage;
                    break;
                case ("news"):
                    all_cmd_usage[33] = usage;
                    break;
                case ("option"):
                    all_cmd_usage[34] = usage;
                    break;
                case ("outlook"):
                    all_cmd_usage[35] = usage;
                    break;
                case ("password"):
                    all_cmd_usage[36] = usage;
                    break;
                case ("pwd"):
                    all_cmd_usage[37] = usage;
                    break;
                case ("rename"):
                    all_cmd_usage[38] = usage;
                    break;
                case ("reset"):
                    all_cmd_usage[39] = usage;
                    break;
                case ("resize"):
                    all_cmd_usage[40] = usage;
                    break;
                case ("rm"):
                    all_cmd_usage[41] = usage;
                    break;
                case ("rmdir"):
                    all_cmd_usage[42] = usage;
                    break;
                case ("script"):
                    all_cmd_usage[43] = usage;
                    break;
                case ("seed"):
                    all_cmd_usage[44] = usage;
                    break;
                case ("shutdown"):
                    all_cmd_usage[45] = usage;
                    break;
                case ("skype"):
                    all_cmd_usage[46] = usage;
                    break;
                case ("sl"):
                    all_cmd_usage[47] = usage;
                    break;
                case ("taskkill"):
                    all_cmd_usage[48] = usage;
                    break;
                case ("tasklist"):
                    all_cmd_usage[49] = usage;
                    break;
                case ("textedit"):
                    all_cmd_usage[50] = usage;
                    break;
                case ("time"):
                    all_cmd_usage[51] = usage;
                    break;
                case ("touch"):
                    all_cmd_usage[52] = usage;
                    break;
                case ("translate"):
                    all_cmd_usage[53] = usage;
                    break;
                case ("ts"):
                    all_cmd_usage[54] = usage;
                    break;
                case ("tv"):
                    all_cmd_usage[55] = usage;
                    break;
                case ("twitter"):
                    all_cmd_usage[56] = usage;
                    break;
                case ("uninstall"):
                    all_cmd_usage[57] = usage;
                    break;
                case ("url"):
                    all_cmd_usage[58] = usage;
                    break;
                case ("wait"):
                    all_cmd_usage[59] = usage;
                    break;
                case ("weather"):
                    all_cmd_usage[60] = usage;
                    break;
                case ("whoami"):
                    all_cmd_usage[61] = usage;
                    break;
                case ("wikipedia"):
                    all_cmd_usage[62] = usage;
                    break;
                case ("yahoo"):
                    all_cmd_usage[63] = usage;
                    break;
                case ("youtube"):
                    all_cmd_usage[64] = usage;
                    break;
                default:
                    return (false);
            }

            return (true);
        }
    }
}
