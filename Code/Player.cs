using Godot;
using System;
using System.Collections.Generic;

public partial class Player : Node2D {
	private Node2D parent_node;
	private int total_score;
	private List<Domino> dominos = new List<Domino>();
	private HBoxContainer domino_hand;
	private Domino selected_domino; 

	public Player() {
		GD.Print("player default constructor.");
	}

	// NOTE: this constructor was causing an error.
	// was it because you didn't have a default constructor?
	public Player(Node2D _parent_node) {
		GD.Print("Player: player constructor.");
		parent_node = _parent_node;
	}

	public override void _Ready() {
		GD.Print("Player: player ready.");
		domino_hand = GetNode<HBoxContainer>("DominoHand");
	}

	public override void _Process(double delta) {

	}

	public void AddDominoToHand(Domino new_domino) {
		dominos.Add(new_domino);
		domino_hand.AddChild(new_domino);

		// TODO: find out if connecting signals in C# is actually bugged
		// IsConnected() was giving you trouble. 
		// currently calling -= before += to prevent multiple subscriptions.
		// (duped custom signal connections)
		new_domino.DominoSelected -= SetSelectedDomino;
		new_domino.DominoSelected += SetSelectedDomino;

		GD.Print("Player: domino added to hand.");
	}

	// on DominoSelected signal, SetDomino(). 
	public void SetSelectedDomino(Domino _selected_domino) {
		GD.Print("Domino: SetDomino()");
		selected_domino = _selected_domino;
		GD.Print(selected_domino);
	}
}
