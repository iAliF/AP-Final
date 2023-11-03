using System;
using System.Collections.Generic;
using System.Linq;

namespace FinalProjectNew
{
    public enum Gender
    {
        Male,
        Female
    }

    public class Customer
    {
        public readonly string FirstName;
        public readonly string LastName;
        public readonly int CustomerId;
        public readonly long NationalCode;
        public readonly Gender Gender;
        public readonly int BirthYear;
        public readonly string Province;
        public readonly string City;

        public string FullName => $"{FirstName} {LastName}";

        public Customer(string firstName, string lastName, int customerId, long nationalCode, Gender gender,
            int birthYear, string province, string city)
        {
            FirstName = firstName;
            LastName = lastName;
            CustomerId = customerId;
            NationalCode = nationalCode;
            Gender = gender;
            BirthYear = birthYear;
            Province = province;
            City = city;
        }

        public override string ToString()
        {
            return $@"Full Name: ${FullName}
ID: ${CustomerId}
National Code: {NationalCode}
Gender: {Gender}
Birth Year: {BirthYear}
Province: {Province}
City: {City}
";
        }
    }

    public class Dealer
    {
        public readonly string Name;
        public readonly int EstablishedYear;
        public readonly int Code;
        public readonly string OwnerFirstName;
        public readonly string OwnerLastName;
        public readonly string Province;
        public readonly string City;

        public string OwnerFullName => $"{OwnerFirstName} {OwnerLastName}";

        public Dealer(string name, int establishedYear, int code, string ownerFirstName, string ownerLastName,
            string province, string city)
        {
            Name = name;
            EstablishedYear = establishedYear;
            Code = code;
            OwnerFirstName = ownerFirstName;
            OwnerLastName = ownerLastName;
            Province = province;
            City = city;
        }

        public override string ToString()
        {
            return $@"Name: {Name}
Owner: {OwnerFullName}
Code: {Code}
Established Year: {EstablishedYear}
Province: {Province}
City: {City}
";
        }
    }

    public class Product
    {
        public readonly string Name;
        public readonly int Code;
        private double _price;
        public readonly string Brand;
        public readonly double Weight;

        public double Price
        {
            get => _price;
            set
            {
                if (value <= 0)
                    throw new Exception("Product price should be greater than zero");

                _price = value;
            }
        }

        public Product(string name, int code, double price, string brand, double weight)
        {
            Name = name;
            Code = code;
            Price = price;
            Brand = brand;
            Weight = weight;
        }

        public override string ToString()
        {
            return $@"Name: {Name}
Code: {Code}
Price: {Price}
Brand: {Brand}
Weight: {Weight}
";
        }
    }

    public class CustomerProduct
    {
        public readonly Customer Customer;
        public readonly Product Product;
        public readonly Dealer Dealer;
        private int _count;
        public readonly DateTime BuyTime;


        public int Count
        {
            get => _count;
            set
            {
                if (value <= 0)
                    throw new Exception("CustomerProduct count should be greater than zero");

                _count = value;
            }
        }

        public string CustomerFullName => Customer.FullName;
        public int CustomerId => Customer.CustomerId;
        public int ProductCode => Product.Code;
        public int DealerCode => Dealer.Code;

        public double Price => Product.Price;

        public double TotalPrice => _count * Price;

        public CustomerProduct(Customer customer, Product product, Dealer dealer, int count) : this(customer, product,
            dealer, count, DateTime.Now)
        {
        }

        public CustomerProduct(Customer customer, Product product, Dealer dealer, int count, DateTime buyTime)
        {
            Customer = customer;
            Product = product;
            Dealer = dealer;
            _count = count;
            BuyTime = buyTime;
        }

        public override string ToString()
        {
            return $@"Customer: {Customer.FullName}({CustomerId})
Product: {Product.Name}({ProductCode})
Dealer: {Dealer.Name}({DealerCode})
Count: {_count}
Buy Time: {BuyTime}
";
        }
    }

    public static class ShoppingSystem
    {
        private static readonly List<Customer> Customers = new List<Customer>();
        private static readonly List<Product> Products = new List<Product>();
        private static readonly List<Dealer> Dealers = new List<Dealer>();
        private static readonly List<CustomerProduct> CustomerProducts = new List<CustomerProduct>();

        private static void AddToList<TType>(TType obj, List<TType> list, int searchBy, Func<int, TType> func)
        {
            if (func(searchBy) != null)
                throw new Exception($"Duplicate entry with {searchBy} key");

            list.Add(obj);
        }

