# Game-Maker-Studio-2-Server-Library
Game maker studio 2 tcp netwroking server. 

Everything you need to compile the server is at the Auth Server folder.<br>
At the release tab, you will find the complete library for game maker studio 2 and hovering the functions will explain what everything works.

The server was built to let anyone create their own packets and informations, all you need to know is a basic c# coding skills and patience to test everything :)

Special thanks to FatalSheep and CinderFire for the old basic networking tutorial at the Game maker studio forum (rest in peace tutorial.. XD)

## Compiling the server
In order to compile the server you will need to install Visual Studio 2022 Community edition. <br>

The server is prepared to be compiled either in x86 or x64 bits, and it should be able to run it on windows, linux, and macOs systems without further problems.

## Creating Packets
Creating packets is simple: on the client side you can call the the function ``` sn_send_new_packet(); ``` or ``` sn_send_new_nodata_packet(); ``` from the library.
<br> For the ``` sn_send_new_packet(); ``` you need to pass an function inside like this:
```gml 
  // Packet with informations to send to the server.
  // You can use any parameter name inside the function, but remember that the whatever the name you chose, it will have the buffer object created by the sn_send_new_packet() inside it.
  sn_send_new_packet(function (_packet) {
    buffer_write(_packet, buffer_string, "test string"); // <-- First he buffer provided by the main function, second the data type (buffer_string, buffer_u8 up to 64, buffer_f8 up to 64, buffer_bool, buffer_text)
    buffer_write(_packet, buffer_u16, 1); // <-- i'll send another one too, but this time as a number
  }, 1025);// <-- the 1025 number is the packet identifier (packet header) you gonna add on the server side so the server can process the data you sent.

  // And... thats it, you just need now treat it on the server side
```
Now, to treat the packet from the client on the server side, you just need to do something like this:
```csharp 
  // At the socketHelper class you're gonna find the Read function, and inside the switch you gonna add a new case with the same packet id (packet header) you chose on the server.
  case 1025:
    {
      //first you chose the variable type, since we sent a string on the client, the variable must be the same.
      // Note that the buffer will read the packet from the start, on the client we sent one string and one int value, so you should declare the variables in the same order, like below:
      readBuffer.Read(out string msg);
      readBuffer.Read(out UInt16 msg); // <-- note that the int is a 16bits integer type, the same we sent from the client.

      // here we can do whatever you need to do with those values, like saving it on a database or .. whatever XD
      // Lets suppose i treated the data and i'm sendint a new string and integer value to te client.
      string newMsg = "new msg";
      UInt16 newInt = 3;

      // And then you might wanna write it back to the client
      BufferStream buffer = new BufferStream(BufferSize, BufferAlignment);// Start a new packet for the giving minimun size and alignment (should be the same as game maker's client)
      buffer.Seek(0); // <-- Start the packet from the position 0
      ushort constant_out = 1025; // <-- it doesnt need to be the same packet number, but if you wanna make things easy to understand you can use the same to know which one it is on both sides.
      buffer.Write(constant_out);// <-- Write the information to the packet you're gonna send
      buffer.Write(newMsg);// <-- the order here isnt important, but you must put everything after the constant_out value, and at the client, you need to read it at the same order.
      buffer.Write(newInt);
      SendMessage(buffer);// <-- send it back to the client who sent the packet.
    }
```
And last but not least, inside the client (Object Obj_NetController on game maker studio 2) you can go to the Asyncronous Networking action and inside the piece of code you will find all the packets that the client can receive.
<br> You just need to add the packet number 1025 you sent from the server on the switch, like this:
```gml 
  case 1025: 
    {
      // Here you should do the same as the server side, the buffer system will read from the start so you need to read the data in the same order you sent on the server.
      var newMsg = buffer_read(read_buffer, buffer_string);
      var newInt = buffer_read(read_buffer, buffer_u16);

      // And here you can use the variables the way you want.
      break;// <-- Remember that for packets such as Pings and informations that the server needs to constantly send informations to the client, the break should not be declared.
    }
```
And thats how you set up packets on the server! :)

## Features
The server has some basic features, but at this point you can only use the Auth Server. Or if you just want to make the auth and game server in the same place, you can build your packets inside the project and edit it like you want.
<br><br>The following features comes along the server, such as:

  - Complete TCP Handler;
  - Basic Packet Decrypting;
  - BufferStream system.
  - Packet list.
  - Disconnection handler.
  - Ping handler. (15ms average + true ms from each packet sent).
  - Initial packet for connection.
<br><br>
Any questions, please drop a message at my discord: yuuto.x
