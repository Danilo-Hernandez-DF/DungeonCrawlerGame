namespace UtilsModule {
    public interface IInventoryHolder {
        public int InventorySize { get; }
        public Inventory heldInventory { get; }
    }
}