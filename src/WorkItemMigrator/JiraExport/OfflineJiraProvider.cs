using Atlassian.Jira;
using Migration.Common.Log;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace JiraExport
{
    public class OfflineJiraProvider : IJiraProvider
    {
        private DirectoryInfo backupDirectory { get; set; }

        private XDocument entities { get; set; }

        private XDocument activeObjects { get; set; }

        public JiraSettings Settings { get; set; }

        public async Task<List<RevisionAction<JiraAttachment>>> DownloadAttachments(JiraRevision rev)
        {
            //TODO:attachments
            return new List<RevisionAction<JiraAttachment>>();
        }

        public IEnumerable<IJiraItem> EnumerateIssues(string jql, HashSet<string> skipList, JiraProvider.DownloadOptions downloadOptions)
        {
            //screw jql for now
            foreach (XElement elem in entities.Descendants(XName.Get("Issue")))
            {
                var jiraItem = new OfflineJiraItem()
                {
                    Key = elem.Attribute(XName.Get("projectKey")).Value + "-" + elem.Attribute(XName.Get("number")).Value,
                    EpicParent = "TODO:link",
                    Parent = "TODO:link",
                    Type = elem.Attribute(XName.Get("type")).Value
                    //RemoteIssue = "TODO:link",
                };
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime time = DateTime.Now;

                //strip .fff from "yyyy-MM-dd HH:mm:ss.fff"
                string timeStr = elem.Attribute(XName.Get("created")).Value;
                if (timeStr.Length > 19)
                {
                    timeStr = timeStr.Substring(0, 19);
                }
                if (DateTime.TryParseExact(timeStr, "yyyy-MM-dd HH:mm:ss", provider, DateTimeStyles.None, out DateTime outTime))
                {
                    time = outTime;
                }
                else
                {
                    Logger.Log(LogLevel.Warning, $"Could not parse Time for {jiraItem.Key}.");
                }

                Dictionary<string, object> fields = elem.Attributes().ToList().ToDictionary(x => x.Name.LocalName, y => (object)y.Value);
                if (elem.HasElements)
                {
                    XElement descriptionElem = elem.Descendants(XName.Get("description")).FirstOrDefault();
                    if (descriptionElem != null)
                    {
                        fields.Add("description$Rendered", descriptionElem.Value);
                    }
                }
                jiraItem.Revisions.Add(new JiraRevision(jiraItem)
                {
                    Time = time,
                    Author = elem.Attribute(XName.Get("creator")).Value,// reporter,
                    AttachmentActions = new List<RevisionAction<JiraAttachment>>(),
                    Fields = fields,
                    LinkActions = null
                });
                yield return jiraItem;
            }
        }

        public string GetCustomId(string propertyName)
        {
            return "TODO:doesnt matter?";
        }

        public int GetItemCount(string jql)
        {
            //screw jql, just full for now
            return entities.Descendants(XName.Get("Issue")).Count();
        }

        public JiraProvider.JiraVersion GetJiraVersion()
        {
            return new JiraProvider.JiraVersion("Todo:Implement", "careface");
        }

        public string GetUserEmail(string usernameOrAccountId)
        {
            return usernameOrAccountId;//throw new NotImplementedException();
        }

        public static OfflineJiraProvider Initialize(string backupLocation, JiraSettings settings)
        {
            if (!Directory.Exists(backupLocation))
            {
                throw new ArgumentException("backupLocation not correct");
            }
            OfflineJiraProvider provider = new OfflineJiraProvider();
            provider.backupDirectory = new DirectoryInfo(backupLocation);
            provider.Settings = settings;
            Logger.Log(LogLevel.Info, "Loading backup files into ram");
            FileInfo[] files = provider.backupDirectory.GetFiles();
            foreach (FileInfo file in files)
            {
                if (file.Name == "activeobjects.xml")
                {
                    provider.activeObjects = XDocument.Load(file.FullName);
                }
                else if (file.Name == "entities.xml")
                {
                    provider.entities = XDocument.Load(file.FullName);
                }
            }
            return provider;
        }
    }
}