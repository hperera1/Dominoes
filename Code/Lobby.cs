using Godot;
using System;
using Godot.Collections;
using System.Diagnostics.CodeAnalysis;

public partial class Lobby : Control
{
    private const int DefaultPort = 8911; // An arbitrary number.
    // private const int MaxNumberOfPeers = 1; // How many people we want to have in a game

    private LineEdit _address;
    private Button _hostButton;
    private Button _joinButton;
    private Label _statusOk;
    private Label _statusFail;
    private ENetMultiplayerPeer _peer = new();

    public override void _Ready() {
        // Get nodes - the generic is a class, argument is node path.
        _address = GetNode<LineEdit>("Address");
        _hostButton = GetNode<Button>("HostButton");
        _joinButton = GetNode<Button>("JoinButton");
        _statusOk = GetNode<Label>("StatusOk");
        _statusFail = GetNode<Label>("StatusFail");

        _hostButton.Pressed += OnHostPressed;
        _joinButton.Pressed += OnJoinPressed;
        // Multiplayer.PeerConnected += (peer_id) => GD.Print($"{Multiplayer.GetUniqueId()} now connected to {peer_id}.");
        Multiplayer.PeerConnected += (peer_id) => PlayerConnected((int) peer_id);

        // Connect all callbacks related to networking.
        // Note: Use snake_case when talking to engine API.
        
        // GetTree().Connect("peer_connected", new Callable(this, nameof(PlayerConnected)));
        // GetTree().Connect("network_peer_disconnected", new Callable(this, nameof(PlayerDisconnected)));
        // GetTree().Connect("connected_to_server", new Callable(this, nameof(ConnectedOk)));
        // GetTree().Connect("connection_failed", new Callable(this, nameof(ConnectedFail)));
        // GetTree().Connect("server_disconnected", new Callable(this, nameof(ServerDisconnected)));
    }

    private void OnHostPressed() {
		// _peer.Host.Compress
        // _peer.CompressionMode = ENetMultiplayerPeer.CompressionModeEnum.RangeCoder;
		// INFO: Might need to look into this compression mode thing. I don't understand it right now but the API got shuffled around a bit.
        _peer.Close();
        Error err = _peer.CreateServer(DefaultPort);
        // Error err = _peer.CreateServer(DefaultPort, MaxNumberOfPeers);
        if (err != Error.Ok)
        {
            GD.Print(err.ToString());
            // Is another server running?
            SetStatus("Can't host, address in use.", false);
            return;
        }

        Multiplayer.MultiplayerPeer = _peer;
        _hostButton.Disabled = true;
        _joinButton.Disabled = true;
        SetStatus("Waiting for player...", true);
    }

    private void OnJoinPressed() {
        string ip = _address.Text;
        if (!ip.IsValidIPAddress())
        {
            SetStatus("IP address is invalid", false);
            return;
        }

        _peer = new ENetMultiplayerPeer();
        // _peer.CompressionMode = ENetMultiplayerPeer.CompressionModeEnum.RangeCoder;

        _peer.CreateClient(ip, DefaultPort);
        Multiplayer.MultiplayerPeer = _peer;
        SetStatus("Connecting...", true);
    }

    // Network callbacks from SceneTree

    // Callback from SceneTree.
    private void PlayerConnected(int id) { 
        // Someone connected, start the game!
        var domino_game = ResourceLoader.Load<PackedScene>("res://MainGameScene.tscn").Instantiate();

        // Connect deferred so we can safely erase it from the callback.
        domino_game.Connect("GameFinished", new Callable(this, nameof(EndGame)), (int) ConnectFlags.Deferred);

        GetTree().Root.AddChild(domino_game);
        Hide();
    }

    private void PlayerDisconnected(int id) {
        EndGame(GetTree().GetMultiplayer().IsServer() ? "Client disconnected" : "Server disconnected");
    }

    // Callback from SceneTree, only for clients (not server).
    private void ConnectedOk() {
        // This function is not needed for this project.
    }

    // Callback from SceneTree, only for clients (not server).
    private void ConnectedFail() {
        SetStatus("Couldn't connect", false);

        // GetTree().NetworkPeer = null; // Remove peer.
		// INFO: NetworkPeer could be now the GetMultiplayer().MultiplayerPeer property.
        GetTree().GetMultiplayer().MultiplayerPeer = null;
        _hostButton.Disabled = false;
        _joinButton.Disabled = false;
    }

    private void ServerDisconnected() {
        EndGame("Server disconnected");
    }

    // Game creation functions

    private void EndGame(string withError = "") {
        if (HasNode("/root/Pong"))
        {
            // Erase immediately, otherwise network might show
            // errors (this is why we connected deferred above).
            GetNode("/root/Pong").Free();
            Show();
        }

        // GetTree().NetworkPeer = null; // Remove peer.
		// INFO: NetworkPeer could be now the GetMultiplayer().MultiplayerPeer property.
        GetTree().GetMultiplayer().MultiplayerPeer = null;
        _hostButton.Disabled = false;
        _joinButton.Disabled = false;

        SetStatus(withError, false);
    }

    private void SetStatus(string text, bool isOk) {
        // Simple way to show status.
        if (isOk)
        {
            _statusOk.Text = text;
            _statusFail.Text = "";
        }
        else
        {
            _statusOk.Text = "";
            _statusFail.Text = text;
        }
    }
}
