using Godot;
using System;

public partial class Domino : TextureRect {
	int top_value; 
	int bot_value;
	Node2D domino_sprites;
	Sprite2D top_sprite;
	Sprite2D bot_sprite;
	
	public override void _Ready() {
		GD.Print("domino ready.");
	}

	public override void _Process(double delta) {
		
	}

	public Domino() {
		GD.Print("domino default constructor.");
	}

	public Domino(int _top_value, int _bot_value) {
		top_value = _top_value;
		bot_value = _bot_value;
	}

	public void CopyValues(Domino _domino) {
		top_value = _domino.top_value;
		bot_value = _domino.bot_value;
	}

	public void PrintDomino() {
		GD.Print(top_value + ", " + bot_value);
	}
}