        public static void AddCustomer(Customer customer)
        {
            AddToList(customer, Customers, customer.CustomerId, GetCustomerById);
        }

        public static void RemoveCustomer(int id)
        {
            var query = from item in CustomerProducts
                where item.CustomerId == id
                select item;

            RemoveObject(id, Customers, GetCustomerById, query, CustomerProducts);
        }

        public static void AddProduct(Product product)
        {
            AddToList(product, Products, product.Code, GetProductByCode);
        }

        public static void RemoveProduct(int code)
        {
            var query = from item in CustomerProducts
                where item.ProductCode == code
                select item;

            RemoveObject(code, Products, GetProductByCode, query, CustomerProducts);
        }

        public static void AddDealer(Dealer dealer)
        {
            AddToList(dealer, Dealers, dealer.Code, GetDealerByCode);
        }

        public static void RemoveDealer(int code)
        {
            var query = from item in CustomerProducts
                where item.DealerCode == code
                select item;

            RemoveObject(code, Dealers, GetDealerByCode, query, CustomerProducts);
        }

        public static void AddCustomerProduct(CustomerProduct customerProduct)
        {
            if (GetCustomerProduct(customerProduct.Customer, customerProduct.Product, customerProduct.Dealer) != null)
                throw new Exception(
                    $"Duplicate entry with {customerProduct.CustomerId} / {customerProduct.ProductCode} / {customerProduct.DealerCode}");

            CustomerProducts.Add(customerProduct);
        }

        public static Customer GetCustomerById(int id)
        {
            return FirstOrNull(from customer in Customers
                where customer.CustomerId == id
                select customer) as Customer;
        }


        public static Customer GetCustomerByNationalCode(int code)
        {
            return FirstOrNull(from customer in Customers
                where customer.NationalCode == code
                select customer) as Customer;
        }


        public static Product GetProductByCode(int code)
        {
            return FirstOrNull(from product in Products
                where product.Code == code
                select product) as Product;
        }


        public static Dealer GetDealerByCode(int code)
        {
            return FirstOrNull(from dealer in Dealers
                where dealer.Code == code
                select dealer) as Dealer;
        }


        public static CustomerProduct GetCustomerProduct(Customer customer, Product product, Dealer dealer)
        {
            return FirstOrNull(from cp in CustomerProducts
                where cp.CustomerId == customer.CustomerId && cp.ProductCode == product.Code &&
                      cp.DealerCode == dealer.Code
                select cp) as CustomerProduct;
        }

        public static CustomerProduct GetCustomerProductById(int id, int pCode, int dCode)
        {
            return FirstOrNull(from cp in CustomerProducts
                where cp.CustomerId == id && cp.ProductCode == pCode && cp.DealerCode == dCode
                select cp) as CustomerProduct;
        }


        public static double TotalPurchaseOfCustomer(int id)
        {
            var obj = GetCustomerById(id);
            if (obj == null)
                return -1;

            return (from item in CustomerProducts
                where item.CustomerId == id
                select item.TotalPrice).Sum();
        }

        public static List<Product> GetCustomerPurchasedProducts(int id)
        {
            var obj = GetCustomerById(id);
            if (obj == null)
                return null;

            return (from item in CustomerProducts
                where item.CustomerId == id
                select item.Product).ToList();
        }

        public static List<Customer> GetProductCustomers(int code)
        {
            var obj = GetProductByCode(code);
            if (obj == null)
                return null;

            return (from item in CustomerProducts
                where item.ProductCode == code
                select item.Customer).ToList();
        }

        public static List<Product> GetDealerProducts(int code)
        {
            if (GetDealerByCode(code) == null)
                return null;

            var list = (from item in CustomerProducts
                where item.DealerCode == code
                select item.Product).ToList();

            var uList = new List<Product>();
            foreach (var item in list)
                if (!uList.Contains(item))
                    uList.Add(item);

            return uList;
        }


        public static int ProductTotalSales(int code)
        {
            if (GetProductByCode(code) == null)
                return -1;

            return (from item in CustomerProducts
                where item.ProductCode == code
                select item.Count).Sum();
        }

        public static Dictionary<Dealer, int> GetDealersAndTotalSales()
        {
            var dict = new Dictionary<Dealer, int>();
            foreach (var dealer in Dealers)
            {
                var total = (from item in CustomerProducts
                        where item.DealerCode == dealer.Code
                        select item.Count
                    ).Sum();
                dict[dealer] = total;
            }

            return dict;
        }

