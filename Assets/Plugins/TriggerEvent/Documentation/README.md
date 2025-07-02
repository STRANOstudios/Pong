# TriggerEvent - Professional Unity Event Visualization & Safety Tool

**TriggerEvent** is a powerful, professional-grade extension of UnityEvent developed by **PsychoGarden**. It enhances event management in Unity, offering a streamlined workflow, safety protections, and clear visualization.

## Key Features

-   **Visualize** event connections (origin → target) in the SceneView.
    
-   **Customizable connection colors** per event.
    
-   **Automatic self-reference protection** to prevent recursive loops.
    
-   **Per-event settings** (Show/Hide, DisplayMode: All, None, OnSelected).
    
-   **Global settings** via EditorPrefs (optional and configurable).
    
-   **Hierarchy highlights** for linked GameObjects.
    
-   **Optimized SceneView Handles drawing**.
    
-   **Caching system** for high performance even on large scenes.
    

## Installation

1.  Import the `TriggerEvent` package into your Unity project.
    
2.  Add a `TriggerEvent` field to your MonoBehaviour script:
    

```csharp
using PsychoGarden.TriggerEvents;

public class ExampleTrigger : MonoBehaviour
{
    public TriggerEvent onTriggered;
}

```

3.  Configure options directly in the Inspector.
    

## Folder Structure

```
Assets/
└── TriggerEvent/
    ├── Runtime/
    │   ├── ColliderVisualizer.cs
    │   ├── TriggerEvent.cs
    │   └── PsychoGarden.TriggerEvent.asmdef
    ├── Editor/
    │   ├── ColliderVisualizer.cs
    │   ├── EditorPrefsUtils.CS
    │   ├── TriggerEventDrawer.cs
    │   ├── TriggerEventHandles.cs
    │   ├── TriggerEventHierarchyDrawer.cs
    │   ├── TriggerEventSettings.cs
    │   ├── TriggerEventConnectionPopup.cs
    │   └── PsychoGarden.TriggerEvent.Editor.asmdef
    ├── Utils/
    │    ├── HandlesExtensions.cs
    │    ├── LineAttribute.cs
    │    ├── LineAttributeDrawer.cs
    │    └── PsychoGarden.TriggerEvent.Utils.asmdef
    ├── Documentation/
    │   ├── README.md
    │   └── License.txt
    └── Demo/
        ├── Code Example/
        │   ├── ExampleTriggerEvent.cs
        │   ├── ExampleTarget.cs
        │   ├── SlideMovement.cs
        │   └── TriggerController.cs
        ├── DemoScene.unity
        └── DemoSceneTrigger.unity

```

## Usage Tips

-   Use **per-event settings** if you need specific behavior for a single TriggerEvent.
    
-   Use the **Global Settings** window (Tools ➞ TriggerEvent ➞ Settings) to configure default behavior project-wide.
    
-   Customize your connection colors for clarity in complex scenes.
    
-   TriggerEvent auto-refreshes connections when you edit listeners.
    

## Requirements

-   Unity 2021.3 LTS or newer.
    
-   Compatible with URP, HDRP, and Built-in pipelines.
    

## License

(c) 2024 PsychoGarden. All Rights Reserved.

----------

_TriggerEvent was crafted to deliver professional event management, high editor performance, and ultimate clarity for Unity developers._  
_Built with PsychoGarden._