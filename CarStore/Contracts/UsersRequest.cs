namespace CarsStore.Contracts;
    public class UpdateUserRequest
    {
        public string UserName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
    }
    