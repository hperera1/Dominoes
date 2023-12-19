using Godot;
using System;
using System.IO;

public partial class Domino : TextureRect {
	// custom signals require an event handler from the object emitting the signal.
	[Signal] public delegate void DominoSelectedEventHandler(Domino domino);

	int top_value; 
	int bot_value;
	TextureRect top_texture;
	TextureRect bot_texture;
	string domino_textures_path = "res://Assets/Images/DominoNumbers/";
	
	private bool grabbed = false;

    public Domino() {
		GD.Print("Domino: domino default constructor.");
	}

	public Domino(int _top_value, int _bot_value) {
		top_value = _top_value;
		bot_value = _bot_value;
	}

	public override void _Ready() {
		top_texture = GetNode<TextureRect>("./TopTexture");
		bot_texture = GetNode<TextureRect>("./BottomTexture");
		SetDominoTextures();
		GD.Print("Domino: domino ready.");
		GD.Print("Domino: domino values: (top=" + top_value + " | bot=" + bot_value + ").");
		AddToGroup("DRAGGABLE");
	}

	public override void _Process(double delta) {
		
	}

    public override void _PhysicsProcess(double delta)
    {
		if(grabbed) {
				
		}
    }

    public override Variant _GetDragData(Vector2 at_position)
    {
		GD.Print("Domino: " + this + " get drop data running.");
		return this;
    }

    public override bool _CanDropData(Vector2 at_position, Variant data)
    {
		GD.Print("Domino: " + this + " can drop data running.");
		Domino other_domino = (Domino) data;
		if(this == other_domino) {
			return false;
		}

		return true;
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
		GD.Print("Domino: " + this + " dropping data.");
    }

	public void DominoClicked(Node _viewport, InputEvent input, int _shape_idx) {
		if (input.IsActionReleased("DominoClick")) {
			GD.Print("Domino: DominoClicked()");
			// emit signal to hand
			EmitSignal(SignalName.DominoSelected, this);
		}
	}

	public void CopyValues(Domino _domino) {
		top_value = _domino.top_value;
		bot_value = _domino.bot_value;
	}

	private void SetDominoTextures() {
		top_texture.Texture = GD.Load<Texture2D>(domino_textures_path + "/" + top_value.ToString() + ".png");
		bot_texture.Texture = GD.Load<Texture2D>(domino_textures_path + "/" + bot_value.ToString() + ".png");
	}

	public void PrintDomino() {
		GD.Print(top_value + ", " + bot_value);
	}
}
