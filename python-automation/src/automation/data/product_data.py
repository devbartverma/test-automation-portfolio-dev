from dataclasses import dataclass


@dataclass(frozen=True)
class Product:
    """Immutable product model mirroring the JS-TS PRODUCTS data store."""
    data_test: str
    name: str
    price: float


class Products:
    BACKPACK = Product(
        data_test="add-to-cart-sauce-labs-backpack",
        name="Sauce Labs Backpack",
        price=29.99,
    )
    BIKE_LIGHT = Product(
        data_test="add-to-cart-sauce-labs-bike-light",
        name="Sauce Labs Bike Light",
        price=9.99,
    )
    BOLT_TSHIRT = Product(
        data_test="add-to-cart-sauce-labs-bolt-t-shirt",
        name="Sauce Labs Bolt T-Shirt",
        price=15.99,
    )
    FLEECE_JACKET = Product(
        data_test="add-to-cart-sauce-labs-fleece-jacket",
        name="Sauce Labs Fleece Jacket",
        price=49.99,
    )
    ONESIE = Product(
        data_test="add-to-cart-sauce-labs-onesie",
        name="Sauce Labs Onesie",
        price=7.99,
    )
    RED_TSHIRT = Product(
        data_test="add-to-cart-test.allthethings()-t-shirt-(red)",
        name="Test.allTheThings() T-Shirt (Red)",
        price=15.99,
    )

    ALL = [BACKPACK, BIKE_LIGHT, BOLT_TSHIRT, FLEECE_JACKET, ONESIE, RED_TSHIRT]


class SortOptions:
    NAME_AZ = "az"
    NAME_ZA = "za"
    PRICE_LO_HI = "lohi"
    PRICE_HI_LO = "hilo"
