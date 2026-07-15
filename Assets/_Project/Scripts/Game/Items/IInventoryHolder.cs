namespace Game {
    public interface IInventoryHolder {
        public int InventorySize { get; }
        public Inventory HeldInventory { get; }
    }
}