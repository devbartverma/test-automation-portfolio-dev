namespace CSharpPlaywright.Fixtures;

public static class Users
{
    public const string StandardUsername = "standard_user";
    public const string StandardPassword = "secret_sauce";
    public const string LockedUsername = "locked_out_user";
    public const string LockedPassword = "secret_sauce";
    public const string PerformanceGlitchUsername = "performance_glitch_user";
    public const string PerformanceGlitchPassword = "secret_sauce";
    public const string InvalidUsername = "invalid_user";
    public const string InvalidPassword = "wrong_password";
}

public static class Urls
{
    public const string Base = "https://www.saucedemo.com";
    public const string Inventory = "https://www.saucedemo.com/inventory.html";
    public const string Cart = "https://www.saucedemo.com/cart.html";
    public const string CheckoutStep1 = "https://www.saucedemo.com/checkout-step-one.html";
    public const string CheckoutStep2 = "https://www.saucedemo.com/checkout-step-two.html";
    public const string CheckoutComplete = "https://www.saucedemo.com/checkout-complete.html";
}

public static class ErrorMessages
{
    public const string LockedOut = "Epic sadface: Sorry, this user has been locked out.";
    public const string InvalidCredentials = "Epic sadface: Username and password do not match any user in this service";
    public const string MissingUsername = "Epic sadface: Username is required";
    public const string MissingPassword = "Epic sadface: Password is required";
    public const string MissingFirstName = "Error: First Name is required";
    public const string MissingLastName = "Error: Last Name is required";
    public const string MissingPostalCode = "Error: Postal Code is required";
}

public static class CustomerData
{
    public const string FirstName = "Devbrat";
    public const string LastName = "Verma";
    public const string PostalCode = "110001";
}
