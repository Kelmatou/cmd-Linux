using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd_Linux
{
    public static class Web
    {
        static public string convert_key_to_url_search_format(string key)
        {
            string url_format = "";

            for (int i = 0; i < key.Length; i++)
            {
                if (key[i] == ' ')
                {
                    url_format = url_format + '+';
                }
                else if (key[i] == '+')
                {
                    url_format = url_format + "%2B";
                }
                else if (key[i] == '&')
                {
                    url_format = url_format + "%26";
                }
                else if (key[i] == '=')
                {
                    url_format = url_format + "%3D";
                }
                else if (key[i] == '#')
                {
                    url_format = url_format + "%23";
                }
                else
                {
                    url_format = url_format + key[i];
                }
            }

            return (url_format);
        }

        static public string web_language_code_google_trad(string language)
        {
            switch(language)
            {
                case ("afrikaans"):
                    return ("af");
                case ("albanian"):
                    return ("sq");
                case ("arabic"):
                    return ("ar");
                case ("armenian"):
                    return ("hy");
                case ("azeri"):
                    return ("az");
                case ("basque"):
                    return ("eu");
                case ("bengali"):
                    return ("bn");
                case ("belarusian"):
                    return ("be");
                case ("burmese"):
                    return ("my");
                case ("bosnian"):
                    return ("bs");
                case ("bulgarian"):
                    return ("bg");
                case ("catalan"):
                    return ("ca");
                case ("cebuano"):
                    return ("ceb");
                case ("chichewa"):
                    return ("ny");
                case ("chinese"):
                    return ("zh-CN");
                case ("singhalese"):
                    return ("si");
                case("corean"):
                    return ("ko");
                case ("creole"):
                    return ("ht");
                case ("croatian"):
                    return ("hr");
                case ("danish"):
                    return ("da");
                case ("esperanto"):
                    return ("eo");
                case ("estonian"):
                    return ("et");
                case ("english"):
                    return ("en");
                case ("finnish"):
                    return ("fi");
                case ("french"):
                    return ("fr");
                case ("galician"):
                    return ("gl");
                case ("welsh"):
                    return ("cy");
                case ("georgian"):
                    return ("ka");
                case ("german"):
                    return ("de");
                case ("japanese"):
                    return ("ja");
                case ("italian"):
                    return ("it");
                case ("greek"):
                    return ("el");
                case ("gujarati"):
                    return ("gu");
                case ("hausa"):
                    return ("ha");
                case ("hebrew"):
                    return ("iw");
                case ("indi"):
                    return ("hi");
                case ("hmong"):
                    return ("hmn");
                case ("hungarian"):
                    return ("hu");
                case ("igbo"):
                    return ("ig");
                case ("indonesian"):
                    return ("id");
                case ("irish"):
                    return ("ga");
                case ("icelandic"):
                    return ("is");
                case ("javanese"):
                    return ("jw");
                case ("kannada"):
                    return ("kn");
                case ("kazakh"):
                    return ("kk");
                case ("khmer"):
                    return ("km");
                case ("laos"):
                    return ("lo");
                case ("latin"):
                    return ("la");
                case ("latvian"):
                    return ("lv");
                case ("lithuanian"):
                    return ("lt");
                case ("macedonian"):
                    return ("mk");
                case ("malaysian"):
                    return ("ms");
                case ("malayalam"):
                    return ("ml");
                case ("malagasy"):
                    return ("mg");
                case ("maltese"):
                    return ("mt");
                case ("maori"):
                    return ("mi");
                case ("marathi"):
                    return ("mr");
                case ("mongolian"):
                    return ("mn");
                case ("dutch"):
                    return ("nl");
                case ("nepalese"):
                    return ("ne");
                case ("norwegian"):
                    return ("no");
                case ("uzbek"):
                    return ("uz");
                case ("punjabi"):
                    return ("pa");
                case ("persian"):
                    return ("fa");
                case ("romanian"):
                    return ("ro");
                case ("serbian"):
                    return ("sr");
                case ("sesotho"):
                    return ("st");
                case ("slovak"):
                    return ("sk");
                case ("slovenian"):
                    return ("sl");
                case ("somali"):
                    return ("so");
                case ("sundanese"):
                    return ("su");
                case ("swahili"):
                    return ("sw");
                case ("tajik"):
                    return ("tg");
                case ("tagalog"):
                    return ("tl");
                case ("tamil"):
                    return ("ta");
                case ("czech"):
                    return ("cs");
                case ("telugu"):
                    return ("te");
                case ("thai"):
                    return ("th");
                case ("ukrainian"):
                    return ("uk");
                case ("urdu"):
                    return ("ur");
                case ("vietnamese"):
                    return ("vi");
                case ("yiddish"):
                    return ("yi");
                case ("yoruba"):
                    return ("yo");
                case ("zulu"):
                    return ("zu");
                case ("polish"):
                    return ("pl");
                case ("portuguese"):
                    return ("pt");
                case ("russian"):
                    return ("ru");
                case ("spanish"):
                    return ("es");
                case ("swedish"):
                    return ("sv");
                case ("turkish"):
                    return ("tr");
                default:
                    return ("");
            }
        }

        static public string web_language_code(string language)
        {
            switch (language)
            {
                case ("afrikaans"):
                    return ("af");
                case ("albanian"):
                    return ("sq");
                case ("arabic"):
                    return ("ar");
                case ("armenian"):
                    return ("hy");
                case ("azeri"):
                    return ("az");
                case ("basque"):
                    return ("eu");
                case ("bengali"):
                    return ("bn");
                case ("belarusian"):
                    return ("be");
                case ("burmese"):
                    return ("my");
                case ("bosnian"):
                    return ("bs");
                case ("bulgarian"):
                    return ("bg");
                case ("catalan"):
                    return ("ca");
                case ("cebuano"):
                    return ("ceb");
                case ("chichewa"):
                    return ("ny");
                case ("chinese"):
                    return ("zh");
                case ("singhalese"):
                    return ("si");
                case ("corean"):
                    return ("ko");
                case ("creole"):
                    return ("ht");
                case ("croatian"):
                    return ("hr");
                case ("danish"):
                    return ("da");
                case ("esperanto"):
                    return ("eo");
                case ("estonian"):
                    return ("et");
                case ("english"):
                    return ("en");
                case ("finnish"):
                    return ("fi");
                case ("french"):
                    return ("fr");
                case ("galician"):
                    return ("gl");
                case ("welsh"):
                    return ("cy");
                case ("georgian"):
                    return ("ka");
                case ("german"):
                    return ("de");
                case ("japanese"):
                    return ("ja");
                case ("italian"):
                    return ("it");
                case ("greek"):
                    return ("el");
                case ("gujarati"):
                    return ("gu");
                case ("hausa"):
                    return ("ha");
                case ("hebrew"):
                    return ("iw");
                case ("indi"):
                    return ("hi");
                case ("hmong"):
                    return ("hmn");
                case ("hungarian"):
                    return ("hu");
                case ("igbo"):
                    return ("ig");
                case ("indonesian"):
                    return ("id");
                case ("irish"):
                    return ("ga");
                case ("icelandic"):
                    return ("is");
                case ("javanese"):
                    return ("jw");
                case ("kannada"):
                    return ("kn");
                case ("kazakh"):
                    return ("kk");
                case ("khmer"):
                    return ("km");
                case ("laos"):
                    return ("lo");
                case ("latin"):
                    return ("la");
                case ("latvian"):
                    return ("lv");
                case ("lithuanian"):
                    return ("lt");
                case ("macedonian"):
                    return ("mk");
                case ("malaysian"):
                    return ("ms");
                case ("malayalam"):
                    return ("ml");
                case ("malagasy"):
                    return ("mg");
                case ("maltese"):
                    return ("mt");
                case ("maori"):
                    return ("mi");
                case ("marathi"):
                    return ("mr");
                case ("mongolian"):
                    return ("mn");
                case ("dutch"):
                    return ("nl");
                case ("nepalese"):
                    return ("ne");
                case ("norwegian"):
                    return ("no");
                case ("uzbek"):
                    return ("uz");
                case ("punjabi"):
                    return ("pa");
                case ("persian"):
                    return ("fa");
                case ("romanian"):
                    return ("ro");
                case ("serbian"):
                    return ("sr");
                case ("sesotho"):
                    return ("st");
                case ("slovak"):
                    return ("sk");
                case ("slovenian"):
                    return ("sl");
                case ("somali"):
                    return ("so");
                case ("sundanese"):
                    return ("su");
                case ("swahili"):
                    return ("sw");
                case ("tajik"):
                    return ("tg");
                case ("tagalog"):
                    return ("tl");
                case ("tamil"):
                    return ("ta");
                case ("czech"):
                    return ("cs");
                case ("telugu"):
                    return ("te");
                case ("thai"):
                    return ("th");
                case ("ukrainian"):
                    return ("uk");
                case ("urdu"):
                    return ("ur");
                case ("vietnamese"):
                    return ("vi");
                case ("yiddish"):
                    return ("yi");
                case ("yoruba"):
                    return ("yo");
                case ("zulu"):
                    return ("zu");
                case ("polish"):
                    return ("pl");
                case ("portuguese"):
                    return ("pt");
                case ("russian"):
                    return ("ru");
                case ("spanish"):
                    return ("es");
                case ("swedish"):
                    return ("sv");
                case ("turkish"):
                    return ("tr");
                default:
                    return ("");
            }
        }
    }
}
