using Godot;
using System;
using System.Collections.Generic;

public partial class Player : Node2D {
	private Node2D parent_node;
	private int total_score;
	private List<Domino> dominos = new List<Domino>();
	private HBoxContainer domino_hand;

	public override void _Ready() {
		GD.Print("player ready.");
		domino_hand = GetNode<HBoxContainer>("DominoHand");
	}

	public override void _Process(double delta) {

	}

	public Player() {
		GD.Print("player default constructor.");
	}

	// NOTE: this constructor is causing an error.
	// is it because you didn't have a default constructor?
	public Player(Node2D _parent_node) {
		GD.Print("player constructor.");
		parent_node = _parent_node;
	}

	public void AddDominoToHand(Domino new_domino) {
		dominos.Add(new_domino);
		domino_hand.AddChild(new_domino);
	}
}
