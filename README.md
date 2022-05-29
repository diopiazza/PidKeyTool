# PidKeyTool  CN/EN Version
网页版: https://pidkey.top

## 微软密钥检测工具

可用于检测零售密钥的有效状态和批量密钥的剩余次数.  
无需系统证书环境支持.直连微软服务器检测.  
检测结果存储于软件目录的KeyList.db数据库中.   

## 问题 Question
    1. 问：用该工具会不会泄露密钥? 
       答：该检测工具的原理是:解码密钥后，通过模拟硬件数据结合密钥数据生成用户证书，发包给微软服务器获取返回结果，所以至始至终只连接微软服务器，如果不放心可以用相关工具抓包分析是否有第三方链接地址。 
    2. 问：会不会对本机造成影响?
       答：传统的密钥检测工具通过安装密钥到系统获取系统返回的错误代码，该工具直接绕过了这一步，所以跟系统证书没有任何关系。
    3. 问：我想知道检测结果为020的密钥是否还能通过微软的电话自助网页(Microsoft Self Service for Mobile site)在线获取确认ID？
       答：菜单中有设置是否要获取该项结果的设置，如果你不知道webact的Token值，软件已经内置了，可以不用设置。注意该项设置会影响检测速度。
    4. 问：能不能检测Office绑定密钥还是否有效?
       答：绑定密钥需要登陆自己的微软账户检测，该账户信息会记录在注册表中(HKEY_CURRENT_USER\Software\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers下的OutlookUsername和OutlookPassword)，以便下次不用再次登陆。纯本地操作，完全不用担心泄露账号。  
    5. 问：每次更新软件后原来的检测结果会不会丢失?  
       答：密钥的检测结构存储于目录的KeyList.db，该数据库没有加密，你可以用DB Browser for SQLite（https://sqlitebrowser.org/ )直接打开浏览，同一目录下该数据库不会被覆盖，你也可以移动旧的数据库到升级后软件的目录。
    6. 问：密钥复活了怎么检测?  
       答：在密钥管理菜单右键，有重新检测菜单，可以选中所有需要重新检测的密钥列表进行重新检测。如果主程序界面是批量检测界面，将会检测所有选中的密钥。如果是单独检测界面，只有检测一个密钥。


![image](https://github.com/laomms/PidKeyBatch/blob/master/checks.gif)

![image](https://github.com/laomms/PidKeyBatch/blob/master/record.gif)

# Tool for check Microsoft Product Key of Windows/Office
#### 1.No windows certificate environment support is required.
#### 2.Support win6.0-win11, office2010-office2021.
#### 3.Automatically store the detection results to a local database.
#### 4.Support check MAK key remaining times.
#### 5.Support check retail key HRESULT code.
#### 6.Support to detect whether the 020 key can obtain the confirmation ID online.
 
 <!---
 [![](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.sandbox.paypal.com/donate?hosted_button_id=WTE9HCGSXGTAN)
 -->
 



## Where can find product key：

[windows10](https://philka.ru/forum/topic/46610-kliuchi-aktivatcii-windows-10-vse-redaktcii/page-309)  
[windows7](https://philka.ru/forum/topic/46608-kliuchi-aktivatcii-windows-7-vsekh-redaktcii/page-134)  
[windows8](https://philka.ru/forum/topic/46609-kliuchi-aktivatcii-windows-8-81-vsekh-redaktcii/page-89)  
[office](https://philka.ru/forum/topic/47480-kliuchi-aktivatcii-microsoft-office-all-version/page-115?hl=office)  
[All](http://forum.rsload.net/)  
[other1](https://vn-z.vn/threads/tong-hop-key-windows-va-office.10945/)   
[other2](https://www.aihao.cc/)   







