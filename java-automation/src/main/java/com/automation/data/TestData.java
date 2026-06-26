package com.automation.data;

public class TestData {
    public static class Users {
        public static final String STANDARD_USERNAME = "standard_user";
        public static final String STANDARD_PASSWORD = "secret_sauce";
        public static final String LOCKED_USERNAME = "locked_out_user";
        public static final String LOCKED_PASSWORD = "secret_sauce";
        public static final String PERFORMANCE_GLITCH_USERNAME = "performance_glitch_user";
        public static final String PERFORMANCE_GLITCH_PASSWORD = "secret_sauce";
        public static final String INVALID_USERNAME = "invalid_user";
        public static final String INVALID_PASSWORD = "wrong_password";
    }

    public static class Urls {
        public static final String BASE = "https://www.saucedemo.com";
        public static final String INVENTORY = "https://www.saucedemo.com/inventory.html";
        public static final String CART = "https://www.saucedemo.com/cart.html";
        public static final String CHECKOUT_STEP_1 = "https://www.saucedemo.com/checkout-step-one.html";
        public static final String CHECKOUT_STEP_2 = "https://www.saucedemo.com/checkout-step-two.html";
        public static final String CHECKOUT_COMPLETE = "https://www.saucedemo.com/checkout-complete.html";
    }

    public static class ErrorMessages {
        public static final String LOCKED_OUT = "Epic sadface: Sorry, this user has been locked out.";
        public static final String INVALID_CREDENTIALS = "Epic sadface: Username and password do not match any user in this service";
        public static final String MISSING_USERNAME = "Epic sadface: Username is required";
        public static final String MISSING_PASSWORD = "Epic sadface: Password is required";
        public static final String MISSING_FIRST_NAME = "Error: First Name is required";
        public static final String MISSING_LAST_NAME = "Error: Last Name is required";
        public static final String MISSING_POSTAL_CODE = "Error: Postal Code is required";
    }

    public static class CustomerData {
        public static final String FIRST_NAME = "Devbrat";
        public static final String LAST_NAME = "Verma";
        public static final String POSTAL_CODE = "110001";
    }
}
