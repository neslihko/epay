namespace CustomerAPI
{
    using CustomerAPI.DTO;

    using System;

    public static class Generator
    {
        static readonly string[] firstNames = new string[] { "Leia", "Sadie", "Jose", "Sara", "Frank", "Dewey", "Tomas", "Joel", "Lukas", "Carlos" };
        static readonly string[] lastNames = new string[] { "Liberty", "Ray", "Harrison", "Ronan", "Drew", "Powell", "Larsen", "Chan", "Anderson", "Lane" };
        static readonly Random shuffle = new();
        const int minAge = 10, maxAge = 90;

        public static Customer GetRandomCustomer(int id) => new()
        {
            Age = shuffle.Next(minAge, maxAge),
            Id = id,
            FirstName = firstNames[shuffle.Next(0, firstNames.Length - 1)],
            LastName = lastNames[shuffle.Next(0, lastNames.Length - 1)],
        };

        public static bool IsValidCustomer(Customer customer)
        {
            return customer != null &&
                !string.IsNullOrWhiteSpace(customer.FirstName) &&
                !string.IsNullOrWhiteSpace(customer.LastName) &&
                customer.Id >= 0;
        }
    }
}
