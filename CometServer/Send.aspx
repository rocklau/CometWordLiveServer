<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Send.aspx.cs" Inherits="CometServer.Send" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>发送窗口</title>
</head>
<body>
    <script language="javascript" type="text/javascript" src="Scripts/jquery-1.4.1.min.js"></script> 
    <script type="text/javascript" language="javascript"> 
     
      function getQueryStringByName(name){
         var result = location.search.match(new RegExp("[\?\&]" + name+ "=([^\&]+)","i"));
         if(result == null || result.length < 1){
              return "";
          }
           
          return decodeURI(result[1]);
     }
    //发言者昵称
    var name = getQueryStringByName("name");
    //房间id
    var room = getQueryStringByName("room");
    //key= MD5(chatkey+name) 不包含加号
    var key = getQueryStringByName("key");


    //网友区直接发言   
    function sendMsgAll() {
        var msg = $("#msg").val();
        var toName = $("#toName").val();  
        $.ajax({
        type: "POST",
        url: "Send.ashx",
        data: { from: toName, to:room, msg: msg ,key:key},
        success: function() { $("#msg").val(""); }});
    }
    //网友区需要审核发言    
    function sendMsg() {
        var msg = $("#msg").val();
        var toName = $("#toName").val(); 
            
        $.ajax({
        type: "POST",
        url: "ClientSend.ashx",
        data: { from: toName, to: room, msg: msg,key:key },
        success: function() { $("#msg").val(""); }});
    }
    //嘉宾区需要审核发言  
    function sendVipMsg() {
        var msg = $("#msg").val();
        var toName = $("#toName").val();
       
        $.ajax({
        type: "POST",
        url: "ClientSend.ashx",
        data: { from: toName, to: room+"_vip", msg: msg,key:key },
        success: function() { $("#msg").val(""); }});
    }
    //嘉宾区直接发言  
    function sendVipMsgAll() {
        var msg = $("#msg").val();
        var toName = $("#toName").val();

        $.ajax({
            type: "POST",
            url: "Send.ashx",
            data: { from: toName, to: room + "_vip", msg: msg, key: key },
            success: function () { $("#msg").val(""); } 
        });
    }


    $(document).ready(function () {
        if (name == "")
            name = $("#toName").val();
        else
            $("#toName").val(name);
        if (room == "")
            room = "chats";                            //默认房间
        if (key == "")
            key = "b97b0eb4a12610169a5e7847ccc248d9"; //此处是md5(oiaoboio匿名网友) alert("请在地址内加入key的值");


    });
    document.onkeydown = function(evt){
 
        var evt = window.event?window.event:evt;
        if(evt.keyCode==13)
        {
            sendMsgAll();
        }
    }
    function goDataPage() {

        this.location.href = "GetData.ashx?room=" + room;
    
    }         
    </script> 
    我的名字：<input type="text" value="匿名网友" id="toName" disabled  width="50" /><br/>
    发言内容：<input type="text" value="" id="msg" />    
    <input type="button" value="发言" onclick="sendMsgAll()" />
    <input type="button" value="提问" onclick="sendVipMsg()" /> 
    <a href="javascript:void(0);" onclick="javascript:goDataPage();"  >网友历史记录</a> 
     
   
</body>
</html>
