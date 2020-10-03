using UnityEngine;
using UnityEngine.UIElements;

public class ExampleUILayout4
	: MonoBehaviour {
	void Start() {
	    var root = GetComponent<UIDocument>().rootVisualElement;
	    var ve = new VisualElement();
	    ve.style.position = Position.Absolute;
	    ve.style.width = 100;
	    ve.style.height = 100;
	    ve.style.backgroundColor = Color.green;
	    ve.transform.scale = Vector3.one ;
	    root.Add(ve);
    }
}
