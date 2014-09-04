#文字直播程序 Word Live 
本开源程序使用F# mongodb

CometChatInstall.rar 为打包程序,可直接使用

mongo目录为数据库

 	第一次请执行install.bat来安装  
        stop.bat用于停止数据库
        run.bat用于重新启动数据库

##使用说明
Chat.aspx

	功能:		聊天内容窗口 
	使用方法:	请使用iframe 调用
	地址格式:   Chat.aspx?name=username&room=123&key=md5(chatkey+username)
	变量说明:	username为用户昵称,chatkey必须跟本程序web.config中的chatkey值相同

	嘉宾区地址: Chat.aspx?name=username&room=123_vip&key=md5(chatkey+username) //此处为room的id和"_vip"两个字符串合并


	管理员界面: Chat.aspx?name=admin&room=123&key=md5(chatkey+admin)  //此处的name的值为admin不可更改
             
Send.aspx

	功能:       聊天发送
	使用方法:   直接把代码复制到需要的html页面内,注意使用jquery

GetData.ashx

	功能:		操作聊天历史数据
	GetData.ashx?action=remove&msg=你好&room=123    删除历史数据,123房间的"你好"消息
	GetData.ashx?action=remove&from=rocky&room=123  删除历史数据,123房间的"rocky"发过的消息
	GetData.ashx?action=clear&room=123				清空数据
	GetData.ashx?room=123	
