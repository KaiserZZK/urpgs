// Defines how our system increments the stats when components
//  get added to the build. 

using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Scripts.BuildTree
{
    [CreateAssetMenu(
        fileName="New Component",
        menuName ="Build Tree/New Component",
        order=0
    )]

    public class ScriptableComponent : ScriptableObject
    {
        public List<UpgradeData> UpgradeData = new List<UpgradeData>();

        //public bool IsAbility; 
        public string ComponentName;
        public ComponentClasses ComponentClass;
        public bool OverwriteDescription;
        [TextArea(1, 4)] public string ComponentDescription;
        public Sprite ComponentIcon;

        // NOTE not really relevant at this moment, but useful as a reference
        public List<ScriptableComponent> ComponentPrerequisites = new List<ScriptableComponent>();
        public int ComponentTier;
        public int Cost;

        public enum ComponentClasses
        {
            Red,
            Green,
            Blue
        }

        private void OnValidate()
        {
            ComponentName = name;
            if (UpgradeData.Count == 0) return;
            if (OverwriteDescription) return;
            
            GenerateComponentDescription();
            
        }

        private void GenerateComponentDescription()
        {
            //if (IsAbility)
            //{
            //    switch (UpgradeData[0].StatType)
            //    {
            //        case StatTypes.DoubleJump:
            //            ComponentDescription = $"{ComponentName} grants the Double Jump ability.";
            //            break;
            //        case StatTypes.Dash:
            //            // do something
            //    }

            //}
            //else
            //{

            //}

            StringBuilder sb = new StringBuilder();
            sb.Append($"{ComponentName} increases \n");
            for (int i=0; i<UpgradeData.Count; i++)
            {
                sb.Append(UpgradeData[i].StatType.ToString());
                sb.Append(" by ");
                sb.Append(UpgradeData[i].StatIncreaseAmount.ToString());
                sb.Append(UpgradeData[i].IsPercentage ? "%" : " point(s)");
                if (i == UpgradeData.Count -2) sb.Append(" and\n");
                else sb.Append(i < UpgradeData.Count - 1 ? ", \n" : ".");
            }

            ComponentDescription = sb.ToString();

        }

    }

    [System.Serializable]
    public class UpgradeData
    {
        public StatTypes StatType;
        public int StatIncreaseAmount;
        public bool IsPercentage; // increment by amount/proportion

    }

    public enum StatTypes
    {
        Stability,
        Adaptability,
        Strength
    }

}