using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.BuildTree
{
    public class BuildTreeManager : MonoBehaviour
    {

        private int _stability, _adaptability, _strength; // stats
        private int _emptySlots;

        public int Stability => _stability;
        public int Adaptability => _adaptability;
        public int Strength => _strength;

        // TODO: goals need to be adjutable
        public bool StabilityGoalAchieved => _stability >= 40;
        public bool AdaptabilityGoalAchieved => _adaptability >= 50;
        public bool StrengthGoalAchieved => _strength >= 40;

        public int EmptySlots => _emptySlots;
        public UnityAction OnEmptySlotsChanged;
        private List<ScriptableComponent> _addedComponents = new List<ScriptableComponent>();

        // Keep track of which slot each component is in
        private Dictionary<ScriptableComponent, int> _componentSlots = new Dictionary<ScriptableComponent, int>();
        private Interpreter _interpreter;

        private void Awake()
        {
            // TODO: start empty slot needs to be adjustable
            _stability = 0;
            _adaptability = 0;
            _strength = 0;
            _emptySlots = 7;

            _interpreter = FindObjectOfType<Interpreter>();
        }

        public void ConsumeEmptySlot()
        {
            _emptySlots--;
            OnEmptySlotsChanged?.Invoke();
        }

        public bool CanAddComponent(ScriptableComponent component)
        {
            // TODO also consider class difference with adjacent component
            return _emptySlots > 0 || _componentSlots.ContainsKey(component); // Allow if empty slot available or if component already added
        }


        public void AddComponent(ScriptableComponent component, int slotIndex)
        {
            if (!CanAddComponent(component)) return;
            if (!_componentSlots.ContainsKey(component))
            {
                // If the component is being added to the build tree for the first time
                ModifyStats(component, true); // Increase stats
                _addedComponents.Add(component);
                _emptySlots--;
            }
            // Update the slot information
            _componentSlots[component] = slotIndex;
            NotifyInterpreter(component, slotIndex);
            OnEmptySlotsChanged?.Invoke();
        }

        public void RemoveComponent(ScriptableComponent component)
        {
            if (!_componentSlots.ContainsKey(component)) return;

            // Remove the component from the build tree and reset stats
            ModifyStats(component, false); // Decrease stats
            _addedComponents.Remove(component);
            _componentSlots.Remove(component);
            _emptySlots++;
            OnEmptySlotsChanged?.Invoke();
        }

        public void ModifyStats(ScriptableComponent component, bool increase)
        {
            foreach (UpgradeData data in component.UpgradeData)
            {
                switch (data.StatType)
                {
                    case StatTypes.Adaptability:
                        ModifyStats(ref _adaptability, data, increase);
                        break;
                    case StatTypes.Strength:
                        ModifyStats(ref _strength, data, increase);
                        break;
                    case StatTypes.Stability:
                        ModifyStats(ref _stability, data, increase);
                        break;
                }
            }
        }

        private void ModifyStats(ref int stat, UpgradeData data, bool increase)
        {
            bool isPercent = data.IsPercentage;
            int amount = isPercent ? (int)(stat * (data.StatIncreaseAmount / 100f)) : data.StatIncreaseAmount;
            stat += increase ? amount : -amount;
        }

        // NOTE not really relevant at this moment, but useful as a reference
        public bool IsComponentAdded(ScriptableComponent component)
        {
            return _addedComponents.Contains(component);
        }

        public bool PreReqMet(ScriptableComponent component)
        {
            return component.ComponentPrerequisites.Count == 0 || component.ComponentPrerequisites.All(_addedComponents.Contains);
        }

        private void NotifyInterpreter(ScriptableComponent component, int slotIndex)
        {
            string message = $"Component {component.name} has been added to slot {slotIndex}";
            _interpreter.Interpret(message);  // Send message to the Interpreter
        }
    }
}