using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Case : MonoBehaviour {

	public enum case_type{
		LIGHT,
		DARK
	}

	[SerializeField]
	private case_type type;

	public case_type getType(){return type;}

	private int indexInPlate;

	public void init(int nb){
		transform.position = new Vector3(-4 + (nb%8), 0,-4 + (nb/8));
	}
}
