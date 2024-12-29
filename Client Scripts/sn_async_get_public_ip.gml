/*
* SocketNet is a small set of functions, exported as a library,
* to make online game development easy for everyone.
*
* Creator: CodedBytes.
* Copyright: CodedBytes.
*
* Script Purpose: Responsible for setting up a packet receiver.
*/

///@desc Responsible for retrieving your public ip address. if you chose to not send the connection packet to the server, this function will return your public id address.
///@param {Bool} _do_connection_packet [Its a boolean parameter that makes the function send or not the packet for connection identification]
///@return [Returns Void or String]
function sn_async_get_public_ip(_do_connection_packet) 
{
	// Check if the request succeeds
	if (ds_map_exists(async_load, "id")) {
	    var request_id = async_load[? "id"];
	    if (async_load[? "status"] == 0)
		{
	        var response = async_load[? "result"];
	        var json_data = json_decode(response);

	        if (json_data != undefined && ds_map_exists(json_data, "ip"))
			{
				global.client_ip_address = json_data[? "ip"];
				
				if(_do_connection_packet)
				{
					// Sends the connection packet to the server
					sn_send_new_packet(function(_packet){ buffer_write(_packet, buffer_string, global.client_ip_address); }, 2000);
			
					alarm[0] = -1;
					alarm[1] = game_get_speed(gamespeed_fps) / 1;
				}
				else return json_data[? "ip"];
	        } 
			else { show_message("Error wile decoding the ip address."); }
	    } else { show_message("Error while getting your public ip address. Error Code: " + string(async_load[? "status"])); }
	}
}