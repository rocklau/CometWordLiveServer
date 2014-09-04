#light

namespace Comet.Chating 
 
open System
open System.Collections.Generic
open System.Web
open System.Web.Security;
open System.Web.Script.Serialization
type ReceiveHandler() =

    let mutable m_context = null
    let mutable m_endWork = null

    interface IHttpAsyncHandler with
        member h.IsReusable = false
        member h.ProcessRequest(context) = failwith "not supported"

        member h.BeginProcessRequest(c, cb, state) =
            m_context <- c

            let name = c.Request.QueryString.Item("name") 
            let room = c.Request.QueryString.Item("room")  
            let key = m_context.Request.QueryString.Item("key").ToLower() 
            let  k= System.Configuration.ConfigurationSettings.get_AppSettings().Get("chatkey")+name
            let pwd=FormsAuthentication.HashPasswordForStoringInConfigFile(k, "MD5").ToLower()
            if not(pwd.Equals(key)) then  
                let beginWork, e, _ = Async.AsBeginEnd (fun()->async{m_context.Response.End()} )
                beginWork ((),cb, state)
            else                        
                let receive = Chat.receive name room
                let beginWork, e, _ = Async.AsBeginEnd (fun()->receive )
                m_endWork <- new Func<_, _>(e)

                beginWork ((),cb, state)

        member h.EndProcessRequest(ar) =
            let convert (m: Chat.ChatMsg) =
                let o = new Dictionary<_, _>();
                o.Add("from", m.From)
                o.Add("text", m.Text)
                o

            let result = m_endWork.Invoke ar

            let serializer = new JavaScriptSerializer()
            
            m_context.Response.Write "<script>parent.callback('"         
            result
            |> List.map convert
            |> serializer.Serialize          
            |> m_context.Response.Write  
            m_context.Response.Write "');</script>" 
            