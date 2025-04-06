using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using PinChecker.Models;
using PinChecker.Models.Configurations;
using PinChecker.Models.Enums;
using PinChecker.Models.Extensions;
using System.Text.RegularExpressions;

namespace PinChecker.Services.Implementations;

/// <summary>
/// Service implementation for retrieving Potato Pins shop inventory information.
/// </summary>
public class PotatoPinsService(IOptions<PlaywrightServiceConfig> config) : BasePlaywrightService(config)
{
    protected async override Task<Shop> GetShopStatusAsync_Implementation()
    {
        await _page.WaitForSelectorAsync("div.product-list-thumb-info", new() { State = WaitForSelectorState.Visible });

        var htmlContent = await _page.ContentAsync();

        var shop = new Shop
        {
            Name = "Potato Pins",
            Items = [],
        };
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlContent);

        // Find all divs with the product-list-thumb-info class
        var shopItemNodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='product-list-thumb-info']");

        if (shopItemNodes == null)
            return shop;

        foreach (var node in shopItemNodes)
        {
            var item = new ShopItem();

            // Extract name
            var nameNode = node.SelectSingleNode(".//div[@class='product-list-thumb-name']");
            if (nameNode != null)
                item.ItemName = nameNode.InnerText.Trim();
            else
                item.ItemName = "No Name Given";

            // Extract price - find the highest price if range is provided
            var priceNode = node.SelectSingleNode(".//div[@class='product-list-thumb-price']");
            if (priceNode != null)
            {
                var priceText = priceNode.InnerText.Trim();
                // Extract all numbers from the price text
                var prices = Regex.Matches(priceText, @"(\d+\.\d+)|(\d+)")
                                  .Cast<Match>()
                                  .Select(m => double.Parse(m.Value))
                                  .ToList();

                // Set the highest price if found
                if (prices.Count != 0)
                    item.Cost = prices.Max();
            }

            // Extract status
            var statusNode = node.SelectSingleNode(".//div[@class='product-list-thumb-status']");
            if (statusNode != null)
                item.Status = statusNode.InnerText.ToShopStatus();
            else
                item.Status = ShopStatus.Available;

            // Extract the href attribute from the anchor tag
            var anchorNode = node.ParentNode;
            if (anchorNode != null && anchorNode.Attributes["href"] != null)
                item.Link = $"{_config.Url.TrimEnd('/')}{anchorNode.Attributes["href"].Value}";

            shop.Items.Add(item);
        }

        return shop;
    }
}