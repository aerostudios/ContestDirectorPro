# Setup

## UWP

* When this app has reached a stable state, it will be released to the windows store *

In order to test the UWP application on a machine other than your dev box, you will need to sign it w/ a cert and then set up the host machine to be able to side load packages.  See the Microsoft documenation for this. 

[Side Loading](https://docs.microsoft.com/en-us/windows/application-management/sideload-apps-in-windows-10)

[Self-Signing an App](https://docs.microsoft.com/en-us/windows/uwp/packaging/create-certificate-package-signing)

## Hosting the web application (on a non-dev box)

The web application out of the box will work on any machine with IIS and Websockets installed.  Follow the How-To & Microsoft documentation to set up IIS and websockets.
[IIS Setup](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?tabs=aspnetcore2x#iis-configuration) 

[.NET Core Setup](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?tabs=aspnetcore2x#install-the-net-core-windows-server-hosting-bundle)

[Enable Websockets](https://docs.microsoft.com/en-us/iis/configuration/system.webserver/websocket)

When you set up IIS, you will need to create a virtual directory to host the website in order to run the site.  This is partly because of how the site data storage was designed.   If the process running the app pool for the default directory in IIS has write permissions to the folder holding the site code, you don't need to do this.  However, I'm guessing most people do not have this set up or want to do so.  You will need to perform the following tasks to do so. 

+ In IIS create your site, be sure to follow these steps and make sure the directory you are copying the site files to has write access.  [Create the virtual directory](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?tabs=aspnetcore2x#install-web-deploy-when-publishing-with-visual-studio)
+ Copy the deployment bits to the directory you created above
