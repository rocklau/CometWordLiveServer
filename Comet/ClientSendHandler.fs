#light  
#if INTERACTIVE
#r "FSharp.PowerPack.Linq.dll"
#r "System.Core.dll"
#endif
namespace Comet.Chating

open Comet
open System
open System.Web 
open System.Web.Security;
open System.Linq
open MongoDB
open MongoDB.Linq
type ClientSendHandler() =

    interface IHttpHandler with
        member h.IsReusable = false
        member h.ProcessRequest(context) = 
            let from = context.Request.Form.Item("from");
            let room = context.Request.Form.Item("to") 
            let msg = context.Request.Form.Item("msg")
            let key = context.Request.Form.Item("key").ToLower() 
            let  k= System.Configuration.ConfigurationSettings.get_AppSettings().Get("chatkey")+from
            let pwd=FormsAuthentication.HashPasswordForStoringInConfigFile(k, "MD5").ToLower()
            if pwd.Equals(key) then  
                  Chat.send from "admin" msg  room  
            context.Response.Write "提交成功,等待管理员审核内容!"