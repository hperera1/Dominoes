using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

public partial class DominoPool {
	private PackedScene domino_scene;
	private static Random rng = new Random();
	List<Domino> domino_pool = new List<Domino>();
	List<Domino> spent_dominos = new List<Domino>();

	public DominoPool(string domino_file) {
		GD.Print("domino pool constructor.");
		domino_scene = ResourceLoader.Load<PackedScene>("res://Assets/Domino.tscn");
		GenerateDominos(domino_file);
	}

	// TODO: I don't like this argument being a string. come up with something else?
	// I like being able to make the domino pool from a text file though, means it's customizeable.
	private void GenerateDominos(string domino_file) {
		List<int> top_values = new List<int>();
		List<int> bot_values = new List<int>();

		// TODO: getting weird -35 value from text file leading to having to remove the last index.
		string path = @"Code\DominoValues\" + domino_file;
		GD.Print("Path: " + path);
		if(File.Exists(path)) {
			using (StreamReader sr = File.OpenText(path)) {
				char value;
				while ((value = (char)sr.Read()) != '\n') {
					top_values.Add(value - '0');
				}
				while ((value = (char)sr.Read()) != '\n') {
					bot_values.Add(value - '0');
				}

				top_values.RemoveAt(top_values.Count - 1);
				bot_values.RemoveAt(bot_values.Count - 1);
			}
		}

		for(int i = 0; i < top_values.Count; i++) {
			Domino new_domino = new Domino(top_values[i], bot_values[i]);
			domino_pool.Add(new_domino);
		}

		ShufflePool();
		GD.Print("Dominoes in pool: " + domino_pool.Count);
	}

	private void ShufflePool() {
		int n = domino_pool.Count;
		while (n > 1) {
			n--;
			int k = rng.Next(n + 1);
			Domino temp_domino = domino_pool[k];
			domino_pool[k] = domino_pool[n];
			domino_pool[n] = temp_domino;
		}
	}

	public Domino GetNextDomino() {
		if(domino_pool.Count > 0) {
			Domino next_domino = domino_pool[0];
			Domino new_domino = domino_scene.Instantiate<Domino>();
			new_domino.CopyValues(next_domino);
			domino_pool.RemoveAt(0);
			spent_dominos.Add(next_domino);
			return new_domino;
		}

		return null;
	}

	public void PrintPool() {
		foreach (Domino domino in domino_pool) {
			domino.PrintDomino();
		}
	}
}
