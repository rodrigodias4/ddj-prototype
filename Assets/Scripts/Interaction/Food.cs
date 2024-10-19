public class Food : InteractableItem
{
    public enum Order { Burger, Ham, Stew };
    public Order order;
    public override string GetTooltip()
	{
		return "Get Food";
	}
}