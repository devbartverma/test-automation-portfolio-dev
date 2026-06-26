namespace CSharpPlaywright.Fixtures;

/// <summary>
/// Strongly-typed product model mirroring the JS-TS PRODUCTS data store.
/// DataTest is the add-to-cart button's data-test id, Name is the display name,
/// and Price is the catalogue price in USD.
/// </summary>
public record Product(string DataTest, string Name, double Price);

public static class Products
{
    public static readonly Product Backpack =
        new("add-to-cart-sauce-labs-backpack", "Sauce Labs Backpack", 29.99);

    public static readonly Product BikeLight =
        new("add-to-cart-sauce-labs-bike-light", "Sauce Labs Bike Light", 9.99);

    public static readonly Product BoltTShirt =
        new("add-to-cart-sauce-labs-bolt-t-shirt", "Sauce Labs Bolt T-Shirt", 15.99);

    public static readonly Product FleeceJacket =
        new("add-to-cart-sauce-labs-fleece-jacket", "Sauce Labs Fleece Jacket", 49.99);

    public static readonly Product Onesie =
        new("add-to-cart-sauce-labs-onesie", "Sauce Labs Onesie", 7.99);

    public static readonly Product RedTShirt =
        new("add-to-cart-test.allthethings()-t-shirt-(red)", "Test.allTheThings() T-Shirt (Red)", 15.99);

    /// <summary>All six products, used for bulk add-to-cart scenarios.</summary>
    public static readonly Product[] All =
    {
        Backpack,
        BikeLight,
        BoltTShirt,
        FleeceJacket,
        Onesie,
        RedTShirt,
    };
}

public static class SortOptions
{
    public const string NameAZ = "az";
    public const string NameZA = "za";
    public const string PriceLoHi = "lohi";
    public const string PriceHiLo = "hilo";
}
