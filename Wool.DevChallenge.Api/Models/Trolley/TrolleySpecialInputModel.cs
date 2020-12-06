using System.Collections.Generic;

namespace Wool.DevChallenge.Api.Models.Trolley
{
    public class TrolleySpecialInputModel
    {
        public IEnumerable<TrolleyProductQuantityInputModel> Quantities { get; set; }
        public long Total { get; set; }
    }
}