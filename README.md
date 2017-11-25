# uiFlow README

An extensible node-based UI editor for flexible UI flows & usage between projects.

### Setup
Add UIFlow.cs to an empty GameObject. If a UI already exists in the scene you can parent the canvas to the new GameObject.


### Editor
* Drag the canvas with your mouse by selecting any empty spot.
* Right click to bring up the Node menu.
* Button to reset view to nodes.
* Save Flows to be used in a different projects.

### Nodes

**In/Out Nodes**  
These nodes are the basic connectors, a set of nodes that triggers the UI flow and a set that leaves it.
Includes *OnSceneLoad*,*OnActivationCalled* for **IN** and *LoadScene*, *Deactivate* for **OUT.**

**Panel & Window Nodes**  
These are the nodes you'll use the most.

**Custom Nodes**  
Custom nodes that have been build by the team or other users.

**Macro Nodes**  
Nodes containing Flows which contain specialized *In* and *Out* nodes to connect to the containing flow. These can be used anywhere in the project

### Connections
Connections are made by clicking an OUT point then an IN or dragged between two points. No IN > IN or OUT > OUT connections can be made.

### Hierarchy View
Where the magic happens.

### Right Click Menus

**On Background**
* Create Node