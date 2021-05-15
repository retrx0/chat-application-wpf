# Private and group chat application with C# WPF.NET

## Description
Simple chat application using LAN with TCP/IP socket programming technology in C#. Application is a multi thread network application and works in a non-blocking way.
Both group and private messaging are also implemented in this code together with the ability to share relatively small files.

It is a single page applcation that serves both as a server and a client depending on the role the user wants instead of the formal seperate server and client applications.

For simplicity and demonstration purpose user authentication is simply handled with a simple xml file, which can be found [here](/Telecomms/assets/files/users.xml).

## Previews
### Login Page

Test account: 
> Username: user<br>
> Password: pass

<img src='/Telecomms/assets/img/Capture1.PNG' width='550' height='500' />

### Chat Page

- UID: Represents user unique identifier that will be used by other clients to add that user.

<img src='/Telecomms/assets/img/Capture2.PNG' width='550' height='400' />

- Users can join any group with the group ID specified below the UID.

<img src='/Telecomms/assets/img/Capture3.PNG' width='550' height='400' />