        public static int ProductTotalDealersCount(int code)
        {
            var f = from item in CustomerProducts
                where item.ProductCode == code
                select item.Dealer;

            return f.Distinct().Count();
        }


        private static void RemoveObject<TObject, TKey, TSObject>(TKey key, List<TObject> objList,
            Func<TKey, TObject> search, IEnumerable<TSObject> secondQuery, List<TSObject> secondList)
        {
            var obj = search(key);
            if (obj == null)
                return;

            objList.Remove(obj);

            foreach (var item in secondQuery) secondList.Remove(item);
        }


        private static object FirstOrNull(IEnumerable<object> enumerable)
        {
            var array = enumerable.ToArray();
            return array.Any() ? array.First() : null;
        }
    }

    internal static class Program
    {
        private static readonly Dictionary<string, Action> HelpDict = new Dictionary<string, Action>()
        {
            { "1: Add a Product", AddProduct },
            { "2: Remove a Product", RemoveProduct },
            { "3: Add a Customer", AddCustomer },
            { "4: Remove a Customer", RemoveCustomer },
            { "5: Add a Dealer", AddDealer },
            { "6: Remove a Dealer", RemoveDealer },
            { "7: Buy a Product by a Customer", BuyProduct },
            { "8: Calculate and display the total purchase price of a customer", TotalPurchase },
            { "9: Get Customers list of a specific Product", GetCustomersOfProduct },
            { "10: Get Products list of a specific Dealer", GetProductsOfDealer },
            { "11: Get number of sales of a Product", GetProductSalesNumber },
            { "12: Get list of Products purchased by a Customer", GetCustomerPurchasedProducts },
            { "13: Get list of Dealers and their total sales", GetDealersAndSales },
            { "14: Quit", null }
        };

        public static readonly string HelpMessage = string.Join("\n", HelpDict.Keys.ToList());

        private static readonly int HelpCount = HelpDict.Count;
        private static readonly int ExitOption = 14;


        public static void Main(string[] args)
        {
            int option;

            while (true)
            {
                var valid = GetOption(out option);
                if (!valid)
                {
                    Console.WriteLine("Invalid option");
                    continue;
                }

                if (option == ExitOption)
                {
                    Console.WriteLine("Exiting ...");
                    break;
                }

                var key = HelpDict.Keys.ToArray()[option - 1];
                var func = HelpDict[key];
                if (func != null)
                    func();

                Console.ReadKey();
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Please enter a number to execute the corresponding command");
            Console.WriteLine(HelpMessage);
        }


        private static bool GetOption(out int option)
        {
            ShowHelp();
            Console.Write("~ ");

            try
            {
                option = Convert.ToInt16(Console.ReadLine());
                if (option < 0 || option > HelpCount)
                    return false;
                return true;
            }
            catch (Exception)
            {
                option = -1;
                return false;
            }
        }

        private static TType GetField<TType>(string name, Func<string, TType> func)
        {
            Console.Write($"Enter {name}:");
            return func(Console.ReadLine());
        }

        private static Dictionary<string, string> GetFields(string[] fields)
        {
            var outFields = new Dictionary<string, string>();

            foreach (var name in fields)
            {
                Console.Write($"Enter {FirstLetterToUpper(name)}: ");
                outFields[name] = Console.ReadLine();
            }

            return outFields;
        }

        public static string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }


