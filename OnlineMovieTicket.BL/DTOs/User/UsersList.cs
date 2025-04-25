namespace OnlineMovieTicket.BL.DTOs.User
{
    public class UsersList
    {
        public IEnumerable<UserDTO>? Users {get; set;}
        public int TotalCount {get; set;}
        public int FilterCount {get; set;}
    }
}