/*
* SocketNet is a small set of functions, exported as a library,
* to make online game development easy for everyone.
*
* Creator: CodedBytes.
* Copyright: CodedBytes.
*
* Script Purpose: Responsible for setting up a packet receiver.
*/

///@desc Responsible for setting up a packet receiver. All you need to do is to pass a function to it with an switch inside, which will receive the packet headers of each packet.
///@param {Function} _packet_list [Its the switch statement that you're gonna use to define what the client will do based on the packet header received from the incoming data];
///@return Void
function sn_async_packet_receiver(_packet_list) 
{
	// Get the id of the socket.
	var _socket_id = ds_map_find_value(async_load, "id");

	// Check if socket is our own socket.
	if (_socket_id = global.socket)
	{
	    // Check the type of network event.
	    var _type = ds_map_find_value(async_load, "type");
	    switch (_type)
	    {
	        // Incomming Data Event.
			case network_type_data:
			{
				// Get the packet from the server.
				var _read_buffer = ds_map_find_value(async_load, "buffer");
				var _packet_header = buffer_read(_read_buffer, buffer_u16);
				
				// Client Packet list
				_packet_list(_packet_header);

				break;
			}
	    }
	}
}