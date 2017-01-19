using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Security.Permissions;
using System.Security.Cryptography;
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
    public static class Library
    {

        //STRING & CHAR

        public static bool isSameChar(char c1, char c2)
        {
            if (c1 == c2)
                return (true);
            else if (c1 >= 65 && c1 <= 90)
                return (c1 + 32 == c2);
            else if (c1 >= 97 && c1 <= 122)
                return (c1 - 32 == c2);

            return (false);
        }

        public static bool isSameWord(string s1, string s2)
        {
            if (s1.Length == s2.Length)
            {
                if (s1 == s2)
                    return (true);
                for (int i = 0; i < s1.Length; i++)
                {
                    if (!isSameChar(s1[i], s2[i]))
                        return (false);
                }
            }
            else
                return (false);

            return (true);
        }

        public static string extractShorterPath(string full_path)
        {
            int i = full_path.Length - 1;

            while (i >= 0 && full_path[i] != '\\' && full_path[i] != '/')
                i--;

            if (i < 0)
                return (full_path);
            else
                return (full_path.Substring(i + 1, full_path.Length - i - 1));
        }

        public static string extractString(string s, char startCode, char endCode)
        {
            int start = 0;
            int end = 0;
            while (start < s.Length && s[start] != startCode)
                start++;
            end = start + 1;
            while (end < s.Length && s[end] != endCode)
                end++;
            return (start < s.Length - 1 || end < s.Length ? s.Substring(start + 1, end - start - 1) : "");
        }

        public static string removeAllChar(string s, char toRemove)
        {
            string result = "";
            for(int i = 0 ; i < s.Length; i++)
            {
                if (s[i] != toRemove)
                    result += s[i];
            }
            return result;
        }

        public static int firstOccurence(string s, string pattern, int begin = 0)
        {
            int i = begin;
            while (i <= s.Length - pattern.Length && s.Substring(i, pattern.Length) != pattern)
                i++;
            return (i <= s.Length - pattern.Length ? i : -1);
        }

        //FILES

        public static void createFile(string fileName)
        {
            StreamWriter creator = new StreamWriter(fileName);
            creator.Close();
        }

        public static bool saveFile(string fileName, List<string> content)
        {
            try
            {
                StreamWriter writer = new StreamWriter(fileName);
                for (int i = 0; i < content.Count; i++)
                    writer.WriteLine(content[i]);
                writer.Close();
                return (true);
            }
            catch (Exception)
            {
            }
            return (false);
        }

        public static List<string> getFileContent(string fileName)
        {
            List<string> content = new List<string>();
            string curLine = "";

            if(File.Exists(fileName))
            {
                try
                {
                    StreamReader reader = new StreamReader(fileName);
                    curLine = reader.ReadLine();
                    while (curLine != null)
                    {
                        content.Add(curLine);
                        curLine = reader.ReadLine();
                    }
                    reader.Close();
                }
                catch (Exception)
                {
                }
            }
            
            return (content);
        }

        public static bool deleteFile(string fileName)
        {
            if(File.Exists(fileName))
            {
                try
                {
                    File.Delete(fileName);
                    return (true);
                }
                catch (Exception)
                {
                }
            }
            
            return (false);
        }

        public static List<string> getAllFiles(string directory)
        {
            return (Directory.EnumerateFiles(directory).ToList());
        }

        public static List<string> getAllDirectories(string directory)
        {
            return (Directory.EnumerateDirectories(directory).ToList());
        } 

        //TAB AND LIST

        public static string[] removeArrElement(string[] tab, int index)
        {
            string[] result = new string[tab.Length - 1];
            bool found = false;
            if (index >= 0 && index < tab.Length)
            {
                for(int i = 0; i < tab.Length; i++)
                {
                    if (i != index)
                        result[found ? i - 1 : i] = tab[i];
                    else
                        found = true;
                }
            }
            else
                return (tab);
            return (result);
        }

        public static string[] insertArrElement(string[] tab, string element, int index)
        {
            string[] result = new string[tab.Length + 1]; 
            if (index >= 0 && index <= tab.Length)
            {
                for (int i = 0; i <= tab.Length; i++)
                {
                    if (i == index)
                        result[i] = element;
                    else
                        result[i] = tab[i < index ? i : i - 1];
                }
            }
            else
                return (tab);
            return (result);
        }

        public static string concArrToString(string[] tab, int begin, int end, bool addSpace = false)
        {
            string result = "";
            for (int i = begin; i < tab.Length && i < end; i++)
                result += (i == begin ? "" : addSpace ? " " : "") + tab[i];
            return (result);
        }

        public static string concListToString(List<string> list, int begin, int end, bool addSpace = false)
        {
            string result = "";
            for (int i = begin; i < list.Count && i < end; i++)
                result += (i == begin ? "" : addSpace ? " " : "") + list[i];
            return (result);
        }

        public static int firstStringIndex(string[] tab, string searched)
        {
            int i = 0;
            while (i < tab.Length && tab[i] != searched)
                i++;
            return (i < tab.Length ? i : -1);
        }

        public static int lastStringIndex(string[] tab, string searched)
        {
            int i = tab.Length - 1;
            while (i >= 0 && tab[i] != searched)
                i--;
            return (i >= 0 ? i : -1);
        }

        //DATETIME

        public static bool isSameDate(DateTime date1, DateTime date2, bool exactYear, bool exactMonth, bool exactDay, bool exactHour, bool exactMin, bool exactSec)
        {
            return ((date1.Year == date2.Year || !exactYear)
                && (date1.Month == date2.Month || !exactMonth)
                && (date1.Day == date2.Day || !exactDay)
                && (date1.Hour == date2.Hour || !exactHour)
                && (date1.Minute == date2.Minute || !exactMin)
                && (date1.Second == date2.Second || !exactSec));
        }

        public static string getDateFileFormat(DateTime date)
        {
            return (date.Year + "_" + date.Month + "_" + date.Day + "_" + date.Hour + "_" + date.Minute + "_" + date.Second);
        }

        public static DateTime getDateTimeFromString(string dateTimeString)
        {
            //tomorrow = next day, same time
            //"day of the week" = day mentionned, same time
            //date format: dd[/mm[/yyyy]]
            //time format: hh[:mm[:ss]
            //NOTE: if date is "dd" format, this function will assume it's a date only if "dd" is a value over 23 (max "hh"). Add month to precise date
            //if date is not valid, this function will return 1/1/1970 date
            DateTime result = DateTime.Now;
        
            if(Library.isSameWord(dateTimeString, "tomorrow"))
                result = DateTime.Now.AddDays(1);
            else if(Library.isSameWord(dateTimeString, "monday"))
            {
                do
                {
                    result = result.AddDays(1);
                }while(result.DayOfWeek != DayOfWeek.Monday);
            }
            else if(Library.isSameWord(dateTimeString, "tuesday"))
            {
                do
                {
                    result = result.AddDays(1);
                }while(result.DayOfWeek != DayOfWeek.Tuesday);
            }
            else if(Library.isSameWord(dateTimeString, "wednesday"))
            {
                do
                {
                    result = result.AddDays(1);
                }while(result.DayOfWeek != DayOfWeek.Wednesday);
            }
            else if(Library.isSameWord(dateTimeString, "thursday"))
            {
                do
                {
                    result = result.AddDays(1);
                }while(result.DayOfWeek != DayOfWeek.Thursday);
            }
            else if(Library.isSameWord(dateTimeString, "friday"))
            {
                do
                {
                    result = result.AddDays(1);
                }while(result.DayOfWeek != DayOfWeek.Friday);
            }
            else if(Library.isSameWord(dateTimeString, "saturday"))
            {
                do
                {
                    result = result.AddDays(1);
                }while(result.DayOfWeek != DayOfWeek.Saturday);
            }
            else if(Library.isSameWord(dateTimeString, "sunday"))
            {
                do
                {
                    result = result.AddDays(1);
                } while (result.DayOfWeek != DayOfWeek.Sunday);
            }
            else
            {
                int parameter1 = 0;
                int parameter2 = 0;
                int parameter3 = 0;
            
                if(dateTimeString.Split('/').Length > 1)
                {
                    string[] date = dateTimeString.Split('/');
                    parameter1 = 1;
                    parameter2 = 1;
                    parameter3 = 1970;
                    try
                    {
                        parameter1 = Convert.ToInt32(date[0]);
                        try
                        {
                            parameter2 = Convert.ToInt32(date[1]);
                            if(date.Length > 2)
                            {
                                try
                                {
                                    parameter3 = Convert.ToInt32(date[2]);
                                }
                                catch(Exception)
                                {
                                }
                            }
                            else
                                parameter3 = DateTime.Now.Year;
                        }
                        catch(Exception)
                        {
                        }
                    }
                    catch(Exception)
                    {
                    }
                    try
                    {
                        result = new DateTime(parameter3, parameter2, parameter1);
                    }
                    catch(Exception)
                    {
                        return (new DateTime(1970, 1, 1));
                    }
                }
                else if(dateTimeString.Split(':').Length > 1)
                {
                    string[] time = dateTimeString.Split(':');
                    try
                    {
                        parameter1 = Convert.ToInt32(time[0]);
                        try
                        {
                            parameter2 = Convert.ToInt32(time[1]);
                            if(time.Length > 2)
                            {
                                try
                                {
                                    parameter3 = Convert.ToInt32(time[2]);
                                }
                                catch(Exception)
                                {
                                }
                            }
                            else
                                parameter3 = 0;
                        }
                        catch(Exception)
                        {
                        }
                    }
                    catch(Exception)
                    {
                    }
                    try
                    {
                        result = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, parameter1, parameter2, parameter3);
                    }
                    catch (Exception)
                    {
                        return (new DateTime(1970, 1, 1));
                    }
                }
                else
                {
                    if(dateTimeString.Length == 1 || dateTimeString.Length == 2)
                    {
                        try
                        {
                            int value = Convert.ToInt32(dateTimeString);
                            if(value > 23)
                            {
                                try
                                {
                                    result = new DateTime(DateTime.Now.Year, DateTime.Now.Month, value);
                                }
                                catch (Exception)
                                {
                                    return (new DateTime(1970, 1, 1));
                                }
                            }
                            else if(value >= 0)
                            {
                                try
                                {
                                    result = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, value, 0, 0);
                                }
                                catch (Exception)
                                {
                                    return (new DateTime(1970, 1, 1));
                                }
                            }
                        }
                        catch(Exception)
                        {
                            result = new DateTime(1970, 1, 1);
                        }
                    }
                    else
                        result = new DateTime(1970, 1, 1);
                }
        }
        
        return(result);
    }

        public static DateTime getDateFromFileName(string fileName)
        {//file name: 2016_7_13_9_42_0 = 09:42:00 on 13/07/2016
            DateTime result = new DateTime(1970, 1, 1);
            string[] dateElements = fileName.Split('_');

            if (dateElements.Length == 6)
            {
                try
                {
                    result = new DateTime(Convert.ToInt32(dateElements[0]), Convert.ToInt32(dateElements[1]), Convert.ToInt32(dateElements[2]), Convert.ToInt32(dateElements[3]), Convert.ToInt32(dateElements[4]), Convert.ToInt32(dateElements[5]));
                }
                catch (Exception)
                {
                }
            }
            return (result);
        }

        public static bool getYearMonthDay(string date, ref DateTime result)
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;

            switch(date)
            {
                case ("tomorrow"):
                    result = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, result.Hour, result.Minute, result.Second);
                    result = result.AddDays(1);
                    break;
                case ("monday"):
                    result = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, result.Hour, result.Minute, result.Second);
                    do
                    {
                        result = result.AddDays(1);
                    } while (result.DayOfWeek != DayOfWeek.Monday);
                    break;
                case ("tuesday"):
                    result = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, result.Hour, result.Minute, result.Second);
                    do
                    {
                        result = result.AddDays(1);
                    } while (result.DayOfWeek != DayOfWeek.Tuesday);
                    break;
                case ("wednesday"):
                    result = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, result.Hour, result.Minute, result.Second);
                    do
                    {
                        result = result.AddDays(1);
                    } while (result.DayOfWeek != DayOfWeek.Wednesday);
                    break;
                case ("thursday"):
                    result = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, result.Hour, result.Minute, result.Second);
                    do
                    {
                        result = result.AddDays(1);
                    } while (result.DayOfWeek != DayOfWeek.Thursday);
                    break;
                case ("friday"):
                    result = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, result.Hour, result.Minute, result.Second);
                    do
                    {
                        result = result.AddDays(1);
                    } while (result.DayOfWeek != DayOfWeek.Friday);
                    break;
                case ("saturday"):
                    result = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, result.Hour, result.Minute, result.Second);
                    do
                    {
                        result = result.AddDays(1);
                    } while (result.DayOfWeek != DayOfWeek.Saturday);
                    break;
                case ("sunday"):
                    result = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, result.Hour, result.Minute, result.Second);
                    do
                    {
                        result = result.AddDays(1);
                    } while (result.DayOfWeek != DayOfWeek.Sunday);
                    break;
                default:
                    string[] elements = date.Split('/');
                    if (elements.Length < 4)
                    {
                        try
                        {
                            day = Convert.ToInt32(elements[0]);
                        }
                        catch (Exception)
                        {
                            return (false);
                        }

                        if (elements.Length > 1)
                        {
                            try
                            {
                                month = Convert.ToInt32(elements[1]);
                            }
                            catch (Exception)
                            {
                                return (false);
                            }
                            if (elements.Length > 2)
                            {
                                try
                                {
                                    year = Convert.ToInt32(elements[2]);
                                }
                                catch (Exception)
                                {
                                    return (false);
                                }
                            }
                        }
                    }
                    else
                        return (false);
                    result = new DateTime(year, month, day, result.Hour, result.Minute, result.Second);
                    break;
            }
            return (true);
        }

        public static bool getHourMinuteSecond(string time, ref DateTime result)
        {
            string[] elements = time.Split(':');
            int hour = DateTime.Now.Hour;
            int minute = DateTime.Now.Minute;
            int second = DateTime.Now.Second;
            if (elements.Length < 4)
            {
                try
                {
                    hour = Convert.ToInt32(elements[0]);
                }
                catch (Exception)
                {
                    return (false);
                }

                if (elements.Length > 1)
                {
                    try
                    {
                        minute = Convert.ToInt32(elements[1]);
                    }
                    catch (Exception)
                    {
                        return (false);
                    }
                    if (elements.Length > 2)
                    {
                        try
                        {
                            second = Convert.ToInt32(elements[2]);
                        }
                        catch (Exception)
                        {
                            return (false);
                        }
                    }
                }
            }
            else
                return (false);

            result = new DateTime(result.Year, result.Month, result.Day, hour, minute, second);
            return (true);
        }

        public static string dateFormat(DateTime date)
        {//hh:mm:ss on dd/mm/yyyy
            return ((date.Hour < 10 ? "0" : "") + date.Hour + ":" + (date.Minute < 10 ? "0" : "") + date.Minute + ":" + (date.Second < 10 ? "0" : "") + date.Second + " on " + (date.Day < 10 ? "0" : "") + date.Day + "/" + (date.Month < 10 ? "0" : "") + date.Month + "/" + date.Year);
        }

        public static bool date2IsBeforeDate1(DateTime date1, DateTime date2)
        {
            return (date1 >= date2);
        }

        public static int get_day_number(DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case (DayOfWeek.Monday):
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

        public static string getMonthName(int month)
        {
            switch(month)
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

        public static void printHourMinuteSecond(DateTime date)
        {
            Console.Write((date.Hour < 10 ? "0" : "") + date.Hour + ":" + (date.Minute < 10 ? "0" : "") + date.Minute + ":" + (date.Second < 10 ? "0" : "") + date.Second);
        }

        //COLOR

        public static string get_color_name(ConsoleColor color)
        {
            switch (color)
            {
                case (ConsoleColor.Black):
                    return ("Default");
                case (ConsoleColor.Blue):
                    return ("Blue");
                case (ConsoleColor.Cyan):
                    return ("Cyan");
                case (ConsoleColor.DarkBlue):
                    return ("Dark Blue");
                case (ConsoleColor.DarkCyan):
                    return ("Dark Cyan");
                case (ConsoleColor.DarkGray):
                    return ("Dark Gray");
                case (ConsoleColor.DarkGreen):
                    return ("Dark Green");
                case (ConsoleColor.DarkMagenta):
                    return ("Dark Magenta");
                case (ConsoleColor.DarkRed):
                    return ("Dark Red");
                case (ConsoleColor.DarkYellow):
                    return ("Dark Yellow");
                case (ConsoleColor.Gray):
                    return ("Gray");
                case (ConsoleColor.Green):
                    return ("Green");
                case (ConsoleColor.Magenta):
                    return ("Magenta");
                case (ConsoleColor.Red):
                    return ("Red");
                case (ConsoleColor.White):
                    return ("White");
                case (ConsoleColor.Yellow):
                    return ("Yellow");
                default:
                    return ("Unkown");
            }
        }

        public static ConsoleColor get_color_name(string color)
        {
            switch (color)
            {
                case ("Default"):
                    return (ConsoleColor.Black);
                case ("Blue"):
                    return (ConsoleColor.Blue);
                case ("Cyan"):
                    return (ConsoleColor.Cyan);
                case ("Dark Blue"):
                    return (ConsoleColor.DarkBlue);
                case ("Dark Cyan"):
                    return (ConsoleColor.DarkCyan);
                case ("Dark Gray"):
                    return (ConsoleColor.DarkGray);
                case ("Dark Green"):
                    return (ConsoleColor.DarkGreen);
                case ("Dark Magenta"):
                    return (ConsoleColor.DarkMagenta);
                case ("Dark Red"):
                    return (ConsoleColor.DarkRed);
                case ("Dark Yellow"):
                    return (ConsoleColor.DarkYellow);
                case ("Gray"):
                    return (ConsoleColor.Gray);
                case ("Green"):
                    return (ConsoleColor.Green);
                case ("Magenta"):
                    return (ConsoleColor.Magenta);
                case ("Red"):
                    return (ConsoleColor.Red);
                case ("White"):
                    return (ConsoleColor.White);
                case ("Yellow"):
                    return (ConsoleColor.Yellow);
                default:
                    return (ConsoleColor.Black);
            }
        }

        public static ConsoleColor get_next_color(ConsoleColor color)
        {
            switch (color)
            {
                case (ConsoleColor.Black):
                    return (ConsoleColor.Blue);
                case (ConsoleColor.Blue):
                    return (ConsoleColor.Cyan);
                case (ConsoleColor.Cyan):
                    return (ConsoleColor.DarkBlue);
                case (ConsoleColor.DarkBlue):
                    return (ConsoleColor.DarkCyan);
                case (ConsoleColor.DarkCyan):
                    return (ConsoleColor.DarkGray);
                case (ConsoleColor.DarkGray):
                    return (ConsoleColor.DarkGreen);
                case (ConsoleColor.DarkGreen):
                    return (ConsoleColor.DarkMagenta);
                case (ConsoleColor.DarkMagenta):
                    return (ConsoleColor.DarkRed);
                case (ConsoleColor.DarkRed):
                    return (ConsoleColor.DarkYellow);
                case (ConsoleColor.DarkYellow):
                    return (ConsoleColor.Gray);
                case (ConsoleColor.Gray):
                    return (ConsoleColor.Green);
                case (ConsoleColor.Green):
                    return (ConsoleColor.Magenta);
                case (ConsoleColor.Magenta):
                    return (ConsoleColor.Red);
                case (ConsoleColor.Red):
                    return (ConsoleColor.White);
                case (ConsoleColor.White):
                    return (ConsoleColor.Yellow);
                case (ConsoleColor.Yellow):
                    return (ConsoleColor.Black);
                default:
                    return (ConsoleColor.Black);
            }
        }

        public static ConsoleColor get_prev_color(ConsoleColor color)
        {
            switch (color)
            {
                case (ConsoleColor.Black):
                    return (ConsoleColor.Yellow);
                case (ConsoleColor.Blue):
                    return (ConsoleColor.Black);
                case (ConsoleColor.Cyan):
                    return (ConsoleColor.Blue);
                case (ConsoleColor.DarkBlue):
                    return (ConsoleColor.Cyan);
                case (ConsoleColor.DarkCyan):
                    return (ConsoleColor.DarkBlue);
                case (ConsoleColor.DarkGray):
                    return (ConsoleColor.DarkCyan);
                case (ConsoleColor.DarkGreen):
                    return (ConsoleColor.DarkGray);
                case (ConsoleColor.DarkMagenta):
                    return (ConsoleColor.DarkGreen);
                case (ConsoleColor.DarkRed):
                    return (ConsoleColor.DarkMagenta);
                case (ConsoleColor.DarkYellow):
                    return (ConsoleColor.DarkRed);
                case (ConsoleColor.Gray):
                    return (ConsoleColor.DarkYellow);
                case (ConsoleColor.Green):
                    return (ConsoleColor.Gray);
                case (ConsoleColor.Magenta):
                    return (ConsoleColor.Green);
                case (ConsoleColor.Red):
                    return (ConsoleColor.Magenta);
                case (ConsoleColor.White):
                    return (ConsoleColor.Red);
                case (ConsoleColor.Yellow):
                    return (ConsoleColor.White);
                default:
                    return (ConsoleColor.Black);
            }
        }

        //SECURITY

        private const int Keysize = 256;
        private const int DerivationIterations = 1000;

        public static string Encrypt(string plainText, string passPhrase)
        {
            try
            {
                // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
                // so that the same Salt and IV values can be used when decrypting.  
                var saltStringBytes = Generate256BitsOfRandomEntropy();
                var ivStringBytes = Generate256BitsOfRandomEntropy();
                var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
                {
                    var keyBytes = password.GetBytes(Keysize / 8);
                    using (var symmetricKey = new RijndaelManaged())
                    {
                        symmetricKey.BlockSize = 256;
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;
                        using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                                {
                                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                    cryptoStream.FlushFinalBlock();
                                    // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                    var cipherTextBytes = saltStringBytes;
                                    cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                    cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                    memoryStream.Close();
                                    cryptoStream.Close();
                                    return Convert.ToBase64String(cipherTextBytes);
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception)
            {
                return ("");
            }
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
            try
            {
                // Get the complete stream of bytes that represent:
                // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
                var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
                // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
                var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
                // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
                var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
                // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
                var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

                using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
                {
                    var keyBytes = password.GetBytes(Keysize / 8);
                    using (var symmetricKey = new RijndaelManaged())
                    {
                        symmetricKey.BlockSize = 256;
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;
                        using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                        {
                            using (var memoryStream = new MemoryStream(cipherTextBytes))
                            {
                                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                                {
                                    var plainTextBytes = new byte[cipherTextBytes.Length];
                                    var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                    memoryStream.Close();
                                    cryptoStream.Close();
                                    return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception)
            {
                return ("");
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        //DISPLAY

        public static void print_n_space(int n)
        {
            for (int i = 0; i < n; i++)
                Console.Write(" ");
        }

        //DIVERS

        #region APIs
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hwnd);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SetFocus(IntPtr hwnd);
        #endregion APIs //fonctions win32

        public static void ShowApp(string app_path)
        {
            IntPtr h = Process.GetCurrentProcess().MainWindowHandle;
            ShowWindow(h, 5);
            SetForegroundWindow(h);
            SetFocus(h);
            System.Diagnostics.Debug.WriteLine(h);
        }

        public static void bufferCheck(int amount = 300)
        {
            if (Console.CursorTop == Console.BufferHeight - 2)
            {
                if (Console.BufferHeight >= 32700)
                    Console.Clear();
                else
                    Console.SetBufferSize(Console.BufferWidth, Console.BufferHeight + amount);
            }
        }

        public static void displayReminders(string appdata_dir)
        {
            List<string> reminderContent = Library.getFileContent(appdata_dir + "/reminder/" + getDateFileFormat(DateTime.Now));
            bufferCheck();
            Console.SetCursorPosition(0, Console.CursorTop + 1);
            
            for (int i = 0; i < reminderContent.Count; i++)
            {
                Console.Write("> ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("reminder: " + reminderContent[i]);
                Console.ResetColor();
            }   
            deleteFile(appdata_dir + "/reminder/" + getDateFileFormat(DateTime.Now));
        }

        public static void erase_script_line(string script_path)
        {
            List<string> script_content = Library.getFileContent(script_path);

            if (script_content.Count <= 1)
                Library.deleteFile(script_path);
            else
            {
                script_content.RemoveAt(0);
                Library.saveFile(script_path, script_content);
            }
        }
    }
}
