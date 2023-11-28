using Godot;
using System;
using System.Collections.Generic;

public partial class GameController : Node2D {
	[Export]
	public PackedScene player_scene; 
	
	// default player count = 2. This can and will be configured later.
	private int player_count = 2;
	private List<Node2D> player_nodes = new List<Node2D>();
	private List<Player> players = new List<Player>(); 

	private DominoPool domino_pool;
	private int domino_hand_max = 10;

	public override void _Ready() {
		// high level concept:
		// prompt lobby leader for settings (later)
		// apply settings (later)
		// initialize players (now)
		// distribute dominos (now)
		// start game (now)
		for(int i = 1; i <= player_count; i++) {
			Node2D player_node = GetNode<Node2D>("./Players/Player" + i.ToString());
			player_nodes.Add(player_node);
		}

		StartGame();
	}

	public override void _Process(double delta) {
		
	}

	private void StartGame() {
		// initialize players
		for (int i = 0; i < player_count; i++) {
			GD.Print("new player instanced.");
			Player new_player = player_scene.Instantiate<Player>();
			player_nodes[i].AddChild(new_player);
			players.Add(new_player);
			GD.Print("new player added.");
		}

		// create domino pool from configured domino setting
		domino_pool = new DominoPool("VanillaDominoes.txt");

		// distribute dominos to players
		for (int i = 0; i < domino_hand_max; i++) {
			for (int j = 0; j < player_count; j++) {
				Domino new_domino = domino_pool.GetNextDomino();
				players[j].AddDominoToHand(new_domino);
			}
		}
	}
}
