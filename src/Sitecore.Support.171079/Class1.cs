using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Support.Shell.Framework.Pipelines
{
    public class NewFolderItem:Sitecore.Shell.Framework.Pipelines.NewFolderItem
    {
        public new void Execute(ClientPipelineArgs args)
        {
            if (args.HasResult)
            {
                Database database = Factory.GetDatabase(args.Parameters["database"]);
                Assert.IsNotNull(database, "database");
                string str = args.Parameters["id"];
                string str2 = StringUtil.GetString(new string[] { args.Parameters["master"] });

                //fix for #171079
                ID itemId = new ID(str);
                Item item = database.GetItem(itemId, LanguageManager.GetLanguage(args.Parameters["language"]));
                if (item != null)
                {
                    TemplateItem item2 = (str2.Length > 0) ? database.Templates[str2] : database.Templates[TemplateIDs.Folder];
                    if (item2 != null)
                    {
                        Log.Audit(this, "Create folder: {0}", new string[] { AuditFormatter.FormatItem(item) });
                        item2.AddTo(item, args.Result);
                    }
                    else
                    {
                        Context.ClientPage.ClientResponse.Alert(Translate.Text("Neither {0} master or {1} template was found.", new object[] { str2, str2 }));
                        args.AbortPipeline();
                    }
                }
                else
                {
                    Context.ClientPage.ClientResponse.Alert("Item not found.");
                    args.AbortPipeline();
                }
            }
        }
    }
}