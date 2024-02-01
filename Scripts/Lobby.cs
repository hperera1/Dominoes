using Godot;
using System;

public partial class Lobby : Panel
{
	private const int DefaultPort = 8910; // An arbitrary number.
	private const int MaxNumberOfPeers = 1; // How many people we want to have in a game
	private LineEdit _address;
	private Button _hostButton;
	private Button _joinButton;
	private Label _statusOk;
	private Label _statusFail;
	private ENetMultiplayerPeer _peer = new();
	private int game_seed = 0;

	public override void _Ready() {
		_address = GetNode<LineEdit>("Address");
		_hostButton = GetNode<Button>("HostButton");
		_joinButton = GetNode<Button>("JoinButton");

		_hostButton.Pressed += OnHostPressed;
		_joinButton.Pressed += OnJoinPressed;

		Multiplayer.ConnectedToServer += ConnectedToServer;
		Multiplayer.PeerConnected += (peer_id) => PeerConnected((int) peer_id);
		GD.Print("Lobby._Ready(): ready finished.");
	}

	private void PeerConnected(int id) { 
		GD.Print("Lobby.PeerConnected(): player connected, id of: " + id + ".");
		// StartGame();
		// Hide();
	}

	private void ConnectedToServer() {
		GD.Print("Lobby.ConnectedToServer(): connected to server. From: " + Multiplayer.GetUniqueId() + ".");

		// the server, who has a unique id of 1, is the only one making this call. it is sending the players name and id out. 
		RpcId(1, nameof(SendPlayerInformation), GetNode<LineEdit>("PlayerName").Text, Multiplayer.GetUniqueId(), game_seed);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void StartGame() {
		GD.Print("Lobby.StartGame: starting game. Printing player info out before. From: " + Multiplayer.GetUniqueId() + ".");
		foreach(PlayerInfo curr_info in GameManager.players) {
			GD.Print(curr_info.player_name + " | ID = " + curr_info.player_id + " | SEED = " + curr_info.rand_seed);
		}

		Node2D game_scene = ResourceLoader.Load<PackedScene>("res://MainGame.tscn").Instantiate<Node2D>();
		GetTree().Root.AddChild(game_scene);
	}

	private void OnHostPressed() {
		_peer.Close();
		Error err = _peer.CreateServer(DefaultPort);
		GD.Print("Lobby.OnHostPressed(): host button pressed, server created.");

		if (err != Error.Ok)
		{
			GD.PrintErr(err.ToString());
			return;
		}

		Multiplayer.MultiplayerPeer = _peer;
		_hostButton.Disabled = true;
		_joinButton.Disabled = true;
		game_seed = Guid.NewGuid().GetHashCode();
		SendPlayerInformation(GetNode<LineEdit>("PlayerName").Text, Multiplayer.GetUniqueId(), game_seed);
	}

	private void OnJoinPressed() {
		string ip = _address.Text;
		GD.Print("Lobby.OnJoinPressed(): join button pressed, player joined. Player: " + Multiplayer.GetUniqueId() + ".");

		if (!ip.IsValidIPAddress())
		{
			GD.PrintErr("Lobby.OnJoinPressed(): Invalid IP.");
			return;
		}

		_peer = new ENetMultiplayerPeer();
		_peer.CreateClient(ip, DefaultPort);
		Multiplayer.MultiplayerPeer = _peer;
	}

	private void OnStartPressed() {
		Rpc(nameof(StartGame));
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void SendPlayerInformation(string _player_name, int _player_id, int _rand_seed) {
		bool player_found = false;
		foreach (PlayerInfo curr_info in GameManager.players) {
			if(curr_info.player_id == 1) {
				_rand_seed = curr_info.rand_seed;
			}
			if(curr_info.player_id == _player_id) {
				player_found = true;
			}
		}

		PlayerInfo player_info = new PlayerInfo() {
			player_name = _player_name,
			player_id = _player_id,
			rand_seed = _rand_seed
		};

		if(!player_found) {
			GameManager.players.Add(player_info);
		}

		// i dont think this chunk of code actually works like how it should. adds duplicates?
		// if(!GameManager.players.Contains(player_info)) {
		// 	GameManager.players.Add(player_info);
		// }

		if(Multiplayer.IsServer()) {
			GD.Print("Lobby.SendPlayerInformation(): the server is calling SendPlayerInformation.");
			foreach (PlayerInfo curr_info in GameManager.players) {
				Rpc(nameof(SendPlayerInformation), curr_info.player_name, curr_info.player_id, curr_info.rand_seed);
			}
		}
	}
}
