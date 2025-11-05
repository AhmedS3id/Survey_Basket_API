using Survey_Basket_API.Models;

namespace Survey_Basket_API.Services
{
    public  interface IPollServices
    {
        IEnumerable<Poll> GetAll();

        public Poll Get(int id);

        public Poll Add(Poll poll);
        public bool updated(int id ,Poll poll);

        public bool delete(int id);
    }
}
