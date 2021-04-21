# PidKeyBatch   CN/EN Version
密钥检测工具,检测结果存储于软件目录的KeyList.db数据库中,附数据库浏览工具:https://sqlitebrowser.org/dl/ .或者直接在检测工具菜单的数据库中编辑查看.    
**注意: 本地检测请事先用证书安装器安装相应的证书.  

###### MAK Key ➞ pidgenx decode ➞ Extendend PID ➞ Get count   
###### Retal Key ➞ SLInstallProofOfPurchase(Install Key) ➞ Get pKeyId ➞ Get pSkuId ➞ SLpGetLicenseAcquisitionInfo get algorithm data ➞ post data    

V9.0  
支持已被数字权利激活的机子查询win10零售密钥的错误代码。

ByPass Windows 10 digital license,Supports checking error code for the win10 retail key under digital activated  machine.

V9.2  
支持在Win10下检测win7和Win8.1零售密钥错误代码

Support detecting win7 and Win8.1 retail keys error code under Win10

V9.3  
因切换需要消耗时间造成不连贯问题.取消Win10下检测win7和Win8.1密钥

Cancel detecting win7 and Win8.1 keys under Win10

V9.5  
解决Office在线密钥失效后本机检测依然为有效的错误提示.增加在线检测功能.

Add online detection function.

![image](https://github.com/laomms/PidKeyBatch/blob/master/checks.gif)

# Tool for check Microsoft Product Key for All Windows Version and All Office Version

#The activation test is best tested in the virtual machine environment with the key type relative to avoid affecting the local certificate environment.

#Example: For test Windows 10 Product Key, this tool must be started on a virtual machine (or PC) with Windows 10 this rule is valid for each product key

#Start the tool right click and on "english" to start it in English

#1.This software comes with a certificate, so no certificate file support is required.

#2.Also, you can import any version's certificate in this software.

#3.Automatically store the detection results to a local database。

#4.After the software is started, the system certificate environment will be modified, which will affect the original certificate of the system. If you want to restore, right-click on the main interface to select the recovery certificate environment. After the restart, it will return to normal.

#5.Support MAK query for all version keys.

#6.Support version key query of Office series 2010-2019 professional enhanced version.

#7.If you query Windows7 or Windows8.1 under Windws10, the version error will be displayed. The opposite is also true.

#8.Under Windows 10, the theory can query all Windows 10 versions of the key, the query process may automatically switch the system version, the special version of the key may not be able to feedback the error code.

#9.Support all Win7 version key queries under Windows 7.

#10.All Windows 8.1 key query is supported under Windows 8.1.

#11.Batch query automatic filtering key and automatically deduplicate key query.

#12.Support for error code queries.

#13.Support certificate management

#14.The rebuild tokens function is mainly used to repair the tokens file when the exception is activated. This function will invalidate the activated certificate. It will also cause the Office certificate to be lost.



