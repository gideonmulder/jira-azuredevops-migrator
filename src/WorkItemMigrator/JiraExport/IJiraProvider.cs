using Atlassian.Jira;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

using System.Collections.Generic;

using System.Threading.Tasks;

namespace JiraExport
{
    public interface IJiraProvider
    {
        JiraSettings Settings { get; }

        Task<List<RevisionAction<JiraAttachment>>> DownloadAttachments(JiraRevision rev);

        IEnumerable<IJiraItem> EnumerateIssues(string jql, HashSet<string> skipList, JiraProvider.DownloadOptions downloadOptions);

        string GetCustomId(string propertyName);

        int GetItemCount(string jql);

        JiraProvider.JiraVersion GetJiraVersion();

        string GetUserEmail(string usernameOrAccountId);
    }
}