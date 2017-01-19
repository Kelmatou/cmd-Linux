using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace cmd_Linux
{
    public class NotificationManager
    {
        private List<string> contentType = new List<string>();//type of notification (execution, print...)
        private List<string> content = new List<string>();//content of notificiation
        private List<DateTime> date = new List<DateTime>();//date of notification trigger

        public List<string> getContent()
        {
            return (content);
        }

        public List<string> getContentType()
        {
            return (contentType);
        }

        public List<DateTime> getDate()
        {
            return (date);
        }

        public NotificationManager(string sourceFile)
        {
            List<string> fileContent = new List<string>();
            if(File.Exists(sourceFile))
            {
                fileContent = Library.getFileContent(sourceFile);
                for (int i = 0; i < fileContent.Count; i++)
                    parseNotification(fileContent[i]);
            }
        }

        private void parseNotification(string notification)
        {
            int separator1 = Library.firstOccurence(notification, "@");
            int separator2 = separator1 != -1 ? Library.firstOccurence(notification, "@", separator1 + 1) : -1;
            string date = "";
            string command = "";
            string content = "";
            DateTime notificationDate = new DateTime(1970, 1, 1);
            
            if(separator1 != -1 && separator2 != -1 && separator2 < notification.Length - 1)
            {
                date = notification.Substring(0, separator1);
                command = notification.Substring(separator1 + 1, separator2 - separator1 - 1);
                content = notification.Substring(separator2 + 1);

                notificationDate = Library.getDateFromFileName(date);

                if(notificationDate != new DateTime(1970, 1, 1) && isValidContentType(command))
                {
                    contentType.Add(command);
                    this.content.Add(content);
                    this.date.Add(notificationDate);
                }
            }
        }

        private bool isValidContentType(string contentType)
        {
            return (contentType == "PRINT" || contentType == "EXE" || contentType == "NOTIF" || contentType == "COLOR");
        }

        private List<string> getNotificationFile()
        {
            List<string> fileContent = new List<string>();
            for (int i = 0; i < this.contentType.Count; i++)
                fileContent.Add(Library.getDateFileFormat(this.date[i]) + "@" + this.contentType[i] + "@" + this.content[i]);
            return (fileContent);
        }

        public bool isNotificationTrigger()
        {
            for(int i = 0; i < date.Count; i++)
            {
                if (date[i] == DateTime.Now)
                    return (true);
            }
            return (false);
        }

        public void triggerNotifications(DateTime dateNotification, bool allowBeforeDate = false)
        {
            int i = 0;
            bool modification = false;
            while(i < date.Count)
            {
                if(date[i] == dateNotification || (allowBeforeDate && Library.date2IsBeforeDate1(dateNotification, date[i])))
                {
                    if (contentType[i] == "PRINT")
                    {
                        ConsoleColor currentColor = Console.ForegroundColor;
                        Console.ResetColor();
                        Console.Write("\n> ");
                        Console.ForegroundColor = currentColor;
                        Console.WriteLine(content[i]);
                        Program.print_cmd_intro_line();
                    }
                    else if (contentType[i] == "EXE")
                    {
                        Console.WriteLine();
                        Execution.execute_input(Interpreter.parse_input(content[i], ShellEnvironment.appdata_dir));
                        Program.print_cmd_intro_line();
                    }
                    else if (contentType[i] == "COLOR")
                        changeColor(content[i]);
                    else if (contentType[i] == "NOTIF")
                        Library.ShowApp(Assembly.GetExecutingAssembly().Location);
                    this.contentType.RemoveAt(i);
                    this.content.RemoveAt(i);
                    this.date.RemoveAt(i);
                    modification = true;
                    i--;
                }
                i++;
            }
            if (modification)
                Library.saveFile(ShellEnvironment.appdata_dir + "/notification", getNotificationFile());
        }

        public bool addNotification(string appdata_dir, DateTime date, string contentType, string content)
        {
            this.contentType.Add(contentType);
            this.content.Add(content);
            this.date.Add(date);
            return (Library.saveFile(appdata_dir + "/notification", getNotificationFile()));
        }

        public bool removeNotification(string appdata_dir, DateTime date, string contentType, string content)
        {
            int i = 0;
            while (i < this.contentType.Count && this.contentType[i] != contentType && this.content[i] != content && this.date[i] != date)
                i++;
            if(i < this.contentType.Count)
            {
                this.contentType.RemoveAt(i);
                this.content.RemoveAt(i);
                this.date.RemoveAt(i);
                return (Library.saveFile(appdata_dir + "/notification", getNotificationFile()));
            }
            return (false);
        }

        public bool removeNotification(string appdata_dir, int index)
        {
            if (index >= 0 && index < this.contentType.Count)
            {
                this.contentType.RemoveAt(index);
                this.content.RemoveAt(index);
                this.date.RemoveAt(index);
                return (Library.saveFile(appdata_dir + "/notification", getNotificationFile()));
            }
            return (false);
        }

        public int searchNotification(DateTime date, string contentType, string content, bool exactYear, bool exactMonth, bool exactDay, bool exactHour, bool exactMin, bool exactSec, int begin = 0)
        {
            int i = begin;

            while (i < this.contentType.Count && (this.contentType[i] != contentType || !Library.isSameDate(this.date[i],date, exactYear, exactMonth, exactDay, exactHour, exactMin, exactSec) || this.content[i] != content))
                i++;

            return (i < this.contentType.Count ? i : -1);
        }

        public int searchNotification(DateTime date, string contentType, bool exactYear, bool exactMonth, bool exactDay, bool exactHour, bool exactMin, bool exactSec, int begin = 0)
        {
            int i = begin;

            while (i < this.contentType.Count && (this.contentType[i] != contentType || !Library.isSameDate(this.date[i], date, exactYear, exactMonth, exactDay, exactHour, exactMin, exactSec)))
                i++;

            return (i < this.contentType.Count ? i : -1);
        }
        
        private void changeColor(string color)
        {
            switch (color)
            {
                case ("BLACK"):
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
                case ("BLUE"):
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case ("CYAN"):
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case ("DARKBLUE"):
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    break;
                case ("DARKCYAN"):
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;
                case ("DARKGRAY"):
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case ("DARKGREEN"):
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case ("DARKMAGENTA"):
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    break;
                case ("DARKRED"):
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case ("DARKYELLOW"):
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case ("GRAY"):
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case ("GREEN"):
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case ("MAGENTA"):
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case ("RED"):
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case ("WHITE"):
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case ("YELLOW"):
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                default:
                    Console.ResetColor();
                    break;
            }
        }

        /*
         TEMPLATE OF NOTIFICATION FILE:
         * 2016_09_06_15_55@PRINT@this is a test
         * 2016_09_06_15_55@EXE@echo test
         */
    }
}
