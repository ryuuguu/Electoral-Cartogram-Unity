using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
 
public class ScrollViewTest3 :MonoBehaviour
{
    //needs to changed to use resources not AssetDatabase
    // stopping webgl compiles
    
    ScrollView itemContainer;
    VisualTreeAsset itemTemplate;
    public void Start()
    {
        // Create UI
        
        var uiDoc= GetComponent<UIDocument>();
        var root = uiDoc.rootVisualElement;
        
        
        var ui = new VisualElement();
        ui.AddToClassList("horizontalContainer");
        var addItemBtn = new Button();
        addItemBtn.name = "addItemBtn";
        addItemBtn.text = "Add Item";
        ui.Add(addItemBtn);
        itemContainer = new ScrollView();
        itemContainer.name = "itemContainer";
        ui.Add(itemContainer);
        
        
        var holder = new VisualElement();
        holder.style.position = Position.Absolute;
        holder.style.width = 500;
        holder.style.height = 300;
        holder.style.backgroundColor = Color.white;
        holder.transform.scale = Vector3.one ;
        root.Add(holder);
        holder.Add(ui);
        
        // Support dynamic items
        
        addItemBtn.clicked += AddListItem;
    }


    VisualElement Item() {
        var result = new VisualElement();
        var label = new Label();
        label.name = "itemIdLbl";
        result.Add(label);
        var deleteBtn = new Button();
        deleteBtn.name = "deleteItemBtn";
        deleteBtn.text = "X";
        result.Add(deleteBtn);
        return result;
    }
    
    void AddListItem()
    {
        var itemUi =Item();
        var itemIdLbl = itemUi.Q<Label>("itemIdLbl");
        itemIdLbl.text = System.Guid.NewGuid().ToString();
        var deleteItemBtn = itemUi.Q<Button>("deleteItemBtn");
        deleteItemBtn.clicked += () => itemUi.RemoveFromHierarchy();
        itemContainer.Add(itemUi);
    }
    
}