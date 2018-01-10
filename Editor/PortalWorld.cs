using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRC.Core;
using VRCSDK2;

public class PortalWorld : EditorWindow
{

    [MenuItem("Snail/Generate Portal World")]
    public static void OpenAutoWorld()
    {
        EditorWindow.GetWindow<PortalWorld>();
    }

    private VRC_PortalMarker portal;
    private VRC_SceneDescriptor world;
    private string blueprintId;
    private string worlds;

    public void OnEnable()
    {
        portal = AssetDatabase.LoadAssetAtPath<VRC_PortalMarker>("Assets\\VRCSDK\\Prefabs\\World\\VRCPortalMarker.prefab");
        if (portal == null)
        {
            // Is the SDK not installed? Is the above asset path is wrong
            Debug.LogError("Failed to load VRCPortalMarker.prefab. Generation will not work.");
        }

        world = AssetDatabase.LoadAssetAtPath<VRC_SceneDescriptor>("Assets\\VRCSDK\\Prefabs\\World\\VRCWorld.prefab");
        if (portal == null)
        {
            // Is the SDK not installed? Is the above asset path is wrong
            Debug.LogError("Failed to load VRCWorld.prefab. Generation will not work.");
        }

    }
    public void OnGUI()
    {
        GUILayout.Label("Paste world ids below. One per line.");
        worlds = EditorGUILayout.TextArea(worlds, GUILayout.Height(position.height - 80));
        GUILayout.Label("Optional Blueprint ID");
        blueprintId = EditorGUILayout.TextField(blueprintId);
        if (GUILayout.Button("Generate")) generateWorld();
    }

    public void generateWorld()
    {
        // Delete any/all portal or scene descriptors first.
        foreach(var portal in GameObject.FindObjectsOfType<VRC_PortalMarker>())
        {
            DestroyImmediate(portal.gameObject);
        }

        foreach(var existingScene in GameObject.FindObjectsOfType<VRC_SceneDescriptor>())
        {
            DestroyImmediate(existingScene.gameObject);
        }
        

        // Start the spawn back a little bit. Portals are from (0,0,0) to (whatever, 0, 0);
        VRC_SceneDescriptor scene = Instantiate(world, new Vector3(0,0,-2), Quaternion.identity);
        if(blueprintId != null)
            scene.GetComponent<PipelineManager>().blueprintId = blueprintId;
        
        // Place all the portals
        Vector3 position = Vector3.zero;
        Vector3 step = new Vector3(2, 0, 0);
        
        foreach(string line in worlds.Split('\n'))
        {
            string worldId = line.Trim();
            if (!worldId.StartsWith("wrld")) {
                Debug.Log("Skipped Line:" + line);
                continue; // Skip bad lines.
            }
            Debug.Log("Add portal: " + worldId);
            VRC_PortalMarker marker = Instantiate(
                portal,
                position,
                Quaternion.identity);
            marker.roomId = worldId;

            position += step;
        }

        // Setup the plane.
        GameObject plane = GameObject.Find("Plane");
        if (plane == null)
        {
            plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        }
        
        plane.transform.position = (position - step) / 2 - new Vector3(0,0,2); 
        plane.transform.localScale = position / 2 * .1f + Vector3.one;
    }
}
