
using UnityEngine;
using UnityEngine.UI;
using Myth;

namespace Myth
{
    public partial class DebuggerComponent
    {
        private sealed class OperationsWindow : ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Operations</b>");
                GUILayout.BeginVertical("box");
                {
                    PoolComponent poolComponent = GameEntry.GetBaseComponent<PoolComponent>();
                    if (poolComponent != null)
                    {
                        if (GUILayout.Button("Object Pool Release", GUILayout.Height(30f)))
                        {
                            poolComponent.Release();
                        }

                        if (GUILayout.Button("Object Pool Release All Unused", GUILayout.Height(30f)))
                        {
                            poolComponent.ReleaseAllUnused();
                        }
                    }

                    ResourceComponent resourceCompoent = GameEntry.GetBaseComponent<ResourceComponent>();
                    if (resourceCompoent != null)
                    {
                        if (GUILayout.Button("Unload Unused Assets", GUILayout.Height(30f)))
                        {
                            resourceCompoent.ForceUnloadUnusedAssets(false);
                        }

                        if (GUILayout.Button("Unload Unused Assets and Garbage Collect", GUILayout.Height(30f)))
                        {
                            resourceCompoent.ForceUnloadUnusedAssets(true);
                        }
                    }

                    if (GUILayout.Button("Shutdown Game Framework (None)", GUILayout.Height(30f)))
                    {
                        //GameEntry.Shutdown(ShutdownType.None);
                    }
                    if (GUILayout.Button("Shutdown Game Framework (Restart)", GUILayout.Height(30f)))
                    {
                        //GameEntry.Shutdown(ShutdownType.Restart);
                    }
                    if (GUILayout.Button("Shutdown Game Framework (Quit)", GUILayout.Height(30f)))
                    {
                        //GameEntry.Shutdown(ShutdownType.Quit);
                    }
                }
                GUILayout.EndVertical();
            }
        }
    }
}
