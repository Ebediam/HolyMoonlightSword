using BS;

namespace HolyMoonlightSword
{
    // This create an item module that can be referenced in the item JSON
    public class ItemModuleHolyMoonlightSword : ItemModule
    {
        public float switchSpeed = 10f;
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<ItemHolyMoonlightSword>();
        }
    }
}
