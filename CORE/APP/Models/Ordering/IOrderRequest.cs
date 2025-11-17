namespace CORE.APP.Models.Ordering;

public interface IOrderRequest
{
    public string OrderEntityPropertyName { get; set; }
    
    public bool IsOrderDescending { get; set; }
}