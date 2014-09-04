using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB;
using MongoDB.Linq;
namespace CometServer
{
    public class message
    {
        public string addtime { get; set; }
        public string from { get; set; }
        public string msg { get; set; }
        public Oid _id { get; set; }
    }
    /// <summary>
    /// Summary description for GetData
    /// </summary>
    /// 
    public class GetData : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            Mongo mongo = new Mongo();
            try
            {
                string room = "chats";
                if (context.Request.QueryString["room"] != null && context.Request.QueryString["room"].Length > 0)
                    room = context.Request.QueryString["room"].ToString();
                string action = "list";
                if (context.Request.QueryString["action"] != null && context.Request.QueryString["action"].Length > 0)
                    action = context.Request.QueryString["action"].ToString();
                string msg = "";
                if (context.Request.QueryString["msg"] != null && context.Request.QueryString["msg"].Length > 0)
                    msg = context.Request.QueryString["msg"].ToString();
                string from = "";
                if (context.Request.QueryString["from"] != null && context.Request.QueryString["from"].Length > 0)
                    from = context.Request.QueryString["from"].ToString();
                mongo.Connect();

                var db = mongo.GetDatabase("chatDB").GetCollection<message>(room);
                switch (action)
                {
                    case "rooms":
                        foreach (var r in mongo.GetDatabase("chatDB").GetCollectionNames())
	                    {
                             string r2 = r.Replace("chatDB.", "");
                            if (r.Contains("system.indexes") ||r.Contains(".$_id_"))
                            {
                                continue;
                            } 
                           context.Response.Write(" <a href=\"?room=" + r2 + "&action=list\" target=\"_blank\">" + r2 + "</a><br/> ");
	                    }
                        break;
                    case "list":
                        var col = db.Linq().OrderByDescending(doc => doc._id);
                        if (col.Count() > 0)
                            context.Response.Write("<br/><a href=\"?action=clear&room=" + room + "\" target=\"_blank\">删除全部</a><br/><br/> ");
                        else
                            context.Response.Write("无数据");

                        foreach (var item in col)
                        {
                            context.Response.Write(item.addtime + " [" + item.from + "] 说: " + item.msg + " <a id=\""+item._id+"\" href=\"?room=" + room + "&action=remove&msg=" + item.msg + "&from=" + item.from + "\" target=\"_blank\">删除</a><br/> ");
                        }

                        break;
                    case "remove":
                        if (from.Length > 0 && msg.Length > 0)
                        {
                            db.Remove(new Document { { "from", from }, { "msg", msg } });

                        }
                        else if (from.Length > 0)
                        {
                            db.Remove(new Document { { "from", from } });

                        }
                        else if (msg.Length > 0)
                        {
                            db.Remove(new Document { { "msg", msg } });
                        }
                        break;
                    case "clear":
                        db.Remove(new Document { });
                        break;
                    case "dropdatabase":
                        mongo.GetDatabase("chatDB").SendCommand("dropDatabase");
                        break;
                    default:
                        break;
                }


            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

                mongo.Disconnect();
            }


        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}

public static class OidExtensions
{
    public static Oid ToOid(this string str)
    {
        if (str.Length == 24)
            return new Oid(str);

        return new Oid(str.Replace("\"", ""));
    }
}