using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace easy_indeed
{
    class Program
    {
        static string url = "https://www.indeed.com/jobs?q=software+engineer&l=Bellevue,+WA&explvl=entry_level&start=";
        static int pageStart = 0;
        static List<string> keyWords = new List<string>();
        static List<string> links = new List<string>();
        static List<string> lines = new List<string>();
        static HashSet<string> linksToRemember = new HashSet<string>();

        static string nextPage = "https://www.indeed.com/viewjob";

        static void Main(string[] args)
        {
            // keywords that I want to search for
            keyWords.Add("New");
            keyWords.Add("Graduate");
            keyWords.Add("University");
            keyWords.Add("Grad");
            keyWords.Add("Entry");
            keyWords.Add("Junior");
            keyWords.Add("Associate");
            keyWords.Add("Engineer I");

            getPage(1400);

            sendToFile(linksToRemember);
        }

        static void getPage(int pages)
        {
            // loop for all the pages for software engineer
            for (; pageStart <= pages; pageStart += 10)
            {
                Console.WriteLine(pageStart);
                try
                {
                    WebRequest wr = WebRequest.Create(url + pageStart.ToString());
                    WebResponse response = wr.GetResponse();
                    Stream data = response.GetResponseStream();
                    string stuff = String.Empty;

                    StreamReader sr = new StreamReader(data);

                    while ((stuff = sr.ReadLine()) != null)
                    {
                        lines.Add(stuff);
                    }
                }
                catch (WebException we) { };

                checkString(lines);
                
            }
        }

        // look for a specific div tag in the html
        static void checkString(List<string> strings)
        {
            for (int i = 0; i < strings.Count; i++)
            {
                if (strings[i].Contains("<div class=\"title\">"))
                {
                    // if we find what we want we add the second half of the url to a list so we can write it to a file and visit it later
                    if(catStrings(strings, i))
                    {
                        string temp = strings[i + 4].Substring(29);
                        temp = temp.Replace("\"", "");
                        if (temp[0] == '?')
                        {
                            linksToRemember.Add(temp);
                        }
                    }
                    
                }
            }
        }

        // concat a few strings to make it easier to parse
        static bool catStrings(List<string> strings, int index)
        {
            string newString = "";
            int end = index + 13;

            for(; index < end; index++)
            {
                newString += strings[index];
            }


            foreach (string s in keyWords)
            {
                // check if the title of the job matches what I want
                if (newString.Contains("Software") &&
                    newString.Contains(s))
                {
                    return true;
                }
            }

            return false;
        }

        static void sendToFile(HashSet<string> hs)
        {
            StreamWriter sw = new StreamWriter("Links.txt");

            foreach(string s in hs)
            {
                sw.WriteLine(nextPage + s);
                sw.Flush();
            }
        }

        /*<div class="title">
        <a
                target="_blank"
                id="jl_2c5444eb53f739e6"
                href="/company/SR-Education-Group/jobs/Entry-Level-Software-Engineer-2c5444eb53f739e6?fccid=3356393c9d83d46c&vjs=3"
                onmousedown="return rclk(this,jobmap[8],1);"
                onclick=" setRefineByCookie([&#039;explvl&#039;]); return rclk(this,jobmap[8],true,1);"
                rel="noopener nofollow"
                title="Entry Level Software Engineer"
                class="jobtitle turnstileLink "
                data-tn-element="jobTitle"
        >
            Entry Level <b>Software</b> <b>Engineer</b></a>

        </div>*/
    }
}
