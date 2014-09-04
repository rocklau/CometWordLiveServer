<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Chat.aspx.cs" Inherits="CometServer.Chat" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Chat</title>
</head>
<body>
    <script language="javascript" type="text/javascript" src="Scripts/jquery-1.4.1.min.js"></script> 
    <script type="text/javascript" language="javascript">
    function getQueryStringByName(name) {
        var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
        if (result == null || result.length < 1) {
            return "";
        }
        return unescape(result[1]);
    }
    //发言者昵称
    var name = getQueryStringByName("name");
    //房间id
    var room = getQueryStringByName("room");
    //key= MD5(chatkey+name) 不包含加号
    var key = getQueryStringByName("key");  

    //管理员审核
    function broadcast(from,msg) { 
        $.ajax({
        type: "POST",
        url: "Send.ashx",
        data: { from: from, to: room, msg: msg, key: key },
        success: function() { $("#msg").val("");  }});
    }  
    //回调数据    
    var callback = function  (messages) {  
        messages=jQuery.parseJSON(messages); 
        for (var i = messages.length - 1; i >= 0; i--) {
        var m = messages[i];
        if(name=="admin")
        record("data",m.from+": " + m.text  +"<a href=\"javascript:broadcast('"+m.from+"','"+m.text+"');\">通过审核</a>");
        else
        record("data",m.from+": " + m.text)  ;
               
        } 

        if($.browser.msie) {

            ifrpush.frames["chatdata"].document.location.reload();
            
        }else
        {
            reloadIframe("chatdata");
             
        }    
    }
   
    
    function reloadIframe(frm)
    {
        var ofrm1 = document.getElementById(frm).document;    
        if (ofrm1==undefined)            
            ofrm1=   document.getElementById(frm).contentWindow.document;             
        else if (ofrm1==undefined)         
            ofrm1=  document.frames[frm].document;
      
        ofrm1.location.reload();
    }
     
    function record(div,text) {  
    $("#"+div).prepend($("<div></div>").html( text)); 
    }  


    var    ifrpush=""; 
    
    $(document).ready(function(){
        if(name=="")
            name= $("#toName").val();
        else
            $("#toName").val(name);   
        if(room=="")
            room="chats";
        if(key=="")
            alert("请在地址内加入key的值");
        document.title = name; 
        if($.browser.msie) {
            ifrpush = new ActiveXObject("htmlfile");            

            ifrpush.open();  
            var ifrDiv = ifrpush.createElement("div"); 
            ifrpush.appendChild(ifrDiv);  
            ifrpush.parentWindow.callback=callback; 
            ifrDiv.innerHTML = "<iframe id='chatdata' src='Receive.ashx?room="+room+"&name="+name+"&key="+key+"'></iframe>";  
            ifrpush.close();  
             
           
    
        }else
        {
            var   ifr=document.createElement("iframe");
            ifr.id="chatdata";
            ifr.style.display="none";
            ifr.src="Receive.ashx?room="+room+"&name="+name+"&key="+key;
            document.body.insertBefore(ifr);
            
            
              
        }


    }); 
    </script>
    <br />
    <br /> 
    网友发言
    <div id="data" style="height: 200px;  width:300px; overflow: scroll;">
    </div>  
</body>
</html>
