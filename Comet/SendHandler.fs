#light
#if INTERACTIVE
#r "FSharp.PowerPack.Linq.dll"
#r "System.Core.dll"
#endif
namespace Comet.Chating
namespace Comet.Chating
 
open System
open System.Web
open System.Web.Security;
open System.Linq
open MongoDB
open MongoDB.Linq 
type SendHandler() =
    let mutable m_context = null
    let mutable m_endWork = null

    interface IHttpAsyncHandler with
        member h.IsReusable = false
        member h.ProcessRequest(context) = failwith "not supported"

        member h.BeginProcessRequest(c, cb, state) =
            m_context <- c 
            let from = m_context.Request.Form.Item("from")
            let room = m_context.Request.Form.Item("to")
            let msg = m_context.Request.Form.Item("msg")          
            let key = m_context.Request.Form.Item("key").ToLower() 
            let  k= System.Configuration.ConfigurationSettings.get_AppSettings().Get("chatkey")+from
            let admink= System.Configuration.ConfigurationSettings.get_AppSettings().Get("chatkey")+"admin"
            let pwd=FormsAuthentication.HashPasswordForStoringInConfigFile(k, "MD5").ToLower()
            let adminpwd=FormsAuthentication.HashPasswordForStoringInConfigFile(admink, "MD5").ToLower()
            if not(pwd.Equals(key) || adminpwd.Equals(key)) then  
                let beginWork, e, _ = Async.AsBeginEnd (fun()->async{m_context.Response.End()} )
                beginWork ((),cb, state)
            else
                let mongo = new Mongo()
                let connected = mongo.Connect()
                let db = mongo.["chatDB"]
                let timeNow = System.DateTime.Now.ToString()
                let doc= new MongoDB.Document() 
                doc.["msg"] <- msg 
                doc.["from"] <- from 
                doc.["addtime"] <-   timeNow  
                db.[room].Insert(doc) 
                let connected= mongo.Disconnect()  
                
                if from<>"admin" && adminpwd.Equals(key) then 
                    let sendall = Chat.sendallwithoutadmin from  msg room
                    let beginWork, e, _ = Async.AsBeginEnd (fun()->sendall )
                    m_endWork <- new Func<_, _>(e)
                    beginWork ((),cb, state)
                else               
                    let sendall = Chat.sendall from  msg room
                    let beginWork, e, _ = Async.AsBeginEnd (fun()->sendall )
                    m_endWork <- new Func<_, _>(e)
                    beginWork ((),cb, state)
 
        member h.EndProcessRequest(ar) = 

            let result = m_endWork.Invoke ar
  
            m_context.Response.Write "sent" 
 
