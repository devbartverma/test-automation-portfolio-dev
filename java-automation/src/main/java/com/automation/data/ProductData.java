package com.automation.data;

import java.util.Arrays;
import java.util.Collections;
import java.util.List;

public class ProductData {

    /**
     * Product model — mirrors the JS PRODUCTS data store so all four language
     * suites share identical product definitions.
     */
    public static class Product {
        private final String dataTest;
        private final String name;
        private final double price;

        public Product(String dataTest, String name, double price) {
            this.dataTest = dataTest;
            this.name = name;
            this.price = price;
        }

        public String getDataTest() {
            return dataTest;
        }

        public String getName() {
            return name;
        }

        public double getPrice() {
            return price;
        }
    }

    public static class Products {
        public static final Product BACKPACK =
            new Product("add-to-cart-sauce-labs-backpack", "Sauce Labs Backpack", 29.99);
        public static final Product BIKE_LIGHT =
            new Product("add-to-cart-sauce-labs-bike-light", "Sauce Labs Bike Light", 9.99);
        public static final Product BOLT_TSHIRT =
            new Product("add-to-cart-sauce-labs-bolt-t-shirt", "Sauce Labs Bolt T-Shirt", 15.99);
        public static final Product FLEECE_JACKET =
            new Product("add-to-cart-sauce-labs-fleece-jacket", "Sauce Labs Fleece Jacket", 49.99);
        public static final Product ONESIE =
            new Product("add-to-cart-sauce-labs-onesie", "Sauce Labs Onesie", 7.99);
        public static final Product RED_TSHIRT =
            new Product("add-to-cart-test.allthethings()-t-shirt-(red)", "Test.allTheThings() T-Shirt (Red)", 15.99);

        public static final List<Product> ALL = Collections.unmodifiableList(
            Arrays.asList(BACKPACK, BIKE_LIGHT, BOLT_TSHIRT, FLEECE_JACKET, ONESIE, RED_TSHIRT));
    }

    public static class SortOptions {
        public static final String NAME_AZ = "az";
        public static final String NAME_ZA = "za";
        public static final String PRICE_LO_HI = "lohi";
        public static final String PRICE_HI_LO = "hilo";
    }
}
