using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using static Scripts.BuildTree.ScriptableComponent;

namespace Scripts.BuildTree
{
    [CreateAssetMenu(
        fileName = "New Component Library",
        menuName = "Build Tree/New Component Library",
        order = 0
    )]

    public class ScriptableComponentLibrary : ScriptableObject
    {
        public List<ScriptableComponent> ComponentLibrary;

        public List<ScriptableComponent> GetComponentsOfClass(ComponentClasses componentClass)
        {
            return ComponentLibrary.Where(component => component.ComponentClass == componentClass).ToList();
        }
    }

}