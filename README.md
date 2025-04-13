


# Botnet-With-Minecraft

This project is completely as you heard in the title, I made a plugin that turns minecraft servers into botnets. I used python and java in this project and additionally c# for the compiler application but downloading the compiler application is not mandatory.


---
# **Disclaimer ⚠️**
- **This software was developed for personal training and sandbox testing. The developer of the software is not responsible in any way for its misuse. By downloading or using this program, you acknowledge that you are solely responsible for any consequences that may result from the use of this program.**
---


## How does it work?

First of all, there is a server, this is you, you start the API on your computer and it starts listening. A minecraft server sends a request to the API it opens with the register endpoint and writes the minecraft server IP to register.txt. Then, it starts a server on port 8080 on the minecraft server and starts listening. There are 2 commands that can come from the server, one is ping and the other is the online command. Although the ping command is our main event, if we think that this plugin has 10,000 people, a small ddos ​​attack can be made. Also, when the other command is reached to the online endpoint, a request is sent to the server and if it returns 200, it is marked as online. You can use the dashboard made with flask+html to send a ping command and see online servers.
## Technologies Used

**API:** [Python](https://www.python.org/downloads/)

**Server:** [JAVA](https://www.python.org/downloads/)

**Builder:** [C#](https://learn.microsoft.com/tr-tr/dotnet/csharp/)

## Setup

Clone the project

```bash
git clone https://github.com/DarkMirrorq/Botnet-With-Minecraft.git
```
Enter Admin Server Directory Then Enter Comand
```bash
pip install -r requirements.txt
```

Firstly make sure you have static ip if you don't have use [ngrok](https://ngrok.com/). If you are goint use ngrok make sure you set server for static domain [ngrok domains](https://dashboard.ngrok.com/domains). After these steps run app.py it will run dashboard on http://localhost:5000 u can manage things in this.


The server configuration has done. Next step is plugin configuration.

In this case there are 2 ways if you are not advanced user use the compiler in the release section you just download enter server url and it's done.

If you are advanced user this path shows you how to compile the plugin.


Install [Apache Maven](https://maven.apache.org/download.cgi) then add bin directory to path. Then download [Java 21](https://www.oracle.com/tr/java/technologies/downloads/) make sure Java home is set. 

After these reboot your computer. Enter cmd this comand

```bash
mvn --version
```
You  should see something like this
```bash
Apache Maven 3.9.9 (8e8579a9e76f7d015ee5ec7bfcdc97d260186937)
Maven home: C:\apache-maven-3.9.9
Java version: 21.0.6, vendor: Oracle Corporation, runtime: C:\Program Files\Java\jdk-21
Default locale: tr_TR, platform encoding: UTF-8
OS name: "windows 11", version: "10.0", arch: "amd64", family: "windows"
```
After that go to **src\main\resources\.env** file and change the api url to your static ip or ngrok domain

After you set edit Main.java in **src\main\java\me\plugin** on line 18 there is a variable called debugMode if you want logs on server log for testing you can leave this true. But if you want use in real life switch to false. 

When you done setup switch to main directory of plugin then enter the comand
 
```bash
mvn clean install
```
After some time you will see **BUILD SUCCES** message. The plugin will be in target directory **Minecraft Plugin\target** it will be named as **aktiviteplugin-1.0-SNAPSHOT.jar**

Now you can distribute this to anywhere when server started it will register ip to api den it's part of botnet. Enjoy!

If you wish you can compile the plugin compiler by yourself Downlaod [Visual Studio](https://visualstudio.microsoft.com) then download **.NET Desktop Developement** Pack. After that open the **.sln** file in **Plugin Compiler** folder u can use in visual studio but if you want you want to export as exe follow these steps.

Right click the label called **Plugin Derleyici** on rigt column next to boxed c# logo. Then click publish select folder as publishing location after click agaşn to folder. Then choose a export folder from your computer then click end. After tab closed click show all setting then change the distribution option to in itself then select target runtime to your operating system hit safe the hit publish. You will wait 10-20 seconds acording to your computer when build is done u can use compiler. Before runnig make sure maven and plugşn souurce code is in apps root directory maven should named as maven and source code as source_code. After this you completed the compiler.


If you have any problem or any question you can create a issue ticket. 


