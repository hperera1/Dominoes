using Godot;

public class PlayerInfo 
{
    public string player_name;
    public int player_id;
    public int rand_seed;     
    public int score;

    public void PrintPlayerInfo() {
        GD.Print("PlayerInfo | " + player_name + ", " + player_id + ", " + rand_seed);
    }
}