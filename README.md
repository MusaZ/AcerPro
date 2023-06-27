## AcerPro Assignment

***

This Web App provides tracking App with URL in given periodic time to users. And if App URL has Response Code except 2xx then App sends to Email to active user. User can add, update and delete App and can add new user to app.   

This App requires __.NET 6+__ for works. You can check with below code whether it is install or not in the powershell or cmd(Administration require):
````
dotnet --version
````

This code print version of .NET if you have it. And you have this then must do below steps:

1. You have to have SQL Server and at least an run instance.
2. Create a User-Secrets file and set "ConnStr" key and give your DB Connection String to this key.
3. As well as you set "MailUser" and "MailPass" keys and relate data for Email send as Email sender.
4. You have to allow App in your Firewall or Internet Security Software for sending mail.
5. Also you maybe need to adjust mail setting to app use for mail send.

Thanks.

[Linkedin Page][id] 

[id]:https://www.linkedin.com/in/musa-yi%C4%9Fit/
