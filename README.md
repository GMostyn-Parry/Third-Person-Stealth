# Third-Person-Stealth
A basic "stealth" game demonstration project, using the Unity Engine, where the objective is to reach the finish area (area with a checkered banner surrounding it), and not get caught by the robot (represented by a capsule with a red eye).
## Features
- AI that chases the player, using a state machine for logic representation.
- A camera that avoids collision with level geometry, and the player, but lacks smoothing.
- Two levels that are loaded additively to a main level that holds level-independent logic.
- Buttons that can interacted with by the player when they are in-range.
- Locked doors that are opened by pressing all linked buttons.
## Demonstration Video
https://youtu.be/jDWT_zdZE28
## Details
**Author:** George Mostyn-Parry\
**Finished:** 2019-05-03\
**Last Compiled With:** Unity 2019.1.2f1.

This project uses no audio of any kind.
## Controls
```
W - Forward
S - Back
A - Left
D - Right
F - Interact
E - Interact
Escape - Pause Game
```
Certain GUI elements will appear during play, these can be clicked with the mouse cursor.
## Technical
The following is a series of sections explaining the technical background of various features of the project.
### Finite State Machine
This implementation is based on the implementation from this tutorial from Unity:\
https://unity3d.com/learn/tutorials/topics/navigation/intro-and-session-goals

The state machine is formed from the following classes:
- **States** - ScriptableObjects that define a state in a finite state machine, each storing an array of state actions, and possible transitions; they continuously perform all state actions, repeatedly evaluate the need to transition to a new state, and execute the on-transition actions when a transition event occurs. The first transition to evaluate to true in the transition array is performed, all other transitions are ignored.
- **Actions** - ScriptableObjects that cause an agent to perform a single simple action; i.e. patrol, search, chase, and catch. There are two implicit types of actions; state actions that happen continuously while in a state, and on-transition actions that are only performed during a state transition.
- **Conditions** - ScriptableObjects that evaluate if something is true based on the current state of the agent, and the world.
- **Transitions** - A simple data class type that stores how a state should transition to another; the condition it transitions on, whether it transitions on the condition evaluating to true, the state it transitions to, and the actions to perform before changing state.

AI agents (i.e. the Chaser prefab) have a StateController script attached to them; this script controls the flow of logic, and the storing of high-level information; explicitly, and through a blackboard. This state controller is passed to the ScriptableObject classes, so they can manipulate the agent, and evaluate its state.

Every tick the StateController's *Update()* function is called, which causes it to call the *UpdateState()* function on the current state by passing itself as a parameter. The state will perform all actions plugged into it, and then evaluate all defined transitions for that state, while also passing the state controller into the actions, and conditions. If a condition for a transition evaluates to true, then the actions that should occur on that transition are performed, and the state controller is then passed the state it should transition to.
### Camera Collision
This implementation is based on the implementation from this tutorial from Renaissance Coders:\
https://www.youtube.com/watch?v=MkbovxhwM4I&list=PL4CCSwmU04MjDqjY_gdroxHe85Ex5Q6Dy&index=4

Every tick, after all other objects have moved, the desired follow position, based on player input, is calculated.

The corners of the view frustrum's near plane are then calculated for if the camera was placed in that position. A linecast from the target (i.e. player) to each of the corners checks for collisions to calculate a collision avoidance vector to avoid shearing for that corner. The final collision avoidance vector is in the direction of the average of all avoidance vectors multipled by the shortest collision distance plus some padding. The avoidance vector is subtractred from the desired position, as the linecasts were from the target to the camera, due to colliders overlapping the starting point of a cast not triggering a collision.

If the camera is too close to the target, due to the camera being placed between the target and a nearby mesh, then the camera is raised above the target to avoid shearing, by interpolating height based on how close the camera is to the mesh.

#### Flaws
- Unable to implement smoothing, as it caused a ghosting/stutter effect on the target(player) mesh.
- There is a slight jump between the views on no collision and collision, due to the amount of padding added to avoid shearing.
- Jumps due to changing of the corner that is the shortest collision distance, which is obvious due to the lack of smoothing.
### Multiple Levels
This implementation is based on the implementation from this tutorial from Catlike Coding:\
https://catlikecoding.com/unity/tutorials/object-management/multiple-scenes/

The project has a high-level scene called 'Game' that contains the level-independent logic, such as level changing and the camera. Levels (Level1, Level2) are loaded additively with the 'Game' scene, and assigned as the active scene. If a level is already loaded, then it is unloaded before the new level is loaded in.

Any high-level logic that requires level-dependent logic (i.e. the camera target) is assigned on level load using the *OnSceneLoaded(...)* function.
### Proximity Buttons
This implementation is based on the implementation from this tutorial from Jayanam:\
https://www.youtube.com/watch?v=90OiysC4j5Y&list=PLboXykqtm8dynMisqs4_oKvAIZedixvtf&index=11

The project has an abstract base class called *Toggle* that defines a class that can be toggled into an active, and deactive, state and emits an event when changing to a specific state.

The buttons implement the interface IInteractable that defines an object that can be interacted with by the player. When the player steps into the button's trigger the button is added to the player's list of interactables, if the player presses the interact key then the player will interact will all interactables in their list; which will cause the button to toggle state. When the button toggles state it will change all toggles in its list of controlled toggles to the same state as itself, and emit an event informing of its new state.
### Locked Puzzle Doors
The project contains a class that represents an AND logic gate called *AndCheck*. This class has a list of input toggles that it checks the state of whenever one of them switch to the active state. When all inputs are in the active state then it will change to active itself; and, just like the buttons, it has a list of controlled toggles that the *AndCheck* will change to its new state. When an input toggle is deactivated, then it will change to deactive from active, if necessary.

Doors are just another *Toggle* object that open when active, but when controlled by an *AndCheck* class they will only open when all buttons tracked by the *AndCheck* become true.
