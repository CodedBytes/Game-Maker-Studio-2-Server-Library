/*
* SocketNet is a small set of functions, exported as a library,
* to make online game development easy for everyone.
*
* Creator: CodedBytes.
* Copyright: CodedBytes.
*
* Script Purpose: Responsible for creating a new packet to send to the server.
*/

///@desc Responsible for creating a new packet, ready to sent to the server. This version is meant to be used for no data information requests to the server or ping related packets.
///@param {Real} _packet_header [Its a number of the packets header, starting from 1 up to 9999, it acts like an id of the packet];
///@return Void;
function sn_send_new_nodata_packet(_packet_header) 
{
	if(_packet_header > 0 && _packet_header < 10000)
	{
		var packet = buffer_create(global.buffer_size, global.buffer_type, global.buffer_align);
		buffer_seek(packet, buffer_seek_start, 0);
		buffer_write(packet, buffer_u16, _packet_header);

		// Sends the packet
		network_send_raw(global.socket, packet, buffer_tell(packet));
		buffer_delete(packet);
	}
	else { show_error("Your packet header is either higher than 9999 or its equals to 0", true); }
}
