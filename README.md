# uiFlow  
An extensible node-based UI editor for flexible UI flows & usage between projects.

### Setup  
Add UIFlow.cs to an empty GameObject. If a UI already exists in the scene you can parent the canvas to the new GameObject.
If a non-uiFlow menu is present uiFlow will let you set up automatically or manually.

### Editor  
* Drag the canvas with your mouse by selecting any empty spot.
* Right click to bring up the Node menu.
* Button to reset view to nodes.
* Save Flows to be used in a different projects.

### Nodes

**In/Out Nodes**  
These nodes deal with UI-wide control, when to start running uiFlow and when to leave it. These are the only standard nodes that have only one input or output.
Includes *OnSceneLoad*,*OnActivationCalled* for **IN**. *LoadScene*, *Deactivate* for **OUT**.

**Basic Nodes**  
- They dynamically resize based on the number of IN/OUTs  
- Right click to create new OUT  
- IN/OUT connections are renamable, automatically renaming UI text  
- The Focus button lets you bring up the corresponding UI in the editor as if in-game  

**Custom Nodes**  
Custom nodes that have been build by the team or other users. Easy to create custom node classes mean developers can add custom editors and logic into the flow.
These nodes are completely modular and can be directly used by themselves with uiFlow in another project.

**Macro Nodes**  
Nodes containing Flows which contain specialized *In* and *Out* nodes to connect to the containing flow. These can be used anywhere in the project. An example use case would be a settings menu, which may want to be accessed from ingame as well as the menu. Creating a macro for this menu would mean changes made in the main menu will apply to the pause menu.

### Connections
Connections are made by clicking an OUT point then an IN or dragged between two points. No IN > IN or OUT > OUT connections can be made.
You can use connections to define where & *how* the flow moves.
You can set a connection to be a normal connection, or an Extension: Current UI stays open and is interactable. or an Overlay: Current UI stays open but is no longer interactable and may have an effect applied (blur, pixelate) rendering behind the new UI.

### Hierarchy View
Where the magic happens. Each node, connection, label change, IN/OUT added, etcetera applies to the Hierarchy. Proper UI wiring is automated by the editor, including text content, hierarchy organisation, GameObject names, and management of active UI elements.