using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace RemoteUrl
{
    public class WebPage
    {
        public string Url { get; set; }
        public string Source { get; set; }
        public string UpdatedSource { get; set; }

        public WebPage() { }

        public string GetSource()
        {
            return GetSource(this.Url);
        }

        public string GetSource(string url)
        {
            String source;
            WebResponse objResponse;
            WebRequest objRequest = HttpWebRequest.Create(url);
            objResponse = objRequest.GetResponse();
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                source = sr.ReadToEnd();
                sr.Close();
            }

            // set object source
            this.Source = source;

            // return source
            return source;
        }

        public string UpdateSource()
        {
            return UpdateSource(this.Source, this.Url);
        }

        public string UpdateSource(string sourceInput, string url)
        {
            string output = sourceInput;

            // HTTP:// ?
            bool isHttpThere = url.IndexOf("http://") > -1;
            if (!isHttpThere)
            {
                url = "http://" + url;
            }

            var remoteUri = new Uri(url);

            // update remote html
            string siteRoot = "http://" + remoteUri.Authority + "/";
            string lastUrlSegment = remoteUri.Segments.Last();
            string directoryName = System.IO.Path.GetDirectoryName(remoteUri.AbsolutePath);
            string siteDirectoryPath = !String.IsNullOrEmpty(directoryName) ? (directoryName.Replace("\\", "/") + "/").TrimStart('/') : "";

            // replace all absolute paths
            output = output.
                Replace("href=\"../", "href=\"/").
                Replace("src=\"../", "src=\"/").
                Replace("src=\"//", "src=\"http://").
                Replace("href=\"//", "href=\"http://").
                Replace("href=\"/", "href=\"" + siteRoot).
                Replace("value=\"../", "value=\"" + siteRoot).
                Replace("value=\"/", "value=\"" + siteRoot).
                Replace("'movie','../", "'movie','" + siteRoot).
                Replace("'movie','/", "'movie','" + siteRoot).
                Replace("src=\"/", "src=\"" + siteRoot).
                Replace("'/", "'" + siteRoot);

            Regex zoneSrc_regex = new Regex("src=(\"|')((.|\n)*?)(\"|')", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            MatchCollection zoneSrc_matches = zoneSrc_regex.Matches(output);

            foreach (Match zoneMatch in zoneSrc_matches)
            {
                if ((zoneMatch.Value.IndexOf("http://") < 0) && (zoneMatch.Value.IndexOf("https://") < 0))
                {
                    // get updated code
                    string zoneUpdate = zoneMatch.Value.
                        Replace("src=\"", "src=\"" + siteRoot).
                        Replace("src='", "src='" + siteRoot).
                        Replace("src=\"/", "src=\"" + siteRoot).
                        Replace("src='/", "src='" + siteRoot);

                    // update layoutInput
                    output = output.Replace(zoneMatch.Value, zoneUpdate);
                }
            }

            Regex zoneHref_regex = new Regex("href=(\"|')((.|\n)*?)(\"|')", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            MatchCollection zoneHref_matches = zoneHref_regex.Matches(output);

            foreach (Match zoneMatch in zoneHref_matches)
            {
                if ((zoneMatch.Value.IndexOf("http://") < 0) && (zoneMatch.Value.IndexOf("https://") < 0))
                {
                    // get updated code
                    string zoneUpdate = zoneMatch.Value.
                        Replace("href=\"", "href=\"" + siteRoot).
                        Replace("href='", "href='" + siteRoot).
                        Replace("href=\"/", "href=\"" + siteRoot).
                        Replace("href='/", "href='" + siteRoot);

                    // update layoutInput
                    output = output.Replace(zoneMatch.Value, zoneUpdate);
                }
            }

            // Flash Object Embed (non IE)
            Regex zoneFlashObjectEmbed_regex = new Regex(@"<object ((.|\n)*?)>", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            MatchCollection zoneFlashObjectEmbed_matches = zoneFlashObjectEmbed_regex.Matches(output);

            foreach (Match zoneMatch in zoneFlashObjectEmbed_matches)
            {
                // get updated code
                string zoneUpdate = zoneMatch.Value.
                    Replace("data=\"../", "data=\"/").
                    Replace("value=\"../", "value=\"/").
                    Replace("movie\" value=\"", "param name=\"movie\" value=\"" + siteRoot).
                    Replace("data=\"", "data=\"" + url);

                // update layoutInput
                output = output.Replace(zoneMatch.Value, zoneUpdate);
            }

            // Flash Object Embed (IE)
            Regex zoneFlashObjectEmbed_IE_regex = new Regex(@"<param ((.|\n)*?)>", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            MatchCollection zoneFlashObjectEmbed_IE_matches = zoneFlashObjectEmbed_IE_regex.Matches(output);

            foreach (Match zoneMatch in zoneFlashObjectEmbed_IE_matches)
            {
                // get updated code
                string nestedZoneUpdate = zoneMatch.Value.
                    Replace("name=\"movie\" value=\"", "name=\"movie\" value=\"" + siteRoot); ;

                // update layoutInput
                output = output.Replace(zoneMatch.Value, nestedZoneUpdate);
            }

            // All background tag properties
            Regex zoneAllTags_regex = new Regex("background=\"((.|\n)*?)\"", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            MatchCollection zoneAllTags_matches = zoneAllTags_regex.Matches(output);

            foreach (Match zoneMatch in zoneAllTags_matches)
            {
                if ((zoneMatch.Value.IndexOf("http://") < 0) && (zoneMatch.Value.IndexOf("https://") < 0))
                {
                    // get updated code
                    string zoneUpdate = zoneMatch.Value.
                        Replace("background=\"", "background=\"" + siteRoot).
                        Replace("background=\"/", "background=\"" + siteRoot).
                        Replace("background='/", "background='" + siteRoot);

                    // update layoutInput
                    output = output.Replace(zoneMatch.Value, zoneUpdate);
                }
            }

            // All background images
            Regex zoneCssBgImgTags_regex = new Regex(@"url\(((.|\n)*?)\)", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            MatchCollection zoneCssBgImgTags_matches = zoneCssBgImgTags_regex.Matches(output);

            foreach (Match zoneMatch in zoneCssBgImgTags_matches)
            {
                if ((zoneMatch.Value.IndexOf("http://") < 0) && (zoneMatch.Value.IndexOf("https://") < 0))
                {
                    // get updated code
                    string zoneUpdate = zoneMatch.Value.
                        Replace("url(", "url(" + siteRoot);

                    // update layoutInput
                    output = output.Replace(zoneMatch.Value, zoneUpdate);
                }
            }

            // set object value
            this.UpdatedSource = output;

            // return value
            return output;
        }
    }
}
