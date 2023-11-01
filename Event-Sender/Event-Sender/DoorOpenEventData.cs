namespace Event_Sender
{
    internal class DoorOpenEventData
    {
        public int DoorId { get; }
        public string BuildingName { get; }
        public string CustomerName { get; }
        public string Email { get; }

        // Random number generator
        static Random rand = new Random();

        public DoorOpenEventData(string buildingName, string customerName, string email)
        {
            DoorId = rand.Next(100, 600);
            BuildingName = buildingName;
            CustomerName = customerName;
            Email = email;
        }

        // constructor with no parameters that generates random data
        public DoorOpenEventData()
        {
            DoorId = Random.Shared.Next(100, 600);
            BuildingName = GetRandomBuildingName();
            CustomerName = GetRandomCompanyName();
            Email = GetFakeEmail(CustomerName);
        }



        public override bool Equals(object? obj)
        {
            return obj is DoorOpenEventData other &&
                   DoorId == other.DoorId &&
                   BuildingName == other.BuildingName &&
                   CustomerName == other.CustomerName &&
                   Email == other.Email;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DoorId, BuildingName, CustomerName, Email);
        }

        #region Fake data

        // Function to generate a random building name
        static string GetRandomBuildingName()
        {
            // Possible parts of the building name
            string[] prefixes = { "Green", "Sky", "River", "Sun" };
            string[] middles = { "View", "Tower", "Pointe", "Heights" };
            string[] suffixes = { "Plaza", "Center", "Building", "Complex" };

            // Randomly select one part from each array
            string prefix = prefixes[rand.Next(prefixes.Length)];
            string middle = middles[rand.Next(middles.Length)];
            string suffix = suffixes[rand.Next(suffixes.Length)];

            // Combine them to form the final name
            return $"{prefix} {middle} {suffix}";
        }

        // Function to generate a random company name
        private static string GetRandomCompanyName()
        {
            // Possible parts of the company name
            string[] prefixes = { "Info", "Tech", "Net", "Global" };
            string[] middles = { "Solutions", "Systems", "Services", "Logistics" };
            string[] suffixes = { "Corp", "Inc", "LLC", "Ltd" };

            // Randomly select one part from each array
            string prefix = prefixes[rand.Next(prefixes.Length)];
            string middle = middles[rand.Next(middles.Length)];
            string suffix = suffixes[rand.Next(suffixes.Length)];

            // Combine them to form the final name
            return $"{prefix} {middle} {suffix}";
        }

        private static string GetFakeEmail(string companyName)
        {
            // Possible email usernames
            string[] usernames = { "john.doe", "jane.doe", "alex.jones", "emily.smith" };

            // Get a random username
            string username = usernames[rand.Next(usernames.Length)];

            // Sanitize the company name to be email-friendly
            string sanitizedCompanyName = companyName.Replace(" ", "").Replace(".", "").ToLower();

            // Create fake email address
            return $"{username}@{sanitizedCompanyName}.com";

            #endregion}
        }
    }
}
