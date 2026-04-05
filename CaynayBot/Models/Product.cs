namespace CaynayBot.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public ProductChCategory ProductChCategory { get; set; }
    }

    internal class Products
    {
        private static List<Product> _Products = new List<Product>();

        public static async Task<List<Product>> GetAll()
        {
            SetProducts();
            return _Products;
        }

        public static void Add(Product product) => _Products.Add(product);

        public static void SetProducts()
        {
            // Заполнение данными при создании экземпляра (или можно сделать статический конструктор)
            if (_Products.Count == 0)
            {
                // === ПИЦЦА ===
                Add(new Product { Name = "Пепперони", Price = 160, ProductChCategory = ProductChCategory.Pizza });
                Add(new Product { Name = "Курочка барбекю", Price = 160, ProductChCategory = ProductChCategory.Pizza });
                Add(new Product { Name = "Ветчина с грибами", Price = 160, ProductChCategory = ProductChCategory.Pizza });
                Add(new Product { Name = "Четыре сыра", Price = 160, ProductChCategory = ProductChCategory.Pizza });
                Add(new Product { Name = "Гавайская с ананасами", Price = 160, ProductChCategory = ProductChCategory.Pizza });
                Add(new Product { Name = "Цезарь с креветками", Price = 160, ProductChCategory = ProductChCategory.Pizza });
                Add(new Product { Name = "Мясное барбекю", Price = 160, ProductChCategory = ProductChCategory.Pizza });

                // === КОФЕ ===
                Add(new Product { Name = "Эспрессо (30 мл)", Price = 100, ProductChCategory = ProductChCategory.Coffee });
                Add(new Product { Name = "Капучино (250 мл)", Price = 150, ProductChCategory = ProductChCategory.Coffee });
                Add(new Product { Name = "Латте (малый)", Price = 150, ProductChCategory = ProductChCategory.Coffee });
                Add(new Product { Name = "Латте (большой)", Price = 250, ProductChCategory = ProductChCategory.Coffee });
                Add(new Product { Name = "Тёплое какао (250 мл)", Price = 150, ProductChCategory = ProductChCategory.Coffee });
                Add(new Product { Name = "Американо (250 мл)", Price = 120, ProductChCategory = ProductChCategory.Coffee });

                // === ЗАКУСКИ/СНЭКИ ===
                Add(new Product { Name = "Банановые чипсы", Price = 130, ProductChCategory = ProductChCategory.Snacks });
                Add(new Product { Name = "Миндаль", Price = 100, ProductChCategory = ProductChCategory.Snacks });
                Add(new Product { Name = "Чипсы Лейс (средние)", Price = 120, ProductChCategory = ProductChCategory.Snacks });
                Add(new Product { Name = "Чипсы Лейс (большие)", Price = 250, ProductChCategory = ProductChCategory.Snacks });
                Add(new Product { Name = "Чипсы Gramzz", Price = 80, ProductChCategory = ProductChCategory.Snacks });

                // === ФРУКТОВЫЙ ЧАЙ ===
                Add(new Product { Name = "Брусника-каркаде", Price = 150, ProductChCategory = ProductChCategory.FruitTea });
                Add(new Product { Name = "Смородина-малина (мята)", Price = 150, ProductChCategory = ProductChCategory.FruitTea });
                Add(new Product { Name = "Облепиховый с апельсином", Price = 160, ProductChCategory = ProductChCategory.FruitTea });
                Add(new Product { Name = "Манго-маракуйя", Price = 150, ProductChCategory = ProductChCategory.FruitTea });
                Add(new Product { Name = "Глинтвейн", Price = 170, ProductChCategory = ProductChCategory.FruitTea });
                Add(new Product { Name = "Имбирь-лимон", Price = 150, ProductChCategory = ProductChCategory.FruitTea });
                Add(new Product { Name = "Имбирь-апельсин", Price = 150, ProductChCategory = ProductChCategory.FruitTea });
                Add(new Product { Name = "Смородина-чабрец", Price = 150, ProductChCategory = ProductChCategory.FruitTea });
                Add(new Product { Name = "Клюква-апельсин", Price = 150, ProductChCategory = ProductChCategory.FruitTea });
                Add(new Product { Name = "Рябиновый", Price = 150, ProductChCategory = ProductChCategory.FruitTea });

                // === ЧАЙ (РАЗВЕСНОЙ/ШТУЧНЫЙ) ===
                // Для простоты создаю отдельные позиции с указанием минимальной цены/формата
                Add(new Product { Name = "Габа Мёд (1г)", Price = 40, ProductChCategory = ProductChCategory.LooseLeafTea });
                Add(new Product { Name = "Габа Мёд (от 100г, цена за 1г)", Price = 25, ProductChCategory = ProductChCategory.LooseLeafTea });
                Add(new Product { Name = "Пуэр Золотая Почка (1г)", Price = 30, ProductChCategory = ProductChCategory.LooseLeafTea });
                Add(new Product { Name = "Пуэр Золотая Почка (от 100г, цена за 1г)", Price = 20, ProductChCategory = ProductChCategory.LooseLeafTea });
                Add(new Product { Name = "Золотая Империя (1 шт.)", Price = 160, ProductChCategory = ProductChCategory.LooseLeafTea });
                Add(new Product { Name = "Золотая Империя (от 10 шт., цена за 1 шт.)", Price = 120, ProductChCategory = ProductChCategory.LooseLeafTea });
                Add(new Product { Name = "Пуэр Путь Чая (1г)", Price = 25, ProductChCategory = ProductChCategory.LooseLeafTea });
                Add(new Product { Name = "Пуэр Путь Чая (от 100г, цена за 1г)", Price = 15, ProductChCategory = ProductChCategory.LooseLeafTea });
                Add(new Product { Name = "Пуэр Чунь Ау Шу (1г)", Price = 25, ProductChCategory = ProductChCategory.LooseLeafTea });
                Add(new Product { Name = "Пуэр Чунь Ау Шу (от 100г, цена за 1г)", Price = 15, ProductChCategory = ProductChCategory.LooseLeafTea });
                Add(new Product { Name = "Саган (1г)", Price = 40, ProductChCategory = ProductChCategory.LooseLeafTea });
                Add(new Product { Name = "Саган (от 100г, цена за 1г)", Price = 25, ProductChCategory = ProductChCategory.LooseLeafTea });
                Add(new Product { Name = "ДХП (Да Хун Пао) (1г)", Price = 45, ProductChCategory = ProductChCategory.LooseLeafTea });
                Add(new Product { Name = "ДХП (Да Хун Пао) (от 100г, цена за 1г)", Price = 30, ProductChCategory = ProductChCategory.LooseLeafTea });
                Add(new Product { Name = "ДХП (элит) (1г)", Price = 70, ProductChCategory = ProductChCategory.LooseLeafTea });
                Add(new Product { Name = "ДХП (элит) (от 100г, цена за 1г)", Price = 50, ProductChCategory = ProductChCategory.LooseLeafTea });
                Add(new Product { Name = "Шен (1г)", Price = 25, ProductChCategory = ProductChCategory.LooseLeafTea });
                Add(new Product { Name = "Шен (от 100г, цена за 1г)", Price = 18, ProductChCategory = ProductChCategory.LooseLeafTea });
                Add(new Product { Name = "Тегуанинь (1г)", Price = 35, ProductChCategory = ProductChCategory.LooseLeafTea });
                Add(new Product { Name = "Тегуанинь (от 100г, цена за 1г)", Price = 22, ProductChCategory = ProductChCategory.LooseLeafTea });
                Add(new Product { Name = "Беловолосая Обезьяна (1г)", Price = 40, ProductChCategory = ProductChCategory.LooseLeafTea });
                Add(new Product { Name = "Беловолосая Обезьяна (от 100г, цена за 1г)", Price = 25, ProductChCategory = ProductChCategory.LooseLeafTea });

                // === ДЕСЕРТЫ ===
                Add(new Product { Name = "Наполеон", Price = 420, ProductChCategory = ProductChCategory.Desserts });
                Add(new Product { Name = "Медовик", Price = 450, ProductChCategory = ProductChCategory.Desserts });
                Add(new Product { Name = "Пирожное картошка", Price = 200, ProductChCategory = ProductChCategory.Desserts });
                Add(new Product { Name = "Орешки", Price = 60, ProductChCategory = ProductChCategory.Desserts });
                Add(new Product { Name = "Моти", Price = 160, ProductChCategory = ProductChCategory.Desserts });
                Add(new Product { Name = "Фруктовый салат", Price = 200, ProductChCategory = ProductChCategory.Desserts }); // цена не указана

                // === НАПИТКИ ===
                Add(new Product { Name = "Лимонад фруктовый (1л)", Price = 500, ProductChCategory = ProductChCategory.Drinks }); // цена не указана
                Add(new Product { Name = "Мохито (1л)", Price = 260, ProductChCategory = ProductChCategory.Drinks }); // цена не указана
                Add(new Product { Name = "Добрый ж/б", Price = 120, ProductChCategory = ProductChCategory.Drinks });
                Add(new Product { Name = "Добрый бут.", Price = 110, ProductChCategory = ProductChCategory.Drinks });
                Add(new Product { Name = "Сок (1л)", Price = 130, ProductChCategory = ProductChCategory.Drinks });
                Add(new Product { Name = "Адреналин", Price = 150, ProductChCategory = ProductChCategory.Drinks }); // цена не указана

                // === ПРОЧЕЕ ===
                Add(new Product { Name = "Плойка", Price = 250, ProductChCategory = ProductChCategory.Other });
                Add(new Product { Name = "Бутер сладкий", Price = 250, ProductChCategory = ProductChCategory.Other });
                Add(new Product { Name = "Бутер сосиска", Price = 250, ProductChCategory = ProductChCategory.Other });
            }
        }
    }

    public enum ProductChCategory : int
    {
        Pizza = 0,
        Coffee = 1,
        Snacks = 2,
        FruitTea = 3,
        LooseLeafTea = 4,
        Desserts = 5,
        Drinks = 6,
        Other = 7
    }
}