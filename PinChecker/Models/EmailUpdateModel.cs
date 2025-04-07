namespace PinChecker.Models;

public class EmailUpdateModel
{
    public List<ShopChanges> Changes { get; set; }

    // Email Appearance
    public string EmailTitle { get; set; }
    public string EmailSubtitle { get; set; }
    public string FooterMessage { get; set; }

    // Section Headers
    public string NewItemsHeader { get; set; }
    public string ChangedItemsHeader { get; set; }
    public string RemovedItemsHeader { get; set; }
}