        private static void AddProduct()
        {
            var fields = GetFields(new[] { "name", "code", "price", "brand", "weight" });

            try
            {
                ShoppingSystem.AddProduct(new Product(
                    fields["name"],
                    Convert.ToInt32(fields["code"]),
                    Convert.ToDouble(fields["price"]),
                    fields["brand"],
                    Convert.ToDouble(fields["weight"])
                ));
                Console.WriteLine("Product has been successfully added");
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid entries");
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }

        private static void RemoveProduct()
        {
            var code = GetField("Product Code", Convert.ToInt32);
            ShoppingSystem.RemoveProduct(code);
            Console.WriteLine("Done");
        }

        private static void AddCustomer()
        {
            var fields = GetFields(new[]
                { "firstName", "lastName", "id", "nationalCode", "gender", "birthYear", "province", "city" });

            var gender = fields["gender"].ToLower() == "male" ? Gender.Male : Gender.Female;
            try
            {
                ShoppingSystem.AddCustomer(new Customer(
                    fields["firstName"],
                    fields["lastName"],
                    Convert.ToInt32(fields["id"]),
                    Convert.ToInt64(fields["nationalCode"]),
                    gender,
                    Convert.ToInt32(fields["birthYear"]),
                    fields["province"],
                    fields["city"]
                ));
                Console.WriteLine("Customer has been successfully added");
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid entries");
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }

        private static void RemoveCustomer()
        {
            var id = GetField("Customer's ID", Convert.ToInt32);
            ShoppingSystem.RemoveCustomer(id);
            Console.WriteLine("Done");
        }

        private static void AddDealer()
        {
            var fields = GetFields(new[]
                { "name", "establishedYear", "code", "ownerFirstName", "ownerLastName", "province", "city" });

            try
            {
                ShoppingSystem.AddDealer(new Dealer(
                    fields["name"],
                    Convert.ToInt32(fields["establishedYear"]),
                    Convert.ToInt32(fields["code"]),
                    fields["ownerFirstName"],
                    fields["ownerLastName"],
                    fields["province"],
                    fields["city"]
                ));
                Console.WriteLine("Dealer has been successfully added");
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid entries");
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }

        private static void RemoveDealer()
        {
            var code = GetField("Dealer's Code", Convert.ToInt32);
            ShoppingSystem.RemoveDealer(code);
            Console.WriteLine("Done");
        }

        private static void BuyProduct()
        {
            var fields = GetFields(new[] { "customerId", "productCode", "dealerCode", "count" });
            var customer = ShoppingSystem.GetCustomerById(Convert.ToInt32(fields["customerId"]));
            var product = ShoppingSystem.GetProductByCode(Convert.ToInt32(fields["productCode"]));
            var dealer = ShoppingSystem.GetDealerByCode(Convert.ToInt32(fields["dealerCode"]));

            try
            {
                if (customer == null || product == null || dealer == null)
                {
                    Console.WriteLine("Invalid entries");
                    return;
                }

                ShoppingSystem.AddCustomerProduct(new CustomerProduct(
                    customer,
                    product,
                    dealer,
                    Convert.ToInt32(fields["count"])
                ));
                Console.WriteLine("Product has been successfully bought");
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid entries");
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }

        private static void TotalPurchase()
        {
            var id = GetField("Customer's ID", Convert.ToInt32);
            var res = ShoppingSystem.TotalPurchaseOfCustomer(id);
            Console.WriteLine(res < 0 ? "Invalid ID" : $"Total => {res}");
        }

        private static void GetCustomersOfProduct()
        {
            var id = GetField("Product ID", Convert.ToInt32);
            var customers = ShoppingSystem.GetProductCustomers(id);

            if (customers == null)
            {
                Console.WriteLine("Invalid ID");
                return;
            }

            Console.WriteLine($"Product customers: ");
            foreach (var customer in customers) Console.WriteLine($"~ {customer.FullName}");
        }

        private static void GetProductsOfDealer()
        {
            var code = GetField("Dealer's Code", Convert.ToInt32);
            var prods = ShoppingSystem.GetDealerProducts(code);
            if (prods == null)
            {
                Console.WriteLine("Invalid ID");
                return;
            }

            Console.WriteLine($"Dealer products: ");
            foreach (var item in prods) Console.WriteLine($"> {item.Name}");
        }

        private static void GetProductSalesNumber()
        {
            var code = GetField("Product Code", Convert.ToInt32);
            var res = ShoppingSystem.ProductTotalSales(code);
            Console.WriteLine(res < 0 ? "Invalid Code" : $"Product sales number: {res}");
        }

        private static void GetCustomerPurchasedProducts()
        {
            var id = GetField("Customer's ID", Convert.ToInt32);
            var res = ShoppingSystem.GetCustomerPurchasedProducts(id);

            if (res == null)
            {
                Console.WriteLine("Invalid id");
                return;
            }

            Console.WriteLine("Customer's Products:");
            foreach (var product in res) Console.WriteLine($"> {product.Name} (ID: {product.Code})");
        }

        private static void GetDealersAndSales()
        {
            var res = ShoppingSystem.GetDealersAndTotalSales();

            Console.WriteLine("Dealers and Total Sales:");
            foreach (var item in res) Console.WriteLine($"> {item.Key.Name}: {item.Value}");
        }
    }
}