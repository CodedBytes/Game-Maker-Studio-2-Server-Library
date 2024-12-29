/*
* SocketNet is a small set of functions, exported as a library,
* to make online game development easy for everyone.
*
* Creator: CodedBytes.
* Copyright: CodedBytes.
*
* Script Purpose: Responsible for instantiate the base conection with the server
*/

///@desc Responsible for initiation the base connection with the server. It will also start the async http request for public ip, so you should have the sn_receive_ip_address(); on your Async HTTP event, or else an exception will appear on the game. The Function returns VOID.
///@param {Constant.SocketType} _socket_type [Its the socket type for the connection, whether its TCP or UDP (mine is TCP)];
///@param {String} _server_ip [Its the server ip address you pu on the server side, wich will be hosted the server on];
///@param {Real} _server_port [Its the server port, where the server is located in the ip adress provided earlier];
///@param {Real} _buffer_size [Its the server's buffer size, the default is 256, but you maight wanna increase it, just make sure the server has the same number];
///@param {Real} _buffer_align [Its the server's buffer alignment, you can set this in whatever number you want, but make sure the server has the same number];
///@return [];
function sn_start_server(_socket_type, _server_ip, _server_port, _buffer_size, _buffer_align) 
{
	// Global Variables Setup
	global.connected_to_server = false;
	global.ip_address = _server_ip;
	global.client_ip_address = "0.0.0.0";
	global.main_server_port = _server_port;
	global.socket = network_create_socket(_socket_type);
	global.ping = 0;
	global.ping_step = 0;
	global.ping_timeout = game_get_speed(gamespeed_fps) * 2;

	// Buffers
	global.buffer_size = _buffer_size;
	global.buffer_type = buffer_grow;
	global.buffer_align = _buffer_align;

	// Connection
	var connect = network_connect_raw(global.socket, global.ip_address, global.main_server_port);
	
	// Pushing IPV4
	if(connect != -1)
	{
		global.connected_to_server = true;
		url = "https://api.ipify.org?format=json";
		request = http_get(url);
	}
	else global.connected_to_server = false;

	// Timeout for the read and write threads of the connection
	network_set_timeout(global.socket, 5000, 5000);
}