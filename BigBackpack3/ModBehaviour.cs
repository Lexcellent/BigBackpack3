using ItemStatsSystem;
using ItemStatsSystem.Stats;

namespace BigBackpack3
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        protected override void OnAfterSetup()
        {
            LevelManager.OnAfterLevelInitialized -= OnAfterLevelInitialized;
            LevelManager.OnAfterLevelInitialized += OnAfterLevelInitialized;
        }

        protected override void OnBeforeDeactivate()
        {
            LevelManager.OnAfterLevelInitialized -= OnAfterLevelInitialized;
        }

        void OnAfterLevelInitialized()
        {
            // 背包物品变化
            LevelManager.Instance.MainCharacter.CharacterItem.Inventory.onContentChanged -= OnContentChanged;
            LevelManager.Instance.MainCharacter.CharacterItem.Inventory.onContentChanged += OnContentChanged;
            // 背包排序
            LevelManager.Instance.MainCharacter.CharacterItem.Inventory.onInventorySorted -= OnInventorySorted;
            LevelManager.Instance.MainCharacter.CharacterItem.Inventory.onInventorySorted += OnInventorySorted;
            // 背包容量变化
            LevelManager.Instance.MainCharacter.CharacterItem.Inventory.onCapacityChanged -= OnCapacityChanged;
            LevelManager.Instance.MainCharacter.CharacterItem.Inventory.onCapacityChanged += OnCapacityChanged;
            // 先手动触发一次
            OnContentChanged(LevelManager.Instance.MainCharacter.CharacterItem.Inventory, 0);
        }

        void OnCapacityChanged(Inventory inventory)
        {
            // 如果空闲格子小于10个 扩容20个格子
            if (inventory.Content.Count > inventory.Capacity - 10)
            {
                var addCount = 20;
                LevelManager.Instance.MainCharacter.CharacterItem.AddModifier("InventoryCapacity",
                    new Modifier(ModifierType.Add, addCount, LevelManager.Instance.MainCharacter.CharacterItem));
                inventory.SetCapacity(inventory.Content.Count + addCount);
            }
            // 更新负重，每格5kg
            var mainCharacterMaxWeight = LevelManager.Instance.MainCharacter.MaxWeight;
            LevelManager.Instance.MainCharacter.CharacterItem.AddModifier("MaxWeight",
                new Modifier(ModifierType.Add, inventory.Capacity * 5 - mainCharacterMaxWeight,
                    LevelManager.Instance.MainCharacter.CharacterItem));
        }

        void OnContentChanged(Inventory inventory, int index)
        {
            OnCapacityChanged(inventory);
        }

        void OnInventorySorted(Inventory inventory)
        {
            // Debug.Log($"背包排序，物品个数: {inventory.Content.Count},背包容量: {inventory.Capacity}");
            // 排序后物品个数只会减少，因为物品个数也计算了空白格，只考虑缩容，防止背包过大加载时间长
            if (inventory.Content.Count < inventory.Capacity - 30)
            {
                var addCount = 20;
                LevelManager.Instance.MainCharacter.CharacterItem.AddModifier("InventoryCapacity",
                    new Modifier(ModifierType.Add, -(inventory.Capacity - (inventory.Content.Count + addCount)),
                        inventory));
                inventory.SetCapacity(inventory.Content.Count + addCount);
            }
            // 更新负重，每格5kg
            var mainCharacterMaxWeight = LevelManager.Instance.MainCharacter.MaxWeight;
            LevelManager.Instance.MainCharacter.CharacterItem.AddModifier("MaxWeight",
                new Modifier(ModifierType.Add, inventory.Capacity * 5 - mainCharacterMaxWeight,
                    LevelManager.Instance.MainCharacter.CharacterItem));
        }
    }
}