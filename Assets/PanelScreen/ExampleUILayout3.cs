using UnityEngine;
using UnityEngine.UIElements;

public class ExampleUILayout3 : MonoBehaviour {
	void Start() {
	    var root = GetComponent<UIDocument>().rootVisualElement;
	    var ve = new VisualElement();
	    ve.style.position = Position.Absolute;
	    ve.style.width = 1;
	    ve.style.height = 1;
	    ve.style.backgroundColor = Color.green;
	    ve.transform.scale = Vector3.one * 100;
	    root.Add(ve);
    }
}
