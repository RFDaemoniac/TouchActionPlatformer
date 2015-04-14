namespace NodeCanvas{

	interface ISavable{

		string saveKey{get;}
		string Save();
		bool Load();
	}
}