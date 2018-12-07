using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	[SerializeField]
	private GameObject plate;

	[SerializeField]
	private GameObject light_case, dark_case;

	private Case[] cases;

	void Start(){
		cases = new Case[64];
		initPlate();
	}

	void initPlate(){
		bool isLight = true;
		for(int i = 0; i < 64; i++){
			GameObject caseGo = isLight ? light_case : dark_case;
			Case createCase = Instantiate(caseGo, plate.transform, false).GetComponent<Case>();
			createCase.init(i);
			cases[i] = createCase;
			if((i+1) % 8 != 0)
				isLight = !isLight;
		}
	}
}
